using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl.Operations
{
	internal abstract class XslGeneralVariable : XslCompiledElement, IXsltContextVariable
	{
		protected XslVariableInformation var;

		public XmlQualifiedName Name
		{
			get
			{
				return var.Name;
			}
		}

		public XPathResultType VariableType
		{
			get
			{
				return XPathResultType.Any;
			}
		}

		public abstract bool IsLocal { get; }

		public abstract bool IsParam { get; }

		public XslGeneralVariable(Compiler c)
			: base(c)
		{
		}

		protected override void Compile(Compiler c)
		{
			if (c.Debugger != null)
			{
				c.Debugger.DebugCompile(base.DebugInput);
			}
			var = new XslVariableInformation(c);
		}

		public abstract override void Evaluate(XslTransformProcessor p);

		protected abstract object GetValue(XslTransformProcessor p);

		public object Evaluate(XsltContext xsltContext)
		{
			object value = GetValue(((XsltCompiledContext)xsltContext).Processor);
			if (value is XPathNodeIterator)
			{
				return new WrapperIterator(((XPathNodeIterator)value).Clone(), xsltContext);
			}
			return value;
		}
	}
}
