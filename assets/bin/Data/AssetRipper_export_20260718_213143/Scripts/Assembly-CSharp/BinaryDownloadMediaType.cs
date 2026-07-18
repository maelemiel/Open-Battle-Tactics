using System;
using System.IO;
using System.Security.Cryptography;

public class BinaryDownloadMediaType : MediaType
{
	private string _folder;

	public BinaryDownloadMediaType(string folder)
	{
		_folder = folder;
	}

	public object Marshall(object o)
	{
		return null;
	}

	public T Unmarshall<T>(RestResponse response)
	{
		return default(T);
	}

	public object ParseResponseStream(RestResponse response)
	{
		string text = (string)response.OriginalRequest.Config("filename");
		if (text == null)
		{
			string[] array = response.OriginalRequest.Path.Split('/');
			text = array[array.Length - 1];
		}
		string text2 = _folder + text;
		SaveToDisk(text2, response);
		return text2;
	}

	private void SaveToDisk(string path, RestResponse response)
	{
		MD5 mD = MD5.Create();
		using (Stream stream = response.Stream)
		{
			using (Stream stream2 = new FileStream(path, FileMode.Create))
			{
				byte[] array = new byte[16384];
				int num;
				while ((num = stream.Read(array, 0, array.Length)) != 0)
				{
					mD.TransformBlock(array, 0, num, array, 0);
					stream2.Write(array, 0, num);
				}
				stream2.Close();
				mD.TransformFinalBlock(array, 0, 0);
				string text = (string)response.OriginalRequest.Config("md5");
				if (text != null)
				{
					string strA = mD.Hash.ToHex();
					if (string.Compare(strA, text, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						File.Delete(path);
						Log.ErrorTag("MD5 IS DIFFERENT!", null, "Networklayer");
					}
				}
			}
		}
	}
}
