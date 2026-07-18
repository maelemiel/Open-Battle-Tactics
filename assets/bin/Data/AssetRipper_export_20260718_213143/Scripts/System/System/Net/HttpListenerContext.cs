using System.Security.Principal;
using System.Text;

namespace System.Net
{
	public sealed class HttpListenerContext
	{
		private HttpListenerRequest request;

		private HttpListenerResponse response;

		private IPrincipal user;

		private System.Net.HttpConnection cnc;

		private string error;

		private int err_status = 400;

		internal HttpListener Listener;

		internal int ErrorStatus
		{
			get
			{
				return err_status;
			}
			set
			{
				err_status = value;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				return error;
			}
			set
			{
				error = value;
			}
		}

		internal bool HaveError
		{
			get
			{
				return error != null;
			}
		}

		internal System.Net.HttpConnection Connection
		{
			get
			{
				return cnc;
			}
		}

		public HttpListenerRequest Request
		{
			get
			{
				return request;
			}
		}

		public HttpListenerResponse Response
		{
			get
			{
				return response;
			}
		}

		public IPrincipal User
		{
			get
			{
				return user;
			}
		}

		internal HttpListenerContext(System.Net.HttpConnection cnc)
		{
			this.cnc = cnc;
			request = new HttpListenerRequest(this);
			response = new HttpListenerResponse(this);
		}

		internal void ParseAuthentication(AuthenticationSchemes expectedSchemes)
		{
			if (expectedSchemes == AuthenticationSchemes.Anonymous)
			{
				return;
			}
			string text = request.Headers["Authorization"];
			if (text != null && text.Length >= 2)
			{
				string[] array = text.Split(new char[1] { ' ' }, 2);
				if (string.Compare(array[0], "basic", true) == 0)
				{
					user = ParseBasicAuthentication(array[1]);
				}
			}
		}

		internal IPrincipal ParseBasicAuthentication(string authData)
		{
			try
			{
				string text = null;
				string text2 = null;
				int num = -1;
				string text3 = Encoding.Default.GetString(Convert.FromBase64String(authData));
				num = text3.IndexOf(':');
				text2 = text3.Substring(num + 1);
				text3 = text3.Substring(0, num);
				num = text3.IndexOf('\\');
				text = ((num <= 0) ? text3 : text3.Substring(num));
				HttpListenerBasicIdentity identity = new HttpListenerBasicIdentity(text, text2);
				return new GenericPrincipal(identity, new string[0]);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
