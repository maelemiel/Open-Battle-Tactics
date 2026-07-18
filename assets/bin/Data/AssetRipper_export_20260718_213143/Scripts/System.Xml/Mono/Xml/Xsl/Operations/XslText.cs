using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslText : XslCompiledElement
	{
		private bool disableOutputEscaping;

		private string text = string.Empty;

		private bool isWhitespace;

		public XslText(Compiler c, bool isWhitespace)
			: base(c)
		{
			this.isWhitespace = isWhitespace;
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			c.CheckExtraAttributes("text", "disable-output-escaping");
			text = c.Input.Value;
			if (c.Input.NodeType == XPathNodeType.Element)
			{
				disableOutputEscaping = c.ParseYesNoAttribute("disable-output-escaping", false);
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			if (!disableOutputEscaping)
			{
				if (isWhitespace)
				{
					p.Out.WriteWhitespace(text);
				}
				else
				{
					p.Out.WriteString(text);
				}
			}
			else
			{
				p.Out.WriteRaw(text);
			}
		}
	}
}
