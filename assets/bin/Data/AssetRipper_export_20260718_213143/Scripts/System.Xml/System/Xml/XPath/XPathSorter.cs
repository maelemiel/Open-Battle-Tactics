using System.Collections;
using System.Globalization;
using Mono.Xml.XPath;

namespace System.Xml.XPath
{
	internal class XPathSorter
	{
		private class XPathNumberComparer : IComparer
		{
			private int _nMulSort;

			public XPathNumberComparer(XmlSortOrder orderSort)
			{
				_nMulSort = ((orderSort == XmlSortOrder.Ascending) ? 1 : (-1));
			}

			int IComparer.Compare(object o1, object o2)
			{
				double num = (double)o1;
				double num2 = (double)o2;
				if (num < num2)
				{
					return -_nMulSort;
				}
				if (num > num2)
				{
					return _nMulSort;
				}
				if (num == num2)
				{
					return 0;
				}
				if (double.IsNaN(num))
				{
					return (!double.IsNaN(num2)) ? (-_nMulSort) : 0;
				}
				return _nMulSort;
			}
		}

		private class XPathTextComparer : IComparer
		{
			private int _nMulSort;

			private int _nMulCase;

			private XmlCaseOrder _orderCase;

			private CultureInfo _ci;

			public XPathTextComparer(XmlSortOrder orderSort, XmlCaseOrder orderCase, string strLang)
			{
				_orderCase = orderCase;
				_nMulCase = ((orderCase != XmlCaseOrder.UpperFirst) ? 1 : (-1));
				_nMulSort = ((orderSort == XmlSortOrder.Ascending) ? 1 : (-1));
				if (strLang == null || strLang == string.Empty)
				{
					_ci = CultureInfo.CurrentCulture;
				}
				else
				{
					_ci = new CultureInfo(strLang);
				}
			}

			int IComparer.Compare(object o1, object o2)
			{
				string strA = (string)o1;
				string strB = (string)o2;
				int num = string.Compare(strA, strB, true, _ci);
				if (num != 0 || _orderCase == XmlCaseOrder.None)
				{
					return num * _nMulSort;
				}
				return _nMulSort * _nMulCase * string.Compare(strA, strB, false, _ci);
			}
		}

		private readonly Expression _expr;

		private readonly IComparer _cmp;

		private readonly XmlDataType _type;

		public XPathSorter(object expr, IComparer cmp)
		{
			_expr = ExpressionFromObject(expr);
			_cmp = cmp;
			_type = XmlDataType.Text;
		}

		public XPathSorter(object expr, XmlSortOrder orderSort, XmlCaseOrder orderCase, string lang, XmlDataType dataType)
		{
			_expr = ExpressionFromObject(expr);
			_type = dataType;
			if (dataType == XmlDataType.Number)
			{
				_cmp = new XPathNumberComparer(orderSort);
			}
			else
			{
				_cmp = new XPathTextComparer(orderSort, orderCase, lang);
			}
		}

		private static Expression ExpressionFromObject(object expr)
		{
			if (expr is CompiledExpression)
			{
				return ((CompiledExpression)expr).ExpressionNode;
			}
			if (expr is string)
			{
				return new XPathParser().Compile((string)expr);
			}
			throw new XPathException("Invalid query object");
		}

		public object Evaluate(BaseIterator iter)
		{
			if (_type == XmlDataType.Number)
			{
				return _expr.EvaluateNumber(iter);
			}
			return _expr.EvaluateString(iter);
		}

		public int Compare(object o1, object o2)
		{
			return _cmp.Compare(o1, o2);
		}
	}
}
