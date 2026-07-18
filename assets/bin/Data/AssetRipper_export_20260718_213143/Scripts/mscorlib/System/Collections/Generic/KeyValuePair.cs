using System.Diagnostics;

namespace System.Collections.Generic
{
	[Serializable]
	[DebuggerDisplay("{value}", Name = "[{key}]")]
	public struct KeyValuePair<TKey, TValue>
	{
		private TKey key;

		private TValue value;

		public TKey Key
		{
			get
			{
				return key;
			}
			private set
			{
				key = value;
			}
		}

		public TValue Value
		{
			get
			{
				return value;
			}
			private set
			{
				this.value = value;
			}
		}

		public KeyValuePair(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			return "[" + ((Key == null) ? string.Empty : Key.ToString()) + ", " + ((Value == null) ? string.Empty : Value.ToString()) + "]";
		}
	}
}
