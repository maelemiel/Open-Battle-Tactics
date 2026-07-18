using System.Runtime.InteropServices;

namespace System.IO
{
	[CLSCompliant(false)]
	public class UnmanagedMemoryStream : Stream
	{
		private long length;

		private bool closed;

		private long capacity;

		private FileAccess fileaccess;

		private IntPtr initial_pointer;

		private long initial_position;

		private long current_position;

		public override bool CanRead
		{
			get
			{
				return !closed && fileaccess != FileAccess.Write;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return !closed;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !closed && fileaccess != FileAccess.Read;
			}
		}

		public long Capacity
		{
			get
			{
				if (closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				return capacity;
			}
		}

		public override long Length
		{
			get
			{
				if (closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				return length;
			}
		}

		public override long Position
		{
			get
			{
				if (closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				return current_position;
			}
			set
			{
				if (closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Non-negative number required.");
				}
				if (value > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("value", "The position is larger than Int32.MaxValue.");
				}
				current_position = value;
			}
		}

		[CLSCompliant(false)]
		public unsafe byte* PositionPointer
		{
			get
			{
				if (closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				if (current_position >= length)
				{
					throw new IndexOutOfRangeException("value");
				}
				return (byte*)(void*)initial_pointer + current_position;
			}
			set
			{
				if (closed)
				{
					throw new ObjectDisposedException("The stream is closed");
				}
				if (value < (void*)initial_pointer)
				{
					throw new IOException("Address is below the inital address");
				}
				Position = (long)(IntPtr)(value - (ulong)(void*)initial_pointer);
			}
		}

		internal event EventHandler Closed;

		protected UnmanagedMemoryStream()
		{
			closed = true;
		}

		public unsafe UnmanagedMemoryStream(byte* pointer, long length)
		{
			Initialize(pointer, length, length, FileAccess.Read);
		}

		public unsafe UnmanagedMemoryStream(byte* pointer, long length, long capacity, FileAccess access)
		{
			Initialize(pointer, length, capacity, access);
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			if (closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("The length of the buffer array minus the offset parameter is less than the count parameter");
			}
			if (fileaccess == FileAccess.Write)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (current_position >= length)
			{
				return 0;
			}
			int num = (int)((current_position + count >= length) ? (length - current_position) : count);
			Marshal.Copy(new IntPtr(initial_pointer.ToInt64() + current_position), buffer, offset, num);
			current_position += num;
			return num;
		}

		public override int ReadByte()
		{
			if (closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (fileaccess == FileAccess.Write)
			{
				throw new NotSupportedException("Stream does not support reading");
			}
			if (current_position >= length)
			{
				return -1;
			}
			return Marshal.ReadByte(initial_pointer, (int)current_position++);
		}

		public override long Seek(long offset, SeekOrigin loc)
		{
			if (closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			long num;
			switch (loc)
			{
			case SeekOrigin.Begin:
				if (offset < 0)
				{
					throw new IOException("An attempt was made to seek before the beginning of the stream");
				}
				num = initial_position;
				break;
			case SeekOrigin.Current:
				num = current_position;
				break;
			case SeekOrigin.End:
				num = length;
				break;
			default:
				throw new ArgumentException("Invalid SeekOrigin option");
			}
			num += offset;
			if (num < initial_position)
			{
				throw new IOException("An attempt was made to seek before the beginning of the stream");
			}
			current_position = num;
			return current_position;
		}

		public override void SetLength(long value)
		{
			if (closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
			}
			if (value > capacity)
			{
				throw new IOException("Unable to expand length of this stream beyond its capacity.");
			}
			if (fileaccess == FileAccess.Read)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
			length = value;
			if (length < current_position)
			{
				current_position = length;
			}
		}

		public override void Flush()
		{
			if (closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!closed)
			{
				closed = true;
				if (this.Closed != null)
				{
					this.Closed(this, null);
				}
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("The buffer parameter is a null reference");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Non-negative number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Non-negative number required.");
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("The length of the buffer array minus the offset parameter is less than the count parameter");
			}
			if (current_position > capacity - count)
			{
				throw new NotSupportedException("Unable to expand length of this stream beyond its capacity.");
			}
			if (fileaccess == FileAccess.Read)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
			for (int i = 0; i < count; i++)
			{
				Marshal.WriteByte(initial_pointer, (int)current_position++, buffer[offset + i]);
			}
			if (current_position > length)
			{
				length = current_position;
			}
		}

		public override void WriteByte(byte value)
		{
			if (closed)
			{
				throw new ObjectDisposedException("The stream is closed");
			}
			if (current_position == capacity)
			{
				throw new NotSupportedException("The current position is at the end of the capacity of the stream");
			}
			if (fileaccess == FileAccess.Read)
			{
				throw new NotSupportedException("Stream does not support writing.");
			}
			Marshal.WriteByte(initial_pointer, (int)current_position, value);
			current_position++;
			if (current_position > length)
			{
				length = current_position;
			}
		}

		protected unsafe void Initialize(byte* pointer, long length, long capacity, FileAccess access)
		{
			if (pointer == null)
			{
				throw new ArgumentNullException("pointer");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "Non-negative number required.");
			}
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", "Non-negative number required.");
			}
			if (length > capacity)
			{
				throw new ArgumentOutOfRangeException("length", "The length cannot be greater than the capacity.");
			}
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("access", "Enum value was out of legal range.");
			}
			fileaccess = access;
			this.length = length;
			this.capacity = capacity;
			initial_position = 0L;
			current_position = initial_position;
			initial_pointer = new IntPtr(pointer);
			closed = false;
		}
	}
}
