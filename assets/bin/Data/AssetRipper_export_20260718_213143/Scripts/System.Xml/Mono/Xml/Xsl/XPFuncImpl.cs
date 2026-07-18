using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal abstract class XPFuncImpl : IXsltContextFunction
	{
		private int minargs;

		private int maxargs;

		private XPathResultType returnType;

		private XPathResultType[] argTypes;

		public int Minargs
		{
			get
			{
				return minargs;
			}
		}

		public int Maxargs
		{
			get
			{
				return maxargs;
			}
		}

		public XPathResultType ReturnType
		{
			get
			{
				return returnType;
			}
		}

		public XPathResultType[] ArgTypes
		{
			get
			{
				return argTypes;
			}
		}

		public XPFuncImpl()
		{
		}

		public XPFuncImpl(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes)
		{
			Init(minArgs, maxArgs, returnType, argTypes);
		}

		protected void Init(int minArgs, int maxArgs, XPathResultType returnType, XPathResultType[] argTypes)
		{
			minargs = minArgs;
			maxargs = maxArgs;
			this.returnType = returnType;
			this.argTypes = argTypes;
		}

		public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
		{
			return Invoke((XsltCompiledContext)xsltContext, args, docContext);
		}

		public abstract object Invoke(XsltCompiledContext xsltContext, object[] args, XPathNavigator docContext);

		public static XPathResultType GetXPathType(Type type, XPathNavigator node)
		{
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.String:
				return XPathResultType.String;
			case TypeCode.Boolean:
				return XPathResultType.Boolean;
			case TypeCode.Object:
				if (typeof(XPathNavigator).IsAssignableFrom(type) || typeof(IXPathNavigable).IsAssignableFrom(type))
				{
					return XPathResultType.Navigator;
				}
				if (typeof(XPathNodeIterator).IsAssignableFrom(type))
				{
					return XPathResultType.NodeSet;
				}
				return XPathResultType.Any;
			case TypeCode.DateTime:
				throw new XsltException("Invalid type DateTime was specified.", null, node);
			default:
				return XPathResultType.Number;
			}
		}
	}
}
