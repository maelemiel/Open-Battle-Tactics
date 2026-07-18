namespace System.Xml.Serialization
{
	public class XmlAttributeEventArgs : EventArgs
	{
		private XmlAttribute attr;

		private int lineNumber;

		private int linePosition;

		private object obj;

		private string expectedAttributes;

		public XmlAttribute Attr
		{
			get
			{
				return attr;
			}
		}

		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return linePosition;
			}
		}

		public object ObjectBeingDeserialized
		{
			get
			{
				return obj;
			}
		}

		public string ExpectedAttributes
		{
			get
			{
				return expectedAttributes;
			}
			internal set
			{
				expectedAttributes = value;
			}
		}

		internal XmlAttributeEventArgs(XmlAttribute attr, int lineNum, int linePos, object source)
		{
			this.attr = attr;
			lineNumber = lineNum;
			linePosition = linePos;
			obj = source;
		}
	}
}
