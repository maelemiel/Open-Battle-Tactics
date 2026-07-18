using System.Runtime.InteropServices;
using System.Text;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class Rfc2898DeriveBytes : DeriveBytes
	{
		private const int defaultIterations = 1000;

		private int _iteration;

		private byte[] _salt;

		private HMACSHA1 _hmac;

		private byte[] _buffer;

		private int _pos;

		private int _f;

		public int IterationCount
		{
			get
			{
				return _iteration;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("IterationCount < 1");
				}
				_iteration = value;
			}
		}

		public byte[] Salt
		{
			get
			{
				return (byte[])_salt.Clone();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Salt");
				}
				if (value.Length < 8)
				{
					throw new ArgumentException("Salt < 8 bytes");
				}
				_salt = (byte[])value.Clone();
			}
		}

		public Rfc2898DeriveBytes(string password, byte[] salt)
			: this(password, salt, 1000)
		{
		}

		public Rfc2898DeriveBytes(string password, byte[] salt, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			Salt = salt;
			IterationCount = iterations;
			_hmac = new HMACSHA1(Encoding.UTF8.GetBytes(password));
		}

		public Rfc2898DeriveBytes(byte[] password, byte[] salt, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			Salt = salt;
			IterationCount = iterations;
			_hmac = new HMACSHA1(password);
		}

		public Rfc2898DeriveBytes(string password, int saltSize)
			: this(password, saltSize, 1000)
		{
		}

		public Rfc2898DeriveBytes(string password, int saltSize, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			if (saltSize < 0)
			{
				throw new ArgumentOutOfRangeException("invalid salt length");
			}
			Salt = KeyBuilder.Key(saltSize);
			IterationCount = iterations;
			_hmac = new HMACSHA1(Encoding.UTF8.GetBytes(password));
		}

		private byte[] F(byte[] s, int c, int i)
		{
			s[s.Length - 4] = (byte)(i >> 24);
			s[s.Length - 3] = (byte)(i >> 16);
			s[s.Length - 2] = (byte)(i >> 8);
			s[s.Length - 1] = (byte)i;
			byte[] array = _hmac.ComputeHash(s);
			byte[] buffer = array;
			for (int j = 1; j < c; j++)
			{
				byte[] array2 = _hmac.ComputeHash(buffer);
				for (int k = 0; k < 20; k++)
				{
					array[k] ^= array2[k];
				}
				buffer = array2;
			}
			return array;
		}

		public override byte[] GetBytes(int cb)
		{
			if (cb < 1)
			{
				throw new ArgumentOutOfRangeException("cb");
			}
			int num = cb / 20;
			if (cb % 20 != 0)
			{
				num++;
			}
			byte[] array = new byte[cb];
			int num2 = 0;
			if (_pos > 0)
			{
				int num3 = Math.Min(20 - _pos, cb);
				Buffer.BlockCopy(_buffer, _pos, array, 0, num3);
				if (num3 >= cb)
				{
					return array;
				}
				_pos = 0;
				num2 = num3;
			}
			byte[] array2 = new byte[_salt.Length + 4];
			Buffer.BlockCopy(_salt, 0, array2, 0, _salt.Length);
			for (int i = 1; i <= num; i++)
			{
				_buffer = F(array2, _iteration, ++_f);
				int num4 = ((i != num) ? 20 : (array.Length - num2));
				Buffer.BlockCopy(_buffer, _pos, array, num2, num4);
				num2 += _pos + num4;
				_pos = ((num4 != 20) ? num4 : 0);
			}
			return array;
		}

		public override void Reset()
		{
			_buffer = null;
			_pos = 0;
			_f = 0;
		}
	}
}
