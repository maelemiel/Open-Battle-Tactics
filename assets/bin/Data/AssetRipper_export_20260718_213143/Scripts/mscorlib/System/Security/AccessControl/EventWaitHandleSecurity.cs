using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class EventWaitHandleSecurity : NativeObjectSecurity
	{
		public override Type AccessRightType
		{
			get
			{
				return typeof(EventWaitHandleRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(EventWaitHandleAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(EventWaitHandleAuditRule);
			}
		}

		public EventWaitHandleSecurity()
		{
			throw new NotImplementedException();
		}

		public override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new EventWaitHandleAccessRule(identityReference, (EventWaitHandleRights)accessMask, type);
		}

		[MonoTODO]
		public void AddAccessRule(EventWaitHandleAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAccessRule(EventWaitHandleAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleAll(EventWaitHandleAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleSpecific(EventWaitHandleAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void ResetAccessRule(EventWaitHandleAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAccessRule(EventWaitHandleAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new EventWaitHandleAuditRule(identityReference, (EventWaitHandleRights)accessMask, flags);
		}

		[MonoTODO]
		public void AddAuditRule(EventWaitHandleAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAuditRule(EventWaitHandleAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleAll(EventWaitHandleAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleSpecific(EventWaitHandleAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAuditRule(EventWaitHandleAuditRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
