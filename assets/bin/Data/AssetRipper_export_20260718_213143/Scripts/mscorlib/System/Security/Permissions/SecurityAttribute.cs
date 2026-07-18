using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[Obsolete("CAS support is not available with Silverlight applications.")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public abstract class SecurityAttribute : Attribute
	{
		private SecurityAction m_Action;

		private bool m_Unrestricted;

		public bool Unrestricted
		{
			get
			{
				return m_Unrestricted;
			}
			set
			{
				m_Unrestricted = value;
			}
		}

		public SecurityAction Action
		{
			get
			{
				return m_Action;
			}
			set
			{
				m_Action = value;
			}
		}

		protected SecurityAttribute(SecurityAction action)
		{
			Action = action;
		}

		public abstract IPermission CreatePermission();
	}
}
