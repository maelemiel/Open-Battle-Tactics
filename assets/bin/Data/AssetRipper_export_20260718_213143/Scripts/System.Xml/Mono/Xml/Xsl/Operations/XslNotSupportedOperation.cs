using System.Collections;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslNotSupportedOperation : XslCompiledElement
	{
		private string name;

		private ArrayList fallbacks;

		public XslNotSupportedOperation(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			name = c.Input.LocalName;
			if (!c.Input.MoveToFirstChild())
			{
				return;
			}
			do
			{
				if (c.Input.NodeType == XPathNodeType.Element && !(c.Input.LocalName != "fallback") && !(c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform"))
				{
					if (fallbacks == null)
					{
						fallbacks = new ArrayList();
					}
					fallbacks.Add(new XslFallback(c));
				}
			}
			while (c.Input.MoveToNext());
			c.Input.MoveToParent();
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			if (fallbacks != null)
			{
				foreach (XslFallback fallback in fallbacks)
				{
					fallback.Evaluate(p);
				}
				return;
			}
			throw new XsltException(string.Format("'{0}' element is not supported as a template content in XSLT 1.0.", name), null);
		}
	}
}
