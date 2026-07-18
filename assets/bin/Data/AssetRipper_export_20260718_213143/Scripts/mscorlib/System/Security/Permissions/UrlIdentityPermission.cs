using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class UrlIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		private const int version = 1;

		private string url;

		public string Url
		{
			get
			{
				return url;
			}
			set
			{
				url = ((value != null) ? value : string.Empty);
			}
		}

		public UrlIdentityPermission(PermissionState state)
		{
			CodeAccessPermission.CheckPermissionState(state, false);
			url = string.Empty;
		}

		public UrlIdentityPermission(string site)
		{
			if (site == null)
			{
				throw new ArgumentNullException("site");
			}
			url = site;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 13;
		}

		public override IPermission Copy()
		{
			if (url == null)
			{
				return new UrlIdentityPermission(PermissionState.None);
			}
			return new UrlIdentityPermission(url);
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			string text = esd.Attribute("Url");
			if (text == null)
			{
				url = string.Empty;
			}
			else
			{
				Url = text;
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			UrlIdentityPermission urlIdentityPermission = Cast(target);
			if (urlIdentityPermission == null || IsEmpty())
			{
				return null;
			}
			if (Match(urlIdentityPermission.url))
			{
				if (url.Length > urlIdentityPermission.url.Length)
				{
					return Copy();
				}
				return urlIdentityPermission.Copy();
			}
			return null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			UrlIdentityPermission urlIdentityPermission = Cast(target);
			if (urlIdentityPermission == null)
			{
				return IsEmpty();
			}
			if (IsEmpty())
			{
				return true;
			}
			if (urlIdentityPermission.url == null)
			{
				return false;
			}
			int num = urlIdentityPermission.url.LastIndexOf('*');
			if (num == -1)
			{
				num = urlIdentityPermission.url.Length;
			}
			return string.Compare(url, 0, urlIdentityPermission.url, 0, num, true, CultureInfo.InvariantCulture) == 0;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (!IsEmpty())
			{
				securityElement.AddAttribute("Url", url);
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			UrlIdentityPermission urlIdentityPermission = Cast(target);
			if (urlIdentityPermission == null)
			{
				return Copy();
			}
			if (IsEmpty() && urlIdentityPermission.IsEmpty())
			{
				return null;
			}
			if (urlIdentityPermission.IsEmpty())
			{
				return Copy();
			}
			if (IsEmpty())
			{
				return urlIdentityPermission.Copy();
			}
			if (Match(urlIdentityPermission.url))
			{
				if (url.Length < urlIdentityPermission.url.Length)
				{
					return Copy();
				}
				return urlIdentityPermission.Copy();
			}
			throw new ArgumentException(Locale.GetText("Cannot union two different urls."), "target");
		}

		private bool IsEmpty()
		{
			return url == null || url.Length == 0;
		}

		private UrlIdentityPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			UrlIdentityPermission urlIdentityPermission = target as UrlIdentityPermission;
			if (urlIdentityPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(UrlIdentityPermission));
			}
			return urlIdentityPermission;
		}

		private bool Match(string target)
		{
			if (url == null || target == null)
			{
				return false;
			}
			int num = url.LastIndexOf('*');
			int num2 = target.LastIndexOf('*');
			int num3 = int.MaxValue;
			return string.Compare(length: (num == -1 && num2 == -1) ? Math.Max(url.Length, target.Length) : ((num == -1) ? num2 : ((num2 != -1) ? Math.Min(num, num2) : num)), strA: url, indexA: 0, strB: target, indexB: 0, ignoreCase: true, culture: CultureInfo.InvariantCulture) == 0;
		}
	}
}
