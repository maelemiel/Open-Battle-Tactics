using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public sealed class DriveInfo : ISerializable
	{
		private enum _DriveType
		{
			GenericUnix = 0,
			Linux = 1,
			Windows = 2
		}

		private _DriveType _drive_type;

		private string drive_format;

		private string path;

		public long AvailableFreeSpace
		{
			get
			{
				ulong availableFreeSpace;
				ulong totalSize;
				ulong totalFreeSpace;
				GetDiskFreeSpace(path, out availableFreeSpace, out totalSize, out totalFreeSpace);
				return (long)((availableFreeSpace <= long.MaxValue) ? availableFreeSpace : long.MaxValue);
			}
		}

		public long TotalFreeSpace
		{
			get
			{
				ulong availableFreeSpace;
				ulong totalSize;
				ulong totalFreeSpace;
				GetDiskFreeSpace(path, out availableFreeSpace, out totalSize, out totalFreeSpace);
				return (long)((totalFreeSpace <= long.MaxValue) ? totalFreeSpace : long.MaxValue);
			}
		}

		public long TotalSize
		{
			get
			{
				ulong availableFreeSpace;
				ulong totalSize;
				ulong totalFreeSpace;
				GetDiskFreeSpace(path, out availableFreeSpace, out totalSize, out totalFreeSpace);
				return (long)((totalSize <= long.MaxValue) ? totalSize : long.MaxValue);
			}
		}

		[MonoTODO("Currently get only works on Mono/Unix; set not implemented")]
		public string VolumeLabel
		{
			get
			{
				if (_drive_type != _DriveType.Windows)
				{
					return path;
				}
				return path;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string DriveFormat
		{
			get
			{
				return drive_format;
			}
		}

		public DriveType DriveType
		{
			get
			{
				return (DriveType)GetDriveTypeInternal(path);
			}
		}

		public string Name
		{
			get
			{
				return path;
			}
		}

		public DirectoryInfo RootDirectory
		{
			get
			{
				return new DirectoryInfo(path);
			}
		}

		[MonoTODO("It always returns true")]
		public bool IsReady
		{
			get
			{
				if (_drive_type != _DriveType.Windows)
				{
					return true;
				}
				return true;
			}
		}

		private DriveInfo(_DriveType _drive_type, string path, string fstype)
		{
			this._drive_type = _drive_type;
			drive_format = fstype;
			this.path = path;
		}

		public DriveInfo(string driveName)
		{
			DriveInfo[] drives = GetDrives();
			DriveInfo[] array = drives;
			foreach (DriveInfo driveInfo in array)
			{
				if (driveInfo.path == driveName)
				{
					path = driveInfo.path;
					drive_format = driveInfo.drive_format;
					path = driveInfo.path;
					return;
				}
			}
			throw new ArgumentException("The drive name does not exist", "driveName");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		private static void GetDiskFreeSpace(string path, out ulong availableFreeSpace, out ulong totalSize, out ulong totalFreeSpace)
		{
			MonoIOError error;
			if (!GetDiskFreeSpaceInternal(path, out availableFreeSpace, out totalSize, out totalFreeSpace, out error))
			{
				throw MonoIO.GetException(path, error);
			}
		}

		private static StreamReader TryOpen(string name)
		{
			if (File.Exists(name))
			{
				return new StreamReader(name, Encoding.ASCII);
			}
			return null;
		}

		private static DriveInfo[] LinuxGetDrives()
		{
			using (StreamReader streamReader = TryOpen("/proc/mounts"))
			{
				ArrayList arrayList = new ArrayList();
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					if (text.StartsWith("rootfs"))
					{
						continue;
					}
					int num = text.IndexOf(' ');
					if (num == -1)
					{
						continue;
					}
					string text2 = text.Substring(num + 1);
					num = text2.IndexOf(' ');
					if (num != -1)
					{
						string text3 = text2.Substring(0, num);
						text2 = text2.Substring(num + 1);
						num = text2.IndexOf(' ');
						if (num != -1)
						{
							string fstype = text2.Substring(0, num);
							arrayList.Add(new DriveInfo(_DriveType.Linux, text3, fstype));
						}
					}
				}
				return (DriveInfo[])arrayList.ToArray(typeof(DriveInfo));
			}
		}

		private static DriveInfo[] UnixGetDrives()
		{
			DriveInfo[] array = null;
			try
			{
				using (StreamReader streamReader = TryOpen("/proc/sys/kernel/ostype"))
				{
					if (streamReader != null)
					{
						string text = streamReader.ReadLine();
						if (text == "Linux")
						{
							array = LinuxGetDrives();
						}
					}
				}
				if (array != null)
				{
					return array;
				}
			}
			catch (Exception)
			{
			}
			return new DriveInfo[1]
			{
				new DriveInfo(_DriveType.GenericUnix, "/", "unixfs")
			};
		}

		private static DriveInfo[] WindowsGetDrives()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Currently only implemented on Mono/Linux")]
		public static DriveInfo[] GetDrives()
		{
			int platform = (int)Environment.Platform;
			if (platform == 4 || platform == 128 || platform == 6)
			{
				return UnixGetDrives();
			}
			return WindowsGetDrives();
		}

		public override string ToString()
		{
			return Name;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetDiskFreeSpaceInternal(string pathName, out ulong freeBytesAvail, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes, out MonoIOError error);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint GetDriveTypeInternal(string rootPathName);
	}
}
