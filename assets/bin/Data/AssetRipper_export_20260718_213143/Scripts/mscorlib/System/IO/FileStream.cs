using System.IO.IsolatedStorage;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	[ComVisible(true)]
	public class FileStream : Stream
	{
		private delegate int ReadDelegate(byte[] buffer, int offset, int count);

		private delegate void WriteDelegate(byte[] buffer, int offset, int count);

		internal const int DefaultBufferSize = 8192;

		private FileAccess access;

		private bool owner;

		private bool async;

		private bool canseek;

		private long append_startpos;

		private bool anonymous;

		private byte[] buf;

		private int buf_size;

		private int buf_length;

		private int buf_offset;

		private bool buf_dirty;

		private long buf_start;

		private string name = "[Unknown]";

		private IntPtr handle;

		private SafeFileHandle safeHandle;

		public override bool CanRead
		{
			get
			{
				return access == FileAccess.Read || access == FileAccess.ReadWrite;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return access == FileAccess.Write || access == FileAccess.ReadWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return canseek;
			}
		}

		public virtual bool IsAsync
		{
			get
			{
				return async;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public override long Length
		{
			get
			{
				if (handle == MonoIO.InvalidHandle)
				{
					throw new ObjectDisposedException("Stream has been closed");
				}
				if (!CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				FlushBufferIfDirty();
				MonoIOError error;
				long length = MonoIO.GetLength(handle, out error);
				if (error != MonoIOError.ERROR_SUCCESS)
				{
					throw MonoIO.GetException(GetSecureFileName(name), error);
				}
				return length;
			}
		}

		public override long Position
		{
			get
			{
				if (handle == MonoIO.InvalidHandle)
				{
					throw new ObjectDisposedException("Stream has been closed");
				}
				if (!CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				return buf_start + buf_offset;
			}
			set
			{
				if (handle == MonoIO.InvalidHandle)
				{
					throw new ObjectDisposedException("Stream has been closed");
				}
				if (!CanSeek)
				{
					throw new NotSupportedException("The stream does not support seeking");
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("Attempt to set the position to a negative value");
				}
				Seek(value, SeekOrigin.Begin);
			}
		}

		[Obsolete("Use SafeFileHandle instead")]
		public virtual IntPtr Handle
		{
			get
			{
				return handle;
			}
		}

		public virtual SafeFileHandle SafeFileHandle
		{
			get
			{
				SafeFileHandle result = ((safeHandle == null) ? new SafeFileHandle(handle, owner) : safeHandle);
				FlushBuffer();
				return result;
			}
		}

		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access) instead")]
		public FileStream(IntPtr handle, FileAccess access)
			: this(handle, access, true, 8192, false)
		{
		}

		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access) instead")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle)
			: this(handle, access, ownsHandle, 8192, false)
		{
		}

		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) instead")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize)
			: this(handle, access, ownsHandle, bufferSize, false)
		{
		}

		[Obsolete("Use FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync)
			: this(handle, access, ownsHandle, bufferSize, isAsync, false)
		{
		}

		internal FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync, bool noBuffering)
		{
			this.handle = MonoIO.InvalidHandle;
			if (handle == this.handle)
			{
				throw new ArgumentException("handle", Locale.GetText("Invalid."));
			}
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("access");
			}
			MonoIOError error;
			MonoFileType fileType = MonoIO.GetFileType(handle, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(name, error);
			}
			switch (fileType)
			{
			case MonoFileType.Unknown:
				throw new IOException("Invalid handle.");
			case MonoFileType.Disk:
				canseek = true;
				break;
			default:
				canseek = false;
				break;
			}
			this.handle = handle;
			this.access = access;
			owner = ownsHandle;
			async = isAsync;
			anonymous = false;
			InitBuffer(bufferSize, noBuffering);
			if (canseek)
			{
				buf_start = MonoIO.Seek(handle, 0L, SeekOrigin.Current, out error);
				if (error != MonoIOError.ERROR_SUCCESS)
				{
					throw MonoIO.GetException(name, error);
				}
			}
			append_startpos = 0L;
		}

		public FileStream(string path, FileMode mode)
			: this(path, mode, (mode != FileMode.Append) ? FileAccess.ReadWrite : FileAccess.Write, FileShare.Read, 8192, false, FileOptions.None)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access)
			: this(path, mode, access, (access != FileAccess.Write) ? FileShare.Read : FileShare.None, 8192, false, false)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share)
			: this(path, mode, access, share, 8192, false, FileOptions.None)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize)
			: this(path, mode, access, share, bufferSize, false, FileOptions.None)
		{
		}

		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
			: this(path, mode, access, share, bufferSize, useAsync, FileOptions.None)
		{
		}

		internal FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool isAsync, bool anonymous)
			: this(path, mode, access, share, bufferSize, anonymous, isAsync ? FileOptions.Asynchronous : FileOptions.None)
		{
		}

		internal FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool anonymous, FileOptions options)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path is empty");
			}
			share &= ~FileShare.Inheritable;
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Positive number required.");
			}
			if (mode < FileMode.CreateNew || mode > FileMode.Append)
			{
				if (anonymous)
				{
					throw new ArgumentException("mode", "Enum value was out of legal range.");
				}
				throw new ArgumentOutOfRangeException("mode", "Enum value was out of legal range.");
			}
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				if (anonymous)
				{
					throw new IsolatedStorageException("Enum value for FileAccess was out of legal range.");
				}
				throw new ArgumentOutOfRangeException("access", "Enum value was out of legal range.");
			}
			if ((share < FileShare.None) || share > (FileShare.ReadWrite | FileShare.Delete))
			{
				if (anonymous)
				{
					throw new IsolatedStorageException("Enum value for FileShare was out of legal range.");
				}
				throw new ArgumentOutOfRangeException("share", "Enum value was out of legal range.");
			}
			if (path.IndexOfAny(Path.InvalidPathChars) != -1)
			{
				throw new ArgumentException("Name has invalid chars");
			}
			if (Directory.Exists(path))
			{
				string text = Locale.GetText("Access to the path '{0}' is denied.");
				throw new UnauthorizedAccessException(string.Format(text, GetSecureFileName(path, false)));
			}
			if (mode == FileMode.Append && (access & FileAccess.Read) == FileAccess.Read)
			{
				throw new ArgumentException("Append access can be requested only in write-only mode.");
			}
			if ((access & FileAccess.Write) == 0 && mode != FileMode.Open && mode != FileMode.OpenOrCreate)
			{
				string text2 = Locale.GetText("Combining FileMode: {0} with FileAccess: {1} is invalid.");
				throw new ArgumentException(string.Format(text2, access, mode));
			}
			string text3 = ((Path.DirectorySeparatorChar == '/' || path.IndexOf('/') < 0) ? Path.GetDirectoryName(path) : Path.GetDirectoryName(Path.GetFullPath(path)));
			if (text3.Length > 0)
			{
				string fullPath = Path.GetFullPath(text3);
				if (!Directory.Exists(fullPath))
				{
					string text4 = Locale.GetText("Could not find a part of the path \"{0}\".");
					throw new IsolatedStorageException(string.Format(text4, (!anonymous) ? Path.GetFullPath(path) : text3));
				}
			}
			if (access == FileAccess.Read && mode != FileMode.Create && mode != FileMode.OpenOrCreate && mode != FileMode.CreateNew && !File.Exists(path))
			{
				string text5 = Locale.GetText("Could not find file \"{0}\".");
				string secureFileName = GetSecureFileName(path);
				throw new IsolatedStorageException(string.Format(text5, secureFileName));
			}
			if (!anonymous)
			{
				name = path;
			}
			MonoIOError error;
			handle = MonoIO.Open(path, mode, access, share, options, out error);
			if (handle == MonoIO.InvalidHandle)
			{
				throw MonoIO.GetException(GetSecureFileName(path), error);
			}
			this.access = access;
			owner = true;
			this.anonymous = anonymous;
			if (MonoIO.GetFileType(handle, out error) == MonoFileType.Disk)
			{
				canseek = true;
				async = (options & FileOptions.Asynchronous) != 0;
			}
			else
			{
				canseek = false;
				async = false;
			}
			if (access == FileAccess.Read && canseek && bufferSize == 8192)
			{
				long length = Length;
				if (bufferSize > length)
				{
					bufferSize = (int)((length >= 1000) ? length : 1000);
				}
			}
			InitBuffer(bufferSize, false);
			if (mode == FileMode.Append)
			{
				Seek(0L, SeekOrigin.End);
				append_startpos = Position;
			}
			else
			{
				append_startpos = 0L;
			}
		}

		public override int ReadByte()
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (buf_size == 0)
			{
				if (ReadData(handle, buf, 0, 1) == 0)
				{
					return -1;
				}
				return buf[0];
			}
			if (buf_offset >= buf_length)
			{
				RefillBuffer();
				if (buf_length == 0)
				{
					return -1;
				}
			}
			return buf[buf_offset++];
		}

		public override void WriteByte(byte value)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing");
			}
			if (buf_offset == buf_size)
			{
				FlushBuffer();
			}
			if (buf_size == 0)
			{
				buf[0] = value;
				buf_dirty = true;
				buf_length = 1;
				FlushBuffer();
				return;
			}
			buf[buf_offset++] = value;
			if (buf_offset > buf_length)
			{
				buf_length = buf_offset;
			}
			buf_dirty = true;
		}

		public override int Read([In][Out] byte[] array, int offset, int count)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (!CanRead)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			int num = array.Length;
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > num)
			{
				throw new ArgumentException("destination offset is beyond array size");
			}
			if (offset > num - count)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			if (async)
			{
				IAsyncResult asyncResult = BeginRead(array, offset, count, null, null);
				return EndRead(asyncResult);
			}
			return ReadInternal(array, offset, count);
		}

		private int ReadInternal(byte[] dest, int offset, int count)
		{
			int num = 0;
			int num2 = ReadSegment(dest, offset, count);
			num += num2;
			count -= num2;
			if (count == 0)
			{
				return num;
			}
			if (count > buf_size)
			{
				FlushBuffer();
				num2 = ReadData(handle, dest, offset + num, count);
				buf_start += num2;
			}
			else
			{
				RefillBuffer();
				num2 = ReadSegment(dest, offset + num, count);
			}
			return num + num2;
		}

		public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!CanRead)
			{
				throw new NotSupportedException("This stream does not support reading");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (numBytes < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (numBytes > array.Length - offset)
			{
				throw new ArgumentException("Buffer too small. numBytes/offset wrong.");
			}
			if (!async)
			{
				return base.BeginRead(array, offset, numBytes, userCallback, stateObject);
			}
			ReadDelegate readDelegate = ReadInternal;
			return readDelegate.BeginInvoke(array, offset, numBytes, userCallback, stateObject);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!async)
			{
				return base.EndRead(asyncResult);
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			ReadDelegate readDelegate = asyncResult2.AsyncDelegate as ReadDelegate;
			if (readDelegate == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			return readDelegate.EndInvoke(asyncResult);
		}

		public override void Write(byte[] array, int offset, int count)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (offset > array.Length - count)
			{
				throw new ArgumentException("Reading would overrun buffer");
			}
			if (!CanWrite)
			{
				throw new NotSupportedException("Stream does not support writing");
			}
			if (async)
			{
				IAsyncResult asyncResult = BeginWrite(array, offset, count, null, null);
				EndWrite(asyncResult);
			}
			else
			{
				WriteInternal(array, offset, count);
			}
		}

		private void WriteInternal(byte[] src, int offset, int count)
		{
			if (count > buf_size)
			{
				FlushBuffer();
				int num = count;
				while (num > 0)
				{
					MonoIOError error;
					int num2 = MonoIO.Write(handle, src, offset, num, out error);
					if (error != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(GetSecureFileName(name), error);
					}
					num -= num2;
					offset += num2;
				}
				buf_start += count;
				return;
			}
			int num3 = 0;
			while (count > 0)
			{
				int num4 = WriteSegment(src, offset + num3, count);
				num3 += num4;
				count -= num4;
				if (count == 0)
				{
					break;
				}
				FlushBuffer();
			}
		}

		public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!CanWrite)
			{
				throw new NotSupportedException("This stream does not support writing");
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (numBytes < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", "Must be >= 0");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Must be >= 0");
			}
			if (numBytes > array.Length - offset)
			{
				throw new ArgumentException("array too small. numBytes/offset wrong.");
			}
			if (!async)
			{
				return base.BeginWrite(array, offset, numBytes, userCallback, stateObject);
			}
			FileStreamAsyncResult fileStreamAsyncResult = new FileStreamAsyncResult(userCallback, stateObject);
			fileStreamAsyncResult.BytesRead = -1;
			fileStreamAsyncResult.Count = numBytes;
			fileStreamAsyncResult.OriginalCount = numBytes;
			if (buf_dirty)
			{
				MemoryStream memoryStream = new MemoryStream();
				FlushBuffer(memoryStream);
				memoryStream.Write(array, offset, numBytes);
				offset = 0;
				numBytes = (int)memoryStream.Length;
			}
			WriteDelegate writeDelegate = WriteInternal;
			return writeDelegate.BeginInvoke(array, offset, numBytes, userCallback, stateObject);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!async)
			{
				base.EndWrite(asyncResult);
				return;
			}
			AsyncResult asyncResult2 = asyncResult as AsyncResult;
			if (asyncResult2 == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			WriteDelegate writeDelegate = asyncResult2.AsyncDelegate as WriteDelegate;
			if (writeDelegate == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			writeDelegate.EndInvoke(asyncResult);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!CanSeek)
			{
				throw new NotSupportedException("The stream does not support seeking");
			}
			long num;
			switch (origin)
			{
			case SeekOrigin.End:
				num = Length + offset;
				break;
			case SeekOrigin.Current:
				num = Position + offset;
				break;
			case SeekOrigin.Begin:
				num = offset;
				break;
			default:
				throw new ArgumentException("origin", "Invalid SeekOrigin");
			}
			if (num < 0)
			{
				throw new IOException("Attempted to Seek before the beginning of the stream");
			}
			if (num < append_startpos)
			{
				throw new IOException("Can't seek back over pre-existing data in append mode");
			}
			FlushBuffer();
			MonoIOError error;
			buf_start = MonoIO.Seek(handle, num, SeekOrigin.Begin, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(GetSecureFileName(name), error);
			}
			return buf_start;
		}

		public override void SetLength(long value)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (!CanSeek)
			{
				throw new NotSupportedException("The stream does not support seeking");
			}
			if (!CanWrite)
			{
				throw new NotSupportedException("The stream does not support writing");
			}
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("value is less than 0");
			}
			Flush();
			MonoIOError error;
			MonoIO.SetLength(handle, value, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(GetSecureFileName(name), error);
			}
			if (Position > value)
			{
				Position = value;
			}
		}

		public override void Flush()
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			FlushBuffer();
		}

		public virtual void Lock(long position, long length)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (position < 0)
			{
				throw new ArgumentOutOfRangeException("position must not be negative");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length must not be negative");
			}
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			MonoIOError error;
			MonoIO.Lock(handle, position, length, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(GetSecureFileName(name), error);
			}
		}

		public virtual void Unlock(long position, long length)
		{
			if (handle == MonoIO.InvalidHandle)
			{
				throw new ObjectDisposedException("Stream has been closed");
			}
			if (position < 0)
			{
				throw new ArgumentOutOfRangeException("position must not be negative");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length must not be negative");
			}
			MonoIOError error;
			MonoIO.Unlock(handle, position, length, out error);
			if (error != MonoIOError.ERROR_SUCCESS)
			{
				throw MonoIO.GetException(GetSecureFileName(name), error);
			}
		}

		~FileStream()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			Exception ex = null;
			if (handle != MonoIO.InvalidHandle)
			{
				try
				{
					FlushBuffer();
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				if (owner)
				{
					MonoIOError error;
					MonoIO.Close(handle, out error);
					if (error != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(GetSecureFileName(name), error);
					}
					handle = MonoIO.InvalidHandle;
				}
			}
			canseek = false;
			access = (FileAccess)0;
			if (disposing)
			{
				buf = null;
			}
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
			if (ex != null)
			{
				throw ex;
			}
		}

		private int ReadSegment(byte[] dest, int dest_offset, int count)
		{
			if (count > buf_length - buf_offset)
			{
				count = buf_length - buf_offset;
			}
			if (count > 0)
			{
				Buffer.BlockCopy(buf, buf_offset, dest, dest_offset, count);
				buf_offset += count;
			}
			return count;
		}

		private int WriteSegment(byte[] src, int src_offset, int count)
		{
			if (count > buf_size - buf_offset)
			{
				count = buf_size - buf_offset;
			}
			if (count > 0)
			{
				Buffer.BlockCopy(src, src_offset, buf, buf_offset, count);
				buf_offset += count;
				if (buf_offset > buf_length)
				{
					buf_length = buf_offset;
				}
				buf_dirty = true;
			}
			return count;
		}

		private void FlushBuffer(Stream st)
		{
			if (buf_dirty)
			{
				MonoIOError error;
				if (CanSeek)
				{
					MonoIO.Seek(handle, buf_start, SeekOrigin.Begin, out error);
					if (error != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(GetSecureFileName(name), error);
					}
				}
				if (st == null)
				{
					MonoIO.Write(handle, buf, 0, buf_length, out error);
					if (error != MonoIOError.ERROR_SUCCESS)
					{
						throw MonoIO.GetException(GetSecureFileName(name), error);
					}
				}
				else
				{
					st.Write(buf, 0, buf_length);
				}
			}
			buf_start += buf_offset;
			buf_offset = (buf_length = 0);
			buf_dirty = false;
		}

		private void FlushBuffer()
		{
			FlushBuffer(null);
		}

		private void FlushBufferIfDirty()
		{
			if (buf_dirty)
			{
				FlushBuffer(null);
			}
		}

		private void RefillBuffer()
		{
			FlushBuffer(null);
			buf_length = ReadData(handle, buf, 0, buf_size);
		}

		private int ReadData(IntPtr handle, byte[] buf, int offset, int count)
		{
			int num = 0;
			MonoIOError error;
			num = MonoIO.Read(handle, buf, offset, count, out error);
			switch (error)
			{
			case MonoIOError.ERROR_BROKEN_PIPE:
				num = 0;
				break;
			default:
				throw MonoIO.GetException(GetSecureFileName(name), error);
			case MonoIOError.ERROR_SUCCESS:
				break;
			}
			if (num == -1)
			{
				throw new IOException();
			}
			return num;
		}

		private void InitBuffer(int size, bool noBuffering)
		{
			if (noBuffering)
			{
				size = 0;
				buf = new byte[1];
			}
			else
			{
				if (size <= 0)
				{
					throw new ArgumentOutOfRangeException("bufferSize", "Positive number required.");
				}
				if (size < 8)
				{
					size = 8;
				}
				buf = new byte[size];
			}
			buf_size = size;
			buf_start = 0L;
			buf_offset = (buf_length = 0);
			buf_dirty = false;
		}

		private string GetSecureFileName(string filename)
		{
			return (!anonymous) ? Path.GetFullPath(filename) : Path.GetFileName(filename);
		}

		private string GetSecureFileName(string filename, bool full)
		{
			return anonymous ? Path.GetFileName(filename) : ((!full) ? filename : Path.GetFullPath(filename));
		}
	}
}
