using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class PasswordDeriveBytes : DeriveBytes
	{
		private string HashNameValue;

		private byte[] SaltValue;

		private int IterationsValue;

		private HashAlgorithm hash;

		private int state;

		private byte[] password;

		private byte[] initial;

		private byte[] output;

		private int position;

		private int hashnumber;

		public string HashName
		{
			get
			{
				return HashNameValue;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("HashName");
				}
				if (state != 0)
				{
					throw new CryptographicException(Locale.GetText("Can't change this property at this stage"));
				}
				HashNameValue = value;
			}
		}

		public int IterationCount
		{
			get
			{
				return IterationsValue;
			}
			set
			{
				if (value < 1)
				{
					throw new ArgumentOutOfRangeException("> 0", "IterationCount");
				}
				if (state != 0)
				{
					throw new CryptographicException(Locale.GetText("Can't change this property at this stage"));
				}
				IterationsValue = value;
			}
		}

		public byte[] Salt
		{
			get
			{
				if (SaltValue == null)
				{
					return null;
				}
				return (byte[])SaltValue.Clone();
			}
			set
			{
				if (state != 0)
				{
					throw new CryptographicException(Locale.GetText("Can't change this property at this stage"));
				}
				if (value != null)
				{
					SaltValue = (byte[])value.Clone();
				}
				else
				{
					SaltValue = null;
				}
			}
		}

		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt)
		{
			Prepare(strPassword, rgbSalt, "SHA1", 100);
		}

		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, CspParameters cspParams)
		{
			Prepare(strPassword, rgbSalt, "SHA1", 100);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, string strHashName, int iterations)
		{
			Prepare(strPassword, rgbSalt, strHashName, iterations);
		}

		public PasswordDeriveBytes(string strPassword, byte[] rgbSalt, string strHashName, int iterations, CspParameters cspParams)
		{
			Prepare(strPassword, rgbSalt, strHashName, iterations);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt)
		{
			Prepare(password, salt, "SHA1", 100);
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt, CspParameters cspParams)
		{
			Prepare(password, salt, "SHA1", 100);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations)
		{
			Prepare(password, salt, hashName, iterations);
		}

		public PasswordDeriveBytes(byte[] password, byte[] salt, string hashName, int iterations, CspParameters cspParams)
		{
			Prepare(password, salt, hashName, iterations);
			if (cspParams != null)
			{
				throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono for PasswordDeriveBytes."));
			}
		}

		~PasswordDeriveBytes()
		{
			if (initial != null)
			{
				Array.Clear(initial, 0, initial.Length);
				initial = null;
			}
			Array.Clear(password, 0, password.Length);
		}

		private void Prepare(string strPassword, byte[] rgbSalt, string strHashName, int iterations)
		{
			if (strPassword == null)
			{
				throw new ArgumentNullException("strPassword");
			}
			byte[] bytes = Encoding.UTF8.GetBytes(strPassword);
			Prepare(bytes, rgbSalt, strHashName, iterations);
			Array.Clear(bytes, 0, bytes.Length);
		}

		private void Prepare(byte[] password, byte[] rgbSalt, string strHashName, int iterations)
		{
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			this.password = (byte[])password.Clone();
			Salt = rgbSalt;
			HashName = strHashName;
			IterationCount = iterations;
			state = 0;
		}

		public byte[] CryptDeriveKey(string algname, string alghashname, int keySize, byte[] rgbIV)
		{
			if (keySize > 128)
			{
				throw new CryptographicException(Locale.GetText("Key Size can't be greater than 128 bits"));
			}
			throw new NotSupportedException(Locale.GetText("CspParameters not supported by Mono"));
		}

		[Obsolete("see Rfc2898DeriveBytes for PKCS#5 v2 support")]
		public override byte[] GetBytes(int cb)
		{
			if (cb < 1)
			{
				throw new IndexOutOfRangeException("cb");
			}
			if (state == 0)
			{
				Reset();
				state = 1;
			}
			byte[] array = new byte[cb];
			int num = 0;
			int num2 = Math.Max(1, IterationsValue - 1);
			if (output == null)
			{
				output = initial;
				for (int i = 0; i < num2 - 1; i++)
				{
					output = hash.ComputeHash(output);
				}
			}
			while (num < cb)
			{
				byte[] array2 = null;
				if (hashnumber == 0)
				{
					array2 = hash.ComputeHash(output);
				}
				else
				{
					if (hashnumber >= 1000)
					{
						throw new CryptographicException(Locale.GetText("too long"));
					}
					string text = Convert.ToString(hashnumber);
					array2 = new byte[output.Length + text.Length];
					for (int j = 0; j < text.Length; j++)
					{
						array2[j] = (byte)text[j];
					}
					Buffer.BlockCopy(output, 0, array2, text.Length, output.Length);
					array2 = hash.ComputeHash(array2);
				}
				int val = array2.Length - position;
				int num3 = Math.Min(cb - num, val);
				Buffer.BlockCopy(array2, position, array, num, num3);
				num += num3;
				position += num3;
				while (position >= array2.Length)
				{
					position -= array2.Length;
					hashnumber++;
				}
			}
			return array;
		}

		public override void Reset()
		{
			state = 0;
			position = 0;
			hashnumber = 0;
			hash = HashAlgorithm.Create(HashNameValue);
			if (SaltValue != null)
			{
				hash.TransformBlock(password, 0, password.Length, password, 0);
				hash.TransformFinalBlock(SaltValue, 0, SaltValue.Length);
				initial = hash.Hash;
			}
			else
			{
				initial = hash.ComputeHash(password);
			}
		}
	}
}
