using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[Serializable]
	[ComVisible(true)]
	public sealed class FileInfo : FileSystemInfo
	{
		private bool exists;

		public override bool Exists
		{
			get
			{
				Refresh(false);
				if (stat.Attributes == MonoIO.InvalidFileAttributes)
				{
					return false;
				}
				if ((stat.Attributes & FileAttributes.Directory) != 0)
				{
					return false;
				}
				return exists;
			}
		}

		public override string Name
		{
			get
			{
				return Path.GetFileName(FullPath);
			}
		}

		public bool IsReadOnly
		{
			get
			{
				if (!Exists)
				{
					throw new FileNotFoundException("Could not find file \"" + OriginalPath + "\".", OriginalPath);
				}
				return (stat.Attributes & FileAttributes.ReadOnly) != 0;
			}
			set
			{
				if (!Exists)
				{
					throw new FileNotFoundException("Could not find file \"" + OriginalPath + "\".", OriginalPath);
				}
				FileAttributes attributes = File.GetAttributes(FullPath);
				attributes = ((!value) ? (attributes & ~FileAttributes.ReadOnly) : (attributes | FileAttributes.ReadOnly));
				File.SetAttributes(FullPath, attributes);
			}
		}

		public long Length
		{
			get
			{
				if (!Exists)
				{
					throw new FileNotFoundException("Could not find file \"" + OriginalPath + "\".", OriginalPath);
				}
				return stat.Length;
			}
		}

		public string DirectoryName
		{
			get
			{
				return Path.GetDirectoryName(FullPath);
			}
		}

		public DirectoryInfo Directory
		{
			get
			{
				return new DirectoryInfo(DirectoryName);
			}
		}

		public FileInfo(string fileName)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			CheckPath(fileName);
			OriginalPath = fileName;
			FullPath = Path.GetFullPath(fileName);
		}

		private FileInfo(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		internal override void InternalRefresh()
		{
			exists = File.Exists(FullPath);
		}

		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		[ComVisible(false)]
		public void Encrypt()
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}

		[ComVisible(false)]
		[MonoLimitation("File encryption isn't supported (even on NTFS).")]
		public void Decrypt()
		{
			throw new NotSupportedException(Locale.GetText("File encryption isn't supported on any file system."));
		}

		public StreamReader OpenText()
		{
			return new StreamReader(Open(FileMode.Open, FileAccess.Read));
		}

		public StreamWriter CreateText()
		{
			return new StreamWriter(Open(FileMode.Create, FileAccess.Write));
		}

		public StreamWriter AppendText()
		{
			return new StreamWriter(Open(FileMode.Append, FileAccess.Write));
		}

		public FileStream Create()
		{
			return File.Create(FullPath);
		}

		public FileStream OpenRead()
		{
			return Open(FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public FileStream OpenWrite()
		{
			return Open(FileMode.OpenOrCreate, FileAccess.Write);
		}

		public FileStream Open(FileMode mode)
		{
			return Open(mode, FileAccess.ReadWrite);
		}

		public FileStream Open(FileMode mode, FileAccess access)
		{
			return Open(mode, access, FileShare.None);
		}

		public FileStream Open(FileMode mode, FileAccess access, FileShare share)
		{
			return new FileStream(FullPath, mode, access, share);
		}

		public override void Delete()
		{
			MonoIOError error;
			if (MonoIO.Exists(FullPath, out error))
			{
				if (MonoIO.ExistsDirectory(FullPath, out error))
				{
					throw new UnauthorizedAccessException("Access to the path \"" + FullPath + "\" is denied.");
				}
				if (!MonoIO.DeleteFile(FullPath, out error))
				{
					throw MonoIO.GetException(OriginalPath, error);
				}
			}
		}

		public void MoveTo(string destFileName)
		{
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (!(destFileName == Name) && !(destFileName == FullName))
			{
				if (!File.Exists(FullPath))
				{
					throw new FileNotFoundException();
				}
				File.Move(FullPath, destFileName);
				FullPath = Path.GetFullPath(destFileName);
			}
		}

		public FileInfo CopyTo(string destFileName)
		{
			return CopyTo(destFileName, false);
		}

		public FileInfo CopyTo(string destFileName, bool overwrite)
		{
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destFileName");
			}
			string fullPath = Path.GetFullPath(destFileName);
			if (overwrite && File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
			File.Copy(FullPath, fullPath);
			return new FileInfo(fullPath);
		}

		public override string ToString()
		{
			return Name;
		}

		[ComVisible(false)]
		public FileInfo Replace(string destinationFileName, string destinationBackupFileName)
		{
			string text = null;
			if (!Exists)
			{
				throw new FileNotFoundException();
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			if (destinationFileName.Length == 0)
			{
				throw new ArgumentException("An empty file name is not valid.", "destinationFileName");
			}
			text = Path.GetFullPath(destinationFileName);
			if (!File.Exists(text))
			{
				throw new FileNotFoundException();
			}
			FileAttributes attributes = File.GetAttributes(text);
			if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
			{
				throw new UnauthorizedAccessException();
			}
			if (destinationBackupFileName != null)
			{
				if (destinationBackupFileName.Length == 0)
				{
					throw new ArgumentException("An empty file name is not valid.", "destinationBackupFileName");
				}
				File.Copy(text, Path.GetFullPath(destinationBackupFileName), true);
			}
			File.Copy(FullPath, text, true);
			File.Delete(FullPath);
			return new FileInfo(text);
		}

		[ComVisible(false)]
		public FileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
		{
			throw new NotImplementedException();
		}
	}
}
