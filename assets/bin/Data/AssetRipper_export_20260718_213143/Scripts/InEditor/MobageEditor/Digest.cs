using System;
using System.Security.Cryptography;
using System.Text;

namespace MobageEditor
{
	public class Digest
	{
		public static string Md5Hash(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			MD5 mD = MD5.Create();
			byte[] array = mD.ComputeHash(bytes);
			return BitConverter.ToString(array).Replace("-", string.Empty);
		}
	}
}
