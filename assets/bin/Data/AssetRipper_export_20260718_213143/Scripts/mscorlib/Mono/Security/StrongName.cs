using System;
using System.Configuration.Assemblies;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Mono.Security.Cryptography;

namespace Mono.Security
{
	internal sealed class StrongName
	{
		internal class StrongNameSignature
		{
			private byte[] hash;

			private byte[] signature;

			private uint signaturePosition;

			private uint signatureLength;

			private uint metadataPosition;

			private uint metadataLength;

			private byte cliFlag;

			private uint cliFlagPosition;

			public byte[] Hash
			{
				get
				{
					return hash;
				}
				set
				{
					hash = value;
				}
			}

			public byte[] Signature
			{
				get
				{
					return signature;
				}
				set
				{
					signature = value;
				}
			}

			public uint MetadataPosition
			{
				get
				{
					return metadataPosition;
				}
				set
				{
					metadataPosition = value;
				}
			}

			public uint MetadataLength
			{
				get
				{
					return metadataLength;
				}
				set
				{
					metadataLength = value;
				}
			}

			public uint SignaturePosition
			{
				get
				{
					return signaturePosition;
				}
				set
				{
					signaturePosition = value;
				}
			}

			public uint SignatureLength
			{
				get
				{
					return signatureLength;
				}
				set
				{
					signatureLength = value;
				}
			}

			public byte CliFlag
			{
				get
				{
					return cliFlag;
				}
				set
				{
					cliFlag = value;
				}
			}

			public uint CliFlagPosition
			{
				get
				{
					return cliFlagPosition;
				}
				set
				{
					cliFlagPosition = value;
				}
			}
		}

		internal enum StrongNameOptions
		{
			Metadata = 0,
			Signature = 1
		}

		private RSA rsa;

		private byte[] publicKey;

		private byte[] keyToken;

		private string tokenAlgorithm;

		private static object lockObject = new object();

		private static bool initialized = false;

		public bool CanSign
		{
			get
			{
				if (rsa == null)
				{
					return false;
				}
				if (RSA is RSACryptoServiceProvider)
				{
					return !(rsa as RSACryptoServiceProvider).PublicOnly;
				}
				if (RSA is RSAManaged)
				{
					return !(rsa as RSAManaged).PublicOnly;
				}
				try
				{
					RSAParameters rSAParameters = rsa.ExportParameters(true);
					return rSAParameters.D != null && rSAParameters.P != null && rSAParameters.Q != null;
				}
				catch (CryptographicException)
				{
					return false;
				}
			}
		}

		public RSA RSA
		{
			get
			{
				if (rsa == null)
				{
					rsa = RSA.Create();
				}
				return rsa;
			}
			set
			{
				rsa = value;
				InvalidateCache();
			}
		}

		public byte[] PublicKey
		{
			get
			{
				if (publicKey == null)
				{
					byte[] array = CryptoConvert.ToCapiKeyBlob(rsa, false);
					publicKey = new byte[32 + (rsa.KeySize >> 3)];
					publicKey[0] = array[4];
					publicKey[1] = array[5];
					publicKey[2] = array[6];
					publicKey[3] = array[7];
					publicKey[4] = 4;
					publicKey[5] = 128;
					publicKey[6] = 0;
					publicKey[7] = 0;
					byte[] bytes = BitConverterLE.GetBytes(publicKey.Length - 12);
					publicKey[8] = bytes[0];
					publicKey[9] = bytes[1];
					publicKey[10] = bytes[2];
					publicKey[11] = bytes[3];
					publicKey[12] = 6;
					Buffer.BlockCopy(array, 1, publicKey, 13, publicKey.Length - 13);
					publicKey[23] = 49;
				}
				return (byte[])publicKey.Clone();
			}
		}

		public byte[] PublicKeyToken
		{
			get
			{
				if (keyToken == null)
				{
					byte[] array = PublicKey;
					if (array == null)
					{
						return null;
					}
					HashAlgorithm hashAlgorithm = HashAlgorithm.Create(TokenAlgorithm);
					byte[] array2 = hashAlgorithm.ComputeHash(array);
					keyToken = new byte[8];
					Buffer.BlockCopy(array2, array2.Length - 8, keyToken, 0, 8);
					Array.Reverse(keyToken, 0, 8);
				}
				return (byte[])keyToken.Clone();
			}
		}

		public string TokenAlgorithm
		{
			get
			{
				if (tokenAlgorithm == null)
				{
					tokenAlgorithm = "SHA1";
				}
				return tokenAlgorithm;
			}
			set
			{
				string text = value.ToUpper(CultureInfo.InvariantCulture);
				if (text == "SHA1" || text == "MD5")
				{
					tokenAlgorithm = value;
					InvalidateCache();
					return;
				}
				throw new ArgumentException("Unsupported hash algorithm for token");
			}
		}

		public StrongName()
		{
		}

		public StrongName(int keySize)
		{
			rsa = new RSAManaged(keySize);
		}

		public StrongName(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (data.Length == 16)
			{
				int num = 0;
				int num2 = 0;
				while (num < data.Length)
				{
					num2 += data[num++];
				}
				if (num2 == 4)
				{
					publicKey = (byte[])data.Clone();
				}
			}
			else
			{
				RSA = CryptoConvert.FromCapiKeyBlob(data);
				if (rsa == null)
				{
					throw new ArgumentException("data isn't a correctly encoded RSA public key");
				}
			}
		}

		public StrongName(RSA rsa)
		{
			if (rsa == null)
			{
				throw new ArgumentNullException("rsa");
			}
			RSA = rsa;
		}

		private void InvalidateCache()
		{
			publicKey = null;
			keyToken = null;
		}

		public byte[] GetBytes()
		{
			return CryptoConvert.ToCapiPrivateKeyBlob(RSA);
		}

		private uint RVAtoPosition(uint r, int sections, byte[] headers)
		{
			for (int i = 0; i < sections; i++)
			{
				uint num = BitConverterLE.ToUInt32(headers, i * 40 + 20);
				uint num2 = BitConverterLE.ToUInt32(headers, i * 40 + 12);
				int num3 = (int)BitConverterLE.ToUInt32(headers, i * 40 + 8);
				if (num2 <= r && r < num2 + num3)
				{
					return num + r - num2;
				}
			}
			return 0u;
		}

		internal StrongNameSignature StrongHash(Stream stream, StrongNameOptions options)
		{
			StrongNameSignature strongNameSignature = new StrongNameSignature();
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(TokenAlgorithm);
			CryptoStream cryptoStream = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write);
			byte[] array = new byte[128];
			stream.Read(array, 0, 128);
			if (BitConverterLE.ToUInt16(array, 0) != 23117)
			{
				return null;
			}
			uint num = BitConverterLE.ToUInt32(array, 60);
			cryptoStream.Write(array, 0, 128);
			if (num != 128)
			{
				byte[] array2 = new byte[num - 128];
				stream.Read(array2, 0, array2.Length);
				cryptoStream.Write(array2, 0, array2.Length);
			}
			byte[] array3 = new byte[248];
			stream.Read(array3, 0, 248);
			if (BitConverterLE.ToUInt32(array3, 0) != 17744)
			{
				return null;
			}
			if (BitConverterLE.ToUInt16(array3, 4) != 332)
			{
				return null;
			}
			byte[] src = new byte[8];
			Buffer.BlockCopy(src, 0, array3, 88, 4);
			Buffer.BlockCopy(src, 0, array3, 152, 8);
			cryptoStream.Write(array3, 0, 248);
			ushort num2 = BitConverterLE.ToUInt16(array3, 6);
			int num3 = num2 * 40;
			byte[] array4 = new byte[num3];
			stream.Read(array4, 0, num3);
			cryptoStream.Write(array4, 0, num3);
			uint r = BitConverterLE.ToUInt32(array3, 232);
			uint num4 = RVAtoPosition(r, num2, array4);
			int num5 = (int)BitConverterLE.ToUInt32(array3, 236);
			byte[] array5 = new byte[num5];
			stream.Position = num4;
			stream.Read(array5, 0, num5);
			uint r2 = BitConverterLE.ToUInt32(array5, 32);
			strongNameSignature.SignaturePosition = RVAtoPosition(r2, num2, array4);
			strongNameSignature.SignatureLength = BitConverterLE.ToUInt32(array5, 36);
			uint r3 = BitConverterLE.ToUInt32(array5, 8);
			strongNameSignature.MetadataPosition = RVAtoPosition(r3, num2, array4);
			strongNameSignature.MetadataLength = BitConverterLE.ToUInt32(array5, 12);
			if (options == StrongNameOptions.Metadata)
			{
				cryptoStream.Close();
				hashAlgorithm.Initialize();
				byte[] array6 = new byte[strongNameSignature.MetadataLength];
				stream.Position = strongNameSignature.MetadataPosition;
				stream.Read(array6, 0, array6.Length);
				strongNameSignature.Hash = hashAlgorithm.ComputeHash(array6);
				return strongNameSignature;
			}
			for (int i = 0; i < num2; i++)
			{
				uint num6 = BitConverterLE.ToUInt32(array4, i * 40 + 20);
				int num7 = (int)BitConverterLE.ToUInt32(array4, i * 40 + 16);
				byte[] array7 = new byte[num7];
				stream.Position = num6;
				stream.Read(array7, 0, num7);
				if (num6 <= strongNameSignature.SignaturePosition && strongNameSignature.SignaturePosition < num6 + num7)
				{
					int num8 = (int)(strongNameSignature.SignaturePosition - num6);
					if (num8 > 0)
					{
						cryptoStream.Write(array7, 0, num8);
					}
					strongNameSignature.Signature = new byte[strongNameSignature.SignatureLength];
					Buffer.BlockCopy(array7, num8, strongNameSignature.Signature, 0, (int)strongNameSignature.SignatureLength);
					Array.Reverse(strongNameSignature.Signature);
					int num9 = (int)(num8 + strongNameSignature.SignatureLength);
					int num10 = num7 - num9;
					if (num10 > 0)
					{
						cryptoStream.Write(array7, num9, num10);
					}
				}
				else
				{
					cryptoStream.Write(array7, 0, num7);
				}
			}
			cryptoStream.Close();
			strongNameSignature.Hash = hashAlgorithm.Hash;
			return strongNameSignature;
		}

		public byte[] Hash(string fileName)
		{
			FileStream fileStream = File.OpenRead(fileName);
			StrongNameSignature strongNameSignature = StrongHash(fileStream, StrongNameOptions.Metadata);
			fileStream.Close();
			return strongNameSignature.Hash;
		}

		public bool Sign(string fileName)
		{
			bool flag = false;
			StrongNameSignature strongNameSignature;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				strongNameSignature = StrongHash(fileStream, StrongNameOptions.Signature);
				fileStream.Close();
			}
			if (strongNameSignature.Hash == null)
			{
				return false;
			}
			byte[] array = null;
			try
			{
				RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rsa);
				rSAPKCS1SignatureFormatter.SetHashAlgorithm(TokenAlgorithm);
				array = rSAPKCS1SignatureFormatter.CreateSignature(strongNameSignature.Hash);
				Array.Reverse(array);
			}
			catch (CryptographicException)
			{
				return false;
			}
			using (FileStream fileStream2 = File.OpenWrite(fileName))
			{
				fileStream2.Position = strongNameSignature.SignaturePosition;
				fileStream2.Write(array, 0, array.Length);
				fileStream2.Close();
				return true;
			}
		}

		public bool Verify(string fileName)
		{
			bool flag = false;
			using (FileStream fileStream = File.OpenRead(fileName))
			{
				flag = Verify(fileStream);
				fileStream.Close();
				return flag;
			}
		}

		public bool Verify(Stream stream)
		{
			StrongNameSignature strongNameSignature = StrongHash(stream, StrongNameOptions.Signature);
			if (strongNameSignature.Hash == null)
			{
				return false;
			}
			try
			{
				AssemblyHashAlgorithm algorithm = AssemblyHashAlgorithm.SHA1;
				if (tokenAlgorithm == "MD5")
				{
					algorithm = AssemblyHashAlgorithm.MD5;
				}
				return Verify(rsa, algorithm, strongNameSignature.Hash, strongNameSignature.Signature);
			}
			catch (CryptographicException)
			{
				return false;
			}
		}

		public static bool IsAssemblyStrongnamed(string assemblyName)
		{
			if (!initialized)
			{
				lock (lockObject)
				{
					if (!initialized)
					{
						initialized = true;
					}
				}
			}
			try
			{
				AssemblyName assemblyName2 = AssemblyName.GetAssemblyName(assemblyName);
				if (assemblyName2 == null)
				{
					return false;
				}
				byte[] mappedPublicKey = StrongNameManager.GetMappedPublicKey(assemblyName2.GetPublicKeyToken());
				if (mappedPublicKey == null || mappedPublicKey.Length < 12)
				{
					mappedPublicKey = assemblyName2.GetPublicKey();
					if (mappedPublicKey == null || mappedPublicKey.Length < 12)
					{
						return false;
					}
				}
				if (!StrongNameManager.MustVerify(assemblyName2))
				{
					return true;
				}
				RSA rSA = CryptoConvert.FromCapiPublicKeyBlob(mappedPublicKey, 12);
				StrongName strongName = new StrongName(rSA);
				return strongName.Verify(assemblyName);
			}
			catch
			{
				return false;
			}
		}

		public static bool VerifySignature(byte[] publicKey, int algorithm, byte[] hash, byte[] signature)
		{
			try
			{
				RSA rSA = CryptoConvert.FromCapiPublicKeyBlob(publicKey);
				return Verify(rSA, (AssemblyHashAlgorithm)algorithm, hash, signature);
			}
			catch
			{
				return false;
			}
		}

		private static bool Verify(RSA rsa, AssemblyHashAlgorithm algorithm, byte[] hash, byte[] signature)
		{
			RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rsa);
			switch (algorithm)
			{
			case AssemblyHashAlgorithm.MD5:
				rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
				break;
			default:
				rSAPKCS1SignatureDeformatter.SetHashAlgorithm("SHA1");
				break;
			}
			return rSAPKCS1SignatureDeformatter.VerifySignature(hash, signature);
		}
	}
}
