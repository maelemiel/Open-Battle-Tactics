using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
	public sealed class HostProtectionAttribute : CodeAccessSecurityAttribute
	{
		private HostProtectionResource _resources;

		public bool ExternalProcessMgmt
		{
			get
			{
				return (_resources & HostProtectionResource.ExternalProcessMgmt) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.ExternalProcessMgmt;
				}
				else
				{
					_resources &= ~HostProtectionResource.ExternalProcessMgmt;
				}
			}
		}

		public bool ExternalThreading
		{
			get
			{
				return (_resources & HostProtectionResource.ExternalThreading) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.ExternalThreading;
				}
				else
				{
					_resources &= ~HostProtectionResource.ExternalThreading;
				}
			}
		}

		public bool MayLeakOnAbort
		{
			get
			{
				return (_resources & HostProtectionResource.MayLeakOnAbort) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.MayLeakOnAbort;
				}
				else
				{
					_resources &= ~HostProtectionResource.MayLeakOnAbort;
				}
			}
		}

		[ComVisible(true)]
		public bool SecurityInfrastructure
		{
			get
			{
				return (_resources & HostProtectionResource.SecurityInfrastructure) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.SecurityInfrastructure;
				}
				else
				{
					_resources &= ~HostProtectionResource.SecurityInfrastructure;
				}
			}
		}

		public bool SelfAffectingProcessMgmt
		{
			get
			{
				return (_resources & HostProtectionResource.SelfAffectingProcessMgmt) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.SelfAffectingProcessMgmt;
				}
				else
				{
					_resources &= ~HostProtectionResource.SelfAffectingProcessMgmt;
				}
			}
		}

		public bool SelfAffectingThreading
		{
			get
			{
				return (_resources & HostProtectionResource.SelfAffectingThreading) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.SelfAffectingThreading;
				}
				else
				{
					_resources &= ~HostProtectionResource.SelfAffectingThreading;
				}
			}
		}

		public bool SharedState
		{
			get
			{
				return (_resources & HostProtectionResource.SharedState) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.SharedState;
				}
				else
				{
					_resources &= ~HostProtectionResource.SharedState;
				}
			}
		}

		public bool Synchronization
		{
			get
			{
				return (_resources & HostProtectionResource.Synchronization) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.Synchronization;
				}
				else
				{
					_resources &= ~HostProtectionResource.Synchronization;
				}
			}
		}

		public bool UI
		{
			get
			{
				return (_resources & HostProtectionResource.UI) != 0;
			}
			set
			{
				if (value)
				{
					_resources |= HostProtectionResource.UI;
				}
				else
				{
					_resources &= ~HostProtectionResource.UI;
				}
			}
		}

		public HostProtectionResource Resources
		{
			get
			{
				return _resources;
			}
			set
			{
				_resources = value;
			}
		}

		public HostProtectionAttribute()
			: base(SecurityAction.LinkDemand)
		{
		}

		public HostProtectionAttribute(SecurityAction action)
			: base(action)
		{
			if (action != SecurityAction.LinkDemand)
			{
				string message = string.Format(Locale.GetText("Only {0} is accepted."), SecurityAction.LinkDemand);
				throw new ArgumentException(message, "action");
			}
		}

		public override IPermission CreatePermission()
		{
			return null;
		}
	}
}
