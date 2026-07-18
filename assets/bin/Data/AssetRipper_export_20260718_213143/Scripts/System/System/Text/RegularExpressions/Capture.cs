namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Capture
	{
		internal int index;

		internal int length;

		internal string text;

		public int Index
		{
			get
			{
				return index;
			}
		}

		public int Length
		{
			get
			{
				return length;
			}
		}

		public string Value
		{
			get
			{
				return (text != null) ? text.Substring(index, length) : string.Empty;
			}
		}

		internal string Text
		{
			get
			{
				return text;
			}
		}

		internal Capture(string text)
			: this(text, 0, 0)
		{
		}

		internal Capture(string text, int index, int length)
		{
			this.text = text;
			this.index = index;
			this.length = length;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}
