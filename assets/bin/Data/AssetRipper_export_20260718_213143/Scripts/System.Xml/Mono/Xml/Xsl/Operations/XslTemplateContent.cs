using System.Collections;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslTemplateContent : XslCompiledElementBase
	{
		private ArrayList content = new ArrayList();

		private bool hasStack;

		private int stackSize;

		private XPathNodeType parentType;

		private bool xslForEach;

		public XPathNodeType ParentType
		{
			get
			{
				return parentType;
			}
		}

		public XslTemplateContent(Compiler c, XPathNodeType parentType, bool xslForEach)
			: base(c)
		{
			this.parentType = parentType;
			this.xslForEach = xslForEach;
			Compile(c);
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			hasStack = c.CurrentVariableScope == null;
			c.PushScope();
			do
			{
				XPathNavigator input = c.Input;
				switch (input.NodeType)
				{
				case XPathNodeType.Element:
					switch (input.NamespaceURI)
					{
					case "http://www.w3.org/1999/XSL/Transform":
						switch (input.LocalName)
						{
						case "apply-imports":
							content.Add(new XslApplyImports(c));
							break;
						case "apply-templates":
							content.Add(new XslApplyTemplates(c));
							break;
						case "attribute":
							if (ParentType == XPathNodeType.All || ParentType == XPathNodeType.Element)
							{
								content.Add(new XslAttribute(c));
							}
							break;
						case "call-template":
							content.Add(new XslCallTemplate(c));
							break;
						case "choose":
							content.Add(new XslChoose(c));
							break;
						case "comment":
							if (ParentType == XPathNodeType.All || ParentType == XPathNodeType.Element)
							{
								content.Add(new XslComment(c));
							}
							break;
						case "copy":
							content.Add(new XslCopy(c));
							break;
						case "copy-of":
							content.Add(new XslCopyOf(c));
							break;
						case "element":
							if (ParentType == XPathNodeType.All || ParentType == XPathNodeType.Element)
							{
								content.Add(new XslElement(c));
							}
							break;
						case "for-each":
							content.Add(new XslForEach(c));
							break;
						case "if":
							content.Add(new XslIf(c));
							break;
						case "message":
							content.Add(new XslMessage(c));
							break;
						case "number":
							content.Add(new XslNumber(c));
							break;
						case "processing-instruction":
							if (ParentType == XPathNodeType.All || ParentType == XPathNodeType.Element)
							{
								content.Add(new XslProcessingInstruction(c));
							}
							break;
						case "text":
							content.Add(new XslText(c, false));
							break;
						case "value-of":
							content.Add(new XslValueOf(c));
							break;
						case "variable":
							content.Add(new XslLocalVariable(c));
							break;
						case "sort":
							if (xslForEach)
							{
								break;
							}
							throw new XsltCompileException("'sort' element is not allowed here as a templete content", null, input);
						default:
							content.Add(new XslNotSupportedOperation(c));
							break;
						case "fallback":
							break;
						}
						break;
					default:
						if (!c.IsExtensionNamespace(input.NamespaceURI))
						{
							content.Add(new XslLiteralElement(c));
						}
						else
						{
							if (!input.MoveToFirstChild())
							{
								break;
							}
							do
							{
								if (input.NamespaceURI == "http://www.w3.org/1999/XSL/Transform" && input.LocalName == "fallback")
								{
									content.Add(new XslFallback(c));
								}
							}
							while (input.MoveToNext());
							input.MoveToParent();
						}
						break;
					}
					break;
				case XPathNodeType.SignificantWhitespace:
					content.Add(new XslText(c, true));
					break;
				case XPathNodeType.Text:
					content.Add(new XslText(c, false));
					break;
				}
			}
			while (c.Input.MoveToNext());
			if (hasStack)
			{
				stackSize = c.PopScope().VariableHighTide;
				hasStack = stackSize > 0;
			}
			else
			{
				c.PopScope();
			}
		}

		public override void Evaluate(XslTransformProcessor p)
		{
			if (p.Debugger != null)
			{
				p.Debugger.DebugExecute(p, base.DebugInput);
			}
			if (hasStack)
			{
				p.PushStack(stackSize);
			}
			int count = content.Count;
			for (int i = 0; i < count; i++)
			{
				((XslOperation)content[i]).Evaluate(p);
			}
			if (hasStack)
			{
				p.PopStack();
			}
		}
	}
}
