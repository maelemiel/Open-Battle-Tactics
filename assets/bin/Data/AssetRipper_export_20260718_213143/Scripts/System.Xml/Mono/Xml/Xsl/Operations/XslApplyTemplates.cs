using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslApplyTemplates : XslCompiledElement
	{
		private XPathExpression select;

		private XmlQualifiedName mode;

		private ArrayList withParams;

		private XslSortEvaluator sortEvaluator;

		public XslApplyTemplates(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("apply-templates", "select", "mode");
			select = c.CompileExpression(c.GetAttribute("select"));
			mode = c.ParseQNameAttribute("mode");
			ArrayList arrayList = null;
			if (c.Input.MoveToFirstChild())
			{
				do
				{
					switch (c.Input.NodeType)
					{
					case XPathNodeType.Element:
						if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
						{
							throw new XsltCompileException("Unexpected element", null, c.Input);
						}
						switch (c.Input.LocalName)
						{
						case "with-param":
							if (withParams == null)
							{
								withParams = new ArrayList();
							}
							withParams.Add(new XslVariableInformation(c));
							break;
						case "sort":
							if (arrayList == null)
							{
								arrayList = new ArrayList();
							}
							if (select == null)
							{
								select = c.CompileExpression("*");
							}
							arrayList.Add(new Sort(c));
							break;
						default:
							throw new XsltCompileException("Unexpected element", null, c.Input);
						}
						break;
					default:
						throw new XsltCompileException("Unexpected node type " + c.Input.NodeType, null, c.Input);
					case XPathNodeType.SignificantWhitespace:
					case XPathNodeType.Whitespace:
					case XPathNodeType.ProcessingInstruction:
					case XPathNodeType.Comment:
						break;
					}
				}
				while (c.Input.MoveToNext());
				c.Input.MoveToParent();
			}
			if (arrayList != null)
			{
				sortEvaluator = new XslSortEvaluator(select, (Sort[])arrayList.ToArray(typeof(Sort)));
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			if (select == null)
			{
				p.ApplyTemplates(p.CurrentNode.SelectChildren(XPathNodeType.All), mode, withParams);
				return;
			}
			XPathNodeIterator nodes = ((sortEvaluator == null) ? p.Select(select) : sortEvaluator.SortedSelect(p));
			p.ApplyTemplates(nodes, mode, withParams);
		}
	}
}
