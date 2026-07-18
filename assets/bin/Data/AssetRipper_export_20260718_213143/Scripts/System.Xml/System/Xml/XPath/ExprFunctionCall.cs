using System.Collections;
using System.Xml.Xsl;

namespace System.Xml.XPath
{
	internal class ExprFunctionCall : Expression
	{
		protected readonly XmlQualifiedName _name;

		protected readonly bool resolvedName;

		protected readonly ArrayList _args = new ArrayList();

		public override XPathResultType ReturnType
		{
			get
			{
				return XPathResultType.Any;
			}
		}

		internal override bool Peer
		{
			get
			{
				return false;
			}
		}

		public ExprFunctionCall(XmlQualifiedName name, FunctionArguments args, IStaticXsltContext ctx)
		{
			if (ctx != null)
			{
				name = ctx.LookupQName(name.ToString());
				resolvedName = true;
			}
			_name = name;
			if (args != null)
			{
				args.ToArrayList(_args);
			}
		}

		public static Expression Factory(XmlQualifiedName name, FunctionArguments args, IStaticXsltContext ctx)
		{
			if (name.Namespace != null && name.Namespace != string.Empty)
			{
				return new ExprFunctionCall(name, args, ctx);
			}
			switch (name.Name)
			{
			case "last":
				return new XPathFunctionLast(args);
			case "position":
				return new XPathFunctionPosition(args);
			case "count":
				return new XPathFunctionCount(args);
			case "id":
				return new XPathFunctionId(args);
			case "local-name":
				return new XPathFunctionLocalName(args);
			case "namespace-uri":
				return new XPathFunctionNamespaceUri(args);
			case "name":
				return new XPathFunctionName(args);
			case "string":
				return new XPathFunctionString(args);
			case "concat":
				return new XPathFunctionConcat(args);
			case "starts-with":
				return new XPathFunctionStartsWith(args);
			case "contains":
				return new XPathFunctionContains(args);
			case "substring-before":
				return new XPathFunctionSubstringBefore(args);
			case "substring-after":
				return new XPathFunctionSubstringAfter(args);
			case "substring":
				return new XPathFunctionSubstring(args);
			case "string-length":
				return new XPathFunctionStringLength(args);
			case "normalize-space":
				return new XPathFunctionNormalizeSpace(args);
			case "translate":
				return new XPathFunctionTranslate(args);
			case "boolean":
				return new XPathFunctionBoolean(args);
			case "not":
				return new XPathFunctionNot(args);
			case "true":
				return new XPathFunctionTrue(args);
			case "false":
				return new XPathFunctionFalse(args);
			case "lang":
				return new XPathFunctionLang(args);
			case "number":
				return new XPathFunctionNumber(args);
			case "sum":
				return new XPathFunctionSum(args);
			case "floor":
				return new XPathFunctionFloor(args);
			case "ceiling":
				return new XPathFunctionCeil(args);
			case "round":
				return new XPathFunctionRound(args);
			default:
				return new ExprFunctionCall(name, args, ctx);
			}
		}

		public override string ToString()
		{
			string text = string.Empty;
			for (int i = 0; i < _args.Count; i++)
			{
				Expression expression = (Expression)_args[i];
				if (text != string.Empty)
				{
					text += ", ";
				}
				text += expression.ToString();
			}
			return _name.ToString() + '(' + text + ')';
		}

		public override XPathResultType GetReturnType(BaseIterator iter)
		{
			return XPathResultType.Any;
		}

		private XPathResultType[] GetArgTypes(BaseIterator iter)
		{
			XPathResultType[] array = new XPathResultType[_args.Count];
			for (int i = 0; i < _args.Count; i++)
			{
				array[i] = ((Expression)_args[i]).GetReturnType(iter);
			}
			return array;
		}

		public override object Evaluate(BaseIterator iter)
		{
			XPathResultType[] argTypes = GetArgTypes(iter);
			IXsltContextFunction xsltContextFunction = null;
			XsltContext xsltContext = iter.NamespaceManager as XsltContext;
			if (xsltContext != null)
			{
				xsltContextFunction = ((!resolvedName) ? xsltContext.ResolveFunction(_name.Namespace, _name.Name, argTypes) : xsltContext.ResolveFunction(_name, argTypes));
			}
			if (xsltContextFunction == null)
			{
				throw new XPathException("function " + _name.ToString() + " not found");
			}
			object[] array = new object[_args.Count];
			if (xsltContextFunction.Maxargs != 0)
			{
				XPathResultType[] argTypes2 = xsltContextFunction.ArgTypes;
				for (int i = 0; i < _args.Count; i++)
				{
					XPathResultType type = ((argTypes2 != null) ? ((i >= argTypes2.Length) ? argTypes2[argTypes2.Length - 1] : argTypes2[i]) : XPathResultType.Any);
					Expression expression = (Expression)_args[i];
					object obj = expression.EvaluateAs(iter, type);
					array[i] = obj;
				}
			}
			return xsltContextFunction.Invoke(xsltContext, array, iter.Current);
		}
	}
}
