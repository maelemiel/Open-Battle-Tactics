using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Schema;

namespace System.Xml.Serialization
{
	internal class SerializationCodeGenerator
	{
		private class HookInfo
		{
			public HookType HookType;

			public Type Type;

			public string Member;

			public XmlMappingAccess Direction;
		}

		private XmlMapping _typeMap;

		private SerializationFormat _format;

		private TextWriter _writer;

		private int _tempVarId;

		private int _indent;

		private Hashtable _uniqueNames = new Hashtable();

		private int _methodId;

		private SerializerInfo _config;

		private ArrayList _mapsToGenerate = new ArrayList();

		private ArrayList _fixupCallbacks;

		private ArrayList _referencedTypes = new ArrayList();

		private GenerationResult[] _results;

		private GenerationResult _result;

		private XmlMapping[] _xmlMaps;

		private CodeIdentifiers classNames = new CodeIdentifiers();

		private ArrayList _listsToFill = new ArrayList();

		private Hashtable _hookVariables;

		private Stack _hookContexts;

		private Stack _hookOpenHooks;

		public GenerationResult[] GenerationResults
		{
			get
			{
				return _results;
			}
		}

		public ArrayList ReferencedTypes
		{
			get
			{
				return _referencedTypes;
			}
		}

		public SerializationCodeGenerator(XmlMapping[] xmlMaps)
			: this(xmlMaps, null)
		{
		}

		public SerializationCodeGenerator(XmlMapping[] xmlMaps, SerializerInfo config)
		{
			_xmlMaps = xmlMaps;
			_config = config;
		}

		public SerializationCodeGenerator(XmlMapping xmlMap, SerializerInfo config)
		{
			_xmlMaps = new XmlMapping[1] { xmlMap };
			_config = config;
		}

		public static void Generate(string configFileName, string outputPath)
		{
			SerializationCodeGeneratorConfiguration serializationCodeGeneratorConfiguration = null;
			StreamReader streamReader = new StreamReader(configFileName);
			try
			{
				XmlReflectionImporter xmlReflectionImporter = new XmlReflectionImporter();
				xmlReflectionImporter.AllowPrivateTypes = true;
				XmlSerializer xmlSerializer = new XmlSerializer(xmlReflectionImporter.ImportTypeMapping(typeof(SerializationCodeGeneratorConfiguration)));
				serializationCodeGeneratorConfiguration = (SerializationCodeGeneratorConfiguration)xmlSerializer.Deserialize(streamReader);
			}
			finally
			{
				streamReader.Close();
			}
			if (outputPath == null)
			{
				outputPath = string.Empty;
			}
			CodeIdentifiers codeIdentifiers = new CodeIdentifiers();
			if (serializationCodeGeneratorConfiguration.Serializers == null)
			{
				return;
			}
			SerializerInfo[] serializers = serializationCodeGeneratorConfiguration.Serializers;
			foreach (SerializerInfo serializerInfo in serializers)
			{
				Type type;
				if (serializerInfo.Assembly != null)
				{
					Assembly assembly;
					try
					{
						assembly = Assembly.Load(serializerInfo.Assembly);
					}
					catch
					{
						assembly = Assembly.LoadFrom(serializerInfo.Assembly);
					}
					type = assembly.GetType(serializerInfo.ClassName, true);
				}
				else
				{
					type = Type.GetType(serializerInfo.ClassName);
				}
				if (type == null)
				{
					throw new InvalidOperationException("Type " + serializerInfo.ClassName + " not found");
				}
				string text = serializerInfo.OutFileName;
				if (text == null || text.Length == 0)
				{
					int num = serializerInfo.ClassName.LastIndexOf(".");
					text = ((num == -1) ? serializerInfo.ClassName : serializerInfo.ClassName.Substring(num + 1));
					text = codeIdentifiers.AddUnique(text, type) + "Serializer.cs";
				}
				StreamWriter streamWriter = new StreamWriter(Path.Combine(outputPath, text));
				try
				{
					XmlTypeMapping xmlMap;
					if (serializerInfo.SerializationFormat == SerializationFormat.Literal)
					{
						XmlReflectionImporter xmlReflectionImporter2 = new XmlReflectionImporter();
						xmlMap = xmlReflectionImporter2.ImportTypeMapping(type);
					}
					else
					{
						SoapReflectionImporter soapReflectionImporter = new SoapReflectionImporter();
						xmlMap = soapReflectionImporter.ImportTypeMapping(type);
					}
					SerializationCodeGenerator serializationCodeGenerator = new SerializationCodeGenerator(xmlMap, serializerInfo);
					serializationCodeGenerator.GenerateSerializers(streamWriter);
				}
				finally
				{
					streamWriter.Close();
				}
			}
		}

		public void GenerateSerializers(TextWriter writer)
		{
			_writer = writer;
			_results = new GenerationResult[_xmlMaps.Length];
			WriteLine("// It is automatically generated");
			WriteLine("using System;");
			WriteLine("using System.Xml;");
			WriteLine("using System.Xml.Schema;");
			WriteLine("using System.Xml.Serialization;");
			WriteLine("using System.Text;");
			WriteLine("using System.Collections;");
			WriteLine("using System.Globalization;");
			if (_config != null && _config.NamespaceImports != null && _config.NamespaceImports.Length > 0)
			{
				string[] namespaceImports = _config.NamespaceImports;
				foreach (string text in namespaceImports)
				{
					WriteLine("using " + text + ";");
				}
			}
			WriteLine(string.Empty);
			string text2 = null;
			string text3 = null;
			string text4 = null;
			string text5 = null;
			string text6 = null;
			if (_config != null)
			{
				text2 = _config.ReaderClassName;
				text3 = _config.WriterClassName;
				text4 = _config.BaseSerializerClassName;
				text5 = _config.ImplementationClassName;
				text6 = _config.Namespace;
			}
			if (text2 == null || text2.Length == 0)
			{
				text2 = "GeneratedReader";
			}
			if (text3 == null || text3.Length == 0)
			{
				text3 = "GeneratedWriter";
			}
			if (text4 == null || text4.Length == 0)
			{
				text4 = "BaseXmlSerializer";
			}
			if (text5 == null || text5.Length == 0)
			{
				text5 = "XmlSerializerContract";
			}
			text2 = GetUniqueClassName(text2);
			text3 = GetUniqueClassName(text3);
			text4 = GetUniqueClassName(text4);
			text5 = GetUniqueClassName(text5);
			Hashtable hashtable = new Hashtable();
			Hashtable hashtable2 = new Hashtable();
			for (int j = 0; j < _xmlMaps.Length; j++)
			{
				_typeMap = _xmlMaps[j];
				if (_typeMap == null)
				{
					continue;
				}
				_result = hashtable2[_typeMap] as GenerationResult;
				if (_result != null)
				{
					_results[j] = _result;
					continue;
				}
				_result = new GenerationResult();
				_results[j] = _result;
				hashtable2[_typeMap] = _result;
				string text7 = ((!(_typeMap is XmlTypeMapping)) ? ((XmlMembersMapping)_typeMap).ElementName : CodeIdentifier.MakeValid(((XmlTypeMapping)_typeMap).TypeData.CSharpName));
				_result.ReaderClassName = text2;
				_result.WriterClassName = text3;
				_result.BaseSerializerClassName = text4;
				_result.ImplementationClassName = text5;
				if (text6 == null || text6.Length == 0)
				{
					_result.Namespace = "Mono.GeneratedSerializers." + _typeMap.Format;
				}
				else
				{
					_result.Namespace = text6;
				}
				_result.WriteMethodName = GetUniqueName("rwo", _typeMap, "WriteRoot_" + text7);
				_result.ReadMethodName = GetUniqueName("rro", _typeMap, "ReadRoot_" + text7);
				_result.Mapping = _typeMap;
				ArrayList arrayList = (ArrayList)hashtable[_result.Namespace];
				if (arrayList == null)
				{
					arrayList = new ArrayList();
					hashtable[_result.Namespace] = arrayList;
				}
				arrayList.Add(_result);
			}
			foreach (DictionaryEntry item in hashtable)
			{
				ArrayList arrayList2 = (ArrayList)item.Value;
				WriteLine("namespace " + item.Key);
				WriteLineInd("{");
				if (_config == null || !_config.NoReader)
				{
					GenerateReader(text2, arrayList2);
				}
				WriteLine(string.Empty);
				if (_config == null || !_config.NoWriter)
				{
					GenerateWriter(text3, arrayList2);
				}
				WriteLine(string.Empty);
				GenerateContract(arrayList2);
				WriteLineUni("}");
				WriteLine(string.Empty);
			}
		}

		private void UpdateGeneratedTypes(ArrayList list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				XmlTypeMapping xmlTypeMapping = list[i] as XmlTypeMapping;
				if (xmlTypeMapping != null && !_referencedTypes.Contains(xmlTypeMapping.TypeData.Type))
				{
					_referencedTypes.Add(xmlTypeMapping.TypeData.Type);
				}
			}
		}

		private static string ToCSharpFullName(Type type)
		{
			return TypeData.ToCSharpName(type, true);
		}

		public void GenerateContract(ArrayList generatedMaps)
		{
			if (generatedMaps.Count == 0)
			{
				return;
			}
			GenerationResult generationResult = (GenerationResult)generatedMaps[0];
			string baseSerializerClassName = generationResult.BaseSerializerClassName;
			string text = ((_config != null && _config.GenerateAsInternal) ? "internal" : "public");
			WriteLine(string.Empty);
			WriteLine(text + " class " + baseSerializerClassName + " : System.Xml.Serialization.XmlSerializer");
			WriteLineInd("{");
			WriteLineInd("protected override System.Xml.Serialization.XmlSerializationReader CreateReader () {");
			WriteLine("return new " + generationResult.ReaderClassName + " ();");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLineInd("protected override System.Xml.Serialization.XmlSerializationWriter CreateWriter () {");
			WriteLine("return new " + generationResult.WriterClassName + " ();");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLineInd("public override bool CanDeserialize (System.Xml.XmlReader xmlReader) {");
			WriteLine("return true;");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
			foreach (GenerationResult generatedMap in generatedMaps)
			{
				generatedMap.SerializerClassName = GetUniqueClassName(generatedMap.Mapping.ElementName + "Serializer");
				WriteLine(text + " sealed class " + generatedMap.SerializerClassName + " : " + baseSerializerClassName);
				WriteLineInd("{");
				WriteLineInd("protected override void Serialize (object obj, System.Xml.Serialization.XmlSerializationWriter writer) {");
				WriteLine("((" + generatedMap.WriterClassName + ")writer)." + generatedMap.WriteMethodName + "(obj);");
				WriteLineUni("}");
				WriteLine(string.Empty);
				WriteLineInd("protected override object Deserialize (System.Xml.Serialization.XmlSerializationReader reader) {");
				WriteLine("return ((" + generatedMap.ReaderClassName + ")reader)." + generatedMap.ReadMethodName + "();");
				WriteLineUni("}");
				WriteLineUni("}");
				WriteLine(string.Empty);
			}
			WriteLine("#if !TARGET_JVM");
			WriteLine(text + " class " + generationResult.ImplementationClassName + " : System.Xml.Serialization.XmlSerializerImplementation");
			WriteLineInd("{");
			WriteLine("System.Collections.Hashtable readMethods = null;");
			WriteLine("System.Collections.Hashtable writeMethods = null;");
			WriteLine("System.Collections.Hashtable typedSerializers = null;");
			WriteLine(string.Empty);
			WriteLineInd("public override System.Xml.Serialization.XmlSerializationReader Reader {");
			WriteLineInd("get {");
			WriteLine("return new " + generationResult.ReaderClassName + "();");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLineInd("public override System.Xml.Serialization.XmlSerializationWriter Writer {");
			WriteLineInd("get {");
			WriteLine("return new " + generationResult.WriterClassName + "();");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLineInd("public override System.Collections.Hashtable ReadMethods {");
			WriteLineInd("get {");
			WriteLineInd("lock (this) {");
			WriteLineInd("if (readMethods == null) {");
			WriteLine("readMethods = new System.Collections.Hashtable ();");
			foreach (GenerationResult generatedMap2 in generatedMaps)
			{
				WriteLine("readMethods.Add (@\"" + generatedMap2.Mapping.GetKey() + "\", @\"" + generatedMap2.ReadMethodName + "\");");
			}
			WriteLineUni("}");
			WriteLine("return readMethods;");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLineInd("public override System.Collections.Hashtable WriteMethods {");
			WriteLineInd("get {");
			WriteLineInd("lock (this) {");
			WriteLineInd("if (writeMethods == null) {");
			WriteLine("writeMethods = new System.Collections.Hashtable ();");
			foreach (GenerationResult generatedMap3 in generatedMaps)
			{
				WriteLine("writeMethods.Add (@\"" + generatedMap3.Mapping.GetKey() + "\", @\"" + generatedMap3.WriteMethodName + "\");");
			}
			WriteLineUni("}");
			WriteLine("return writeMethods;");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLineInd("public override System.Collections.Hashtable TypedSerializers {");
			WriteLineInd("get {");
			WriteLineInd("lock (this) {");
			WriteLineInd("if (typedSerializers == null) {");
			WriteLine("typedSerializers = new System.Collections.Hashtable ();");
			foreach (GenerationResult generatedMap4 in generatedMaps)
			{
				WriteLine("typedSerializers.Add (@\"" + generatedMap4.Mapping.GetKey() + "\", new " + generatedMap4.SerializerClassName + "());");
			}
			WriteLineUni("}");
			WriteLine("return typedSerializers;");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLine("public override XmlSerializer GetSerializer (Type type)");
			WriteLineInd("{");
			WriteLine("switch (type.FullName) {");
			foreach (GenerationResult generatedMap5 in generatedMaps)
			{
				if (generatedMap5.Mapping is XmlTypeMapping)
				{
					WriteLineInd("case \"" + ((XmlTypeMapping)generatedMap5.Mapping).TypeData.CSharpFullName + "\":");
					WriteLine("return (XmlSerializer) TypedSerializers [\"" + generatedMap5.Mapping.GetKey() + "\"];");
					WriteLineUni(string.Empty);
				}
			}
			WriteLine("}");
			WriteLine("return base.GetSerializer (type);");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLineInd("public override bool CanSerialize (System.Type type) {");
			foreach (GenerationResult generatedMap6 in generatedMaps)
			{
				if (generatedMap6.Mapping is XmlTypeMapping)
				{
					WriteLine("if (type == typeof(" + (generatedMap6.Mapping as XmlTypeMapping).TypeData.CSharpFullName + ")) return true;");
				}
			}
			WriteLine("return false;");
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLine("#endif");
		}

		public void GenerateWriter(string writerClassName, ArrayList maps)
		{
			_mapsToGenerate = new ArrayList();
			InitHooks();
			if (_config == null || !_config.GenerateAsInternal)
			{
				WriteLine("public class " + writerClassName + " : XmlSerializationWriter");
			}
			else
			{
				WriteLine("internal class " + writerClassName + " : XmlSerializationWriter");
			}
			WriteLineInd("{");
			WriteLine("const string xmlNamespace = \"http://www.w3.org/2000/xmlns/\";");
			WriteLine("static readonly System.Reflection.MethodInfo toBinHexStringMethod = typeof (XmlConvert).GetMethod (\"ToBinHexString\", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new Type [] {typeof (byte [])}, null);");
			WriteLine("static string ToBinHexString (byte [] input)");
			WriteLineInd("{");
			WriteLine("return input == null ? null : (string) toBinHexStringMethod.Invoke (null, new object [] {input});");
			WriteLineUni("}");
			for (int i = 0; i < maps.Count; i++)
			{
				GenerationResult generationResult = (GenerationResult)maps[i];
				_typeMap = generationResult.Mapping;
				_format = _typeMap.Format;
				_result = generationResult;
				GenerateWriteRoot();
			}
			for (int j = 0; j < _mapsToGenerate.Count; j++)
			{
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)_mapsToGenerate[j];
				GenerateWriteObject(xmlTypeMapping);
				if (xmlTypeMapping.TypeData.SchemaType == SchemaTypes.Enum)
				{
					GenerateGetXmlEnumValue(xmlTypeMapping);
				}
			}
			GenerateWriteInitCallbacks();
			UpdateGeneratedTypes(_mapsToGenerate);
			WriteLineUni("}");
		}

		private void GenerateWriteRoot()
		{
			WriteLine("public void " + _result.WriteMethodName + " (object o)");
			WriteLineInd("{");
			WriteLine("WriteStartDocument ();");
			if (_typeMap is XmlTypeMapping)
			{
				WriteLine(GetRootTypeName() + " ob = (" + GetRootTypeName() + ") o;");
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)_typeMap;
				if (xmlTypeMapping.TypeData.SchemaType == SchemaTypes.Class || xmlTypeMapping.TypeData.SchemaType == SchemaTypes.Array)
				{
					WriteLine("TopLevelElement ();");
				}
				if (_format == SerializationFormat.Literal)
				{
					WriteLine(GetWriteObjectName(xmlTypeMapping) + " (ob, " + GetLiteral(xmlTypeMapping.ElementName) + ", " + GetLiteral(xmlTypeMapping.Namespace) + ", true, false, true);");
				}
				else
				{
					RegisterReferencingMap(xmlTypeMapping);
					WriteLine("WritePotentiallyReferencingElement (" + GetLiteral(xmlTypeMapping.ElementName) + ", " + GetLiteral(xmlTypeMapping.Namespace) + ", ob, " + GetTypeOf(xmlTypeMapping.TypeData) + ", true, false);");
				}
			}
			else
			{
				if (!(_typeMap is XmlMembersMapping))
				{
					throw new InvalidOperationException("Unknown type map");
				}
				WriteLine("object[] pars = (object[]) o;");
				GenerateWriteMessage((XmlMembersMapping)_typeMap);
			}
			if (_format == SerializationFormat.Encoded)
			{
				WriteLine("WriteReferencedElements ();");
			}
			WriteLineUni("}");
			WriteLine(string.Empty);
		}

		private void GenerateWriteMessage(XmlMembersMapping membersMap)
		{
			if (membersMap.HasWrapperElement)
			{
				WriteLine("TopLevelElement ();");
				WriteLine("WriteStartElement (" + GetLiteral(membersMap.ElementName) + ", " + GetLiteral(membersMap.Namespace) + ", (" + GetLiteral(_format == SerializationFormat.Encoded) + "));");
			}
			GenerateWriteObjectElement(membersMap, "pars", true);
			if (membersMap.HasWrapperElement)
			{
				WriteLine("WriteEndElement();");
			}
		}

		private void GenerateGetXmlEnumValue(XmlTypeMapping map)
		{
			EnumMap enumMap = (EnumMap)map.ObjectMap;
			string text = null;
			string text2 = null;
			if (enumMap.IsFlags)
			{
				text = GetUniqueName("gxe", map, "_xmlNames" + map.XmlType);
				Write("static readonly string[] " + text + " = { ");
				for (int i = 0; i < enumMap.XmlNames.Length; i++)
				{
					if (i > 0)
					{
						_writer.Write(',');
					}
					_writer.Write('"');
					_writer.Write(enumMap.XmlNames[i]);
					_writer.Write('"');
				}
				_writer.WriteLine(" };");
				text2 = GetUniqueName("gve", map, "_values" + map.XmlType);
				Write("static readonly long[] " + text2 + " = { ");
				for (int j = 0; j < enumMap.Values.Length; j++)
				{
					if (j > 0)
					{
						_writer.Write(',');
					}
					_writer.Write(enumMap.Values[j].ToString(CultureInfo.InvariantCulture));
					_writer.Write("L");
				}
				_writer.WriteLine(" };");
				WriteLine(string.Empty);
			}
			WriteLine("string " + GetGetEnumValueName(map) + " (" + map.TypeData.CSharpFullName + " val)");
			WriteLineInd("{");
			WriteLineInd("switch (val) {");
			for (int k = 0; k < enumMap.EnumNames.Length; k++)
			{
				WriteLine("case " + map.TypeData.CSharpFullName + ".@" + enumMap.EnumNames[k] + ": return " + GetLiteral(enumMap.XmlNames[k]) + ";");
			}
			if (enumMap.IsFlags)
			{
				WriteLineInd("default:");
				WriteLine("if (val.ToString () == \"0\") return string.Empty;");
				Write("return FromEnum ((long) val, " + text + ", " + text2);
				_writer.Write(", typeof (");
				_writer.Write(map.TypeData.CSharpFullName);
				_writer.Write(").FullName");
				_writer.Write(')');
				WriteUni(";");
			}
			else
			{
				WriteLine("default: throw CreateInvalidEnumValueException ((long) val, typeof (" + map.TypeData.CSharpFullName + ").FullName);");
			}
			WriteLineUni("}");
			WriteLineUni("}");
			WriteLine(string.Empty);
		}

		private void GenerateWriteObject(XmlTypeMapping typeMap)
		{
			WriteLine("void " + GetWriteObjectName(typeMap) + " (" + typeMap.TypeData.CSharpFullName + " ob, string element, string namesp, bool isNullable, bool needType, bool writeWrappingElem)");
			WriteLineInd("{");
			PushHookContext();
			SetHookVar("$TYPE", typeMap.TypeData.CSharpName);
			SetHookVar("$FULLTYPE", typeMap.TypeData.CSharpFullName);
			SetHookVar("$OBJECT", "ob");
			SetHookVar("$NULLABLE", "isNullable");
			if (GenerateWriteHook(HookType.type, typeMap.TypeData.Type))
			{
				WriteLineUni("}");
				WriteLine(string.Empty);
				PopHookContext();
				return;
			}
			if (!typeMap.TypeData.IsValueType)
			{
				WriteLine("if (((object)ob) == null)");
				WriteLineInd("{");
				WriteLineInd("if (isNullable)");
				if (_format == SerializationFormat.Literal)
				{
					WriteLine("WriteNullTagLiteral(element, namesp);");
				}
				else
				{
					WriteLine("WriteNullTagEncoded (element, namesp);");
				}
				WriteLineUni("return;");
				WriteLineUni("}");
				WriteLine(string.Empty);
			}
			if (typeMap.TypeData.SchemaType == SchemaTypes.XmlNode)
			{
				if (_format == SerializationFormat.Literal)
				{
					WriteLine("WriteElementLiteral (ob, \"\", \"\", true, false);");
				}
				else
				{
					WriteLine("WriteElementEncoded (ob, \"\", \"\", true, false);");
				}
				GenerateEndHook();
				WriteLineUni("}");
				WriteLine(string.Empty);
				PopHookContext();
				return;
			}
			if (typeMap.TypeData.SchemaType == SchemaTypes.XmlSerializable)
			{
				WriteLine("WriteSerializable (ob, element, namesp, isNullable);");
				GenerateEndHook();
				WriteLineUni("}");
				WriteLine(string.Empty);
				PopHookContext();
				return;
			}
			ArrayList derivedTypes = typeMap.DerivedTypes;
			WriteLine("System.Type type = ob.GetType ();");
			WriteLine("if (type == typeof(" + typeMap.TypeData.CSharpFullName + "))");
			WriteLine("{ }");
			for (int i = 0; i < derivedTypes.Count; i++)
			{
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)derivedTypes[i];
				WriteLineInd("else if (type == typeof(" + xmlTypeMapping.TypeData.CSharpFullName + ")) { ");
				WriteLine(GetWriteObjectName(xmlTypeMapping) + "((" + xmlTypeMapping.TypeData.CSharpFullName + ")ob, element, namesp, isNullable, true, writeWrappingElem);");
				WriteLine("return;");
				WriteLineUni("}");
			}
			if (typeMap.TypeData.Type == typeof(object))
			{
				WriteLineInd("else {");
				WriteLineInd("if (ob.GetType().IsArray && typeof(XmlNode).IsAssignableFrom(ob.GetType().GetElementType())) {");
				WriteLine("Writer.WriteStartElement (" + GetLiteral(typeMap.ElementName) + ", " + GetLiteral(typeMap.Namespace) + ");");
				WriteLineInd("foreach (XmlNode node in (System.Collections.IEnumerable) ob)");
				WriteLineUni("node.WriteTo (Writer);");
				WriteLineUni("Writer.WriteEndElement ();");
				WriteLine("}");
				WriteLineInd("else");
				WriteLineUni("WriteTypedPrimitive (element, namesp, ob, true);");
				WriteLine("return;");
				WriteLineUni("}");
			}
			else
			{
				WriteLineInd("else {");
				WriteLine("throw CreateUnknownTypeException (ob);");
				WriteLineUni("}");
				WriteLine(string.Empty);
				WriteLineInd("if (writeWrappingElem) {");
				if (_format == SerializationFormat.Encoded)
				{
					WriteLine("needType = true;");
				}
				WriteLine("WriteStartElement (element, namesp, ob);");
				WriteLineUni("}");
				WriteLine(string.Empty);
				WriteLine("if (needType) WriteXsiType(" + GetLiteral(typeMap.XmlType) + ", " + GetLiteral(typeMap.XmlTypeNamespace) + ");");
				WriteLine(string.Empty);
				switch (typeMap.TypeData.SchemaType)
				{
				case SchemaTypes.Class:
					GenerateWriteObjectElement(typeMap, "ob", false);
					break;
				case SchemaTypes.Array:
					GenerateWriteListElement(typeMap, "ob");
					break;
				case SchemaTypes.Primitive:
					GenerateWritePrimitiveElement(typeMap, "ob");
					break;
				case SchemaTypes.Enum:
					GenerateWriteEnumElement(typeMap, "ob");
					break;
				}
				WriteLine("if (writeWrappingElem) WriteEndElement (ob);");
			}
			GenerateEndHook();
			WriteLineUni("}");
			WriteLine(string.Empty);
			PopHookContext();
		}

		private void GenerateWriteObjectElement(XmlMapping xmlMap, string ob, bool isValueList)
		{
			XmlTypeMapping xmlTypeMapping = xmlMap as XmlTypeMapping;
			Type type = ((xmlTypeMapping == null) ? typeof(object[]) : xmlTypeMapping.TypeData.Type);
			ClassMap classMap = (ClassMap)xmlMap.ObjectMap;
			if (!GenerateWriteHook(HookType.attributes, type))
			{
				if (classMap.NamespaceDeclarations != null)
				{
					WriteLine("WriteNamespaceDeclarations ((XmlSerializerNamespaces) " + ob + ".@" + classMap.NamespaceDeclarations.Name + ");");
					WriteLine(string.Empty);
				}
				XmlTypeMapMember defaultAnyAttributeMember = classMap.DefaultAnyAttributeMember;
				if (defaultAnyAttributeMember != null && !GenerateWriteMemberHook(type, defaultAnyAttributeMember))
				{
					string text = GenerateMemberHasValueCondition(defaultAnyAttributeMember, ob, isValueList);
					if (text != null)
					{
						WriteLineInd("if (" + text + ") {");
					}
					string obTempVar = GetObTempVar();
					WriteLine("ICollection " + obTempVar + " = " + GenerateGetMemberValue(defaultAnyAttributeMember, ob, isValueList) + ";");
					WriteLineInd("if (" + obTempVar + " != null) {");
					string obTempVar2 = GetObTempVar();
					WriteLineInd("foreach (XmlAttribute " + obTempVar2 + " in " + obTempVar + ")");
					WriteLineInd("if (" + obTempVar2 + ".NamespaceURI != xmlNamespace)");
					WriteLine("WriteXmlAttribute (" + obTempVar2 + ", " + ob + ");");
					Unindent();
					Unindent();
					WriteLineUni("}");
					if (text != null)
					{
						WriteLineUni("}");
					}
					WriteLine(string.Empty);
					GenerateEndHook();
				}
				ICollection attributeMembers = classMap.AttributeMembers;
				if (attributeMembers != null)
				{
					foreach (XmlTypeMapMemberAttribute item in attributeMembers)
					{
						if (!GenerateWriteMemberHook(type, item))
						{
							string value = GenerateGetMemberValue(item, ob, isValueList);
							string text2 = GenerateMemberHasValueCondition(item, ob, isValueList);
							if (text2 != null)
							{
								WriteLineInd("if (" + text2 + ") {");
							}
							string text3 = GenerateGetStringValue(item.MappedType, item.TypeData, value, false);
							WriteLine("WriteAttribute (" + GetLiteral(item.AttributeName) + ", " + GetLiteral(item.Namespace) + ", " + text3 + ");");
							if (text2 != null)
							{
								WriteLineUni("}");
							}
							GenerateEndHook();
						}
					}
					WriteLine(string.Empty);
				}
				GenerateEndHook();
			}
			if (GenerateWriteHook(HookType.elements, type))
			{
				return;
			}
			ICollection elementMembers = classMap.ElementMembers;
			if (elementMembers != null)
			{
				foreach (XmlTypeMapMemberElement item2 in elementMembers)
				{
					if (GenerateWriteMemberHook(type, item2))
					{
						continue;
					}
					string text4 = GenerateMemberHasValueCondition(item2, ob, isValueList);
					if (text4 != null)
					{
						WriteLineInd("if (" + text4 + ") {");
					}
					string text5 = GenerateGetMemberValue(item2, ob, isValueList);
					Type type2 = item2.GetType();
					if (type2 == typeof(XmlTypeMapMemberList))
					{
						GenerateWriteMemberElement((XmlTypeMapElementInfo)item2.ElementInfo[0], text5);
					}
					else if (type2 == typeof(XmlTypeMapMemberFlatList))
					{
						WriteLineInd("if (" + text5 + " != null) {");
						GenerateWriteListContent(ob, item2.TypeData, ((XmlTypeMapMemberFlatList)item2).ListMap, text5, false);
						WriteLineUni("}");
					}
					else if (type2 == typeof(XmlTypeMapMemberAnyElement))
					{
						WriteLineInd("if (" + text5 + " != null) {");
						GenerateWriteAnyElementContent((XmlTypeMapMemberAnyElement)item2, text5);
						WriteLineUni("}");
					}
					else if (type2 == typeof(XmlTypeMapMemberAnyElement))
					{
						WriteLineInd("if (" + text5 + " != null) {");
						GenerateWriteAnyElementContent((XmlTypeMapMemberAnyElement)item2, text5);
						WriteLineUni("}");
					}
					else if (type2 != typeof(XmlTypeMapMemberAnyAttribute))
					{
						if (type2 != typeof(XmlTypeMapMemberElement))
						{
							throw new InvalidOperationException("Unknown member type");
						}
						if (item2.ElementInfo.Count == 1)
						{
							GenerateWriteMemberElement((XmlTypeMapElementInfo)item2.ElementInfo[0], text5);
						}
						else if (item2.ChoiceMember != null)
						{
							string text6 = ob + ".@" + item2.ChoiceMember;
							foreach (XmlTypeMapElementInfo item3 in item2.ElementInfo)
							{
								WriteLineInd("if (" + text6 + " == " + GetLiteral(item3.ChoiceValue) + ") {");
								GenerateWriteMemberElement(item3, GetCast(item3.TypeData, item2.TypeData, text5));
								WriteLineUni("}");
							}
						}
						else
						{
							bool flag = true;
							foreach (XmlTypeMapElementInfo item4 in item2.ElementInfo)
							{
								WriteLineInd(((!flag) ? "else " : string.Empty) + "if (" + text5 + " is " + item4.TypeData.CSharpFullName + ") {");
								GenerateWriteMemberElement(item4, GetCast(item4.TypeData, item2.TypeData, text5));
								WriteLineUni("}");
								flag = false;
							}
						}
					}
					if (text4 != null)
					{
						WriteLineUni("}");
					}
					GenerateEndHook();
				}
			}
			GenerateEndHook();
		}

		private void GenerateWriteMemberElement(XmlTypeMapElementInfo elem, string memberValue)
		{
			switch (elem.TypeData.SchemaType)
			{
			case SchemaTypes.XmlNode:
			{
				string ob = ((!elem.WrappedElement) ? string.Empty : elem.ElementName);
				if (_format == SerializationFormat.Literal)
				{
					WriteMetCall("WriteElementLiteral", memberValue, GetLiteral(ob), GetLiteral(elem.Namespace), GetLiteral(elem.IsNullable), "false");
				}
				else
				{
					WriteMetCall("WriteElementEncoded", memberValue, GetLiteral(ob), GetLiteral(elem.Namespace), GetLiteral(elem.IsNullable), "false");
				}
				break;
			}
			case SchemaTypes.Primitive:
			case SchemaTypes.Enum:
				if (_format == SerializationFormat.Literal)
				{
					GenerateWritePrimitiveValueLiteral(memberValue, elem.ElementName, elem.Namespace, elem.MappedType, elem.TypeData, elem.WrappedElement, elem.IsNullable);
				}
				else
				{
					GenerateWritePrimitiveValueEncoded(memberValue, elem.ElementName, elem.Namespace, new XmlQualifiedName(elem.TypeData.XmlType, elem.DataTypeNamespace), elem.MappedType, elem.TypeData, elem.WrappedElement, elem.IsNullable);
				}
				break;
			case SchemaTypes.Array:
				WriteLineInd("if (" + memberValue + " != null) {");
				if (elem.MappedType.MultiReferenceType)
				{
					WriteMetCall("WriteReferencingElement", GetLiteral(elem.ElementName), GetLiteral(elem.Namespace), memberValue, GetLiteral(elem.IsNullable));
					RegisterReferencingMap(elem.MappedType);
				}
				else
				{
					WriteMetCall("WriteStartElement", GetLiteral(elem.ElementName), GetLiteral(elem.Namespace), memberValue);
					GenerateWriteListContent(null, elem.TypeData, (ListMap)elem.MappedType.ObjectMap, memberValue, false);
					WriteMetCall("WriteEndElement", memberValue);
				}
				WriteLineUni("}");
				if (elem.IsNullable)
				{
					WriteLineInd("else");
					if (_format == SerializationFormat.Literal)
					{
						WriteMetCall("WriteNullTagLiteral", GetLiteral(elem.ElementName), GetLiteral(elem.Namespace));
					}
					else
					{
						WriteMetCall("WriteNullTagEncoded", GetLiteral(elem.ElementName), GetLiteral(elem.Namespace));
					}
					Unindent();
				}
				break;
			case SchemaTypes.Class:
				if (elem.MappedType.MultiReferenceType)
				{
					RegisterReferencingMap(elem.MappedType);
					if (elem.MappedType.TypeData.Type == typeof(object))
					{
						WriteMetCall("WritePotentiallyReferencingElement", GetLiteral(elem.ElementName), GetLiteral(elem.Namespace), memberValue, "null", "false", GetLiteral(elem.IsNullable));
					}
					else
					{
						WriteMetCall("WriteReferencingElement", GetLiteral(elem.ElementName), GetLiteral(elem.Namespace), memberValue, GetLiteral(elem.IsNullable));
					}
				}
				else
				{
					WriteMetCall(GetWriteObjectName(elem.MappedType), memberValue, GetLiteral(elem.ElementName), GetLiteral(elem.Namespace), GetLiteral(elem.IsNullable), "false", "true");
				}
				break;
			case SchemaTypes.XmlSerializable:
				WriteMetCall("WriteSerializable", "(" + ToCSharpFullName(elem.MappedType.TypeData.Type) + ") " + memberValue, GetLiteral(elem.ElementName), GetLiteral(elem.Namespace), GetLiteral(elem.IsNullable));
				break;
			default:
				throw new NotSupportedException("Invalid value type");
			}
		}

		private void GenerateWriteListElement(XmlTypeMapping typeMap, string ob)
		{
			if (_format == SerializationFormat.Encoded)
			{
				string itemCount = GenerateGetListCount(typeMap.TypeData, ob);
				string localName;
				string ns;
				GenerateGetArrayType((ListMap)typeMap.ObjectMap, itemCount, out localName, out ns);
				string text = ((!(ns != string.Empty)) ? GetLiteral(localName) : ("FromXmlQualifiedName (new XmlQualifiedName(" + localName + "," + ns + "))"));
				WriteMetCall("WriteAttribute", GetLiteral("arrayType"), GetLiteral("http://schemas.xmlsoap.org/soap/encoding/"), text);
			}
			GenerateWriteListContent(null, typeMap.TypeData, (ListMap)typeMap.ObjectMap, ob, false);
		}

		private void GenerateWriteAnyElementContent(XmlTypeMapMemberAnyElement member, string memberValue)
		{
			bool flag = member.TypeData.Type == typeof(XmlElement);
			string text;
			if (flag)
			{
				text = memberValue;
			}
			else
			{
				text = GetObTempVar();
				WriteLineInd("foreach (XmlNode " + text + " in " + memberValue + ") {");
			}
			string obTempVar = GetObTempVar();
			WriteLine("XmlNode " + obTempVar + " = " + text + ";");
			WriteLine("if (" + obTempVar + " is XmlElement) {");
			if (!member.IsDefaultAny)
			{
				for (int i = 0; i < member.ElementInfo.Count; i++)
				{
					XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)member.ElementInfo[i];
					string text2 = "(" + obTempVar + ".LocalName == " + GetLiteral(xmlTypeMapElementInfo.ElementName) + " && " + obTempVar + ".NamespaceURI == " + GetLiteral(xmlTypeMapElementInfo.Namespace) + ")";
					if (i == member.ElementInfo.Count - 1)
					{
						text2 += ") {";
					}
					if (i == 0)
					{
						WriteLineInd("if (" + text2);
					}
					else
					{
						WriteLine("|| " + text2);
					}
				}
			}
			WriteLine("}");
			WriteLine("else " + obTempVar + ".WriteTo (Writer);");
			if (_format == SerializationFormat.Literal)
			{
				WriteLine("WriteElementLiteral (" + obTempVar + ", \"\", \"\", false, true);");
			}
			else
			{
				WriteLine("WriteElementEncoded (" + obTempVar + ", \"\", \"\", false, true);");
			}
			if (!member.IsDefaultAny)
			{
				WriteLineUni("}");
				WriteLineInd("else");
				WriteLine("throw CreateUnknownAnyElementException (" + obTempVar + ".Name, " + obTempVar + ".NamespaceURI);");
				Unindent();
			}
			if (!flag)
			{
				WriteLineUni("}");
			}
		}

		private void GenerateWritePrimitiveElement(XmlTypeMapping typeMap, string ob)
		{
			string text = GenerateGetStringValue(typeMap, typeMap.TypeData, ob, false);
			WriteLine("Writer.WriteString (" + text + ");");
		}

		private void GenerateWriteEnumElement(XmlTypeMapping typeMap, string ob)
		{
			string text = GenerateGetEnumXmlValue(typeMap, ob);
			WriteLine("Writer.WriteString (" + text + ");");
		}

		private string GenerateGetStringValue(XmlTypeMapping typeMap, TypeData type, string value, bool isNullable)
		{
			if (type.SchemaType == SchemaTypes.Array)
			{
				string strTempVar = GetStrTempVar();
				WriteLine("string " + strTempVar + " = null;");
				WriteLineInd("if (" + value + " != null) {");
				string text = GenerateWriteListContent(null, typeMap.TypeData, (ListMap)typeMap.ObjectMap, value, true);
				WriteLine(strTempVar + " = " + text + ".ToString ().Trim ();");
				WriteLineUni("}");
				return strTempVar;
			}
			if (type.SchemaType == SchemaTypes.Enum)
			{
				if (isNullable)
				{
					return "(" + value + ").HasValue ? " + GenerateGetEnumXmlValue(typeMap, "(" + value + ").Value") + " : null";
				}
				return GenerateGetEnumXmlValue(typeMap, value);
			}
			if (type.Type == typeof(XmlQualifiedName))
			{
				return "FromXmlQualifiedName (" + value + ")";
			}
			if (value == null)
			{
				return null;
			}
			return XmlCustomFormatter.GenerateToXmlString(type, value);
		}

		private string GenerateGetEnumXmlValue(XmlTypeMapping typeMap, string ob)
		{
			return GetGetEnumValueName(typeMap) + " (" + ob + ")";
		}

		private string GenerateGetListCount(TypeData listType, string ob)
		{
			if (listType.Type.IsArray)
			{
				return "ob.Length";
			}
			return "ob.Count";
		}

		private void GenerateGetArrayType(ListMap map, string itemCount, out string localName, out string ns)
		{
			string text = ((!(itemCount != string.Empty)) ? "[]" : string.Empty);
			XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)map.ItemInfo[0];
			if (xmlTypeMapElementInfo.TypeData.SchemaType == SchemaTypes.Array)
			{
				string localName2;
				GenerateGetArrayType((ListMap)xmlTypeMapElementInfo.MappedType.ObjectMap, string.Empty, out localName2, out ns);
				localName = localName2 + text;
			}
			else if (xmlTypeMapElementInfo.MappedType != null)
			{
				localName = xmlTypeMapElementInfo.MappedType.XmlType + text;
				ns = xmlTypeMapElementInfo.MappedType.Namespace;
			}
			else
			{
				localName = xmlTypeMapElementInfo.TypeData.XmlType + text;
				ns = xmlTypeMapElementInfo.DataTypeNamespace;
			}
			if (itemCount != string.Empty)
			{
				localName = "\"" + localName + "[\" + " + itemCount + " + \"]\"";
				ns = GetLiteral(ns);
			}
		}

		private string GenerateWriteListContent(string container, TypeData listType, ListMap map, string ob, bool writeToString)
		{
			string text = null;
			if (writeToString)
			{
				text = GetStrTempVar();
				WriteLine("System.Text.StringBuilder " + text + " = new System.Text.StringBuilder();");
			}
			if (listType.Type.IsArray)
			{
				string numTempVar = GetNumTempVar();
				WriteLineInd("for (int " + numTempVar + " = 0; " + numTempVar + " < " + ob + ".Length; " + numTempVar + "++) {");
				GenerateListLoop(container, map, ob + "[" + numTempVar + "]", numTempVar, listType.ListItemTypeData, text);
				WriteLineUni("}");
			}
			else if (typeof(ICollection).IsAssignableFrom(listType.Type))
			{
				string numTempVar2 = GetNumTempVar();
				WriteLineInd("for (int " + numTempVar2 + " = 0; " + numTempVar2 + " < " + ob + ".Count; " + numTempVar2 + "++) {");
				GenerateListLoop(container, map, ob + "[" + numTempVar2 + "]", numTempVar2, listType.ListItemTypeData, text);
				WriteLineUni("}");
			}
			else
			{
				if (!typeof(IEnumerable).IsAssignableFrom(listType.Type))
				{
					throw new Exception("Unsupported collection type");
				}
				string obTempVar = GetObTempVar();
				WriteLineInd("foreach (" + listType.ListItemTypeData.CSharpFullName + " " + obTempVar + " in " + ob + ") {");
				GenerateListLoop(container, map, obTempVar, null, listType.ListItemTypeData, text);
				WriteLineUni("}");
			}
			return text;
		}

		private void GenerateListLoop(string container, ListMap map, string item, string index, TypeData itemTypeData, string targetString)
		{
			bool flag = map.ItemInfo.Count > 1;
			if (map.ChoiceMember != null && container != null && index != null)
			{
				WriteLineInd("if ((" + container + ".@" + map.ChoiceMember + " == null) || (" + index + " >= " + container + ".@" + map.ChoiceMember + ".Length))");
				WriteLine("throw CreateInvalidChoiceIdentifierValueException (" + container + ".GetType().ToString(), \"" + map.ChoiceMember + "\");");
				Unindent();
			}
			if (flag)
			{
				WriteLine("if (((object)" + item + ") == null) { }");
			}
			foreach (XmlTypeMapElementInfo item2 in map.ItemInfo)
			{
				if (map.ChoiceMember != null && flag)
				{
					WriteLineInd("else if (" + container + ".@" + map.ChoiceMember + "[" + index + "] == " + GetLiteral(item2.ChoiceValue) + ") {");
				}
				else if (flag)
				{
					WriteLineInd("else if (" + item + ".GetType() == typeof(" + item2.TypeData.CSharpFullName + ")) {");
				}
				if (targetString == null)
				{
					GenerateWriteMemberElement(item2, GetCast(item2.TypeData, itemTypeData, item));
				}
				else
				{
					string text = GenerateGetStringValue(item2.MappedType, item2.TypeData, GetCast(item2.TypeData, itemTypeData, item), false);
					WriteLine(targetString + ".Append (" + text + ").Append (\" \");");
				}
				if (flag)
				{
					WriteLineUni("}");
				}
			}
			if (flag)
			{
				WriteLine("else throw CreateUnknownTypeException (" + item + ");");
			}
		}

		private void GenerateWritePrimitiveValueLiteral(string memberValue, string name, string ns, XmlTypeMapping mappedType, TypeData typeData, bool wrapped, bool isNullable)
		{
			if (!wrapped)
			{
				string text = GenerateGetStringValue(mappedType, typeData, memberValue, false);
				WriteMetCall("WriteValue", text);
			}
			else if (isNullable)
			{
				if (typeData.Type == typeof(XmlQualifiedName))
				{
					WriteMetCall("WriteNullableQualifiedNameLiteral", GetLiteral(name), GetLiteral(ns), memberValue);
				}
				else
				{
					string text2 = GenerateGetStringValue(mappedType, typeData, memberValue, true);
					WriteMetCall("WriteNullableStringLiteral", GetLiteral(name), GetLiteral(ns), text2);
				}
			}
			else if (typeData.Type == typeof(XmlQualifiedName))
			{
				WriteMetCall("WriteElementQualifiedName", GetLiteral(name), GetLiteral(ns), memberValue);
			}
			else
			{
				string text3 = GenerateGetStringValue(mappedType, typeData, memberValue, false);
				WriteMetCall("WriteElementString", GetLiteral(name), GetLiteral(ns), text3);
			}
		}

		private void GenerateWritePrimitiveValueEncoded(string memberValue, string name, string ns, XmlQualifiedName xsiType, XmlTypeMapping mappedType, TypeData typeData, bool wrapped, bool isNullable)
		{
			if (!wrapped)
			{
				string text = GenerateGetStringValue(mappedType, typeData, memberValue, false);
				WriteMetCall("WriteValue", text);
			}
			else if (isNullable)
			{
				if (typeData.Type == typeof(XmlQualifiedName))
				{
					WriteMetCall("WriteNullableQualifiedNameEncoded", GetLiteral(name), GetLiteral(ns), memberValue, GetLiteral(xsiType));
				}
				else
				{
					string text2 = GenerateGetStringValue(mappedType, typeData, memberValue, true);
					WriteMetCall("WriteNullableStringEncoded", GetLiteral(name), GetLiteral(ns), text2, GetLiteral(xsiType));
				}
			}
			else if (typeData.Type == typeof(XmlQualifiedName))
			{
				WriteMetCall("WriteElementQualifiedName", GetLiteral(name), GetLiteral(ns), memberValue, GetLiteral(xsiType));
			}
			else
			{
				string text3 = GenerateGetStringValue(mappedType, typeData, memberValue, false);
				WriteMetCall("WriteElementString", GetLiteral(name), GetLiteral(ns), text3, GetLiteral(xsiType));
			}
		}

		private string GenerateGetMemberValue(XmlTypeMapMember member, string ob, bool isValueList)
		{
			if (isValueList)
			{
				return GetCast(member.TypeData, TypeTranslator.GetTypeData(typeof(object)), ob + "[" + member.GlobalIndex + "]");
			}
			return ob + ".@" + member.Name;
		}

		private string GenerateMemberHasValueCondition(XmlTypeMapMember member, string ob, bool isValueList)
		{
			if (isValueList)
			{
				return ob + ".Length > " + member.GlobalIndex;
			}
			if (member.DefaultValue != DBNull.Value)
			{
				string text = ob + ".@" + member.Name;
				if (member.DefaultValue == null)
				{
					return text + " != null";
				}
				if (member.TypeData.SchemaType == SchemaTypes.Enum)
				{
					return text + " != " + GetCast(member.TypeData, GetLiteral(member.DefaultValue));
				}
				return text + " != " + GetLiteral(member.DefaultValue);
			}
			if (member.IsOptionalValueType)
			{
				return ob + ".@" + member.Name + "Specified";
			}
			return null;
		}

		private void GenerateWriteInitCallbacks()
		{
			WriteLine("protected override void InitCallbacks ()");
			WriteLineInd("{");
			if (_format == SerializationFormat.Encoded)
			{
				foreach (XmlMapping item in _mapsToGenerate)
				{
					XmlTypeMapping xmlTypeMapping = item as XmlTypeMapping;
					if (xmlTypeMapping != null)
					{
						WriteMetCall("AddWriteCallback", GetTypeOf(xmlTypeMapping.TypeData), GetLiteral(xmlTypeMapping.XmlType), GetLiteral(xmlTypeMapping.Namespace), "new XmlSerializationWriteCallback (" + GetWriteObjectCallbackName(xmlTypeMapping) + ")");
					}
				}
			}
			WriteLineUni("}");
			WriteLine(string.Empty);
			if (_format != SerializationFormat.Encoded)
			{
				return;
			}
			foreach (XmlTypeMapping item2 in _mapsToGenerate)
			{
				XmlTypeMapping xmlTypeMapping3 = item2;
				if (xmlTypeMapping3 != null)
				{
					if (xmlTypeMapping3.TypeData.SchemaType == SchemaTypes.Enum)
					{
						WriteWriteEnumCallback(xmlTypeMapping3);
					}
					else
					{
						WriteWriteObjectCallback(xmlTypeMapping3);
					}
				}
			}
		}

		private void WriteWriteEnumCallback(XmlTypeMapping map)
		{
			WriteLine("void " + GetWriteObjectCallbackName(map) + " (object ob)");
			WriteLineInd("{");
			WriteMetCall(GetWriteObjectName(map), GetCast(map.TypeData, "ob"), GetLiteral(map.ElementName), GetLiteral(map.Namespace), "false", "true", "false");
			WriteLineUni("}");
			WriteLine(string.Empty);
		}

		private void WriteWriteObjectCallback(XmlTypeMapping map)
		{
			WriteLine("void " + GetWriteObjectCallbackName(map) + " (object ob)");
			WriteLineInd("{");
			WriteMetCall(GetWriteObjectName(map), GetCast(map.TypeData, "ob"), GetLiteral(map.ElementName), GetLiteral(map.Namespace), "false", "false", "false");
			WriteLineUni("}");
			WriteLine(string.Empty);
		}

		public void GenerateReader(string readerClassName, ArrayList maps)
		{
			if (_config == null || !_config.GenerateAsInternal)
			{
				WriteLine("public class " + readerClassName + " : XmlSerializationReader");
			}
			else
			{
				WriteLine("internal class " + readerClassName + " : XmlSerializationReader");
			}
			WriteLineInd("{");
			WriteLine("static readonly System.Reflection.MethodInfo fromBinHexStringMethod = typeof (XmlConvert).GetMethod (\"FromBinHexString\", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new Type [] {typeof (string)}, null);");
			WriteLine("static byte [] FromBinHexString (string input)");
			WriteLineInd("{");
			WriteLine("return input == null ? null : (byte []) fromBinHexStringMethod.Invoke (null, new object [] {input});");
			WriteLineUni("}");
			_mapsToGenerate = new ArrayList();
			_fixupCallbacks = new ArrayList();
			InitHooks();
			for (int i = 0; i < maps.Count; i++)
			{
				GenerationResult generationResult = (GenerationResult)maps[i];
				_typeMap = generationResult.Mapping;
				_format = _typeMap.Format;
				_result = generationResult;
				GenerateReadRoot();
			}
			for (int j = 0; j < _mapsToGenerate.Count; j++)
			{
				XmlTypeMapping xmlTypeMapping = _mapsToGenerate[j] as XmlTypeMapping;
				if (xmlTypeMapping != null)
				{
					GenerateReadObject(xmlTypeMapping);
					if (xmlTypeMapping.TypeData.SchemaType == SchemaTypes.Enum)
					{
						GenerateGetEnumValueMethod(xmlTypeMapping);
					}
				}
			}
			GenerateReadInitCallbacks();
			if (_format == SerializationFormat.Encoded)
			{
				GenerateFixupCallbacks();
				GenerateFillerCallbacks();
			}
			WriteLineUni("}");
			UpdateGeneratedTypes(_mapsToGenerate);
		}

		private void GenerateReadRoot()
		{
			WriteLine("public object " + _result.ReadMethodName + " ()");
			WriteLineInd("{");
			WriteLine("Reader.MoveToContent();");
			if (_typeMap is XmlTypeMapping)
			{
				XmlTypeMapping xmlTypeMapping = (XmlTypeMapping)_typeMap;
				if (_format == SerializationFormat.Literal)
				{
					if (xmlTypeMapping.TypeData.SchemaType == SchemaTypes.XmlNode)
					{
						if (xmlTypeMapping.TypeData.Type == typeof(XmlDocument))
						{
							WriteLine("return ReadXmlDocument (false);");
						}
						else
						{
							WriteLine("return ReadXmlNode (false);");
						}
					}
					else
					{
						WriteLineInd("if (Reader.LocalName != " + GetLiteral(xmlTypeMapping.ElementName) + " || Reader.NamespaceURI != " + GetLiteral(xmlTypeMapping.Namespace) + ")");
						WriteLine("throw CreateUnknownNodeException();");
						Unindent();
						WriteLine("return " + GetReadObjectCall(xmlTypeMapping, GetLiteral(xmlTypeMapping.IsNullable), "true") + ";");
					}
				}
				else
				{
					WriteLine("object ob = null;");
					WriteLine("Reader.MoveToContent();");
					WriteLine("if (Reader.NodeType == System.Xml.XmlNodeType.Element) ");
					WriteLineInd("{");
					WriteLineInd("if (Reader.LocalName == " + GetLiteral(xmlTypeMapping.ElementName) + " && Reader.NamespaceURI == " + GetLiteral(xmlTypeMapping.Namespace) + ")");
					WriteLine("ob = ReadReferencedElement();");
					Unindent();
					WriteLineInd("else ");
					WriteLine("throw CreateUnknownNodeException();");
					Unindent();
					WriteLineUni("}");
					WriteLineInd("else ");
					WriteLine("UnknownNode(null);");
					Unindent();
					WriteLine(string.Empty);
					WriteLine("ReadReferencedElements();");
					WriteLine("return ob;");
					RegisterReferencingMap(xmlTypeMapping);
				}
			}
			else
			{
				WriteLine("return " + GenerateReadMessage((XmlMembersMapping)_typeMap) + ";");
			}
			WriteLineUni("}");
			WriteLine(string.Empty);
		}

		private string GenerateReadMessage(XmlMembersMapping typeMap)
		{
			WriteLine("object[] parameters = new object[" + typeMap.Count + "];");
			WriteLine(string.Empty);
			if (typeMap.HasWrapperElement)
			{
				if (_format == SerializationFormat.Encoded)
				{
					WriteLine("while (Reader.NodeType == System.Xml.XmlNodeType.Element)");
					WriteLineInd("{");
					WriteLine("string root = Reader.GetAttribute (\"root\", " + GetLiteral("http://schemas.xmlsoap.org/soap/encoding/") + ");");
					WriteLine("if (root == null || System.Xml.XmlConvert.ToBoolean(root)) break;");
					WriteLine("ReadReferencedElement ();");
					WriteLine("Reader.MoveToContent ();");
					WriteLineUni("}");
					WriteLine(string.Empty);
					WriteLine("if (Reader.NodeType != System.Xml.XmlNodeType.EndElement)");
					WriteLineInd("{");
					WriteLineInd("if (Reader.IsEmptyElement) {");
					WriteLine("Reader.Skip();");
					WriteLine("Reader.MoveToContent();");
					WriteLineUni("}");
					WriteLineInd("else {");
					WriteLine("Reader.ReadStartElement();");
					GenerateReadMembers(typeMap, (ClassMap)typeMap.ObjectMap, "parameters", true, false);
					WriteLine("ReadEndElement();");
					WriteLineUni("}");
					WriteLine(string.Empty);
					WriteLine("Reader.MoveToContent();");
					WriteLineUni("}");
				}
				else
				{
					ClassMap classMap = (ClassMap)typeMap.ObjectMap;
					ArrayList allMembers = classMap.AllMembers;
					for (int i = 0; i < allMembers.Count; i++)
					{
						XmlTypeMapMember xmlTypeMapMember = (XmlTypeMapMember)allMembers[i];
						if (!xmlTypeMapMember.IsReturnValue && xmlTypeMapMember.TypeData.IsValueType)
						{
							GenerateSetMemberValueFromAttr(xmlTypeMapMember, "parameters", string.Format("({0}) Activator.CreateInstance(typeof({0}), true)", xmlTypeMapMember.TypeData.FullTypeName), true);
						}
					}
					WriteLine("while (Reader.NodeType != System.Xml.XmlNodeType.EndElement && Reader.ReadState == ReadState.Interactive)");
					WriteLineInd("{");
					WriteLine("if (Reader.IsStartElement(" + GetLiteral(typeMap.ElementName) + ", " + GetLiteral(typeMap.Namespace) + "))");
					WriteLineInd("{");
					bool first = false;
					GenerateReadAttributeMembers(typeMap, (ClassMap)typeMap.ObjectMap, "parameters", true, ref first);
					WriteLine("if (Reader.IsEmptyElement)");
					WriteLineInd("{");
					WriteLine("Reader.Skip(); Reader.MoveToContent(); continue;");
					WriteLineUni("}");
					WriteLine("Reader.ReadStartElement();");
					GenerateReadMembers(typeMap, (ClassMap)typeMap.ObjectMap, "parameters", true, false);
					WriteLine("ReadEndElement();");
					WriteLine("break;");
					WriteLineUni("}");
					WriteLineInd("else ");
					WriteLine("UnknownNode(null);");
					Unindent();
					WriteLine(string.Empty);
					WriteLine("Reader.MoveToContent();");
					WriteLineUni("}");
				}
			}
			else
			{
				GenerateReadMembers(typeMap, (ClassMap)typeMap.ObjectMap, "parameters", true, _format == SerializationFormat.Encoded);
			}
			if (_format == SerializationFormat.Encoded)
			{
				WriteLine("ReadReferencedElements();");
			}
			return "parameters";
		}

		private void GenerateReadObject(XmlTypeMapping typeMap)
		{
			string isNullable;
			if (_format == SerializationFormat.Literal)
			{
				WriteLine("public " + typeMap.TypeData.CSharpFullName + " " + GetReadObjectName(typeMap) + " (bool isNullable, bool checkType)");
				isNullable = "isNullable";
			}
			else
			{
				WriteLine("public object " + GetReadObjectName(typeMap) + " ()");
				isNullable = "true";
			}
			WriteLineInd("{");
			PushHookContext();
			SetHookVar("$TYPE", typeMap.TypeData.CSharpName);
			SetHookVar("$FULLTYPE", typeMap.TypeData.CSharpFullName);
			SetHookVar("$NULLABLE", "isNullable");
			switch (typeMap.TypeData.SchemaType)
			{
			case SchemaTypes.Class:
				GenerateReadClassInstance(typeMap, isNullable, "checkType");
				break;
			case SchemaTypes.Array:
			{
				string text = GenerateReadListElement(typeMap, null, isNullable, true);
				if (text != null)
				{
					WriteLine("return " + text + ";");
				}
				break;
			}
			case SchemaTypes.XmlNode:
				GenerateReadXmlNodeElement(typeMap, isNullable);
				break;
			case SchemaTypes.Primitive:
				GenerateReadPrimitiveElement(typeMap, isNullable);
				break;
			case SchemaTypes.Enum:
				GenerateReadEnumElement(typeMap, isNullable);
				break;
			case SchemaTypes.XmlSerializable:
				GenerateReadXmlSerializableElement(typeMap, isNullable);
				break;
			default:
				throw new Exception("Unsupported map type");
			}
			WriteLineUni("}");
			WriteLine(string.Empty);
			PopHookContext();
		}

		private void GenerateReadClassInstance(XmlTypeMapping typeMap, string isNullable, string checkType)
		{
			SetHookVar("$OBJECT", "ob");
			if (!typeMap.TypeData.IsValueType)
			{
				WriteLine(typeMap.TypeData.CSharpFullName + " ob = null;");
				if (GenerateReadHook(HookType.type, typeMap.TypeData.Type))
				{
					WriteLine("return ob;");
					return;
				}
				if (_format == SerializationFormat.Literal)
				{
					WriteLine("if (" + isNullable + " && ReadNull()) return null;");
					WriteLine(string.Empty);
					WriteLine("if (checkType) ");
					WriteLineInd("{");
				}
				else
				{
					WriteLine("if (ReadNull()) return null;");
					WriteLine(string.Empty);
				}
			}
			else
			{
				WriteLine(typeMap.TypeData.CSharpFullName + string.Format(" ob = ({0}) Activator.CreateInstance(typeof({0}), true);", typeMap.TypeData.CSharpFullName));
				if (GenerateReadHook(HookType.type, typeMap.TypeData.Type))
				{
					WriteLine("return ob;");
					return;
				}
			}
			WriteLine("System.Xml.XmlQualifiedName t = GetXsiType();");
			WriteLine("if (t == null)");
			if (typeMap.TypeData.Type != typeof(object))
			{
				WriteLine("{ }");
			}
			else
			{
				WriteLine("\treturn " + GetCast(typeMap.TypeData, "ReadTypedPrimitive (new System.Xml.XmlQualifiedName(\"anyType\", System.Xml.Schema.XmlSchema.Namespace))") + ";");
			}
			foreach (XmlTypeMapping derivedType in typeMap.DerivedTypes)
			{
				WriteLineInd("else if (t.Name == " + GetLiteral(derivedType.XmlType) + " && t.Namespace == " + GetLiteral(derivedType.XmlTypeNamespace) + ")");
				WriteLine("return " + GetReadObjectCall(derivedType, isNullable, checkType) + ";");
				Unindent();
			}
			WriteLine("else if (t.Name != " + GetLiteral(typeMap.XmlType) + " || t.Namespace != " + GetLiteral(typeMap.XmlTypeNamespace) + ")");
			if (typeMap.TypeData.Type == typeof(object))
			{
				WriteLine("\treturn " + GetCast(typeMap.TypeData, "ReadTypedPrimitive (t)") + ";");
			}
			else
			{
				WriteLine("\tthrow CreateUnknownTypeException(t);");
			}
			if (!typeMap.TypeData.IsValueType)
			{
				if (_format == SerializationFormat.Literal)
				{
					WriteLineUni("}");
				}
				if (typeMap.TypeData.Type.IsAbstract)
				{
					GenerateEndHook();
					WriteLine("return ob;");
					return;
				}
				WriteLine(string.Empty);
				WriteLine(string.Format("ob = ({0}) Activator.CreateInstance(typeof({0}), true);", typeMap.TypeData.CSharpFullName));
			}
			WriteLine(string.Empty);
			WriteLine("Reader.MoveToElement();");
			WriteLine(string.Empty);
			GenerateReadMembers(typeMap, (ClassMap)typeMap.ObjectMap, "ob", false, false);
			WriteLine(string.Empty);
			GenerateEndHook();
			WriteLine("return ob;");
		}

		private void GenerateReadMembers(XmlMapping xmlMap, ClassMap map, string ob, bool isValueList, bool readByOrder)
		{
			XmlTypeMapping xmlTypeMapping = xmlMap as XmlTypeMapping;
			Type type = ((xmlTypeMapping == null) ? typeof(object[]) : xmlTypeMapping.TypeData.Type);
			bool first = false;
			GenerateReadAttributeMembers(xmlMap, map, ob, isValueList, ref first);
			if (!isValueList)
			{
				WriteLine("Reader.MoveToElement();");
				WriteLineInd("if (Reader.IsEmptyElement) {");
				WriteLine("Reader.Skip ();");
				GenerateSetListMembersDefaults(xmlTypeMapping, map, ob, isValueList);
				WriteLine("return " + ob + ";");
				WriteLineUni("}");
				WriteLine(string.Empty);
				WriteLine("Reader.ReadStartElement();");
			}
			WriteLine("Reader.MoveToContent();");
			WriteLine(string.Empty);
			if (!GenerateReadHook(HookType.elements, type))
			{
				string[] array = null;
				if (map.ElementMembers != null && !readByOrder)
				{
					string text = string.Empty;
					array = new string[map.ElementMembers.Count];
					int num = 0;
					foreach (XmlTypeMapMember elementMember in map.ElementMembers)
					{
						if (!(elementMember is XmlTypeMapMemberElement) || !((XmlTypeMapMemberElement)elementMember).IsXmlTextCollector)
						{
							array[num] = GetBoolTempVar();
							if (text.Length > 0)
							{
								text += ", ";
							}
							text = text + array[num] + "=false";
						}
						num++;
					}
					if (text.Length > 0)
					{
						text = "bool " + text;
						WriteLine(text + ";");
					}
					WriteLine(string.Empty);
				}
				string[] array2 = null;
				string[] array3 = null;
				string[] array4 = null;
				if (map.FlatLists != null)
				{
					array2 = new string[map.FlatLists.Count];
					array3 = new string[map.FlatLists.Count];
					string text2 = "int ";
					for (int i = 0; i < map.FlatLists.Count; i++)
					{
						XmlTypeMapMemberElement xmlTypeMapMemberElement = (XmlTypeMapMemberElement)map.FlatLists[i];
						array2[i] = GetNumTempVar();
						if (i > 0)
						{
							text2 += ", ";
						}
						text2 = text2 + array2[i] + "=0";
						if (!MemberHasReadReplaceHook(type, xmlTypeMapMemberElement))
						{
							array3[i] = GetObTempVar();
							WriteLine(xmlTypeMapMemberElement.TypeData.CSharpFullName + " " + array3[i] + ";");
							if (IsReadOnly(xmlTypeMapping, xmlTypeMapMemberElement, xmlTypeMapMemberElement.TypeData, isValueList))
							{
								string text3 = GenerateGetMemberValue(xmlTypeMapMemberElement, ob, isValueList);
								WriteLine(array3[i] + " = " + text3 + ";");
							}
							else if (xmlTypeMapMemberElement.TypeData.Type.IsArray)
							{
								string text3 = GenerateInitializeList(xmlTypeMapMemberElement.TypeData);
								WriteLine(array3[i] + " = " + text3 + ";");
							}
							else
							{
								WriteLine(array3[i] + " = " + GenerateGetMemberValue(xmlTypeMapMemberElement, ob, isValueList) + ";");
								WriteLineInd("if (((object)" + array3[i] + ") == null) {");
								WriteLine(array3[i] + " = " + GenerateInitializeList(xmlTypeMapMemberElement.TypeData) + ";");
								GenerateSetMemberValue(xmlTypeMapMemberElement, ob, array3[i], isValueList);
								WriteLineUni("}");
							}
						}
						if (xmlTypeMapMemberElement.ChoiceMember != null)
						{
							if (array4 == null)
							{
								array4 = new string[map.FlatLists.Count];
							}
							array4[i] = GetObTempVar();
							string text4 = GenerateInitializeList(xmlTypeMapMemberElement.ChoiceTypeData);
							WriteLine(xmlTypeMapMemberElement.ChoiceTypeData.CSharpFullName + " " + array4[i] + " = " + text4 + ";");
						}
					}
					WriteLine(text2 + ";");
					WriteLine(string.Empty);
				}
				if (_format == SerializationFormat.Encoded && map.ElementMembers != null)
				{
					_fixupCallbacks.Add(xmlMap);
					WriteLine("Fixup fixup = new Fixup(" + ob + ", new XmlSerializationFixupCallback(" + GetFixupCallbackName(xmlMap) + "), " + map.ElementMembers.Count + ");");
					WriteLine("AddFixup (fixup);");
					WriteLine(string.Empty);
				}
				ArrayList arrayList = null;
				int num2;
				if (readByOrder)
				{
					num2 = ((map.ElementMembers != null) ? map.ElementMembers.Count : 0);
				}
				else
				{
					arrayList = new ArrayList();
					arrayList.AddRange(map.AllElementInfos);
					num2 = arrayList.Count;
					WriteLine("while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) ");
					WriteLineInd("{");
					WriteLine("if (Reader.NodeType == System.Xml.XmlNodeType.Element) ");
					WriteLineInd("{");
				}
				first = true;
				for (int j = 0; j < num2; j++)
				{
					XmlTypeMapElementInfo xmlTypeMapElementInfo = ((!readByOrder) ? ((XmlTypeMapElementInfo)arrayList[j]) : map.GetElement(j));
					if (!readByOrder)
					{
						if (xmlTypeMapElementInfo.IsTextElement || xmlTypeMapElementInfo.IsUnnamedAnyElement)
						{
							continue;
						}
						string text5 = ((!first) ? "else " : string.Empty);
						text5 += "if (";
						if (!xmlTypeMapElementInfo.Member.IsReturnValue || _format != SerializationFormat.Encoded)
						{
							text5 = text5 + "Reader.LocalName == " + GetLiteral(xmlTypeMapElementInfo.ElementName);
							if (!map.IgnoreMemberNamespace)
							{
								text5 = text5 + " && Reader.NamespaceURI == " + GetLiteral(xmlTypeMapElementInfo.Namespace);
							}
							text5 += " && ";
						}
						text5 = text5 + "!" + array[xmlTypeMapElementInfo.Member.Index] + ") {";
						WriteLineInd(text5);
					}
					if (xmlTypeMapElementInfo.Member.GetType() == typeof(XmlTypeMapMemberList))
					{
						if (_format == SerializationFormat.Encoded && xmlTypeMapElementInfo.MultiReferenceType)
						{
							string obTempVar = GetObTempVar();
							WriteLine("object " + obTempVar + " = ReadReferencingElement (out fixup.Ids[" + xmlTypeMapElementInfo.Member.Index + "]);");
							RegisterReferencingMap(xmlTypeMapElementInfo.MappedType);
							WriteLineInd("if (fixup.Ids[" + xmlTypeMapElementInfo.Member.Index + "] == null) {");
							if (IsReadOnly(xmlTypeMapping, xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, isValueList))
							{
								WriteLine("throw CreateReadOnlyCollectionException (" + GetLiteral(xmlTypeMapElementInfo.TypeData.CSharpFullName) + ");");
							}
							else
							{
								GenerateSetMemberValue(xmlTypeMapElementInfo.Member, ob, GetCast(xmlTypeMapElementInfo.Member.TypeData, obTempVar), isValueList);
							}
							WriteLineUni("}");
							if (!xmlTypeMapElementInfo.MappedType.TypeData.Type.IsArray)
							{
								WriteLineInd("else {");
								if (IsReadOnly(xmlTypeMapping, xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, isValueList))
								{
									WriteLine(obTempVar + " = " + GenerateGetMemberValue(xmlTypeMapElementInfo.Member, ob, isValueList) + ";");
								}
								else
								{
									WriteLine(obTempVar + " = " + GenerateCreateList(xmlTypeMapElementInfo.MappedType.TypeData.Type) + ";");
									GenerateSetMemberValue(xmlTypeMapElementInfo.Member, ob, GetCast(xmlTypeMapElementInfo.Member.TypeData, obTempVar), isValueList);
								}
								WriteLine("AddFixup (new CollectionFixup (" + obTempVar + ", new XmlSerializationCollectionFixupCallback (" + GetFillListName(xmlTypeMapElementInfo.Member.TypeData) + "), fixup.Ids[" + xmlTypeMapElementInfo.Member.Index + "]));");
								WriteLine("fixup.Ids[" + xmlTypeMapElementInfo.Member.Index + "] = null;");
								WriteLineUni("}");
							}
						}
						else if (!GenerateReadMemberHook(type, xmlTypeMapElementInfo.Member))
						{
							if (IsReadOnly(xmlTypeMapping, xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, isValueList))
							{
								GenerateReadListElement(xmlTypeMapElementInfo.MappedType, GenerateGetMemberValue(xmlTypeMapElementInfo.Member, ob, isValueList), GetLiteral(xmlTypeMapElementInfo.IsNullable), false);
							}
							else if (xmlTypeMapElementInfo.MappedType.TypeData.Type.IsArray)
							{
								if (xmlTypeMapElementInfo.IsNullable)
								{
									GenerateSetMemberValue(xmlTypeMapElementInfo.Member, ob, GenerateReadListElement(xmlTypeMapElementInfo.MappedType, null, GetLiteral(xmlTypeMapElementInfo.IsNullable), true), isValueList);
								}
								else
								{
									string obTempVar2 = GetObTempVar();
									WriteLine(xmlTypeMapElementInfo.MappedType.TypeData.CSharpFullName + " " + obTempVar2 + " = " + GenerateReadListElement(xmlTypeMapElementInfo.MappedType, null, GetLiteral(xmlTypeMapElementInfo.IsNullable), true) + ";");
									WriteLineInd("if (((object)" + obTempVar2 + ") != null) {");
									GenerateSetMemberValue(xmlTypeMapElementInfo.Member, ob, obTempVar2, isValueList);
									WriteLineUni("}");
								}
							}
							else
							{
								string obTempVar3 = GetObTempVar();
								WriteLine(xmlTypeMapElementInfo.MappedType.TypeData.CSharpFullName + " " + obTempVar3 + " = " + GenerateGetMemberValue(xmlTypeMapElementInfo.Member, ob, isValueList) + ";");
								WriteLineInd("if (((object)" + obTempVar3 + ") == null) {");
								WriteLine(obTempVar3 + " = " + GenerateCreateList(xmlTypeMapElementInfo.MappedType.TypeData.Type) + ";");
								GenerateSetMemberValue(xmlTypeMapElementInfo.Member, ob, obTempVar3, isValueList);
								WriteLineUni("}");
								GenerateReadListElement(xmlTypeMapElementInfo.MappedType, obTempVar3, GetLiteral(xmlTypeMapElementInfo.IsNullable), true);
							}
							GenerateEndHook();
						}
						if (!readByOrder)
						{
							WriteLine(array[xmlTypeMapElementInfo.Member.Index] + " = true;");
						}
					}
					else if (xmlTypeMapElementInfo.Member.GetType() == typeof(XmlTypeMapMemberFlatList))
					{
						XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList = (XmlTypeMapMemberFlatList)xmlTypeMapElementInfo.Member;
						if (!GenerateReadArrayMemberHook(type, xmlTypeMapElementInfo.Member, array2[xmlTypeMapMemberFlatList.FlatArrayIndex]))
						{
							GenerateAddListValue(xmlTypeMapMemberFlatList.TypeData, array3[xmlTypeMapMemberFlatList.FlatArrayIndex], array2[xmlTypeMapMemberFlatList.FlatArrayIndex], GenerateReadObjectElement(xmlTypeMapElementInfo), !IsReadOnly(xmlTypeMapping, xmlTypeMapElementInfo.Member, xmlTypeMapElementInfo.TypeData, isValueList));
							if (xmlTypeMapMemberFlatList.ChoiceMember != null)
							{
								GenerateAddListValue(xmlTypeMapMemberFlatList.ChoiceTypeData, array4[xmlTypeMapMemberFlatList.FlatArrayIndex], array2[xmlTypeMapMemberFlatList.FlatArrayIndex], GetLiteral(xmlTypeMapElementInfo.ChoiceValue), true);
							}
							GenerateEndHook();
						}
						WriteLine(array2[xmlTypeMapMemberFlatList.FlatArrayIndex] + "++;");
					}
					else if (xmlTypeMapElementInfo.Member.GetType() == typeof(XmlTypeMapMemberAnyElement))
					{
						XmlTypeMapMemberAnyElement xmlTypeMapMemberAnyElement = (XmlTypeMapMemberAnyElement)xmlTypeMapElementInfo.Member;
						if (xmlTypeMapMemberAnyElement.TypeData.IsListType)
						{
							if (!GenerateReadArrayMemberHook(type, xmlTypeMapElementInfo.Member, array2[xmlTypeMapMemberAnyElement.FlatArrayIndex]))
							{
								GenerateAddListValue(xmlTypeMapMemberAnyElement.TypeData, array3[xmlTypeMapMemberAnyElement.FlatArrayIndex], array2[xmlTypeMapMemberAnyElement.FlatArrayIndex], GetReadXmlNode(xmlTypeMapMemberAnyElement.TypeData.ListItemTypeData, false), true);
								GenerateEndHook();
							}
							WriteLine(array2[xmlTypeMapMemberAnyElement.FlatArrayIndex] + "++;");
						}
						else if (!GenerateReadMemberHook(type, xmlTypeMapElementInfo.Member))
						{
							GenerateSetMemberValue(xmlTypeMapMemberAnyElement, ob, GetReadXmlNode(xmlTypeMapMemberAnyElement.TypeData, false), isValueList);
							GenerateEndHook();
						}
					}
					else
					{
						if (xmlTypeMapElementInfo.Member.GetType() != typeof(XmlTypeMapMemberElement))
						{
							throw new InvalidOperationException("Unknown member type");
						}
						if (!readByOrder)
						{
							WriteLine(array[xmlTypeMapElementInfo.Member.Index] + " = true;");
						}
						if (_format == SerializationFormat.Encoded)
						{
							string obTempVar4 = GetObTempVar();
							RegisterReferencingMap(xmlTypeMapElementInfo.MappedType);
							if (xmlTypeMapElementInfo.Member.TypeData.SchemaType != SchemaTypes.Primitive)
							{
								WriteLine("object " + obTempVar4 + " = ReadReferencingElement (out fixup.Ids[" + xmlTypeMapElementInfo.Member.Index + "]);");
							}
							else
							{
								WriteLine("object " + obTempVar4 + " = ReadReferencingElement (" + GetLiteral(xmlTypeMapElementInfo.Member.TypeData.XmlType) + ", " + GetLiteral("http://www.w3.org/2001/XMLSchema") + ", out fixup.Ids[" + xmlTypeMapElementInfo.Member.Index + "]);");
							}
							if (xmlTypeMapElementInfo.MultiReferenceType)
							{
								WriteLineInd("if (fixup.Ids[" + xmlTypeMapElementInfo.Member.Index + "] == null) {");
							}
							else
							{
								WriteLineInd("if (" + obTempVar4 + " != null) {");
							}
							GenerateSetMemberValue(xmlTypeMapElementInfo.Member, ob, GetCast(xmlTypeMapElementInfo.Member.TypeData, obTempVar4), isValueList);
							WriteLineUni("}");
						}
						else if (!GenerateReadMemberHook(type, xmlTypeMapElementInfo.Member))
						{
							if (xmlTypeMapElementInfo.ChoiceValue != null)
							{
								XmlTypeMapMemberElement xmlTypeMapMemberElement2 = (XmlTypeMapMemberElement)xmlTypeMapElementInfo.Member;
								WriteLine(ob + ".@" + xmlTypeMapMemberElement2.ChoiceMember + " = " + GetLiteral(xmlTypeMapElementInfo.ChoiceValue) + ";");
							}
							GenerateSetMemberValue(xmlTypeMapElementInfo.Member, ob, GenerateReadObjectElement(xmlTypeMapElementInfo), isValueList);
							GenerateEndHook();
						}
					}
					if (!readByOrder)
					{
						WriteLineUni("}");
					}
					else
					{
						WriteLine("Reader.MoveToContent();");
					}
					first = false;
				}
				if (!readByOrder)
				{
					if (!first)
					{
						WriteLineInd("else {");
					}
					if (map.DefaultAnyElementMember != null)
					{
						XmlTypeMapMemberAnyElement defaultAnyElementMember = map.DefaultAnyElementMember;
						if (defaultAnyElementMember.TypeData.IsListType)
						{
							if (!GenerateReadArrayMemberHook(type, defaultAnyElementMember, array2[defaultAnyElementMember.FlatArrayIndex]))
							{
								GenerateAddListValue(defaultAnyElementMember.TypeData, array3[defaultAnyElementMember.FlatArrayIndex], array2[defaultAnyElementMember.FlatArrayIndex], GetReadXmlNode(defaultAnyElementMember.TypeData.ListItemTypeData, false), true);
								GenerateEndHook();
							}
							WriteLine(array2[defaultAnyElementMember.FlatArrayIndex] + "++;");
						}
						else if (!GenerateReadMemberHook(type, defaultAnyElementMember))
						{
							GenerateSetMemberValue(defaultAnyElementMember, ob, GetReadXmlNode(defaultAnyElementMember.TypeData, false), isValueList);
							GenerateEndHook();
						}
					}
					else if (!GenerateReadHook(HookType.unknownElement, type))
					{
						WriteLine("UnknownNode (" + ob + ");");
						GenerateEndHook();
					}
					if (!first)
					{
						WriteLineUni("}");
					}
					WriteLineUni("}");
					if (map.XmlTextCollector != null)
					{
						WriteLine("else if (Reader.NodeType == System.Xml.XmlNodeType.Text || Reader.NodeType == System.Xml.XmlNodeType.CDATA)");
						WriteLineInd("{");
						if (map.XmlTextCollector is XmlTypeMapMemberExpandable)
						{
							XmlTypeMapMemberExpandable xmlTypeMapMemberExpandable = (XmlTypeMapMemberExpandable)map.XmlTextCollector;
							XmlTypeMapMemberFlatList xmlTypeMapMemberFlatList2 = xmlTypeMapMemberExpandable as XmlTypeMapMemberFlatList;
							TypeData typeData = ((xmlTypeMapMemberFlatList2 != null) ? xmlTypeMapMemberFlatList2.ListMap.FindTextElement().TypeData : xmlTypeMapMemberExpandable.TypeData.ListItemTypeData);
							if (!GenerateReadArrayMemberHook(type, map.XmlTextCollector, array2[xmlTypeMapMemberExpandable.FlatArrayIndex]))
							{
								string value = ((typeData.Type != typeof(string)) ? GetReadXmlNode(typeData, false) : "Reader.ReadString()");
								GenerateAddListValue(xmlTypeMapMemberExpandable.TypeData, array3[xmlTypeMapMemberExpandable.FlatArrayIndex], array2[xmlTypeMapMemberExpandable.FlatArrayIndex], value, true);
								GenerateEndHook();
							}
							WriteLine(array2[xmlTypeMapMemberExpandable.FlatArrayIndex] + "++;");
						}
						else if (!GenerateReadMemberHook(type, map.XmlTextCollector))
						{
							XmlTypeMapMemberElement xmlTypeMapMemberElement3 = (XmlTypeMapMemberElement)map.XmlTextCollector;
							XmlTypeMapElementInfo xmlTypeMapElementInfo2 = (XmlTypeMapElementInfo)xmlTypeMapMemberElement3.ElementInfo[0];
							if (xmlTypeMapElementInfo2.TypeData.Type == typeof(string))
							{
								GenerateSetMemberValue(xmlTypeMapMemberElement3, ob, "ReadString (" + GenerateGetMemberValue(xmlTypeMapMemberElement3, ob, isValueList) + ")", isValueList);
							}
							else
							{
								WriteLineInd("{");
								string strTempVar = GetStrTempVar();
								WriteLine("string " + strTempVar + " = Reader.ReadString();");
								GenerateSetMemberValue(xmlTypeMapMemberElement3, ob, GenerateGetValueFromXmlString(strTempVar, xmlTypeMapElementInfo2.TypeData, xmlTypeMapElementInfo2.MappedType, xmlTypeMapElementInfo2.IsNullable), isValueList);
								WriteLineUni("}");
							}
							GenerateEndHook();
						}
						WriteLineUni("}");
					}
					WriteLine("else");
					WriteLine("\tUnknownNode(" + ob + ");");
					WriteLine(string.Empty);
					WriteLine("Reader.MoveToContent();");
					WriteLineUni("}");
				}
				else
				{
					WriteLine("Reader.MoveToContent();");
				}
				if (array3 != null)
				{
					WriteLine(string.Empty);
					foreach (XmlTypeMapMemberExpandable flatList in map.FlatLists)
					{
						if (!MemberHasReadReplaceHook(type, flatList))
						{
							string text6 = array3[flatList.FlatArrayIndex];
							if (flatList.TypeData.Type.IsArray)
							{
								WriteLine(text6 + " = (" + flatList.TypeData.CSharpFullName + ") ShrinkArray (" + text6 + ", " + array2[flatList.FlatArrayIndex] + ", " + GetTypeOf(flatList.TypeData.Type.GetElementType()) + ", true);");
							}
							if (!IsReadOnly(xmlTypeMapping, flatList, flatList.TypeData, isValueList) && flatList.TypeData.Type.IsArray)
							{
								GenerateSetMemberValue(flatList, ob, text6, isValueList);
							}
						}
					}
				}
				if (array4 != null)
				{
					WriteLine(string.Empty);
					foreach (XmlTypeMapMemberExpandable flatList2 in map.FlatLists)
					{
						if (!MemberHasReadReplaceHook(type, flatList2) && flatList2.ChoiceMember != null)
						{
							string text7 = array4[flatList2.FlatArrayIndex];
							WriteLine(text7 + " = (" + flatList2.ChoiceTypeData.CSharpFullName + ") ShrinkArray (" + text7 + ", " + array2[flatList2.FlatArrayIndex] + ", " + GetTypeOf(flatList2.ChoiceTypeData.Type.GetElementType()) + ", true);");
							WriteLine(ob + ".@" + flatList2.ChoiceMember + " = " + text7 + ";");
						}
					}
				}
				GenerateSetListMembersDefaults(xmlTypeMapping, map, ob, isValueList);
				GenerateEndHook();
			}
			if (!isValueList)
			{
				WriteLine(string.Empty);
				WriteLine("ReadEndElement();");
			}
		}

		private void GenerateReadAttributeMembers(XmlMapping xmlMap, ClassMap map, string ob, bool isValueList, ref bool first)
		{
			XmlTypeMapping xmlTypeMapping = xmlMap as XmlTypeMapping;
			Type type = ((xmlTypeMapping == null) ? typeof(object[]) : xmlTypeMapping.TypeData.Type);
			if (GenerateReadHook(HookType.attributes, type))
			{
				return;
			}
			XmlTypeMapMember defaultAnyAttributeMember = map.DefaultAnyAttributeMember;
			if (defaultAnyAttributeMember != null)
			{
				WriteLine("int anyAttributeIndex = 0;");
				WriteLine(defaultAnyAttributeMember.TypeData.CSharpFullName + " anyAttributeArray = null;");
			}
			WriteLine("while (Reader.MoveToNextAttribute())");
			WriteLineInd("{");
			first = true;
			if (map.AttributeMembers != null)
			{
				foreach (XmlTypeMapMemberAttribute attributeMember in map.AttributeMembers)
				{
					WriteLineInd(((!first) ? "else " : string.Empty) + "if (Reader.LocalName == " + GetLiteral(attributeMember.AttributeName) + " && Reader.NamespaceURI == " + GetLiteral(attributeMember.Namespace) + ") {");
					if (!GenerateReadMemberHook(type, attributeMember))
					{
						GenerateSetMemberValue(attributeMember, ob, GenerateGetValueFromXmlString("Reader.Value", attributeMember.TypeData, attributeMember.MappedType, false), isValueList);
						GenerateEndHook();
					}
					WriteLineUni("}");
					first = false;
				}
			}
			WriteLineInd(((!first) ? "else " : string.Empty) + "if (IsXmlnsAttribute (Reader.Name)) {");
			if (map.NamespaceDeclarations != null && !GenerateReadMemberHook(type, map.NamespaceDeclarations))
			{
				string text = ob + ".@" + map.NamespaceDeclarations.Name;
				WriteLine("if (" + text + " == null) " + text + " = new XmlSerializerNamespaces ();");
				WriteLineInd("if (Reader.Prefix == \"xmlns\")");
				WriteLine(text + ".Add (Reader.LocalName, Reader.Value);");
				Unindent();
				WriteLineInd("else");
				WriteLine(text + ".Add (\"\", Reader.Value);");
				Unindent();
				GenerateEndHook();
			}
			WriteLineUni("}");
			WriteLineInd("else {");
			if (defaultAnyAttributeMember != null)
			{
				if (!GenerateReadArrayMemberHook(type, defaultAnyAttributeMember, "anyAttributeIndex"))
				{
					WriteLine("System.Xml.XmlAttribute attr = (System.Xml.XmlAttribute) Document.ReadNode(Reader);");
					if (typeof(XmlSchemaAnnotated).IsAssignableFrom(type))
					{
						WriteLine("ParseWsdlArrayType (attr);");
					}
					GenerateAddListValue(defaultAnyAttributeMember.TypeData, "anyAttributeArray", "anyAttributeIndex", GetCast(defaultAnyAttributeMember.TypeData.ListItemTypeData, "attr"), true);
					GenerateEndHook();
				}
				WriteLine("anyAttributeIndex++;");
			}
			else if (!GenerateReadHook(HookType.unknownAttribute, type))
			{
				WriteLine("UnknownNode (" + ob + ");");
				GenerateEndHook();
			}
			WriteLineUni("}");
			WriteLineUni("}");
			if (defaultAnyAttributeMember != null && !MemberHasReadReplaceHook(type, defaultAnyAttributeMember))
			{
				WriteLine(string.Empty);
				WriteLine("anyAttributeArray = (" + defaultAnyAttributeMember.TypeData.CSharpFullName + ") ShrinkArray (anyAttributeArray, anyAttributeIndex, " + GetTypeOf(defaultAnyAttributeMember.TypeData.Type.GetElementType()) + ", true);");
				GenerateSetMemberValue(defaultAnyAttributeMember, ob, "anyAttributeArray", isValueList);
			}
			WriteLine(string.Empty);
			WriteLine("Reader.MoveToElement ();");
			GenerateEndHook();
		}

		private void GenerateSetListMembersDefaults(XmlTypeMapping typeMap, ClassMap map, string ob, bool isValueList)
		{
			if (map.ListMembers == null)
			{
				return;
			}
			ArrayList listMembers = map.ListMembers;
			for (int i = 0; i < listMembers.Count; i++)
			{
				XmlTypeMapMember xmlTypeMapMember = (XmlTypeMapMember)listMembers[i];
				if (!IsReadOnly(typeMap, xmlTypeMapMember, xmlTypeMapMember.TypeData, isValueList))
				{
					WriteLineInd("if (" + GenerateGetMemberValue(xmlTypeMapMember, ob, isValueList) + " == null) {");
					GenerateSetMemberValue(xmlTypeMapMember, ob, GenerateInitializeList(xmlTypeMapMember.TypeData), isValueList);
					WriteLineUni("}");
				}
			}
		}

		private bool IsReadOnly(XmlTypeMapping map, XmlTypeMapMember member, TypeData memType, bool isValueList)
		{
			if (isValueList)
			{
				return !memType.HasPublicConstructor;
			}
			return member.IsReadOnly(map.TypeData.Type) || !memType.HasPublicConstructor;
		}

		private void GenerateSetMemberValue(XmlTypeMapMember member, string ob, string value, bool isValueList)
		{
			if (isValueList)
			{
				WriteLine(ob + "[" + member.GlobalIndex + "] = " + value + ";");
				return;
			}
			WriteLine(ob + ".@" + member.Name + " = " + value + ";");
			if (member.IsOptionalValueType)
			{
				WriteLine(ob + "." + member.Name + "Specified = true;");
			}
		}

		private void GenerateSetMemberValueFromAttr(XmlTypeMapMember member, string ob, string value, bool isValueList)
		{
			if (member.TypeData.Type.IsEnum)
			{
				value = GetCast(member.TypeData.Type, value);
			}
			GenerateSetMemberValue(member, ob, value, isValueList);
		}

		private string GenerateReadObjectElement(XmlTypeMapElementInfo elem)
		{
			switch (elem.TypeData.SchemaType)
			{
			case SchemaTypes.XmlNode:
				return GetReadXmlNode(elem.TypeData, true);
			case SchemaTypes.Primitive:
			case SchemaTypes.Enum:
				return GenerateReadPrimitiveValue(elem);
			case SchemaTypes.Array:
				return GenerateReadListElement(elem.MappedType, null, GetLiteral(elem.IsNullable), true);
			case SchemaTypes.Class:
				return GetReadObjectCall(elem.MappedType, GetLiteral(elem.IsNullable), "true");
			case SchemaTypes.XmlSerializable:
				return GetCast(elem.TypeData, string.Format("({0}) ReadSerializable (({0}) Activator.CreateInstance(typeof({0}), true))", elem.TypeData.CSharpFullName));
			default:
				throw new NotSupportedException("Invalid value type");
			}
		}

		private string GenerateReadPrimitiveValue(XmlTypeMapElementInfo elem)
		{
			if (elem.TypeData.Type == typeof(XmlQualifiedName))
			{
				if (elem.IsNullable)
				{
					return "ReadNullableQualifiedName ()";
				}
				return "ReadElementQualifiedName ()";
			}
			if (elem.IsNullable)
			{
				string strTempVar = GetStrTempVar();
				WriteLine("string " + strTempVar + " = ReadNullableString ();");
				return GenerateGetValueFromXmlString(strTempVar, elem.TypeData, elem.MappedType, true);
			}
			string strTempVar2 = GetStrTempVar();
			WriteLine("string " + strTempVar2 + " = Reader.ReadElementString ();");
			return GenerateGetValueFromXmlString(strTempVar2, elem.TypeData, elem.MappedType, false);
		}

		private string GenerateGetValueFromXmlString(string value, TypeData typeData, XmlTypeMapping typeMap, bool isNullable)
		{
			if (typeData.SchemaType == SchemaTypes.Array)
			{
				return GenerateReadListString(typeMap, value);
			}
			if (typeData.SchemaType == SchemaTypes.Enum)
			{
				return GenerateGetEnumValue(typeMap, value, isNullable);
			}
			if (typeData.Type == typeof(XmlQualifiedName))
			{
				return "ToXmlQualifiedName (" + value + ")";
			}
			return XmlCustomFormatter.GenerateFromXmlString(typeData, value);
		}

		private string GenerateReadListElement(XmlTypeMapping typeMap, string list, string isNullable, bool canCreateInstance)
		{
			Type type = typeMap.TypeData.Type;
			ListMap listMap = (ListMap)typeMap.ObjectMap;
			bool flag = typeMap.TypeData.Type.IsArray;
			if (canCreateInstance && typeMap.TypeData.HasPublicConstructor)
			{
				if (list == null)
				{
					list = GetObTempVar();
					WriteLine(typeMap.TypeData.CSharpFullName + " " + list + " = null;");
					if (flag)
					{
						WriteLineInd("if (!ReadNull()) {");
					}
					WriteLine(list + " = " + GenerateCreateList(type) + ";");
				}
				else if (flag)
				{
					WriteLineInd("if (!ReadNull()) {");
				}
			}
			else
			{
				if (list == null)
				{
					WriteLine("throw CreateReadOnlyCollectionException (" + GetLiteral(typeMap.TypeData.CSharpFullName) + ");");
					return list;
				}
				WriteLineInd("if (((object)" + list + ") == null)");
				WriteLine("throw CreateReadOnlyCollectionException (" + GetLiteral(typeMap.TypeData.CSharpFullName) + ");");
				Unindent();
				flag = false;
			}
			WriteLineInd("if (Reader.IsEmptyElement) {");
			WriteLine("Reader.Skip();");
			if (type.IsArray)
			{
				WriteLine(list + " = (" + typeMap.TypeData.CSharpFullName + ") ShrinkArray (" + list + ", 0, " + GetTypeOf(type.GetElementType()) + ", false);");
			}
			Unindent();
			WriteLineInd("} else {");
			string numTempVar = GetNumTempVar();
			WriteLine("int " + numTempVar + " = 0;");
			WriteLine("Reader.ReadStartElement();");
			WriteLine("Reader.MoveToContent();");
			WriteLine(string.Empty);
			WriteLine("while (Reader.NodeType != System.Xml.XmlNodeType.EndElement) ");
			WriteLineInd("{");
			WriteLine("if (Reader.NodeType == System.Xml.XmlNodeType.Element) ");
			WriteLineInd("{");
			bool flag2 = true;
			foreach (XmlTypeMapElementInfo item in listMap.ItemInfo)
			{
				WriteLineInd(((!flag2) ? "else " : string.Empty) + "if (Reader.LocalName == " + GetLiteral(item.ElementName) + " && Reader.NamespaceURI == " + GetLiteral(item.Namespace) + ") {");
				GenerateAddListValue(typeMap.TypeData, list, numTempVar, GenerateReadObjectElement(item), false);
				WriteLine(numTempVar + "++;");
				WriteLineUni("}");
				flag2 = false;
			}
			if (!flag2)
			{
				WriteLine("else UnknownNode (null);");
			}
			else
			{
				WriteLine("UnknownNode (null);");
			}
			WriteLineUni("}");
			WriteLine("else UnknownNode (null);");
			WriteLine(string.Empty);
			WriteLine("Reader.MoveToContent();");
			WriteLineUni("}");
			WriteLine("ReadEndElement();");
			if (type.IsArray)
			{
				WriteLine(list + " = (" + typeMap.TypeData.CSharpFullName + ") ShrinkArray (" + list + ", " + numTempVar + ", " + GetTypeOf(type.GetElementType()) + ", false);");
			}
			WriteLineUni("}");
			if (flag)
			{
				WriteLineUni("}");
			}
			return list;
		}

		private string GenerateReadListString(XmlTypeMapping typeMap, string values)
		{
			Type type = typeMap.TypeData.Type;
			ListMap listMap = (ListMap)typeMap.ObjectMap;
			string text = ToCSharpFullName(type.GetElementType());
			string obTempVar = GetObTempVar();
			WriteLine(text + "[] " + obTempVar + ";");
			string strTempVar = GetStrTempVar();
			WriteLine("string " + strTempVar + " = " + values + ".Trim();");
			WriteLineInd("if (" + strTempVar + " != string.Empty) {");
			string obTempVar2 = GetObTempVar();
			WriteLine("string[] " + obTempVar2 + " = " + strTempVar + ".Split (' ');");
			WriteLine(obTempVar + " = new " + GetArrayDeclaration(type, obTempVar2 + ".Length") + ";");
			XmlTypeMapElementInfo xmlTypeMapElementInfo = (XmlTypeMapElementInfo)listMap.ItemInfo[0];
			string numTempVar = GetNumTempVar();
			WriteLineInd("for (int " + numTempVar + " = 0; " + numTempVar + " < " + obTempVar2 + ".Length; " + numTempVar + "++)");
			WriteLine(obTempVar + "[" + numTempVar + "] = " + GenerateGetValueFromXmlString(obTempVar2 + "[" + numTempVar + "]", xmlTypeMapElementInfo.TypeData, xmlTypeMapElementInfo.MappedType, xmlTypeMapElementInfo.IsNullable) + ";");
			Unindent();
			WriteLineUni("}");
			WriteLine("else");
			WriteLine("\t" + obTempVar + " = new " + GetArrayDeclaration(type, "0") + ";");
			return obTempVar;
		}

		private string GetArrayDeclaration(Type type, string length)
		{
			Type elementType = type.GetElementType();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[').Append(length).Append(']');
			while (elementType.IsArray)
			{
				stringBuilder.Append("[]");
				elementType = elementType.GetElementType();
			}
			stringBuilder.Insert(0, ToCSharpFullName(elementType));
			return stringBuilder.ToString();
		}

		private void GenerateAddListValue(TypeData listType, string list, string index, string value, bool canCreateInstance)
		{
			Type type = listType.Type;
			if (type.IsArray)
			{
				WriteLine(list + " = (" + ToCSharpFullName(type) + ") EnsureArrayIndex (" + list + ", " + index + ", " + GetTypeOf(type.GetElementType()) + ");");
				WriteLine(list + "[" + index + "] = " + value + ";");
			}
			else
			{
				WriteLine("if (((object)" + list + ") == null)");
				if (canCreateInstance)
				{
					WriteLine("\t" + list + string.Format(" = ({0}) Activator.CreateInstance(typeof({0}), true);", listType.CSharpFullName));
				}
				else
				{
					WriteLine("\tthrow CreateReadOnlyCollectionException (" + GetLiteral(listType.CSharpFullName) + ");");
				}
				WriteLine(list + ".Add (" + value + ");");
			}
		}

		private string GenerateCreateList(Type listType)
		{
			if (listType.IsArray)
			{
				return "(" + ToCSharpFullName(listType) + ") EnsureArrayIndex (null, 0, " + GetTypeOf(listType.GetElementType()) + ")";
			}
			return "new " + ToCSharpFullName(listType) + "()";
		}

		private string GenerateInitializeList(TypeData listType)
		{
			if (listType.Type.IsArray)
			{
				return "null";
			}
			return "new " + listType.CSharpFullName + "()";
		}

		private void GenerateFillerCallbacks()
		{
			foreach (TypeData item in _listsToFill)
			{
				string fillListName = GetFillListName(item);
				WriteLine("void " + fillListName + " (object list, object source)");
				WriteLineInd("{");
				WriteLine("if (((object)list) == null) throw CreateReadOnlyCollectionException (" + GetLiteral(item.CSharpFullName) + ");");
				WriteLine(string.Empty);
				WriteLine(item.CSharpFullName + " dest = (" + item.CSharpFullName + ") list;");
				WriteLine("foreach (object ob in (IEnumerable)source)");
				WriteLine("\tdest.Add (" + GetCast(item.ListItemTypeData, "ob") + ");");
				WriteLineUni("}");
				WriteLine(string.Empty);
			}
		}

		private void GenerateReadXmlNodeElement(XmlTypeMapping typeMap, string isNullable)
		{
			WriteLine("return " + GetReadXmlNode(typeMap.TypeData, false) + ";");
		}

		private void GenerateReadPrimitiveElement(XmlTypeMapping typeMap, string isNullable)
		{
			WriteLine("XmlQualifiedName t = GetXsiType();");
			WriteLine("if (t == null) t = new XmlQualifiedName (" + GetLiteral(typeMap.XmlType) + ", " + GetLiteral(typeMap.Namespace) + ");");
			WriteLine("return " + GetCast(typeMap.TypeData, "ReadTypedPrimitive (t)") + ";");
		}

		private void GenerateReadEnumElement(XmlTypeMapping typeMap, string isNullable)
		{
			WriteLine("Reader.ReadStartElement ();");
			WriteLine(typeMap.TypeData.CSharpFullName + " res = " + GenerateGetEnumValue(typeMap, "Reader.ReadString()", false) + ";");
			WriteLineInd("if (Reader.NodeType != XmlNodeType.None)");
			WriteLineUni("Reader.ReadEndElement ();");
			WriteLine("return res;");
		}

		private string GenerateGetEnumValue(XmlTypeMapping typeMap, string val, bool isNullable)
		{
			if (isNullable)
			{
				return "(" + val + ") != null ? " + GetGetEnumValueName(typeMap) + " (" + val + ") : (" + typeMap.TypeData.CSharpFullName + "?) null";
			}
			return GetGetEnumValueName(typeMap) + " (" + val + ")";
		}

		private void GenerateGetEnumValueMethod(XmlTypeMapping typeMap)
		{
			string text = GetGetEnumValueName(typeMap);
			EnumMap enumMap = (EnumMap)typeMap.ObjectMap;
			if (enumMap.IsFlags)
			{
				string text2 = text + "_Switch";
				WriteLine(typeMap.TypeData.CSharpFullName + " " + text + " (string xmlName)");
				WriteLineInd("{");
				WriteLine("xmlName = xmlName.Trim();");
				WriteLine("if (xmlName.Length == 0) return (" + typeMap.TypeData.CSharpFullName + ")0;");
				WriteLine(typeMap.TypeData.CSharpFullName + " sb = (" + typeMap.TypeData.CSharpFullName + ")0;");
				WriteLine("string[] enumNames = xmlName.Split (null);");
				WriteLine("foreach (string name in enumNames)");
				WriteLineInd("{");
				WriteLine("if (name == string.Empty) continue;");
				WriteLine("sb |= " + text2 + " (name); ");
				WriteLineUni("}");
				WriteLine("return sb;");
				WriteLineUni("}");
				WriteLine(string.Empty);
				text = text2;
			}
			WriteLine(typeMap.TypeData.CSharpFullName + " " + text + " (string xmlName)");
			WriteLineInd("{");
			GenerateGetSingleEnumValue(typeMap, "xmlName");
			WriteLineUni("}");
			WriteLine(string.Empty);
		}

		private void GenerateGetSingleEnumValue(XmlTypeMapping typeMap, string val)
		{
			EnumMap enumMap = (EnumMap)typeMap.ObjectMap;
			WriteLine("switch (" + val + ")");
			WriteLineInd("{");
			EnumMap.EnumMapMember[] members = enumMap.Members;
			foreach (EnumMap.EnumMapMember enumMapMember in members)
			{
				WriteLine("case " + GetLiteral(enumMapMember.XmlName) + ": return " + typeMap.TypeData.CSharpFullName + ".@" + enumMapMember.EnumName + ";");
			}
			WriteLineInd("default:");
			WriteLine("throw CreateUnknownConstantException (" + val + ", typeof(" + typeMap.TypeData.CSharpFullName + "));");
			Unindent();
			WriteLineUni("}");
		}

		private void GenerateReadXmlSerializableElement(XmlTypeMapping typeMap, string isNullable)
		{
			WriteLine("Reader.MoveToContent ();");
			WriteLine("if (Reader.NodeType == XmlNodeType.Element)");
			WriteLineInd("{");
			WriteLine("if (Reader.LocalName == " + GetLiteral(typeMap.ElementName) + " && Reader.NamespaceURI == " + GetLiteral(typeMap.Namespace) + ")");
			WriteLine(string.Format("\treturn ({0}) ReadSerializable (({0}) Activator.CreateInstance(typeof({0}), true));", typeMap.TypeData.CSharpFullName));
			WriteLine("else");
			WriteLine("\tthrow CreateUnknownNodeException ();");
			WriteLineUni("}");
			WriteLine("else UnknownNode (null);");
			WriteLine(string.Empty);
			WriteLine("return null;");
		}

		private void GenerateReadInitCallbacks()
		{
			WriteLine("protected override void InitCallbacks ()");
			WriteLineInd("{");
			if (_format == SerializationFormat.Encoded)
			{
				foreach (XmlMapping item in _mapsToGenerate)
				{
					XmlTypeMapping xmlTypeMapping = item as XmlTypeMapping;
					if (xmlTypeMapping != null && (xmlTypeMapping.TypeData.SchemaType == SchemaTypes.Class || xmlTypeMapping.TypeData.SchemaType == SchemaTypes.Enum))
					{
						WriteMetCall("AddReadCallback", GetLiteral(xmlTypeMapping.XmlType), GetLiteral(xmlTypeMapping.Namespace), GetTypeOf(xmlTypeMapping.TypeData.Type), "new XmlSerializationReadCallback (" + GetReadObjectName(xmlTypeMapping) + ")");
					}
				}
			}
			WriteLineUni("}");
			WriteLine(string.Empty);
			WriteLine("protected override void InitIDs ()");
			WriteLine("{");
			WriteLine("}");
			WriteLine(string.Empty);
		}

		private void GenerateFixupCallbacks()
		{
			foreach (XmlMapping fixupCallback in _fixupCallbacks)
			{
				bool flag = fixupCallback is XmlMembersMapping;
				string text = (flag ? "object[]" : ((XmlTypeMapping)fixupCallback).TypeData.CSharpFullName);
				WriteLine("void " + GetFixupCallbackName(fixupCallback) + " (object obfixup)");
				WriteLineInd("{");
				WriteLine("Fixup fixup = (Fixup)obfixup;");
				WriteLine(text + " source = (" + text + ") fixup.Source;");
				WriteLine("string[] ids = fixup.Ids;");
				WriteLine(string.Empty);
				ClassMap classMap = (ClassMap)fixupCallback.ObjectMap;
				ICollection elementMembers = classMap.ElementMembers;
				if (elementMembers != null)
				{
					foreach (XmlTypeMapMember item in elementMembers)
					{
						WriteLineInd("if (ids[" + item.Index + "] != null)");
						string text2 = "GetTarget(ids[" + item.Index + "])";
						if (!flag)
						{
							text2 = GetCast(item.TypeData, text2);
						}
						GenerateSetMemberValue(item, "source", text2, flag);
						Unindent();
					}
				}
				WriteLineUni("}");
				WriteLine(string.Empty);
			}
		}

		private string GetReadXmlNode(TypeData type, bool wrapped)
		{
			if (type.Type == typeof(XmlDocument))
			{
				return GetCast(type, TypeTranslator.GetTypeData(typeof(XmlDocument)), "ReadXmlDocument (" + GetLiteral(wrapped) + ")");
			}
			return GetCast(type, TypeTranslator.GetTypeData(typeof(XmlNode)), "ReadXmlNode (" + GetLiteral(wrapped) + ")");
		}

		private void InitHooks()
		{
			_hookContexts = new Stack();
			_hookOpenHooks = new Stack();
			_hookVariables = new Hashtable();
		}

		private void PushHookContext()
		{
			_hookContexts.Push(_hookVariables);
			_hookVariables = (Hashtable)_hookVariables.Clone();
		}

		private void PopHookContext()
		{
			_hookVariables = (Hashtable)_hookContexts.Pop();
		}

		private void SetHookVar(string var, string value)
		{
			_hookVariables[var] = value;
		}

		private bool GenerateReadHook(HookType hookType, Type type)
		{
			return GenerateHook(hookType, XmlMappingAccess.Read, type, null);
		}

		private bool GenerateWriteHook(HookType hookType, Type type)
		{
			return GenerateHook(hookType, XmlMappingAccess.Write, type, null);
		}

		private bool GenerateWriteMemberHook(Type type, XmlTypeMapMember member)
		{
			SetHookVar("$MEMBER", member.Name);
			return GenerateHook(HookType.member, XmlMappingAccess.Write, type, member.Name);
		}

		private bool GenerateReadMemberHook(Type type, XmlTypeMapMember member)
		{
			SetHookVar("$MEMBER", member.Name);
			return GenerateHook(HookType.member, XmlMappingAccess.Read, type, member.Name);
		}

		private bool GenerateReadArrayMemberHook(Type type, XmlTypeMapMember member, string index)
		{
			SetHookVar("$INDEX", index);
			return GenerateReadMemberHook(type, member);
		}

		private bool MemberHasReadReplaceHook(Type type, XmlTypeMapMember member)
		{
			if (_config == null)
			{
				return false;
			}
			return _config.GetHooks(HookType.member, XmlMappingAccess.Read, HookAction.Replace, type, member.Name).Count > 0;
		}

		private bool GenerateHook(HookType hookType, XmlMappingAccess dir, Type type, string member)
		{
			GenerateHooks(hookType, dir, type, null, HookAction.InsertBefore);
			if (GenerateHooks(hookType, dir, type, null, HookAction.Replace))
			{
				GenerateHooks(hookType, dir, type, null, HookAction.InsertAfter);
				return true;
			}
			HookInfo hookInfo = new HookInfo();
			hookInfo.HookType = hookType;
			hookInfo.Type = type;
			hookInfo.Member = member;
			hookInfo.Direction = dir;
			_hookOpenHooks.Push(hookInfo);
			return false;
		}

		private void GenerateEndHook()
		{
			HookInfo hookInfo = (HookInfo)_hookOpenHooks.Pop();
			GenerateHooks(hookInfo.HookType, hookInfo.Direction, hookInfo.Type, hookInfo.Member, HookAction.InsertAfter);
		}

		private bool GenerateHooks(HookType hookType, XmlMappingAccess dir, Type type, string member, HookAction action)
		{
			if (_config == null)
			{
				return false;
			}
			ArrayList hooks = _config.GetHooks(hookType, dir, action, type, null);
			if (hooks.Count == 0)
			{
				return false;
			}
			foreach (Hook item in hooks)
			{
				string text = item.GetCode(action);
				foreach (DictionaryEntry hookVariable in _hookVariables)
				{
					text = text.Replace((string)hookVariable.Key, (string)hookVariable.Value);
				}
				WriteMultilineCode(text);
			}
			return true;
		}

		private string GetRootTypeName()
		{
			if (_typeMap is XmlTypeMapping)
			{
				return ((XmlTypeMapping)_typeMap).TypeData.CSharpFullName;
			}
			return "object[]";
		}

		private string GetNumTempVar()
		{
			return "n" + _tempVarId++;
		}

		private string GetObTempVar()
		{
			return "o" + _tempVarId++;
		}

		private string GetStrTempVar()
		{
			return "s" + _tempVarId++;
		}

		private string GetBoolTempVar()
		{
			return "b" + _tempVarId++;
		}

		private string GetUniqueName(string uniqueGroup, object ob, string name)
		{
			name = name.Replace("[]", "_array");
			Hashtable hashtable = (Hashtable)_uniqueNames[uniqueGroup];
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				_uniqueNames[uniqueGroup] = hashtable;
			}
			string text = (string)hashtable[ob];
			if (text != null)
			{
				return text;
			}
			foreach (string value in hashtable.Values)
			{
				if (value == name)
				{
					return GetUniqueName(uniqueGroup, ob, name + _methodId++);
				}
			}
			hashtable[ob] = name;
			return name;
		}

		private void RegisterReferencingMap(XmlTypeMapping typeMap)
		{
			if (typeMap != null && !_mapsToGenerate.Contains(typeMap))
			{
				_mapsToGenerate.Add(typeMap);
			}
		}

		private string GetWriteObjectName(XmlTypeMapping typeMap)
		{
			if (!_mapsToGenerate.Contains(typeMap))
			{
				_mapsToGenerate.Add(typeMap);
			}
			return GetUniqueName("rw", typeMap, "WriteObject_" + typeMap.XmlType);
		}

		private string GetReadObjectName(XmlTypeMapping typeMap)
		{
			if (!_mapsToGenerate.Contains(typeMap))
			{
				_mapsToGenerate.Add(typeMap);
			}
			return GetUniqueName("rr", typeMap, "ReadObject_" + typeMap.XmlType);
		}

		private string GetGetEnumValueName(XmlTypeMapping typeMap)
		{
			if (!_mapsToGenerate.Contains(typeMap))
			{
				_mapsToGenerate.Add(typeMap);
			}
			return GetUniqueName("ge", typeMap, "GetEnumValue_" + typeMap.XmlType);
		}

		private string GetWriteObjectCallbackName(XmlTypeMapping typeMap)
		{
			if (!_mapsToGenerate.Contains(typeMap))
			{
				_mapsToGenerate.Add(typeMap);
			}
			return GetUniqueName("wc", typeMap, "WriteCallback_" + typeMap.XmlType);
		}

		private string GetFixupCallbackName(XmlMapping typeMap)
		{
			if (!_mapsToGenerate.Contains(typeMap))
			{
				_mapsToGenerate.Add(typeMap);
			}
			if (typeMap is XmlTypeMapping)
			{
				return GetUniqueName("fc", typeMap, "FixupCallback_" + ((XmlTypeMapping)typeMap).XmlType);
			}
			return GetUniqueName("fc", typeMap, "FixupCallback__Message");
		}

		private string GetUniqueClassName(string s)
		{
			return classNames.AddUnique(s, null);
		}

		private string GetReadObjectCall(XmlTypeMapping typeMap, string isNullable, string checkType)
		{
			if (_format == SerializationFormat.Literal)
			{
				return GetReadObjectName(typeMap) + " (" + isNullable + ", " + checkType + ")";
			}
			return GetCast(typeMap.TypeData, GetReadObjectName(typeMap) + " ()");
		}

		private string GetFillListName(TypeData td)
		{
			if (!_listsToFill.Contains(td))
			{
				_listsToFill.Add(td);
			}
			return GetUniqueName("fl", td, "Fill_" + CodeIdentifier.MakeValid(td.CSharpName));
		}

		private string GetCast(TypeData td, TypeData tdval, string val)
		{
			if (td.CSharpFullName == tdval.CSharpFullName)
			{
				return val;
			}
			return GetCast(td, val);
		}

		private string GetCast(TypeData td, string val)
		{
			return "((" + td.CSharpFullName + ") " + val + ")";
		}

		private string GetCast(Type td, string val)
		{
			return "((" + ToCSharpFullName(td) + ") " + val + ")";
		}

		private string GetTypeOf(TypeData td)
		{
			return "typeof(" + td.CSharpFullName + ")";
		}

		private string GetTypeOf(Type td)
		{
			return "typeof(" + ToCSharpFullName(td) + ")";
		}

		private string GetLiteral(object ob)
		{
			if (ob == null)
			{
				return "null";
			}
			if (ob is string)
			{
				return "\"" + ob.ToString().Replace("\"", "\"\"") + "\"";
			}
			if (ob is DateTime)
			{
				return "new DateTime (" + ((DateTime)ob).Ticks + ")";
			}
			if (ob is DateTimeOffset)
			{
				return "new DateTimeOffset (" + ((DateTimeOffset)ob).Ticks + ")";
			}
			if (ob is TimeSpan)
			{
				return "new TimeSpan (" + ((TimeSpan)ob).Ticks + ")";
			}
			if (ob is bool)
			{
				return (!(bool)ob) ? "false" : "true";
			}
			if (ob is XmlQualifiedName)
			{
				XmlQualifiedName xmlQualifiedName = (XmlQualifiedName)ob;
				return "new XmlQualifiedName (" + GetLiteral(xmlQualifiedName.Name) + "," + GetLiteral(xmlQualifiedName.Namespace) + ")";
			}
			if (ob is Enum)
			{
				string value = ToCSharpFullName(ob.GetType());
				StringBuilder stringBuilder = new StringBuilder();
				string text = Enum.Format(ob.GetType(), ob, "g");
				string[] array = text.Split(',');
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					string text3 = text2.Trim();
					if (text3.Length != 0)
					{
						if (stringBuilder.Length > 0)
						{
							stringBuilder.Append(" | ");
						}
						stringBuilder.Append(value);
						stringBuilder.Append('.');
						stringBuilder.Append(text3);
					}
				}
				return stringBuilder.ToString();
			}
			return (!(ob is IFormattable)) ? ob.ToString() : ((IFormattable)ob).ToString(null, CultureInfo.InvariantCulture);
		}

		private void WriteLineInd(string code)
		{
			WriteLine(code);
			_indent++;
		}

		private void WriteLineUni(string code)
		{
			if (_indent > 0)
			{
				_indent--;
			}
			WriteLine(code);
		}

		private void Write(string code)
		{
			if (code.Length > 0)
			{
				_writer.Write(new string('\t', _indent));
			}
			_writer.Write(code);
		}

		private void WriteUni(string code)
		{
			if (_indent > 0)
			{
				_indent--;
			}
			_writer.Write(code);
			_writer.WriteLine(string.Empty);
		}

		private void WriteLine(string code)
		{
			if (code.Length > 0)
			{
				_writer.Write(new string('\t', _indent));
			}
			_writer.WriteLine(code);
		}

		private void WriteMultilineCode(string code)
		{
			string text = new string('\t', _indent);
			code = code.Replace("\r", string.Empty);
			code = code.Replace("\t", string.Empty);
			while (code.StartsWith("\n"))
			{
				code = code.Substring(1);
			}
			while (code.EndsWith("\n"))
			{
				code = code.Substring(0, code.Length - 1);
			}
			code = code.Replace("\n", "\n" + text);
			WriteLine(code);
		}

		private string Params(params string[] pars)
		{
			string text = string.Empty;
			foreach (string text2 in pars)
			{
				if (text != string.Empty)
				{
					text += ", ";
				}
				text += text2;
			}
			return text;
		}

		private void WriteMetCall(string method, params string[] pars)
		{
			WriteLine(method + " (" + Params(pars) + ");");
		}

		private void Unindent()
		{
			_indent--;
		}
	}
}
