using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PortableDeviceApiLib;

namespace FileCopy {

	//***********************************************************************************
	//
	// ウィンドウズポータブルデバイスを列挙するクラス
	//
	// ※ ウィンドウズポータブルデバイスとは？
	//    USBで接続した iPhone、Andorid、外付けHDDなど
	//    一部のデバイスはドライブとして認識されるが、認識されないものもある。
	//    ドライブとして認識されなくても、ファイルを列挙することができる。
	//
	//***********************************************************************************

	enum CopyAction {
		Non,
		NameOnly,
		All,
	};


	class Wpd {

		public const uint WPD_OBJECT_ID = 2;
		public const uint WPD_OBJECT_PARENT_ID = 3;
		public const uint WPD_OBJECT_NAME = 4;
		public const uint WPD_OBJECT_CONTENT_TYPE = 7;
		public const uint WPD_OBJECT_SIZE = 11;
		public const uint WPD_OBJECT_ORIGINAL_FILE_NAME = 12;
		public const uint WPD_OBJECT_DATE_CREATED = 18;
		public const uint WPD_OBJECT_DATE_MODIFIED = 19;
		public const uint WPD_OBJECT_DATE_AUTHORED = 20;

		public const uint WPD_FUNCTIONAL_OBJECT_CATEGORY = 2;
		public const uint WPD_CLIENT_NAME = 2;
		public const uint WPD_CLIENT_MAJOR_VERSION = 3;
		public const uint WPD_CLIENT_MINOR_VERSION = 4;
		public const uint WPD_CLIENT_REVISION = 5;
		public const uint WPD_RESOURCE_DEFAULT = 0;
		public const uint WPD_STORAGE_FREE_SPACE_IN_BYTES = 5;

		public Wpd() {

		}





		//------------------------------------------------------------------------
		// 先頭3文字が c:\ など、アルファベットの１文字＋「:\」なら
		// ウィンドウズドライブと判定する。
		//------------------------------------------------------------------------
		public static bool IsWindowsDrive( string path ) {

			if ( path.Length >= 3 ) {
				if ( path[1] == ':' && path[2] == '\\' ) {
					return true;
				}
			}

			return false;
		}





		//------------------------------------------------------------------------
		// 接続されているウィンドウズポータブルデバイスを認識する。
		// 戻り値は認識したデバイス一覧
		// ※ デバイスが保持するフォルダの認識ではないので注意！
		//------------------------------------------------------------------------
		public static WPDDevice[] SearchDevices() {

			PortableDeviceManager deviceManager = new PortableDeviceManager();

			deviceManager.RefreshDeviceList();
			uint count = 0;
			deviceManager.GetDevices( null, ref count );

			string[] devicesIDs = new string[count];
			WPDDevice[] devices = new WPDDevice[count];

			deviceManager.GetDevices( devicesIDs, ref count );
			for (int i = 0; i < count; i++) {
				devices[i] = new WPDDevice();
				devices[i].DeviceID = devicesIDs[i];
				devices[i].kind = ObjectKind.DEVICE;
			  }

			IPortableDeviceValues clientInfo = (IPortableDeviceValues)new PortableDeviceTypesLib.PortableDeviceValuesClass();

			for (int i = 0; i < count; i++) {
				devices[i].DeviceClass = new PortableDeviceClass();
				devices[i].DeviceClass.Open( devices[i].DeviceID, clientInfo );

				IPortableDeviceContent content;
				devices[i].DeviceClass.Content(out content);

				IPortableDeviceProperties properties;
				content.Properties(out properties);

				IPortableDeviceValues propertyValues;
				properties.GetValues("DEVICE", null, out propertyValues);

				//wpdDevice[i].DeviceClass.Close();

				string name;
				_tagpropertykey property = new _tagpropertykey();
				property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);
				property.pid = 4;
				try {
					propertyValues.GetStringValue(property, out name);
					devices[i].DeviceName = name;
				} catch (System.Runtime.InteropServices.COMException) {
					devices[i].DeviceName = "Non";
				}
			}

			deviceManager = null;

			return devices;
		}





		//------------------------------------------------------------------------
		// デバイスが保持するファイルやフォルダを列挙する
		//------------------------------------------------------------------------
		public static List<PortableDeviceObject> GetDeviceFiles( WPDDevice device, CopyAction action, string folderID = "DEVICE" ) {

			// folderID = "DEVICE"
			// → デバイスにある最上位フォルダを検索

			IPortableDeviceContent content;
			device.DeviceClass.Content(out content);

			IPortableDeviceProperties properties;
			content.Properties(out properties);

			IEnumPortableDeviceObjectIDs objectIDs;
			string FolderID = folderID;
			content.EnumObjects(0, FolderID, null, out objectIDs);

			List<PortableDeviceObject> objs = new List<PortableDeviceObject>();

			string objectID;
			uint fetched = 0;

			while (true) {
				objectIDs.Next(1, out objectID, ref fetched);
				if (fetched <= 0) break;

				PortableDeviceObject currentObject = WrapObject(properties, objectID, action);

				switch (currentObject.kind) {
					case ObjectKind.FILE:
						objs.Add( currentObject );
						break;
					case ObjectKind.FOLDER:
						objs.Add( currentObject );
						break;
				}
			}

			return objs;
		}





		//------------------------------------------------------------------------
		// プロパティからファイルまたはフォルダを判定。
		// 加えて、名前や作成日などの情報を取得する。
		//------------------------------------------------------------------------
		public static PortableDeviceObject WrapObject(IPortableDeviceProperties properties, string objectID, CopyAction action) {

			const string err_name = "(no name)";

			IPortableDeviceKeyCollection keys;
			properties.GetSupportedProperties(objectID, out keys);

			IPortableDeviceValues values;
			properties.GetValues(objectID, keys, out values);


			_tagpropertykey property = new _tagpropertykey();
			property.fmtid = new Guid(0xEF6B490D, 0x5CD8, 0x437A, 0xAF, 0xFC, 0xDA, 0x8B, 0x60, 0xEE, 0x4A, 0x3C);


			string name = "", originalName = "";
			FileDate date = new FileDate();

			if ( action == CopyAction.NameOnly || action == CopyAction.All ) {

				// name の取得
				property.pid = WPD_OBJECT_NAME;
				try {
					values.GetStringValue(property, out name);
				} catch (System.Runtime.InteropServices.COMException e) {
					name = err_name;
				}

				// original name の取得
				property.pid = WPD_OBJECT_ORIGINAL_FILE_NAME;
				try {
					values.GetStringValue(property, out originalName);
				} catch ( Exception e ) {
					originalName = err_name;
				}
			}

			if ( action == CopyAction.All ) {
				// Modified date の取得
				property.pid = WPD_OBJECT_DATE_MODIFIED;
				try {
					string strDate;
					values.GetStringValue(property, out strDate);
					date.Modified = DateTime.ParseExact( strDate, "yyyy/MM/dd:HH:mm:ss.fff", null );
				} catch ( System.Runtime.InteropServices.COMException e ) {
					date.Modified = DateTime.MinValue;
				}


				// Created date の取得
				property.pid = WPD_OBJECT_DATE_CREATED;
				try {
					string strDate;
					values.GetStringValue(property, out strDate);
					date.Created = DateTime.ParseExact( strDate, "yyyy/MM/dd:HH:mm:ss.fff", null );
				} catch ( System.Runtime.InteropServices.COMException e ) {
					date.Created = DateTime.MinValue;
				}


				// Authored date の取得
				property.pid = WPD_OBJECT_DATE_AUTHORED;
				try {
					string strDate;
					values.GetStringValue(property, out strDate);
					date.Authored = DateTime.ParseExact( strDate, "yyyy/MM/dd:HH:mm:ss.fff", null );
				} catch ( System.Runtime.InteropServices.COMException e ) {
					date.Authored = DateTime.MinValue;
				}

			} else {
				date.Modified = DateTime.MinValue;
				date.Created = DateTime.MinValue;
				date.Authored = DateTime.MinValue;
			}


			// type の取得
			Guid contentType;
			property.pid = WPD_OBJECT_CONTENT_TYPE;
			try {
				values.GetGuidValue(property, out contentType);
			} catch ( System.Runtime.InteropServices.COMException e ) {
				PortableDeviceObject obj = new PortableDeviceObject();
				obj.Id = null;
				obj.Name = name;
				obj.kind = ObjectKind.FOLDER;
				obj.Date.Created = date.Created;
				obj.Date.Modified = date.Modified;
				obj.Date.Authored = date.Authored;
				return obj;
			}

			Guid folderType = new Guid(0x27E2E392, 0xA111, 0x48E0, 0xAB, 0x0C, 0xE1, 0x77, 0x05, 0xA0, 0x5F, 0x85);
			Guid functionalType = new Guid(0x99ED0160, 0x17FF, 0x4C44, 0x9D, 0x98, 0x1D, 0x7A, 0x6F, 0x94, 0x19, 0x21);

			if ( name.Equals( err_name ) ) {
				name = originalName;
			}

			// フォルダ
			if (contentType == folderType || contentType == functionalType) {
				PortableDeviceObject fobj = new PortableDeviceObject();
				fobj.Id = objectID;
				fobj.Name = name;
				fobj.kind = ObjectKind.FOLDER;
				fobj.Date.Created = date.Created;
				fobj.Date.Modified = date.Modified;
				fobj.Date.Authored = date.Authored;
				return fobj;
			}

			// ファイル
			PortableDeviceObject robj = new PortableDeviceObject();
			robj.Id = objectID;
			robj.Name = name;
			robj.kind = ObjectKind.FILE;
			robj.Date.Created = date.Created;
			robj.Date.Modified = date.Modified;
			robj.Date.Authored = date.Authored;
			return robj;
		}





		//------------------------------------------------------------------------
		// デバイスをダウンロードする。
		// FileIDがダウンロード元ファイルのID、FilePathがダウンロード先ファイル名
		//------------------------------------------------------------------------
		public static PortableDeviceObject Download(WPDDevice device, string FileID, string FilePath) {

			IPortableDeviceContent content;
			device.DeviceClass.Content(out content);

			IPortableDeviceProperties properties;
			content.Properties(out properties);

			PortableDeviceObject obj = WrapObject(properties, FileID, CopyAction.All);

			IPortableDeviceResources resources;
			content.Transfer(out resources);

			PortableDeviceApiLib.IStream wpdStream;
			uint optimalTransferSize = 0;

			var property = new _tagpropertykey();
			property.fmtid = new Guid(0xE81E79BE, 0x34F0, 0x41BF, 0xB5, 0x3F, 0xF1, 0xA0, 0x6A, 0xE8, 0x78, 0x42);
			property.pid = WPD_RESOURCE_DEFAULT;

			resources.GetStream(FileID, ref property, 0, ref optimalTransferSize, out wpdStream);
			System.Runtime.InteropServices.ComTypes.IStream sourceStream = (System.Runtime.InteropServices.ComTypes.IStream)wpdStream;

			System.IO.FileStream targetStream = new System.IO.FileStream(FilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);

			int BUFFER_SIZE = 32767;
			byte[] buffer = new byte[BUFFER_SIZE];

			IntPtr pbytesRead = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int)));

			while (true) {
				sourceStream.Read(buffer, BUFFER_SIZE, pbytesRead);

				int bytesRead = Marshal.ReadInt32(pbytesRead);
				if (bytesRead <= 0) {
					break;
				}
				targetStream.Write(buffer, 0, bytesRead);
			}
			targetStream.Close();

			Marshal.ReleaseComObject(sourceStream);
			Marshal.ReleaseComObject(wpdStream);

			return obj;
		}

	}






	public enum ObjectKind {
		DEVICE,
		FOLDER,
		FILE
	};


	class WPDDevice : WPDBaseObject {

		public string DeviceID;
		public string DeviceName;
		public PortableDeviceClass DeviceClass;

		public override string ToString() {
			return string.Format( "WPDデバイス名:{0}\r\n", DeviceName );
		}

	}






	//***********************************************************************************
	// ウィンドウズポータブルデバイスの情報
	//***********************************************************************************
	public class PortableDeviceObject : WPDBaseObject {
		public string Id { get; set; }
		public string Name { get; set; }

//		WPDDevice Owner { get; set; }
	}





	//***********************************************************************************
	// ウィンドウズポータブルデバイス、ウィンドウズポータブルデバイスオブジェクトの
	// 基底クラス。
	//***********************************************************************************
	public class WPDBaseObject {
		public ObjectKind kind { get; set; }
		public FileDate Date = new FileDate();
	}





	//***********************************************************************************
	// ファイル情報
	//***********************************************************************************
	public class FileDate {
		public DateTime Created { get; set; }
		public DateTime Authored { get; set; }
		public DateTime Modified { get; set; }
	}
}
