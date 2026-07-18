using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class StrongName : IBuiltInEvidence, IIdentityPermissionFactory
	{
		private StrongNamePublicKeyBlob publickey;

		private string name;

		private Version version;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public StrongNamePublicKeyBlob PublicKey
		{
			get
			{
				return publickey;
			}
		}

		public Version Version
		{
			get
			{
				return version;
			}
		}

		public StrongName(StrongNamePublicKeyBlob blob, string name, Version version)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (version == null)
			{
				throw new ArgumentNullException("version");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(Locale.GetText("Empty"), "name");
			}
			publickey = blob;
			this.name = name;
			this.version = version;
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return ((!verbose) ? 1 : 5) + name.Length;
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
			return new StrongName(publickey, name, version);
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new StrongNameIdentityPermission(publickey, name, version);
		}

		public override bool Equals(object o)
		{
			StrongName strongName = o as StrongName;
			if (strongName == null)
			{
				return false;
			}
			if (name != strongName.Name)
			{
				return false;
			}
			if (!Version.Equals(strongName.Version))
			{
				return false;
			}
			return PublicKey.Equals(strongName.PublicKey);
		}

		public override int GetHashCode()
		{
			return publickey.GetHashCode();
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement(typeof(StrongName).Name);
			securityElement.AddAttribute("version", "1");
			securityElement.AddAttribute("Key", publickey.ToString());
			securityElement.AddAttribute("Name", name);
			securityElement.AddAttribute("Version", version.ToString());
			return securityElement.ToString();
		}
	}
}
