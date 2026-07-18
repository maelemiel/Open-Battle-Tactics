using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class AsnEncodedDataEnumerator : IEnumerator
	{
		private AsnEncodedDataCollection _collection;

		private int _position;

		object IEnumerator.Current
		{
			get
			{
				if (_position < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return _collection[_position];
			}
		}

		public AsnEncodedData Current
		{
			get
			{
				if (_position < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				return _collection[_position];
			}
		}

		internal AsnEncodedDataEnumerator(AsnEncodedDataCollection collection)
		{
			_collection = collection;
			_position = -1;
		}

		public bool MoveNext()
		{
			if (++_position < _collection.Count)
			{
				return true;
			}
			_position = _collection.Count - 1;
			return false;
		}

		public void Reset()
		{
			_position = -1;
		}
	}
}
