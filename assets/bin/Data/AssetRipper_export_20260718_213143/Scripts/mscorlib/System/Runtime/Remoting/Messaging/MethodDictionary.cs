using System.Collections;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	internal class MethodDictionary : IEnumerable, ICollection, IDictionary
	{
		private class DictionaryEnumerator : IEnumerator, IDictionaryEnumerator
		{
			private MethodDictionary _methodDictionary;

			private IDictionaryEnumerator _hashtableEnum;

			private int _posMethod;

			public object Current
			{
				get
				{
					return Entry.Value;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					if (_posMethod >= 0)
					{
						return new DictionaryEntry(_methodDictionary._methodKeys[_posMethod], _methodDictionary.GetMethodProperty(_methodDictionary._methodKeys[_posMethod]));
					}
					if (_posMethod == -1 || _hashtableEnum == null)
					{
						throw new InvalidOperationException("The enumerator is positioned before the first element of the collection or after the last element");
					}
					return _hashtableEnum.Entry;
				}
			}

			public object Key
			{
				get
				{
					return Entry.Key;
				}
			}

			public object Value
			{
				get
				{
					return Entry.Value;
				}
			}

			public DictionaryEnumerator(MethodDictionary methodDictionary)
			{
				_methodDictionary = methodDictionary;
				object hashtableEnum;
				if (_methodDictionary._internalProperties != null)
				{
					IDictionaryEnumerator enumerator = _methodDictionary._internalProperties.GetEnumerator();
					hashtableEnum = enumerator;
				}
				else
				{
					hashtableEnum = null;
				}
				_hashtableEnum = (IDictionaryEnumerator)hashtableEnum;
				_posMethod = -1;
			}

			public bool MoveNext()
			{
				if (_posMethod != -2)
				{
					_posMethod++;
					if (_posMethod < _methodDictionary._methodKeys.Length)
					{
						return true;
					}
					_posMethod = -2;
				}
				if (_hashtableEnum == null)
				{
					return false;
				}
				while (_hashtableEnum.MoveNext())
				{
					if (!_methodDictionary.IsOverridenKey((string)_hashtableEnum.Key))
					{
						return true;
					}
				}
				return false;
			}

			public void Reset()
			{
				_posMethod = -1;
				_hashtableEnum.Reset();
			}
		}

		private IDictionary _internalProperties;

		protected IMethodMessage _message;

		private string[] _methodKeys;

		private bool _ownProperties;

		internal bool HasInternalProperties
		{
			get
			{
				if (_internalProperties != null)
				{
					if (_internalProperties is MethodDictionary)
					{
						return ((MethodDictionary)_internalProperties).HasInternalProperties;
					}
					return _internalProperties.Count > 0;
				}
				return false;
			}
		}

		internal IDictionary InternalProperties
		{
			get
			{
				if (_internalProperties != null && _internalProperties is MethodDictionary)
				{
					return ((MethodDictionary)_internalProperties).InternalProperties;
				}
				return _internalProperties;
			}
		}

		public string[] MethodKeys
		{
			get
			{
				return _methodKeys;
			}
			set
			{
				_methodKeys = value;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public object this[object key]
		{
			get
			{
				string text = (string)key;
				for (int i = 0; i < _methodKeys.Length; i++)
				{
					if (_methodKeys[i] == text)
					{
						return GetMethodProperty(text);
					}
				}
				if (_internalProperties != null)
				{
					return _internalProperties[key];
				}
				return null;
			}
			set
			{
				Add(key, value);
			}
		}

		public ICollection Keys
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < _methodKeys.Length; i++)
				{
					arrayList.Add(_methodKeys[i]);
				}
				if (_internalProperties != null)
				{
					foreach (string key in _internalProperties.Keys)
					{
						if (!IsOverridenKey(key))
						{
							arrayList.Add(key);
						}
					}
				}
				return arrayList;
			}
		}

		public ICollection Values
		{
			get
			{
				ArrayList arrayList = new ArrayList();
				for (int i = 0; i < _methodKeys.Length; i++)
				{
					arrayList.Add(GetMethodProperty(_methodKeys[i]));
				}
				if (_internalProperties != null)
				{
					foreach (DictionaryEntry internalProperty in _internalProperties)
					{
						if (!IsOverridenKey((string)internalProperty.Key))
						{
							arrayList.Add(internalProperty.Value);
						}
					}
				}
				return arrayList;
			}
		}

		public int Count
		{
			get
			{
				if (_internalProperties != null)
				{
					return _internalProperties.Count + _methodKeys.Length;
				}
				return _methodKeys.Length;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public MethodDictionary(IMethodMessage message)
		{
			_message = message;
		}

		public MethodDictionary(string[] keys)
		{
			_methodKeys = keys;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new DictionaryEnumerator(this);
		}

		protected virtual IDictionary AllocInternalProperties()
		{
			_ownProperties = true;
			return new Hashtable();
		}

		public IDictionary GetInternalProperties()
		{
			if (_internalProperties == null)
			{
				_internalProperties = AllocInternalProperties();
			}
			return _internalProperties;
		}

		private bool IsOverridenKey(string key)
		{
			if (_ownProperties)
			{
				return false;
			}
			string[] methodKeys = _methodKeys;
			foreach (string text in methodKeys)
			{
				if (key == text)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual object GetMethodProperty(string key)
		{
			switch (key)
			{
			case "__Uri":
				return _message.Uri;
			case "__MethodName":
				return _message.MethodName;
			case "__TypeName":
				return _message.TypeName;
			case "__MethodSignature":
				return _message.MethodSignature;
			case "__CallContext":
				return _message.LogicalCallContext;
			case "__Args":
				return _message.Args;
			case "__OutArgs":
				return ((IMethodReturnMessage)_message).OutArgs;
			case "__Return":
				return ((IMethodReturnMessage)_message).ReturnValue;
			default:
				return null;
			}
		}

		protected virtual void SetMethodProperty(string key, object value)
		{
			switch (key)
			{
			case "__CallContext":
			case "__OutArgs":
			case "__Return":
				break;
			case "__MethodName":
			case "__TypeName":
			case "__MethodSignature":
			case "__Args":
				throw new ArgumentException("key was invalid");
			case "__Uri":
				((IInternalMessage)_message).Uri = (string)value;
				break;
			}
		}

		public void Add(object key, object value)
		{
			string text = (string)key;
			for (int i = 0; i < _methodKeys.Length; i++)
			{
				if (_methodKeys[i] == text)
				{
					SetMethodProperty(text, value);
					return;
				}
			}
			if (_internalProperties == null)
			{
				_internalProperties = AllocInternalProperties();
			}
			_internalProperties[key] = value;
		}

		public void Clear()
		{
			if (_internalProperties != null)
			{
				_internalProperties.Clear();
			}
		}

		public bool Contains(object key)
		{
			string text = (string)key;
			for (int i = 0; i < _methodKeys.Length; i++)
			{
				if (_methodKeys[i] == text)
				{
					return true;
				}
			}
			if (_internalProperties != null)
			{
				return _internalProperties.Contains(key);
			}
			return false;
		}

		public void Remove(object key)
		{
			string text = (string)key;
			for (int i = 0; i < _methodKeys.Length; i++)
			{
				if (_methodKeys[i] == text)
				{
					throw new ArgumentException("key was invalid");
				}
			}
			if (_internalProperties != null)
			{
				_internalProperties.Remove(key);
			}
		}

		public void CopyTo(Array array, int index)
		{
			Values.CopyTo(array, index);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new DictionaryEnumerator(this);
		}
	}
}
