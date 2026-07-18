using System.Collections;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Mono.Xml.XPath;

namespace Mono.Xml.Xsl.Operations
{
	internal class XslVariableInformation
	{
		private XmlQualifiedName name;

		private XPathExpression select;

		private XslOperation content;

		public XmlQualifiedName Name
		{
			get
			{
				return name;
			}
		}

		internal XPathExpression Select
		{
			get
			{
				return select;
			}
		}

		internal XslOperation Content
		{
			get
			{
				return content;
			}
		}

		public XslVariableInformation(Compiler c)
		{
			c.CheckExtraAttributes(c.Input.LocalName, "name", "select");
			c.AssertAttribute("name");
			name = c.ParseQNameAttribute("name");
			try
			{
				XmlConvert.VerifyName(name.Name);
			}
			catch (XmlException innerException)
			{
				throw new XsltCompileException("Variable name is not qualified name", innerException, c.Input);
			}
			string attribute = c.GetAttribute("select");
			if (attribute != null && attribute != string.Empty)
			{
				select = c.CompileExpression(c.GetAttribute("select"));
			}
			else if (c.Input.MoveToFirstChild())
			{
				content = c.CompileTemplateContent();
				c.Input.MoveToParent();
			}
		}

		public object Evaluate(XslTransformProcessor p)
		{
			if (select != null)
			{
				object obj = p.Evaluate(select);
				if (obj is XPathNodeIterator)
				{
					ArrayList arrayList = new ArrayList();
					XPathNodeIterator xPathNodeIterator = (XPathNodeIterator)obj;
					while (xPathNodeIterator.MoveNext())
					{
						arrayList.Add(xPathNodeIterator.Current.Clone());
					}
					obj = new ListIterator(arrayList, p.XPathContext);
				}
				return obj;
			}
			if (content != null)
			{
				DTMXPathDocumentWriter2 dTMXPathDocumentWriter = new DTMXPathDocumentWriter2(p.Root.NameTable, 200);
				Outputter newOutput = new GenericOutputter(dTMXPathDocumentWriter, p.Outputs, null, true);
				p.PushOutput(newOutput);
				if (p.CurrentNodeset.CurrentPosition == 0)
				{
					p.NodesetMoveNext();
				}
				content.Evaluate(p);
				p.PopOutput();
				return dTMXPathDocumentWriter.CreateDocument().CreateNavigator();
			}
			return string.Empty;
		}
	}
}
