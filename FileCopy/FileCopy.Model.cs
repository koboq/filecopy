using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace FileCopy {
	partial class FileCopy {

		private enum CopyType {
			Non,
			SrcIsFile,
			DstIsFile,
			BothAreFiles,
			BothAreDirectories,
		};

		private enum OverwriteMethod {
			Skip,
			Overwrite,
			Update,
		};


		private class CopyModel {

			public OverwriteMethod Method { get; set; }
			public bool FlagCopyCreateTime { get; set; }
			public bool FlagCopyLastWrittenTime { get; set; }
			public bool FlagCopyLastAccessTime { get; set; }

			public bool FlagCopySubDirectory { get; set; }

			public delegate void CopyCallback( int index, System.IO.FileInfo srcFile, System.IO.FileInfo dstFile );

			public bool Suspend { get; set; }


			public string RegulatePath( string path ) {

				if ( path.Length <= 0 ) {
					return "";
				}

				string pathRegulated = path.Trim().Replace( '/', '\\' );
				while ( pathRegulated[ pathRegulated.Length - 1 ] == '\\' ) {
					pathRegulated = pathRegulated.Remove( pathRegulated.Length - 1 );
				}

				return pathRegulated;
			}


			public string GetFullPath( string path ) {

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( path );

				return dir.FullName;
			}


			public CopyType PathCheck( string srcPath, string dstPath ) {

				System.IO.FileInfo srcFile = new System.IO.FileInfo( srcPath );
				System.IO.FileInfo dstFile = new System.IO.FileInfo( dstPath );

				return PathCheck( srcFile, dstFile );
			}


			public CopyType PathCheck( System.IO.FileInfo srcFile, System.IO.FileInfo dstFile ) {

				// 存在しないファイルの場合 Attribute は -1 となる。
				// つまりディレクトリ扱い

				bool isDirSrc = srcFile.Attributes.HasFlag( FileAttributes.Directory );
				bool isDirDst = dstFile.Attributes.HasFlag( FileAttributes.Directory );

				if ( !isDirSrc && !isDirDst ) {
					return CopyType.BothAreFiles;
				}

				if ( !isDirSrc && isDirDst ) {
					return CopyType.SrcIsFile;
				}

				if ( isDirSrc && !isDirDst ) {
					return CopyType.DstIsFile;
				}

				return CopyType.BothAreDirectories;
			}



			public CopyType CopyFileWithCheck( System.IO.FileInfo srcFile, System.IO.FileInfo dstFile ) {

				CopyType type = PathCheck( srcFile, dstFile );
				if ( type == CopyType.BothAreFiles || type == CopyType.SrcIsFile ) {
					CopyFile( srcFile, dstFile );
				}

				return type;
			}


			public void CreateDirectories( System.IO.DirectoryInfo dir ) {

				string[] folders = dir.FullName.Split( '\\' );
				string path = folders[0] + '\\';
				if ( !Directory.Exists( path ) ) {
					// 存在しないドライブ
				} else {

					for ( int i = 1; i < folders.Length; i ++ ) {
						path += folders[i] + '\\';
						if ( !Directory.Exists( path ) ) {
							Directory.CreateDirectory( path );
						}
					}

				}
			}


			public void CreateDirectories( string filePath ) {
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( filePath );
				CreateDirectories( dir );
			}


			public string[] CheckExistDirectory( System.IO.DirectoryInfo dir ) {

				string pathExist = "", pathNotExist = "";

				string[] folders = dir.FullName.Split( '\\' );

				int i;
				for ( i = 0; i < folders.Length; i ++ ) {
					if ( !Directory.Exists( pathExist + folders[i] ) ) {
						break;
					}
					pathExist += folders[i] + '\\';
				}

				for ( ; i < folders.Length; i ++ ) {
					pathNotExist += folders[i] + "\\";
				}

				string[] ret = new string[2];
				ret[0] = pathExist;
				ret[1] = pathNotExist;

				return ret;
			}


			public string[] CheckExistDirectory( string filePath ) {
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( filePath );
				return CheckExistDirectory( dir );
			}


			public void CopyFile( string srcPath, string dstPath ) {

				System.IO.FileInfo srcFile = new System.IO.FileInfo( srcPath );
				System.IO.FileInfo dstFile = new System.IO.FileInfo( dstPath );

				CopyFile( srcFile, dstFile );
			}


			public void CopyFile( System.IO.FileInfo srcFile, System.IO.FileInfo dstFile ) {

				System.IO.File.Copy( srcFile.FullName, dstFile.FullName, true );
				dstFile = new System.IO.FileInfo( dstFile.FullName );

				if ( FlagCopyCreateTime ) {
					dstFile.CreationTime = srcFile.CreationTime;
					dstFile.CreationTimeUtc = srcFile.CreationTimeUtc;
				}

				if ( FlagCopyLastAccessTime ) {
					dstFile.LastAccessTime = srcFile.LastAccessTime;
					dstFile.LastAccessTimeUtc = srcFile.LastAccessTimeUtc;
				}

				if ( FlagCopyLastWrittenTime ) {
					dstFile.LastWriteTime = srcFile.LastWriteTime;
					dstFile.LastWriteTimeUtc = srcFile.LastWriteTimeUtc;
				} else {
					dstFile.LastWriteTime = DateTime.Now;
					dstFile.LastWriteTimeUtc = DateTime.UtcNow;
				}
			}


			private void CopyFile( System.IO.FileInfo srcFile, System.IO.FileInfo dstFile, bool doCopy ) {

				if ( doCopy ) {
					CopyFile( srcFile, dstFile );
				}

			}


			public CopyType CopyFileToDirectory( string srcPath, string dstPath, bool check = false ) {

				if ( check ) {
					CopyType type = PathCheck( srcPath, dstPath );
					if ( type != CopyType.SrcIsFile ) {
						return type;
					}
				}

				System.IO.FileInfo srcFile = new System.IO.FileInfo( srcPath );

				string dst = dstPath + "\\" + srcFile.Name;
				System.IO.FileInfo dstFile = new System.IO.FileInfo( dst );

				return CopyFileWithCheck( srcFile, dstFile );
			}


			public void CopyDirectory( string srcPath, string dstPath, CopyCallback callback, ref int countCopy, bool doCopy = true ) {

				if ( Suspend == true ) {
					return;
				}

				if ( !Directory.Exists( dstPath ) ) {
					if ( doCopy ) {
						Directory.CreateDirectory( dstPath );
					}
				}

				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo( srcPath );
				System.IO.DirectoryInfo[] srcDirs = dir.GetDirectories( "*", System.IO.SearchOption.TopDirectoryOnly );

				if ( FlagCopySubDirectory ) {
					foreach ( var srcDir in srcDirs ) {

						if ( Suspend == true ) {
							break;
						}

						CopyDirectory( srcPath + "\\" + srcDir.Name, dstPath + "\\" + srcDir.Name, callback, ref countCopy, doCopy );
					}
				}

				System.IO.FileInfo[] srcFiles = dir.GetFiles( "*", System.IO.SearchOption.TopDirectoryOnly );
				foreach( var srcFile in srcFiles ) {

					if ( Suspend == true ) {
						break;
					}

					string dst = dstPath + "\\" + srcFile.Name;
					System.IO.FileInfo dstFile = new System.IO.FileInfo( dst );

					if ( !dstFile.Exists ) {

						CopyFile( srcFile, dstFile, doCopy );
						countCopy ++;
						if ( callback != null ) {
							callback( countCopy, srcFile, dstFile );
						}

					} else {

						switch ( Method ) {

						case OverwriteMethod.Overwrite:

							CopyFile( srcFile, dstFile, doCopy );
							countCopy ++;
							if ( callback != null ) {
								callback( countCopy, srcFile, dstFile );
							}

							break;

						case OverwriteMethod.Update:

							if ( dstFile.LastWriteTime.Ticks < srcFile.LastWriteTime.Ticks ) {
								CopyFile( srcFile, dstFile, doCopy );
								countCopy ++;
								if ( callback != null ) {
									callback( countCopy, srcFile, dstFile );
								}
							}

							break;

//						case OverwriteMethod.NewName:
//
//							if ( dstFile.Exists ) {
//
//								string name = dstFile.Name;
//								int count = name.LastIndexOf( '.' );
//								name = name.Substring( 0, count );
//
//								int number = 0;
//								int pos1 = name.LastIndexOf( '(' );
//								int pos2 = name.LastIndexOf( ')' );
//								if ( pos1 >= 0 && pos2 >= 0 ) {
//									string strNumber = name.Substring( pos1 + 1, pos2 - pos1 - 1 );
//
//									bool isDigit = strNumber.All(Char.IsDigit);
//									if ( isDigit ) {
//										int tmp = int.Parse( strNumber );
//										number ++;
//									}
//
//									name = name.Substring( 0, pos1 - 1 );
//								}
//
//								name = name + "(" + number + ")";
//
//								dstFile = new System.IO.FileInfo( dstFile.DirectoryName + "\\" + name + dstFile.Extension ); 
//							}
//
//							break;

						}
					}
				}
			}





			public void CopyWpdFile( WPDDevice device, string id, System.IO.FileInfo dstFile ) {

				PortableDeviceObject obj = Wpd.Download( device, id, dstFile.FullName );

				dstFile = new System.IO.FileInfo( dstFile.FullName );

				if ( FlagCopyCreateTime ) {
					if ( !obj.Date.Created.Equals( DateTime.MinValue ) ) {
						dstFile.CreationTime = obj.Date.Created;
					} else if ( !obj.Date.Modified.Equals( DateTime.MinValue ) ) {
						dstFile.CreationTime = obj.Date.Modified;
					}
//					dstFile.CreationTimeUtc = srcFile.CreationTimeUtc;
				}

				if ( FlagCopyLastAccessTime ) {
					// Wpdでは使用しない
//					dstFile.LastAccessTime = srcFile.LastAccessTime;
//					dstFile.LastAccessTimeUtc = srcFile.LastAccessTimeUtc;
				}

				if ( FlagCopyLastWrittenTime && !obj.Date.Modified.Equals( DateTime.MinValue ) ) {
					dstFile.LastWriteTime = obj.Date.Modified;
//					dstFile.LastWriteTimeUtc = srcFile.LastWriteTimeUtc;
				} else {
					dstFile.LastWriteTime = DateTime.Now;
//					dstFile.LastWriteTimeUtc = DateTime.UtcNow;
				}
			}





			private void CopyWpdFile( WPDDevice device, PortableDeviceObject obj, System.IO.FileInfo dstFile, bool doCopy ) {

				if ( doCopy ) {
					CopyWpdFile( device, obj.Id, dstFile );
				}

			}





			public int CopyDirectoryWpdToFolder( WPDDevice device, List<PortableDeviceObject> wpdObjects, string dstPath, CopyCallback callback, bool doCopy = true ) {

				int countCopy = 0;

				void Copy( List<PortableDeviceObject> objs, string folderPath ) {

					if ( !Directory.Exists( dstPath ) ) {
						if ( doCopy ) {
							Directory.CreateDirectory( dstPath );
						}
					}

					foreach ( var obj in objs ) {

						if ( obj.kind == ObjectKind.FILE ) {

							string dst = folderPath + "\\" + obj.Name;
							System.IO.FileInfo dstFile = new System.IO.FileInfo( dst );

							if ( !dstFile.Exists ) {

								CopyWpdFile( device, obj, dstFile, doCopy );
								countCopy ++;
								if ( callback != null ) {
									callback( countCopy, null, dstFile );
								}

							} else {

								switch ( Method ) {

								case OverwriteMethod.Overwrite:

									CopyWpdFile( device, obj, dstFile, doCopy );
									countCopy ++;
									if ( callback != null ) {
										callback( countCopy, null, dstFile );
									}

									break;

								case OverwriteMethod.Update:

									if ( dstFile.LastWriteTime.Ticks < obj.Date.Modified.Ticks ) {
										CopyWpdFile( device, obj, dstFile, doCopy );
										countCopy ++;
										if ( callback != null ) {
											callback( countCopy, null, dstFile );
										}
									}

									break;
								}

							}

						} else if ( obj.kind == ObjectKind.FOLDER ) {

							if ( FlagCopySubDirectory ) {

								CopyAction action = (doCopy) ? CopyAction.All : CopyAction.NameOnly;

								var folderObjects = Wpd.GetDeviceFiles( device, action, obj.Id );
								Copy( folderObjects, folderPath + "\\" + obj.Name );
							}

						}
					}
				};


				Copy( wpdObjects, dstPath );

				return countCopy;
			}





		}
	}
}
