using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace MobageEditor
{
	public class NGPipesSender : IDictionaryChannel
	{
		private string hostname;

		private bool closing;

		private List<JsonData> pendingMessagesQueue;

		private List<JsonData> inFlightEventsQueue;

		public NGPipesSender(string hostname)
		{
			closing = false;
			this.hostname = hostname;
			pendingMessagesQueue = new List<JsonData>();
			inFlightEventsQueue = new List<JsonData>();
			checkDrainQueue();
		}

		private void checkDrainQueue()
		{
		}

		public void Send(JsonData msg)
		{
			string s = msg.ToJson();
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			string requestUriString = "https://" + hostname + "/pipes/r.2/bulk_record_stats";
			HttpWebRequest httpWebRequest = System.Net.WebRequest.Create(requestUriString) as HttpWebRequest;
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/json+batch";
			httpWebRequest.ContentLength = bytes.Length;
			httpWebRequest.Headers["X-Ngpipes-Api"] = "1.0";
			httpWebRequest.Headers["If-None-Match"] = "0";
			using (Stream stream = httpWebRequest.GetRequestStream())
			{
				stream.Write(bytes, 0, bytes.Length);
				stream.Close();
			}
			WebResponse response = httpWebRequest.GetResponse();
			using (Stream stream2 = response.GetResponseStream())
			{
				new StreamReader(stream2, Encoding.UTF8);
			}
		}
	}
}
