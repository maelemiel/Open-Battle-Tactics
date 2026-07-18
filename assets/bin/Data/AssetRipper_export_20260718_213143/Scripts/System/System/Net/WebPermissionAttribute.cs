using System.Security;
using System.Security.Permissions;

namespace System.Net
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class WebPermissionAttribute : CodeAccessSecurityAttribute
	{
		private object m_accept;

		private object m_connect;

		public string Accept
		{
			get
			{
				if (m_accept == null)
				{
					return null;
				}
				return (m_accept as System.Net.WebPermissionInfo).Info;
			}
			set
			{
				if (m_accept != null)
				{
					AlreadySet("Accept", "Accept");
				}
				m_accept = new System.Net.WebPermissionInfo(System.Net.WebPermissionInfoType.InfoString, value);
			}
		}

		public string AcceptPattern
		{
			get
			{
				if (m_accept == null)
				{
					return null;
				}
				return (m_accept as System.Net.WebPermissionInfo).Info;
			}
			set
			{
				if (m_accept != null)
				{
					AlreadySet("Accept", "AcceptPattern");
				}
				if (value == null)
				{
					throw new ArgumentNullException("AcceptPattern");
				}
				m_accept = new System.Net.WebPermissionInfo(System.Net.WebPermissionInfoType.InfoUnexecutedRegex, value);
			}
		}

		public string Connect
		{
			get
			{
				if (m_connect == null)
				{
					return null;
				}
				return (m_connect as System.Net.WebPermissionInfo).Info;
			}
			set
			{
				if (m_connect != null)
				{
					AlreadySet("Connect", "Connect");
				}
				m_connect = new System.Net.WebPermissionInfo(System.Net.WebPermissionInfoType.InfoString, value);
			}
		}

		public string ConnectPattern
		{
			get
			{
				if (m_connect == null)
				{
					return null;
				}
				return (m_connect as System.Net.WebPermissionInfo).Info;
			}
			set
			{
				if (m_connect != null)
				{
					AlreadySet("Connect", "ConnectConnectPattern");
				}
				if (value == null)
				{
					throw new ArgumentNullException("ConnectPattern");
				}
				m_connect = new System.Net.WebPermissionInfo(System.Net.WebPermissionInfoType.InfoUnexecutedRegex, value);
			}
		}

		public WebPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new WebPermission(PermissionState.Unrestricted);
			}
			WebPermission webPermission = new WebPermission();
			if (m_accept != null)
			{
				webPermission.AddPermission(NetworkAccess.Accept, (System.Net.WebPermissionInfo)m_accept);
			}
			if (m_connect != null)
			{
				webPermission.AddPermission(NetworkAccess.Connect, (System.Net.WebPermissionInfo)m_connect);
			}
			return webPermission;
		}

		internal void AlreadySet(string parameter, string property)
		{
			string text = Locale.GetText("The parameter '{0}' can be set only once.");
			throw new ArgumentException(string.Format(text, parameter), property);
		}
	}
}
