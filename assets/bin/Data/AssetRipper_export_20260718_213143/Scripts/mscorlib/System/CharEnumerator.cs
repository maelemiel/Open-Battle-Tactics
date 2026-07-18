using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public sealed class CharEnumerator : IEnumerator, IDisposable, ICloneable, IEnumerator<char>
	{
		private string str;

		private int index;

		private int length;

		object IEnumerator.Current
		{
			get
			{
				return Current;
			}
		}

		public char Current
		{
			get
			{
				if (index == -1 || index >= length)
				{
					throw new InvalidOperationException(Locale.GetText("The position is not valid."));
				}
				return str[index];
			}
		}

		internal CharEnumerator(string s)
		{
			str = s;
			index = -1;
			length = s.Length;
		}

		void IDisposable.Dispose()
		{
		}

		public object Clone()
		{
			CharEnumerator charEnumerator = new CharEnumerator(str);
			charEnumerator.index = index;
			return charEnumerator;
		}

		public bool MoveNext()
		{
			index++;
			if (index >= length)
			{
				index = length;
				return false;
			}
			return true;
		}

		public void Reset()
		{
			index = -1;
		}
	}
}
