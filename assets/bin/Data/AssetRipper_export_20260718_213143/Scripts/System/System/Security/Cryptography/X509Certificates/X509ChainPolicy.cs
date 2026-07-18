namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509ChainPolicy
	{
		private OidCollection apps;

		private OidCollection cert;

		private X509Certificate2Collection store;

		private X509RevocationFlag rflag;

		private X509RevocationMode mode;

		private TimeSpan timeout;

		private X509VerificationFlags vflags;

		private DateTime vtime;

		public OidCollection ApplicationPolicy
		{
			get
			{
				return apps;
			}
		}

		public OidCollection CertificatePolicy
		{
			get
			{
				return cert;
			}
		}

		public X509Certificate2Collection ExtraStore
		{
			get
			{
				return store;
			}
		}

		public X509RevocationFlag RevocationFlag
		{
			get
			{
				return rflag;
			}
			set
			{
				if (value < X509RevocationFlag.EndCertificateOnly || value > X509RevocationFlag.ExcludeRoot)
				{
					throw new ArgumentException("RevocationFlag");
				}
				rflag = value;
			}
		}

		public X509RevocationMode RevocationMode
		{
			get
			{
				return mode;
			}
			set
			{
				if (value < X509RevocationMode.NoCheck || value > X509RevocationMode.Offline)
				{
					throw new ArgumentException("RevocationMode");
				}
				mode = value;
			}
		}

		public TimeSpan UrlRetrievalTimeout
		{
			get
			{
				return timeout;
			}
			set
			{
				timeout = value;
			}
		}

		public X509VerificationFlags VerificationFlags
		{
			get
			{
				return vflags;
			}
			set
			{
				if ((value | X509VerificationFlags.AllFlags) != X509VerificationFlags.AllFlags)
				{
					throw new ArgumentException("VerificationFlags");
				}
				vflags = value;
			}
		}

		public DateTime VerificationTime
		{
			get
			{
				return vtime;
			}
			set
			{
				vtime = value;
			}
		}

		public X509ChainPolicy()
		{
			Reset();
		}

		public void Reset()
		{
			apps = new OidCollection();
			cert = new OidCollection();
			store = new X509Certificate2Collection();
			rflag = X509RevocationFlag.ExcludeRoot;
			mode = X509RevocationMode.Online;
			timeout = TimeSpan.Zero;
			vflags = X509VerificationFlags.NoFlag;
			vtime = DateTime.Now;
		}
	}
}
