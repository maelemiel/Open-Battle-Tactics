using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class HMAC : KeyedHashAlgorithm
	{
		private bool _disposed;

		private string _hashName;

		private HashAlgorithm _algo;

		private BlockProcessor _block;

		private int _blockSizeValue;

		protected int BlockSizeValue
		{
			get
			{
				return _blockSizeValue;
			}
			set
			{
				_blockSizeValue = value;
			}
		}

		public string HashName
		{
			get
			{
				return _hashName;
			}
			set
			{
				_hashName = value;
				_algo = HashAlgorithm.Create(_hashName);
			}
		}

		public override byte[] Key
		{
			get
			{
				return (byte[])base.Key.Clone();
			}
			set
			{
				if (value != null && value.Length > 64)
				{
					base.Key = _algo.ComputeHash(value);
				}
				else
				{
					base.Key = (byte[])value.Clone();
				}
			}
		}

		internal BlockProcessor Block
		{
			get
			{
				if (_block == null)
				{
					_block = new BlockProcessor(_algo, BlockSizeValue >> 3);
				}
				return _block;
			}
		}

		protected HMAC()
		{
			_disposed = false;
			_blockSizeValue = 64;
		}

		private byte[] KeySetup(byte[] key, byte padding)
		{
			byte[] array = new byte[BlockSizeValue];
			for (int i = 0; i < key.Length; i++)
			{
				array[i] = (byte)(key[i] ^ padding);
			}
			for (int j = key.Length; j < BlockSizeValue; j++)
			{
				array[j] = padding;
			}
			return array;
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				base.Dispose(disposing);
			}
		}

		protected override void HashCore(byte[] rgb, int ib, int cb)
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("HMACSHA1");
			}
			if (State == 0)
			{
				Initialize();
				State = 1;
			}
			Block.Core(rgb, ib, cb);
		}

		protected override byte[] HashFinal()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("HMAC");
			}
			State = 0;
			Block.Final();
			byte[] hash = _algo.Hash;
			byte[] array = KeySetup(Key, 92);
			_algo.Initialize();
			_algo.TransformBlock(array, 0, array.Length, array, 0);
			_algo.TransformFinalBlock(hash, 0, hash.Length);
			byte[] hash2 = _algo.Hash;
			_algo.Initialize();
			Array.Clear(array, 0, array.Length);
			Array.Clear(hash, 0, hash.Length);
			return hash2;
		}

		public override void Initialize()
		{
			if (_disposed)
			{
				throw new ObjectDisposedException("HMAC");
			}
			State = 0;
			Block.Initialize();
			byte[] array = KeySetup(Key, 54);
			_algo.Initialize();
			Block.Core(array);
			Array.Clear(array, 0, array.Length);
		}

		public new static HMAC Create()
		{
			return Create("System.Security.Cryptography.HMAC");
		}

		public new static HMAC Create(string algorithmName)
		{
			return (HMAC)CryptoConfig.CreateFromName(algorithmName);
		}
	}
}
