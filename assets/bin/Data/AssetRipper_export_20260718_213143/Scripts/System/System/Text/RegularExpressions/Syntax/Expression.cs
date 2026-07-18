namespace System.Text.RegularExpressions.Syntax
{
	internal abstract class Expression
	{
		public abstract void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse);

		public abstract void GetWidth(out int min, out int max);

		public int GetFixedWidth()
		{
			int min;
			int max;
			GetWidth(out min, out max);
			if (min == max)
			{
				return min;
			}
			return -1;
		}

		public virtual System.Text.RegularExpressions.Syntax.AnchorInfo GetAnchorInfo(bool reverse)
		{
			return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, GetFixedWidth());
		}

		public abstract bool IsComplex();
	}
}
