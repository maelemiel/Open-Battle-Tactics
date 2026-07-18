using System.Collections;
using System.Xml.Serialization;

namespace System.Xml.Schema
{
	public class XmlSchemaSimpleTypeUnion : XmlSchemaSimpleTypeContent
	{
		private const string xmlname = "union";

		private XmlSchemaObjectCollection baseTypes;

		private XmlQualifiedName[] memberTypes;

		private object[] validatedTypes;

		private XmlSchemaSimpleType[] validatedSchemaTypes;

		[XmlElement("simpleType", typeof(XmlSchemaSimpleType))]
		public XmlSchemaObjectCollection BaseTypes
		{
			get
			{
				return baseTypes;
			}
		}

		[XmlAttribute("memberTypes")]
		public XmlQualifiedName[] MemberTypes
		{
			get
			{
				return memberTypes;
			}
			set
			{
				memberTypes = value;
			}
		}

		[XmlIgnore]
		public XmlSchemaSimpleType[] BaseMemberTypes
		{
			get
			{
				return validatedSchemaTypes;
			}
		}

		internal object[] ValidatedTypes
		{
			get
			{
				return validatedTypes;
			}
		}

		public XmlSchemaSimpleTypeUnion()
		{
			baseTypes = new XmlSchemaObjectCollection();
		}

		internal override void SetParent(XmlSchemaObject parent)
		{
			base.SetParent(parent);
			foreach (XmlSchemaObject baseType in BaseTypes)
			{
				baseType.SetParent(this);
			}
		}

		internal override int Compile(ValidationEventHandler h, XmlSchema schema)
		{
			if (CompilationId == schema.CompilationId)
			{
				return 0;
			}
			errorCount = 0;
			int num = BaseTypes.Count;
			foreach (XmlSchemaObject baseType in baseTypes)
			{
				if (baseType != null && baseType is XmlSchemaSimpleType)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)baseType;
					errorCount += xmlSchemaSimpleType.Compile(h, schema);
				}
				else
				{
					error(h, "baseTypes can't have objects other than a simpletype");
				}
			}
			if (memberTypes != null)
			{
				for (int i = 0; i < memberTypes.Length; i++)
				{
					if (memberTypes[i] == null || !XmlSchemaUtil.CheckQName(MemberTypes[i]))
					{
						error(h, "Invalid membertype");
						memberTypes[i] = XmlQualifiedName.Empty;
					}
					else
					{
						num += MemberTypes.Length;
					}
				}
			}
			if (num == 0)
			{
				error(h, "Atleast one simpletype or membertype must be present");
			}
			XmlSchemaUtil.CompileID(base.Id, this, schema.IDCollection, h);
			CompilationId = schema.CompilationId;
			return errorCount;
		}

		internal override int Validate(ValidationEventHandler h, XmlSchema schema)
		{
			if (IsValidated(schema.ValidationId))
			{
				return errorCount;
			}
			ArrayList arrayList = new ArrayList();
			if (MemberTypes != null)
			{
				XmlQualifiedName[] array = MemberTypes;
				foreach (XmlQualifiedName xmlQualifiedName in array)
				{
					object obj = null;
					XmlSchemaType xmlSchemaType = schema.FindSchemaType(xmlQualifiedName) as XmlSchemaSimpleType;
					if (xmlSchemaType != null)
					{
						errorCount += xmlSchemaType.Validate(h, schema);
						obj = xmlSchemaType;
					}
					else if (xmlQualifiedName == XmlSchemaComplexType.AnyTypeName)
					{
						obj = XmlSchemaSimpleType.AnySimpleType;
					}
					else if (xmlQualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema" || xmlQualifiedName.Namespace == "http://www.w3.org/2003/11/xpath-datatypes")
					{
						obj = XmlSchemaDatatype.FromName(xmlQualifiedName);
						if (obj == null)
						{
							error(h, "Invalid schema type name was specified: " + xmlQualifiedName);
						}
					}
					else if (!schema.IsNamespaceAbsent(xmlQualifiedName.Namespace))
					{
						error(h, string.Concat("Referenced base schema type ", xmlQualifiedName, " was not found in the corresponding schema."));
					}
					arrayList.Add(obj);
				}
			}
			if (BaseTypes != null)
			{
				foreach (XmlSchemaSimpleType baseType in BaseTypes)
				{
					baseType.Validate(h, schema);
					arrayList.Add(baseType);
				}
			}
			validatedTypes = arrayList.ToArray();
			if (validatedTypes != null)
			{
				validatedSchemaTypes = new XmlSchemaSimpleType[validatedTypes.Length];
				for (int j = 0; j < validatedTypes.Length; j++)
				{
					object obj2 = validatedTypes[j];
					XmlSchemaSimpleType xmlSchemaSimpleType2 = obj2 as XmlSchemaSimpleType;
					if (xmlSchemaSimpleType2 == null && obj2 != null)
					{
						xmlSchemaSimpleType2 = XmlSchemaType.GetBuiltInSimpleType(((XmlSchemaDatatype)obj2).TypeCode);
					}
					validatedSchemaTypes[j] = xmlSchemaSimpleType2;
				}
			}
			ValidationId = schema.ValidationId;
			return errorCount;
		}

		internal static XmlSchemaSimpleTypeUnion Read(XmlSchemaReader reader, ValidationEventHandler h)
		{
			XmlSchemaSimpleTypeUnion xmlSchemaSimpleTypeUnion = new XmlSchemaSimpleTypeUnion();
			reader.MoveToElement();
			if (reader.NamespaceURI != "http://www.w3.org/2001/XMLSchema" || reader.LocalName != "union")
			{
				XmlSchemaObject.error(h, "Should not happen :1: XmlSchemaSimpleTypeUnion.Read, name=" + reader.Name, null);
				reader.Skip();
				return null;
			}
			xmlSchemaSimpleTypeUnion.LineNumber = reader.LineNumber;
			xmlSchemaSimpleTypeUnion.LinePosition = reader.LinePosition;
			xmlSchemaSimpleTypeUnion.SourceUri = reader.BaseURI;
			while (reader.MoveToNextAttribute())
			{
				if (reader.Name == "id")
				{
					xmlSchemaSimpleTypeUnion.Id = reader.Value;
				}
				else if (reader.Name == "memberTypes")
				{
					string[] array = XmlSchemaUtil.SplitList(reader.Value);
					xmlSchemaSimpleTypeUnion.memberTypes = new XmlQualifiedName[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						Exception innerEx;
						xmlSchemaSimpleTypeUnion.memberTypes[i] = XmlSchemaUtil.ToQName(reader, array[i], out innerEx);
						if (innerEx != null)
						{
							XmlSchemaObject.error(h, "'" + array[i] + "' is not a valid memberType", innerEx);
						}
					}
				}
				else if ((reader.NamespaceURI == string.Empty && reader.Name != "xmlns") || reader.NamespaceURI == "http://www.w3.org/2001/XMLSchema")
				{
					XmlSchemaObject.error(h, reader.Name + " is not a valid attribute for union", null);
				}
				else
				{
					XmlSchemaUtil.ReadUnhandledAttribute(reader, xmlSchemaSimpleTypeUnion);
				}
			}
			reader.MoveToElement();
			if (reader.IsEmptyElement)
			{
				return xmlSchemaSimpleTypeUnion;
			}
			int num = 1;
			while (reader.ReadNextElement())
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					if (reader.LocalName != "union")
					{
						XmlSchemaObject.error(h, "Should not happen :2: XmlSchemaSimpleTypeUnion.Read, name=" + reader.Name, null);
					}
					break;
				}
				if (num <= 1 && reader.LocalName == "annotation")
				{
					num = 2;
					XmlSchemaAnnotation xmlSchemaAnnotation = XmlSchemaAnnotation.Read(reader, h);
					if (xmlSchemaAnnotation != null)
					{
						xmlSchemaSimpleTypeUnion.Annotation = xmlSchemaAnnotation;
					}
				}
				else if (num <= 2 && reader.LocalName == "simpleType")
				{
					num = 2;
					XmlSchemaSimpleType xmlSchemaSimpleType = XmlSchemaSimpleType.Read(reader, h);
					if (xmlSchemaSimpleType != null)
					{
						xmlSchemaSimpleTypeUnion.baseTypes.Add(xmlSchemaSimpleType);
					}
				}
				else
				{
					reader.RaiseInvalidElementError();
				}
			}
			return xmlSchemaSimpleTypeUnion;
		}
	}
}
