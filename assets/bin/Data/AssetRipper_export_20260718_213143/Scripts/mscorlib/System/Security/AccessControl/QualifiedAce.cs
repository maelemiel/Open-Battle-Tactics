namespace System.Security.AccessControl
{
	public abstract class QualifiedAce : KnownAce
	{
		private AceQualifier ace_qualifier;

		private bool is_callback;

		private byte[] opaque;

		public AceQualifier AceQualifier
		{
			get
			{
				return ace_qualifier;
			}
		}

		public bool IsCallback
		{
			get
			{
				return is_callback;
			}
		}

		public int OpaqueLength
		{
			get
			{
				return opaque.Length;
			}
		}

		internal QualifiedAce(InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AceQualifier aceQualifier, bool isCallback, byte[] opaque)
			: base(inheritanceFlags, propagationFlags)
		{
			ace_qualifier = aceQualifier;
			is_callback = isCallback;
			SetOpaque(opaque);
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
