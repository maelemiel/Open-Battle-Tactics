using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Mono.Security;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class Url : IBuiltInEvidence, IIdentityPermissionFactory
	{
		private string origin_url;

		public string Value
		{
			get
			{
				return origin_url;
			}
		}

		public Url(string name)
			: this(name, false)
		{
		}

		internal Url(string name, bool validated)
		{
			origin_url = ((!validated) ? Prepare(name) : name);
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return ((!verbose) ? 1 : 3) + origin_url.Length;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			return 0;
		}

		public object Copy()
		{
			return new Url(origin_url, true);
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new UrlIdentityPermission(origin_url);
		}

		public override bool Equals(object o)
		{
			Url url = o as Url;
			if (url == null)
			{
				return false;
			}
			string text = url.Value;
			string text2 = origin_url;
			if (text.IndexOf(Uri.SchemeDelimiter) < 0)
			{
				text = "file://" + text;
			}
			if (text2.IndexOf(Uri.SchemeDelimiter) < 0)
			{
				text2 = "file://" + text2;
			}
			return string.Compare(text, text2, true, CultureInfo.InvariantCulture) == 0;
		}

		public override int GetHashCode()
		{
			string text = origin_url;
			if (text.IndexOf(Uri.SchemeDelimiter) < 0)
			{
				text = "file://" + text;
			}
			return text.GetHashCode();
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.Url");
			securityElement.AddAttribute("version", "1");
			securityElement.AddChild(new SecurityElement("Url", origin_url));
			return securityElement.ToString();
		}

		private string Prepare(string url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("Url");
			}
			if (url == string.Empty)
			{
				throw new FormatException(Locale.GetText("Invalid (empty) Url"));
			}
			int num = url.IndexOf(Uri.SchemeDelimiter);
			if (num > 0)
			{
				if (url.StartsWith("file://"))
				{
					url = "file://" + url.Substring(7);
				}
				Uri uri = new Uri(url, false, false);
				url = uri.ToString();
			}
			int num2 = url.Length - 1;
			if (url[num2] == '/')
			{
				url = url.Substring(0, num2);
			}
			return url;
		}
	}
}
