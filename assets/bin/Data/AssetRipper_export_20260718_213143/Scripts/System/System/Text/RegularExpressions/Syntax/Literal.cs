namespace System.Text.RegularExpressions.Syntax
{
	internal class Literal : System.Text.RegularExpressions.Syntax.Expression
	{
		private string str;

		private bool ignore;

		public string String
		{
			get
			{
				return str;
			}
			set
			{
				str = value;
			}
		}

		public bool IgnoreCase
		{
			get
			{
				return ignore;
			}
			set
			{
				ignore = value;
			}
		}

		public Literal(string str, bool ignore)
		{
			this.str = str;
			this.ignore = ignore;
		}

		public static void CompileLiteral(string str, System.Text.RegularExpressions.ICompiler cmp, bool ignore, bool reverse)
		{
			if (str.Length != 0)
			{
				if (str.Length == 1)
				{
					cmp.EmitCharacter(str[0], false, ignore, reverse);
				}
				else
				{
					cmp.EmitString(str, ignore, reverse);
				}
			}
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			CompileLiteral(str, cmp, ignore, reverse);
		}

		public override void GetWidth(out int min, out int max)
		{
			min = (max = str.Length);
		}

		public override System.Text.RegularExpressions.Syntax.AnchorInfo GetAnchorInfo(bool reverse)
		{
			return new System.Text.RegularExpressions.Syntax.AnchorInfo(this, 0, str.Length, str, ignore);
		}

		public override bool IsComplex()
		{
			return false;
		}
	}
}
