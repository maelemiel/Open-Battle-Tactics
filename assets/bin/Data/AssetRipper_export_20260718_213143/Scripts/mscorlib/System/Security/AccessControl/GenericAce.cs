namespace System.Security.AccessControl
{
	public abstract class GenericAce
	{
		private InheritanceFlags inheritance;

		private PropagationFlags propagation;

		private AceFlags aceflags;

		private AceType ace_type;

		public AceFlags AceFlags
		{
			get
			{
				return aceflags;
			}
			set
			{
				aceflags = value;
			}
		}

		public AceType AceType
		{
			get
			{
				return ace_type;
			}
		}

		public AuditFlags AuditFlags
		{
			get
			{
				AuditFlags auditFlags = AuditFlags.None;
				if ((aceflags & AceFlags.SuccessfulAccess) != AceFlags.None)
				{
					auditFlags |= AuditFlags.Success;
				}
				if ((aceflags & AceFlags.FailedAccess) != AceFlags.None)
				{
					auditFlags |= AuditFlags.Failure;
				}
				return auditFlags;
			}
		}

		public abstract int BinaryLength { get; }

		public InheritanceFlags InheritanceFlags
		{
			get
			{
				return inheritance;
			}
		}

		[MonoTODO]
		public bool IsInherited
		{
			get
			{
				return false;
			}
		}

		public PropagationFlags PropagationFlags
		{
			get
			{
				return propagation;
			}
		}

		internal GenericAce(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags)
		{
			inheritance = inheritanceFlags;
			propagation = propagationFlags;
		}

		internal GenericAce(AceType type)
		{
			if (type <= AceType.SystemAlarmCallbackObject)
			{
				throw new ArgumentOutOfRangeException("type");
			}
			ace_type = type;
		}

		[MonoTODO]
		public GenericAce Copy()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static GenericAce CreateFromBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public sealed override bool Equals(object o)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public abstract void GetBinaryForm(byte[] binaryForm, int offset);

		[MonoTODO]
		public sealed override int GetHashCode()
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static bool operator ==(GenericAce left, GenericAce right)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static bool operator !=(GenericAce left, GenericAce right)
		{
			throw new NotImplementedException();
		}
	}
}
