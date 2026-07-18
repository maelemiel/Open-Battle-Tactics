using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslAttribute : XslCompiledElement
	{
		private XslAvt name;

		private XslAvt ns;

		private string calcName;

		private string calcNs;

		private string calcPrefix;

		private Hashtable nsDecls;

		private XslOperation value;

		public XslAttribute(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			XPathNavigator xPathNavigator = c.Input.Clone();
			nsDecls = c.GetNamespacesToCopy();
			c.CheckExtraAttributes("attribute", "name", "namespace");
			name = c.ParseAvtAttribute("name");
			if (name == null)
			{
				throw new XsltCompileException("Attribute \"name\" is required on XSLT attribute element", null, c.Input);
			}
			ns = c.ParseAvtAttribute("namespace");
			calcName = XslAvt.AttemptPreCalc(ref name);
			calcPrefix = string.Empty;
			if (calcName != null)
			{
				int num = calcName.IndexOf(':');
				calcPrefix = ((num >= 0) ? calcName.Substring(0, num) : string.Empty);
				calcName = ((num >= 0) ? calcName.Substring(num + 1, calcName.Length - num - 1) : calcName);
				try
				{
					XmlConvert.VerifyNCName(calcName);
					if (calcPrefix != string.Empty)
					{
						XmlConvert.VerifyNCName(calcPrefix);
					}
				}
				catch (XmlException innerException)
				{
					throw new XsltCompileException("Invalid attribute name", innerException, c.Input);
				}
			}
			if (calcPrefix != string.Empty)
			{
				calcPrefix = c.CurrentStylesheet.GetActualPrefix(calcPrefix);
				if (calcPrefix == null)
				{
					calcPrefix = string.Empty;
				}
			}
			if (calcPrefix != string.Empty && ns == null)
			{
				calcNs = xPathNavigator.GetNamespace(calcPrefix);
			}
			else if (ns != null)
			{
				calcNs = XslAvt.AttemptPreCalc(ref ns);
			}
			if (c.Input.MoveToFirstChild())
			{
				value = c.CompileTemplateContent(XPathNodeType.Attribute);
				c.Input.MoveToParent();
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			string text = ((calcName == null) ? name.Evaluate(p) : calcName);
			string text2 = ((calcNs != null) ? calcNs : ((ns == null) ? string.Empty : ns.Evaluate(p)));
			string text3 = ((calcPrefix == null) ? string.Empty : calcPrefix);
			if (text == "xmlns")
			{
				return;
			}
			int num = text.IndexOf(':');
			if (num > 0)
			{
				text3 = text.Substring(0, num);
				text = text.Substring(num + 1);
				if (text2 == string.Empty && text3 == "xml")
				{
					text2 = "http://www.w3.org/XML/1998/namespace";
				}
				else if (text2 == string.Empty)
				{
					text2 = (string)nsDecls[text3];
					if (text2 == null)
					{
						text2 = string.Empty;
					}
				}
			}
			if (text3 == "xmlns")
			{
				text3 = string.Empty;
			}
			XmlConvert.VerifyName(text);
			p.Out.WriteAttributeString(text3, text, text2, (value != null) ? value.EvaluateAsString(p) : string.Empty);
		}
	}
}
