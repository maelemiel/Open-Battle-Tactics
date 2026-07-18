using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class StrongNameMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

		private StrongNamePublicKeyBlob blob;

		private string name;

		private Version assemblyVersion;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public Version Version
		{
			get
			{
				return assemblyVersion;
			}
			set
			{
				assemblyVersion = value;
			}
		}

		public StrongNamePublicKeyBlob PublicKey
		{
			get
			{
				return blob;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("PublicKey");
				}
				blob = value;
			}
		}

		public StrongNameMembershipCondition(StrongNamePublicKeyBlob blob, string name, Version version)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			this.blob = blob;
			this.name = name;
			if (version != null)
			{
				assemblyVersion = (Version)version.Clone();
			}
		}

		internal StrongNameMembershipCondition(SecurityElement e)
		{
			FromXml(e);
		}

		internal StrongNameMembershipCondition()
		{
		}

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				StrongName strongName = hostEnumerator.Current as StrongName;
				if (strongName != null)
				{
					if (!strongName.PublicKey.Equals(blob))
					{
						return false;
					}
					if (name != null && name != strongName.Name)
					{
						return false;
					}
					if (assemblyVersion != null && !assemblyVersion.Equals(strongName.Version))
					{
						return false;
					}
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new StrongNameMembershipCondition(blob, name, assemblyVersion);
		}

		public override bool Equals(object o)
		{
			StrongNameMembershipCondition strongNameMembershipCondition = o as StrongNameMembershipCondition;
			if (strongNameMembershipCondition == null)
			{
				return false;
			}
			if (!strongNameMembershipCondition.PublicKey.Equals(PublicKey))
			{
				return false;
			}
			if (name != strongNameMembershipCondition.Name)
			{
				return false;
			}
			if (assemblyVersion != null)
			{
				return assemblyVersion.Equals(strongNameMembershipCondition.Version);
			}
			return strongNameMembershipCondition.Version == null;
		}

		public override int GetHashCode()
		{
			return blob.GetHashCode();
		}

		public void FromXml(SecurityElement e)
		{
			FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", version, version);
			blob = StrongNamePublicKeyBlob.FromString(e.Attribute("PublicKeyBlob"));
			name = e.Attribute("Name");
			string text = e.Attribute("AssemblyVersion");
			if (text == null)
			{
				assemblyVersion = null;
			}
			else
			{
				assemblyVersion = new Version(text);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder("StrongName - ");
			stringBuilder.Append(blob);
			if (name != null)
			{
				stringBuilder.AppendFormat(" name = {0}", name);
			}
			if (assemblyVersion != null)
			{
				stringBuilder.AppendFormat(" version = {0}", assemblyVersion);
			}
			return stringBuilder.ToString();
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = MembershipConditionHelper.Element(typeof(StrongNameMembershipCondition), version);
			if (blob != null)
			{
				securityElement.AddAttribute("PublicKeyBlob", blob.ToString());
			}
			if (name != null)
			{
				securityElement.AddAttribute("Name", name);
			}
			if (assemblyVersion != null)
			{
				string text = assemblyVersion.ToString();
				if (text != "0.0")
				{
					securityElement.AddAttribute("AssemblyVersion", text);
				}
			}
			return securityElement;
		}
	}
}
