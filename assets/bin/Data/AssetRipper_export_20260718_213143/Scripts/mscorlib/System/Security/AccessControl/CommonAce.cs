using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CommonAce : QualifiedAce
	{
		[MonoTODO]
		public override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public CommonAce(AceFlags flags, AceQualifier qualifier, int accessMask, SecurityIdentifier sid, bool isCallback, byte[] opaque)
			: base(InheritanceFlags.None, PropagationFlags.None, qualifier, isCallback, opaque)
		{
			base.AccessMask = accessMask;
			base.SecurityIdentifier = sid;
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
