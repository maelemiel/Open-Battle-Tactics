using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class AsnEncodedDataCollection : ICollection, IEnumerable
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

		public AsnEncodedData this[int index]
		{
			get
			{
				return (AsnEncodedData)_list[index];
			}
		}

		public object SyncRoot
		{
			get
			{
				return _list.SyncRoot;
			}
		}

		public AsnEncodedDataCollection()
		{
			_list = new ArrayList();
		}

		public AsnEncodedDataCollection(AsnEncodedData asnEncodedData)
		{
			_list = new ArrayList();
			_list.Add(asnEncodedData);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			_list.CopyTo(array, index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new AsnEncodedDataEnumerator(this);
		}

		public int Add(AsnEncodedData asnEncodedData)
		{
			return _list.Add(asnEncodedData);
		}

		public void CopyTo(AsnEncodedData[] array, int index)
		{
			_list.CopyTo(array, index);
		}

		public AsnEncodedDataEnumerator GetEnumerator()
		{
			return new AsnEncodedDataEnumerator(this);
		}

		public void Remove(AsnEncodedData asnEncodedData)
		{
			_list.Remove(asnEncodedData);
		}
	}
}
