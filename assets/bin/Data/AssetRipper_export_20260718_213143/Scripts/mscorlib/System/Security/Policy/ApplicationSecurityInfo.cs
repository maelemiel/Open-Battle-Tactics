using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public sealed class ApplicationSecurityInfo
	{
		private ActivationContext _context;

		private Evidence _evidence;

		private ApplicationId _appid;

		private PermissionSet _defaultSet;

		private ApplicationId _deployid;

		public Evidence ApplicationEvidence
		{
			get
			{
				return _evidence;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ApplicationEvidence");
				}
				_evidence = value;
			}
		}

		public ApplicationId ApplicationId
		{
			get
			{
				return _appid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("ApplicationId");
				}
				_appid = value;
			}
		}

		public PermissionSet DefaultRequestSet
		{
			get
			{
				if (_defaultSet == null)
				{
					return new PermissionSet(PermissionState.None);
				}
				return _defaultSet;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("DefaultRequestSet");
				}
				_defaultSet = value;
			}
		}

		public ApplicationId DeploymentId
		{
			get
			{
				return _deployid;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("DeploymentId");
				}
				_deployid = value;
			}
		}

		public ApplicationSecurityInfo(ActivationContext activationContext)
		{
			if (activationContext == null)
			{
				throw new ArgumentNullException("activationContext");
			}
			_context = activationContext;
		}
	}
}
