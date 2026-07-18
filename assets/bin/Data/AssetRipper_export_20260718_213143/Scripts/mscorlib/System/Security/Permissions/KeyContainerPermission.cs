using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class KeyContainerPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private KeyContainerPermissionAccessEntryCollection _accessEntries;

		private KeyContainerPermissionFlags _flags;

		public KeyContainerPermissionAccessEntryCollection AccessEntries
		{
			get
			{
				return _accessEntries;
			}
		}

		public KeyContainerPermissionFlags Flags
		{
			get
			{
				return _flags;
			}
		}

		public KeyContainerPermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				_flags = KeyContainerPermissionFlags.AllFlags;
			}
		}

		public KeyContainerPermission(KeyContainerPermissionFlags flags)
		{
			SetFlags(flags);
		}

		public KeyContainerPermission(KeyContainerPermissionFlags flags, KeyContainerPermissionAccessEntry[] accessList)
		{
			SetFlags(flags);
			if (accessList != null)
			{
				foreach (KeyContainerPermissionAccessEntry accessEntry in accessList)
				{
					_accessEntries.Add(accessEntry);
				}
			}
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 16;
		}

		public override IPermission Copy()
		{
			if (_accessEntries.Count == 0)
			{
				return new KeyContainerPermission(_flags);
			}
			KeyContainerPermissionAccessEntry[] array = new KeyContainerPermissionAccessEntry[_accessEntries.Count];
			_accessEntries.CopyTo(array, 0);
			return new KeyContainerPermission(_flags, array);
		}

		[MonoTODO("(2.0) missing support for AccessEntries")]
		public override void FromXml(SecurityElement securityElement)
		{
			CodeAccessPermission.CheckSecurityElement(securityElement, "securityElement", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(securityElement))
			{
				_flags = KeyContainerPermissionFlags.AllFlags;
			}
			else
			{
				_flags = (KeyContainerPermissionFlags)(int)Enum.Parse(typeof(KeyContainerPermissionFlags), securityElement.Attribute("Flags"));
			}
		}

		[MonoTODO("(2.0)")]
		public override IPermission Intersect(IPermission target)
		{
			return null;
		}

		[MonoTODO("(2.0)")]
		public override bool IsSubsetOf(IPermission target)
		{
			return false;
		}

		public bool IsUnrestricted()
		{
			return _flags == KeyContainerPermissionFlags.AllFlags;
		}

		[MonoTODO("(2.0) missing support for AccessEntries")]
		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			KeyContainerPermission keyContainerPermission = Cast(target);
			if (keyContainerPermission == null)
			{
				return Copy();
			}
			KeyContainerPermissionAccessEntryCollection keyContainerPermissionAccessEntryCollection = new KeyContainerPermissionAccessEntryCollection();
			KeyContainerPermissionAccessEntryEnumerator enumerator = _accessEntries.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyContainerPermissionAccessEntry current = enumerator.Current;
				keyContainerPermissionAccessEntryCollection.Add(current);
			}
			KeyContainerPermissionAccessEntryEnumerator enumerator2 = keyContainerPermission._accessEntries.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyContainerPermissionAccessEntry current2 = enumerator2.Current;
				if (_accessEntries.IndexOf(current2) == -1)
				{
					keyContainerPermissionAccessEntryCollection.Add(current2);
				}
			}
			if (keyContainerPermissionAccessEntryCollection.Count == 0)
			{
				return new KeyContainerPermission(_flags | keyContainerPermission._flags);
			}
			KeyContainerPermissionAccessEntry[] array = new KeyContainerPermissionAccessEntry[keyContainerPermissionAccessEntryCollection.Count];
			keyContainerPermissionAccessEntryCollection.CopyTo(array, 0);
			return new KeyContainerPermission(_flags | keyContainerPermission._flags, array);
		}

		private void SetFlags(KeyContainerPermissionFlags flags)
		{
			if ((flags & KeyContainerPermissionFlags.AllFlags) != KeyContainerPermissionFlags.NoFlags)
			{
				string message = string.Format(Locale.GetText("Invalid enum {0}"), flags);
				throw new ArgumentException(message, "KeyContainerPermissionFlags");
			}
			_flags = flags;
		}

		private KeyContainerPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			KeyContainerPermission keyContainerPermission = target as KeyContainerPermission;
			if (keyContainerPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(KeyContainerPermission));
			}
			return keyContainerPermission;
		}
	}
}
