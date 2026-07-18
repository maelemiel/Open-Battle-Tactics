using System.ComponentModel.Design;
using System.Reflection;

namespace System.ComponentModel
{
	public sealed class LicenseManager
	{
		private static LicenseContext mycontext;

		private static object contextLockUser;

		private static object lockObject = new object();

		public static LicenseContext CurrentContext
		{
			get
			{
				lock (lockObject)
				{
					if (mycontext == null)
					{
						mycontext = new System.ComponentModel.Design.RuntimeLicenseContext();
					}
					return mycontext;
				}
			}
			set
			{
				lock (lockObject)
				{
					if (contextLockUser == null)
					{
						mycontext = value;
						return;
					}
					throw new InvalidOperationException("The CurrentContext property of the LicenseManager is currently locked and cannot be changed.");
				}
			}
		}

		public static LicenseUsageMode UsageMode
		{
			get
			{
				return CurrentContext.UsageMode;
			}
		}

		private LicenseManager()
		{
		}

		public static object CreateWithContext(Type type, LicenseContext creationContext)
		{
			return CreateWithContext(type, creationContext, new object[0]);
		}

		public static object CreateWithContext(Type type, LicenseContext creationContext, object[] args)
		{
			object obj = null;
			lock (lockObject)
			{
				object contextUser = new object();
				LicenseContext currentContext = CurrentContext;
				CurrentContext = creationContext;
				LockContext(contextUser);
				try
				{
					return Activator.CreateInstance(type, args);
				}
				catch (TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
				finally
				{
					UnlockContext(contextUser);
					CurrentContext = currentContext;
				}
			}
		}

		public static bool IsLicensed(Type type)
		{
			License license = null;
			if (!privateGetLicense(type, null, false, out license))
			{
				return false;
			}
			if (license != null)
			{
				license.Dispose();
			}
			return true;
		}

		public static bool IsValid(Type type)
		{
			License license = null;
			if (!privateGetLicense(type, null, false, out license))
			{
				return false;
			}
			if (license != null)
			{
				license.Dispose();
			}
			return true;
		}

		public static bool IsValid(Type type, object instance, out License license)
		{
			return privateGetLicense(type, null, false, out license);
		}

		public static void LockContext(object contextUser)
		{
			lock (lockObject)
			{
				contextLockUser = contextUser;
			}
		}

		public static void UnlockContext(object contextUser)
		{
			lock (lockObject)
			{
				if (contextLockUser != null)
				{
					if (contextLockUser != contextUser)
					{
						throw new ArgumentException("The CurrentContext property of the LicenseManager can only be unlocked with the same contextUser.");
					}
					contextLockUser = null;
				}
			}
		}

		public static void Validate(Type type)
		{
			License license = null;
			if (!privateGetLicense(type, null, true, out license))
			{
				throw new LicenseException(type, null);
			}
			if (license != null)
			{
				license.Dispose();
			}
		}

		public static License Validate(Type type, object instance)
		{
			License license = null;
			if (!privateGetLicense(type, instance, true, out license))
			{
				throw new LicenseException(type, instance);
			}
			return license;
		}

		private static bool privateGetLicense(Type type, object instance, bool allowExceptions, out License license)
		{
			bool result = false;
			License license2 = null;
			LicenseProviderAttribute licenseProviderAttribute = (LicenseProviderAttribute)Attribute.GetCustomAttribute(type, typeof(LicenseProviderAttribute), true);
			if (licenseProviderAttribute != null)
			{
				Type licenseProvider = licenseProviderAttribute.LicenseProvider;
				if (licenseProvider != null)
				{
					LicenseProvider licenseProvider2 = (LicenseProvider)Activator.CreateInstance(licenseProvider);
					if (licenseProvider2 != null)
					{
						license2 = licenseProvider2.GetLicense(CurrentContext, type, instance, allowExceptions);
						if (license2 != null)
						{
							result = true;
						}
					}
				}
			}
			else
			{
				result = true;
			}
			license = license2;
			return result;
		}
	}
}
