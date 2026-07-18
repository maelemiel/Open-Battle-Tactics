using System.Runtime.InteropServices;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	[ComVisible(false)]
	public sealed class SemaphoreSecurity : NativeObjectSecurity
	{
		public override Type AccessRightType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public SemaphoreSecurity()
			: base(false, ResourceType.Unknown)
		{
		}

		public SemaphoreSecurity(string name, AccessControlSections includesections)
			: base(false, ResourceType.Unknown, name, includesections)
		{
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			throw new NotImplementedException();
		}

		public void AddAccessRule(SemaphoreAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void AddAuditRule(SemaphoreAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			throw new NotImplementedException();
		}

		public bool RemoveAccessRule(SemaphoreAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAccessRuleAll(SemaphoreAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAccessRuleSpecific(SemaphoreAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public bool RemoveAuditRule(SemaphoreAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAuditRuleAll(SemaphoreAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAuditRuleSpecific(SemaphoreAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public void ResetAccessRule(SemaphoreAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void SetAccessRule(SemaphoreAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void SetAuditRule(SemaphoreAuditRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
