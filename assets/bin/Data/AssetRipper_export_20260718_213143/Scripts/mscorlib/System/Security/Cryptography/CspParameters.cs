using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class CspParameters
	{
		private CspProviderFlags _Flags;

		public string KeyContainerName;

		public int KeyNumber;

		public string ProviderName;

		public int ProviderType;

		private SecureString _password;

		private IntPtr _windowHandle;

		public CspProviderFlags Flags
		{
			get
			{
				return _Flags;
			}
			set
			{
				_Flags = value;
			}
		}

		[MonoTODO("access control isn't implemented")]
		public CryptoKeySecurity CryptoKeySecurity
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public SecureString KeyPassword
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}

		public IntPtr ParentWindowHandle
		{
			get
			{
				return _windowHandle;
			}
			set
			{
				_windowHandle = value;
			}
		}

		public CspParameters()
			: this(1)
		{
		}

		public CspParameters(int dwTypeIn)
			: this(dwTypeIn, null)
		{
		}

		public CspParameters(int dwTypeIn, string strProviderNameIn)
			: this(dwTypeIn, null, null)
		{
		}

		public CspParameters(int dwTypeIn, string strProviderNameIn, string strContainerNameIn)
		{
			ProviderType = dwTypeIn;
			ProviderName = strProviderNameIn;
			KeyContainerName = strContainerNameIn;
			KeyNumber = -1;
		}

		public CspParameters(int providerType, string providerName, string keyContainerName, CryptoKeySecurity cryptoKeySecurity, IntPtr parentWindowHandle)
			: this(providerType, providerName, keyContainerName)
		{
			if (cryptoKeySecurity != null)
			{
				CryptoKeySecurity = cryptoKeySecurity;
			}
			_windowHandle = parentWindowHandle;
		}

		public CspParameters(int providerType, string providerName, string keyContainerName, CryptoKeySecurity cryptoKeySecurity, SecureString keyPassword)
			: this(providerType, providerName, keyContainerName)
		{
			if (cryptoKeySecurity != null)
			{
				CryptoKeySecurity = cryptoKeySecurity;
			}
			_password = keyPassword;
		}
	}
}
