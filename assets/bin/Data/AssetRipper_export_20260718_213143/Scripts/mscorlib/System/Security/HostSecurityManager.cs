using System.Reflection;
using System.Runtime.Hosting;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace System.Security
{
	[Serializable]
	[ComVisible(true)]
	public class HostSecurityManager
	{
		public virtual PolicyLevel DomainPolicy
		{
			get
			{
				return null;
			}
		}

		public virtual HostSecurityManagerOptions Flags
		{
			get
			{
				return HostSecurityManagerOptions.AllFlags;
			}
		}

		public virtual ApplicationTrust DetermineApplicationTrust(Evidence applicationEvidence, Evidence activatorEvidence, TrustManagerContext context)
		{
			if (applicationEvidence == null)
			{
				throw new ArgumentNullException("applicationEvidence");
			}
			ActivationArguments activationArguments = null;
			foreach (object item in applicationEvidence)
			{
				activationArguments = item as ActivationArguments;
				if (activationArguments != null)
				{
					break;
				}
			}
			if (activationArguments == null)
			{
				string text = Locale.GetText("No {0} found in {1}.");
				throw new ArgumentException(string.Format(text, "ActivationArguments", "Evidence"), "applicationEvidence");
			}
			if (activationArguments.ActivationContext == null)
			{
				string text2 = Locale.GetText("No {0} found in {1}.");
				throw new ArgumentException(string.Format(text2, "ActivationContext", "ActivationArguments"), "applicationEvidence");
			}
			if (ApplicationSecurityManager.DetermineApplicationTrust(activationArguments.ActivationContext, context))
			{
				if (activationArguments.ApplicationIdentity == null)
				{
					return new ApplicationTrust();
				}
				return new ApplicationTrust(activationArguments.ApplicationIdentity);
			}
			return null;
		}

		public virtual Evidence ProvideAppDomainEvidence(Evidence inputEvidence)
		{
			return inputEvidence;
		}

		public virtual Evidence ProvideAssemblyEvidence(Assembly loadedAssembly, Evidence inputEvidence)
		{
			return inputEvidence;
		}

		public virtual PermissionSet ResolvePolicy(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new NullReferenceException("evidence");
			}
			return SecurityManager.ResolvePolicy(evidence);
		}
	}
}
