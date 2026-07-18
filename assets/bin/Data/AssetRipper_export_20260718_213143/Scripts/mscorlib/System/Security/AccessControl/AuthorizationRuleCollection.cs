using System.Collections;

namespace System.Security.AccessControl
{
	public sealed class AuthorizationRuleCollection : ReadOnlyCollectionBase
	{
		public AuthorizationRule this[int index]
		{
			get
			{
				return (AuthorizationRule)base.InnerList[index];
			}
		}

		private AuthorizationRuleCollection(AuthorizationRule[] rules)
		{
			base.InnerList.AddRange(rules);
		}

		public void CopyTo(AuthorizationRule[] rules, int index)
		{
			base.InnerList.CopyTo(rules, index);
		}
	}
}
