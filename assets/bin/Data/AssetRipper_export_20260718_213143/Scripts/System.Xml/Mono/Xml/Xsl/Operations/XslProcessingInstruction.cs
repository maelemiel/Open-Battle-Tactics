using System.Globalization;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslProcessingInstruction : XslCompiledElement
	{
		private XslAvt name;

		private XslOperation value;

		public XslProcessingInstruction(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			c.CheckExtraAttributes("processing-instruction", "name");
			name = c.ParseAvtAttribute("name");
			if (c.Input.MoveToFirstChild())
			{
				value = c.CompileTemplateContent(XPathNodeType.ProcessingInstruction);
				c.Input.MoveToParent();
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			string text = name.Evaluate(p);
			if (string.Compare(text, "xml", true, CultureInfo.InvariantCulture) == 0)
			{
				throw new XsltException("Processing instruction name was evaluated to \"xml\"", null, p.CurrentNode);
			}
			if (text.IndexOf(':') < 0)
			{
				p.Out.WriteProcessingInstruction(text, (value != null) ? value.EvaluateAsString(p) : string.Empty);
			}
		}
	}
}
