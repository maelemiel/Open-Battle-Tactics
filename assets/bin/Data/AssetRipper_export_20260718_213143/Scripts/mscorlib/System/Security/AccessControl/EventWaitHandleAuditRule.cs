using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class EventWaitHandleAuditRule : AuditRule
	{
		private EventWaitHandleRights rights;

		public EventWaitHandleRights EventWaitHandleRights
		{
			get
			{
				return rights;
			}
		}

		public EventWaitHandleAuditRule(IdentityReference identity, EventWaitHandleRights eventRights, AuditFlags flags)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
			if (eventRights < EventWaitHandleRights.Modify || eventRights > EventWaitHandleRights.FullControl)
			{
				throw new ArgumentOutOfRangeException("eventRights");
			}
			if (flags < AuditFlags.None || flags > AuditFlags.Failure)
			{
				throw new ArgumentOutOfRangeException("flags");
			}
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (eventRights == (EventWaitHandleRights)0)
			{
				throw new ArgumentNullException("eventRights");
			}
			if (flags == AuditFlags.None)
			{
				throw new ArgumentException("flags");
			}
			if (!(identity is SecurityIdentifier))
			{
				throw new ArgumentException("identity");
			}
			rights = eventRights;
		}
	}
}
