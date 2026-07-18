using System;
using System.Collections;
using System.Globalization;

namespace Mono.Security.Protocol.Tls
{
	internal sealed class CipherSuiteCollection : IEnumerable, ICollection, IList
	{
		private ArrayList cipherSuites;

		private SecurityProtocolType protocol;

		object IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (CipherSuite)value;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return cipherSuites.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return cipherSuites.SyncRoot;
			}
		}

		public CipherSuite this[string name]
		{
			get
			{
				return (CipherSuite)cipherSuites[IndexOf(name)];
			}
			set
			{
				cipherSuites[IndexOf(name)] = value;
			}
		}

		public CipherSuite this[int index]
		{
			get
			{
				return (CipherSuite)cipherSuites[index];
			}
			set
			{
				cipherSuites[index] = value;
			}
		}

		public CipherSuite this[short code]
		{
			get
			{
				return (CipherSuite)cipherSuites[IndexOf(code)];
			}
			set
			{
				cipherSuites[IndexOf(code)] = value;
			}
		}

		public int Count
		{
			get
			{
				return cipherSuites.Count;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return cipherSuites.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return cipherSuites.IsReadOnly;
			}
		}

		public CipherSuiteCollection(SecurityProtocolType protocol)
		{
			this.protocol = protocol;
			cipherSuites = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return cipherSuites.GetEnumerator();
		}

		bool IList.Contains(object value)
		{
			return cipherSuites.Contains(value as CipherSuite);
		}

		int IList.IndexOf(object value)
		{
			return cipherSuites.IndexOf(value as CipherSuite);
		}

		void IList.Insert(int index, object value)
		{
			cipherSuites.Insert(index, value as CipherSuite);
		}

		void IList.Remove(object value)
		{
			cipherSuites.Remove(value as CipherSuite);
		}

		void IList.RemoveAt(int index)
		{
			cipherSuites.RemoveAt(index);
		}

		int IList.Add(object value)
		{
			return cipherSuites.Add(value as CipherSuite);
		}

		public void CopyTo(Array array, int index)
		{
			cipherSuites.CopyTo(array, index);
		}

		public void Clear()
		{
			cipherSuites.Clear();
		}

		public int IndexOf(string name)
		{
			int num = 0;
			foreach (CipherSuite cipherSuite in cipherSuites)
			{
				if (cultureAwareCompare(cipherSuite.Name, name))
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public int IndexOf(short code)
		{
			int num = 0;
			foreach (CipherSuite cipherSuite in cipherSuites)
			{
				if (cipherSuite.Code == code)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public CipherSuite Add(short code, string name, CipherAlgorithmType cipherType, HashAlgorithmType hashType, ExchangeAlgorithmType exchangeType, bool exportable, bool blockMode, byte keyMaterialSize, byte expandedKeyMaterialSize, short effectiveKeyBytes, byte ivSize, byte blockSize)
		{
			switch (protocol)
			{
			case SecurityProtocolType.Default:
			case SecurityProtocolType.Tls:
				return add(new TlsCipherSuite(code, name, cipherType, hashType, exchangeType, exportable, blockMode, keyMaterialSize, expandedKeyMaterialSize, effectiveKeyBytes, ivSize, blockSize));
			case SecurityProtocolType.Ssl3:
				return add(new SslCipherSuite(code, name, cipherType, hashType, exchangeType, exportable, blockMode, keyMaterialSize, expandedKeyMaterialSize, effectiveKeyBytes, ivSize, blockSize));
			default:
				throw new NotSupportedException("Unsupported security protocol type.");
			}
		}

		private TlsCipherSuite add(TlsCipherSuite cipherSuite)
		{
			cipherSuites.Add(cipherSuite);
			return cipherSuite;
		}

		private SslCipherSuite add(SslCipherSuite cipherSuite)
		{
			cipherSuites.Add(cipherSuite);
			return cipherSuite;
		}

		private bool cultureAwareCompare(string strA, string strB)
		{
			return CultureInfo.CurrentCulture.CompareInfo.Compare(strA, strB, CompareOptions.IgnoreCase | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreWidth) == 0;
		}
	}
}
