namespace System.Xml.Serialization
{
	public class XmlElementEventArgs : EventArgs
	{
		private XmlElement attr;

		private int lineNumber;

		private int linePosition;

		private object obj;

		private string expectedElements;

		public XmlElement Element
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

		public string ExpectedElements
		{
			get
			{
				return expectedElements;
			}
			internal set
			{
				expectedElements = value;
			}
		}

		internal XmlElementEventArgs(XmlElement attr, int lineNum, int linePos, object source)
		{
			this.attr = attr;
			lineNumber = lineNum;
			linePosition = linePos;
			obj = source;
		}
	}
}
