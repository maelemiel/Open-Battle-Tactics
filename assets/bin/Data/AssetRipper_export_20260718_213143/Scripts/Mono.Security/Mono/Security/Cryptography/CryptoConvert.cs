using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Mono.Security.Cryptography
{
	public sealed class CryptoConvert
	{
		private CryptoConvert()
		{
		}

		private static int ToInt32LE(byte[] bytes, int offset)
		{
			return (bytes[offset + 3] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 1] << 8) | bytes[offset];
		}

		private static uint ToUInt32LE(byte[] bytes, int offset)
		{
			return (uint)((bytes[offset + 3] << 24) | (bytes[offset + 2] << 16) | (bytes[offset + 1] << 8) | bytes[offset]);
		}

		private static byte[] GetBytesLE(int val)
		{
			return new byte[4]
			{
				(byte)(val & 0xFF),
				(byte)((val >> 8) & 0xFF),
				(byte)((val >> 16) & 0xFF),
				(byte)((val >> 24) & 0xFF)
			};
		}

		private static byte[] Trim(byte[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != 0)
				{
					byte[] array2 = new byte[array.Length - i];
					Buffer.BlockCopy(array, i, array2, 0, array2.Length);
					return array2;
				}
			}
			return null;
		}

		public static RSA FromCapiPrivateKeyBlob(byte[] blob)
		{
			return FromCapiPrivateKeyBlob(blob, 0);
		}

		public static RSA FromCapiPrivateKeyBlob(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			RSAParameters parameters = default(RSAParameters);
			try
			{
				if (blob[offset] != 7 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || ToUInt32LE(blob, offset + 8) != 843141970)
				{
					throw new CryptographicException("Invalid blob header");
				}
				int num = ToInt32LE(blob, offset + 12);
				byte[] array = new byte[4];
				Buffer.BlockCopy(blob, offset + 16, array, 0, 4);
				Array.Reverse(array);
				parameters.Exponent = Trim(array);
				int num2 = offset + 20;
				int num3 = num >> 3;
				parameters.Modulus = new byte[num3];
				Buffer.BlockCopy(blob, num2, parameters.Modulus, 0, num3);
				Array.Reverse(parameters.Modulus);
				num2 += num3;
				int num4 = num3 >> 1;
				parameters.P = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.P, 0, num4);
				Array.Reverse(parameters.P);
				num2 += num4;
				parameters.Q = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.Q, 0, num4);
				Array.Reverse(parameters.Q);
				num2 += num4;
				parameters.DP = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.DP, 0, num4);
				Array.Reverse(parameters.DP);
				num2 += num4;
				parameters.DQ = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.DQ, 0, num4);
				Array.Reverse(parameters.DQ);
				num2 += num4;
				parameters.InverseQ = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.InverseQ, 0, num4);
				Array.Reverse(parameters.InverseQ);
				num2 += num4;
				parameters.D = new byte[num3];
				if (num2 + num3 + offset <= blob.Length)
				{
					Buffer.BlockCopy(blob, num2, parameters.D, 0, num3);
					Array.Reverse(parameters.D);
				}
			}
			catch (Exception inner)
			{
				throw new CryptographicException("Invalid blob.", inner);
			}
			RSA rSA = null;
			try
			{
				rSA = RSA.Create();
				rSA.ImportParameters(parameters);
			}
			catch (CryptographicException ex)
			{
				try
				{
					CspParameters cspParameters = new CspParameters();
					cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
					rSA = new RSACryptoServiceProvider(cspParameters);
					rSA.ImportParameters(parameters);
				}
				catch
				{
					throw ex;
				}
			}
			return rSA;
		}

		public static DSA FromCapiPrivateKeyBlobDSA(byte[] blob)
		{
			return FromCapiPrivateKeyBlobDSA(blob, 0);
		}

		public static DSA FromCapiPrivateKeyBlobDSA(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			DSAParameters parameters = default(DSAParameters);
			try
			{
				if (blob[offset] != 7 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || ToUInt32LE(blob, offset + 8) != 844321604)
				{
					throw new CryptographicException("Invalid blob header");
				}
				int num = ToInt32LE(blob, offset + 12);
				int num2 = num >> 3;
				int num3 = offset + 16;
				parameters.P = new byte[num2];
				Buffer.BlockCopy(blob, num3, parameters.P, 0, num2);
				Array.Reverse(parameters.P);
				num3 += num2;
				parameters.Q = new byte[20];
				Buffer.BlockCopy(blob, num3, parameters.Q, 0, 20);
				Array.Reverse(parameters.Q);
				num3 += 20;
				parameters.G = new byte[num2];
				Buffer.BlockCopy(blob, num3, parameters.G, 0, num2);
				Array.Reverse(parameters.G);
				num3 += num2;
				parameters.X = new byte[20];
				Buffer.BlockCopy(blob, num3, parameters.X, 0, 20);
				Array.Reverse(parameters.X);
				num3 += 20;
				parameters.Counter = ToInt32LE(blob, num3);
				num3 += 4;
				parameters.Seed = new byte[20];
				Buffer.BlockCopy(blob, num3, parameters.Seed, 0, 20);
				Array.Reverse(parameters.Seed);
				num3 += 20;
			}
			catch (Exception inner)
			{
				throw new CryptographicException("Invalid blob.", inner);
			}
			DSA dSA = null;
			try
			{
				dSA = DSA.Create();
				dSA.ImportParameters(parameters);
			}
			catch (CryptographicException ex)
			{
				try
				{
					CspParameters cspParameters = new CspParameters();
					cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
					dSA = new DSACryptoServiceProvider(cspParameters);
					dSA.ImportParameters(parameters);
				}
				catch
				{
					throw ex;
				}
			}
			return dSA;
		}

		public static byte[] ToCapiPrivateKeyBlob(RSA rsa)
		{
			RSAParameters rSAParameters = rsa.ExportParameters(true);
			int num = rSAParameters.Modulus.Length;
			byte[] array = new byte[20 + (num << 2) + (num >> 1)];
			array[0] = 7;
			array[1] = 2;
			array[5] = 36;
			array[8] = 82;
			array[9] = 83;
			array[10] = 65;
			array[11] = 50;
			byte[] bytesLE = GetBytesLE(num << 3);
			array[12] = bytesLE[0];
			array[13] = bytesLE[1];
			array[14] = bytesLE[2];
			array[15] = bytesLE[3];
			int num2 = 16;
			int num3 = rSAParameters.Exponent.Length;
			while (num3 > 0)
			{
				array[num2++] = rSAParameters.Exponent[--num3];
			}
			num2 = 20;
			byte[] modulus = rSAParameters.Modulus;
			int num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = rSAParameters.P;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = rSAParameters.Q;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = rSAParameters.DP;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = rSAParameters.DQ;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = rSAParameters.InverseQ;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = rSAParameters.D;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			return array;
		}

		public static byte[] ToCapiPrivateKeyBlob(DSA dsa)
		{
			DSAParameters dSAParameters = dsa.ExportParameters(true);
			int num = dSAParameters.P.Length;
			byte[] array = new byte[16 + num + 20 + num + 20 + 4 + 20];
			array[0] = 7;
			array[1] = 2;
			array[5] = 34;
			array[8] = 68;
			array[9] = 83;
			array[10] = 83;
			array[11] = 50;
			byte[] bytesLE = GetBytesLE(num << 3);
			array[12] = bytesLE[0];
			array[13] = bytesLE[1];
			array[14] = bytesLE[2];
			array[15] = bytesLE[3];
			int num2 = 16;
			byte[] p = dSAParameters.P;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, num);
			num2 += num;
			p = dSAParameters.Q;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, 20);
			num2 += 20;
			p = dSAParameters.G;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, num);
			num2 += num;
			p = dSAParameters.X;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, 20);
			num2 += 20;
			Buffer.BlockCopy(GetBytesLE(dSAParameters.Counter), 0, array, num2, 4);
			num2 += 4;
			p = dSAParameters.Seed;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, 20);
			return array;
		}

		public static RSA FromCapiPublicKeyBlob(byte[] blob)
		{
			return FromCapiPublicKeyBlob(blob, 0);
		}

		public static RSA FromCapiPublicKeyBlob(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			try
			{
				if (blob[offset] != 6 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || ToUInt32LE(blob, offset + 8) != 826364754)
				{
					throw new CryptographicException("Invalid blob header");
				}
				int num = ToInt32LE(blob, offset + 12);
				RSAParameters parameters = new RSAParameters
				{
					Exponent = new byte[3]
				};
				parameters.Exponent[0] = blob[offset + 18];
				parameters.Exponent[1] = blob[offset + 17];
				parameters.Exponent[2] = blob[offset + 16];
				int srcOffset = offset + 20;
				int num2 = num >> 3;
				parameters.Modulus = new byte[num2];
				Buffer.BlockCopy(blob, srcOffset, parameters.Modulus, 0, num2);
				Array.Reverse(parameters.Modulus);
				RSA rSA = null;
				try
				{
					rSA = RSA.Create();
					rSA.ImportParameters(parameters);
				}
				catch (CryptographicException)
				{
					CspParameters cspParameters = new CspParameters();
					cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
					rSA = new RSACryptoServiceProvider(cspParameters);
					rSA.ImportParameters(parameters);
				}
				return rSA;
			}
			catch (Exception inner)
			{
				throw new CryptographicException("Invalid blob.", inner);
			}
		}

		public static DSA FromCapiPublicKeyBlobDSA(byte[] blob)
		{
			return FromCapiPublicKeyBlobDSA(blob, 0);
		}

		public static DSA FromCapiPublicKeyBlobDSA(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			try
			{
				if (blob[offset] != 6 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || ToUInt32LE(blob, offset + 8) != 827544388)
				{
					throw new CryptographicException("Invalid blob header");
				}
				int num = ToInt32LE(blob, offset + 12);
				DSAParameters parameters = default(DSAParameters);
				int num2 = num >> 3;
				int num3 = offset + 16;
				parameters.P = new byte[num2];
				Buffer.BlockCopy(blob, num3, parameters.P, 0, num2);
				Array.Reverse(parameters.P);
				num3 += num2;
				parameters.Q = new byte[20];
				Buffer.BlockCopy(blob, num3, parameters.Q, 0, 20);
				Array.Reverse(parameters.Q);
				num3 += 20;
				parameters.G = new byte[num2];
				Buffer.BlockCopy(blob, num3, parameters.G, 0, num2);
				Array.Reverse(parameters.G);
				num3 += num2;
				parameters.Y = new byte[num2];
				Buffer.BlockCopy(blob, num3, parameters.Y, 0, num2);
				Array.Reverse(parameters.Y);
				num3 += num2;
				parameters.Counter = ToInt32LE(blob, num3);
				num3 += 4;
				parameters.Seed = new byte[20];
				Buffer.BlockCopy(blob, num3, parameters.Seed, 0, 20);
				Array.Reverse(parameters.Seed);
				num3 += 20;
				DSA dSA = DSA.Create();
				dSA.ImportParameters(parameters);
				return dSA;
			}
			catch (Exception inner)
			{
				throw new CryptographicException("Invalid blob.", inner);
			}
		}

		public static byte[] ToCapiPublicKeyBlob(RSA rsa)
		{
			RSAParameters rSAParameters = rsa.ExportParameters(false);
			int num = rSAParameters.Modulus.Length;
			byte[] array = new byte[20 + num];
			array[0] = 6;
			array[1] = 2;
			array[5] = 36;
			array[8] = 82;
			array[9] = 83;
			array[10] = 65;
			array[11] = 49;
			byte[] bytesLE = GetBytesLE(num << 3);
			array[12] = bytesLE[0];
			array[13] = bytesLE[1];
			array[14] = bytesLE[2];
			array[15] = bytesLE[3];
			int num2 = 16;
			int num3 = rSAParameters.Exponent.Length;
			while (num3 > 0)
			{
				array[num2++] = rSAParameters.Exponent[--num3];
			}
			num2 = 20;
			byte[] modulus = rSAParameters.Modulus;
			int num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			return array;
		}

		public static byte[] ToCapiPublicKeyBlob(DSA dsa)
		{
			DSAParameters dSAParameters = dsa.ExportParameters(false);
			int num = dSAParameters.P.Length;
			byte[] array = new byte[16 + num + 20 + num + num + 4 + 20];
			array[0] = 6;
			array[1] = 2;
			array[5] = 34;
			array[8] = 68;
			array[9] = 83;
			array[10] = 83;
			array[11] = 49;
			byte[] bytesLE = GetBytesLE(num << 3);
			array[12] = bytesLE[0];
			array[13] = bytesLE[1];
			array[14] = bytesLE[2];
			array[15] = bytesLE[3];
			int num2 = 16;
			byte[] p = dSAParameters.P;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, num);
			num2 += num;
			p = dSAParameters.Q;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, 20);
			num2 += 20;
			p = dSAParameters.G;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, num);
			num2 += num;
			p = dSAParameters.Y;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, num);
			num2 += num;
			Buffer.BlockCopy(GetBytesLE(dSAParameters.Counter), 0, array, num2, 4);
			num2 += 4;
			p = dSAParameters.Seed;
			Array.Reverse(p);
			Buffer.BlockCopy(p, 0, array, num2, 20);
			return array;
		}

		public static RSA FromCapiKeyBlob(byte[] blob)
		{
			return FromCapiKeyBlob(blob, 0);
		}

		public static RSA FromCapiKeyBlob(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			switch (blob[offset])
			{
			case 0:
				if (blob[offset + 12] == 6)
				{
					return FromCapiPublicKeyBlob(blob, offset + 12);
				}
				break;
			case 6:
				return FromCapiPublicKeyBlob(blob, offset);
			case 7:
				return FromCapiPrivateKeyBlob(blob, offset);
			}
			throw new CryptographicException("Unknown blob format.");
		}

		public static DSA FromCapiKeyBlobDSA(byte[] blob)
		{
			return FromCapiKeyBlobDSA(blob, 0);
		}

		public static DSA FromCapiKeyBlobDSA(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			switch (blob[offset])
			{
			case 6:
				return FromCapiPublicKeyBlobDSA(blob, offset);
			case 7:
				return FromCapiPrivateKeyBlobDSA(blob, offset);
			default:
				throw new CryptographicException("Unknown blob format.");
			}
		}

		public static byte[] ToCapiKeyBlob(AsymmetricAlgorithm keypair, bool includePrivateKey)
		{
			if (keypair == null)
			{
				throw new ArgumentNullException("keypair");
			}
			if (keypair is RSA)
			{
				return ToCapiKeyBlob((RSA)keypair, includePrivateKey);
			}
			if (keypair is DSA)
			{
				return ToCapiKeyBlob((DSA)keypair, includePrivateKey);
			}
			return null;
		}

		public static byte[] ToCapiKeyBlob(RSA rsa, bool includePrivateKey)
		{
			if (rsa == null)
			{
				throw new ArgumentNullException("rsa");
			}
			if (includePrivateKey)
			{
				return ToCapiPrivateKeyBlob(rsa);
			}
			return ToCapiPublicKeyBlob(rsa);
		}

		public static byte[] ToCapiKeyBlob(DSA dsa, bool includePrivateKey)
		{
			if (dsa == null)
			{
				throw new ArgumentNullException("dsa");
			}
			if (includePrivateKey)
			{
				return ToCapiPrivateKeyBlob(dsa);
			}
			return ToCapiPublicKeyBlob(dsa);
		}

		public static string ToHex(byte[] input)
		{
			if (input == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
			foreach (byte b in input)
			{
				stringBuilder.Append(b.ToString("X2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		private static byte FromHexChar(char c)
		{
			if (c >= 'a' && c <= 'f')
			{
				return (byte)(c - 97 + 10);
			}
			if (c >= 'A' && c <= 'F')
			{
				return (byte)(c - 65 + 10);
			}
			if (c >= '0' && c <= '9')
			{
				return (byte)(c - 48);
			}
			throw new ArgumentException("invalid hex char");
		}

		public static byte[] FromHex(string hex)
		{
			if (hex == null)
			{
				return null;
			}
			if ((hex.Length & 1) == 1)
			{
				throw new ArgumentException("Length must be a multiple of 2");
			}
			byte[] array = new byte[hex.Length >> 1];
			int num = 0;
			int num2 = 0;
			while (num < array.Length)
			{
				array[num] = (byte)(FromHexChar(hex[num2++]) << 4);
				array[num++] += FromHexChar(hex[num2++]);
			}
			return array;
		}
	}
}
