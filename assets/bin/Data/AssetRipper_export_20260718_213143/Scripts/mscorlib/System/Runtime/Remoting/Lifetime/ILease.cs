using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Lifetime
{
	[ComVisible(true)]
	public interface ILease
	{
		TimeSpan CurrentLeaseTime { get; }

		LeaseState CurrentState { get; }

		TimeSpan InitialLeaseTime { get; set; }

		TimeSpan RenewOnCallTime { get; set; }

		TimeSpan SponsorshipTimeout { get; set; }

		void Register(ISponsor obj);

		void Register(ISponsor obj, TimeSpan renewalTime);

		TimeSpan Renew(TimeSpan renewalTime);

		void Unregister(ISponsor obj);
	}
}
