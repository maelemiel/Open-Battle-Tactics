using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public sealed class DirectoryInfo : FileSystemInfo
	{
		private string current;

		private string parent;

		public override bool Exists
		{
			get
			{
				Refresh(false);
				if (stat.Attributes == MonoIO.InvalidFileAttributes)
				{
					return false;
				}
				if ((stat.Attributes & FileAttributes.Directory) == 0)
				{
					return false;
				}
				return true;
			}
		}

		public override string Name
		{
			get
			{
				return current;
			}
		}

		public DirectoryInfo Parent
		{
			get
			{
				if (parent == null || parent.Length == 0)
				{
					return null;
				}
				return new DirectoryInfo(parent);
			}
		}

		public DirectoryInfo Root
		{
			get
			{
				string pathRoot = Path.GetPathRoot(FullPath);
				if (pathRoot == null)
				{
					return null;
				}
				return new DirectoryInfo(pathRoot);
			}
		}

		public DirectoryInfo(string path)
			: this(path, false)
		{
		}

		internal DirectoryInfo(string path, bool simpleOriginalPath)
		{
			CheckPath(path);
			FullPath = Path.GetFullPath(path);
			if (simpleOriginalPath)
			{
				OriginalPath = Path.GetFileName(path);
			}
			else
			{
				OriginalPath = path;
			}
			Initialize();
		}

		private DirectoryInfo(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			Initialize();
		}

		private void Initialize()
		{
			int num = FullPath.Length - 1;
			if (num > 1 && FullPath[num] == Path.DirectorySeparatorChar)
			{
				num--;
			}
			int num2 = FullPath.LastIndexOf(Path.DirectorySeparatorChar, num);
			if (num2 == -1 || (num2 == 0 && num == 0))
			{
				current = FullPath;
				parent = null;
				return;
			}
			current = FullPath.Substring(num2 + 1, num - num2);
			if (num2 == 0 && !Environment.IsRunningOnWindows)
			{
				parent = Path.DirectorySeparatorStr;
			}
			else
			{
				parent = FullPath.Substring(0, num2);
			}
			if (Environment.IsRunningOnWindows && parent.Length == 2 && parent[1] == ':' && char.IsLetter(parent[0]))
			{
				parent += Path.DirectorySeparatorChar;
			}
		}

		public void Create()
		{
			Directory.CreateDirectory(FullPath);
		}

		public DirectoryInfo CreateSubdirectory(string path)
		{
			CheckPath(path);
			path = Path.Combine(FullPath, path);
			Directory.CreateDirectory(path);
			return new DirectoryInfo(path);
		}

		public FileInfo[] GetFiles()
		{
			return GetFiles("*");
		}

		public FileInfo[] GetFiles(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string[] files = Directory.GetFiles(FullPath, searchPattern);
			FileInfo[] array = new FileInfo[files.Length];
			int num = 0;
			string[] array2 = files;
			foreach (string fileName in array2)
			{
				array[num++] = new FileInfo(fileName);
			}
			return array;
		}

		public DirectoryInfo[] GetDirectories()
		{
			return GetDirectories("*");
		}

		public DirectoryInfo[] GetDirectories(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			string[] directories = Directory.GetDirectories(FullPath, searchPattern);
			DirectoryInfo[] array = new DirectoryInfo[directories.Length];
			int num = 0;
			string[] array2 = directories;
			foreach (string path in array2)
			{
				array[num++] = new DirectoryInfo(path);
			}
			return array;
		}

		public FileSystemInfo[] GetFileSystemInfos()
		{
			return GetFileSystemInfos("*");
		}

		public FileSystemInfo[] GetFileSystemInfos(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (!Directory.Exists(FullPath))
			{
				throw new IOException("Invalid directory");
			}
			string[] directories = Directory.GetDirectories(FullPath, searchPattern);
			string[] files = Directory.GetFiles(FullPath, searchPattern);
			FileSystemInfo[] array = new FileSystemInfo[directories.Length + files.Length];
			int num = 0;
			string[] array2 = directories;
			foreach (string path in array2)
			{
				array[num++] = new DirectoryInfo(path);
			}
			string[] array3 = files;
			foreach (string fileName in array3)
			{
				array[num++] = new FileInfo(fileName);
			}
			return array;
		}

		public override void Delete()
		{
			Delete(false);
		}

		public void Delete(bool recursive)
		{
			Directory.Delete(FullPath, recursive);
		}

		public void MoveTo(string destDirName)
		{
			if (destDirName == null)
			{
				throw new ArgumentNullException("destDirName");
			}
			if (destDirName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destDirName");
			}
			Directory.Move(FullPath, Path.GetFullPath(destDirName));
		}

		public override string ToString()
		{
			return OriginalPath;
		}

		public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
		{
			switch (searchOption)
			{
			case SearchOption.TopDirectoryOnly:
				return GetDirectories(searchPattern);
			case SearchOption.AllDirectories:
			{
				Queue queue = new Queue(GetDirectories(searchPattern));
				Queue queue2 = new Queue();
				while (queue.Count > 0)
				{
					DirectoryInfo directoryInfo = (DirectoryInfo)queue.Dequeue();
					DirectoryInfo[] directories = directoryInfo.GetDirectories(searchPattern);
					DirectoryInfo[] array = directories;
					foreach (DirectoryInfo obj in array)
					{
						queue.Enqueue(obj);
					}
					queue2.Enqueue(directoryInfo);
				}
				DirectoryInfo[] array2 = new DirectoryInfo[queue2.Count];
				queue2.CopyTo(array2, 0);
				return array2;
			}
			default:
			{
				string text = Locale.GetText("Invalid enum value '{0}' for '{1}'.", searchOption, "SearchOption");
				throw new ArgumentOutOfRangeException("searchOption", text);
			}
			}
		}

		internal int GetFilesSubdirs(ArrayList l, string pattern)
		{
			FileInfo[] array = null;
			try
			{
				array = GetFiles(pattern);
			}
			catch (UnauthorizedAccessException)
			{
				return 0;
			}
			int num = array.Length;
			l.Add(array);
			DirectoryInfo[] directories = GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				num += directoryInfo.GetFilesSubdirs(l, pattern);
			}
			return num;
		}

		public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
		{
			switch (searchOption)
			{
			case SearchOption.TopDirectoryOnly:
				return GetFiles(searchPattern);
			case SearchOption.AllDirectories:
			{
				ArrayList arrayList = new ArrayList();
				int filesSubdirs = GetFilesSubdirs(arrayList, searchPattern);
				int num = 0;
				FileInfo[] array = new FileInfo[filesSubdirs];
				{
					foreach (FileInfo[] item in arrayList)
					{
						item.CopyTo(array, num);
						num += item.Length;
					}
					return array;
				}
			}
			default:
			{
				string text = Locale.GetText("Invalid enum value '{0}' for '{1}'.", searchOption, "SearchOption");
				throw new ArgumentOutOfRangeException("searchOption", text);
			}
			}
		}
	}
}
