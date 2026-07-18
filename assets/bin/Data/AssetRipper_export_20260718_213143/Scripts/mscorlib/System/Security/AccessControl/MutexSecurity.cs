using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class MutexSecurity : NativeObjectSecurity
	{
		public override Type AccessRightType
		{
			get
			{
				return typeof(MutexRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(MutexAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(MutexAuditRule);
			}
		}

		public MutexSecurity()
		{
		}

		public MutexSecurity(string name, AccessControlSections includeSections)
		{
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new MutexAccessRule(identityReference, (MutexRights)accessMask, type);
		}

		[MonoTODO]
		public void AddAccessRule(MutexAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAccessRule(MutexAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleAll(MutexAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleSpecific(MutexAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void ResetAccessRule(MutexAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAccessRule(MutexAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new MutexAuditRule(identityReference, (MutexRights)accessMask, flags);
		}

		[MonoTODO]
		public void AddAuditRule(MutexAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAuditRule(MutexAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleAll(MutexAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleSpecific(MutexAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAuditRule(MutexAuditRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
