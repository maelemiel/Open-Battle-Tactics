using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;
using Mono.Security;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class UrlMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

		private Url url;

		private string userUrl;

		public string Url
		{
			get
			{
				if (userUrl == null)
				{
					userUrl = url.Value;
				}
				return userUrl;
			}
			set
			{
				url = new Url(value);
			}
		}

		public UrlMembershipCondition(string url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			CheckUrl(url);
			userUrl = url;
			this.url = new Url(url);
		}

		internal UrlMembershipCondition(Url url, string userUrl)
		{
			this.url = (Url)url.Copy();
			this.userUrl = userUrl;
		}

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			string value = url.Value;
			int num = value.LastIndexOf("*");
			if (num == -1)
			{
				num = value.Length;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				if (hostEnumerator.Current is Url && string.Compare(value, 0, (hostEnumerator.Current as Url).Value, 0, num, true, CultureInfo.InvariantCulture) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new UrlMembershipCondition(url, userUrl);
		}

		public override bool Equals(object o)
		{
			UrlMembershipCondition urlMembershipCondition = o as UrlMembershipCondition;
			if (o == null)
			{
				return false;
			}
			string value = url.Value;
			int num = value.Length;
			if (value[num - 1] == '*')
			{
				num--;
				if (value[num - 1] == '/')
				{
					num--;
				}
			}
			return string.Compare(value, 0, urlMembershipCondition.Url, 0, num, true, CultureInfo.InvariantCulture) == 0;
		}

		public void FromXml(SecurityElement e)
		{
			FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", version, version);
			string text = e.Attribute("Url");
			if (text != null)
			{
				CheckUrl(text);
				url = new Url(text);
			}
			else
			{
				url = null;
			}
			userUrl = text;
		}

		public override int GetHashCode()
		{
			return url.GetHashCode();
		}

		public override string ToString()
		{
			return "Url - " + Url;
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = MembershipConditionHelper.Element(typeof(UrlMembershipCondition), version);
			securityElement.AddAttribute("Url", userUrl);
			return securityElement;
		}

		internal void CheckUrl(string url)
		{
			int num = url.IndexOf(Uri.SchemeDelimiter);
			string uriString = ((num >= 0) ? url : ("file://" + url));
			Uri uri = new Uri(uriString, false, false);
			if (uri.Host.IndexOf('*') >= 1)
			{
				string text = Locale.GetText("Invalid * character in url");
				throw new ArgumentException(text, "name");
			}
		}
	}
}
