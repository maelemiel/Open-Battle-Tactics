using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class ObjectAccessRule : AccessRule
	{
		private Guid object_type;

		private Guid inherited_object_type;

		public Guid InheritedObjectType
		{
			get
			{
				return inherited_object_type;
			}
		}

		public ObjectAceFlags ObjectFlags
		{
			get
			{
				ObjectAceFlags objectAceFlags = ObjectAceFlags.None;
				if (object_type != Guid.Empty)
				{
					objectAceFlags |= ObjectAceFlags.ObjectAceTypePresent;
				}
				if (inherited_object_type != Guid.Empty)
				{
					objectAceFlags |= ObjectAceFlags.InheritedObjectAceTypePresent;
				}
				return objectAceFlags;
			}
		}

		public Guid ObjectType
		{
			get
			{
				return object_type;
			}
		}

		protected ObjectAccessRule(IdentityReference identity, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, Guid objectType, Guid inheritedObjectType, AccessControlType type)
			: base(identity, accessMask, isInherited, inheritanceFlags, propagationFlags, type)
		{
			object_type = objectType;
			inherited_object_type = inheritedObjectType;
		}
	}
}
