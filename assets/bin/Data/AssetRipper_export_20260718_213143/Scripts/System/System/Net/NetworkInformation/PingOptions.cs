namespace System.Net.NetworkInformation
{
	public class PingOptions
	{
		private int ttl = 128;

		private bool dont_fragment;

		public bool DontFragment
		{
			get
			{
				return dont_fragment;
			}
			set
			{
				dont_fragment = value;
			}
		}

		public int Ttl
		{
			get
			{
				return ttl;
			}
			set
			{
				ttl = value;
			}
		}

		public PingOptions()
		{
		}

		public PingOptions(int ttl, bool dontFragment)
		{
			if (ttl <= 0)
			{
				throw new ArgumentOutOfRangeException("Must be greater than zero.", "ttl");
			}
			this.ttl = ttl;
			dont_fragment = dontFragment;
		}
	}
}
