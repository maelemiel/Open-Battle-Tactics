using System;
using System.IO;
using System.Security.Cryptography;

namespace Mono.Security.Authenticode
{
	internal class AuthenticodeBase
	{
		public const string spcIndirectDataContext = "1.3.6.1.4.1.311.2.1.4";

		private byte[] fileblock;

		private FileStream fs;

		private int blockNo;

		private int blockLength;

		private int peOffset;

		private int dirSecurityOffset;

		private int dirSecuritySize;

		private int coffSymbolTableOffset;

		internal int PEOffset
		{
			get
			{
				if (blockNo < 1)
				{
					ReadFirstBlock();
				}
				return peOffset;
			}
		}

		internal int CoffSymbolTableOffset
		{
			get
			{
				if (blockNo < 1)
				{
					ReadFirstBlock();
				}
				return coffSymbolTableOffset;
			}
		}

		internal int SecurityOffset
		{
			get
			{
				if (blockNo < 1)
				{
					ReadFirstBlock();
				}
				return dirSecurityOffset;
			}
		}

		public AuthenticodeBase()
		{
			fileblock = new byte[4096];
		}

		internal void Open(string filename)
		{
			if (fs != null)
			{
				Close();
			}
			fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		internal void Close()
		{
			if (fs != null)
			{
				fs.Close();
				fs = null;
				blockNo = 0;
			}
		}

		internal bool ReadFirstBlock()
		{
			if (fs == null)
			{
				return false;
			}
			fs.Position = 0L;
			blockLength = fs.Read(fileblock, 0, fileblock.Length);
			blockNo = 1;
			if (blockLength < 64)
			{
				return false;
			}
			if (BitConverterLE.ToUInt16(fileblock, 0) != 23117)
			{
				return false;
			}
			peOffset = BitConverterLE.ToInt32(fileblock, 60);
			if (peOffset > fileblock.Length)
			{
				string message = string.Format(Locale.GetText("Header size too big (> {0} bytes)."), fileblock.Length);
				throw new NotSupportedException(message);
			}
			if (peOffset > fs.Length)
			{
				return false;
			}
			if (BitConverterLE.ToUInt32(fileblock, peOffset) != 17744)
			{
				return false;
			}
			dirSecurityOffset = BitConverterLE.ToInt32(fileblock, peOffset + 152);
			dirSecuritySize = BitConverterLE.ToInt32(fileblock, peOffset + 156);
			coffSymbolTableOffset = BitConverterLE.ToInt32(fileblock, peOffset + 12);
			return true;
		}

		internal byte[] GetSecurityEntry()
		{
			if (blockNo < 1)
			{
				ReadFirstBlock();
			}
			if (dirSecuritySize > 8)
			{
				byte[] array = new byte[dirSecuritySize - 8];
				fs.Position = dirSecurityOffset + 8;
				fs.Read(array, 0, array.Length);
				return array;
			}
			return null;
		}

		internal byte[] GetHash(HashAlgorithm hash)
		{
			if (blockNo < 1)
			{
				ReadFirstBlock();
			}
			fs.Position = blockLength;
			int num = 0;
			long num2;
			if (dirSecurityOffset > 0)
			{
				if (dirSecurityOffset < blockLength)
				{
					blockLength = dirSecurityOffset;
					num2 = 0L;
				}
				else
				{
					num2 = dirSecurityOffset - blockLength;
				}
			}
			else if (coffSymbolTableOffset > 0)
			{
				fileblock[PEOffset + 12] = 0;
				fileblock[PEOffset + 13] = 0;
				fileblock[PEOffset + 14] = 0;
				fileblock[PEOffset + 15] = 0;
				fileblock[PEOffset + 16] = 0;
				fileblock[PEOffset + 17] = 0;
				fileblock[PEOffset + 18] = 0;
				fileblock[PEOffset + 19] = 0;
				if (coffSymbolTableOffset < blockLength)
				{
					blockLength = coffSymbolTableOffset;
					num2 = 0L;
				}
				else
				{
					num2 = coffSymbolTableOffset - blockLength;
				}
			}
			else
			{
				num = (int)(fs.Length & 7);
				if (num > 0)
				{
					num = 8 - num;
				}
				num2 = fs.Length - blockLength;
			}
			int num3 = peOffset + 88;
			hash.TransformBlock(fileblock, 0, num3, fileblock, 0);
			num3 += 4;
			hash.TransformBlock(fileblock, num3, 60, fileblock, num3);
			num3 += 68;
			if (num2 == 0L)
			{
				hash.TransformFinalBlock(fileblock, num3, blockLength - num3);
			}
			else
			{
				hash.TransformBlock(fileblock, num3, blockLength - num3, fileblock, num3);
				long num4 = num2 >> 12;
				int num5 = (int)(num2 - (num4 << 12));
				if (num5 == 0)
				{
					num4--;
					num5 = 4096;
				}
				while (num4-- > 0)
				{
					fs.Read(fileblock, 0, fileblock.Length);
					hash.TransformBlock(fileblock, 0, fileblock.Length, fileblock, 0);
				}
				if (fs.Read(fileblock, 0, num5) != num5)
				{
					return null;
				}
				if (num > 0)
				{
					hash.TransformBlock(fileblock, 0, num5, fileblock, 0);
					hash.TransformFinalBlock(new byte[num], 0, num);
				}
				else
				{
					hash.TransformFinalBlock(fileblock, 0, num5);
				}
			}
			return hash.Hash;
		}

		protected byte[] HashFile(string fileName, string hashName)
		{
			try
			{
				Open(fileName);
				HashAlgorithm hash = HashAlgorithm.Create(hashName);
				byte[] hash2 = GetHash(hash);
				Close();
				return hash2;
			}
			catch
			{
				return null;
			}
		}
	}
}
