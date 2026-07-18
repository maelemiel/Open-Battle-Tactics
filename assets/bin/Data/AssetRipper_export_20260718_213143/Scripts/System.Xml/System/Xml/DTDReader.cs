using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using Mono.Xml;

namespace System.Xml
{
	internal class DTDReader : IXmlLineInfo
	{
		private const int initialNameCapacity = 256;

		private XmlParserInput currentInput;

		private Stack parserInputStack;

		private char[] nameBuffer;

		private int nameLength;

		private int nameCapacity;

		private StringBuilder valueBuffer;

		private int currentLinkedNodeLineNumber;

		private int currentLinkedNodeLinePosition;

		private int dtdIncludeSect;

		private bool normalization;

		private bool processingInternalSubset;

		private string cachedPublicId;

		private string cachedSystemId;

		private DTDObjectModel DTD;

		public string BaseURI
		{
			get
			{
				return currentInput.BaseURI;
			}
		}

		public bool Normalization
		{
			get
			{
				return normalization;
			}
			set
			{
				normalization = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return currentInput.LineNumber;
			}
		}

		public int LinePosition
		{
			get
			{
				return currentInput.LinePosition;
			}
		}

		public DTDReader(DTDObjectModel dtd, int startLineNumber, int startLinePosition)
		{
			DTD = dtd;
			currentLinkedNodeLineNumber = startLineNumber;
			currentLinkedNodeLinePosition = startLinePosition;
			Init();
		}

		public bool HasLineInfo()
		{
			return true;
		}

		private XmlException NotWFError(string message)
		{
			return new XmlException(this, BaseURI, message);
		}

		private void Init()
		{
			parserInputStack = new Stack();
			nameBuffer = new char[256];
			nameLength = 0;
			nameCapacity = 256;
			valueBuffer = new StringBuilder(512);
		}

		internal DTDObjectModel GenerateDTDObjectModel()
		{
			int count = parserInputStack.Count;
			if (DTD.InternalSubset != null && DTD.InternalSubset.Length > 0)
			{
				processingInternalSubset = true;
				XmlParserInput xmlParserInput = currentInput;
				currentInput = new XmlParserInput(new StringReader(DTD.InternalSubset), DTD.BaseURI, currentLinkedNodeLineNumber, currentLinkedNodeLinePosition);
				currentInput.AllowTextDecl = false;
				bool flag;
				do
				{
					flag = ProcessDTDSubset();
					if (PeekChar() == -1 && parserInputStack.Count > 0)
					{
						PopParserInput();
					}
				}
				while (flag || parserInputStack.Count > count);
				if (dtdIncludeSect != 0)
				{
					throw NotWFError("INCLUDE section is not ended correctly.");
				}
				currentInput = xmlParserInput;
				processingInternalSubset = false;
			}
			if (DTD.SystemId != null && DTD.SystemId != string.Empty && DTD.Resolver != null)
			{
				PushParserInput(DTD.SystemId);
				bool flag;
				do
				{
					flag = ProcessDTDSubset();
					if (PeekChar() == -1 && parserInputStack.Count > 1)
					{
						PopParserInput();
					}
				}
				while (flag || parserInputStack.Count > count + 1);
				if (dtdIncludeSect != 0)
				{
					throw NotWFError("INCLUDE section is not ended correctly.");
				}
				PopParserInput();
			}
			ArrayList arrayList = new ArrayList();
			foreach (DTDEntityDeclaration value in DTD.EntityDecls.Values)
			{
				if (value.NotationName != null)
				{
					value.ScanEntityValue(arrayList);
					arrayList.Clear();
				}
			}
			DTD.ExternalResources.Clear();
			return DTD;
		}

		private bool ProcessDTDSubset()
		{
			SkipWhitespace();
			int num = ReadChar();
			switch (num)
			{
			case -1:
				return false;
			case 37:
			{
				if (processingInternalSubset)
				{
					DTD.InternalSubsetHasPEReference = true;
				}
				string peName = ReadName();
				Expect(59);
				DTDParameterEntityDeclaration pEDecl = GetPEDecl(peName);
				if (pEDecl != null)
				{
					currentInput.PushPEBuffer(pEDecl);
					while (currentInput.HasPEBuffer)
					{
						ProcessDTDSubset();
					}
					SkipWhitespace();
				}
				break;
			}
			case 60:
			{
				int num2 = ReadChar();
				switch (num2)
				{
				case 63:
					ReadProcessingInstruction();
					break;
				case 33:
					CompileDeclaration();
					break;
				case -1:
					throw NotWFError("Unexpected end of stream.");
				default:
					throw NotWFError("Syntax Error after '<' character: " + (char)num2);
				}
				break;
			}
			case 93:
				if (dtdIncludeSect == 0)
				{
					throw NotWFError("Unbalanced end of INCLUDE/IGNORE section.");
				}
				Expect("]>");
				dtdIncludeSect--;
				SkipWhitespace();
				break;
			default:
				throw NotWFError(string.Format("Syntax Error inside doctypedecl markup : {0}({1})", num, (char)num));
			}
			currentInput.AllowTextDecl = false;
			return true;
		}

		private void CompileDeclaration()
		{
			switch (ReadChar())
			{
			case 45:
				Expect(45);
				ReadComment();
				break;
			case 69:
				switch (ReadChar())
				{
				case 78:
				{
					Expect("TITY");
					if (!SkipWhitespace())
					{
						throw NotWFError("Whitespace is required after '<!ENTITY' in DTD entity declaration.");
					}
					while (PeekChar() == 37)
					{
						ReadChar();
						if (!SkipWhitespace())
						{
							ExpandPERef();
							continue;
						}
						TryExpandPERef();
						if (XmlChar.IsNameChar(PeekChar()))
						{
							ReadParameterEntityDecl();
							return;
						}
						throw NotWFError("expected name character");
					}
					DTDEntityDeclaration dTDEntityDeclaration = ReadEntityDecl();
					if (DTD.EntityDecls[dTDEntityDeclaration.Name] == null)
					{
						DTD.EntityDecls.Add(dTDEntityDeclaration.Name, dTDEntityDeclaration);
					}
					break;
				}
				case 76:
				{
					Expect("EMENT");
					DTDElementDeclaration dTDElementDeclaration = ReadElementDecl();
					DTD.ElementDecls.Add(dTDElementDeclaration.Name, dTDElementDeclaration);
					break;
				}
				default:
					throw NotWFError("Syntax Error after '<!E' (ELEMENT or ENTITY must be found)");
				}
				break;
			case 65:
			{
				Expect("TTLIST");
				DTDAttListDeclaration dTDAttListDeclaration = ReadAttListDecl();
				DTD.AttListDecls.Add(dTDAttListDeclaration.Name, dTDAttListDeclaration);
				break;
			}
			case 78:
			{
				Expect("OTATION");
				DTDNotationDeclaration dTDNotationDeclaration = ReadNotationDecl();
				DTD.NotationDecls.Add(dTDNotationDeclaration.Name, dTDNotationDeclaration);
				break;
			}
			case 91:
				SkipWhitespace();
				TryExpandPERef();
				Expect(73);
				switch (ReadChar())
				{
				case 78:
					Expect("CLUDE");
					ExpectAfterWhitespace('[');
					dtdIncludeSect++;
					break;
				case 71:
					Expect("NORE");
					ReadIgnoreSect();
					break;
				}
				break;
			default:
				throw NotWFError("Syntax Error after '<!' characters.");
			}
		}

		private void ReadIgnoreSect()
		{
			ExpectAfterWhitespace('[');
			int num = 1;
			while (num > 0)
			{
				switch (ReadChar())
				{
				case -1:
					throw NotWFError("Unexpected IGNORE section end.");
				case 60:
					if (PeekChar() == 33)
					{
						ReadChar();
						if (PeekChar() == 91)
						{
							ReadChar();
							num++;
						}
					}
					break;
				case 93:
					if (PeekChar() == 93)
					{
						ReadChar();
						if (PeekChar() == 62)
						{
							ReadChar();
							num--;
						}
					}
					break;
				}
			}
			if (num != 0)
			{
				throw NotWFError("IGNORE section is not ended correctly.");
			}
		}

		private DTDElementDeclaration ReadElementDecl()
		{
			DTDElementDeclaration dTDElementDeclaration = new DTDElementDeclaration(DTD);
			dTDElementDeclaration.IsInternalSubset = processingInternalSubset;
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required between '<!ELEMENT' and name in DTD element declaration.");
			}
			TryExpandPERef();
			dTDElementDeclaration.Name = ReadName();
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required between name and content in DTD element declaration.");
			}
			TryExpandPERef();
			ReadContentSpec(dTDElementDeclaration);
			SkipWhitespace();
			TryExpandPERef();
			Expect(62);
			return dTDElementDeclaration;
		}

		private void ReadContentSpec(DTDElementDeclaration decl)
		{
			TryExpandPERef();
			switch (ReadChar())
			{
			case 69:
				decl.IsEmpty = true;
				Expect("MPTY");
				break;
			case 65:
				decl.IsAny = true;
				Expect("NY");
				break;
			case 40:
			{
				DTDContentModel contentModel = decl.ContentModel;
				SkipWhitespace();
				TryExpandPERef();
				if (PeekChar() == 35)
				{
					decl.IsMixedContent = true;
					contentModel.Occurence = DTDOccurence.ZeroOrMore;
					contentModel.OrderType = DTDContentOrderType.Or;
					Expect("#PCDATA");
					SkipWhitespace();
					TryExpandPERef();
					while (PeekChar() != 41)
					{
						SkipWhitespace();
						if (PeekChar() == 37)
						{
							TryExpandPERef();
							continue;
						}
						Expect(124);
						SkipWhitespace();
						TryExpandPERef();
						DTDContentModel dTDContentModel = new DTDContentModel(DTD, decl.Name);
						dTDContentModel.ElementName = ReadName();
						AddContentModel(contentModel.ChildModels, dTDContentModel);
						SkipWhitespace();
						TryExpandPERef();
					}
					Expect(41);
					if (contentModel.ChildModels.Count > 0)
					{
						Expect(42);
					}
					else if (PeekChar() == 42)
					{
						Expect(42);
					}
				}
				else
				{
					contentModel.ChildModels.Add(ReadCP(decl));
					SkipWhitespace();
					while (true)
					{
						if (PeekChar() == 37)
						{
							TryExpandPERef();
							continue;
						}
						if (PeekChar() == 124)
						{
							if (contentModel.OrderType == DTDContentOrderType.Seq)
							{
								throw NotWFError("Inconsistent choice markup in sequence cp.");
							}
							contentModel.OrderType = DTDContentOrderType.Or;
							ReadChar();
							SkipWhitespace();
							AddContentModel(contentModel.ChildModels, ReadCP(decl));
							SkipWhitespace();
							continue;
						}
						if (PeekChar() == 44)
						{
							if (contentModel.OrderType == DTDContentOrderType.Or)
							{
								throw NotWFError("Inconsistent sequence markup in choice cp.");
							}
							contentModel.OrderType = DTDContentOrderType.Seq;
							ReadChar();
							SkipWhitespace();
							contentModel.ChildModels.Add(ReadCP(decl));
							SkipWhitespace();
							continue;
						}
						break;
					}
					Expect(41);
					switch (PeekChar())
					{
					case 63:
						contentModel.Occurence = DTDOccurence.Optional;
						ReadChar();
						break;
					case 42:
						contentModel.Occurence = DTDOccurence.ZeroOrMore;
						ReadChar();
						break;
					case 43:
						contentModel.Occurence = DTDOccurence.OneOrMore;
						ReadChar();
						break;
					}
					SkipWhitespace();
				}
				SkipWhitespace();
				break;
			}
			default:
				throw NotWFError("ContentSpec is missing.");
			}
		}

		private DTDContentModel ReadCP(DTDElementDeclaration elem)
		{
			DTDContentModel dTDContentModel = null;
			TryExpandPERef();
			if (PeekChar() == 40)
			{
				dTDContentModel = new DTDContentModel(DTD, elem.Name);
				ReadChar();
				SkipWhitespace();
				dTDContentModel.ChildModels.Add(ReadCP(elem));
				SkipWhitespace();
				while (true)
				{
					if (PeekChar() == 37)
					{
						TryExpandPERef();
						continue;
					}
					if (PeekChar() == 124)
					{
						if (dTDContentModel.OrderType == DTDContentOrderType.Seq)
						{
							throw NotWFError("Inconsistent choice markup in sequence cp.");
						}
						dTDContentModel.OrderType = DTDContentOrderType.Or;
						ReadChar();
						SkipWhitespace();
						AddContentModel(dTDContentModel.ChildModels, ReadCP(elem));
						SkipWhitespace();
						continue;
					}
					if (PeekChar() == 44)
					{
						if (dTDContentModel.OrderType == DTDContentOrderType.Or)
						{
							throw NotWFError("Inconsistent sequence markup in choice cp.");
						}
						dTDContentModel.OrderType = DTDContentOrderType.Seq;
						ReadChar();
						SkipWhitespace();
						dTDContentModel.ChildModels.Add(ReadCP(elem));
						SkipWhitespace();
						continue;
					}
					break;
				}
				ExpectAfterWhitespace(')');
			}
			else
			{
				TryExpandPERef();
				dTDContentModel = new DTDContentModel(DTD, elem.Name);
				dTDContentModel.ElementName = ReadName();
			}
			switch (PeekChar())
			{
			case 63:
				dTDContentModel.Occurence = DTDOccurence.Optional;
				ReadChar();
				break;
			case 42:
				dTDContentModel.Occurence = DTDOccurence.ZeroOrMore;
				ReadChar();
				break;
			case 43:
				dTDContentModel.Occurence = DTDOccurence.OneOrMore;
				ReadChar();
				break;
			}
			return dTDContentModel;
		}

		private void AddContentModel(DTDContentModelCollection cmc, DTDContentModel cm)
		{
			if (cm.ElementName != null)
			{
				for (int i = 0; i < cmc.Count; i++)
				{
					if (cmc[i].ElementName == cm.ElementName)
					{
						HandleError(new XmlException("Element content must be unique inside mixed content model.", LineNumber, LinePosition, null, BaseURI, null));
						return;
					}
				}
			}
			cmc.Add(cm);
		}

		private void ReadParameterEntityDecl()
		{
			DTDParameterEntityDeclaration dTDParameterEntityDeclaration = new DTDParameterEntityDeclaration(DTD);
			dTDParameterEntityDeclaration.BaseURI = BaseURI;
			dTDParameterEntityDeclaration.XmlResolver = DTD.Resolver;
			dTDParameterEntityDeclaration.Name = ReadName();
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required after name in DTD parameter entity declaration.");
			}
			if (PeekChar() == 83 || PeekChar() == 80)
			{
				ReadExternalID();
				dTDParameterEntityDeclaration.PublicId = cachedPublicId;
				dTDParameterEntityDeclaration.SystemId = cachedSystemId;
				SkipWhitespace();
				dTDParameterEntityDeclaration.Resolve();
				ResolveExternalEntityReplacementText(dTDParameterEntityDeclaration);
			}
			else
			{
				TryExpandPERef();
				int num = ReadChar();
				if (num != 39 && num != 34)
				{
					throw NotWFError("quotation char was expected.");
				}
				ClearValueBuffer();
				bool flag = true;
				while (flag)
				{
					int num2 = ReadChar();
					switch (num2)
					{
					case -1:
						throw NotWFError("unexpected end of stream in entity value definition.");
					case 34:
						if (num == 34)
						{
							flag = false;
						}
						else
						{
							AppendValueChar(34);
						}
						continue;
					case 39:
						if (num == 39)
						{
							flag = false;
						}
						else
						{
							AppendValueChar(39);
						}
						continue;
					}
					if (XmlChar.IsInvalid(num2))
					{
						throw NotWFError("Invalid character was used to define parameter entity.");
					}
					AppendValueChar(num2);
				}
				dTDParameterEntityDeclaration.LiteralEntityValue = CreateValueString();
				ClearValueBuffer();
				ResolveInternalEntityReplacementText(dTDParameterEntityDeclaration);
			}
			ExpectAfterWhitespace('>');
			if (DTD.PEDecls[dTDParameterEntityDeclaration.Name] == null)
			{
				DTD.PEDecls.Add(dTDParameterEntityDeclaration.Name, dTDParameterEntityDeclaration);
			}
		}

		private void ResolveExternalEntityReplacementText(DTDEntityBase decl)
		{
			if (decl.SystemId != null && decl.SystemId.Length > 0)
			{
				XmlTextReader xmlTextReader = new XmlTextReader(decl.LiteralEntityValue, XmlNodeType.Element, null);
				xmlTextReader.SkipTextDeclaration();
				if (decl is DTDEntityDeclaration && DTD.EntityDecls[decl.Name] == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					xmlTextReader.Normalization = Normalization;
					xmlTextReader.Read();
					while (!xmlTextReader.EOF)
					{
						stringBuilder.Append(xmlTextReader.ReadOuterXml());
					}
					decl.ReplacementText = stringBuilder.ToString();
				}
				else
				{
					decl.ReplacementText = xmlTextReader.GetRemainder().ReadToEnd();
				}
			}
			else
			{
				decl.ReplacementText = decl.LiteralEntityValue;
			}
		}

		private void ResolveInternalEntityReplacementText(DTDEntityBase decl)
		{
			string literalEntityValue = decl.LiteralEntityValue;
			int length = literalEntityValue.Length;
			ClearValueBuffer();
			for (int i = 0; i < length; i++)
			{
				int num = literalEntityValue[i];
				int num2 = 0;
				switch (num)
				{
				case 38:
					i++;
					num2 = literalEntityValue.IndexOf(';', i);
					if (num2 < i + 1)
					{
						throw new XmlException(decl, decl.BaseURI, "Invalid reference markup.");
					}
					if (literalEntityValue[i] == '#')
					{
						i++;
						num = GetCharacterReference(decl, literalEntityValue, ref i, num2);
						if (XmlChar.IsInvalid(num))
						{
							throw NotWFError("Invalid character was used to define parameter entity.");
						}
						if (XmlChar.IsInvalid(num))
						{
							throw new XmlException(decl, decl.BaseURI, "Invalid character was found in the entity declaration.");
						}
						AppendValueChar(num);
					}
					else
					{
						string peName = literalEntityValue.Substring(i, num2 - i);
						if (!XmlChar.IsName(peName))
						{
							throw NotWFError(string.Format("'{0}' is not a valid entity reference name.", peName));
						}
						AppendValueChar(38);
						valueBuffer.Append(peName);
						AppendValueChar(59);
						i = num2;
					}
					break;
				case 37:
				{
					i++;
					num2 = literalEntityValue.IndexOf(';', i);
					if (num2 < i + 1)
					{
						throw new XmlException(decl, decl.BaseURI, "Invalid reference markup.");
					}
					string peName = literalEntityValue.Substring(i, num2 - i);
					valueBuffer.Append(GetPEValue(peName));
					i = num2;
					break;
				}
				default:
					AppendValueChar(num);
					break;
				}
			}
			decl.ReplacementText = CreateValueString();
			ClearValueBuffer();
		}

		private int GetCharacterReference(DTDEntityBase li, string value, ref int index, int end)
		{
			int num = 0;
			if (value[index] == 'x')
			{
				try
				{
					num = int.Parse(value.Substring(index + 1, end - index - 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
				}
				catch (FormatException)
				{
					throw new XmlException(li, li.BaseURI, "Invalid number for a character reference.");
				}
			}
			else
			{
				try
				{
					num = int.Parse(value.Substring(index, end - index), CultureInfo.InvariantCulture);
				}
				catch (FormatException)
				{
					throw new XmlException(li, li.BaseURI, "Invalid number for a character reference.");
				}
			}
			index = end;
			return num;
		}

		private string GetPEValue(string peName)
		{
			DTDParameterEntityDeclaration pEDecl = GetPEDecl(peName);
			return (pEDecl == null) ? string.Empty : pEDecl.ReplacementText;
		}

		private DTDParameterEntityDeclaration GetPEDecl(string peName)
		{
			DTDParameterEntityDeclaration dTDParameterEntityDeclaration = DTD.PEDecls[peName];
			if (dTDParameterEntityDeclaration != null)
			{
				if (dTDParameterEntityDeclaration.IsInternalSubset)
				{
					throw NotWFError("Parameter entity is not allowed in internal subset entity '" + peName + "'");
				}
				return dTDParameterEntityDeclaration;
			}
			if ((DTD.SystemId == null && !DTD.InternalSubsetHasPEReference) || DTD.IsStandalone)
			{
				throw NotWFError(string.Format("Parameter entity '{0}' not found.", peName));
			}
			HandleError(new XmlException("Parameter entity " + peName + " not found.", null));
			return null;
		}

		private bool TryExpandPERef()
		{
			if (PeekChar() != 37)
			{
				return false;
			}
			while (PeekChar() == 37)
			{
				TryExpandPERefSpaceKeep();
				SkipWhitespace();
			}
			return true;
		}

		private bool TryExpandPERefSpaceKeep()
		{
			if (PeekChar() == 37)
			{
				if (processingInternalSubset)
				{
					throw NotWFError("Parameter entity reference is not allowed inside internal subset.");
				}
				ReadChar();
				ExpandPERef();
				return true;
			}
			return false;
		}

		private void ExpandPERef()
		{
			string text = ReadName();
			Expect(59);
			DTDParameterEntityDeclaration dTDParameterEntityDeclaration = DTD.PEDecls[text];
			if (dTDParameterEntityDeclaration == null)
			{
				HandleError(new XmlException("Parameter entity " + text + " not found.", null));
			}
			else
			{
				currentInput.PushPEBuffer(dTDParameterEntityDeclaration);
			}
		}

		private DTDEntityDeclaration ReadEntityDecl()
		{
			DTDEntityDeclaration dTDEntityDeclaration = new DTDEntityDeclaration(DTD);
			dTDEntityDeclaration.BaseURI = BaseURI;
			dTDEntityDeclaration.XmlResolver = DTD.Resolver;
			dTDEntityDeclaration.IsInternalSubset = processingInternalSubset;
			TryExpandPERef();
			dTDEntityDeclaration.Name = ReadName();
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required between name and content in DTD entity declaration.");
			}
			TryExpandPERef();
			if (PeekChar() == 83 || PeekChar() == 80)
			{
				ReadExternalID();
				dTDEntityDeclaration.PublicId = cachedPublicId;
				dTDEntityDeclaration.SystemId = cachedSystemId;
				if (SkipWhitespace() && PeekChar() == 78)
				{
					Expect("NDATA");
					if (!SkipWhitespace())
					{
						throw NotWFError("Whitespace is required after NDATA.");
					}
					dTDEntityDeclaration.NotationName = ReadName();
				}
				if (dTDEntityDeclaration.NotationName == null)
				{
					dTDEntityDeclaration.Resolve();
					ResolveExternalEntityReplacementText(dTDEntityDeclaration);
				}
				else
				{
					dTDEntityDeclaration.LiteralEntityValue = string.Empty;
					dTDEntityDeclaration.ReplacementText = string.Empty;
				}
			}
			else
			{
				ReadEntityValueDecl(dTDEntityDeclaration);
				ResolveInternalEntityReplacementText(dTDEntityDeclaration);
			}
			SkipWhitespace();
			TryExpandPERef();
			Expect(62);
			return dTDEntityDeclaration;
		}

		private void ReadEntityValueDecl(DTDEntityDeclaration decl)
		{
			SkipWhitespace();
			int num = ReadChar();
			if (num != 39 && num != 34)
			{
				throw NotWFError("quotation char was expected.");
			}
			ClearValueBuffer();
			while (PeekChar() != num)
			{
				int num2 = ReadChar();
				switch (num2)
				{
				case 37:
				{
					string text = ReadName();
					Expect(59);
					if (decl.IsInternalSubset)
					{
						throw NotWFError(string.Format("Parameter entity is not allowed in internal subset entity '{0}'", text));
					}
					valueBuffer.Append(GetPEValue(text));
					break;
				}
				case -1:
					throw NotWFError("unexpected end of stream.");
				default:
					if (normalization && XmlChar.IsInvalid(num2))
					{
						throw NotWFError("Invalid character was found in the entity declaration.");
					}
					AppendValueChar(num2);
					break;
				}
			}
			string literalEntityValue = CreateValueString();
			ClearValueBuffer();
			Expect(num);
			decl.LiteralEntityValue = literalEntityValue;
		}

		private DTDAttListDeclaration ReadAttListDecl()
		{
			TryExpandPERefSpaceKeep();
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required between ATTLIST and name in DTD attlist declaration.");
			}
			TryExpandPERef();
			string name = ReadName();
			DTDAttListDeclaration dTDAttListDeclaration = DTD.AttListDecls[name];
			if (dTDAttListDeclaration == null)
			{
				dTDAttListDeclaration = new DTDAttListDeclaration(DTD);
			}
			dTDAttListDeclaration.IsInternalSubset = processingInternalSubset;
			dTDAttListDeclaration.Name = name;
			if (!SkipWhitespace() && PeekChar() != 62)
			{
				throw NotWFError("Whitespace is required between name and content in non-empty DTD attlist declaration.");
			}
			TryExpandPERef();
			while (XmlChar.IsNameChar(PeekChar()))
			{
				DTDAttributeDefinition dTDAttributeDefinition = ReadAttributeDefinition();
				if (dTDAttributeDefinition.Datatype.TokenizedType == XmlTokenizedType.ID)
				{
					for (int i = 0; i < dTDAttListDeclaration.Definitions.Count; i++)
					{
						DTDAttributeDefinition dTDAttributeDefinition2 = dTDAttListDeclaration[i];
						if (dTDAttributeDefinition2.Datatype.TokenizedType == XmlTokenizedType.ID)
						{
							HandleError(new XmlException("AttList declaration must not contain two or more ID attributes.", dTDAttributeDefinition.LineNumber, dTDAttributeDefinition.LinePosition, null, dTDAttributeDefinition.BaseURI, null));
							break;
						}
					}
				}
				if (dTDAttListDeclaration[dTDAttributeDefinition.Name] == null)
				{
					dTDAttListDeclaration.Add(dTDAttributeDefinition);
				}
				SkipWhitespace();
				TryExpandPERef();
			}
			SkipWhitespace();
			TryExpandPERef();
			Expect(62);
			return dTDAttListDeclaration;
		}

		private DTDAttributeDefinition ReadAttributeDefinition()
		{
			throw new NotImplementedException();
		}

		private void ReadAttributeDefaultValue(DTDAttributeDefinition def)
		{
			if (PeekChar() == 35)
			{
				ReadChar();
				switch (PeekChar())
				{
				case 82:
					Expect("REQUIRED");
					def.OccurenceType = DTDAttributeOccurenceType.Required;
					break;
				case 73:
					Expect("IMPLIED");
					def.OccurenceType = DTDAttributeOccurenceType.Optional;
					break;
				case 70:
					Expect("FIXED");
					def.OccurenceType = DTDAttributeOccurenceType.Fixed;
					if (!SkipWhitespace())
					{
						throw NotWFError("Whitespace is required between FIXED and actual value in DTD attribute definition.");
					}
					def.UnresolvedDefaultValue = ReadDefaultAttribute();
					break;
				}
			}
			else
			{
				SkipWhitespace();
				TryExpandPERef();
				def.UnresolvedDefaultValue = ReadDefaultAttribute();
			}
			if (def.DefaultValue != null)
			{
				string text = def.Datatype.Normalize(def.DefaultValue);
				bool flag = false;
				object obj = null;
				if (def.EnumeratedAttributeDeclaration.Count > 0 && !def.EnumeratedAttributeDeclaration.Contains(text))
				{
					HandleError(new XmlException("Default value is not one of the enumerated values.", def.LineNumber, def.LinePosition, null, def.BaseURI, null));
					flag = true;
				}
				if (def.EnumeratedNotations.Count > 0 && !def.EnumeratedNotations.Contains(text))
				{
					HandleError(new XmlException("Default value is not one of the enumerated notation values.", def.LineNumber, def.LinePosition, null, def.BaseURI, null));
					flag = true;
				}
				if (!flag)
				{
					try
					{
						obj = def.Datatype.ParseValue(text, DTD.NameTable, null);
					}
					catch (Exception innerException)
					{
						HandleError(new XmlException("Invalid default value for ENTITY type.", def.LineNumber, def.LinePosition, null, def.BaseURI, innerException));
						flag = true;
					}
				}
				if (!flag)
				{
					switch (def.Datatype.TokenizedType)
					{
					case XmlTokenizedType.ENTITY:
						if (DTD.EntityDecls[text] == null)
						{
							HandleError(new XmlException("Specified entity declaration used by default attribute value was not found.", def.LineNumber, def.LinePosition, null, def.BaseURI, null));
						}
						break;
					case XmlTokenizedType.ENTITIES:
					{
						string[] array = obj as string[];
						foreach (string name in array)
						{
							if (DTD.EntityDecls[name] == null)
							{
								HandleError(new XmlException("Specified entity declaration used by default attribute value was not found.", def.LineNumber, def.LinePosition, null, def.BaseURI, null));
							}
						}
						break;
					}
					}
				}
			}
			if (def.Datatype != null && def.Datatype.TokenizedType == XmlTokenizedType.ID && def.UnresolvedDefaultValue != null)
			{
				HandleError(new XmlException("ID attribute must not have fixed value constraint.", def.LineNumber, def.LinePosition, null, def.BaseURI, null));
			}
		}

		private DTDNotationDeclaration ReadNotationDecl()
		{
			DTDNotationDeclaration dTDNotationDeclaration = new DTDNotationDeclaration(DTD);
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required between NOTATION and name in DTD notation declaration.");
			}
			TryExpandPERef();
			dTDNotationDeclaration.Name = ReadName();
			dTDNotationDeclaration.Prefix = string.Empty;
			dTDNotationDeclaration.LocalName = dTDNotationDeclaration.Name;
			SkipWhitespace();
			if (PeekChar() == 80)
			{
				dTDNotationDeclaration.PublicId = ReadPubidLiteral();
				bool flag = SkipWhitespace();
				if (PeekChar() == 39 || PeekChar() == 34)
				{
					if (!flag)
					{
						throw NotWFError("Whitespace is required between public id and system id.");
					}
					dTDNotationDeclaration.SystemId = ReadSystemLiteral(false);
					SkipWhitespace();
				}
			}
			else if (PeekChar() == 83)
			{
				dTDNotationDeclaration.SystemId = ReadSystemLiteral(true);
				SkipWhitespace();
			}
			if (dTDNotationDeclaration.PublicId == null && dTDNotationDeclaration.SystemId == null)
			{
				throw NotWFError("public or system declaration required for \"NOTATION\" declaration.");
			}
			TryExpandPERef();
			Expect(62);
			return dTDNotationDeclaration;
		}

		private void ReadExternalID()
		{
			switch (PeekChar())
			{
			case 83:
				cachedSystemId = ReadSystemLiteral(true);
				break;
			case 80:
				cachedPublicId = ReadPubidLiteral();
				if (!SkipWhitespace())
				{
					throw NotWFError("Whitespace is required between PUBLIC id and SYSTEM id.");
				}
				cachedSystemId = ReadSystemLiteral(false);
				break;
			case 81:
			case 82:
				break;
			}
		}

		private string ReadSystemLiteral(bool expectSYSTEM)
		{
			if (expectSYSTEM)
			{
				Expect("SYSTEM");
				if (!SkipWhitespace())
				{
					throw NotWFError("Whitespace is required after 'SYSTEM'.");
				}
			}
			else
			{
				SkipWhitespace();
			}
			int num = ReadChar();
			int num2 = 0;
			ClearValueBuffer();
			while (num2 != num)
			{
				num2 = ReadChar();
				if (num2 < 0)
				{
					throw NotWFError("Unexpected end of stream in ExternalID.");
				}
				if (num2 != num)
				{
					AppendValueChar(num2);
				}
			}
			return CreateValueString();
		}

		private string ReadPubidLiteral()
		{
			Expect("PUBLIC");
			if (!SkipWhitespace())
			{
				throw NotWFError("Whitespace is required after 'PUBLIC'.");
			}
			int num = ReadChar();
			int num2 = 0;
			ClearValueBuffer();
			while (num2 != num)
			{
				num2 = ReadChar();
				if (num2 < 0)
				{
					throw NotWFError("Unexpected end of stream in ExternalID.");
				}
				if (num2 != num && !XmlChar.IsPubidChar(num2))
				{
					throw NotWFError(string.Format("character '{0}' not allowed for PUBLIC ID", (char)num2));
				}
				if (num2 != num)
				{
					AppendValueChar(num2);
				}
			}
			return CreateValueString();
		}

		internal string ReadName()
		{
			return ReadNameOrNmToken(false);
		}

		private string ReadNmToken()
		{
			return ReadNameOrNmToken(true);
		}

		private string ReadNameOrNmToken(bool isNameToken)
		{
			int num = PeekChar();
			if (isNameToken)
			{
				if (!XmlChar.IsNameChar(num))
				{
					throw NotWFError(string.Format("a nmtoken did not start with a legal character {0} ({1})", num, (char)num));
				}
			}
			else if (!XmlChar.IsFirstNameChar(num))
			{
				throw NotWFError(string.Format("a name did not start with a legal character {0} ({1})", num, (char)num));
			}
			nameLength = 0;
			AppendNameChar(ReadChar());
			while (XmlChar.IsNameChar(PeekChar()))
			{
				AppendNameChar(ReadChar());
			}
			return CreateNameString();
		}

		private void Expect(int expected)
		{
			int num = ReadChar();
			if (num != expected)
			{
				throw NotWFError(string.Format(CultureInfo.InvariantCulture, "expected '{0}' ({1:X}) but found '{2}' ({3:X})", (char)expected, expected, (char)num, num));
			}
		}

		private void Expect(string expected)
		{
			int length = expected.Length;
			for (int i = 0; i < length; i++)
			{
				Expect(expected[i]);
			}
		}

		private void ExpectAfterWhitespace(char c)
		{
			int num;
			do
			{
				num = ReadChar();
			}
			while (XmlChar.IsWhitespace(num));
			if (c != num)
			{
				throw NotWFError(string.Format(CultureInfo.InvariantCulture, "Expected {0} but found {1} [{2}].", c, (char)num, num));
			}
		}

		private bool SkipWhitespace()
		{
			bool result = XmlChar.IsWhitespace(PeekChar());
			while (XmlChar.IsWhitespace(PeekChar()))
			{
				ReadChar();
			}
			return result;
		}

		private int PeekChar()
		{
			return currentInput.PeekChar();
		}

		private int ReadChar()
		{
			return currentInput.ReadChar();
		}

		private void ReadComment()
		{
			currentInput.AllowTextDecl = false;
			while (PeekChar() != -1)
			{
				int num = ReadChar();
				if (num == 45 && PeekChar() == 45)
				{
					ReadChar();
					if (PeekChar() != 62)
					{
						throw NotWFError("comments cannot contain '--'");
					}
					ReadChar();
					break;
				}
				if (XmlChar.IsInvalid(num))
				{
					throw NotWFError("Not allowed character was found.");
				}
			}
		}

		private void ReadProcessingInstruction()
		{
			string text = ReadName();
			if (text == "xml")
			{
				ReadTextDeclaration();
				return;
			}
			if (CultureInfo.InvariantCulture.CompareInfo.Compare(text, "xml", CompareOptions.IgnoreCase) == 0)
			{
				throw NotWFError("Not allowed processing instruction name which starts with 'X', 'M', 'L' was found.");
			}
			currentInput.AllowTextDecl = false;
			if (!SkipWhitespace() && PeekChar() != 63)
			{
				throw NotWFError("Invalid processing instruction name was found.");
			}
			while (PeekChar() != -1)
			{
				int num = ReadChar();
				if (num == 63 && PeekChar() == 62)
				{
					ReadChar();
					break;
				}
			}
		}

		private void ReadTextDeclaration()
		{
			if (!currentInput.AllowTextDecl)
			{
				throw NotWFError("Text declaration cannot appear in this state.");
			}
			currentInput.AllowTextDecl = false;
			SkipWhitespace();
			if (PeekChar() == 118)
			{
				Expect("version");
				ExpectAfterWhitespace('=');
				SkipWhitespace();
				int num = ReadChar();
				char[] array = new char[3];
				int num2 = 0;
				int num3 = num;
				if (num3 != 34 && num3 != 39)
				{
					throw NotWFError("Invalid version declaration inside text declaration.");
				}
				while (PeekChar() != num)
				{
					if (PeekChar() == -1)
					{
						throw NotWFError("Invalid version declaration inside text declaration.");
					}
					if (num2 == 3)
					{
						throw NotWFError("Invalid version number inside text declaration.");
					}
					array[num2] = (char)ReadChar();
					num2++;
					if (num2 == 3 && new string(array) != "1.0")
					{
						throw NotWFError("Invalid version number inside text declaration.");
					}
				}
				ReadChar();
				SkipWhitespace();
			}
			if (PeekChar() == 101)
			{
				Expect("encoding");
				ExpectAfterWhitespace('=');
				SkipWhitespace();
				int num4 = ReadChar();
				int num3 = num4;
				if (num3 == 34 || num3 == 39)
				{
					while (PeekChar() != num4)
					{
						if (ReadChar() == -1)
						{
							throw NotWFError("Invalid encoding declaration inside text declaration.");
						}
					}
					ReadChar();
					SkipWhitespace();
					Expect("?>");
					return;
				}
				throw NotWFError("Invalid encoding declaration inside text declaration.");
			}
			throw NotWFError("Encoding declaration is mandatory in text declaration.");
		}

		private void AppendNameChar(int ch)
		{
			CheckNameCapacity();
			if (ch < 65535)
			{
				nameBuffer[nameLength++] = (char)ch;
				return;
			}
			nameBuffer[nameLength++] = (char)(ch / 65536 + 55296 - 1);
			CheckNameCapacity();
			nameBuffer[nameLength++] = (char)(ch % 65536 + 56320);
		}

		private void CheckNameCapacity()
		{
			if (nameLength == nameCapacity)
			{
				nameCapacity *= 2;
				char[] sourceArray = nameBuffer;
				nameBuffer = new char[nameCapacity];
				Array.Copy(sourceArray, nameBuffer, nameLength);
			}
		}

		private string CreateNameString()
		{
			return DTD.NameTable.Add(nameBuffer, 0, nameLength);
		}

		private void AppendValueChar(int ch)
		{
			if (ch < 65536)
			{
				valueBuffer.Append((char)ch);
				return;
			}
			if (ch > 1114111)
			{
				throw new XmlException("The numeric entity value is too large", null, LineNumber, LinePosition);
			}
			int num = ch - 65536;
			valueBuffer.Append((char)((num >> 10) + 55296));
			valueBuffer.Append((char)((num & 0x3FF) + 56320));
		}

		private string CreateValueString()
		{
			return valueBuffer.ToString();
		}

		private void ClearValueBuffer()
		{
			valueBuffer.Length = 0;
		}

		private string ReadDefaultAttribute()
		{
			ClearValueBuffer();
			TryExpandPERef();
			int num = ReadChar();
			if (num != 39 && num != 34)
			{
				throw NotWFError("an attribute value was not quoted");
			}
			AppendValueChar(num);
			while (PeekChar() != num)
			{
				int num2 = ReadChar();
				switch (num2)
				{
				case 60:
					throw NotWFError("attribute values cannot contain '<'");
				case -1:
					throw NotWFError("unexpected end of file in an attribute value");
				case 38:
				{
					AppendValueChar(num2);
					if (PeekChar() == 35)
					{
						break;
					}
					string text = ReadName();
					Expect(59);
					if (XmlChar.GetPredefinedEntity(text) < 0)
					{
						DTDEntityDeclaration dTDEntityDeclaration = ((DTD != null) ? DTD.EntityDecls[text] : null);
						if ((dTDEntityDeclaration == null || dTDEntityDeclaration.SystemId != null) && (DTD.IsStandalone || (DTD.SystemId == null && !DTD.InternalSubsetHasPEReference)))
						{
							throw NotWFError("Reference to external entities is not allowed in attribute value.");
						}
					}
					valueBuffer.Append(text);
					AppendValueChar(59);
					break;
				}
				default:
					AppendValueChar(num2);
					break;
				}
			}
			ReadChar();
			AppendValueChar(num);
			return CreateValueString();
		}

		private void PushParserInput(string url)
		{
			Uri uri = null;
			try
			{
				if (DTD.BaseURI != null && DTD.BaseURI.Length > 0)
				{
					uri = new Uri(DTD.BaseURI);
				}
			}
			catch (UriFormatException)
			{
			}
			Uri uri2 = ((url == null || url.Length <= 0) ? uri : DTD.Resolver.ResolveUri(uri, url));
			string text = ((!(uri2 != null)) ? string.Empty : uri2.ToString());
			object[] array = parserInputStack.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				XmlParserInput xmlParserInput = (XmlParserInput)array[i];
				if (xmlParserInput.BaseURI == text)
				{
					throw NotWFError("Nested inclusion is not allowed: " + url);
				}
			}
			parserInputStack.Push(currentInput);
			Stream stream = null;
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				stream = DTD.Resolver.GetEntity(uri2, null, typeof(Stream)) as Stream;
				byte[] array2 = new byte[4096];
				int num;
				do
				{
					num = stream.Read(array2, 0, array2.Length);
					memoryStream.Write(array2, 0, num);
				}
				while (num > 0);
				stream.Close();
				memoryStream.Position = 0L;
				currentInput = new XmlParserInput(new XmlStreamReader(memoryStream), text);
			}
			catch (Exception innerException)
			{
				if (stream != null)
				{
					stream.Close();
				}
				int lineNumber = ((currentInput != null) ? currentInput.LineNumber : 0);
				int linePosition = ((currentInput != null) ? currentInput.LinePosition : 0);
				string sourceUri = ((currentInput != null) ? currentInput.BaseURI : string.Empty);
				HandleError(new XmlException("Specified external entity not found. Target URL is " + url + " .", lineNumber, linePosition, null, sourceUri, innerException));
				currentInput = new XmlParserInput(new StringReader(string.Empty), text);
			}
		}

		private void PopParserInput()
		{
			currentInput.Close();
			currentInput = parserInputStack.Pop() as XmlParserInput;
		}

		private void HandleError(XmlException ex)
		{
			DTD.AddError(ex);
		}
	}
}
