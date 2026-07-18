using System.Collections;

namespace System.Xml.XPath
{
	internal abstract class RelationalExpr : ExprBoolean
	{
		public override bool StaticValueAsBoolean
		{
			get
			{
				return HasStaticValue && Compare(_left.StaticValueAsNumber, _right.StaticValueAsNumber);
			}
		}

		public RelationalExpr(Expression left, Expression right)
			: base(left, right)
		{
		}

		public override bool EvaluateBoolean(BaseIterator iter)
		{
			XPathResultType xPathResultType = _left.GetReturnType(iter);
			XPathResultType xPathResultType2 = _right.GetReturnType(iter);
			if (xPathResultType == XPathResultType.Any)
			{
				xPathResultType = Expression.GetReturnType(_left.Evaluate(iter));
			}
			if (xPathResultType2 == XPathResultType.Any)
			{
				xPathResultType2 = Expression.GetReturnType(_right.Evaluate(iter));
			}
			if (xPathResultType == XPathResultType.Navigator)
			{
				xPathResultType = XPathResultType.String;
			}
			if (xPathResultType2 == XPathResultType.Navigator)
			{
				xPathResultType2 = XPathResultType.String;
			}
			if (xPathResultType == XPathResultType.NodeSet || xPathResultType2 == XPathResultType.NodeSet)
			{
				bool fReverse = false;
				Expression expression;
				Expression expression2;
				if (xPathResultType != XPathResultType.NodeSet)
				{
					fReverse = true;
					expression = _right;
					expression2 = _left;
					XPathResultType xPathResultType3 = xPathResultType;
					xPathResultType = xPathResultType2;
					xPathResultType2 = xPathResultType3;
				}
				else
				{
					expression = _left;
					expression2 = _right;
				}
				if (xPathResultType2 == XPathResultType.Boolean)
				{
					bool value = expression.EvaluateBoolean(iter);
					bool value2 = expression2.EvaluateBoolean(iter);
					return Compare(Convert.ToDouble(value), Convert.ToDouble(value2), fReverse);
				}
				BaseIterator baseIterator = expression.EvaluateNodeSet(iter);
				switch (xPathResultType2)
				{
				case XPathResultType.Number:
				case XPathResultType.String:
				{
					double arg2 = expression2.EvaluateNumber(iter);
					while (baseIterator.MoveNext())
					{
						if (Compare(XPathFunctions.ToNumber(baseIterator.Current.Value), arg2, fReverse))
						{
							return true;
						}
					}
					break;
				}
				case XPathResultType.NodeSet:
				{
					BaseIterator baseIterator2 = expression2.EvaluateNodeSet(iter);
					ArrayList arrayList = new ArrayList();
					while (baseIterator.MoveNext())
					{
						arrayList.Add(XPathFunctions.ToNumber(baseIterator.Current.Value));
					}
					while (baseIterator2.MoveNext())
					{
						double arg = XPathFunctions.ToNumber(baseIterator2.Current.Value);
						for (int i = 0; i < arrayList.Count; i++)
						{
							if (Compare((double)arrayList[i], arg))
							{
								return true;
							}
						}
					}
					break;
				}
				}
				return false;
			}
			return Compare(_left.EvaluateNumber(iter), _right.EvaluateNumber(iter));
		}

		public abstract bool Compare(double arg1, double arg2);

		public bool Compare(double arg1, double arg2, bool fReverse)
		{
			if (fReverse)
			{
				return Compare(arg2, arg1);
			}
			return Compare(arg1, arg2);
		}
	}
}
