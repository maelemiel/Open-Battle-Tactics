using System.Collections;

namespace System.Runtime.Remoting.Channels
{
	internal class AggregateEnumerator : IEnumerator, IDictionaryEnumerator
	{
		private IDictionary[] dictionaries;

		private int pos;

		private IDictionaryEnumerator currente;

		public DictionaryEntry Entry
		{
			get
			{
				return currente.Entry;
			}
		}

		public object Key
		{
			get
			{
				return currente.Key;
			}
		}

		public object Value
		{
			get
			{
				return currente.Value;
			}
		}

		public object Current
		{
			get
			{
				return currente.Current;
			}
		}

		public AggregateEnumerator(IDictionary[] dics)
		{
			dictionaries = dics;
			Reset();
		}

		public bool MoveNext()
		{
			if (pos >= dictionaries.Length)
			{
				return false;
			}
			if (!currente.MoveNext())
			{
				pos++;
				if (pos >= dictionaries.Length)
				{
					return false;
				}
				currente = dictionaries[pos].GetEnumerator();
				return MoveNext();
			}
			return true;
		}

		public void Reset()
		{
			pos = 0;
			if (dictionaries.Length > 0)
			{
				currente = dictionaries[0].GetEnumerator();
			}
		}
	}
}
