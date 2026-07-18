using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class PrincipalPermission : IBuiltInPermission, IUnrestrictedPermission, IPermission, ISecurityEncodable
	{
		internal class PrincipalInfo
		{
			private string _name;

			private string _role;

			private bool _isAuthenticated;

			public string Name
			{
				get
				{
					return _name;
				}
			}

			public string Role
			{
				get
				{
					return _role;
				}
			}

			public bool IsAuthenticated
			{
				get
				{
					return _isAuthenticated;
				}
			}

			public PrincipalInfo(string name, string role, bool isAuthenticated)
			{
				_name = name;
				_role = role;
				_isAuthenticated = isAuthenticated;
			}
		}

		private const int version = 1;

		private ArrayList principals;

		public PrincipalPermission(PermissionState state)
		{
			principals = new ArrayList();
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				PrincipalInfo value = new PrincipalInfo(null, null, true);
				principals.Add(value);
			}
		}

		public PrincipalPermission(string name, string role)
			: this(name, role, true)
		{
		}

		public PrincipalPermission(string name, string role, bool isAuthenticated)
		{
			principals = new ArrayList();
			PrincipalInfo value = new PrincipalInfo(name, role, isAuthenticated);
			principals.Add(value);
		}

		internal PrincipalPermission(ArrayList principals)
		{
			this.principals = (ArrayList)principals.Clone();
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 8;
		}

		public IPermission Copy()
		{
			return new PrincipalPermission(principals);
		}

		public void Demand()
		{
			IPrincipal currentPrincipal = Thread.CurrentPrincipal;
			if (currentPrincipal == null)
			{
				throw new SecurityException("no Principal");
			}
			if (principals.Count <= 0)
			{
				return;
			}
			bool flag = false;
			foreach (PrincipalInfo principal in principals)
			{
				if ((principal.Name == null || principal.Name == currentPrincipal.Identity.Name) && (principal.Role == null || currentPrincipal.IsInRole(principal.Role)) && ((principal.IsAuthenticated && currentPrincipal.Identity.IsAuthenticated) || !principal.IsAuthenticated))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				throw new SecurityException("Demand for principal refused.");
			}
		}

		public void FromXml(SecurityElement elem)
		{
			CheckSecurityElement(elem, "elem", 1, 1);
			principals.Clear();
			if (elem.Children == null)
			{
				return;
			}
			foreach (SecurityElement child in elem.Children)
			{
				if (child.Tag != "Identity")
				{
					throw new ArgumentException("not IPermission/Identity");
				}
				string name = child.Attribute("ID");
				string role = child.Attribute("Role");
				string text = child.Attribute("Authenticated");
				bool isAuthenticated = false;
				if (text != null)
				{
					try
					{
						isAuthenticated = bool.Parse(text);
					}
					catch
					{
					}
				}
				PrincipalInfo value = new PrincipalInfo(name, role, isAuthenticated);
				principals.Add(value);
			}
		}

		public IPermission Intersect(IPermission target)
		{
			PrincipalPermission principalPermission = Cast(target);
			if (principalPermission == null)
			{
				return null;
			}
			if (IsUnrestricted())
			{
				return principalPermission.Copy();
			}
			if (principalPermission.IsUnrestricted())
			{
				return Copy();
			}
			PrincipalPermission principalPermission2 = new PrincipalPermission(PermissionState.None);
			foreach (PrincipalInfo principal in principals)
			{
				foreach (PrincipalInfo principal2 in principalPermission.principals)
				{
					if (principal.IsAuthenticated == principal2.IsAuthenticated)
					{
						string text = null;
						if (principal.Name == principal2.Name || principal2.Name == null)
						{
							text = principal.Name;
						}
						else if (principal.Name == null)
						{
							text = principal2.Name;
						}
						string text2 = null;
						if (principal.Role == principal2.Role || principal2.Role == null)
						{
							text2 = principal.Role;
						}
						else if (principal.Role == null)
						{
							text2 = principal2.Role;
						}
						if (text != null || text2 != null)
						{
							PrincipalInfo value = new PrincipalInfo(text, text2, principal.IsAuthenticated);
							principalPermission2.principals.Add(value);
						}
					}
				}
			}
			return (principalPermission2.principals.Count <= 0) ? null : principalPermission2;
		}

		public bool IsSubsetOf(IPermission target)
		{
			PrincipalPermission principalPermission = Cast(target);
			if (principalPermission == null)
			{
				return IsEmpty();
			}
			if (IsUnrestricted())
			{
				return principalPermission.IsUnrestricted();
			}
			if (principalPermission.IsUnrestricted())
			{
				return true;
			}
			foreach (PrincipalInfo principal in principals)
			{
				bool flag = false;
				foreach (PrincipalInfo principal2 in principalPermission.principals)
				{
					if ((principal.Name == principal2.Name || principal2.Name == null) && (principal.Role == principal2.Role || principal2.Role == null) && principal.IsAuthenticated == principal2.IsAuthenticated)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsUnrestricted()
		{
			foreach (PrincipalInfo principal in principals)
			{
				if (principal.Name == null && principal.Role == null && principal.IsAuthenticated)
				{
					return true;
				}
			}
			return false;
		}

		public override string ToString()
		{
			return ToXml().ToString();
		}

		public SecurityElement ToXml()
		{
			SecurityElement securityElement = new SecurityElement("Permission");
			Type type = GetType();
			securityElement.AddAttribute("class", type.FullName + ", " + type.Assembly.ToString().Replace('"', '\''));
			securityElement.AddAttribute("version", 1.ToString());
			foreach (PrincipalInfo principal in principals)
			{
				SecurityElement securityElement2 = new SecurityElement("Identity");
				if (principal.Name != null)
				{
					securityElement2.AddAttribute("ID", principal.Name);
				}
				if (principal.Role != null)
				{
					securityElement2.AddAttribute("Role", principal.Role);
				}
				if (principal.IsAuthenticated)
				{
					securityElement2.AddAttribute("Authenticated", "true");
				}
				securityElement.AddChild(securityElement2);
			}
			return securityElement;
		}

		public IPermission Union(IPermission other)
		{
			PrincipalPermission principalPermission = Cast(other);
			if (principalPermission == null)
			{
				return Copy();
			}
			if (IsUnrestricted() || principalPermission.IsUnrestricted())
			{
				return new PrincipalPermission(PermissionState.Unrestricted);
			}
			PrincipalPermission principalPermission2 = new PrincipalPermission(principals);
			foreach (PrincipalInfo principal in principalPermission.principals)
			{
				principalPermission2.principals.Add(principal);
			}
			return principalPermission2;
		}

		[ComVisible(false)]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			PrincipalPermission principalPermission = obj as PrincipalPermission;
			if (principalPermission == null)
			{
				return false;
			}
			if (principals.Count != principalPermission.principals.Count)
			{
				return false;
			}
			foreach (PrincipalInfo principal in principals)
			{
				bool flag = false;
				foreach (PrincipalInfo principal2 in principalPermission.principals)
				{
					if ((principal.Name == principal2.Name || principal2.Name == null) && (principal.Role == principal2.Role || principal2.Role == null) && principal.IsAuthenticated == principal2.IsAuthenticated)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		private PrincipalPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			PrincipalPermission principalPermission = target as PrincipalPermission;
			if (principalPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(PrincipalPermission));
			}
			return principalPermission;
		}

		private bool IsEmpty()
		{
			return principals.Count == 0;
		}

		internal int CheckSecurityElement(SecurityElement se, string parameterName, int minimumVersion, int maximumVersion)
		{
			if (se == null)
			{
				throw new ArgumentNullException(parameterName);
			}
			if (se.Tag != "Permission")
			{
				string message = string.Format(Locale.GetText("Invalid tag {0}"), se.Tag);
				throw new ArgumentException(message, parameterName);
			}
			int num = minimumVersion;
			string text = se.Attribute("version");
			if (text != null)
			{
				try
				{
					num = int.Parse(text);
				}
				catch (Exception innerException)
				{
					string text2 = Locale.GetText("Couldn't parse version from '{0}'.");
					text2 = string.Format(text2, text);
					throw new ArgumentException(text2, parameterName, innerException);
				}
			}
			if (num < minimumVersion || num > maximumVersion)
			{
				string text3 = Locale.GetText("Unknown version '{0}', expected versions between ['{1}','{2}'].");
				text3 = string.Format(text3, num, minimumVersion, maximumVersion);
				throw new ArgumentException(text3, parameterName);
			}
			return num;
		}
	}
}
