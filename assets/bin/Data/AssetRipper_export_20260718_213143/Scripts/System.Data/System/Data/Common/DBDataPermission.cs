using System.Collections;
using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
	[Serializable]
	public abstract class DBDataPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private bool allowBlankPassword;

		private PermissionState state;

		private Hashtable _connections;

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

		[Obsolete("use DBDataPermission (PermissionState.None)", true)]
		protected DBDataPermission()
			: this(PermissionState.None)
		{
		}

		protected DBDataPermission(DBDataPermission permission)
		{
			if (permission == null)
			{
				throw new ArgumentNullException("permission");
			}
			state = permission.state;
			if (state != PermissionState.Unrestricted)
			{
				allowBlankPassword = permission.allowBlankPassword;
				_connections = (Hashtable)permission._connections.Clone();
			}
		}

		protected DBDataPermission(DBDataPermissionAttribute permissionAttribute)
		{
			if (permissionAttribute == null)
			{
				throw new ArgumentNullException("permissionAttribute");
			}
			_connections = new Hashtable();
			if (permissionAttribute.Unrestricted)
			{
				state = PermissionState.Unrestricted;
				return;
			}
			state = PermissionState.None;
			allowBlankPassword = permissionAttribute.AllowBlankPassword;
			if (permissionAttribute.ConnectionString.Length > 0)
			{
				Add(permissionAttribute.ConnectionString, permissionAttribute.KeyRestrictions, permissionAttribute.KeyRestrictionBehavior);
			}
		}

		protected DBDataPermission(PermissionState state)
		{
			this.state = PermissionHelper.CheckPermissionState(state, true);
			_connections = new Hashtable();
		}

		[Obsolete("use DBDataPermission (PermissionState.None)", true)]
		protected DBDataPermission(PermissionState state, bool allowBlankPassword)
			: this(state)
		{
			this.allowBlankPassword = allowBlankPassword;
		}

		public virtual void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior)
		{
			state = PermissionState.None;
			_connections[connectionString] = new object[2] { restrictions, behavior };
		}

		protected void Clear()
		{
			_connections.Clear();
		}

		public override IPermission Copy()
		{
			DBDataPermission dBDataPermission = CreateInstance();
			dBDataPermission.allowBlankPassword = allowBlankPassword;
			dBDataPermission._connections = (Hashtable)_connections.Clone();
			return dBDataPermission;
		}

		protected virtual DBDataPermission CreateInstance()
		{
			return (DBDataPermission)Activator.CreateInstance(GetType(), PermissionState.None);
		}

		public override void FromXml(SecurityElement securityElement)
		{
			PermissionHelper.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			state = (PermissionHelper.IsUnrestricted(securityElement) ? PermissionState.Unrestricted : PermissionState.None);
			allowBlankPassword = false;
			string text = securityElement.Attribute("AllowBlankPassword");
			if (text != null && !bool.TryParse(text, out allowBlankPassword))
			{
				allowBlankPassword = false;
			}
			if (securityElement.Children == null)
			{
				return;
			}
			foreach (SecurityElement child in securityElement.Children)
			{
				string text2 = child.Attribute("ConnectionString");
				string restrictions = child.Attribute("KeyRestrictions");
				KeyRestrictionBehavior behavior = (KeyRestrictionBehavior)(int)Enum.Parse(typeof(KeyRestrictionBehavior), child.Attribute("KeyRestrictionBehavior"));
				if (text2 != null && text2.Length > 0)
				{
					Add(text2, restrictions, behavior);
				}
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			DBDataPermission dBDataPermission = Cast(target);
			if (dBDataPermission == null)
			{
				return null;
			}
			if (IsUnrestricted())
			{
				if (dBDataPermission.IsUnrestricted())
				{
					DBDataPermission dBDataPermission2 = CreateInstance();
					dBDataPermission2.state = PermissionState.Unrestricted;
					return dBDataPermission2;
				}
				return dBDataPermission.Copy();
			}
			if (dBDataPermission.IsUnrestricted())
			{
				return Copy();
			}
			if (IsEmpty() || dBDataPermission.IsEmpty())
			{
				return null;
			}
			DBDataPermission dBDataPermission3 = CreateInstance();
			dBDataPermission3.allowBlankPassword = allowBlankPassword && dBDataPermission.allowBlankPassword;
			foreach (DictionaryEntry connection in _connections)
			{
				object obj = dBDataPermission._connections[connection.Key];
				if (obj != null)
				{
					dBDataPermission3._connections.Add(connection.Key, connection.Value);
				}
			}
			return (dBDataPermission3._connections.Count <= 0) ? null : dBDataPermission3;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			DBDataPermission dBDataPermission = Cast(target);
			if (dBDataPermission == null)
			{
				return IsEmpty();
			}
			if (dBDataPermission.IsUnrestricted())
			{
				return true;
			}
			if (IsUnrestricted())
			{
				return dBDataPermission.IsUnrestricted();
			}
			if (allowBlankPassword && !dBDataPermission.allowBlankPassword)
			{
				return false;
			}
			if (_connections.Count > dBDataPermission._connections.Count)
			{
				return false;
			}
			foreach (DictionaryEntry connection in _connections)
			{
				object obj = dBDataPermission._connections[connection.Key];
				if (obj == null)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			return state == PermissionState.Unrestricted;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = PermissionHelper.Element(GetType(), 1);
			if (IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else
			{
				securityElement.AddAttribute("AllowBlankPassword", allowBlankPassword.ToString());
				foreach (DictionaryEntry connection in _connections)
				{
					SecurityElement securityElement2 = new SecurityElement("add");
					securityElement2.AddAttribute("ConnectionString", (string)connection.Key);
					object[] array = (object[])connection.Value;
					securityElement2.AddAttribute("KeyRestrictions", (string)array[0]);
					KeyRestrictionBehavior keyRestrictionBehavior = (KeyRestrictionBehavior)(int)array[1];
					securityElement2.AddAttribute("KeyRestrictionBehavior", keyRestrictionBehavior.ToString());
					securityElement.AddChild(securityElement2);
				}
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			DBDataPermission dBDataPermission = Cast(target);
			if (dBDataPermission == null)
			{
				return Copy();
			}
			if (IsEmpty() && dBDataPermission.IsEmpty())
			{
				return Copy();
			}
			DBDataPermission dBDataPermission2 = CreateInstance();
			if (IsUnrestricted() || dBDataPermission.IsUnrestricted())
			{
				dBDataPermission2.state = PermissionState.Unrestricted;
			}
			else
			{
				dBDataPermission2.allowBlankPassword = allowBlankPassword || dBDataPermission.allowBlankPassword;
				dBDataPermission2._connections = new Hashtable(_connections.Count + dBDataPermission._connections.Count);
				foreach (DictionaryEntry connection in _connections)
				{
					dBDataPermission2._connections.Add(connection.Key, connection.Value);
				}
				foreach (DictionaryEntry connection2 in dBDataPermission._connections)
				{
					dBDataPermission2._connections[connection2.Key] = connection2.Value;
				}
			}
			return dBDataPermission2;
		}

		private bool IsEmpty()
		{
			return state != PermissionState.Unrestricted && _connections.Count == 0;
		}

		private DBDataPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			DBDataPermission dBDataPermission = target as DBDataPermission;
			if (dBDataPermission == null)
			{
				PermissionHelper.ThrowInvalidPermission(target, GetType());
			}
			return dBDataPermission;
		}
	}
}
