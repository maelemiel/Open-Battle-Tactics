using System.Net;

namespace MobageEditor
{
	public class JSONRequest : WebRequest
	{
		public JSONRequest(HttpWebRequest request)
			: base(request)
		{
		}

		public static HttpWebRequest MakeRequest(string url, object body, string method, string contentType)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(url);
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.Method = method;
			if (!string.IsNullOrEmpty(contentType))
			{
				httpWebRequest.ContentType = contentType;
			}
			return httpWebRequest;
		}
	}
}
