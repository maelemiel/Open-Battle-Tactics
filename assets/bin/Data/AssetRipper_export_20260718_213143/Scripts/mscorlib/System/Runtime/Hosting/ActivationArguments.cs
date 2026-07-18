using System.Runtime.InteropServices;

namespace System.Runtime.Hosting
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ActivationArguments
	{
		private ActivationContext _context;

		private ApplicationIdentity _identity;

		private string[] _data;

		public ActivationContext ActivationContext
		{
			get
			{
				return _context;
			}
		}

		public string[] ActivationData
		{
			get
			{
				return _data;
			}
		}

		public ApplicationIdentity ApplicationIdentity
		{
			get
			{
				return _identity;
			}
		}

		public ActivationArguments(ActivationContext activationData)
		{
			if (activationData == null)
			{
				throw new ArgumentNullException("activationData");
			}
			_context = activationData;
			_identity = activationData.Identity;
		}

		public ActivationArguments(ApplicationIdentity applicationIdentity)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			_identity = applicationIdentity;
		}

		public ActivationArguments(ActivationContext activationContext, string[] activationData)
		{
			if (activationContext == null)
			{
				throw new ArgumentNullException("activationContext");
			}
			_context = activationContext;
			_identity = activationContext.Identity;
			_data = activationData;
		}

		public ActivationArguments(ApplicationIdentity applicationIdentity, string[] activationData)
		{
			if (applicationIdentity == null)
			{
				throw new ArgumentNullException("applicationIdentity");
			}
			_identity = applicationIdentity;
			_data = activationData;
		}
	}
}
