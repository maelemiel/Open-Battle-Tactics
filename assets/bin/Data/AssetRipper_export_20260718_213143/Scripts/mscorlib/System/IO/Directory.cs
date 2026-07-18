using System.Collections;
using System.Runtime.InteropServices;

namespace System.IO
{
	[ComVisible(true)]
	public static class Directory
	{
		public static DirectoryInfo CreateDirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is empty");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid chars");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Only blank characters in path");
			}
			if (File.Exists(path))
			{
				throw new IOException("Cannot create " + path + " because a file with the same name already exists.");
			}
			if (path == ":")
			{
				throw new ArgumentException("Only ':' In path");
			}
			return CreateDirectoriesInternal(path);
		}

		private static DirectoryInfo CreateDirectoriesInternal(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path, true);
			if (directoryInfo.Parent != null && !directoryInfo.Parent.Exists)
			{
				directoryInfo.Parent.Create();
			}
			MonoIOError error;
			if (!MonoIO.CreateDirectory(path, out error) && error != MonoIOError.ERROR_ALREADY_EXISTS && error != MonoIOError.ERROR_FILE_EXISTS)
			{
				throw MonoIO.GetException(path, error);
			}
			return directoryInfo;
		}

		public static void Delete(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is empty");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid chars");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Only blank characters in path");
			}
			if (path == ":")
			{
				throw new NotSupportedException("Only ':' In path");
			}
			MonoIOError error;
			if ((!MonoIO.ExistsSymlink(path, out error)) ? MonoIO.RemoveDirectory(path, out error) : MonoIO.DeleteFile(path, out error))
			{
				return;
			}
			if (error == MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				if (File.Exists(path))
				{
					throw new IOException("Directory does not exist, but a file of the same name exist.");
				}
				throw new DirectoryNotFoundException("Directory does not exist.");
			}
			throw MonoIO.GetException(path, error);
		}

		private static void RecursiveDelete(string path)
		{
			string[] directories = GetDirectories(path);
			foreach (string path2 in directories)
			{
				MonoIOError error;
				if (MonoIO.ExistsSymlink(path2, out error))
				{
					MonoIO.DeleteFile(path2, out error);
				}
				else
				{
					RecursiveDelete(path2);
				}
			}
			string[] files = GetFiles(path);
			foreach (string path3 in files)
			{
				File.Delete(path3);
			}
			Delete(path);
		}

		public static void Delete(string path, bool recursive)
		{
			CheckPathExceptions(path);
			if (recursive)
			{
				RecursiveDelete(path);
			}
			else
			{
				Delete(path);
			}
		}

		public static bool Exists(string path)
		{
			if (path == null)
			{
				return false;
			}
			MonoIOError error;
			return MonoIO.ExistsDirectory(path, out error);
		}

		public static DateTime GetLastAccessTime(string path)
		{
			return File.GetLastAccessTime(path);
		}

		public static DateTime GetLastAccessTimeUtc(string path)
		{
			return GetLastAccessTime(path).ToUniversalTime();
		}

		public static DateTime GetLastWriteTime(string path)
		{
			return File.GetLastWriteTime(path);
		}

		public static DateTime GetLastWriteTimeUtc(string path)
		{
			return GetLastWriteTime(path).ToUniversalTime();
		}

		public static DateTime GetCreationTime(string path)
		{
			return File.GetCreationTime(path);
		}

		public static DateTime GetCreationTimeUtc(string path)
		{
			return GetCreationTime(path).ToUniversalTime();
		}

		public static string GetCurrentDirectory()
		{
			MonoIOError error;
			string currentDirectory = MonoIO.GetCurrentDirectory(out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(error);
			}
			return currentDirectory;
		}

		public static string[] GetDirectories(string path)
		{
			return GetDirectories(path, "*");
		}

		public static string[] GetDirectories(string path, string searchPattern)
		{
			return GetFileSystemEntries(path, searchPattern, FileAttributes.Directory, FileAttributes.Directory);
		}

		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				return GetDirectories(path, searchPattern);
			}
			ArrayList arrayList = new ArrayList();
			GetDirectoriesRecurse(path, searchPattern, arrayList);
			return (string[])arrayList.ToArray(typeof(string));
		}

		private static void GetDirectoriesRecurse(string path, string searchPattern, ArrayList all)
		{
			all.AddRange(GetDirectories(path, searchPattern));
			string[] directories = GetDirectories(path);
			foreach (string path2 in directories)
			{
				GetDirectoriesRecurse(path2, searchPattern, all);
			}
		}

		public static string GetDirectoryRoot(string path)
		{
			return new string(Path.DirectorySeparatorChar, 1);
		}

		public static string[] GetFiles(string path)
		{
			return GetFiles(path, "*");
		}

		public static string[] GetFiles(string path, string searchPattern)
		{
			return GetFileSystemEntries(path, searchPattern, FileAttributes.Directory, (FileAttributes)0);
		}

		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (searchOption == SearchOption.TopDirectoryOnly)
			{
				return GetFiles(path, searchPattern);
			}
			ArrayList arrayList = new ArrayList();
			GetFilesRecurse(path, searchPattern, arrayList);
			return (string[])arrayList.ToArray(typeof(string));
		}

		private static void GetFilesRecurse(string path, string searchPattern, ArrayList all)
		{
			all.AddRange(GetFiles(path, searchPattern));
			string[] directories = GetDirectories(path);
			foreach (string path2 in directories)
			{
				GetFilesRecurse(path2, searchPattern, all);
			}
		}

		public static string[] GetFileSystemEntries(string path)
		{
			return GetFileSystemEntries(path, "*");
		}

		public static string[] GetFileSystemEntries(string path, string searchPattern)
		{
			return GetFileSystemEntries(path, searchPattern, (FileAttributes)0, (FileAttributes)0);
		}

		public static string[] GetLogicalDrives()
		{
			return Environment.GetLogicalDrives();
		}

		private static bool IsRootDirectory(string path)
		{
			if (Path.DirectorySeparatorChar == '/' && path == "/")
			{
				return true;
			}
			if (Path.DirectorySeparatorChar == '\\' && path.Length == 3 && path.EndsWith(":\\"))
			{
				return true;
			}
			return false;
		}

		public static DirectoryInfo GetParent(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid characters");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("The Path do not have a valid format");
			}
			if (IsRootDirectory(path))
			{
				return null;
			}
			string text = Path.GetDirectoryName(path);
			if (text.Length == 0)
			{
				text = GetCurrentDirectory();
			}
			return new DirectoryInfo(text);
		}

		public static void Move(string sourceDirName, string destDirName)
		{
			if (sourceDirName == null)
			{
				throw new ArgumentNullException("sourceDirName");
			}
			if (destDirName == null)
			{
				throw new ArgumentNullException("destDirName");
			}
			if (sourceDirName.Trim().Length == 0 || sourceDirName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Invalid source directory name: " + sourceDirName, "sourceDirName");
			}
			if (destDirName.Trim().Length == 0 || destDirName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Invalid target directory name: " + destDirName, "destDirName");
			}
			if (sourceDirName == destDirName)
			{
				throw new IOException("Source and destination path must be different.");
			}
			if (Exists(destDirName))
			{
				throw new IOException(destDirName + " already exists.");
			}
			if (!Exists(sourceDirName) && !File.Exists(sourceDirName))
			{
				throw new DirectoryNotFoundException(sourceDirName + " does not exist");
			}
			MonoIOError error;
			if (!MonoIO.MoveFile(sourceDirName, destDirName, out error))
			{
				throw MonoIO.GetException(error);
			}
		}

		public static void SetCreationTime(string path, DateTime creationTime)
		{
			File.SetCreationTime(path, creationTime);
		}

		public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			SetCreationTime(path, creationTimeUtc.ToLocalTime());
		}

		public static void SetCurrentDirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("path string must not be an empty string or whitespace string");
			}
			if (!Exists(path))
			{
				throw new DirectoryNotFoundException("Directory \"" + path + "\" not found.");
			}
			MonoIOError error;
			MonoIO.SetCurrentDirectory(path, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(path, error);
			}
		}

		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			File.SetLastAccessTime(path, lastAccessTime);
		}

		public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			SetLastAccessTime(path, lastAccessTimeUtc.ToLocalTime());
		}

		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			File.SetLastWriteTime(path, lastWriteTime);
		}

		public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			SetLastWriteTime(path, lastWriteTimeUtc.ToLocalTime());
		}

		private static void CheckPathExceptions(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is Empty");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("Only blank characters in path");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid chars");
			}
		}

		private static string[] GetFileSystemEntries(string path, string searchPattern, FileAttributes mask, FileAttributes attrs)
		{
			if (path == null || searchPattern == null)
			{
				throw new ArgumentNullException();
			}
			if (searchPattern.Length == 0)
			{
				return new string[0];
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException("The Path does not have a valid format");
			}
			string path2 = Path.Combine(path, searchPattern);
			string directoryName = Path.GetDirectoryName(path2);
			if (directoryName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Path contains invalid characters");
			}
			if (directoryName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				if (path.IndexOfAny(SearchPattern.InvalidChars) == -1)
				{
					throw new ArgumentException("Path contains invalid characters", "path");
				}
				throw new ArgumentException("Pattern contains invalid characters", "pattern");
			}
			MonoIOError error;
			if (!MonoIO.ExistsDirectory(directoryName, out error))
			{
				MonoIOError error2;
				if (error == MonoIOError.ERROR_SUCCESS && MonoIO.ExistsFile(directoryName, out error2))
				{
					return new string[1] { directoryName };
				}
				if (error != MonoIOError.ERROR_PATH_NOT_FOUND)
				{
					throw MonoIO.GetException(directoryName, error);
				}
				if (directoryName.IndexOfAny(SearchPattern.WildcardChars) == -1)
				{
					throw new DirectoryNotFoundException("Directory '" + directoryName + "' not found.");
				}
				if (path.IndexOfAny(SearchPattern.WildcardChars) == -1)
				{
					throw new ArgumentException("Pattern is invalid", "searchPattern");
				}
				throw new ArgumentException("Path is invalid", "path");
			}
			string path_with_pattern = Path.Combine(directoryName, searchPattern);
			string[] fileSystemEntries = MonoIO.GetFileSystemEntries(path, path_with_pattern, (int)attrs, (int)mask, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(directoryName, error);
			}
			return fileSystemEntries;
		}
	}
}
