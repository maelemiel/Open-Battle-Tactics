using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SiteIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		private const int version = 1;

		private string _site;

		private static bool[] valid = new bool[94]
		{
			true, false, true, true, true, true, true, true, true, true,
			false, false, true, true, false, true, true, true, true, true,
			true, true, true, true, false, false, false, false, false, false,
			false, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, false, false,
			false, true, true, false, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, false, true, true
		};

		public string Site
		{
			get
			{
				if (IsEmpty())
				{
					throw new NullReferenceException("No site.");
				}
				return _site;
			}
			set
			{
				if (!IsValid(value))
				{
					throw new ArgumentException("Invalid site.");
				}
				_site = value;
			}
		}

		public SiteIdentityPermission(PermissionState state)
		{
			CodeAccessPermission.CheckPermissionState(state, false);
		}

		public SiteIdentityPermission(string site)
		{
			Site = site;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 11;
		}

		public override IPermission Copy()
		{
			if (IsEmpty())
			{
				return new SiteIdentityPermission(PermissionState.None);
			}
			return new SiteIdentityPermission(_site);
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			string text = esd.Attribute("Site");
			if (text != null)
			{
				Site = text;
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			SiteIdentityPermission siteIdentityPermission = Cast(target);
			if (siteIdentityPermission == null || IsEmpty())
			{
				return null;
			}
			if (Match(siteIdentityPermission._site))
			{
				string site = ((_site.Length <= siteIdentityPermission._site.Length) ? siteIdentityPermission._site : _site);
				return new SiteIdentityPermission(site);
			}
			return null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			SiteIdentityPermission siteIdentityPermission = Cast(target);
			if (siteIdentityPermission == null)
			{
				return IsEmpty();
			}
			if (_site == null && siteIdentityPermission._site == null)
			{
				return true;
			}
			if (_site == null || siteIdentityPermission._site == null)
			{
				return false;
			}
			int num = siteIdentityPermission._site.IndexOf('*');
			if (num == -1)
			{
				return _site == siteIdentityPermission._site;
			}
			return _site.EndsWith(siteIdentityPermission._site.Substring(num + 1));
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (_site != null)
			{
				securityElement.AddAttribute("Site", _site);
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			SiteIdentityPermission siteIdentityPermission = Cast(target);
			if (siteIdentityPermission == null || siteIdentityPermission.IsEmpty())
			{
				return Copy();
			}
			if (IsEmpty())
			{
				return siteIdentityPermission.Copy();
			}
			if (Match(siteIdentityPermission._site))
			{
				string site = ((_site.Length >= siteIdentityPermission._site.Length) ? siteIdentityPermission._site : _site);
				return new SiteIdentityPermission(site);
			}
			throw new ArgumentException(Locale.GetText("Cannot union two different sites."), "target");
		}

		private bool IsEmpty()
		{
			return _site == null;
		}

		private SiteIdentityPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			SiteIdentityPermission siteIdentityPermission = target as SiteIdentityPermission;
			if (siteIdentityPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(SiteIdentityPermission));
			}
			return siteIdentityPermission;
		}

		private bool IsValid(string s)
		{
			if (s == null || s.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < s.Length; i++)
			{
				ushort num = s[i];
				switch (num)
				{
				default:
					return false;
				case 42:
					if (s.Length > 1 && (s[i + 1] != '.' || i > 0))
					{
						return false;
					}
					break;
				case 33:
				case 34:
				case 35:
				case 36:
				case 37:
				case 38:
				case 39:
				case 40:
				case 41:
				case 43:
				case 44:
				case 45:
				case 46:
				case 47:
				case 48:
				case 49:
				case 50:
				case 51:
				case 52:
				case 53:
				case 54:
				case 55:
				case 56:
				case 57:
				case 58:
				case 59:
				case 60:
				case 61:
				case 62:
				case 63:
				case 64:
				case 65:
				case 66:
				case 67:
				case 68:
				case 69:
				case 70:
				case 71:
				case 72:
				case 73:
				case 74:
				case 75:
				case 76:
				case 77:
				case 78:
				case 79:
				case 80:
				case 81:
				case 82:
				case 83:
				case 84:
				case 85:
				case 86:
				case 87:
				case 88:
				case 89:
				case 90:
				case 91:
				case 92:
				case 93:
				case 94:
				case 95:
				case 96:
				case 97:
				case 98:
				case 99:
				case 100:
				case 101:
				case 102:
				case 103:
				case 104:
				case 105:
				case 106:
				case 107:
				case 108:
				case 109:
				case 110:
				case 111:
				case 112:
				case 113:
				case 114:
				case 115:
				case 116:
				case 117:
				case 118:
				case 119:
				case 120:
				case 121:
				case 122:
				case 123:
				case 124:
				case 125:
				case 126:
					break;
				}
				if (!valid[num - 33])
				{
					return false;
				}
			}
			if (s.Length == 1)
			{
				return s[0] != '.';
			}
			return true;
		}

		private bool Match(string target)
		{
			if (_site == null || target == null)
			{
				return false;
			}
			int num = _site.IndexOf('*');
			int num2 = target.IndexOf('*');
			if (num == -1 && num2 == -1)
			{
				return _site == target;
			}
			if (num == -1)
			{
				return _site.EndsWith(target.Substring(num2 + 1));
			}
			if (num2 == -1)
			{
				return target.EndsWith(_site.Substring(num + 1));
			}
			string text = _site.Substring(num + 1);
			target = target.Substring(num2 + 1);
			if (text.Length > target.Length)
			{
				return text.EndsWith(target);
			}
			return target.EndsWith(text);
		}
	}
}
