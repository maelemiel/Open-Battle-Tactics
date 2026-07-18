using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class RegistrySecurity : NativeObjectSecurity
	{
		public override Type AccessRightType
		{
			get
			{
				return typeof(RegistryRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(RegistryAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(RegistryAuditRule);
			}
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new RegistryAccessRule(identityReference, (RegistryRights)accessMask, inheritanceFlags, propagationFlags, type);
		}

		public void AddAccessRule(RegistryAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void AddAuditRule(RegistryAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new RegistryAuditRule(identityReference, (RegistryRights)accessMask, inheritanceFlags, propagationFlags, flags);
		}

		public bool RemoveAccessRule(RegistryAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAccessRuleAll(RegistryAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAccessRuleSpecific(RegistryAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public bool RemoveAuditRule(RegistryAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAuditRuleAll(RegistryAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public void RemoveAuditRuleSpecific(RegistryAuditRule rule)
		{
			throw new NotImplementedException();
		}

		public void ResetAccessRule(RegistryAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void SetAccessRule(RegistryAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public void SetAuditRule(RegistryAuditRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
