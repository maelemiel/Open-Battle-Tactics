using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.Protocol.Ntlm
{
	public class ChallengeResponse : IDisposable
	{
		private static byte[] magic = new byte[8] { 75, 71, 83, 33, 64, 35, 36, 37 };

		private static byte[] nullEncMagic = new byte[8] { 170, 211, 180, 53, 181, 20, 4, 238 };

		private bool _disposed;

		private byte[] _challenge;

		private byte[] _lmpwd;

		private byte[] _ntpwd;

		public string Password
		{
			get
			{
				return null;
			}
			set
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("too late");
				}
				DES dES = DES.Create();
				dES.Mode = CipherMode.ECB;
				ICryptoTransform cryptoTransform = null;
				if (value == null || value.Length < 1)
				{
					Buffer.BlockCopy(nullEncMagic, 0, _lmpwd, 0, 8);
				}
				else
				{
					dES.Key = PasswordToKey(value, 0);
					cryptoTransform = dES.CreateEncryptor();
					cryptoTransform.TransformBlock(magic, 0, 8, _lmpwd, 0);
				}
				if (value == null || value.Length < 8)
				{
					Buffer.BlockCopy(nullEncMagic, 0, _lmpwd, 8, 8);
				}
				else
				{
					dES.Key = PasswordToKey(value, 7);
					cryptoTransform = dES.CreateEncryptor();
					cryptoTransform.TransformBlock(magic, 0, 8, _lmpwd, 8);
				}
				MD4 mD = MD4.Create();
				byte[] array = ((value != null) ? Encoding.Unicode.GetBytes(value) : new byte[0]);
				byte[] array2 = mD.ComputeHash(array);
				Buffer.BlockCopy(array2, 0, _ntpwd, 0, 16);
				Array.Clear(array, 0, array.Length);
				Array.Clear(array2, 0, array2.Length);
				dES.Clear();
			}
		}

		public byte[] Challenge
		{
			get
			{
				return null;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Challenge");
				}
				if (_disposed)
				{
					throw new ObjectDisposedException("too late");
				}
				_challenge = (byte[])value.Clone();
			}
		}

		public byte[] LM
		{
			get
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("too late");
				}
				return GetResponse(_lmpwd);
			}
		}

		public byte[] NT
		{
			get
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("too late");
				}
				return GetResponse(_ntpwd);
			}
		}

		public ChallengeResponse()
		{
			_disposed = false;
			_lmpwd = new byte[21];
			_ntpwd = new byte[21];
		}

		public ChallengeResponse(string password, byte[] challenge)
			: this()
		{
			Password = password;
			Challenge = challenge;
		}

		~ChallengeResponse()
		{
			if (!_disposed)
			{
				Dispose();
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				Array.Clear(_lmpwd, 0, _lmpwd.Length);
				Array.Clear(_ntpwd, 0, _ntpwd.Length);
				if (_challenge != null)
				{
					Array.Clear(_challenge, 0, _challenge.Length);
				}
				_disposed = true;
			}
		}

		private byte[] GetResponse(byte[] pwd)
		{
			byte[] array = new byte[24];
			DES dES = DES.Create();
			dES.Mode = CipherMode.ECB;
			dES.Key = PrepareDESKey(pwd, 0);
			ICryptoTransform cryptoTransform = dES.CreateEncryptor();
			cryptoTransform.TransformBlock(_challenge, 0, 8, array, 0);
			dES.Key = PrepareDESKey(pwd, 7);
			cryptoTransform = dES.CreateEncryptor();
			cryptoTransform.TransformBlock(_challenge, 0, 8, array, 8);
			dES.Key = PrepareDESKey(pwd, 14);
			cryptoTransform = dES.CreateEncryptor();
			cryptoTransform.TransformBlock(_challenge, 0, 8, array, 16);
			return array;
		}

		private byte[] PrepareDESKey(byte[] key56bits, int position)
		{
			return new byte[8]
			{
				key56bits[position],
				(byte)((key56bits[position] << 7) | (key56bits[position + 1] >> 1)),
				(byte)((key56bits[position + 1] << 6) | (key56bits[position + 2] >> 2)),
				(byte)((key56bits[position + 2] << 5) | (key56bits[position + 3] >> 3)),
				(byte)((key56bits[position + 3] << 4) | (key56bits[position + 4] >> 4)),
				(byte)((key56bits[position + 4] << 3) | (key56bits[position + 5] >> 5)),
				(byte)((key56bits[position + 5] << 2) | (key56bits[position + 6] >> 6)),
				(byte)(key56bits[position + 6] << 1)
			};
		}

		private byte[] PasswordToKey(string password, int position)
		{
			byte[] array = new byte[7];
			int charCount = System.Math.Min(password.Length - position, 7);
			Encoding.ASCII.GetBytes(password.ToUpper(CultureInfo.CurrentCulture), position, charCount, array, 0);
			byte[] result = PrepareDESKey(array, 0);
			Array.Clear(array, 0, array.Length);
			return result;
		}
	}
}
