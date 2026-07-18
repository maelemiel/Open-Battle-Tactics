namespace System.Text.RegularExpressions.Syntax
{
	internal class RegularExpression : System.Text.RegularExpressions.Syntax.Group
	{
		private int group_count;

		public int GroupCount
		{
			get
			{
				return group_count;
			}
			set
			{
				group_count = value;
			}
		}

		public RegularExpression()
		{
			group_count = 0;
		}

		public override void Compile(System.Text.RegularExpressions.ICompiler cmp, bool reverse)
		{
			int min;
			int max;
			GetWidth(out min, out max);
			cmp.EmitInfo(group_count, min, max);
			System.Text.RegularExpressions.Syntax.AnchorInfo anchorInfo = GetAnchorInfo(reverse);
			System.Text.RegularExpressions.LinkRef linkRef = cmp.NewLink();
			cmp.EmitAnchor(reverse, anchorInfo.Offset, linkRef);
			if (anchorInfo.IsPosition)
			{
				cmp.EmitPosition(anchorInfo.Position);
			}
			else if (anchorInfo.IsSubstring)
			{
				cmp.EmitString(anchorInfo.Substring, anchorInfo.IgnoreCase, reverse);
			}
			cmp.EmitTrue();
			cmp.ResolveLink(linkRef);
			base.Compile(cmp, reverse);
			cmp.EmitTrue();
		}
	}
}
