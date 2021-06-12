using PortableDeviceApiLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileCopy {

	//
	// [ 用語 ]
	//
	// 1. WPD
	//    Windows Portable Device のこと。
	//    ウィンドウズに繋がれた外部デバイス。
	//    iPhone や Android などの端末。
	//    ドライブ名を付けられないので特殊なアクセスが必要
	//

	//
	// [ アーキテクチャ ]
	// MVC
	// M = FileCopy.Model.cs
	// V = FileCopy.cs[デザイン]
	// C = FileCopy.cs（このファイル）
	//


	//***********************************************************************************
	// フォームに配置したコントロールの動作に関するソースコード
	// ※ MVC における C の部分
	//***********************************************************************************
	public partial class FileCopy : Form {

		private CopyModel mModel = new CopyModel();

		private List<PortableDeviceObject> mWpdObjects = null;


		public FileCopy() {
			InitializeComponent();

			mModel.FlagCopySubDirectory = checkBoxCopySubDirectory.Checked;
			mModel.FlagCopyCreateTime = checkBoxCopyCreateTime.Checked;
			mModel.FlagCopyLastWrittenTime = checkBoxCopyLastWrittenTime.Checked;
			mModel.FlagCopyLastAccessTime = checkBoxCopyLastAccessTime.Checked;

		}





		//------------------------------------------------------------------------
		// アプリケーションが起動してフォームをロードした時にコールされる
		//------------------------------------------------------------------------
		private void FileCopy_Load(object sender,EventArgs e) {
			treeViewWpd_Load( treeViewWpdSrc );
			treeViewWpd_Load( treeViewWpdDst );
		}





		private void buttonCopy_Click(object sender,EventArgs e) {

			// 動作概要：コピーの実行
			CopyDirectories( true );
		}





		private void buttonCountFiles_Click(object sender,EventArgs e) {

			// 動作概要：コピーするファイル数を数える

			CopyDirectories( false );

		}


		private void textBoxSrc_TextChanged(object sender,EventArgs e) {

			// 動作概要：エディットテキストに直接パスを入力をされた時に限り
			//           ツリービューを探索して指定フォルダを選択する

			if ( textBoxSrc.Focused ) {
				treeView_SyncWithTextBox( treeViewWpdSrc, textBoxSrc );
			}
		}


		private void textBoxDst_TextChanged(object sender,EventArgs e) {

			// 動作概要：textBoxSrc_TextChanged 参照

			if ( textBoxDst.Focused ) {
				treeView_SyncWithTextBox( treeViewWpdDst, textBoxDst );
			}
		}



		private void checkBoxCopySubDirectory_CheckedChanged(object sender,EventArgs e) {
			mModel.FlagCopySubDirectory = checkBoxCopySubDirectory.Checked;
		}


		private void radioButtonSkip_CheckedChanged(object sender,EventArgs e) {
			mModel.Method = OverwriteMethod.Skip;
		}


		private void radioButtonOverwrite_CheckedChanged(object sender,EventArgs e) {
			mModel.Method = OverwriteMethod.Overwrite;
		}


		private void radioButtonUpdate_CheckedChanged(object sender,EventArgs e) {
			mModel.Method = OverwriteMethod.Update;
		}


		private void checkBoxCopyCreateTime_CheckedChanged(object sender,EventArgs e) {
			mModel.FlagCopyCreateTime = checkBoxCopyCreateTime.Checked;
		}

		private void checkBoxCopyLastWrittenTime_CheckedChanged(object sender,EventArgs e) {
			mModel.FlagCopyLastWrittenTime = checkBoxCopyLastWrittenTime.Checked;
		}

		private void checkBoxCopyLastAccessTime_CheckedChanged(object sender,EventArgs e) {
			mModel.FlagCopyLastAccessTime = checkBoxCopyLastAccessTime.Checked;
		}


		private void buttonSuspension_Click(object sender,EventArgs e) {
			mModel.Suspend = true;
			buttonSuspension.Enabled = false;
		}

		private void buttonRecoverDate_Click(object sender,EventArgs e) {

			// まだテスト中
			// Exif情報から作成日を復元する

			string srcFile = "C:\\Users\\Masato\\Desktop\\dst\\DSC_4593_044.JPG";
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap( srcFile );

			var items = (System.Drawing.Imaging.PropertyItem[])bmp.PropertyItems.Clone();
			bmp.Dispose();

			//Exif情報を列挙する
			foreach (System.Drawing.Imaging.PropertyItem item in items) {

				if ( item.Id == 0x9003 ) {
					string val = System.Text.Encoding.ASCII.GetString(item.Value);
					val = val.Trim(new char[] { '\0' });
					string[] datetime = val.Split( ' ' );
					string[] date = datetime[0].Split( ':' );
					string[] time = datetime[1].Split( ':' );

					DateTime CreateTime = new DateTime( int.Parse( date[0] ), int.Parse( date[1] ), int.Parse( date[2] ),
														int.Parse( time[0] ), int.Parse( time[1] ), int.Parse( time[2] ) );


					System.IO.FileInfo dstFile = new System.IO.FileInfo( srcFile );
					dstFile.CreationTime = CreateTime;

				}

//				if ( item.Id == 0x9004 ) {
//					string val = System.Text.Encoding.ASCII.GetString(item.Value);
//					val = val.Trim(new char[] { '\0' });
//					string[] datetime = val.Split( ' ' );
//					string[] date = datetime[0].Split( ':' );
//					string[] time = datetime[1].Split( ':' );
//
//					DateTime UpdateTime = new DateTime( int.Parse( date[0] ), int.Parse( date[1] ), int.Parse( date[2] ),
//														int.Parse( time[0] ), int.Parse( time[1] ), int.Parse( time[2] ) );
//
//					System.IO.FileInfo dstFile = new System.IO.FileInfo( srcFile );
//					dstFile.LastWriteTime = UpdateTime;
//				}


				//データの型を判断
				if (item.Type == 2)
				{	
					//ASCII文字の場合は、文字列に変換する
					string val = System.Text.Encoding.ASCII.GetString(item.Value);
					val = val.Trim(new char[] { '\0' });
					//表示する
					Console.WriteLine("{0:X}:{1}:{2}", item.Id, item.Type, val);
				}
				else
				{
					//表示する
					Console.WriteLine("{0:X}:{1}:{2}", item.Id, item.Type, item.Len);
				}
			}

		}


		void treeViewWpd_Load( TreeView treeView ) {

			treeView.Nodes.Clear();

			//-------------------------------------------
			// ドライブ
			//-------------------------------------------

			foreach ( string drive in Environment.GetLogicalDrives() ) {
				// 新規ノード作成
				// プラスボタンを表示するため空のノードを追加しておく
				TreeNode node = new TreeNode( drive );
				node.Nodes.Add( new TreeNode() );
				treeView.Nodes.Add( node );
			}


			//-------------------------------------------
			// ポータブルデバイス
			//-------------------------------------------

			WPDDevice[] devices = Wpd.SearchDevices();

			foreach ( var device in devices ) {
				if ( !device.DeviceName.Equals( "Non" ) ) {
					TreeNode node = new TreeNode( device.DeviceName );
					node.Tag = device;
					node.Nodes.Add( new TreeNode() );
					treeView.Nodes.Add( node );
				}
			}

		}


		private void treeViewWpdSrc_BeforeExpand(object sender,TreeViewCancelEventArgs e) {

			if ( treeViewWpdSrc.Focused ) {

				TreeNode node = e.Node;
				string path = node.FullPath;

				textBoxSrc.Text = path;

				AddFolderItemsToTreeView( node, path );
			}

		}





		private void treeViewWpdSrc_BeforeSelect(object sender,TreeViewCancelEventArgs e) {
			if ( treeViewWpdSrc.Focused ) {
				textBoxSrc.Text = e.Node.FullPath;

				AddFolderItemsToTreeView( e.Node, e.Node.FullPath, false );
			}
		}





		private void treeViewWpdDst_BeforeExpand(object sender,TreeViewCancelEventArgs e) {

			if ( treeViewWpdDst.Focused ) {

				TreeNode node = e.Node;
				string path = node.FullPath;

				textBoxDst.Text = path;

				AddFolderItemsToTreeView( node, path );
			}

		}





		private void treeViewWpdDst_BeforeSelect(object sender,TreeViewCancelEventArgs e) {
			if ( treeViewWpdDst.Focused ) {
				textBoxDst.Text = e.Node.FullPath;
			}
		}





		private void treeView_SyncWithTextBox( TreeView treeView, TextBox textBox ) {

			// textBox のパスを解析してツリービューの SelectedNode を設定する。

			string path = textBox.Text;
			path = path.Replace( '/', '\\' );
			string[] dirs = path.Split( '\\' );

			treeViewWpd_Load( treeView );

			int i = 0;
			dirs[0] += "\\";
			path = "";

			TreeNode node = null;

			var itr = treeView.Nodes.GetEnumerator();
			while ( itr.MoveNext() ) {
				node = (TreeNode)itr.Current;
				if ( dirs[i].Equals( node.Text ) ) {
					path += dirs[i] + "\\";
					AddFolderItemsToTreeView( node, path );

					if ( ++ i >= dirs.Length ) {
						break;
					}

					itr = node.Nodes.GetEnumerator();
				}
			}

			if ( node != null ) {
				treeView.SelectedNode = node;
				treeView.Focus();
			}

		}





		private void EnableControls() {
			textBoxSrc.Enabled = true;
			textBoxDst.Enabled = true;
			treeViewWpdSrc.Enabled = true;
			treeViewWpdDst.Enabled = true;
			buttonCopy.Enabled = true;
			buttonCountFiles.Enabled = true;
		}




		private void DisableControls() {
			textBoxSrc.Enabled = false;
			textBoxDst.Enabled = false;
			treeViewWpdSrc.Enabled = false;
			treeViewWpdDst.Enabled = false;
			buttonCopy.Enabled = false;
			buttonCountFiles.Enabled = false;
		}





		private WPDDevice GetDeviceFromTreeNode( TreeNode node ) {

			if ( node != null ) {
				TreeNode tmp = node;
				while ( tmp.Parent != null ) {
					tmp = tmp.Parent;
				}

				return (WPDDevice)tmp.Tag;
			}

			return null;
		}





		private void CopyDirectories( bool doCopy ) {

			//
			//  UIスレッドに関する処理は事前にRAMに格納して引数として渡す
			//

			WPDDevice srcDevice, dstDevice;
			string srcPath = textBoxSrc.Text;
			string dstPath = textBoxDst.Text;
			
			if ( !Wpd.IsWindowsDrive( srcPath ) ) {

				srcDevice = GetDeviceFromTreeNode( treeViewWpdSrc.SelectedNode );
				if ( srcDevice == null ) {
					srcPath = "";
				}

			} else {
				srcDevice = null;
				if ( !Directory.Exists( srcPath ) ) {
					srcPath = "";
				}
			}

			if ( !Wpd.IsWindowsDrive( dstPath ) ) {
				dstDevice = GetDeviceFromTreeNode( treeViewWpdDst.SelectedNode );
				if ( dstDevice == null ) {
					dstPath = "";
				}
			} else {
				dstDevice = null;
				if ( !Directory.Exists( dstPath ) ) {
					dstPath = "";
				}
			}

			if ( srcPath.Equals( "" ) || dstPath.Equals( "" ) ) {
				MessageBox.Show( "ファイルパスが不正です", "コピーエラー" );
				return;
			}


			//
			//  コピーは動作が重たくなるので非同期処理
			//
			Task.Run( ()=>{

				Invoke( new Action( DisableControls ) );

				( int countCopy, TimeSpan elapsed ) = CopyDirectoriesAsync( srcDevice, dstDevice, srcPath, dstPath, doCopy );
				if ( countCopy >= 0 ) {
					if ( doCopy ) {
						MessageBox.Show( "コピーしたファイル数：" + countCopy + "個\n" + "計測時間：" + elapsed.ToString(), "コピー完了" );
					} else {
						MessageBox.Show( "コピーするファイル数：" + countCopy + "個\n" + "計測時間：" + elapsed.ToString(), "検索完了" );
					}
				}

				Invoke( new Action( EnableControls ) );
			});
		}





		private (int, TimeSpan) CopyDirectoriesAsync( WPDDevice srcDevice, WPDDevice dstDevice, string srcPath, string dstPath, bool doCopy ) {

			if ( srcPath.Length <= 0 ) {

				MessageBox.Show( "コピー元ファイルを指定してください", "エラー", MessageBoxButtons.OK );
				return ( -1, TimeSpan.Zero );

			} else if ( dstPath.Length <= 0 ) {

				MessageBox.Show( "コピー先ファイルを指定してください", "エラー", MessageBoxButtons.OK );
				return ( -1, TimeSpan.Zero );

			}


			if ( srcPath.Equals( dstPath ) ) {
				MessageBox.Show( "コピー元とコピー先が同じです", "エラー", MessageBoxButtons.OK );
				return ( -1, TimeSpan.Zero );
			}


			if ( Wpd.IsWindowsDrive( srcPath ) ) {

				if ( Wpd.IsWindowsDrive(dstPath ) ) {
					// WinDrive to WinDrive
					return CopyWinFolderToWinFolder( srcPath, dstPath, doCopy );
				} else {
					// WinDrive to WPD
					MessageBox.Show( "現在、ポータブルデバイスに対するコピーはできません。", "エラー", MessageBoxButtons.OK );
				}

			} else {

				if ( Wpd.IsWindowsDrive( dstPath ) ) {

					// WPD to WinDrive
					return CopyWpdFolderToWinFolder( srcDevice, srcPath, dstPath, doCopy );

				} else {
					// WPD to WPD
					MessageBox.Show( "現在、ポータブルデバイスに対するコピーはできません。", "エラー", MessageBoxButtons.OK );
				}

			}

			return ( -1, TimeSpan.Zero );
		}





		List<PortableDeviceObject> GetWpdObjectsFromTreeNode( TreeNode node ) {

			List<PortableDeviceObject> wpdObjects = null;

			var baseObj = (WPDBaseObject)node.Tag;
			if ( baseObj.kind == ObjectKind.DEVICE ) {

				var device = (WPDDevice)baseObj;
				wpdObjects = Wpd.GetDeviceFiles( device, CopyAction.NameOnly );

			} else {

				var device = GetDeviceFromTreeNode( node );
				var obj = (PortableDeviceObject)baseObj;
				wpdObjects = Wpd.GetDeviceFiles( device, CopyAction.NameOnly, obj.Id );

			}

			return wpdObjects;
		}





		private void AddFolderItemsToTreeView( TreeNode node, string folderPath, bool open = true ) {

			if ( open ) {
				node.Nodes.Clear();
			}

			try {
				var dirList = new DirectoryInfo( folderPath );
				foreach( var di in dirList.GetDirectories() ) {

					if ( di.Attributes.HasFlag( System.IO.FileAttributes.Hidden ) ) {

					} else {
						if ( open ) {
							var child = new TreeNode( di.Name );
							child.Nodes.Add( new TreeNode() );
							node.Nodes.Add( child );
						}
					}
				}

			} catch ( DirectoryNotFoundException e ) {

				mWpdObjects = GetWpdObjectsFromTreeNode( node );

				if ( open ) {
					foreach ( var obj in mWpdObjects ) {
						var child = new TreeNode( obj.Name );
						child.Tag = obj;
						child.Nodes.Add( new TreeNode() );
						node.Nodes.Add( child );
					}
				}

			}

		}





		void SetEnableButtonSuspension( bool enable ) {
			buttonSuspension.Enabled = enable;
			mModel.Suspend = false;
		}





		private (int, TimeSpan) CopyWinFolderToWinFolder( string srcPath, string dstPath, bool doCopy ) {

			srcPath = mModel.RegulatePath( srcPath );
			dstPath = mModel.RegulatePath( dstPath );

			string curDir = System.Environment.CurrentDirectory;
			System.Environment.CurrentDirectory = srcPath;

			CopyType type = mModel.PathCheck( srcPath, dstPath );
			if ( type == CopyType.BothAreDirectories ) {

				// 存在しないファイルやフォルダを指定した場合、PathCheck はディレクトリ扱いとなる。

				bool isExistSrcDir = Directory.Exists( srcPath );
				bool isExistDstDir = Directory.Exists( dstPath );

				if ( isExistSrcDir ) {

					bool isCopyOK = true;
					bool doCreateDirectory = false;

					if ( !isExistDstDir && doCopy ) {

						string[] ret = mModel.CheckExistDirectory( dstPath );
						if ( ret[0] == "" ) {

							MessageBox.Show( "ディレクトリが存在しません", "エラー", MessageBoxButtons.OK );
							isCopyOK = false;

						} else {

							string msg = "存在するディレクトリ：" + ret[0] + "\n" +
										 "作成するディレクトリ：" + ret[1] + "\n\n" +
										 "よろしいですか？";
							DialogResult result = MessageBox.Show( msg, "ディレクトリ作成の確認", MessageBoxButtons.OKCancel );
							if ( result == DialogResult.OK ) {
								doCreateDirectory = true;
							} else {
								isCopyOK = false;
							}

						}
					}

					if ( isCopyOK ) {

						string msg = "コピー元：" + mModel.GetFullPath( srcPath ) + "\n" +
									 "コピー先：" + mModel.GetFullPath( dstPath ) + "\n\n";
						if ( doCopy ) {
							msg += "コピーします。\nよろしいですか？";
						} else {
							msg += "コピーするファイル数を確認します。\nよろしいですか？";
						}

						DialogResult result = MessageBox.Show( msg, "最終確認", MessageBoxButtons.OKCancel );
						if ( result == DialogResult.OK ) {

							Invoke( new Action<bool>( SetEnableButtonSuspension ), true );

							System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
							sw.Start();

							if ( doCopy && doCreateDirectory ) {
								mModel.CreateDirectories( dstPath );
							}

							int countCopy = 0;
							if ( doCopy ) {
								mModel.CopyDirectory( srcPath, dstPath, OnCopyFile, ref countCopy, true );
							} else {
								mModel.CopyDirectory( srcPath, dstPath, OnCountFile, ref countCopy, false );
							}

							sw.Stop();
							Debug.Print(sw.Elapsed.ToString());

							Invoke( new Action<bool>( SetEnableButtonSuspension ), false );

							System.Environment.CurrentDirectory = curDir;
							return ( countCopy, sw.Elapsed );
						}
					}

				} else {

					MessageBox.Show( "コピー元ディレクトリが存在しません", "エラー", MessageBoxButtons.OK );

				}

			} else if ( type == CopyType.SrcIsFile ) {

				MessageBox.Show( "コピー先フォルダにファイルを指定することはできません。", "エラー" );

			} else if ( type == CopyType.BothAreFiles ) {

				MessageBox.Show( "特定のファイルの上書きはできません。", "エラー" );

			}

			System.Environment.CurrentDirectory = curDir;

			return ( -1, TimeSpan.Zero );
		}



		private (int, TimeSpan) CopyWpdFolderToWinFolder( WPDDevice device, string srcPath, string dstPath, bool doCopy ) {


//			var device = (WPDDevice)Invoke( new Func<TreeNode,WPDDevice>( GetDeviceFromTreeNode ), treeViewWpdSrc.SelectedNode );
//			var device = GetDeviceFromTreeNode( treeViewWpdSrc.SelectedNode );

			bool isExistDstDir = Directory.Exists( dstPath );

//			string srcPath = treeViewWpdSrc.SelectedNode.FullPath;

			if ( mWpdObjects.Count != 0 ) {

				bool isCopyOK = true;
				bool doCreateDirectory = false;

				if ( !isExistDstDir && doCopy ) {

					string[] ret = mModel.CheckExistDirectory( dstPath );
					if ( ret[0] == "" ) {

						MessageBox.Show( "ディレクトリが存在しません", "エラー", MessageBoxButtons.OK );
						isCopyOK = false;

					} else {

						string msg = "存在するディレクトリ：" + ret[0] + "\n" +
									 "作成するディレクトリ：" + ret[1] + "\n\n" +
									 "よろしいですか？";
						DialogResult result = MessageBox.Show( msg, "ディレクトリ作成の確認", MessageBoxButtons.OKCancel );
						if ( result == DialogResult.OK ) {
							doCreateDirectory = true;
						} else {
							isCopyOK = false;
						}

					}
				}

				if ( isCopyOK ) {

					string msg = "コピー元：" + srcPath + "\n" +
								 "コピー先：" + mModel.GetFullPath( dstPath ) + "\n\n";
					if ( doCopy ) {
						msg += "コピーします。\nよろしいですか？";
					} else {
						msg += "コピーするファイル数を確認します。\nよろしいですか？";
					}

					DialogResult result = MessageBox.Show( msg, "最終確認", MessageBoxButtons.OKCancel );
					if ( result == DialogResult.OK ) {

						Invoke( new Action<bool>( SetEnableButtonSuspension ), true );

						System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
						sw.Start();

						if ( doCopy && doCreateDirectory ) {
							mModel.CreateDirectories( dstPath );
						}

						// 数を数える
						int countCopy;
						if ( doCopy ) {
							countCopy = mModel.CopyDirectoryWpdToFolder( device, mWpdObjects, dstPath, OnCopyFile, true );
						} else {
							countCopy = mModel.CopyDirectoryWpdToFolder( device, mWpdObjects, dstPath, OnCountFile, false );
						}

						sw.Stop();
						Debug.Print(sw.Elapsed.ToString());

						Invoke( new Action<bool>( SetEnableButtonSuspension ), false );

						return ( countCopy, sw.Elapsed );
					}
				}

			} else {

				MessageBox.Show( "コピーするファイルが存在しません", "エラー", MessageBoxButtons.OK );

			}

			return ( -1, TimeSpan.Zero );
		}


		public void UpdateCopyInfo( int nowCount, int maxCount, string text ) {

			void onUpdate() {
				labelDownload.Text = string.Format( "No.{0} : {1}", nowCount, text );
			}

			Invoke( new Action( onUpdate ) );
		}



		public void OnCopyFile( int index, System.IO.FileInfo srcFile, System.IO.FileInfo dstFile ) {
			UpdateCopyInfo( index, progressBarDownload.Maximum, dstFile.FullName );
		}



		public void UpdateCountInfo( int nowCount, string text ) {

			void onUpdate() {
				labelDownload.Text = string.Format( "No.{0} : {1}", nowCount, text );
			}

			Invoke( new Action( onUpdate ) );
		}


		public void OnCountFile( int index, System.IO.FileInfo srcFile, System.IO.FileInfo dstFile ) {
			UpdateCountInfo( index, dstFile.FullName );
		}
	}
}
