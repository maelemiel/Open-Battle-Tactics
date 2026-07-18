using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class HashUtility
{
	public static string MD5(string path)
	{
		using (FileStream inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			MD5 mD = new MD5CryptoServiceProvider();
			byte[] bytes = mD.ComputeHash(inputStream);
			return bytes.ToHex();
		}
	}

	public static string SHA1(string inputStr)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(inputStr);
		SHA1 sHA = new SHA1CryptoServiceProvider();
		byte[] bytes2 = sHA.ComputeHash(bytes);
		return bytes2.ToHex();
	}

	public static string ToHex(this byte[] bytes)
	{
		char[] array = new char[bytes.Length * 2];
		int num = 0;
		int num2 = 0;
		while (num < bytes.Length)
		{
			byte b = (byte)(bytes[num] >> 4);
			array[num2] = (char)((b <= 9) ? (b + 48) : (b + 55 + 32));
			b = (byte)(bytes[num] & 0xF);
			array[++num2] = (char)((b <= 9) ? (b + 48) : (b + 55 + 32));
			num++;
			num2++;
		}
		return new string(array);
	}
}
