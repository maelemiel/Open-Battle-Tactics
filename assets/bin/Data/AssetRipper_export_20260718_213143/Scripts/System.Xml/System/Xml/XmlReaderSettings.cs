using System.Xml.Schema;

namespace System.Xml
{
	public sealed class XmlReaderSettings
	{
		private bool checkCharacters;

		private bool closeInput;

		private ConformanceLevel conformance;

		private bool ignoreComments;

		private bool ignoreProcessingInstructions;

		private bool ignoreWhitespace;

		private int lineNumberOffset;

		private int linePositionOffset;

		private bool prohibitDtd;

		private XmlNameTable nameTable;

		private XmlSchemaSet schemas;

		private bool schemasNeedsInitialization;

		private XmlSchemaValidationFlags validationFlags;

		private ValidationType validationType;

		private XmlResolver xmlResolver;

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

		public bool CloseInput
		{
			get
			{
				return closeInput;
			}
			set
			{
				closeInput = value;
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

		public bool IgnoreComments
		{
			get
			{
				return ignoreComments;
			}
			set
			{
				ignoreComments = value;
			}
		}

		public bool IgnoreProcessingInstructions
		{
			get
			{
				return ignoreProcessingInstructions;
			}
			set
			{
				ignoreProcessingInstructions = value;
			}
		}

		public bool IgnoreWhitespace
		{
			get
			{
				return ignoreWhitespace;
			}
			set
			{
				ignoreWhitespace = value;
			}
		}

		public int LineNumberOffset
		{
			get
			{
				return lineNumberOffset;
			}
			set
			{
				lineNumberOffset = value;
			}
		}

		public int LinePositionOffset
		{
			get
			{
				return linePositionOffset;
			}
			set
			{
				linePositionOffset = value;
			}
		}

		public bool ProhibitDtd
		{
			get
			{
				return prohibitDtd;
			}
			set
			{
				prohibitDtd = value;
			}
		}

		public XmlNameTable NameTable
		{
			get
			{
				return nameTable;
			}
			set
			{
				nameTable = value;
			}
		}

		public XmlSchemaSet Schemas
		{
			get
			{
				if (schemasNeedsInitialization)
				{
					schemas = new XmlSchemaSet();
					schemasNeedsInitialization = false;
				}
				return schemas;
			}
			set
			{
				schemas = value;
				schemasNeedsInitialization = false;
			}
		}

		public XmlSchemaValidationFlags ValidationFlags
		{
			get
			{
				return validationFlags;
			}
			set
			{
				validationFlags = value;
			}
		}

		public ValidationType ValidationType
		{
			get
			{
				return validationType;
			}
			set
			{
				validationType = value;
			}
		}

		public XmlResolver XmlResolver
		{
			internal get
			{
				return xmlResolver;
			}
			set
			{
				xmlResolver = value;
			}
		}

		public event ValidationEventHandler ValidationEventHandler;

		public XmlReaderSettings()
		{
			Reset();
		}

		public XmlReaderSettings Clone()
		{
			return (XmlReaderSettings)MemberwiseClone();
		}

		public void Reset()
		{
			checkCharacters = true;
			closeInput = false;
			conformance = ConformanceLevel.Document;
			ignoreComments = false;
			ignoreProcessingInstructions = false;
			ignoreWhitespace = false;
			lineNumberOffset = 0;
			linePositionOffset = 0;
			prohibitDtd = true;
			schemas = null;
			schemasNeedsInitialization = true;
			validationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.AllowXmlAttributes;
			validationType = ValidationType.None;
			xmlResolver = new XmlUrlResolver();
		}

		internal void OnValidationError(object o, ValidationEventArgs e)
		{
			if (this.ValidationEventHandler != null)
			{
				this.ValidationEventHandler(o, e);
			}
			else if (e.Severity == XmlSeverityType.Error)
			{
				throw e.Exception;
			}
		}

		internal void SetSchemas(XmlSchemaSet schemas)
		{
			this.schemas = schemas;
		}
	}
}
