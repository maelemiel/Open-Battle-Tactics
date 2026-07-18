using System.ComponentModel;
using System.Security.Permissions;

namespace System.Data.Common
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public abstract class DBDataPermissionAttribute : CodeAccessSecurityAttribute
	{
		private bool allowBlankPassword;

		private string keyRestrictions;

		private KeyRestrictionBehavior keyRestrictionBehavior;

		private string connectionString;

		public bool AllowBlankPassword
		{
			get
			{
				return allowBlankPassword;
			}
			set
			{
				allowBlankPassword = value;
			}
		}

		public string KeyRestrictions
		{
			get
			{
				if (keyRestrictions == null)
				{
					return string.Empty;
				}
				return keyRestrictions;
			}
			set
			{
				keyRestrictions = value;
			}
		}

		public string ConnectionString
		{
			get
			{
				if (connectionString == null)
				{
					return string.Empty;
				}
				return connectionString;
			}
			set
			{
				connectionString = value;
			}
		}

		public KeyRestrictionBehavior KeyRestrictionBehavior
		{
			get
			{
				return keyRestrictionBehavior;
			}
			set
			{
				ExceptionHelper.CheckEnumValue(typeof(KeyRestrictionBehavior), value);
				keyRestrictionBehavior = value;
			}
		}

		protected DBDataPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeConnectionString()
		{
			return false;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerializeKeyRestrictions()
		{
			return false;
		}
	}
}
