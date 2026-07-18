using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class ObjectAce : QualifiedAce
	{
		private Guid object_ace_type;

		private Guid inherited_object_type;

		private ObjectAceFlags object_ace_flags;

		[MonoTODO]
		public override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Guid InheritedObjectAceType
		{
			get
			{
				return inherited_object_type;
			}
			set
			{
				inherited_object_type = value;
			}
		}

		public ObjectAceFlags ObjectAceFlags
		{
			get
			{
				return object_ace_flags;
			}
			set
			{
				object_ace_flags = value;
			}
		}

		public Guid ObjectAceType
		{
			get
			{
				return object_ace_type;
			}
			set
			{
				object_ace_type = value;
			}
		}

		public ObjectAce(AceFlags aceFlags, AceQualifier qualifier, int accessMask, SecurityIdentifier sid, ObjectAceFlags flags, Guid type, Guid inheritedType, bool isCallback, byte[] opaque)
			: base(InheritanceFlags.None, PropagationFlags.None, qualifier, isCallback, opaque)
		{
			base.AceFlags = aceFlags;
			base.SecurityIdentifier = sid;
			object_ace_flags = flags;
			object_ace_type = type;
			inherited_object_type = inheritedType;
		}

		[MonoTODO]
		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static int MaxOpaqueLength(bool isCallback)
		{
			throw new NotImplementedException();
		}
	}
}
