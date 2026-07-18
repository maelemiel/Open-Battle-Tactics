using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Mono.Security;
using Mono.Security.Cryptography;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public class StrongNameKeyPair : ISerializable, IDeserializationCallback
	{
		private byte[] _publicKey;

		private string _keyPairContainer;

		private bool _keyPairExported;

		private byte[] _keyPairArray;

		[NonSerialized]
		private RSA _rsa;

		public byte[] PublicKey
		{
			get
			{
				if (_publicKey == null)
				{
					RSA rSA = GetRSA();
					if (rSA == null)
					{
						throw new ArgumentException("invalid keypair");
					}
					byte[] array = CryptoConvert.ToCapiKeyBlob(rSA, false);
					_publicKey = new byte[array.Length + 12];
					_publicKey[0] = 0;
					_publicKey[1] = 36;
					_publicKey[2] = 0;
					_publicKey[3] = 0;
					_publicKey[4] = 4;
					_publicKey[5] = 128;
					_publicKey[6] = 0;
					_publicKey[7] = 0;
					int num = array.Length;
					_publicKey[8] = (byte)(num % 256);
					_publicKey[9] = (byte)(num / 256);
					_publicKey[10] = 0;
					_publicKey[11] = 0;
					Buffer.BlockCopy(array, 0, _publicKey, 12, array.Length);
				}
				return _publicKey;
			}
		}

		public StrongNameKeyPair(byte[] keyPairArray)
		{
			if (keyPairArray == null)
			{
				throw new ArgumentNullException("keyPairArray");
			}
			LoadKey(keyPairArray);
			GetRSA();
		}

		public StrongNameKeyPair(FileStream keyPairFile)
		{
			if (keyPairFile == null)
			{
				throw new ArgumentNullException("keyPairFile");
			}
			byte[] array = new byte[keyPairFile.Length];
			keyPairFile.Read(array, 0, array.Length);
			LoadKey(array);
			GetRSA();
		}

		public StrongNameKeyPair(string keyPairContainer)
		{
			if (keyPairContainer == null)
			{
				throw new ArgumentNullException("keyPairContainer");
			}
			_keyPairContainer = keyPairContainer;
			GetRSA();
		}

		protected StrongNameKeyPair(SerializationInfo info, StreamingContext context)
		{
			_publicKey = (byte[])info.GetValue("_publicKey", typeof(byte[]));
			_keyPairContainer = info.GetString("_keyPairContainer");
			_keyPairExported = info.GetBoolean("_keyPairExported");
			_keyPairArray = (byte[])info.GetValue("_keyPairArray", typeof(byte[]));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("_publicKey", _publicKey, typeof(byte[]));
			info.AddValue("_keyPairContainer", _keyPairContainer);
			info.AddValue("_keyPairExported", _keyPairExported);
			info.AddValue("_keyPairArray", _keyPairArray, typeof(byte[]));
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		private RSA GetRSA()
		{
			if (_rsa != null)
			{
				return _rsa;
			}
			if (_keyPairArray != null)
			{
				try
				{
					_rsa = CryptoConvert.FromCapiKeyBlob(_keyPairArray);
				}
				catch
				{
					_keyPairArray = null;
				}
			}
			return _rsa;
		}

		private void LoadKey(byte[] key)
		{
			try
			{
				if (key.Length == 16)
				{
					int num = 0;
					int num2 = 0;
					while (num < key.Length)
					{
						num2 += key[num++];
					}
					if (num2 == 4)
					{
						_publicKey = (byte[])key.Clone();
					}
				}
				else
				{
					_keyPairArray = key;
				}
			}
			catch
			{
			}
		}

		internal StrongName StrongName()
		{
			RSA rSA = GetRSA();
			if (rSA != null)
			{
				return new StrongName(rSA);
			}
			if (_publicKey != null)
			{
				return new StrongName(_publicKey);
			}
			return null;
		}
	}
}
