namespace System.Text.RegularExpressions
{
	[Serializable]
	public class Match : Group
	{
		private Regex regex;

		private System.Text.RegularExpressions.IMachine machine;

		private int text_length;

		private GroupCollection groups;

		private static Match empty = new Match();

		public static Match Empty
		{
			get
			{
				return empty;
			}
		}

		public virtual GroupCollection Groups
		{
			get
			{
				return groups;
			}
		}

		internal Regex Regex
		{
			get
			{
				return regex;
			}
		}

		private Match()
		{
			regex = null;
			machine = null;
			text_length = 0;
			groups = new GroupCollection(1, 1);
			groups.SetValue(this, 0);
		}

		internal Match(Regex regex, System.Text.RegularExpressions.IMachine machine, string text, int text_length, int n_groups, int index, int length)
			: base(text, index, length)
		{
			this.regex = regex;
			this.machine = machine;
			this.text_length = text_length;
		}

		internal Match(Regex regex, System.Text.RegularExpressions.IMachine machine, string text, int text_length, int n_groups, int index, int length, int n_caps)
			: base(text, index, length, n_caps)
		{
			this.regex = regex;
			this.machine = machine;
			this.text_length = text_length;
			groups = new GroupCollection(n_groups, regex.Gap);
			groups.SetValue(this, 0);
		}

		[System.MonoTODO("not thread-safe")]
		public static Match Synchronized(Match inner)
		{
			if (inner == null)
			{
				throw new ArgumentNullException("inner");
			}
			return inner;
		}

		public Match NextMatch()
		{
			if (this == Empty)
			{
				return Empty;
			}
			int num = ((!regex.RightToLeft) ? (base.Index + base.Length) : base.Index);
			if (base.Length == 0)
			{
				num += ((!regex.RightToLeft) ? 1 : (-1));
			}
			return machine.Scan(regex, base.Text, num, text_length);
		}

		public virtual string Result(string replacement)
		{
			if (replacement == null)
			{
				throw new ArgumentNullException("replacement");
			}
			if (machine == null)
			{
				throw new NotSupportedException("Result cannot be called on failed Match.");
			}
			return machine.Result(replacement, this);
		}
	}
}
