using System.Xml;

namespace Mono.Xml
{
	internal abstract class DTDNode : IXmlLineInfo
	{
		private DTDObjectModel root;

		private bool isInternalSubset;

		private string baseURI;

		private int lineNumber;

		private int linePosition;

		public virtual string BaseURI
		{
			get
			{
				return baseURI;
			}
			set
			{
				baseURI = value;
			}
		}

		public bool IsInternalSubset
		{
			get
			{
				return isInternalSubset;
			}
			set
			{
				isInternalSubset = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return lineNumber;
			}
			set
			{
				lineNumber = value;
			}
		}

		public int LinePosition
		{
			get
			{
				return linePosition;
			}
			set
			{
				linePosition = value;
			}
		}

		protected DTDObjectModel Root
		{
			get
			{
				return root;
			}
		}

		public bool HasLineInfo()
		{
			return lineNumber != 0;
		}

		internal void SetRoot(DTDObjectModel root)
		{
			this.root = root;
			if (baseURI == null)
			{
				BaseURI = root.BaseURI;
			}
		}

		internal XmlException NotWFError(string message)
		{
			return new XmlException(this, BaseURI, message);
		}
	}
}
