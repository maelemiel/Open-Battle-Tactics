using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace System.IO.IsolatedStorage
{
	[ComVisible(true)]
	public class IsolatedStorageFileStream : FileStream
	{
		public override bool CanRead
		{
			get
			{
				return base.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return base.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return base.CanWrite;
			}
		}

		public override SafeFileHandle SafeFileHandle
		{
			get
			{
				throw new IsolatedStorageException(Locale.GetText("Information is restricted"));
			}
		}

		[Obsolete("Use SafeFileHandle - once available")]
		public override IntPtr Handle
		{
			get
			{
				throw new IsolatedStorageException(Locale.GetText("Information is restricted"));
			}
		}

		public override bool IsAsync
		{
			get
			{
				return base.IsAsync;
			}
		}

		public override long Length
		{
			get
			{
				return base.Length;
			}
		}

		public override long Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				base.Position = value;
			}
		}

		public IsolatedStorageFileStream(string path, FileMode mode)
			: this(path, mode, (mode != FileMode.Append) ? FileAccess.ReadWrite : FileAccess.Write, FileShare.Read, 8192, null)
		{
		}

		public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access)
			: this(path, mode, access, (access != FileAccess.Write) ? FileShare.Read : FileShare.None, 8192, null)
		{
		}

		public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share)
			: this(path, mode, access, share, 8192, null)
		{
		}

		public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
			: this(path, mode, access, share, bufferSize, null)
		{
		}

		public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, IsolatedStorageFile isf)
			: base(CreateIsolatedPath(isf, path, mode), mode, access, share, bufferSize, false, true)
		{
		}

		public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, IsolatedStorageFile isf)
			: this(path, mode, access, share, 8192, isf)
		{
		}

		public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, IsolatedStorageFile isf)
			: this(path, mode, access, (access != FileAccess.Write) ? FileShare.Read : FileShare.None, 8192, isf)
		{
		}

		public IsolatedStorageFileStream(string path, FileMode mode, IsolatedStorageFile isf)
			: this(path, mode, (mode != FileMode.Append) ? FileAccess.ReadWrite : FileAccess.Write, FileShare.Read, 8192, isf)
		{
		}

		private static string CreateIsolatedPath(IsolatedStorageFile isf, string path, FileMode mode)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (!Enum.IsDefined(typeof(FileMode), mode))
			{
				throw new ArgumentException("mode");
			}
			if (isf == null)
			{
				StackFrame stackFrame = new StackFrame(3);
				isf = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Domain | IsolatedStorageScope.Assembly, IsolatedStorageFile.GetDomainIdentityFromEvidence(AppDomain.CurrentDomain.Evidence), IsolatedStorageFile.GetAssemblyIdentityFromEvidence(stackFrame.GetMethod().ReflectedType.Assembly.UnprotectedGetEvidence()));
			}
			FileInfo fileInfo = new FileInfo(isf.Root);
			if (!fileInfo.Directory.Exists)
			{
				fileInfo.Directory.Create();
			}
			if (Path.IsPathRooted(path))
			{
				string pathRoot = Path.GetPathRoot(path);
				path = path.Remove(0, pathRoot.Length);
			}
			string text = Path.Combine(isf.Root, path);
			string fullPath = Path.GetFullPath(text);
			fullPath = Path.GetFullPath(text);
			if (!fullPath.StartsWith(isf.Root))
			{
				throw new IsolatedStorageException();
			}
			fileInfo = new FileInfo(text);
			if (!fileInfo.Directory.Exists)
			{
				string text2 = Locale.GetText("Could not find a part of the path \"{0}\".");
				throw new DirectoryNotFoundException(string.Format(text2, path));
			}
			return text;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			return base.BeginRead(buffer, offset, numBytes, userCallback, stateObject);
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			return base.BeginWrite(buffer, offset, numBytes, userCallback, stateObject);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return base.EndRead(asyncResult);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			base.EndWrite(asyncResult);
		}

		public override void Flush()
		{
			base.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return base.Read(buffer, offset, count);
		}

		public override int ReadByte()
		{
			return base.ReadByte();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return base.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			base.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			base.Write(buffer, offset, count);
		}

		public override void WriteByte(byte value)
		{
			base.WriteByte(value);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}
	}
}
