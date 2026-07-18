namespace System.Security.Permissions
{
	[Serializable]
	internal sealed class HostProtectionPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private HostProtectionResource _resources;

		public HostProtectionResource Resources
		{
			get
			{
				return _resources;
			}
			set
			{
				if (!Enum.IsDefined(typeof(HostProtectionResource), value))
				{
					string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
					throw new ArgumentException(message, "HostProtectionResource");
				}
				_resources = value;
			}
		}

		public HostProtectionPermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				_resources = HostProtectionResource.All;
			}
			else
			{
				_resources = HostProtectionResource.None;
			}
		}

		public HostProtectionPermission(HostProtectionResource resources)
		{
			Resources = _resources;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 9;
		}

		public override IPermission Copy()
		{
			return new HostProtectionPermission(_resources);
		}

		public override IPermission Intersect(IPermission target)
		{
			HostProtectionPermission hostProtectionPermission = Cast(target);
			if (hostProtectionPermission == null)
			{
				return null;
			}
			if (IsUnrestricted() && hostProtectionPermission.IsUnrestricted())
			{
				return new HostProtectionPermission(PermissionState.Unrestricted);
			}
			if (IsUnrestricted())
			{
				return hostProtectionPermission.Copy();
			}
			if (hostProtectionPermission.IsUnrestricted())
			{
				return Copy();
			}
			return new HostProtectionPermission(_resources & hostProtectionPermission._resources);
		}

		public override IPermission Union(IPermission target)
		{
			HostProtectionPermission hostProtectionPermission = Cast(target);
			if (hostProtectionPermission == null)
			{
				return Copy();
			}
			if (IsUnrestricted() || hostProtectionPermission.IsUnrestricted())
			{
				return new HostProtectionPermission(PermissionState.Unrestricted);
			}
			return new HostProtectionPermission(_resources | hostProtectionPermission._resources);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			HostProtectionPermission hostProtectionPermission = Cast(target);
			if (hostProtectionPermission == null)
			{
				return _resources == HostProtectionResource.None;
			}
			if (hostProtectionPermission.IsUnrestricted())
			{
				return true;
			}
			if (IsUnrestricted())
			{
				return false;
			}
			return (_resources & ~hostProtectionPermission._resources) == 0;
		}

		public override void FromXml(SecurityElement e)
		{
			CodeAccessPermission.CheckSecurityElement(e, "e", 1, 1);
			_resources = (HostProtectionResource)(int)Enum.Parse(typeof(HostProtectionResource), e.Attribute("Resources"));
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			securityElement.AddAttribute("Resources", _resources.ToString());
			return securityElement;
		}

		public bool IsUnrestricted()
		{
			return _resources == HostProtectionResource.All;
		}

		private HostProtectionPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			HostProtectionPermission hostProtectionPermission = target as HostProtectionPermission;
			if (hostProtectionPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(HostProtectionPermission));
			}
			return hostProtectionPermission;
		}
	}
}
