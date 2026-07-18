using System;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath;
using Mono.Xml.Xsl.Operations;

namespace Mono.Xml.Xsl
{
	internal class XslTemplate
	{
		private XmlQualifiedName name;

		private Pattern match;

		private XmlQualifiedName mode;

		private double priority = double.NaN;

		private ArrayList parameters;

		private XslOperation content;

		private static int nextId;

		public readonly int Id = nextId++;

		private XslStylesheet style;

		private int stackSize;

		public XmlQualifiedName Name
		{
			get
			{
				return name;
			}
		}

		public Pattern Match
		{
			get
			{
				return match;
			}
		}

		public XmlQualifiedName Mode
		{
			get
			{
				return mode;
			}
		}

		public double Priority
		{
			get
			{
				return priority;
			}
		}

		public XslStylesheet Parent
		{
			get
			{
				return style;
			}
		}

		private string LocationMessage
		{
			get
			{
				XslCompiledElementBase xslCompiledElementBase = (XslCompiledElementBase)content;
				return string.Format(" from\nxsl:template {0} at {1} ({2},{3})", Match, style.BaseURI, xslCompiledElementBase.LineNumber, xslCompiledElementBase.LinePosition);
			}
		}

		public XslTemplate(Compiler c)
		{
			if (c == null)
			{
				return;
			}
			style = c.CurrentStylesheet;
			c.PushScope();
			if (c.Input.Name == "template" && c.Input.NamespaceURI == "http://www.w3.org/1999/XSL/Transform" && c.Input.MoveToAttribute("mode", string.Empty))
			{
				c.Input.MoveToParent();
				if (!c.Input.MoveToAttribute("match", string.Empty))
				{
					throw new XsltCompileException("XSLT 'template' element must not have 'mode' attribute when it does not have 'match' attribute", null, c.Input);
				}
				c.Input.MoveToParent();
			}
			if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
			{
				name = XmlQualifiedName.Empty;
				match = c.CompilePattern("/", c.Input);
				mode = XmlQualifiedName.Empty;
			}
			else
			{
				name = c.ParseQNameAttribute("name");
				match = c.CompilePattern(c.GetAttribute("match"), c.Input);
				mode = c.ParseQNameAttribute("mode");
				string attribute = c.GetAttribute("priority");
				if (attribute != null)
				{
					try
					{
						priority = double.Parse(attribute, CultureInfo.InvariantCulture);
					}
					catch (FormatException innerException)
					{
						throw new XsltException("Invalid priority number format", innerException, c.Input);
					}
				}
			}
			Parse(c);
			stackSize = c.PopScope().VariableHighTide;
		}

		private void Parse(Compiler c)
		{
			if (c.Input.NamespaceURI != "http://www.w3.org/1999/XSL/Transform")
			{
				content = c.CompileTemplateContent();
			}
			else
			{
				if (!c.Input.MoveToFirstChild())
				{
					return;
				}
				bool flag = true;
				XPathNavigator xPathNavigator = c.Input.Clone();
				bool flag2 = false;
				do
				{
					if (flag2)
					{
						flag2 = false;
						xPathNavigator.MoveTo(c.Input);
					}
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
						if (c.Input.LocalName != "param")
						{
							flag = false;
							break;
						}
						if (parameters == null)
						{
							parameters = new ArrayList();
						}
						parameters.Add(new XslLocalParam(c));
						flag2 = true;
					}
				}
				while (c.Input.MoveToNext());
				if (!flag)
				{
					c.Input.MoveTo(xPathNavigator);
					content = c.CompileTemplateContent();
				}
				c.Input.MoveToParent();
			}
		}

		private void AppendTemplateFrame(XsltException ex)
		{
			ex.AddTemplateFrame(LocationMessage);
		}

		public virtual void Evaluate(XslTransformProcessor p, Hashtable withParams)
		{
			if (XslTransform.TemplateStackFrameError)
			{
				try
				{
					EvaluateCore(p, withParams);
					return;
				}
				catch (XsltException ex)
				{
					AppendTemplateFrame(ex);
					throw ex;
				}
				catch (Exception)
				{
					XsltException ex3 = new XsltException("Error during XSLT processing: ", null, p.CurrentNode);
					AppendTemplateFrame(ex3);
					throw ex3;
				}
			}
			EvaluateCore(p, withParams);
		}

		private void EvaluateCore(XslTransformProcessor p, Hashtable withParams)
		{
			if (XslTransform.TemplateStackFrameOutput != null)
			{
				XslTransform.TemplateStackFrameOutput.WriteLine(LocationMessage);
			}
			p.PushStack(stackSize);
			if (parameters != null)
			{
				if (withParams == null)
				{
					int count = parameters.Count;
					for (int i = 0; i < count; i++)
					{
						XslLocalParam xslLocalParam = (XslLocalParam)parameters[i];
						xslLocalParam.Evaluate(p);
					}
				}
				else
				{
					int count2 = parameters.Count;
					for (int j = 0; j < count2; j++)
					{
						XslLocalParam xslLocalParam2 = (XslLocalParam)parameters[j];
						object obj = withParams[xslLocalParam2.Name];
						if (obj != null)
						{
							xslLocalParam2.Override(p, obj);
						}
						else
						{
							xslLocalParam2.Evaluate(p);
						}
					}
				}
			}
			if (content != null)
			{
				content.Evaluate(p);
			}
			p.PopStack();
		}

		public void Evaluate(XslTransformProcessor p)
		{
			Evaluate(p, null);
		}
	}
}
