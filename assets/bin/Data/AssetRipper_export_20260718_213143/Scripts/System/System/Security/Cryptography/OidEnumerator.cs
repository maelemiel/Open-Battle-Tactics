using System.Collections;

namespace System.Security.Cryptography
{
	public sealed class OidEnumerator : IEnumerator
	{
		private OidCollection _collection;

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

		public Oid Current
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

		internal OidEnumerator(OidCollection collection)
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
