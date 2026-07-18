using System.Text;

namespace System.Xml
{
	public sealed class XmlWriterSettings
	{
		private bool checkCharacters;

		private bool closeOutput;

		private ConformanceLevel conformance;

		private Encoding encoding;

		private bool indent;

		private string indentChars;

		private string newLineChars;

		private bool newLineOnAttributes;

		private NewLineHandling newLineHandling;

		private bool omitXmlDeclaration;

		private XmlOutputMethod outputMethod;

		public bool CheckCharacters
		{
			get
			{
				return checkCharacters;
			}
			set
			{
				checkCharacters = value;
			}
		}

		public bool CloseOutput
		{
			get
			{
				return closeOutput;
			}
			set
			{
				closeOutput = value;
			}
		}

		public ConformanceLevel ConformanceLevel
		{
			get
			{
				return conformance;
			}
			set
			{
				conformance = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				encoding = value;
			}
		}

		public bool Indent
		{
			get
			{
				return indent;
			}
			set
			{
				indent = value;
			}
		}

		public string IndentChars
		{
			get
			{
				return indentChars;
			}
			set
			{
				indentChars = value;
			}
		}

		public string NewLineChars
		{
			get
			{
				return newLineChars;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				newLineChars = value;
			}
		}

		public bool NewLineOnAttributes
		{
			get
			{
				return newLineOnAttributes;
			}
			set
			{
				newLineOnAttributes = value;
			}
		}

		public NewLineHandling NewLineHandling
		{
			get
			{
				return newLineHandling;
			}
			set
			{
				newLineHandling = value;
			}
		}

		public bool OmitXmlDeclaration
		{
			get
			{
				return omitXmlDeclaration;
			}
			set
			{
				omitXmlDeclaration = value;
			}
		}

		public XmlOutputMethod OutputMethod
		{
			get
			{
				return outputMethod;
			}
		}

		internal NamespaceHandling NamespaceHandling { get; set; }

		public XmlWriterSettings()
		{
			Reset();
		}

		public XmlWriterSettings Clone()
		{
			return (XmlWriterSettings)MemberwiseClone();
		}

		public void Reset()
		{
			checkCharacters = true;
			closeOutput = false;
			conformance = ConformanceLevel.Document;
			encoding = Encoding.UTF8;
			indent = false;
			indentChars = "  ";
			newLineChars = Environment.NewLine;
			newLineOnAttributes = false;
			newLineHandling = NewLineHandling.None;
			omitXmlDeclaration = false;
			outputMethod = XmlOutputMethod.AutoDetect;
		}
	}
}
