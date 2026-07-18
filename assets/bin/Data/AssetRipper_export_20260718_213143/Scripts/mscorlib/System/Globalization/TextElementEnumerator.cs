using System.Collections;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public class TextElementEnumerator : IEnumerator
	{
		private int index;

		private int elementindex;

		private int startpos;

		private string str;

		private string element;

		public object Current
		{
			get
			{
				if (element == null)
				{
					throw new InvalidOperationException();
				}
				return element;
			}
		}

		public int ElementIndex
		{
			get
			{
				if (element == null)
				{
					throw new InvalidOperationException();
				}
				return elementindex + startpos;
			}
		}

		internal TextElementEnumerator(string str, int startpos)
		{
			index = -1;
			this.startpos = startpos;
			this.str = str.Substring(startpos);
			element = null;
		}

		public string GetTextElement()
		{
			if (element == null)
			{
				throw new InvalidOperationException();
			}
			return element;
		}

		public bool MoveNext()
		{
			elementindex = index + 1;
			if (elementindex < str.Length)
			{
				element = StringInfo.GetNextTextElement(str, elementindex);
				index += element.Length;
				return true;
			}
			element = null;
			return false;
		}

		public void Reset()
		{
			element = null;
			index = -1;
		}
	}
}
