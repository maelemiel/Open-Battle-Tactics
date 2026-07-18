using System;
using System.Collections.Generic;
using System.Net;

namespace MobageEditor
{
	public class WebRequest
	{
		[Flags]
		private enum RequestFlags
		{
			None = 0,
			Started = 1,
			Cancelled = 2,
			WasTemporarilyRedirected = 4
		}

		private HttpWebRequest request;

		protected List<TargetAction> targetActions = new List<TargetAction>();

		private RequestFlags requestFlags;

		private object data;

		public HttpWebResponse Response;

		public WebRequest(HttpWebRequest request)
		{
			this.request = request;
		}

		public void Start(Action<HttpWebResponse> block)
		{
			if ((requestFlags & RequestFlags.Started) == RequestFlags.Started)
			{
				return;
			}
			requestFlags |= RequestFlags.Started;
			requestFlags &= ~RequestFlags.WasTemporarilyRedirected;
			data = new object();
			MobageLogger.log(string.Format("Creating operation to execute request to {0}, with body {1}", request.Address, request));
			MobageLogger.log(string.Format("Operation for <{0}> started.", request.Address));
			try
			{
				using (HttpWebResponse obj = (HttpWebResponse)request.GetResponse())
				{
					MobageLogger.log(string.Format("Operation for <{0}> got response.", request.Address));
					block(obj);
				}
			}
			catch (WebException ex)
			{
				int statusCode = (int)((HttpWebResponse)ex.Response).StatusCode;
				MobageLogger.log(string.Format("Failed with HTTP status code {0} while loding {1}", statusCode, this));
				block((HttpWebResponse)ex.Response);
			}
			MobageLogger.log(string.Format("Operation for <{0}> finished.", request.Address));
		}
	}
}
