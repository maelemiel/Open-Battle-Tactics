using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class PermissionRequestEvidence : IBuiltInEvidence
	{
		private PermissionSet requested;

		private PermissionSet optional;

		private PermissionSet denied;

		public PermissionSet DeniedPermissions
		{
			get
			{
				return denied;
			}
		}

		public PermissionSet OptionalPermissions
		{
			get
			{
				return optional;
			}
		}

		public PermissionSet RequestedPermissions
		{
			get
			{
				return requested;
			}
		}

		public PermissionRequestEvidence(PermissionSet request, PermissionSet optional, PermissionSet denied)
		{
			if (request != null)
			{
				requested = new PermissionSet(request);
			}
			if (optional != null)
			{
				this.optional = new PermissionSet(optional);
			}
			if (denied != null)
			{
				this.denied = new PermissionSet(denied);
			}
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			int num = ((!verbose) ? 1 : 3);
			if (requested != null)
			{
				int num2 = requested.ToXml().ToString().Length + (verbose ? 5 : 0);
				num += num2;
			}
			if (optional != null)
			{
				int num3 = optional.ToXml().ToString().Length + (verbose ? 5 : 0);
				num += num3;
			}
			if (denied != null)
			{
				int num4 = denied.ToXml().ToString().Length + (verbose ? 5 : 0);
				num += num4;
			}
			return num;
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

		public PermissionRequestEvidence Copy()
		{
			return new PermissionRequestEvidence(requested, optional, denied);
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.PermissionRequestEvidence");
			securityElement.AddAttribute("version", "1");
			if (requested != null)
			{
				SecurityElement securityElement2 = new SecurityElement("Request");
				securityElement2.AddChild(requested.ToXml());
				securityElement.AddChild(securityElement2);
			}
			if (optional != null)
			{
				SecurityElement securityElement3 = new SecurityElement("Optional");
				securityElement3.AddChild(optional.ToXml());
				securityElement.AddChild(securityElement3);
			}
			if (denied != null)
			{
				SecurityElement securityElement4 = new SecurityElement("Denied");
				securityElement4.AddChild(denied.ToXml());
				securityElement.AddChild(securityElement4);
			}
			return securityElement.ToString();
		}
	}
}
