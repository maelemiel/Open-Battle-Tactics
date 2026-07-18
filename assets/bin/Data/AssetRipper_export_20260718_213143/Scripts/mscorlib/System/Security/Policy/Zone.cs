using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Mono.Security;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class Zone : IBuiltInEvidence, IIdentityPermissionFactory
	{
		private SecurityZone zone;

		public SecurityZone SecurityZone
		{
			get
			{
				return zone;
			}
		}

		public Zone(SecurityZone zone)
		{
			if (!Enum.IsDefined(typeof(SecurityZone), zone))
			{
				string message = string.Format(Locale.GetText("Invalid zone {0}."), zone);
				throw new ArgumentException(message, "zone");
			}
			this.zone = zone;
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return 3;
		}

		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			int num = buffer[position++];
			num += buffer[position++];
			return position;
		}

		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			buffer[position++] = '\u0003';
			buffer[position++] = (char)((int)zone >> 16);
			buffer[position++] = (char)(zone & (SecurityZone)65535);
			return position;
		}

		public object Copy()
		{
			return new Zone(zone);
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new ZoneIdentityPermission(zone);
		}

		[MonoTODO("Not user configurable yet")]
		public static Zone CreateFromUrl(string url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			SecurityZone securityZone = SecurityZone.NoZone;
			if (url.Length == 0)
			{
				return new Zone(securityZone);
			}
			Uri uri = new Uri(url);
			if (securityZone == SecurityZone.NoZone)
			{
				securityZone = (uri.IsFile ? ((!File.Exists(uri.LocalPath)) ? ((string.Compare("FILE://", 0, url, 0, 7, true, CultureInfo.InvariantCulture) == 0) ? SecurityZone.Intranet : SecurityZone.Internet) : SecurityZone.MyComputer) : (uri.IsLoopback ? SecurityZone.Intranet : SecurityZone.Internet));
			}
			return new Zone(securityZone);
		}

		public override bool Equals(object o)
		{
			Zone zone = o as Zone;
			if (zone == null)
			{
				return false;
			}
			return zone.zone == this.zone;
		}

		public override int GetHashCode()
		{
			return (int)zone;
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.Zone");
			securityElement.AddAttribute("version", "1");
			securityElement.AddChild(new SecurityElement("Zone", zone.ToString()));
			return securityElement.ToString();
		}
	}
}
