using System.Text;
using System.Xml;

namespace Mono.Xml.Xsl
{
	internal class XmlWriterEmitter : Emitter
	{
		private XmlWriter writer;

		public XmlWriterEmitter(XmlWriter writer)
		{
			this.writer = writer;
		}

		public override void WriteStartDocument(Encoding encoding, StandaloneType standalone)
		{
			string text = string.Empty;
			switch (standalone)
			{
			case StandaloneType.YES:
				text = " standalone=\"yes\"";
				break;
			case StandaloneType.NO:
				text = " standalone=\"no\"";
				break;
			}
			if (encoding == null)
			{
				writer.WriteProcessingInstruction("xml", "version=\"1.0\"" + text);
			}
			else
			{
				writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"" + encoding.WebName + "\"" + text);
			}
		}

		public override void WriteEndDocument()
		{
		}

		public override void WriteDocType(string type, string publicId, string systemId)
		{
			if (systemId != null)
			{
				writer.WriteDocType(type, publicId, systemId, null);
			}
		}

		public override void WriteStartElement(string prefix, string localName, string nsURI)
		{
			writer.WriteStartElement(prefix, localName, nsURI);
		}

		public override void WriteEndElement()
		{
			writer.WriteEndElement();
		}

		public override void WriteFullEndElement()
		{
			writer.WriteFullEndElement();
		}

		public override void WriteAttributeString(string prefix, string localName, string nsURI, string value)
		{
			writer.WriteAttributeString(prefix, localName, nsURI, value);
		}

		public override void WriteComment(string text)
		{
			while (text.IndexOf("--") >= 0)
			{
				text = text.Replace("--", "- -");
			}
			if (text.EndsWith("-"))
			{
				text += ' ';
			}
			writer.WriteComment(text);
		}

		public override void WriteProcessingInstruction(string name, string text)
		{
			while (text.IndexOf("?>") >= 0)
			{
				text = text.Replace("?>", "? >");
			}
			writer.WriteProcessingInstruction(name, text);
		}

		public override void WriteString(string text)
		{
			writer.WriteString(text);
		}

		public override void WriteRaw(string data)
		{
			writer.WriteRaw(data);
		}

		public override void WriteCDataSection(string text)
		{
			int num = text.IndexOf("]]>");
			if (num >= 0)
			{
				writer.WriteCData(text.Substring(0, num + 2));
				WriteCDataSection(text.Substring(num + 2));
			}
			else
			{
				writer.WriteCData(text);
			}
		}

		public override void WriteWhitespace(string value)
		{
			writer.WriteWhitespace(value);
		}

		public override void Done()
		{
			writer.Flush();
		}
	}
}
