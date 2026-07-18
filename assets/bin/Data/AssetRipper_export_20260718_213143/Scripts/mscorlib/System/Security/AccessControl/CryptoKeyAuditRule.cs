using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CryptoKeyAuditRule : AuditRule
	{
		private CryptoKeyRights rights;

		public CryptoKeyRights CryptoKeyRights
		{
			get
			{
				return rights;
			}
		}

		public CryptoKeyAuditRule(IdentityReference identity, CryptoKeyRights cryptoKeyRights, AuditFlags flags)
			: base(identity, 0, false, InheritanceFlags.None, PropagationFlags.None, flags)
		{
			rights = cryptoKeyRights;
		}

		public CryptoKeyAuditRule(string identity, CryptoKeyRights cryptoKeyRights, AuditFlags flags)
			: this(new SecurityIdentifier(identity), cryptoKeyRights, flags)
		{
		}
	}
}
