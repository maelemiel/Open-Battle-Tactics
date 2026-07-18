using System.Globalization;

namespace System.Xml
{
	public class XmlDeclaration : XmlLinkedNode
	{
		private string encoding = "UTF-8";

		private string standalone;

		private string version;

		public string Encoding
		{
			get
			{
				return encoding;
			}
			set
			{
				encoding = ((value != null) ? value : string.Empty);
			}
		}

		public override string InnerText
		{
			get
			{
				return Value;
			}
			set
			{
				ParseInput(value);
			}
		}

		public override string LocalName
		{
			get
			{
				return "xml";
			}
		}

		public override string Name
		{
			get
			{
				return "xml";
			}
		}

		public override XmlNodeType NodeType
		{
			get
			{
				return XmlNodeType.XmlDeclaration;
			}
		}

		public string Standalone
		{
			get
			{
				return standalone;
			}
			set
			{
				if (value != null)
				{
					if (string.Compare(value, "YES", true, CultureInfo.InvariantCulture) == 0)
					{
						standalone = "yes";
					}
					if (string.Compare(value, "NO", true, CultureInfo.InvariantCulture) == 0)
					{
						standalone = "no";
					}
				}
				else
				{
					standalone = string.Empty;
				}
			}
		}

		public override string Value
		{
			get
			{
				string arg = string.Empty;
				string arg2 = string.Empty;
				if (encoding != string.Empty)
				{
					arg = string.Format(" encoding=\"{0}\"", encoding);
				}
				if (standalone != string.Empty)
				{
					arg2 = string.Format(" standalone=\"{0}\"", standalone);
				}
				return string.Format("version=\"{0}\"{1}{2}", Version, arg, arg2);
			}
			set
			{
				ParseInput(value);
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
		}

		protected internal XmlDeclaration(string version, string encoding, string standalone, XmlDocument doc)
			: base(doc)
		{
			if (encoding == null)
			{
				encoding = string.Empty;
			}
			if (standalone == null)
			{
				standalone = string.Empty;
			}
			this.version = version;
			this.encoding = encoding;
			this.standalone = standalone;
		}

		public override XmlNode CloneNode(bool deep)
		{
			return new XmlDeclaration(Version, Encoding, standalone, OwnerDocument);
		}

		public override void WriteContentTo(XmlWriter w)
		{
		}

		public override void WriteTo(XmlWriter w)
		{
			w.WriteRaw(string.Format("<?xml {0}?>", Value));
		}

		private int SkipWhitespace(string input, int index)
		{
			while (index < input.Length && XmlChar.IsWhitespace(input[index]))
			{
				index++;
			}
			return index;
		}

		private void ParseInput(string input)
		{
			int num = SkipWhitespace(input, 0);
			if (num + 7 > input.Length || input.IndexOf("version", num, 7) != num)
			{
				throw new XmlException("Missing 'version' specification.");
			}
			num = SkipWhitespace(input, num + 7);
			char c = input[num];
			if (c != '=')
			{
				throw new XmlException("Invalid 'version' specification.");
			}
			num++;
			num = SkipWhitespace(input, num);
			c = input[num];
			if (c != '"' && c != '\'')
			{
				throw new XmlException("Invalid 'version' specification.");
			}
			num++;
			int num2 = input.IndexOf(c, num);
			if (num2 < 0 || input.IndexOf("1.0", num, 3) != num)
			{
				throw new XmlException("Invalid 'version' specification.");
			}
			num += 4;
			if (num == input.Length)
			{
				return;
			}
			if (!XmlChar.IsWhitespace(input[num]))
			{
				throw new XmlException("Invalid XML declaration.");
			}
			num = SkipWhitespace(input, num + 1);
			if (num == input.Length)
			{
				return;
			}
			if (input.Length > num + 8 && input.IndexOf("encoding", num, 8) > 0)
			{
				num = SkipWhitespace(input, num + 8);
				c = input[num];
				if (c != '=')
				{
					throw new XmlException("Invalid 'version' specification.");
				}
				num++;
				num = SkipWhitespace(input, num);
				c = input[num];
				if (c != '"' && c != '\'')
				{
					throw new XmlException("Invalid 'encoding' specification.");
				}
				num2 = input.IndexOf(c, num + 1);
				if (num2 < 0)
				{
					throw new XmlException("Invalid 'encoding' specification.");
				}
				Encoding = input.Substring(num + 1, num2 - num - 1);
				num = num2 + 1;
				if (num == input.Length)
				{
					return;
				}
				if (!XmlChar.IsWhitespace(input[num]))
				{
					throw new XmlException("Invalid XML declaration.");
				}
				num = SkipWhitespace(input, num + 1);
			}
			if (input.Length > num + 10 && input.IndexOf("standalone", num, 10) > 0)
			{
				num = SkipWhitespace(input, num + 10);
				c = input[num];
				if (c != '=')
				{
					throw new XmlException("Invalid 'version' specification.");
				}
				num++;
				num = SkipWhitespace(input, num);
				c = input[num];
				if (c != '"' && c != '\'')
				{
					throw new XmlException("Invalid 'standalone' specification.");
				}
				num2 = input.IndexOf(c, num + 1);
				if (num2 < 0)
				{
					throw new XmlException("Invalid 'standalone' specification.");
				}
				string text = input.Substring(num + 1, num2 - num - 1);
				switch (text)
				{
				default:
					throw new XmlException("Invalid standalone specification.");
				case "yes":
				case "no":
					break;
				}
				Standalone = text;
				num = num2 + 1;
				num = SkipWhitespace(input, num);
			}
			if (num == input.Length)
			{
				return;
			}
			throw new XmlException("Invalid XML declaration.");
		}
	}
}
