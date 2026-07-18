using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mono.Xml.Xsl
{
	internal class HtmlEmitter : Emitter
	{
		private class HtmlUriEscape : Uri
		{
			private HtmlUriEscape()
				: base("urn:foo")
			{
			}

			public static string EscapeUri(string input)
			{
				StringBuilder stringBuilder = new StringBuilder();
				int num = 0;
				for (int i = 0; i < input.Length; i++)
				{
					char c = input[i];
					if (c >= ' ' && c <= '\u007f')
					{
						bool flag = false;
						switch (c)
						{
						case '"':
						case '&':
						case '\'':
						case '<':
						case '>':
							flag = true;
							break;
						default:
							flag = Uri.IsExcludedCharacter(c);
							break;
						}
						if (flag)
						{
							stringBuilder.Append(Uri.EscapeString(input.Substring(num, i - num)));
							stringBuilder.Append(c);
							num = i + 1;
						}
					}
				}
				if (num < input.Length)
				{
					stringBuilder.Append(Uri.EscapeString(input.Substring(num)));
				}
				return stringBuilder.ToString();
			}
		}

		private TextWriter writer;

		private Stack elementNameStack;

		private bool openElement;

		private bool openAttribute;

		private int nonHtmlDepth;

		private bool indent;

		private Encoding outputEncoding;

		private string mediaType;

		public HtmlEmitter(TextWriter writer, XslOutput output)
		{
			this.writer = writer;
			indent = output.Indent == "yes" || output.Indent == null;
			elementNameStack = new Stack();
			nonHtmlDepth = -1;
			outputEncoding = ((writer.Encoding != null) ? writer.Encoding : output.Encoding);
			mediaType = output.MediaType;
			if (mediaType == null || mediaType.Length == 0)
			{
				mediaType = "text/html";
			}
		}

		public override void WriteStartDocument(Encoding encoding, StandaloneType standalone)
		{
		}

		public override void WriteEndDocument()
		{
		}

		public override void WriteDocType(string name, string publicId, string systemId)
		{
			writer.Write("<!DOCTYPE html ");
			if (publicId != null)
			{
				writer.Write("PUBLIC \"");
				writer.Write(publicId);
				writer.Write("\" ");
				if (systemId != null)
				{
					writer.Write("\"");
					writer.Write(systemId);
					writer.Write("\"");
				}
			}
			else if (systemId != null)
			{
				writer.Write("SYSTEM \"");
				writer.Write(systemId);
				writer.Write('"');
			}
			writer.Write('>');
			if (indent)
			{
				writer.WriteLine();
			}
		}

		private void CloseAttribute()
		{
			writer.Write('"');
			openAttribute = false;
		}

		private void CloseStartElement()
		{
			if (openAttribute)
			{
				CloseAttribute();
			}
			writer.Write('>');
			openElement = false;
			if (outputEncoding == null || elementNameStack.Count <= 0)
			{
				return;
			}
			switch (((string)elementNameStack.Peek()).ToUpper(CultureInfo.InvariantCulture))
			{
			case "HEAD":
				WriteStartElement(string.Empty, "META", string.Empty);
				WriteAttributeString(string.Empty, "http-equiv", string.Empty, "Content-Type");
				WriteAttributeString(string.Empty, "content", string.Empty, mediaType + "; charset=" + outputEncoding.WebName);
				WriteEndElement();
				break;
			case "STYLE":
			case "SCRIPT":
			{
				writer.WriteLine();
				for (int i = 0; i <= elementNameStack.Count; i++)
				{
					writer.Write("  ");
				}
				break;
			}
			}
		}

		private void Indent(string elementName, bool endIndent)
		{
			if (!indent)
			{
				return;
			}
			switch (elementName.ToUpper(CultureInfo.InvariantCulture))
			{
			case "ADDRESS":
			case "APPLET":
			case "BDO":
			case "BLOCKQUOTE":
			case "BODY":
			case "BUTTON":
			case "CAPTION":
			case "CENTER":
			case "DD":
			case "DEL":
			case "DIR":
			case "DIV":
			case "DL":
			case "DT":
			case "FIELDSET":
			case "HEAD":
			case "HTML":
			case "IFRAME":
			case "INS":
			case "LI":
			case "MAP":
			case "MENU":
			case "NOFRAMES":
			case "NOSCRIPT":
			case "OBJECT":
			case "OPTION":
			case "PRE":
			case "TABLE":
			case "TD":
			case "TH":
			case "TR":
			{
				writer.Write(writer.NewLine);
				int count = elementNameStack.Count;
				for (int i = 0; i < count; i++)
				{
					writer.Write("  ");
				}
				break;
			}
			default:
				if (elementName.Length > 0 && nonHtmlDepth > 0)
				{
					goto case "ADDRESS";
				}
				break;
			}
		}

		public override void WriteStartElement(string prefix, string localName, string nsURI)
		{
			if (openElement)
			{
				CloseStartElement();
			}
			Indent((elementNameStack.Count <= 0) ? string.Empty : (elementNameStack.Peek() as string), false);
			string text = localName;
			writer.Write('<');
			if (nsURI != string.Empty)
			{
				if (prefix != string.Empty)
				{
					text = prefix + ":" + localName;
				}
				if (nonHtmlDepth < 0)
				{
					nonHtmlDepth = elementNameStack.Count + 1;
				}
			}
			writer.Write(text);
			elementNameStack.Push(text);
			openElement = true;
		}

		private bool IsHtmlElement(string localName)
		{
			switch (localName.ToUpper(CultureInfo.InvariantCulture))
			{
			case "A":
			case "ABBR":
			case "ACRONYM":
			case "ADDRESS":
			case "APPLET":
			case "AREA":
			case "B":
			case "BASE":
			case "BASEFONT":
			case "BDO":
			case "BIG":
			case "BLOCKQUOTE":
			case "BODY":
			case "BR":
			case "BUTTON":
			case "CAPTION":
			case "CENTER":
			case "CITE":
			case "CODE":
			case "COL":
			case "COLGROUP":
			case "DD":
			case "DEL":
			case "DFN":
			case "DIR":
			case "DIV":
			case "DL":
			case "DT":
			case "EM":
			case "FIELDSET":
			case "FONT":
			case "FORM":
			case "FRAME":
			case "FRAMESET":
			case "H1":
			case "H2":
			case "H3":
			case "H4":
			case "H5":
			case "H6":
			case "HEAD":
			case "HR":
			case "HTML":
			case "I":
			case "IFRAME":
			case "IMG":
			case "INPUT":
			case "INS":
			case "ISINDEX":
			case "KBD":
			case "LABEL":
			case "LEGEND":
			case "LI":
			case "LINK":
			case "MAP":
			case "MENU":
			case "META":
			case "NOFRAMES":
			case "NOSCRIPT":
			case "OBJECT":
			case "OL":
			case "OPTGROUP":
			case "OPTION":
			case "P":
			case "PARAM":
			case "PRE":
			case "Q":
			case "S":
			case "SAMP":
			case "SCRIPT":
			case "SELECT":
			case "SMALL":
			case "SPAN":
			case "STRIKE":
			case "STRONG":
			case "STYLE":
			case "SUB":
			case "SUP":
			case "TABLE":
			case "TBODY":
			case "TD":
			case "TEXTAREA":
			case "TFOOT":
			case "TH":
			case "THEAD":
			case "TITLE":
			case "TR":
			case "TT":
			case "U":
			case "UL":
			case "VAR":
				return true;
			default:
				return false;
			}
		}

		public override void WriteEndElement()
		{
			WriteFullEndElement();
		}

		public override void WriteFullEndElement()
		{
			string text = elementNameStack.Peek() as string;
			switch (text.ToUpper(CultureInfo.InvariantCulture))
			{
			case "AREA":
			case "BASE":
			case "BASEFONT":
			case "BR":
			case "COL":
			case "FRAME":
			case "HR":
			case "IMG":
			case "INPUT":
			case "ISINDEX":
			case "LINK":
			case "META":
			case "PARAM":
				if (openAttribute)
				{
					CloseAttribute();
				}
				if (openElement)
				{
					writer.Write('>');
				}
				elementNameStack.Pop();
				break;
			default:
				if (openElement)
				{
					CloseStartElement();
				}
				elementNameStack.Pop();
				if (IsHtmlElement(text))
				{
					Indent(text, true);
				}
				writer.Write("</");
				writer.Write(text);
				writer.Write(">");
				break;
			}
			if (nonHtmlDepth > elementNameStack.Count)
			{
				nonHtmlDepth = -1;
			}
			openElement = false;
		}

		public override void WriteAttributeString(string prefix, string localName, string nsURI, string value)
		{
			writer.Write(' ');
			if (prefix != null && prefix.Length != 0)
			{
				writer.Write(prefix);
				writer.Write(":");
			}
			writer.Write(localName);
			if (nonHtmlDepth >= 0)
			{
				writer.Write("=\"");
				openAttribute = true;
				WriteFormattedString(value);
				openAttribute = false;
				writer.Write('"');
				return;
			}
			string text = localName.ToUpper(CultureInfo.InvariantCulture);
			string text2 = ((string)elementNameStack.Peek()).ToLower(CultureInfo.InvariantCulture);
			if ((text == "SELECTED" && text2 == "option") || (text == "CHECKED" && text2 == "input"))
			{
				return;
			}
			writer.Write("=\"");
			openAttribute = true;
			string text3 = null;
			string[] array = null;
			switch (text2)
			{
			case "q":
			case "blockquote":
			case "ins":
			case "del":
				text3 = "cite";
				break;
			case "form":
				text3 = "action";
				break;
			case "a":
			case "area":
			case "link":
			case "base":
				text3 = "href";
				break;
			case "head":
				text3 = "profile";
				break;
			case "input":
				array = new string[2] { "src", "usemap" };
				break;
			case "img":
				array = new string[3] { "src", "usemap", "longdesc" };
				break;
			case "object":
				array = new string[5] { "classid", "codebase", "data", "archive", "usemap" };
				break;
			case "script":
				array = new string[2] { "src", "for" };
				break;
			}
			if (array != null)
			{
				string text4 = localName.ToLower(CultureInfo.InvariantCulture);
				string[] array2 = array;
				foreach (string text5 in array2)
				{
					if (text5 == text4)
					{
						value = HtmlUriEscape.EscapeUri(value);
						break;
					}
				}
			}
			else if (text3 != null && text3 == localName.ToLower(CultureInfo.InvariantCulture))
			{
				value = HtmlUriEscape.EscapeUri(value);
			}
			WriteFormattedString(value);
			openAttribute = false;
			writer.Write('"');
		}

		public override void WriteComment(string text)
		{
			if (openElement)
			{
				CloseStartElement();
			}
			writer.Write("<!--");
			writer.Write(text);
			writer.Write("-->");
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			if (text.IndexOf("?>") > 0)
			{
				throw new ArgumentException("Processing instruction cannot contain \"?>\" as its value.");
			}
			if (openElement)
			{
				CloseStartElement();
			}
			if (elementNameStack.Count > 0)
			{
				Indent(elementNameStack.Peek() as string, false);
			}
			writer.Write("<?");
			writer.Write(name);
			if (text != null && text != string.Empty)
			{
				writer.Write(' ');
				writer.Write(text);
			}
			if (nonHtmlDepth >= 0)
			{
				writer.Write("?>");
			}
			else
			{
				writer.Write(">");
			}
		}

		public override void WriteString(string text)
		{
			if (openElement)
			{
				CloseStartElement();
			}
			WriteFormattedString(text);
		}

		private void WriteFormattedString(string text)
		{
			if (!openAttribute && elementNameStack.Count > 0)
			{
				switch (((string)elementNameStack.Peek()).ToUpper(CultureInfo.InvariantCulture))
				{
				case "SCRIPT":
				case "STYLE":
					writer.Write(text);
					return;
				}
			}
			int num = 0;
			for (int i = 0; i < text.Length; i++)
			{
				switch (text[i])
				{
				case '&':
					if (nonHtmlDepth >= 0 || i + 1 >= text.Length || text[i + 1] != '{')
					{
						writer.Write(text.ToCharArray(), num, i - num);
						writer.Write("&amp;");
						num = i + 1;
					}
					break;
				case '<':
					if (!openAttribute)
					{
						writer.Write(text.ToCharArray(), num, i - num);
						writer.Write("&lt;");
						num = i + 1;
					}
					break;
				case '>':
					if (!openAttribute)
					{
						writer.Write(text.ToCharArray(), num, i - num);
						writer.Write("&gt;");
						num = i + 1;
					}
					break;
				case '"':
					if (openAttribute)
					{
						writer.Write(text.ToCharArray(), num, i - num);
						writer.Write("&quot;");
						num = i + 1;
					}
					break;
				}
			}
			if (text.Length > num)
			{
				writer.Write(text.ToCharArray(), num, text.Length - num);
			}
		}

		public override void WriteRaw(string data)
		{
			if (openElement)
			{
				CloseStartElement();
			}
			writer.Write(data);
		}

		public override void WriteCDataSection(string text)
		{
			if (openElement)
			{
				CloseStartElement();
			}
			writer.Write(text);
		}

		public override void WriteWhitespace(string value)
		{
			if (openElement)
			{
				CloseStartElement();
			}
			writer.Write(value);
		}

		public override void Done()
		{
			writer.Flush();
		}
	}
}
