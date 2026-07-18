namespace System.Security.AccessControl
{
	public sealed class CustomAce : GenericAce
	{
		private byte[] opaque;

		[MonoTODO]
		public static readonly int MaxOpaqueLength;

		[MonoTODO]
		public override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int OpaqueLength
		{
			get
			{
				return opaque.Length;
			}
		}

		public CustomAce(AceType type, AceFlags flags, byte[] opaque)
			: base(type)
		{
			base.AceFlags = flags;
			SetOpaque(opaque);
		}

		[MonoTODO]
		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		public byte[] GetOpaque()
		{
			return (byte[])opaque.Clone();
		}

		public void SetOpaque(byte[] opaque)
		{
			if (opaque == null)
			{
				throw new ArgumentNullException("opaque");
			}
			this.opaque = (byte[])opaque.Clone();
		}
	}
}
