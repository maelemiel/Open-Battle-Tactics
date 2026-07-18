using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class MACTripleDES : KeyedHashAlgorithm
	{
		private TripleDES tdes;

		private MACAlgorithm mac;

		private bool m_disposed;

		[ComVisible(false)]
		public PaddingMode Padding
		{
			get
			{
				return tdes.Padding;
			}
			set
			{
				tdes.Padding = value;
			}
		}

		public MACTripleDES()
		{
			Setup("TripleDES", null);
		}

		public MACTripleDES(byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new ArgumentNullException("rgbKey");
			}
			Setup("TripleDES", rgbKey);
		}

		public MACTripleDES(string strTripleDES, byte[] rgbKey)
		{
			if (rgbKey == null)
			{
				throw new ArgumentNullException("rgbKey");
			}
			if (strTripleDES == null)
			{
				Setup("TripleDES", rgbKey);
			}
			else
			{
				Setup(strTripleDES, rgbKey);
			}
		}

		private void Setup(string strTripleDES, byte[] rgbKey)
		{
			tdes = TripleDES.Create(strTripleDES);
			tdes.Padding = PaddingMode.Zeros;
			if (rgbKey != null)
			{
				tdes.Key = rgbKey;
			}
			HashSizeValue = tdes.BlockSize;
			Key = tdes.Key;
			mac = new MACAlgorithm(tdes);
			m_disposed = false;
		}

		~MACTripleDES()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				if (KeyValue != null)
				{
					Array.Clear(KeyValue, 0, KeyValue.Length);
				}
				if (tdes != null)
				{
					tdes.Clear();
				}
				if (disposing)
				{
					KeyValue = null;
					tdes = null;
				}
				base.Dispose(disposing);
				m_disposed = true;
			}
		}

		public override void Initialize()
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("MACTripleDES");
			}
			State = 0;
			mac.Initialize(KeyValue);
		}

		protected override void HashCore(byte[] rgbData, int ibStart, int cbSize)
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("MACTripleDES");
			}
			if (State == 0)
			{
				Initialize();
				State = 1;
			}
			mac.Core(rgbData, ibStart, cbSize);
		}

		protected override byte[] HashFinal()
		{
			if (m_disposed)
			{
				throw new ObjectDisposedException("MACTripleDES");
			}
			State = 0;
			return mac.Final();
		}
	}
}
