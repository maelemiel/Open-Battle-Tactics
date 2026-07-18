using System.Runtime.ConstrainedExecution;

namespace System.Security
{
	[MonoTODO("work in progress - encryption is missing")]
	public sealed class SecureString : CriticalFinalizerObject, IDisposable
	{
		private const int BlockSize = 16;

		private const int MaxSize = 65536;

		private int length;

		private bool disposed;

		private bool read_only;

		private byte[] data;

		public int Length
		{
			get
			{
				if (disposed)
				{
					throw new ObjectDisposedException("SecureString");
				}
				return length;
			}
		}

		public SecureString()
		{
			Alloc(8, false);
		}

		[CLSCompliant(false)]
		public unsafe SecureString(char* value, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (length < 0 || length > 65536)
			{
				throw new ArgumentOutOfRangeException("length", "< 0 || > 65536");
			}
			this.length = length;
			Alloc(length, false);
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				char c = *(value++);
				data[num++] = (byte)((int)c >> 8);
				data[num++] = (byte)c;
			}
			Encrypt();
		}

		static SecureString()
		{
		}

		public void AppendChar(char c)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("SecureString");
			}
			if (read_only)
			{
				throw new InvalidOperationException(Locale.GetText("SecureString is read-only."));
			}
			if (length == 65536)
			{
				throw new ArgumentOutOfRangeException("length", "> 65536");
			}
			try
			{
				Decrypt();
				int num = length * 2;
				Alloc(++length, true);
				data[num++] = (byte)((int)c >> 8);
				data[num++] = (byte)c;
			}
			finally
			{
				Encrypt();
			}
		}

		public void Clear()
		{
			if (disposed)
			{
				throw new ObjectDisposedException("SecureString");
			}
			if (read_only)
			{
				throw new InvalidOperationException(Locale.GetText("SecureString is read-only."));
			}
			Array.Clear(data, 0, data.Length);
			length = 0;
		}

		public SecureString Copy()
		{
			SecureString secureString = new SecureString();
			secureString.data = (byte[])data.Clone();
			secureString.length = length;
			return secureString;
		}

		public void Dispose()
		{
			disposed = true;
			if (data != null)
			{
				Array.Clear(data, 0, data.Length);
				data = null;
			}
			length = 0;
		}

		public void InsertAt(int index, char c)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("SecureString");
			}
			if (read_only)
			{
				throw new InvalidOperationException(Locale.GetText("SecureString is read-only."));
			}
			if (index < 0 || index > length)
			{
				throw new ArgumentOutOfRangeException("index", "< 0 || > length");
			}
			if (length >= 65536)
			{
				string text = Locale.GetText("Maximum string size is '{0}'.", 65536);
				throw new ArgumentOutOfRangeException("index", text);
			}
			try
			{
				Decrypt();
				Alloc(++length, true);
				int num = index * 2;
				Buffer.BlockCopy(data, num, data, num + 2, data.Length - num - 2);
				data[num++] = (byte)((int)c >> 8);
				data[num] = (byte)c;
			}
			finally
			{
				Encrypt();
			}
		}

		public bool IsReadOnly()
		{
			if (disposed)
			{
				throw new ObjectDisposedException("SecureString");
			}
			return read_only;
		}

		public void MakeReadOnly()
		{
			read_only = true;
		}

		public void RemoveAt(int index)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("SecureString");
			}
			if (read_only)
			{
				throw new InvalidOperationException(Locale.GetText("SecureString is read-only."));
			}
			if (index < 0 || index >= length)
			{
				throw new ArgumentOutOfRangeException("index", "< 0 || > length");
			}
			try
			{
				Decrypt();
				Buffer.BlockCopy(data, index + 1, data, index, data.Length - index - 1);
				Alloc(--length, true);
			}
			finally
			{
				Encrypt();
			}
		}

		public void SetAt(int index, char c)
		{
			if (disposed)
			{
				throw new ObjectDisposedException("SecureString");
			}
			if (read_only)
			{
				throw new InvalidOperationException(Locale.GetText("SecureString is read-only."));
			}
			if (index < 0 || index >= length)
			{
				throw new ArgumentOutOfRangeException("index", "< 0 || > length");
			}
			try
			{
				Decrypt();
				int num = index * 2;
				data[num++] = (byte)((int)c >> 8);
				data[num] = (byte)c;
			}
			finally
			{
				Encrypt();
			}
		}

		private void Encrypt()
		{
			if (data != null && data.Length <= 0)
			{
			}
		}

		private void Decrypt()
		{
			if (data != null && data.Length <= 0)
			{
			}
		}

		private void Alloc(int length, bool realloc)
		{
			if (length < 0 || length > 65536)
			{
				throw new ArgumentOutOfRangeException("length", "< 0 || > 65536");
			}
			int num = (length >> 3) + (((length & 7) != 0) ? 1 : 0) << 4;
			if (!realloc || data == null || num != data.Length)
			{
				if (realloc)
				{
					byte[] array = new byte[num];
					Array.Copy(data, 0, array, 0, Math.Min(data.Length, array.Length));
					Array.Clear(data, 0, data.Length);
					data = array;
				}
				else
				{
					data = new byte[num];
				}
			}
		}

		internal byte[] GetBuffer()
		{
			byte[] array = new byte[length << 1];
			try
			{
				Decrypt();
				Buffer.BlockCopy(data, 0, array, 0, array.Length);
				return array;
			}
			finally
			{
				Encrypt();
			}
		}
	}
}
