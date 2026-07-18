using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Xml.Serialization
{
	public abstract class XmlSerializationWriter : XmlSerializationGeneratedCode
	{
		private class WriteCallbackInfo
		{
			public Type Type;

			public string TypeName;

			public string TypeNs;

			public XmlSerializationWriteCallback Callback;
		}

		private const string xmlNamespace = "http://www.w3.org/2000/xmlns/";

		private const string unexpectedTypeError = "The type {0} was not expected. Use the XmlInclude or SoapInclude attribute to specify types that are not known statically.";

		private ObjectIDGenerator idGenerator;

		private int qnameCount;

		private bool topLevelElement;

		private ArrayList namespaces;

		private XmlWriter writer;

		private Queue referencedElements;

		private Hashtable callbacks;

		private Hashtable serializedObjects;

		protected ArrayList Namespaces
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

		protected XmlWriter Writer
		{
			get
			{
				return writer;
			}
			set
			{
				writer = value;
			}
		}

		[System.MonoTODO]
		protected bool EscapeName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		protected XmlSerializationWriter()
		{
			qnameCount = 0;
			serializedObjects = new Hashtable();
		}

		internal void Initialize(XmlWriter writer, XmlSerializerNamespaces nss)
		{
			this.writer = writer;
			if (nss == null)
			{
				return;
			}
			namespaces = new ArrayList();
			XmlQualifiedName[] array = nss.ToArray();
			foreach (XmlQualifiedName xmlQualifiedName in array)
			{
				if (xmlQualifiedName.Name != string.Empty && xmlQualifiedName.Namespace != string.Empty)
				{
					namespaces.Add(xmlQualifiedName);
				}
			}
		}

		protected void AddWriteCallback(Type type, string typeName, string typeNs, XmlSerializationWriteCallback callback)
		{
			WriteCallbackInfo writeCallbackInfo = new WriteCallbackInfo();
			writeCallbackInfo.Type = type;
			writeCallbackInfo.TypeName = typeName;
			writeCallbackInfo.TypeNs = typeNs;
			writeCallbackInfo.Callback = callback;
			if (callbacks == null)
			{
				callbacks = new Hashtable();
			}
			callbacks.Add(type, writeCallbackInfo);
		}

		protected Exception CreateChoiceIdentifierValueException(string value, string identifier, string name, string ns)
		{
			string message = string.Format("Value '{0}' of the choice identifier '{1}' does not match element '{2}' from namespace '{3}'.", value, identifier, name, ns);
			return new InvalidOperationException(message);
		}

		protected Exception CreateInvalidChoiceIdentifierValueException(string type, string identifier)
		{
			string message = string.Format("Invalid or missing choice identifier '{0}' of type '{1}'.", identifier, type);
			return new InvalidOperationException(message);
		}

		protected Exception CreateMismatchChoiceException(string value, string elementName, string enumValue)
		{
			string message = string.Format("Value of {0} mismatches the type of {1}, you need to set it to {2}.", elementName, value, enumValue);
			return new InvalidOperationException(message);
		}

		protected Exception CreateUnknownAnyElementException(string name, string ns)
		{
			string message = string.Format("The XML element named '{0}' from namespace '{1}' was not expected. The XML element name and namespace must match those provided via XmlAnyElementAttribute(s).", name, ns);
			return new InvalidOperationException(message);
		}

		protected Exception CreateUnknownTypeException(object o)
		{
			return CreateUnknownTypeException(o.GetType());
		}

		protected Exception CreateUnknownTypeException(Type type)
		{
			string message = string.Format("The type {0} may not be used in this context.", type);
			return new InvalidOperationException(message);
		}

		protected static byte[] FromByteArrayBase64(byte[] value)
		{
			return value;
		}

		protected static string FromByteArrayHex(byte[] value)
		{
			return XmlCustomFormatter.FromByteArrayHex(value);
		}

		protected static string FromChar(char value)
		{
			return XmlCustomFormatter.FromChar(value);
		}

		protected static string FromDate(DateTime value)
		{
			return XmlCustomFormatter.FromDate(value);
		}

		protected static string FromDateTime(DateTime value)
		{
			return XmlCustomFormatter.FromDateTime(value);
		}

		protected static string FromEnum(long value, string[] values, long[] ids)
		{
			return XmlCustomFormatter.FromEnum(value, values, ids);
		}

		protected static string FromTime(DateTime value)
		{
			return XmlCustomFormatter.FromTime(value);
		}

		protected static string FromXmlName(string name)
		{
			return XmlCustomFormatter.FromXmlName(name);
		}

		protected static string FromXmlNCName(string ncName)
		{
			return XmlCustomFormatter.FromXmlNCName(ncName);
		}

		protected static string FromXmlNmToken(string nmToken)
		{
			return XmlCustomFormatter.FromXmlNmToken(nmToken);
		}

		protected static string FromXmlNmTokens(string nmTokens)
		{
			return XmlCustomFormatter.FromXmlNmTokens(nmTokens);
		}

		protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName)
		{
			if (xmlQualifiedName == null || xmlQualifiedName == XmlQualifiedName.Empty)
			{
				return null;
			}
			return GetQualifiedName(xmlQualifiedName.Name, xmlQualifiedName.Namespace);
		}

		private string GetId(object o, bool addToReferencesList)
		{
			if (idGenerator == null)
			{
				idGenerator = new ObjectIDGenerator();
			}
			bool firstTime;
			long id = idGenerator.GetId(o, out firstTime);
			return string.Format(CultureInfo.InvariantCulture, "id{0}", id);
		}

		private bool AlreadyQueued(object ob)
		{
			if (idGenerator == null)
			{
				return false;
			}
			bool firstTime;
			idGenerator.HasId(ob, out firstTime);
			return !firstTime;
		}

		private string GetNamespacePrefix(string ns)
		{
			string text = Writer.LookupPrefix(ns);
			if (text == null)
			{
				text = string.Format(CultureInfo.InvariantCulture, "q{0}", ++qnameCount);
				WriteAttribute("xmlns", text, null, ns);
			}
			return text;
		}

		private string GetQualifiedName(string name, string ns)
		{
			if (ns == string.Empty)
			{
				return name;
			}
			string namespacePrefix = GetNamespacePrefix(ns);
			if (namespacePrefix == string.Empty)
			{
				return name;
			}
			return string.Format("{0}:{1}", namespacePrefix, name);
		}

		protected abstract void InitCallbacks();

		protected void TopLevelElement()
		{
			topLevelElement = true;
		}

		protected void WriteAttribute(string localName, byte[] value)
		{
			WriteAttribute(localName, string.Empty, value);
		}

		protected void WriteAttribute(string localName, string value)
		{
			WriteAttribute(string.Empty, localName, string.Empty, value);
		}

		protected void WriteAttribute(string localName, string ns, byte[] value)
		{
			if (value != null)
			{
				Writer.WriteStartAttribute(localName, ns);
				WriteValue(value);
				Writer.WriteEndAttribute();
			}
		}

		protected void WriteAttribute(string localName, string ns, string value)
		{
			WriteAttribute(null, localName, ns, value);
		}

		protected void WriteAttribute(string prefix, string localName, string ns, string value)
		{
			if (value != null)
			{
				Writer.WriteAttributeString(prefix, localName, ns, value);
			}
		}

		private void WriteXmlNode(XmlNode node)
		{
			if (node is XmlDocument)
			{
				node = ((XmlDocument)node).DocumentElement;
			}
			node.WriteTo(Writer);
		}

		protected void WriteElementEncoded(XmlNode node, string name, string ns, bool isNullable, bool any)
		{
			if (name != string.Empty)
			{
				if (node == null)
				{
					if (isNullable)
					{
						WriteNullTagEncoded(name, ns);
					}
				}
				else
				{
					Writer.WriteStartElement(name, ns);
					WriteXmlNode(node);
					Writer.WriteEndElement();
				}
			}
			else
			{
				WriteXmlNode(node);
			}
		}

		protected void WriteElementLiteral(XmlNode node, string name, string ns, bool isNullable, bool any)
		{
			if (name != string.Empty)
			{
				if (node == null)
				{
					if (isNullable)
					{
						WriteNullTagLiteral(name, ns);
					}
				}
				else
				{
					Writer.WriteStartElement(name, ns);
					WriteXmlNode(node);
					Writer.WriteEndElement();
				}
			}
			else
			{
				WriteXmlNode(node);
			}
		}

		protected void WriteElementQualifiedName(string localName, XmlQualifiedName value)
		{
			WriteElementQualifiedName(localName, string.Empty, value, null);
		}

		protected void WriteElementQualifiedName(string localName, string ns, XmlQualifiedName value)
		{
			WriteElementQualifiedName(localName, ns, value, null);
		}

		protected void WriteElementQualifiedName(string localName, XmlQualifiedName value, XmlQualifiedName xsiType)
		{
			WriteElementQualifiedName(localName, string.Empty, value, xsiType);
		}

		protected void WriteElementQualifiedName(string localName, string ns, XmlQualifiedName value, XmlQualifiedName xsiType)
		{
			localName = XmlCustomFormatter.FromXmlNCName(localName);
			WriteStartElement(localName, ns);
			if (xsiType != null)
			{
				WriteXsiType(xsiType.Name, xsiType.Namespace);
			}
			Writer.WriteString(FromXmlQualifiedName(value));
			WriteEndElement();
		}

		protected void WriteElementString(string localName, string value)
		{
			WriteElementString(localName, string.Empty, value, null);
		}

		protected void WriteElementString(string localName, string ns, string value)
		{
			WriteElementString(localName, ns, value, null);
		}

		protected void WriteElementString(string localName, string value, XmlQualifiedName xsiType)
		{
			WriteElementString(localName, string.Empty, value, xsiType);
		}

		protected void WriteElementString(string localName, string ns, string value, XmlQualifiedName xsiType)
		{
			if (value != null)
			{
				if (xsiType != null)
				{
					localName = XmlCustomFormatter.FromXmlNCName(localName);
					WriteStartElement(localName, ns);
					WriteXsiType(xsiType.Name, xsiType.Namespace);
					Writer.WriteString(value);
					WriteEndElement();
				}
				else
				{
					Writer.WriteElementString(localName, ns, value);
				}
			}
		}

		protected void WriteElementStringRaw(string localName, byte[] value)
		{
			WriteElementStringRaw(localName, string.Empty, value, null);
		}

		protected void WriteElementStringRaw(string localName, string value)
		{
			WriteElementStringRaw(localName, string.Empty, value, null);
		}

		protected void WriteElementStringRaw(string localName, byte[] value, XmlQualifiedName xsiType)
		{
			WriteElementStringRaw(localName, string.Empty, value, xsiType);
		}

		protected void WriteElementStringRaw(string localName, string ns, byte[] value)
		{
			WriteElementStringRaw(localName, ns, value, null);
		}

		protected void WriteElementStringRaw(string localName, string ns, string value)
		{
			WriteElementStringRaw(localName, ns, value, null);
		}

		protected void WriteElementStringRaw(string localName, string value, XmlQualifiedName xsiType)
		{
			WriteElementStringRaw(localName, string.Empty, value, null);
		}

		protected void WriteElementStringRaw(string localName, string ns, byte[] value, XmlQualifiedName xsiType)
		{
			if (value != null)
			{
				WriteStartElement(localName, ns);
				if (xsiType != null)
				{
					WriteXsiType(xsiType.Name, xsiType.Namespace);
				}
				if (value.Length > 0)
				{
					Writer.WriteBase64(value, 0, value.Length);
				}
				WriteEndElement();
			}
		}

		protected void WriteElementStringRaw(string localName, string ns, string value, XmlQualifiedName xsiType)
		{
			localName = XmlCustomFormatter.FromXmlNCName(localName);
			WriteStartElement(localName, ns);
			if (xsiType != null)
			{
				WriteXsiType(xsiType.Name, xsiType.Namespace);
			}
			Writer.WriteRaw(value);
			WriteEndElement();
		}

		protected void WriteEmptyTag(string name)
		{
			WriteEmptyTag(name, string.Empty);
		}

		protected void WriteEmptyTag(string name, string ns)
		{
			name = XmlCustomFormatter.FromXmlName(name);
			WriteStartElement(name, ns);
			WriteEndElement();
		}

		protected void WriteEndElement()
		{
			WriteEndElement(null);
		}

		protected void WriteEndElement(object o)
		{
			if (o != null)
			{
				serializedObjects.Remove(o);
			}
			Writer.WriteEndElement();
		}

		protected void WriteId(object o)
		{
			WriteAttribute("id", GetId(o, true));
		}

		protected void WriteNamespaceDeclarations(XmlSerializerNamespaces ns)
		{
			if (ns == null)
			{
				return;
			}
			ICollection values = ns.Namespaces.Values;
			foreach (XmlQualifiedName item in values)
			{
				if (item.Namespace != string.Empty && Writer.LookupPrefix(item.Namespace) != item.Name)
				{
					WriteAttribute("xmlns", item.Name, "http://www.w3.org/2000/xmlns/", item.Namespace);
				}
			}
		}

		protected void WriteNullableQualifiedNameEncoded(string name, string ns, XmlQualifiedName value, XmlQualifiedName xsiType)
		{
			if (value != null)
			{
				WriteElementQualifiedName(name, ns, value, xsiType);
			}
			else
			{
				WriteNullTagEncoded(name, ns);
			}
		}

		protected void WriteNullableQualifiedNameLiteral(string name, string ns, XmlQualifiedName value)
		{
			if (value != null)
			{
				WriteElementQualifiedName(name, ns, value);
			}
			else
			{
				WriteNullTagLiteral(name, ns);
			}
		}

		protected void WriteNullableStringEncoded(string name, string ns, string value, XmlQualifiedName xsiType)
		{
			if (value != null)
			{
				WriteElementString(name, ns, value, xsiType);
			}
			else
			{
				WriteNullTagEncoded(name, ns);
			}
		}

		protected void WriteNullableStringEncodedRaw(string name, string ns, byte[] value, XmlQualifiedName xsiType)
		{
			if (value == null)
			{
				WriteNullTagEncoded(name, ns);
			}
			else
			{
				WriteElementStringRaw(name, ns, value, xsiType);
			}
		}

		protected void WriteNullableStringEncodedRaw(string name, string ns, string value, XmlQualifiedName xsiType)
		{
			if (value == null)
			{
				WriteNullTagEncoded(name, ns);
			}
			else
			{
				WriteElementStringRaw(name, ns, value, xsiType);
			}
		}

		protected void WriteNullableStringLiteral(string name, string ns, string value)
		{
			if (value != null)
			{
				WriteElementString(name, ns, value, null);
			}
			else
			{
				WriteNullTagLiteral(name, ns);
			}
		}

		protected void WriteNullableStringLiteralRaw(string name, string ns, byte[] value)
		{
			if (value == null)
			{
				WriteNullTagLiteral(name, ns);
			}
			else
			{
				WriteElementStringRaw(name, ns, value);
			}
		}

		protected void WriteNullableStringLiteralRaw(string name, string ns, string value)
		{
			if (value == null)
			{
				WriteNullTagLiteral(name, ns);
			}
			else
			{
				WriteElementStringRaw(name, ns, value);
			}
		}

		protected void WriteNullTagEncoded(string name)
		{
			WriteNullTagEncoded(name, string.Empty);
		}

		protected void WriteNullTagEncoded(string name, string ns)
		{
			Writer.WriteStartElement(name, ns);
			Writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			Writer.WriteEndElement();
		}

		protected void WriteNullTagLiteral(string name)
		{
			WriteNullTagLiteral(name, string.Empty);
		}

		protected void WriteNullTagLiteral(string name, string ns)
		{
			WriteStartElement(name, ns);
			Writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
			WriteEndElement();
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o)
		{
			WritePotentiallyReferencingElement(n, ns, o, null, false, false);
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType)
		{
			WritePotentiallyReferencingElement(n, ns, o, ambientType, false, false);
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference)
		{
			WritePotentiallyReferencingElement(n, ns, o, ambientType, suppressReference, false);
		}

		protected void WritePotentiallyReferencingElement(string n, string ns, object o, Type ambientType, bool suppressReference, bool isNullable)
		{
			if (o == null)
			{
				if (isNullable)
				{
					WriteNullTagEncoded(n, ns);
				}
				return;
			}
			WriteStartElement(n, ns, true);
			CheckReferenceQueue();
			if (callbacks != null && callbacks.ContainsKey(o.GetType()))
			{
				WriteCallbackInfo writeCallbackInfo = (WriteCallbackInfo)callbacks[o.GetType()];
				if (o.GetType().IsEnum)
				{
					writeCallbackInfo.Callback(o);
				}
				else if (suppressReference)
				{
					Writer.WriteAttributeString("id", GetId(o, false));
					if (ambientType != o.GetType())
					{
						WriteXsiType(writeCallbackInfo.TypeName, writeCallbackInfo.TypeNs);
					}
					writeCallbackInfo.Callback(o);
				}
				else
				{
					if (!AlreadyQueued(o))
					{
						referencedElements.Enqueue(o);
					}
					Writer.WriteAttributeString("href", "#" + GetId(o, true));
				}
			}
			else
			{
				TypeData typeData = TypeTranslator.GetTypeData(o.GetType());
				if (typeData.SchemaType == SchemaTypes.Primitive)
				{
					WriteXsiType(typeData.XmlType, "http://www.w3.org/2001/XMLSchema");
					Writer.WriteString(XmlCustomFormatter.ToXmlString(typeData, o));
				}
				else
				{
					if (!IsPrimitiveArray(typeData))
					{
						throw new InvalidOperationException("Invalid type: " + o.GetType().FullName);
					}
					if (!AlreadyQueued(o))
					{
						referencedElements.Enqueue(o);
					}
					Writer.WriteAttributeString("href", "#" + GetId(o, true));
				}
			}
			WriteEndElement();
		}

		protected void WriteReferencedElements()
		{
			if (referencedElements == null || callbacks == null)
			{
				return;
			}
			while (referencedElements.Count > 0)
			{
				object obj = referencedElements.Dequeue();
				TypeData typeData = TypeTranslator.GetTypeData(obj.GetType());
				WriteCallbackInfo writeCallbackInfo = (WriteCallbackInfo)callbacks[obj.GetType()];
				if (writeCallbackInfo != null)
				{
					WriteStartElement(writeCallbackInfo.TypeName, writeCallbackInfo.TypeNs, true);
					Writer.WriteAttributeString("id", GetId(obj, false));
					if (typeData.SchemaType != SchemaTypes.Array)
					{
						WriteXsiType(writeCallbackInfo.TypeName, writeCallbackInfo.TypeNs);
					}
					writeCallbackInfo.Callback(obj);
					WriteEndElement();
				}
				else if (IsPrimitiveArray(typeData))
				{
					WriteArray(obj, typeData);
				}
			}
		}

		private bool IsPrimitiveArray(TypeData td)
		{
			if (td.SchemaType == SchemaTypes.Array)
			{
				if (td.ListItemTypeData.SchemaType == SchemaTypes.Primitive || td.ListItemType == typeof(object))
				{
					return true;
				}
				return IsPrimitiveArray(td.ListItemTypeData);
			}
			return false;
		}

		private void WriteArray(object o, TypeData td)
		{
			TypeData typeData = td;
			int num = -1;
			string text;
			do
			{
				typeData = typeData.ListItemTypeData;
				text = typeData.XmlType;
				num++;
			}
			while (typeData.SchemaType == SchemaTypes.Array);
			while (num-- > 0)
			{
				text += "[]";
			}
			WriteStartElement("Array", "http://schemas.xmlsoap.org/soap/encoding/", true);
			Writer.WriteAttributeString("id", GetId(o, false));
			if (td.SchemaType == SchemaTypes.Array)
			{
				Array array = (Array)o;
				int length = array.Length;
				Writer.WriteAttributeString("arrayType", "http://schemas.xmlsoap.org/soap/encoding/", GetQualifiedName(text, "http://www.w3.org/2001/XMLSchema") + "[" + length + "]");
				for (int i = 0; i < length; i++)
				{
					WritePotentiallyReferencingElement("Item", string.Empty, array.GetValue(i), td.ListItemType, false, true);
				}
			}
			WriteEndElement();
		}

		protected void WriteReferencingElement(string n, string ns, object o)
		{
			WriteReferencingElement(n, ns, o, false);
		}

		protected void WriteReferencingElement(string n, string ns, object o, bool isNullable)
		{
			if (o == null)
			{
				if (isNullable)
				{
					WriteNullTagEncoded(n, ns);
				}
				return;
			}
			CheckReferenceQueue();
			if (!AlreadyQueued(o))
			{
				referencedElements.Enqueue(o);
			}
			Writer.WriteStartElement(n, ns);
			Writer.WriteAttributeString("href", "#" + GetId(o, true));
			Writer.WriteEndElement();
		}

		private void CheckReferenceQueue()
		{
			if (referencedElements == null)
			{
				referencedElements = new Queue();
				InitCallbacks();
			}
		}

		[System.MonoTODO]
		protected void WriteRpcResult(string name, string ns)
		{
			throw new NotImplementedException();
		}

		protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable)
		{
			WriteSerializable(serializable, name, ns, isNullable, true);
		}

		protected void WriteSerializable(IXmlSerializable serializable, string name, string ns, bool isNullable, bool wrapped)
		{
			if (serializable == null)
			{
				if (isNullable && wrapped)
				{
					WriteNullTagLiteral(name, ns);
				}
				return;
			}
			if (wrapped)
			{
				Writer.WriteStartElement(name, ns);
			}
			serializable.WriteXml(Writer);
			if (wrapped)
			{
				Writer.WriteEndElement();
			}
		}

		protected void WriteStartDocument()
		{
			if (Writer.WriteState == WriteState.Start)
			{
				Writer.WriteStartDocument();
			}
		}

		protected void WriteStartElement(string name)
		{
			WriteStartElement(name, string.Empty, null, false);
		}

		protected void WriteStartElement(string name, string ns)
		{
			WriteStartElement(name, ns, null, false);
		}

		protected void WriteStartElement(string name, string ns, bool writePrefixed)
		{
			WriteStartElement(name, ns, null, writePrefixed);
		}

		protected void WriteStartElement(string name, string ns, object o)
		{
			WriteStartElement(name, ns, o, false);
		}

		protected void WriteStartElement(string name, string ns, object o, bool writePrefixed)
		{
			WriteStartElement(name, ns, o, writePrefixed, namespaces);
		}

		protected void WriteStartElement(string name, string ns, object o, bool writePrefixed, XmlSerializerNamespaces xmlns)
		{
			if (xmlns == null)
			{
				throw new ArgumentNullException("xmlns");
			}
			WriteStartElement(name, ns, o, writePrefixed, xmlns.ToArray());
		}

		private void WriteStartElement(string name, string ns, object o, bool writePrefixed, ICollection namespaces)
		{
			if (o != null)
			{
				if (serializedObjects.Contains(o))
				{
					throw new InvalidOperationException("A circular reference was detected while serializing an object of type " + o.GetType().Name);
				}
				serializedObjects[o] = o;
			}
			string text = null;
			if (topLevelElement && ns != null && ns.Length != 0)
			{
				foreach (XmlQualifiedName @namespace in namespaces)
				{
					if (@namespace.Namespace == ns)
					{
						text = @namespace.Name;
						writePrefixed = true;
						break;
					}
				}
			}
			if (writePrefixed && ns != string.Empty)
			{
				name = XmlCustomFormatter.FromXmlName(name);
				if (text == null)
				{
					text = Writer.LookupPrefix(ns);
				}
				if (text == null || text.Length == 0)
				{
					text = "q" + ++qnameCount;
				}
				Writer.WriteStartElement(text, name, ns);
			}
			else
			{
				Writer.WriteStartElement(name, ns);
			}
			if (!topLevelElement)
			{
				return;
			}
			if (namespaces != null)
			{
				foreach (XmlQualifiedName namespace2 in namespaces)
				{
					string text2 = Writer.LookupPrefix(namespace2.Namespace);
					if (text2 == null || text2.Length == 0)
					{
						WriteAttribute("xmlns", namespace2.Name, "http://www.w3.org/2000/xmlns/", namespace2.Namespace);
					}
				}
			}
			topLevelElement = false;
		}

		protected void WriteTypedPrimitive(string name, string ns, object o, bool xsiType)
		{
			TypeData typeData = TypeTranslator.GetTypeData(o.GetType());
			if (typeData.SchemaType != SchemaTypes.Primitive)
			{
				throw new InvalidOperationException(string.Format("The type of the argument object '{0}' is not primitive.", typeData.FullTypeName));
			}
			if (name == null)
			{
				ns = ((!typeData.IsXsdType) ? "http://microsoft.com/wsdl/types/" : "http://www.w3.org/2001/XMLSchema");
				name = typeData.XmlType;
			}
			else
			{
				name = XmlCustomFormatter.FromXmlName(name);
			}
			Writer.WriteStartElement(name, ns);
			string value = ((!(o is XmlQualifiedName)) ? XmlCustomFormatter.ToXmlString(typeData, o) : FromXmlQualifiedName((XmlQualifiedName)o));
			if (xsiType)
			{
				if (typeData.SchemaType != SchemaTypes.Primitive)
				{
					throw new InvalidOperationException(string.Format("The type {0} was not expected. Use the XmlInclude or SoapInclude attribute to specify types that are not known statically.", o.GetType().FullName));
				}
				WriteXsiType(typeData.XmlType, (!typeData.IsXsdType) ? "http://microsoft.com/wsdl/types/" : "http://www.w3.org/2001/XMLSchema");
			}
			WriteValue(value);
			Writer.WriteEndElement();
		}

		protected void WriteValue(byte[] value)
		{
			Writer.WriteBase64(value, 0, value.Length);
		}

		protected void WriteValue(string value)
		{
			if (value != null)
			{
				Writer.WriteString(value);
			}
		}

		protected void WriteXmlAttribute(XmlNode node)
		{
			WriteXmlAttribute(node, null);
		}

		protected void WriteXmlAttribute(XmlNode node, object container)
		{
			XmlAttribute xmlAttribute = node as XmlAttribute;
			if (xmlAttribute == null)
			{
				throw new InvalidOperationException("The node must be either type XmlAttribute or a derived type.");
			}
			if (xmlAttribute.NamespaceURI == "http://schemas.xmlsoap.org/wsdl/" && xmlAttribute.LocalName == "arrayType")
			{
				string type;
				string ns;
				string dimensions;
				TypeTranslator.ParseArrayType(xmlAttribute.Value, out type, out ns, out dimensions);
				string qualifiedName = GetQualifiedName(type + dimensions, ns);
				WriteAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, qualifiedName);
			}
			else
			{
				WriteAttribute(xmlAttribute.Prefix, xmlAttribute.LocalName, xmlAttribute.NamespaceURI, xmlAttribute.Value);
			}
		}

		protected void WriteXsiType(string name, string ns)
		{
			if (ns != null && ns != string.Empty)
			{
				WriteAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", GetQualifiedName(name, ns));
			}
			else
			{
				WriteAttribute("type", "http://www.w3.org/2001/XMLSchema-instance", name);
			}
		}

		protected Exception CreateInvalidAnyTypeException(object o)
		{
			if (o == null)
			{
				return new InvalidOperationException("null is invalid as anyType in XmlSerializer");
			}
			return CreateInvalidAnyTypeException(o.GetType());
		}

		protected Exception CreateInvalidAnyTypeException(Type t)
		{
			return new InvalidOperationException(string.Format("An object of type '{0}' is invalid as anyType in XmlSerializer", t));
		}

		protected Exception CreateInvalidEnumValueException(object value, string typeName)
		{
			return new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "'{0}' is not a valid value for {1}.", value, typeName));
		}

		protected static string FromEnum(long value, string[] values, long[] ids, string typeName)
		{
			return XmlCustomFormatter.FromEnum(value, values, ids, typeName);
		}

		[System.MonoTODO]
		protected string FromXmlQualifiedName(XmlQualifiedName xmlQualifiedName, bool ignoreEmpty)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected static Assembly ResolveDynamicAssembly(string assemblyFullName)
		{
			throw new NotImplementedException();
		}
	}
}
