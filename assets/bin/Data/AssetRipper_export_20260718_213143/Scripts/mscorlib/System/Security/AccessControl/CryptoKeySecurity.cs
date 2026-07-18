using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CryptoKeySecurity : NativeObjectSecurity
	{
		public override Type AccessRightType
		{
			get
			{
				return typeof(CryptoKeyRights);
			}
		}

		public override Type AccessRuleType
		{
			get
			{
				return typeof(CryptoKeyAccessRule);
			}
		}

		public override Type AuditRuleType
		{
			get
			{
				return typeof(CryptoKeyAuditRule);
			}
		}

		[MonoTODO]
		public CryptoKeySecurity()
		{
		}

		[MonoTODO]
		public CryptoKeySecurity(CommonSecurityDescriptor securityDescriptor)
		{
		}

		public sealed override AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type)
		{
			return new CryptoKeyAccessRule(identityReference, (CryptoKeyRights)accessMask, type);
		}

		[MonoTODO]
		public void AddAccessRule(CryptoKeyAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAccessRule(CryptoKeyAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleAll(CryptoKeyAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAccessRuleSpecific(CryptoKeyAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void ResetAccessRule(CryptoKeyAccessRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAccessRule(CryptoKeyAccessRule rule)
		{
			throw new NotImplementedException();
		}

		public sealed override AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags)
		{
			return new CryptoKeyAuditRule(identityReference, (CryptoKeyRights)accessMask, flags);
		}

		[MonoTODO]
		public void AddAuditRule(CryptoKeyAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public bool RemoveAuditRule(CryptoKeyAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleAll(CryptoKeyAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void RemoveAuditRuleSpecific(CryptoKeyAuditRule rule)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetAuditRule(CryptoKeyAuditRule rule)
		{
			throw new NotImplementedException();
		}
	}
}
