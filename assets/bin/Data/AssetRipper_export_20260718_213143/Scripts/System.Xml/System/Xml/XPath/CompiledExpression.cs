using System.Collections;
using System.Xml.Xsl;

namespace System.Xml.XPath
{
	internal class CompiledExpression : XPathExpression
	{
		protected IXmlNamespaceResolver _nsm;

		protected Expression _expr;

		private XPathSorters _sorters;

		private string rawExpression;

		public Expression ExpressionNode
		{
			get
			{
				return _expr;
			}
		}

		internal IXmlNamespaceResolver NamespaceManager
		{
			get
			{
				return _nsm;
			}
		}

		public override string Expression
		{
			get
			{
				return rawExpression;
			}
		}

		public override XPathResultType ReturnType
		{
			get
			{
				return _expr.ReturnType;
			}
		}

		public CompiledExpression(string raw, Expression expr)
		{
			_expr = expr.Optimize();
			rawExpression = raw;
		}

		private CompiledExpression(CompiledExpression other)
		{
			_nsm = other._nsm;
			_expr = other._expr;
			rawExpression = other.rawExpression;
		}

		public override XPathExpression Clone()
		{
			return new CompiledExpression(this);
		}

		public override void SetContext(XmlNamespaceManager nsManager)
		{
			_nsm = nsManager;
		}

		public override void SetContext(IXmlNamespaceResolver nsResolver)
		{
			_nsm = nsResolver;
		}

		public object Evaluate(BaseIterator iter)
		{
			if (_sorters != null)
			{
				return EvaluateNodeSet(iter);
			}
			try
			{
				return _expr.Evaluate(iter);
			}
			catch (XPathException)
			{
				throw;
			}
			catch (XsltException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				throw new XPathException("Error during evaluation", innerException);
			}
		}

		public XPathNodeIterator EvaluateNodeSet(BaseIterator iter)
		{
			BaseIterator baseIterator = _expr.EvaluateNodeSet(iter);
			if (_sorters != null)
			{
				return _sorters.Sort(baseIterator);
			}
			return baseIterator;
		}

		public double EvaluateNumber(BaseIterator iter)
		{
			return _expr.EvaluateNumber(iter);
		}

		public string EvaluateString(BaseIterator iter)
		{
			return _expr.EvaluateString(iter);
		}

		public bool EvaluateBoolean(BaseIterator iter)
		{
			return _expr.EvaluateBoolean(iter);
		}

		public override void AddSort(object obj, IComparer cmp)
		{
			if (_sorters == null)
			{
				_sorters = new XPathSorters();
			}
			_sorters.Add(obj, cmp);
		}

		public override void AddSort(object expr, XmlSortOrder orderSort, XmlCaseOrder orderCase, string lang, XmlDataType dataType)
		{
			if (_sorters == null)
			{
				_sorters = new XPathSorters();
			}
			_sorters.Add(expr, orderSort, orderCase, lang, dataType);
		}
	}
}
