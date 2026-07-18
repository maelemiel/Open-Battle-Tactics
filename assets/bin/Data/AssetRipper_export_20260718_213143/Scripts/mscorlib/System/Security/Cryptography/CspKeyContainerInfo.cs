using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class CspKeyContainerInfo
	{
		private CspParameters _params;

		internal bool _random;

		public bool Accessible
		{
			get
			{
				return true;
			}
		}

		public CryptoKeySecurity CryptoKeySecurity
		{
			get
			{
				return null;
			}
		}

		public bool Exportable
		{
			get
			{
				return true;
			}
		}

		public bool HardwareDevice
		{
			get
			{
				return false;
			}
		}

		public string KeyContainerName
		{
			get
			{
				return _params.KeyContainerName;
			}
		}

		public KeyNumber KeyNumber
		{
			get
			{
				return (KeyNumber)_params.KeyNumber;
			}
		}

		public bool MachineKeyStore
		{
			get
			{
				return false;
			}
		}

		public bool Protected
		{
			get
			{
				return false;
			}
		}

		public string ProviderName
		{
			get
			{
				return _params.ProviderName;
			}
		}

		public int ProviderType
		{
			get
			{
				return _params.ProviderType;
			}
		}

		public bool RandomlyGenerated
		{
			get
			{
				return _random;
			}
		}

		public bool Removable
		{
			get
			{
				return false;
			}
		}

		public string UniqueKeyContainerName
		{
			get
			{
				return _params.ProviderName + "\\" + _params.KeyContainerName;
			}
		}

		public CspKeyContainerInfo(CspParameters parameters)
		{
			_params = parameters;
			_random = true;
		}
	}
}
