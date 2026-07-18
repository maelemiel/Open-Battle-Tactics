using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[Serializable]
	[ComVisible(true)]
	public class GenericPrincipal : IPrincipal
	{
		private IIdentity m_identity;

		private string[] m_roles;

		public virtual IIdentity Identity
		{
			get
			{
				return m_identity;
			}
		}

		public GenericPrincipal(IIdentity identity, string[] roles)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			m_identity = identity;
			if (roles != null)
			{
				m_roles = new string[roles.Length];
				for (int i = 0; i < roles.Length; i++)
				{
					m_roles[i] = roles[i];
				}
			}
		}

		public virtual bool IsInRole(string role)
		{
			if (m_roles == null)
			{
				return false;
			}
			int length = role.Length;
			string[] roles = m_roles;
			foreach (string text in roles)
			{
				if (text != null && length == text.Length && string.Compare(role, 0, text, 0, length, true) == 0)
				{
					return true;
				}
			}
			return false;
		}
	}
}
