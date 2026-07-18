using System.Runtime.InteropServices;
using System.Text;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class StrongNamePublicKeyBlob
	{
		internal byte[] pubkey;

		public StrongNamePublicKeyBlob(byte[] publicKey)
		{
			if (publicKey == null)
			{
				throw new ArgumentNullException("publicKey");
			}
			pubkey = publicKey;
		}

		internal static StrongNamePublicKeyBlob FromString(string s)
		{
			if (s == null || s.Length == 0)
			{
				return null;
			}
			int num = s.Length / 2;
			byte[] array = new byte[num];
			int num2 = 0;
			int num3 = 0;
			while (num2 < s.Length)
			{
				byte b = CharToByte(s[num2]);
				byte b2 = CharToByte(s[num2 + 1]);
				array[num3] = Convert.ToByte(b * 16 + b2);
				num2 += 2;
				num3++;
			}
			return new StrongNamePublicKeyBlob(array);
		}

		private static byte CharToByte(char c)
		{
			char c2 = char.ToLowerInvariant(c);
			if (char.IsDigit(c2))
			{
				return (byte)(c2 - 48);
			}
			return (byte)(c2 - 97 + 10);
		}

		public override bool Equals(object obj)
		{
			StrongNamePublicKeyBlob strongNamePublicKeyBlob = obj as StrongNamePublicKeyBlob;
			if (strongNamePublicKeyBlob == null)
			{
				return false;
			}
			bool flag = pubkey.Length == strongNamePublicKeyBlob.pubkey.Length;
			if (flag)
			{
				for (int i = 0; i < pubkey.Length; i++)
				{
					if (pubkey[i] != strongNamePublicKeyBlob.pubkey[i])
					{
						return false;
					}
				}
			}
			return flag;
		}

		public override int GetHashCode()
		{
			int num = 0;
			int num2 = 0;
			int num3 = Math.Min(pubkey.Length, 4);
			while (num2 < num3)
			{
				num = (num << 8) + pubkey[num2++];
			}
			return num;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < pubkey.Length; i++)
			{
				stringBuilder.Append(pubkey[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}
	}
}
