namespace System.Text.RegularExpressions.Syntax
{
	internal class CapturingGroup : System.Text.RegularExpressions.Syntax.Group, IComparable
	{
		private int gid;

		private string name;

		public int Index
		{
			get
			{
				return gid;
			}
			set
			{
				gid = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public bool IsNamed
		{
			get
			{
				return name != null;
			}
		}

		public CapturingGroup()
		{
			gid = 0;
			name = null;
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			cmp.EmitOpen(gid);
			base.Compile(cmp, reverse);
			cmp.EmitClose(gid);
		}

		public override bool IsComplex()
		{
			return true;
		}

		public int CompareTo(object other)
		{
			return gid - ((System.Text.RegularExpressions.Syntax.CapturingGroup)other).gid;
		}
	}
}
