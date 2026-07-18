using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[Serializable]
	[DebuggerDisplay("{_value}", Name = "[{_key}]")]
	[ComVisible(true)]
	public struct DictionaryEntry
	{
		private object _key;

		private object _value;

		public object Key
		{
			get
			{
				return _key;
			}
			set
			{
				_key = value;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public DictionaryEntry(object key, object value)
		{
			_key = key;
			_value = value;
		}
	}
}
