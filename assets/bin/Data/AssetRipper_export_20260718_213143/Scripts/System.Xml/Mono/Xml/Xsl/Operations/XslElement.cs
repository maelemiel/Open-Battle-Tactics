using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslElement : XslCompiledElement
	{
		private XslAvt name;

		private XslAvt ns;

		private string calcName;

		private string calcNs;

		private string calcPrefix;

		private Hashtable nsDecls;

		private bool isEmptyElement;

		private XslOperation value;

		private XmlQualifiedName[] useAttributeSets;

		public XslElement(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("element", "name", "namespace", "use-attribute-sets");
			name = c.ParseAvtAttribute("name");
			ns = c.ParseAvtAttribute("namespace");
			nsDecls = c.GetNamespacesToCopy();
			calcName = XslAvt.AttemptPreCalc(ref name);
			if (calcName != null)
			{
				int num = calcName.IndexOf(':');
				if (num == 0)
				{
					throw new XsltCompileException("Invalid name attribute", null, c.Input);
				}
				calcPrefix = ((num >= 0) ? calcName.Substring(0, num) : string.Empty);
				if (num > 0)
				{
					calcName = calcName.Substring(num + 1);
				}
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
					throw new XsltCompileException("Invalid name attribute", innerException, c.Input);
				}
				if (ns == null)
				{
					calcNs = c.Input.GetNamespace(calcPrefix);
					if (calcPrefix != string.Empty && calcNs == string.Empty)
					{
						throw new XsltCompileException("Invalid name attribute", null, c.Input);
					}
				}
			}
			else if (ns != null)
			{
				calcNs = XslAvt.AttemptPreCalc(ref ns);
			}
			useAttributeSets = c.ParseQNameListAttribute("use-attribute-sets");
			isEmptyElement = c.Input.IsEmptyElement;
			if (c.Input.MoveToFirstChild())
			{
				value = c.CompileTemplateContent(XPathNodeType.Element);
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
			string text2 = ((calcNs != null) ? calcNs : ((ns == null) ? null : ns.Evaluate(p)));
			XmlQualifiedName xmlQualifiedName = XslNameUtil.FromString(text, nsDecls);
			string text3 = xmlQualifiedName.Name;
			if (text2 == null)
			{
				text2 = xmlQualifiedName.Namespace;
			}
			int num = text.IndexOf(':');
			if (num > 0)
			{
				calcPrefix = text.Substring(0, num);
			}
			else if (num == 0)
			{
				XmlConvert.VerifyNCName(string.Empty);
			}
			string text4 = ((calcPrefix == null) ? string.Empty : calcPrefix);
			if (text4 != string.Empty)
			{
				XmlConvert.VerifyNCName(text4);
			}
			XmlConvert.VerifyNCName(text3);
			bool insideCDataElement = p.InsideCDataElement;
			p.PushElementState(text4, text3, text2, false);
			p.Out.WriteStartElement(text4, text3, text2);
			if (useAttributeSets != null)
			{
				XmlQualifiedName[] array = useAttributeSets;
				foreach (XmlQualifiedName xmlQualifiedName2 in array)
				{
					p.ResolveAttributeSet(xmlQualifiedName2).Evaluate(p);
				}
			}
			if (value != null)
			{
				value.Evaluate(p);
			}
			if (isEmptyElement && useAttributeSets == null)
			{
				p.Out.WriteEndElement();
			}
			else
			{
				p.Out.WriteFullEndElement();
			}
			p.PopCDataState(insideCDataElement);
		}
	}
}
