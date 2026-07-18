using System.Collections;
using System.Globalization;
using System.Reflection;

namespace System.Xml.Serialization
{
	[System.MonoTODO]
	public abstract class XmlSerializationReader : XmlSerializationGeneratedCode
	{
		private class WriteCallbackInfo
		{
			public Type Type;

			public string TypeName;

			public string TypeNs;

			public XmlSerializationReadCallback Callback;
		}

		protected class CollectionFixup
		{
			private XmlSerializationCollectionFixupCallback callback;

			private object collection;

			private object collectionItems;

			private string id;

			public XmlSerializationCollectionFixupCallback Callback
			{
				get
				{
					return callback;
				}
			}

			public object Collection
			{
				get
				{
					return collection;
				}
			}

			public object Id
			{
				get
				{
					return id;
				}
			}

			internal object CollectionItems
			{
				get
				{
					return collectionItems;
				}
				set
				{
					collectionItems = value;
				}
			}

			public CollectionFixup(object collection, XmlSerializationCollectionFixupCallback callback, string id)
			{
				this.callback = callback;
				this.collection = collection;
				this.id = id;
			}
		}

		protected class Fixup
		{
			private object source;

			private string[] ids;

			private XmlSerializationFixupCallback callback;

			public XmlSerializationFixupCallback Callback
			{
				get
				{
					return callback;
				}
			}

			public string[] Ids
			{
				get
				{
					return ids;
				}
			}

			public object Source
			{
				get
				{
					return source;
				}
				set
				{
					source = value;
				}
			}

			public Fixup(object o, XmlSerializationFixupCallback callback, int count)
			{
				source = o;
				this.callback = callback;
				ids = new string[count];
			}

			public Fixup(object o, XmlSerializationFixupCallback callback, string[] ids)
			{
				source = o;
				this.ids = ids;
				this.callback = callback;
			}
		}

		protected class CollectionItemFixup
		{
			private Array list;

			private int index;

			private string id;

			public Array Collection
			{
				get
				{
					return list;
				}
			}

			public int Index
			{
				get
				{
					return index;
				}
			}

			public string Id
			{
				get
				{
					return id;
				}
			}

			public CollectionItemFixup(Array list, int index, string id)
			{
				this.list = list;
				this.index = index;
				this.id = id;
			}
		}

		private XmlDocument document;

		private XmlReader reader;

		private ArrayList fixups;

		private Hashtable collFixups;

		private ArrayList collItemFixups;

		private Hashtable typesCallbacks;

		private ArrayList noIDTargets;

		private Hashtable targets;

		private Hashtable delayedListFixups;

		private XmlSerializer eventSource;

		private int delayedFixupId;

		private Hashtable referencedObjects;

		private int readCount;

		private int whileIterationCount;

		private string w3SchemaNS;

		private string w3InstanceNS;

		private string w3InstanceNS2000;

		private string w3InstanceNS1999;

		private string soapNS;

		private string wsdlNS;

		private string nullX;

		private string nil;

		private string typeX;

		private string arrayType;

		private XmlQualifiedName arrayQName;

		protected XmlDocument Document
		{
			get
			{
				if (document == null)
				{
					document = new XmlDocument(reader.NameTable);
				}
				return document;
			}
		}

		protected XmlReader Reader
		{
			get
			{
				return reader;
			}
		}

		[System.MonoTODO]
		protected bool IsReturnValue
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

		protected int ReaderCount
		{
			get
			{
				return readCount;
			}
		}

		[System.MonoTODO]
		protected bool DecodeName
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

		internal void Initialize(XmlReader reader, XmlSerializer eventSource)
		{
			w3SchemaNS = reader.NameTable.Add("http://www.w3.org/2001/XMLSchema");
			w3InstanceNS = reader.NameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
			w3InstanceNS2000 = reader.NameTable.Add("http://www.w3.org/2000/10/XMLSchema-instance");
			w3InstanceNS1999 = reader.NameTable.Add("http://www.w3.org/1999/XMLSchema-instance");
			soapNS = reader.NameTable.Add("http://schemas.xmlsoap.org/soap/encoding/");
			wsdlNS = reader.NameTable.Add("http://schemas.xmlsoap.org/wsdl/");
			nullX = reader.NameTable.Add("null");
			nil = reader.NameTable.Add("nil");
			typeX = reader.NameTable.Add("type");
			arrayType = reader.NameTable.Add("arrayType");
			this.reader = reader;
			this.eventSource = eventSource;
			arrayQName = new XmlQualifiedName("Array", soapNS);
			InitIDs();
		}

		private ArrayList EnsureArrayList(ArrayList list)
		{
			if (list == null)
			{
				list = new ArrayList();
			}
			return list;
		}

		private Hashtable EnsureHashtable(Hashtable hash)
		{
			if (hash == null)
			{
				hash = new Hashtable();
			}
			return hash;
		}

		protected void AddFixup(CollectionFixup fixup)
		{
			collFixups = EnsureHashtable(collFixups);
			collFixups[fixup.Id] = fixup;
			if (delayedListFixups != null && delayedListFixups.ContainsKey(fixup.Id))
			{
				fixup.CollectionItems = delayedListFixups[fixup.Id];
				delayedListFixups.Remove(fixup.Id);
			}
		}

		protected void AddFixup(Fixup fixup)
		{
			fixups = EnsureArrayList(fixups);
			fixups.Add(fixup);
		}

		private void AddFixup(CollectionItemFixup fixup)
		{
			collItemFixups = EnsureArrayList(collItemFixups);
			collItemFixups.Add(fixup);
		}

		protected void AddReadCallback(string name, string ns, Type type, XmlSerializationReadCallback read)
		{
			WriteCallbackInfo writeCallbackInfo = new WriteCallbackInfo();
			writeCallbackInfo.Type = type;
			writeCallbackInfo.TypeName = name;
			writeCallbackInfo.TypeNs = ns;
			writeCallbackInfo.Callback = read;
			typesCallbacks = EnsureHashtable(typesCallbacks);
			typesCallbacks.Add(new XmlQualifiedName(name, ns), writeCallbackInfo);
		}

		protected void AddTarget(string id, object o)
		{
			if (id != null)
			{
				targets = EnsureHashtable(targets);
				if (targets[id] == null)
				{
					targets.Add(id, o);
				}
			}
			else if (o == null)
			{
				noIDTargets = EnsureArrayList(noIDTargets);
				noIDTargets.Add(o);
			}
		}

		private string CurrentTag()
		{
			switch (reader.NodeType)
			{
			case XmlNodeType.Element:
				return string.Format("<{0} xmlns='{1}'>", reader.LocalName, reader.NamespaceURI);
			case XmlNodeType.Attribute:
				return reader.Value;
			case XmlNodeType.Text:
				return "CDATA";
			case XmlNodeType.ProcessingInstruction:
				return "<--";
			case XmlNodeType.Entity:
				return "<?";
			case XmlNodeType.EndElement:
				return ">";
			default:
				return "(unknown)";
			}
		}

		protected Exception CreateCtorHasSecurityException(string typeName)
		{
			string message = string.Format("The type '{0}' cannot be serialized because its parameterless constructor is decorated with declarative security permission attributes. Consider using imperative asserts or demands in the constructor.", typeName);
			return new InvalidOperationException(message);
		}

		protected Exception CreateInaccessibleConstructorException(string typeName)
		{
			string message = string.Format("{0} cannot be serialized because it does not have a default public constructor.", typeName);
			return new InvalidOperationException(message);
		}

		protected Exception CreateAbstractTypeException(string name, string ns)
		{
			string message = "The specified type is abstrace: name='" + name + "' namespace='" + ns + "', at " + CurrentTag();
			return new InvalidOperationException(message);
		}

		protected Exception CreateInvalidCastException(Type type, object value)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "Cannot assign object of type {0} to an object of type {1}.", value.GetType(), type);
			return new InvalidCastException(message);
		}

		protected Exception CreateReadOnlyCollectionException(string name)
		{
			string message = string.Format("Could not serialize {0}. Default constructors are required for collections and enumerators.", name);
			return new InvalidOperationException(message);
		}

		protected Exception CreateUnknownConstantException(string value, Type enumType)
		{
			string message = string.Format("'{0}' is not a valid value for {1}.", value, enumType);
			return new InvalidOperationException(message);
		}

		protected Exception CreateUnknownNodeException()
		{
			string message = CurrentTag() + " was not expected";
			return new InvalidOperationException(message);
		}

		protected Exception CreateUnknownTypeException(XmlQualifiedName type)
		{
			string message = "The specified type was not recognized: name='" + type.Name + "' namespace='" + type.Namespace + "', at " + CurrentTag();
			return new InvalidOperationException(message);
		}

		protected void CheckReaderCount(ref int whileIterations, ref int readerCount)
		{
			whileIterations = whileIterationCount;
			readerCount = readCount;
		}

		protected Array EnsureArrayIndex(Array a, int index, Type elementType)
		{
			if (a != null && index < a.Length)
			{
				return a;
			}
			int length = ((a != null) ? (a.Length * 2) : 32);
			Array array = Array.CreateInstance(elementType, length);
			if (a != null)
			{
				Array.Copy(a, array, index);
			}
			return array;
		}

		[System.MonoTODO]
		protected void FixupArrayRefs(object fixup)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected int GetArrayLength(string name, string ns)
		{
			throw new NotImplementedException();
		}

		protected bool GetNullAttr()
		{
			string attribute = reader.GetAttribute(nullX, w3InstanceNS);
			if (attribute == null)
			{
				attribute = reader.GetAttribute(nil, w3InstanceNS);
				if (attribute == null)
				{
					attribute = reader.GetAttribute(nullX, w3InstanceNS2000);
					if (attribute == null)
					{
						attribute = reader.GetAttribute(nullX, w3InstanceNS1999);
					}
				}
			}
			return attribute != null;
		}

		protected object GetTarget(string id)
		{
			if (targets == null)
			{
				return null;
			}
			object obj = targets[id];
			if (obj != null)
			{
				if (referencedObjects == null)
				{
					referencedObjects = new Hashtable();
				}
				referencedObjects[obj] = obj;
			}
			return obj;
		}

		private bool TargetReady(string id)
		{
			if (targets == null)
			{
				return false;
			}
			return targets.ContainsKey(id);
		}

		protected XmlQualifiedName GetXsiType()
		{
			string attribute = Reader.GetAttribute(typeX, "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute == string.Empty || attribute == null)
			{
				attribute = Reader.GetAttribute(typeX, w3InstanceNS1999);
				if (attribute == string.Empty || attribute == null)
				{
					attribute = Reader.GetAttribute(typeX, w3InstanceNS2000);
					if (attribute == string.Empty || attribute == null)
					{
						return null;
					}
				}
			}
			int num = attribute.IndexOf(":");
			if (num == -1)
			{
				return new XmlQualifiedName(attribute, Reader.NamespaceURI);
			}
			string prefix = attribute.Substring(0, num);
			string name = attribute.Substring(num + 1);
			return new XmlQualifiedName(name, Reader.LookupNamespace(prefix));
		}

		protected abstract void InitCallbacks();

		protected abstract void InitIDs();

		protected bool IsXmlnsAttribute(string name)
		{
			int length = name.Length;
			if (length < 5)
			{
				return false;
			}
			if (length == 5)
			{
				return name == "xmlns";
			}
			return name.StartsWith("xmlns:");
		}

		protected void ParseWsdlArrayType(XmlAttribute attr)
		{
			if (attr.NamespaceURI == wsdlNS && attr.LocalName == arrayType)
			{
				string ns = string.Empty;
				string type;
				string dimensions;
				TypeTranslator.ParseArrayType(attr.Value, out type, out ns, out dimensions);
				if (ns != string.Empty)
				{
					ns = Reader.LookupNamespace(ns) + ":";
				}
				attr.Value = ns + type + dimensions;
			}
		}

		protected XmlQualifiedName ReadElementQualifiedName()
		{
			readCount++;
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return ToXmlQualifiedName(string.Empty);
			}
			reader.ReadStartElement();
			XmlQualifiedName result = ToXmlQualifiedName(reader.ReadString());
			reader.ReadEndElement();
			return result;
		}

		protected void ReadEndElement()
		{
			readCount++;
			while (reader.NodeType == XmlNodeType.Whitespace)
			{
				reader.Skip();
			}
			if (reader.NodeType != XmlNodeType.None)
			{
				reader.ReadEndElement();
			}
			else
			{
				reader.Skip();
			}
		}

		protected bool ReadNull()
		{
			if (!GetNullAttr())
			{
				return false;
			}
			readCount++;
			if (reader.IsEmptyElement)
			{
				reader.Skip();
				return true;
			}
			reader.ReadStartElement();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				UnknownNode(null);
			}
			ReadEndElement();
			return true;
		}

		protected XmlQualifiedName ReadNullableQualifiedName()
		{
			if (ReadNull())
			{
				return null;
			}
			return ReadElementQualifiedName();
		}

		protected string ReadNullableString()
		{
			if (ReadNull())
			{
				return null;
			}
			readCount++;
			return reader.ReadElementString();
		}

		protected bool ReadReference(out string fixupReference)
		{
			string attribute = reader.GetAttribute("href");
			if (attribute == null)
			{
				fixupReference = null;
				return false;
			}
			if (attribute[0] != '#')
			{
				throw new InvalidOperationException("href not found: " + attribute);
			}
			fixupReference = attribute.Substring(1);
			readCount++;
			if (!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				ReadEndElement();
			}
			else
			{
				reader.Skip();
			}
			return true;
		}

		protected object ReadReferencedElement()
		{
			return ReadReferencedElement(Reader.LocalName, Reader.NamespaceURI);
		}

		private WriteCallbackInfo GetCallbackInfo(XmlQualifiedName qname)
		{
			if (typesCallbacks == null)
			{
				typesCallbacks = new Hashtable();
				InitCallbacks();
			}
			return (WriteCallbackInfo)typesCallbacks[qname];
		}

		protected object ReadReferencedElement(string name, string ns)
		{
			XmlQualifiedName xmlQualifiedName = GetXsiType();
			if (xmlQualifiedName == null)
			{
				xmlQualifiedName = new XmlQualifiedName(name, ns);
			}
			string attribute = Reader.GetAttribute("id");
			string attribute2 = Reader.GetAttribute(arrayType, soapNS);
			object resultList;
			if (xmlQualifiedName == arrayQName || (attribute2 != null && attribute2.Length > 0))
			{
				CollectionFixup collectionFixup = ((collFixups == null) ? null : ((CollectionFixup)collFixups[attribute]));
				if (ReadList(out resultList))
				{
					if (collectionFixup != null)
					{
						collectionFixup.Callback(collectionFixup.Collection, resultList);
						collFixups.Remove(attribute);
						resultList = collectionFixup.Collection;
					}
				}
				else if (collectionFixup != null)
				{
					collectionFixup.CollectionItems = (object[])resultList;
					resultList = collectionFixup.Collection;
				}
			}
			else
			{
				WriteCallbackInfo callbackInfo = GetCallbackInfo(xmlQualifiedName);
				resultList = ((callbackInfo != null) ? callbackInfo.Callback() : ReadTypedPrimitive(xmlQualifiedName, attribute != null));
			}
			AddTarget(attribute, resultList);
			return resultList;
		}

		private bool ReadList(out object resultList)
		{
			string attribute = Reader.GetAttribute(arrayType, soapNS);
			if (attribute == null)
			{
				attribute = Reader.GetAttribute(arrayType, wsdlNS);
			}
			XmlQualifiedName xmlQualifiedName = ToXmlQualifiedName(attribute);
			int num = xmlQualifiedName.Name.LastIndexOf('[');
			string text = xmlQualifiedName.Name.Substring(num);
			string text2 = xmlQualifiedName.Name.Substring(0, num);
			int num2 = int.Parse(text.Substring(1, text.Length - 2), CultureInfo.InvariantCulture);
			num = text2.IndexOf('[');
			if (num == -1)
			{
				num = text2.Length;
			}
			string text3 = text2.Substring(0, num);
			string typeName;
			if (xmlQualifiedName.Namespace == w3SchemaNS)
			{
				typeName = TypeTranslator.GetPrimitiveTypeData(text3).Type.FullName + text2.Substring(num);
			}
			else
			{
				WriteCallbackInfo callbackInfo = GetCallbackInfo(new XmlQualifiedName(text3, xmlQualifiedName.Namespace));
				typeName = callbackInfo.Type.FullName + text2.Substring(num) + ", " + callbackInfo.Type.Assembly.FullName;
			}
			Array array = Array.CreateInstance(Type.GetType(typeName), num2);
			bool result = true;
			if (Reader.IsEmptyElement)
			{
				readCount++;
				Reader.Skip();
			}
			else
			{
				Reader.ReadStartElement();
				for (int i = 0; i < num2; i++)
				{
					whileIterationCount++;
					readCount++;
					Reader.MoveToContent();
					string fixupReference;
					object value = ReadReferencingElement(text2, xmlQualifiedName.Namespace, out fixupReference);
					if (fixupReference == null)
					{
						array.SetValue(value, i);
						continue;
					}
					AddFixup(new CollectionItemFixup(array, i, fixupReference));
					result = false;
				}
				whileIterationCount = 0;
				Reader.ReadEndElement();
			}
			resultList = array;
			return result;
		}

		protected void ReadReferencedElements()
		{
			reader.MoveToContent();
			XmlNodeType nodeType = reader.NodeType;
			while (nodeType != XmlNodeType.EndElement && nodeType != XmlNodeType.None)
			{
				whileIterationCount++;
				readCount++;
				ReadReferencedElement();
				reader.MoveToContent();
				nodeType = reader.NodeType;
			}
			whileIterationCount = 0;
			if (delayedListFixups != null)
			{
				foreach (DictionaryEntry delayedListFixup in delayedListFixups)
				{
					AddTarget((string)delayedListFixup.Key, delayedListFixup.Value);
				}
			}
			if (collItemFixups != null)
			{
				foreach (CollectionItemFixup collItemFixup in collItemFixups)
				{
					collItemFixup.Collection.SetValue(GetTarget(collItemFixup.Id), collItemFixup.Index);
				}
			}
			if (collFixups != null)
			{
				ICollection values = collFixups.Values;
				foreach (CollectionFixup item in values)
				{
					item.Callback(item.Collection, item.CollectionItems);
				}
			}
			if (fixups != null)
			{
				foreach (Fixup fixup in fixups)
				{
					fixup.Callback(fixup);
				}
			}
			if (targets == null)
			{
				return;
			}
			foreach (DictionaryEntry target in targets)
			{
				if (target.Value != null && (referencedObjects == null || !referencedObjects.Contains(target.Value)))
				{
					UnreferencedObject((string)target.Key, target.Value);
				}
			}
		}

		protected object ReadReferencingElement(out string fixupReference)
		{
			return ReadReferencingElement(Reader.LocalName, Reader.NamespaceURI, false, out fixupReference);
		}

		protected object ReadReferencingElement(string name, string ns, out string fixupReference)
		{
			return ReadReferencingElement(name, ns, false, out fixupReference);
		}

		protected object ReadReferencingElement(string name, string ns, bool elementCanBeType, out string fixupReference)
		{
			if (ReadNull())
			{
				fixupReference = null;
				return null;
			}
			string text = Reader.GetAttribute("href");
			if (text == string.Empty || text == null)
			{
				fixupReference = null;
				XmlQualifiedName xmlQualifiedName = GetXsiType();
				if (xmlQualifiedName == null)
				{
					xmlQualifiedName = new XmlQualifiedName(name, ns);
				}
				string attribute = Reader.GetAttribute(arrayType, soapNS);
				if (xmlQualifiedName == arrayQName || attribute != null)
				{
					delayedListFixups = EnsureHashtable(delayedListFixups);
					fixupReference = "__<" + delayedFixupId++ + ">";
					object resultList;
					ReadList(out resultList);
					delayedListFixups[fixupReference] = resultList;
					return null;
				}
				WriteCallbackInfo callbackInfo = GetCallbackInfo(xmlQualifiedName);
				if (callbackInfo == null)
				{
					return ReadTypedPrimitive(xmlQualifiedName, true);
				}
				return callbackInfo.Callback();
			}
			if (text.StartsWith("#"))
			{
				text = text.Substring(1);
			}
			readCount++;
			Reader.Skip();
			if (TargetReady(text))
			{
				fixupReference = null;
				return GetTarget(text);
			}
			fixupReference = text;
			return null;
		}

		protected IXmlSerializable ReadSerializable(IXmlSerializable serializable)
		{
			if (ReadNull())
			{
				return null;
			}
			int depth = reader.Depth;
			readCount++;
			serializable.ReadXml(reader);
			Reader.MoveToContent();
			while (reader.Depth > depth)
			{
				reader.Skip();
			}
			if (reader.Depth == depth && reader.NodeType == XmlNodeType.EndElement)
			{
				reader.ReadEndElement();
			}
			return serializable;
		}

		protected string ReadString(string value)
		{
			readCount++;
			if (value == null || value == string.Empty)
			{
				return reader.ReadString();
			}
			return value + reader.ReadString();
		}

		protected object ReadTypedPrimitive(XmlQualifiedName qname)
		{
			return ReadTypedPrimitive(qname, false);
		}

		private object ReadTypedPrimitive(XmlQualifiedName qname, bool reportUnknown)
		{
			if (qname == null)
			{
				qname = GetXsiType();
			}
			TypeData typeData = TypeTranslator.FindPrimitiveTypeData(qname.Name);
			if (typeData == null || typeData.SchemaType != SchemaTypes.Primitive)
			{
				readCount++;
				XmlNode xmlNode = Document.ReadNode(reader);
				if (reportUnknown)
				{
					OnUnknownNode(xmlNode, null, null);
				}
				if (xmlNode.ChildNodes.Count == 0 && xmlNode.Attributes.Count == 0)
				{
					return new object();
				}
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement == null)
				{
					return new XmlNode[1] { xmlNode };
				}
				XmlNode[] array = new XmlNode[xmlElement.Attributes.Count + xmlElement.ChildNodes.Count];
				int num = 0;
				foreach (XmlNode attribute in xmlElement.Attributes)
				{
					array[num++] = attribute;
				}
				{
					foreach (XmlNode childNode in xmlElement.ChildNodes)
					{
						array[num++] = childNode;
					}
					return array;
				}
			}
			if (typeData.Type == typeof(XmlQualifiedName))
			{
				return ReadNullableQualifiedName();
			}
			readCount++;
			return XmlCustomFormatter.FromXmlString(typeData, Reader.ReadElementString());
		}

		protected XmlNode ReadXmlNode(bool wrapped)
		{
			readCount++;
			XmlNode xmlNode = Document.ReadNode(reader);
			if (wrapped)
			{
				return xmlNode.FirstChild;
			}
			return xmlNode;
		}

		protected XmlDocument ReadXmlDocument(bool wrapped)
		{
			readCount++;
			if (wrapped)
			{
				reader.ReadStartElement();
			}
			reader.MoveToContent();
			XmlDocument xmlDocument = new XmlDocument();
			XmlNode newChild = xmlDocument.ReadNode(reader);
			xmlDocument.AppendChild(newChild);
			if (wrapped)
			{
				reader.ReadEndElement();
			}
			return xmlDocument;
		}

		protected void Referenced(object o)
		{
			if (o != null)
			{
				if (referencedObjects == null)
				{
					referencedObjects = new Hashtable();
				}
				referencedObjects[o] = o;
			}
		}

		protected Array ShrinkArray(Array a, int length, Type elementType, bool isNullable)
		{
			if (length == 0 && isNullable)
			{
				return null;
			}
			if (a == null)
			{
				return Array.CreateInstance(elementType, length);
			}
			if (a.Length == length)
			{
				return a;
			}
			Array array = Array.CreateInstance(elementType, length);
			Array.Copy(a, array, length);
			return array;
		}

		protected byte[] ToByteArrayBase64(bool isNull)
		{
			readCount++;
			if (isNull)
			{
				Reader.ReadString();
				return null;
			}
			return ToByteArrayBase64(Reader.ReadString());
		}

		protected static byte[] ToByteArrayBase64(string value)
		{
			return Convert.FromBase64String(value);
		}

		protected byte[] ToByteArrayHex(bool isNull)
		{
			readCount++;
			if (isNull)
			{
				Reader.ReadString();
				return null;
			}
			return ToByteArrayHex(Reader.ReadString());
		}

		protected static byte[] ToByteArrayHex(string value)
		{
			return XmlConvert.FromBinHexString(value);
		}

		protected static char ToChar(string value)
		{
			return XmlCustomFormatter.ToChar(value);
		}

		protected static DateTime ToDate(string value)
		{
			return XmlCustomFormatter.ToDate(value);
		}

		protected static DateTime ToDateTime(string value)
		{
			return XmlCustomFormatter.ToDateTime(value);
		}

		protected static long ToEnum(string value, Hashtable h, string typeName)
		{
			return XmlCustomFormatter.ToEnum(value, h, typeName, true);
		}

		protected static DateTime ToTime(string value)
		{
			return XmlCustomFormatter.ToTime(value);
		}

		protected static string ToXmlName(string value)
		{
			return XmlCustomFormatter.ToXmlName(value);
		}

		protected static string ToXmlNCName(string value)
		{
			return XmlCustomFormatter.ToXmlNCName(value);
		}

		protected static string ToXmlNmToken(string value)
		{
			return XmlCustomFormatter.ToXmlNmToken(value);
		}

		protected static string ToXmlNmTokens(string value)
		{
			return XmlCustomFormatter.ToXmlNmTokens(value);
		}

		protected XmlQualifiedName ToXmlQualifiedName(string value)
		{
			int num = value.LastIndexOf(':');
			string name = XmlConvert.DecodeName(value);
			string name2;
			string text;
			if (num < 0)
			{
				name2 = reader.NameTable.Add(name);
				text = reader.LookupNamespace(string.Empty);
			}
			else
			{
				string text2 = value.Substring(0, num);
				text = reader.LookupNamespace(text2);
				if (text == null)
				{
					throw new InvalidOperationException("namespace " + text2 + " not defined");
				}
				name2 = reader.NameTable.Add(value.Substring(num + 1));
			}
			return new XmlQualifiedName(name2, text);
		}

		protected void UnknownAttribute(object o, XmlAttribute attr)
		{
			UnknownAttribute(o, attr, null);
		}

		protected void UnknownAttribute(object o, XmlAttribute attr, string qnames)
		{
			int lineNum;
			int linePos;
			if (Reader is XmlTextReader)
			{
				lineNum = ((XmlTextReader)Reader).LineNumber;
				linePos = ((XmlTextReader)Reader).LinePosition;
			}
			else
			{
				lineNum = 0;
				linePos = 0;
			}
			XmlAttributeEventArgs e = new XmlAttributeEventArgs(attr, lineNum, linePos, o);
			e.ExpectedAttributes = qnames;
			if (eventSource != null)
			{
				eventSource.OnUnknownAttribute(e);
			}
		}

		protected void UnknownElement(object o, XmlElement elem)
		{
			UnknownElement(o, elem, null);
		}

		protected void UnknownElement(object o, XmlElement elem, string qnames)
		{
			int lineNum;
			int linePos;
			if (Reader is XmlTextReader)
			{
				lineNum = ((XmlTextReader)Reader).LineNumber;
				linePos = ((XmlTextReader)Reader).LinePosition;
			}
			else
			{
				lineNum = 0;
				linePos = 0;
			}
			XmlElementEventArgs e = new XmlElementEventArgs(elem, lineNum, linePos, o);
			e.ExpectedElements = qnames;
			if (eventSource != null)
			{
				eventSource.OnUnknownElement(e);
			}
		}

		protected void UnknownNode(object o)
		{
			UnknownNode(o, null);
		}

		protected void UnknownNode(object o, string qnames)
		{
			OnUnknownNode(ReadXmlNode(false), o, qnames);
		}

		private void OnUnknownNode(XmlNode node, object o, string qnames)
		{
			int linenumber;
			int lineposition;
			if (Reader is XmlTextReader)
			{
				linenumber = ((XmlTextReader)Reader).LineNumber;
				lineposition = ((XmlTextReader)Reader).LinePosition;
			}
			else
			{
				linenumber = 0;
				lineposition = 0;
			}
			if (node is XmlAttribute)
			{
				UnknownAttribute(o, (XmlAttribute)node, qnames);
				return;
			}
			if (node is XmlElement)
			{
				UnknownElement(o, (XmlElement)node, qnames);
				return;
			}
			if (eventSource != null)
			{
				eventSource.OnUnknownNode(new XmlNodeEventArgs(linenumber, lineposition, node.LocalName, node.Name, node.NamespaceURI, node.NodeType, o, node.Value));
			}
			if (Reader.ReadState != ReadState.EndOfFile)
			{
				return;
			}
			throw new InvalidOperationException("End of document found");
		}

		protected void UnreferencedObject(string id, object o)
		{
			if (eventSource != null)
			{
				eventSource.OnUnreferencedObject(new UnreferencedObjectEventArgs(o, id));
			}
		}

		[System.MonoTODO]
		protected string CollapseWhitespace(string value)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected Exception CreateBadDerivationException(string xsdDerived, string nsDerived, string xsdBase, string nsBase, string clrDerived, string clrBase)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected Exception CreateInvalidCastException(Type type, object value, string id)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected Exception CreateMissingIXmlSerializableType(string name, string ns, string clrType)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected string ReadString(string value, bool trim)
		{
			throw new NotImplementedException();
		}

		[System.MonoTODO]
		protected object ReadTypedNull(XmlQualifiedName type)
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
