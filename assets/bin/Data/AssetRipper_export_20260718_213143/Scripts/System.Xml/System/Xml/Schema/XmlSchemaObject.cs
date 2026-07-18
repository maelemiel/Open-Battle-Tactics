using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaObject
	{
		private int lineNumber;

		private int linePosition;

		private string sourceUri;

		private XmlSerializerNamespaces namespaces;

		internal ArrayList unhandledAttributeList;

		internal bool isCompiled;

		internal int errorCount;

		internal Guid CompilationId;

		internal Guid ValidationId;

		internal bool isRedefineChild;

		internal bool isRedefinedComponent;

		internal XmlSchemaObject redefinedObject;

		private XmlSchemaObject parent;

		[XmlIgnore]
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

		[XmlIgnore]
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

		[XmlIgnore]
		public string SourceUri
		{
			get
			{
				return sourceUri;
			}
			set
			{
				sourceUri = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaObject Parent
		{
			get
			{
				return parent;
			}
			set
			{
				parent = value;
			}
		}

		internal XmlSchema AncestorSchema
		{
			get
			{
				for (XmlSchemaObject xmlSchemaObject = Parent; xmlSchemaObject != null; xmlSchemaObject = xmlSchemaObject.Parent)
				{
					if (xmlSchemaObject is XmlSchema)
					{
						return (XmlSchema)xmlSchemaObject;
					}
				}
				throw new Exception(string.Format("INTERNAL ERROR: Parent object is not set properly : {0} ({1},{2})", SourceUri, LineNumber, LinePosition));
			}
		}

		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces Namespaces
		{
			get
			{
				return namespaces;
			}
			set
			{
				namespaces = value;
			}
		}

		protected XmlSchemaObject()
		{
			namespaces = new XmlSerializerNamespaces();
			unhandledAttributeList = null;
			CompilationId = Guid.Empty;
		}

		internal virtual void SetParent(XmlSchemaObject parent)
		{
			Parent = parent;
		}

		internal void error(ValidationEventHandler handle, string message)
		{
			errorCount++;
			error(handle, message, null, this, null);
		}

		internal void warn(ValidationEventHandler handle, string message)
		{
			warn(handle, message, null, this, null);
		}

		internal static void error(ValidationEventHandler handle, string message, Exception innerException)
		{
			error(handle, message, innerException, null, null);
		}

		internal static void warn(ValidationEventHandler handle, string message, Exception innerException)
		{
			warn(handle, message, innerException, null, null);
		}

		internal static void error(ValidationEventHandler handle, string message, Exception innerException, XmlSchemaObject xsobj, object sender)
		{
			ValidationHandler.RaiseValidationEvent(handle, innerException, message, xsobj, sender, null, XmlSeverityType.Error);
		}

		internal static void warn(ValidationEventHandler handle, string message, Exception innerException, XmlSchemaObject xsobj, object sender)
		{
			ValidationHandler.RaiseValidationEvent(handle, innerException, message, xsobj, sender, null, XmlSeverityType.Warning);
		}

		internal virtual int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			return 0;
		}

		internal virtual int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			return 0;
		}

		internal bool IsValidated(Guid validationId)
		{
			return ValidationId == validationId;
		}

		internal virtual void CopyInfo(XmlSchemaParticle obj)
		{
			obj.LineNumber = LineNumber;
			obj.LinePosition = LinePosition;
			obj.SourceUri = SourceUri;
			obj.errorCount = errorCount;
		}
	}
}
