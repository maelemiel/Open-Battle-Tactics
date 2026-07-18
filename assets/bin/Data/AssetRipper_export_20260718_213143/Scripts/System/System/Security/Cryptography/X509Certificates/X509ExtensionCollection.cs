using System.Collections;
using Mono.Security;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509ExtensionCollection : ICollection, IEnumerable
	{
		private ArrayList _list;

		public int Count
		{
			get
			{
				return _list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return _list.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public X509Extension this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new InvalidOperationException("index");
				}
				return (X509Extension)_list[index];
			}
		}

		public X509Extension this[string oid]
		{
			get
			{
				if (oid == null)
				{
					throw new ArgumentNullException("oid");
				}
				if (_list.Count == 0 || oid.Length == 0)
				{
					return null;
				}
				foreach (X509Extension item in _list)
				{
					if (item.Oid.Value.Equals(oid))
					{
						return item;
					}
				}
				return null;
			}
		}

		public X509ExtensionCollection()
		{
			_list = new ArrayList();
		}

		internal X509ExtensionCollection(Mono.Security.X509.X509Certificate cert)
		{
			_list = new ArrayList(cert.Extensions.Count);
			if (cert.Extensions.Count == 0)
			{
				return;
			}
			object[] array = new object[2];
			foreach (Mono.Security.X509.X509Extension extension in cert.Extensions)
			{
				bool critical = extension.Critical;
				string oid = extension.Oid;
				byte[] rawData = null;
				ASN1 value = extension.Value;
				if (value.Tag == 4 && value.Count > 0)
				{
					rawData = value[0].GetBytes();
				}
				array[0] = new AsnEncodedData(oid, rawData);
				array[1] = critical;
				X509Extension x509Extension2 = (X509Extension)CryptoConfig.CreateFromName(oid, array);
				if (x509Extension2 == null)
				{
					x509Extension2 = new X509Extension(oid, rawData, critical);
				}
				_list.Add(x509Extension2);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("negative index");
			}
			if (index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index >= array.Length");
			}
			_list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new X509ExtensionEnumerator(_list);
		}

		public int Add(X509Extension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			return _list.Add(extension);
		}

		public void CopyTo(X509Extension[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("negative index");
			}
			if (index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index >= array.Length");
			}
			_list.CopyTo(array, index);
		}

		public X509ExtensionEnumerator GetEnumerator()
		{
			return new X509ExtensionEnumerator(_list);
		}
	}
}
