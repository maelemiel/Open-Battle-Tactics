using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	public static class File
	{
		private static DateTime? defaultLocalFileTime;

		private static DateTime DefaultLocalFileTime
		{
			get
			{
				DateTime? dateTime = defaultLocalFileTime;
				if (!dateTime.HasValue)
				{
					defaultLocalFileTime = new DateTime(1601, 1, 1).ToLocalTime();
				}
				return defaultLocalFileTime.Value;
			}
		}

		public static void AppendAllText(string path, string contents)
		{
			using (TextWriter textWriter = new StreamWriter(path, true))
			{
				textWriter.Write(contents);
			}
		}

		public static void AppendAllText(string path, string contents, Encoding encoding)
		{
			using (TextWriter textWriter = new StreamWriter(path, true, encoding))
			{
				textWriter.Write(contents);
			}
		}

		public static StreamWriter AppendText(string path)
		{
			return new StreamWriter(path, true);
		}

		public static void Copy(string sourceFileName, string destFileName)
		{
			Copy(sourceFileName, destFileName, false);
		}

		public static void Copy(string sourceFileName, string destFileName, bool overwrite)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "sourceFileName");
			}
			if (sourceFileName.Trim().Length == 0 || sourceFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destFileName");
			}
			if (destFileName.Trim().Length == 0 || destFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			MonoIOError error;
			if (!MonoIO.Exists(sourceFileName, out error))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", sourceFileName), sourceFileName);
			}
			if ((GetAttributes(sourceFileName) & FileAttributes.Directory) == FileAttributes.Directory)
			{
				throw new ArgumentException(Locale.GetText("{0} is a directory", sourceFileName));
			}
			if (MonoIO.Exists(destFileName, out error))
			{
				if ((GetAttributes(destFileName) & FileAttributes.Directory) == FileAttributes.Directory)
				{
					throw new ArgumentException(Locale.GetText("{0} is a directory", destFileName));
				}
				if (!overwrite)
				{
					throw new IOException(Locale.GetText("{0} already exists", destFileName));
				}
			}
			string directoryName = Path.GetDirectoryName(destFileName);
			if (directoryName != string.Empty && !Directory.Exists(directoryName))
			{
				throw new DirectoryNotFoundException(Locale.GetText("Destination directory not found: {0}", directoryName));
			}
			if (!MonoIO.CopyFile(sourceFileName, destFileName, overwrite, out error))
			{
				string text = Locale.GetText("{0}\" or \"{1}", sourceFileName, destFileName);
				throw MonoIO.GetException(text, error);
			}
		}

		public static FileStream Create(string path)
		{
			return Create(path, 8192);
		}

		public static FileStream Create(string path, int bufferSize)
		{
			return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize);
		}

		public static StreamWriter CreateText(string path)
		{
			return new StreamWriter(path, false);
		}

		public static void Delete(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0 || path.IndexOfAny(Path.InvalidPathChars) >= 0)
			{
				throw new ArgumentException("path");
			}
			if (Directory.Exists(path))
			{
				throw new UnauthorizedAccessException(Locale.GetText("{0} is a directory", path));
			}
			string directoryName = Path.GetDirectoryName(path);
			if (directoryName != string.Empty && !Directory.Exists(directoryName))
			{
				throw new DirectoryNotFoundException(Locale.GetText("Could not find a part of the path \"{0}\".", path));
			}
			MonoIOError error;
			if (!MonoIO.DeleteFile(path, out error) && error != MonoIOError.ERROR_FILE_NOT_FOUND)
			{
				throw MonoIO.GetException(path, error);
			}
		}

		public static bool Exists(string path)
		{
			if (path == null || path.Trim().Length == 0 || path.IndexOfAny(Path.InvalidPathChars) >= 0)
			{
				return false;
			}
			MonoIOError error;
			return MonoIO.ExistsFile(path, out error);
		}

		public static FileAttributes GetAttributes(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Path is empty"));
			}
			if (path.IndexOfAny(Path.InvalidPathChars) >= 0)
			{
				throw new ArgumentException(Locale.GetText("Path contains invalid chars"));
			}
			MonoIOError error;
			FileAttributes fileAttributes = MonoIO.GetFileAttributes(path, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(path, error);
			}
			return fileAttributes;
		}

		public static DateTime GetCreationTime(string path)
		{
			CheckPathExceptions(path);
			MonoIOStat stat;
			MonoIOError error;
			if (!MonoIO.GetFileStat(path, out stat, out error))
			{
				if (error == MonoIOError.ERROR_PATH_NOT_FOUND || error == MonoIOError.ERROR_FILE_NOT_FOUND)
				{
					return DefaultLocalFileTime;
				}
				throw new IOException(path);
			}
			return DateTime.FromFileTime(stat.CreationTime);
		}

		public static DateTime GetCreationTimeUtc(string path)
		{
			return GetCreationTime(path).ToUniversalTime();
		}

		public static DateTime GetLastAccessTime(string path)
		{
			CheckPathExceptions(path);
			MonoIOStat stat;
			MonoIOError error;
			if (!MonoIO.GetFileStat(path, out stat, out error))
			{
				if (error == MonoIOError.ERROR_PATH_NOT_FOUND || error == MonoIOError.ERROR_FILE_NOT_FOUND)
				{
					return DefaultLocalFileTime;
				}
				throw new IOException(path);
			}
			return DateTime.FromFileTime(stat.LastAccessTime);
		}

		public static DateTime GetLastAccessTimeUtc(string path)
		{
			return GetLastAccessTime(path).ToUniversalTime();
		}

		public static DateTime GetLastWriteTime(string path)
		{
			CheckPathExceptions(path);
			MonoIOStat stat;
			MonoIOError error;
			if (!MonoIO.GetFileStat(path, out stat, out error))
			{
				if (error == MonoIOError.ERROR_PATH_NOT_FOUND || error == MonoIOError.ERROR_FILE_NOT_FOUND)
				{
					return DefaultLocalFileTime;
				}
				throw new IOException(path);
			}
			return DateTime.FromFileTime(stat.LastWriteTime);
		}

		public static DateTime GetLastWriteTimeUtc(string path)
		{
			return GetLastWriteTime(path).ToUniversalTime();
		}

		public static void Move(string sourceFileName, string destFileName)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "sourceFileName");
			}
			if (sourceFileName.Trim().Length == 0 || sourceFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destFileName");
			}
			if (destFileName.Trim().Length == 0 || destFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("The file name is not valid.");
			}
			MonoIOError error;
			if (!MonoIO.Exists(sourceFileName, out error))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", sourceFileName), sourceFileName);
			}
			string directoryName = Path.GetDirectoryName(destFileName);
			if (directoryName != string.Empty && !Directory.Exists(directoryName))
			{
				throw new DirectoryNotFoundException(Locale.GetText("Could not find a part of the path."));
			}
			if (!MonoIO.MoveFile(sourceFileName, destFileName, out error))
			{
				switch (error)
				{
				case MonoIOError.ERROR_ALREADY_EXISTS:
					throw MonoIO.GetException(error);
				case MonoIOError.ERROR_SHARING_VIOLATION:
					throw MonoIO.GetException(sourceFileName, error);
				default:
					throw MonoIO.GetException(error);
				}
			}
		}

		public static FileStream Open(string path, FileMode mode)
		{
			return new FileStream(path, mode, (mode != FileMode.Append) ? FileAccess.ReadWrite : FileAccess.Write, FileShare.None);
		}

		public static FileStream Open(string path, FileMode mode, FileAccess access)
		{
			return new FileStream(path, mode, access, FileShare.None);
		}

		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return new FileStream(path, mode, access, share);
		}

		public static FileStream OpenRead(string path)
		{
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public static StreamReader OpenText(string path)
		{
			return new StreamReader(path);
		}

		public static FileStream OpenWrite(string path)
		{
			return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		}

		public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
		{
			Replace(sourceFileName, destinationFileName, destinationBackupFileName, false);
		}

		public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			if (sourceFileName.Trim().Length == 0 || sourceFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("sourceFileName");
			}
			if (destinationFileName.Trim().Length == 0 || destinationFileName.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("destinationFileName");
			}
			string fullPath = Path.GetFullPath(sourceFileName);
			string fullPath2 = Path.GetFullPath(destinationFileName);
			MonoIOError error;
			if (MonoIO.ExistsDirectory(fullPath, out error))
			{
				throw new IOException(Locale.GetText("{0} is a directory", sourceFileName));
			}
			if (MonoIO.ExistsDirectory(fullPath2, out error))
			{
				throw new IOException(Locale.GetText("{0} is a directory", destinationFileName));
			}
			if (!Exists(fullPath))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", sourceFileName), sourceFileName);
			}
			if (!Exists(fullPath2))
			{
				throw new FileNotFoundException(Locale.GetText("{0} does not exist", destinationFileName), destinationFileName);
			}
			if (fullPath == fullPath2)
			{
				throw new IOException(Locale.GetText("Source and destination arguments are the same file."));
			}
			string text = null;
			if (destinationBackupFileName != null)
			{
				if (destinationBackupFileName.Trim().Length == 0 || destinationBackupFileName.IndexOfAny(Path.InvalidPathChars) != -1)
				{
					throw new ArgumentException("destinationBackupFileName");
				}
				text = Path.GetFullPath(destinationBackupFileName);
				if (MonoIO.ExistsDirectory(text, out error))
				{
					throw new IOException(Locale.GetText("{0} is a directory", destinationBackupFileName));
				}
				if (fullPath == text)
				{
					throw new IOException(Locale.GetText("Source and backup arguments are the same file."));
				}
				if (fullPath2 == text)
				{
					throw new IOException(Locale.GetText("Destination and backup arguments are the same file."));
				}
			}
			if (!MonoIO.ReplaceFile(fullPath, fullPath2, text, ignoreMetadataErrors, out error))
			{
				throw MonoIO.GetException(error);
			}
		}

		public static void SetAttributes(string path, FileAttributes fileAttributes)
		{
			CheckPathExceptions(path);
			MonoIOError error;
			if (!MonoIO.SetFileAttributes(path, fileAttributes, out error))
			{
				throw MonoIO.GetException(path, error);
			}
		}

		public static void SetCreationTime(string path, DateTime creationTime)
		{
			CheckPathExceptions(path);
			MonoIOError error;
			if (!MonoIO.Exists(path, out error))
			{
				throw MonoIO.GetException(path, error);
			}
			if (!MonoIO.SetCreationTime(path, creationTime, out error))
			{
				throw MonoIO.GetException(path, error);
			}
		}

		public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			SetCreationTime(path, creationTimeUtc.ToLocalTime());
		}

		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			CheckPathExceptions(path);
			MonoIOError error;
			if (!MonoIO.Exists(path, out error))
			{
				throw MonoIO.GetException(path, error);
			}
			if (!MonoIO.SetLastAccessTime(path, lastAccessTime, out error))
			{
				throw MonoIO.GetException(path, error);
			}
		}

		public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			SetLastAccessTime(path, lastAccessTimeUtc.ToLocalTime());
		}

		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			CheckPathExceptions(path);
			MonoIOError error;
			if (!MonoIO.Exists(path, out error))
			{
				throw MonoIO.GetException(path, error);
			}
			if (!MonoIO.SetLastWriteTime(path, lastWriteTime, out error))
			{
				throw MonoIO.GetException(path, error);
			}
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
				throw new ArgumentException(Locale.GetText("Path is empty"));
			}
			if (path.Trim().Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Path is empty"));
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException(Locale.GetText("Path contains invalid chars"));
			}
		}

		public static byte[] ReadAllBytes(string path)
		{
			using (FileStream fileStream = OpenRead(path))
			{
				long length = fileStream.Length;
				if (length > int.MaxValue)
				{
					throw new IOException("Reading more than 2GB with this call is not supported");
				}
				int num = 0;
				int num2 = (int)length;
				byte[] array = new byte[length];
				while (num2 > 0)
				{
					int num3 = fileStream.Read(array, num, num2);
					if (num3 == 0)
					{
						throw new IOException("Unexpected end of stream");
					}
					num += num3;
					num2 -= num3;
				}
				return array;
			}
		}

		public static string[] ReadAllLines(string path)
		{
			using (StreamReader reader = OpenText(path))
			{
				return ReadAllLines(reader);
			}
		}

		public static string[] ReadAllLines(string path, Encoding encoding)
		{
			using (StreamReader reader = new StreamReader(path, encoding))
			{
				return ReadAllLines(reader);
			}
		}

		private static string[] ReadAllLines(StreamReader reader)
		{
			List<string> list = new List<string>();
			while (!reader.EndOfStream)
			{
				list.Add(reader.ReadLine());
			}
			return list.ToArray();
		}

		public static string ReadAllText(string path)
		{
			return ReadAllText(path, Encoding.UTF8Unmarked);
		}

		public static string ReadAllText(string path, Encoding encoding)
		{
			using (StreamReader streamReader = new StreamReader(path, encoding))
			{
				return streamReader.ReadToEnd();
			}
		}

		public static void WriteAllBytes(string path, byte[] bytes)
		{
			using (Stream stream = Create(path))
			{
				stream.Write(bytes, 0, bytes.Length);
			}
		}

		public static void WriteAllLines(string path, string[] contents)
		{
			using (StreamWriter writer = new StreamWriter(path))
			{
				WriteAllLines(writer, contents);
			}
		}

		public static void WriteAllLines(string path, string[] contents, Encoding encoding)
		{
			using (StreamWriter writer = new StreamWriter(path, false, encoding))
			{
				WriteAllLines(writer, contents);
			}
		}

		private static void WriteAllLines(StreamWriter writer, string[] contents)
		{
			foreach (string value in contents)
			{
				writer.WriteLine(value);
			}
		}

		public static void WriteAllText(string path, string contents)
		{
			WriteAllText(path, contents, Encoding.UTF8Unmarked);
		}

		public static void WriteAllText(string path, string contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, false, encoding))
			{
				streamWriter.Write(contents);
			}
		}

		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		public static void Encrypt(string path)
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}

		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		public static void Decrypt(string path)
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}
	}
}
