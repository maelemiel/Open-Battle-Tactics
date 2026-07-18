using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.Authenticode
{
	public class PrivateKey
	{
		private const uint magic = 2964713758u;

		private bool encrypted;

		private RSA rsa;

		private bool weak;

		private int keyType;

		public bool Encrypted
		{
			get
			{
				return encrypted;
			}
		}

		public int KeyType
		{
			get
			{
				return keyType;
			}
			set
			{
				keyType = value;
			}
		}

		public RSA RSA
		{
			get
			{
				return rsa;
			}
			set
			{
				rsa = value;
			}
		}

		public bool Weak
		{
			get
			{
				return !encrypted || weak;
			}
			set
			{
				weak = value;
			}
		}

		public PrivateKey()
		{
			keyType = 2;
		}

		public PrivateKey(byte[] data, string password)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!Decode(data, password))
			{
				throw new CryptographicException(global::Locale.GetText("Invalid data and/or password"));
			}
		}

		private byte[] DeriveKey(byte[] salt, string password)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(password);
			SHA1 sHA = SHA1.Create();
			sHA.TransformBlock(salt, 0, salt.Length, salt, 0);
			sHA.TransformFinalBlock(bytes, 0, bytes.Length);
			byte[] array = new byte[16];
			Buffer.BlockCopy(sHA.Hash, 0, array, 0, 16);
			sHA.Clear();
			Array.Clear(bytes, 0, bytes.Length);
			return array;
		}

		private bool Decode(byte[] pvk, string password)
		{
			if (Mono.Security.BitConverterLE.ToUInt32(pvk, 0) != 2964713758u)
			{
				return false;
			}
			if (Mono.Security.BitConverterLE.ToUInt32(pvk, 4) != 0)
			{
				return false;
			}
			keyType = Mono.Security.BitConverterLE.ToInt32(pvk, 8);
			encrypted = Mono.Security.BitConverterLE.ToUInt32(pvk, 12) == 1;
			int num = Mono.Security.BitConverterLE.ToInt32(pvk, 16);
			int num2 = Mono.Security.BitConverterLE.ToInt32(pvk, 20);
			byte[] array = new byte[num2];
			Buffer.BlockCopy(pvk, 24 + num, array, 0, num2);
			if (num > 0)
			{
				if (password == null)
				{
					return false;
				}
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(pvk, 24, array2, 0, num);
				byte[] array3 = DeriveKey(array2, password);
				RC4 rC = RC4.Create();
				ICryptoTransform cryptoTransform = rC.CreateDecryptor(array3, null);
				cryptoTransform.TransformBlock(array, 8, array.Length - 8, array, 8);
				try
				{
					rsa = CryptoConvert.FromCapiPrivateKeyBlob(array);
					weak = false;
				}
				catch (CryptographicException)
				{
					weak = true;
					Buffer.BlockCopy(pvk, 24 + num, array, 0, num2);
					Array.Clear(array3, 5, 11);
					RC4 rC2 = RC4.Create();
					cryptoTransform = rC2.CreateDecryptor(array3, null);
					cryptoTransform.TransformBlock(array, 8, array.Length - 8, array, 8);
					rsa = CryptoConvert.FromCapiPrivateKeyBlob(array);
				}
				Array.Clear(array3, 0, array3.Length);
			}
			else
			{
				weak = true;
				rsa = CryptoConvert.FromCapiPrivateKeyBlob(array);
				Array.Clear(array, 0, array.Length);
			}
			Array.Clear(pvk, 0, pvk.Length);
			return rsa != null;
		}

		public void Save(string filename)
		{
			Save(filename, null);
		}

		public void Save(string filename, string password)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			byte[] array = null;
			FileStream fileStream = File.Open(filename, FileMode.Create, FileAccess.Write);
			try
			{
				byte[] array2 = new byte[4];
				byte[] bytes = Mono.Security.BitConverterLE.GetBytes(2964713758u);
				fileStream.Write(bytes, 0, 4);
				fileStream.Write(array2, 0, 4);
				bytes = Mono.Security.BitConverterLE.GetBytes(keyType);
				fileStream.Write(bytes, 0, 4);
				encrypted = password != null;
				array = CryptoConvert.ToCapiPrivateKeyBlob(rsa);
				if (encrypted)
				{
					bytes = Mono.Security.BitConverterLE.GetBytes(1);
					fileStream.Write(bytes, 0, 4);
					bytes = Mono.Security.BitConverterLE.GetBytes(16);
					fileStream.Write(bytes, 0, 4);
					bytes = Mono.Security.BitConverterLE.GetBytes(array.Length);
					fileStream.Write(bytes, 0, 4);
					byte[] array3 = new byte[16];
					RC4 rC = RC4.Create();
					byte[] array4 = null;
					try
					{
						RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
						randomNumberGenerator.GetBytes(array3);
						fileStream.Write(array3, 0, array3.Length);
						array4 = DeriveKey(array3, password);
						if (Weak)
						{
							Array.Clear(array4, 5, 11);
						}
						ICryptoTransform cryptoTransform = rC.CreateEncryptor(array4, null);
						cryptoTransform.TransformBlock(array, 8, array.Length - 8, array, 8);
					}
					finally
					{
						Array.Clear(array3, 0, array3.Length);
						Array.Clear(array4, 0, array4.Length);
						rC.Clear();
					}
				}
				else
				{
					fileStream.Write(array2, 0, 4);
					fileStream.Write(array2, 0, 4);
					bytes = Mono.Security.BitConverterLE.GetBytes(array.Length);
					fileStream.Write(bytes, 0, 4);
				}
				fileStream.Write(array, 0, array.Length);
			}
			finally
			{
				Array.Clear(array, 0, array.Length);
				fileStream.Close();
			}
		}

		public static PrivateKey CreateFromFile(string filename)
		{
			return CreateFromFile(filename, null);
		}

		public static PrivateKey CreateFromFile(string filename, string password)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			byte[] array = null;
			using (FileStream fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				array = new byte[fileStream.Length];
				fileStream.Read(array, 0, array.Length);
				fileStream.Close();
			}
			return new PrivateKey(array, password);
		}
	}
}
