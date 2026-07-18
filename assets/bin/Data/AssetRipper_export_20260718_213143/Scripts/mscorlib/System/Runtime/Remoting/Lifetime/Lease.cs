using System.Collections;
using System.Threading;

namespace System.Runtime.Remoting.Lifetime
{
	internal class Lease : MarshalByRefObject, ILease
	{
		private delegate TimeSpan RenewalDelegate(ILease lease);

		private DateTime _leaseExpireTime;

		private LeaseState _currentState;

		private TimeSpan _initialLeaseTime;

		private TimeSpan _renewOnCallTime;

		private TimeSpan _sponsorshipTimeout;

		private ArrayList _sponsors;

		private Queue _renewingSponsors;

		private RenewalDelegate _renewalDelegate;

		public TimeSpan CurrentLeaseTime
		{
			get
			{
				return _leaseExpireTime - DateTime.Now;
			}
		}

		public LeaseState CurrentState
		{
			get
			{
				return _currentState;
			}
		}

		public TimeSpan InitialLeaseTime
		{
			get
			{
				return _initialLeaseTime;
			}
			set
			{
				if (_currentState != LeaseState.Initial)
				{
					throw new RemotingException(string.Concat("InitialLeaseTime property can only be set when the lease is in initial state; state is ", _currentState, "."));
				}
				_initialLeaseTime = value;
				_leaseExpireTime = DateTime.Now + _initialLeaseTime;
				if (value == TimeSpan.Zero)
				{
					_currentState = LeaseState.Null;
				}
			}
		}

		public TimeSpan RenewOnCallTime
		{
			get
			{
				return _renewOnCallTime;
			}
			set
			{
				if (_currentState != LeaseState.Initial)
				{
					throw new RemotingException(string.Concat("RenewOnCallTime property can only be set when the lease is in initial state; state is ", _currentState, "."));
				}
				_renewOnCallTime = value;
			}
		}

		public TimeSpan SponsorshipTimeout
		{
			get
			{
				return _sponsorshipTimeout;
			}
			set
			{
				if (_currentState != LeaseState.Initial)
				{
					throw new RemotingException(string.Concat("SponsorshipTimeout property can only be set when the lease is in initial state; state is ", _currentState, "."));
				}
				_sponsorshipTimeout = value;
			}
		}

		public Lease()
		{
			_currentState = LeaseState.Initial;
			_initialLeaseTime = LifetimeServices.LeaseTime;
			_renewOnCallTime = LifetimeServices.RenewOnCallTime;
			_sponsorshipTimeout = LifetimeServices.SponsorshipTimeout;
			_leaseExpireTime = DateTime.Now + _initialLeaseTime;
		}

		public void Activate()
		{
			_currentState = LeaseState.Active;
		}

		public void Register(ISponsor obj)
		{
			Register(obj, TimeSpan.Zero);
		}

		public void Register(ISponsor obj, TimeSpan renewalTime)
		{
			lock (this)
			{
				if (_sponsors == null)
				{
					_sponsors = new ArrayList();
				}
				_sponsors.Add(obj);
			}
			if (renewalTime != TimeSpan.Zero)
			{
				Renew(renewalTime);
			}
		}

		public TimeSpan Renew(TimeSpan renewalTime)
		{
			DateTime dateTime = DateTime.Now + renewalTime;
			if (dateTime > _leaseExpireTime)
			{
				_leaseExpireTime = dateTime;
			}
			return CurrentLeaseTime;
		}

		public void Unregister(ISponsor obj)
		{
			lock (this)
			{
				if (_sponsors == null)
				{
					return;
				}
				for (int i = 0; i < _sponsors.Count; i++)
				{
					if (object.ReferenceEquals(_sponsors[i], obj))
					{
						_sponsors.RemoveAt(i);
						break;
					}
				}
			}
		}

		internal void UpdateState()
		{
			if (_currentState != LeaseState.Active || CurrentLeaseTime > TimeSpan.Zero)
			{
				return;
			}
			if (_sponsors != null)
			{
				_currentState = LeaseState.Renewing;
				lock (this)
				{
					_renewingSponsors = new Queue(_sponsors);
				}
				CheckNextSponsor();
			}
			else
			{
				_currentState = LeaseState.Expired;
			}
		}

		private void CheckNextSponsor()
		{
			if (_renewingSponsors.Count == 0)
			{
				_currentState = LeaseState.Expired;
				_renewingSponsors = null;
				return;
			}
			ISponsor sponsor = (ISponsor)_renewingSponsors.Peek();
			_renewalDelegate = sponsor.Renewal;
			IAsyncResult asyncResult = _renewalDelegate.BeginInvoke(this, null, null);
			ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, ProcessSponsorResponse, asyncResult, _sponsorshipTimeout, true);
		}

		private void ProcessSponsorResponse(object state, bool timedOut)
		{
			if (!timedOut)
			{
				try
				{
					IAsyncResult result = (IAsyncResult)state;
					TimeSpan timeSpan = _renewalDelegate.EndInvoke(result);
					if (timeSpan != TimeSpan.Zero)
					{
						Renew(timeSpan);
						_currentState = LeaseState.Active;
						_renewingSponsors = null;
						return;
					}
				}
				catch
				{
				}
			}
			Unregister((ISponsor)_renewingSponsors.Dequeue());
			CheckNextSponsor();
		}
	}
}
