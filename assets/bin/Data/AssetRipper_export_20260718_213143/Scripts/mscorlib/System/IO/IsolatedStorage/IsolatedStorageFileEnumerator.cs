using System.Collections;

namespace System.IO.IsolatedStorage
{
	internal class IsolatedStorageFileEnumerator : IEnumerator
	{
		private IsolatedStorageScope _scope;

		private string[] _storages;

		private int _pos;

		public object Current
		{
			get
			{
				if (_pos < 0 || _storages == null || _pos >= _storages.Length)
				{
					return null;
				}
				return new IsolatedStorageFile(_scope, _storages[_pos]);
			}
		}

		public IsolatedStorageFileEnumerator(IsolatedStorageScope scope, string root)
		{
			_scope = scope;
			if (Directory.Exists(root))
			{
				_storages = Directory.GetDirectories(root, "d.*");
			}
			_pos = -1;
		}

		public bool MoveNext()
		{
			if (_storages == null)
			{
				return false;
			}
			return ++_pos < _storages.Length;
		}

		public void Reset()
		{
			_pos = -1;
		}
	}
}
