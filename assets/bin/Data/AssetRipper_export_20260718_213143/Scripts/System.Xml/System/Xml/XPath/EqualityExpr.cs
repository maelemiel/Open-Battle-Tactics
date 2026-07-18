using System.Collections;

namespace System.Xml.XPath
{
	internal abstract class EqualityExpr : ExprBoolean
	{
		private bool trueVal;

		public override bool StaticValueAsBoolean
		{
			get
			{
				if (!HasStaticValue)
				{
					return false;
				}
				if ((_left.ReturnType == XPathResultType.Navigator || _right.ReturnType == XPathResultType.Navigator) && _left.ReturnType == _right.ReturnType)
				{
					return _left.StaticValueAsNavigator.IsSamePosition(_right.StaticValueAsNavigator) == trueVal;
				}
				if ((_left.ReturnType == XPathResultType.Boolean) | (_right.ReturnType == XPathResultType.Boolean))
				{
					return _left.StaticValueAsBoolean == _right.StaticValueAsBoolean == trueVal;
				}
				if ((_left.ReturnType == XPathResultType.Number) | (_right.ReturnType == XPathResultType.Number))
				{
					return _left.StaticValueAsNumber == _right.StaticValueAsNumber == trueVal;
				}
				if ((_left.ReturnType == XPathResultType.String) | (_right.ReturnType == XPathResultType.String))
				{
					return _left.StaticValueAsString == _right.StaticValueAsString == trueVal;
				}
				return _left.StaticValue == _right.StaticValue == trueVal;
			}
		}

		public EqualityExpr(Expression left, Expression right, bool trueVal)
			: base(left, right)
		{
			this.trueVal = trueVal;
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
				Expression expression;
				Expression expression2;
				if (xPathResultType != XPathResultType.NodeSet)
				{
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
					return expression.EvaluateBoolean(iter) == expression2.EvaluateBoolean(iter) == trueVal;
				}
				BaseIterator baseIterator = expression.EvaluateNodeSet(iter);
				switch (xPathResultType2)
				{
				case XPathResultType.Number:
				{
					double num = expression2.EvaluateNumber(iter);
					while (baseIterator.MoveNext())
					{
						if (XPathFunctions.ToNumber(baseIterator.Current.Value) == num == trueVal)
						{
							return true;
						}
					}
					break;
				}
				case XPathResultType.String:
				{
					string text2 = expression2.EvaluateString(iter);
					while (baseIterator.MoveNext())
					{
						if (baseIterator.Current.Value == text2 == trueVal)
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
						arrayList.Add(XPathFunctions.ToString(baseIterator.Current.Value));
					}
					while (baseIterator2.MoveNext())
					{
						string text = XPathFunctions.ToString(baseIterator2.Current.Value);
						for (int i = 0; i < arrayList.Count; i++)
						{
							if (text == (string)arrayList[i] == trueVal)
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
			if (xPathResultType == XPathResultType.Boolean || xPathResultType2 == XPathResultType.Boolean)
			{
				return _left.EvaluateBoolean(iter) == _right.EvaluateBoolean(iter) == trueVal;
			}
			if (xPathResultType == XPathResultType.Number || xPathResultType2 == XPathResultType.Number)
			{
				return _left.EvaluateNumber(iter) == _right.EvaluateNumber(iter) == trueVal;
			}
			return _left.EvaluateString(iter) == _right.EvaluateString(iter) == trueVal;
		}
	}
}
