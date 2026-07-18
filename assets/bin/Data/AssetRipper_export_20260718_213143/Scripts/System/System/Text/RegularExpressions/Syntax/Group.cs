using System.Collections;

namespace System.Text.RegularExpressions.Syntax
{
	internal class Group : System.Text.RegularExpressions.Syntax.CompositeExpression
	{
		public System.Text.RegularExpressions.Syntax.Expression Expression
		{
			get
			{
				return base.Expressions[0];
			}
			set
			{
				base.Expressions[0] = value;
			}
		}

		public void AppendExpression(System.Text.RegularExpressions.Syntax.Expression e)
		{
			base.Expressions.Add(e);
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			int count = base.Expressions.Count;
			for (int i = 0; i < count; i++)
			{
				System.Text.RegularExpressions.Syntax.Expression expression = ((!reverse) ? base.Expressions[i] : base.Expressions[count - i - 1]);
				expression.Compile(cmp, reverse);
			}
		}

		public override void GetWidth(out int min, out int max)
		{
			min = 0;
			max = 0;
			foreach (System.Text.RegularExpressions.Syntax.Expression expression in base.Expressions)
			{
				int min2;
				int max2;
				expression.GetWidth(out min2, out max2);
				min += min2;
				if (max == int.MaxValue || max2 == int.MaxValue)
				{
					max = int.MaxValue;
				}
				else
				{
					max += max2;
				}
			}
		}

		public override System.Text.RegularExpressions.Syntax.AnchorInfo GetAnchorInfo(bool reverse)
		{
			int fixedWidth = GetFixedWidth();
			ArrayList arrayList = new ArrayList();
			System.Text.RegularExpressions.IntervalCollection intervalCollection = new System.Text.RegularExpressions.IntervalCollection();
			int num = 0;
			int count = base.Expressions.Count;
			for (int i = 0; i < count; i++)
			{
				System.Text.RegularExpressions.Syntax.Expression expression = ((!reverse) ? base.Expressions[i] : base.Expressions[count - i - 1]);
				System.Text.RegularExpressions.Syntax.AnchorInfo anchorInfo = expression.GetAnchorInfo(reverse);
				arrayList.Add(anchorInfo);
				if (anchorInfo.IsPosition)
				{
					return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, num + anchorInfo.Offset, fixedWidth, anchorInfo.Position);
				}
				if (anchorInfo.IsSubstring)
				{
					intervalCollection.Add(anchorInfo.GetInterval(num));
				}
				if (anchorInfo.IsUnknownWidth)
				{
					break;
				}
				num += anchorInfo.Width;
			}
			intervalCollection.Normalize();
			System.Text.RegularExpressions.Interval interval = System.Text.RegularExpressions.Interval.Empty;
			foreach (System.Text.RegularExpressions.Interval item in intervalCollection)
			{
				if (item.Size > interval.Size)
				{
					interval = item;
				}
			}
			if (interval.IsEmpty)
			{
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, fixedWidth);
			}
			bool flag = false;
			int num2 = 0;
			num = 0;
			for (int j = 0; j < arrayList.Count; j++)
			{
				System.Text.RegularExpressions.Syntax.AnchorInfo anchorInfo2 = (System.Text.RegularExpressions.Syntax.AnchorInfo)arrayList[j];
				if (anchorInfo2.IsSubstring && interval.Contains(anchorInfo2.GetInterval(num)))
				{
					flag |= anchorInfo2.IgnoreCase;
					arrayList[num2++] = anchorInfo2;
				}
				if (anchorInfo2.IsUnknownWidth)
				{
					break;
				}
				num += anchorInfo2.Width;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int k = 0; k < num2; k++)
			{
				System.Text.RegularExpressions.Syntax.AnchorInfo anchorInfo3 = ((!reverse) ? ((System.Text.RegularExpressions.Syntax.AnchorInfo)arrayList[k]) : ((System.Text.RegularExpressions.Syntax.AnchorInfo)arrayList[num2 - k - 1]));
				stringBuilder.Append(anchorInfo3.Substring);
			}
			if (stringBuilder.Length == interval.Size)
			{
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, interval.low, fixedWidth, stringBuilder.ToString(), flag);
			}
			if (stringBuilder.Length > interval.Size)
			{
				Console.Error.WriteLine("overlapping?");
				return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, fixedWidth);
			}
			throw new SystemException("Shouldn't happen");
		}
	}
}
