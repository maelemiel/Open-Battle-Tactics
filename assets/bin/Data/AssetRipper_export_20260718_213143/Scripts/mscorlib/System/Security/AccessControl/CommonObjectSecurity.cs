using System.Collections.Generic;

namespace System.Security.AccessControl
{
	[MonoTODO("required for NativeObjectSecurity - implementation is missing")]
	public abstract class CommonObjectSecurity : ObjectSecurity
	{
		private List<AccessRule> access_rules = new List<AccessRule>();

		private List<AuditRule> audit_rules = new List<AuditRule>();

		protected CommonObjectSecurity(bool isContainer)
			: base(isContainer, false)
		{
		}

		public AuthorizationRuleCollection GetAccessRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			throw new NotImplementedException();
		}

		public AuthorizationRuleCollection GetAuditRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			throw new NotImplementedException();
		}

		protected void AddAccessRule(AccessRule rule)
		{
			access_rules.Add(rule);
			base.AccessRulesModified = true;
		}

		protected bool RemoveAccessRule(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAccessRuleAll(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAccessRuleSpecific(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void ResetAccessRule(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected void SetAccessRule(AccessRule rule)
		{
			throw new NotImplementedException();
		}

		protected override bool ModifyAccess(AccessControlModification modification, AccessRule rule, out bool modified)
		{
			foreach (AccessRule access_rule in access_rules)
			{
				if (rule != access_rule)
				{
					continue;
				}
				switch (modification)
				{
				case AccessControlModification.Add:
					AddAccessRule(rule);
					break;
				case AccessControlModification.Set:
					SetAccessRule(rule);
					break;
				case AccessControlModification.Reset:
					ResetAccessRule(rule);
					break;
				case AccessControlModification.Remove:
					RemoveAccessRule(rule);
					break;
				case AccessControlModification.RemoveAll:
					RemoveAccessRuleAll(rule);
					break;
				case AccessControlModification.RemoveSpecific:
					RemoveAccessRuleSpecific(rule);
					break;
				}
				modified = true;
				return true;
			}
			modified = false;
			return false;
		}

		protected void AddAuditRule(AuditRule rule)
		{
			audit_rules.Add(rule);
			base.AuditRulesModified = true;
		}

		protected bool RemoveAuditRule(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAuditRuleAll(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void RemoveAuditRuleSpecific(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected void SetAuditRule(AuditRule rule)
		{
			throw new NotImplementedException();
		}

		protected override bool ModifyAudit(AccessControlModification modification, AuditRule rule, out bool modified)
		{
			foreach (AuditRule audit_rule in audit_rules)
			{
				if (rule != audit_rule)
				{
					continue;
				}
				switch (modification)
				{
				case AccessControlModification.Add:
					AddAuditRule(rule);
					break;
				case AccessControlModification.Set:
					SetAuditRule(rule);
					break;
				case AccessControlModification.Remove:
					RemoveAuditRule(rule);
					break;
				case AccessControlModification.RemoveAll:
					RemoveAuditRuleAll(rule);
					break;
				case AccessControlModification.RemoveSpecific:
					RemoveAuditRuleSpecific(rule);
					break;
				}
				base.AuditRulesModified = true;
				modified = true;
				return true;
			}
			modified = false;
			return false;
		}
	}
}
