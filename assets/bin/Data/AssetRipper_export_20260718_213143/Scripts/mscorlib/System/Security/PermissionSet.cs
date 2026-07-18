using System.Collections;
using System.Security.Permissions;
using System.Security.Policy;

namespace System.Security
{
	[Serializable]
	public class PermissionSet
	{
		internal PolicyLevel Resolver { get; set; }

		internal bool DeclarativeSecurity { get; set; }

		public PermissionSet()
		{
		}

		internal PermissionSet(string xml)
		{
		}

		public PermissionSet(PermissionState state)
		{
		}

		public PermissionSet(PermissionSet permSet)
		{
		}

		public IPermission AddPermission(IPermission perm)
		{
			return perm;
		}

		public virtual void Assert()
		{
		}

		public virtual PermissionSet Copy()
		{
			return new PermissionSet(this);
		}

		public virtual void Demand()
		{
		}

		public virtual void PermitOnly()
		{
		}

		public virtual IPermission GetPermission(Type permClass)
		{
			return null;
		}

		public virtual PermissionSet Intersect(PermissionSet other)
		{
			return other;
		}

		public virtual void Deny()
		{
		}

		public virtual void FromXml(SecurityElement et)
		{
		}

		public virtual void CopyTo(Array array, int index)
		{
		}

		public virtual SecurityElement ToXml()
		{
			return null;
		}

		public virtual bool IsSubsetOf(PermissionSet target)
		{
			return true;
		}

		internal void SetReadOnly(bool value)
		{
		}

		public bool IsUnrestricted()
		{
			return true;
		}

		public PermissionSet Union(PermissionSet other)
		{
			return new PermissionSet();
		}

		public virtual IEnumerator GetEnumerator()
		{
			yield break;
		}

		public virtual bool IsEmpty()
		{
			return true;
		}

		internal static PermissionSet CreateFromBinaryFormat(byte[] data)
		{
			return new PermissionSet();
		}
	}
}
