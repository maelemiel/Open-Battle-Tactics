using System.Collections;
using System.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslForEach : XslCompiledElement
	{
		private XPathExpression select;

		private XslOperation children;

		private XslSortEvaluator sortEvaluator;

		public XslForEach(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(c.Input);
			}
			c.CheckExtraAttributes("for-each", "select");
			c.AssertAttribute("select");
			select = c.CompileExpression(c.GetAttribute("select"));
			ArrayList arrayList = null;
			if (c.Input.MoveToFirstChild())
			{
				bool flag = true;
				do
				{
					if (c.Input.NodeType == XPathNodeType.Text)
					{
						flag = false;
						break;
					}
					if (c.Input.NodeType == XPathNodeType.Element)
					{
						if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
						{
							flag = false;
							break;
						}
						if (c.Input.LocalName != "sort")
						{
							flag = false;
							break;
						}
						if (arrayList == null)
						{
							arrayList = new ArrayList();
						}
						arrayList.Add(new Sort(c));
					}
				}
				while (c.Input.MoveToNext());
				if (!flag)
				{
					children = c.CompileTemplateContent();
				}
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
			XPathNodeIterator xPathNodeIterator = ((sortEvaluator == null) ? p.Select(select) : sortEvaluator.SortedSelect(p));
			while (p.NodesetMoveNext(xPathNodeIterator))
			{
				p.PushNodeset(xPathNodeIterator);
				p.PushForEachContext();
				children.Evaluate(p);
				p.PopForEachContext();
				p.PopNodeset();
			}
		}
	}
}
