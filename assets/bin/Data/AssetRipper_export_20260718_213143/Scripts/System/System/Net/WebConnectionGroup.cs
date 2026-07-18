using System.Collections;

namespace System.Net
{
	internal class WebConnectionGroup
	{
		private ServicePoint sPoint;

		private string name;

		private ArrayList connections;

		private Random rnd;

		private Queue queue;

		public string Name
		{
			get
			{
				return name;
			}
		}

		internal Queue Queue
		{
			get
			{
				return queue;
			}
		}

		public WebConnectionGroup(ServicePoint sPoint, string name)
		{
			this.sPoint = sPoint;
			this.name = name;
			connections = new ArrayList(1);
			queue = new Queue();
		}

		public void Close()
		{
			lock (connections)
			{
				WeakReference weakReference = null;
				int count = connections.Count;
				ArrayList arrayList = null;
				for (int i = 0; i < count; i++)
				{
					weakReference = (WeakReference)connections[i];
					System.Net.WebConnection webConnection = weakReference.Target as System.Net.WebConnection;
					if (webConnection != null)
					{
						webConnection.Close(false);
					}
				}
				connections.Clear();
			}
		}

		public System.Net.WebConnection GetConnection(HttpWebRequest request)
		{
			System.Net.WebConnection webConnection = null;
			lock (connections)
			{
				WeakReference weakReference = null;
				int count = connections.Count;
				ArrayList arrayList = null;
				for (int i = 0; i < count; i++)
				{
					weakReference = (WeakReference)connections[i];
					webConnection = weakReference.Target as System.Net.WebConnection;
					if (webConnection == null)
					{
						if (arrayList == null)
						{
							arrayList = new ArrayList(1);
						}
						arrayList.Add(i);
					}
				}
				if (arrayList != null)
				{
					for (int num = arrayList.Count - 1; num >= 0; num--)
					{
						connections.RemoveAt((int)arrayList[num]);
					}
				}
				return CreateOrReuseConnection(request);
			}
		}

		private static void PrepareSharingNtlm(System.Net.WebConnection cnc, HttpWebRequest request)
		{
			if (cnc.NtlmAuthenticated)
			{
				bool flag = false;
				NetworkCredential ntlmCredential = cnc.NtlmCredential;
				NetworkCredential credential = request.Credentials.GetCredential(request.RequestUri, "NTLM");
				if (ntlmCredential.Domain != credential.Domain || ntlmCredential.UserName != credential.UserName || ntlmCredential.Password != credential.Password)
				{
					flag = true;
				}
				if (!flag)
				{
					bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
					bool unsafeAuthenticatedConnectionSharing2 = cnc.UnsafeAuthenticatedConnectionSharing;
					flag = !unsafeAuthenticatedConnectionSharing || unsafeAuthenticatedConnectionSharing != unsafeAuthenticatedConnectionSharing2;
				}
				if (flag)
				{
					cnc.Close(false);
					cnc.ResetNtlm();
				}
			}
		}

		private System.Net.WebConnection CreateOrReuseConnection(HttpWebRequest request)
		{
			int num = connections.Count;
			System.Net.WebConnection webConnection;
			for (int i = 0; i < num; i++)
			{
				WeakReference weakReference = connections[i] as WeakReference;
				webConnection = weakReference.Target as System.Net.WebConnection;
				if (webConnection == null)
				{
					connections.RemoveAt(i);
					num--;
					i--;
				}
				else if (!webConnection.Busy)
				{
					PrepareSharingNtlm(webConnection, request);
					return webConnection;
				}
			}
			if (sPoint.ConnectionLimit > num)
			{
				webConnection = new System.Net.WebConnection(this, sPoint);
				connections.Add(new WeakReference(webConnection));
				return webConnection;
			}
			if (rnd == null)
			{
				rnd = new Random();
			}
			int index = ((num > 1) ? rnd.Next(0, num - 1) : 0);
			WeakReference weakReference2 = (WeakReference)connections[index];
			webConnection = weakReference2.Target as System.Net.WebConnection;
			if (webConnection == null)
			{
				webConnection = new System.Net.WebConnection(this, sPoint);
				connections.RemoveAt(index);
				connections.Add(new WeakReference(webConnection));
			}
			return webConnection;
		}
	}
}
