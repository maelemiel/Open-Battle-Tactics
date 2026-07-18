using System.Data.Common;
using System.Security;
using System.Security.Permissions;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlClientPermission : DBDataPermission
	{
		[Obsolete("Use SqlClientPermission(PermissionState.None)", true)]
		public SqlClientPermission()
			: this(PermissionState.None)
		{
		}

		public SqlClientPermission(PermissionState state)
			: base(state)
		{
		}

		[Obsolete("Use SqlClientPermission(PermissionState.None)", true)]
		public SqlClientPermission(PermissionState state, bool allowBlankPassword)
			: base(state)
		{
			base.AllowBlankPassword = allowBlankPassword;
		}

		internal SqlClientPermission(DBDataPermission permission)
			: base(permission)
		{
		}

		internal SqlClientPermission(DBDataPermissionAttribute attribute)
			: base(attribute)
		{
		}

		public override IPermission Copy()
		{
			return new SqlClientPermission(this);
		}

		public override void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
			base.Add(connectionString, restrictions, behavior);
		}
	}
}
