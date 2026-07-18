using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlClient
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class SqlClientPermissionAttribute : DBDataPermissionAttribute
	{
		public SqlClientPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return new SqlClientPermission(this);
		}
	}
}
