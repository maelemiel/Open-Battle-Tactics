using System.IO;
using System.Text;
using LitJson0;

public class SignedLitJsonMediaType : MediaType
{
	public object Marshall(object o)
	{
		if (o == null)
		{
			o = new JsonObject();
		}
		JsonObject jsonObject = (JsonObject)o;
		JsonObject jsonObject2 = new JsonObject();
		string text = jsonObject.ToJson();
		string inputStr = text + "mSyRT3h6Qk";
		string value = HashUtility.SHA1(inputStr);
		jsonObject2["content"] = text;
		jsonObject2["hash"] = value;
		return jsonObject2.ToJson();
	}

	public T Unmarshall<T>(RestResponse response)
	{
		return (T)(object)JsonMapper.ToObject((string)response.Body);
	}

	public object ParseResponseStream(RestResponse response)
	{
		using (StreamReader streamReader = new StreamReader(response.Stream, Encoding.GetEncoding(response.CharacterSet)))
		{
			return streamReader.ReadToEnd();
		}
	}
}
