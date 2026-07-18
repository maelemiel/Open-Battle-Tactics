namespace NUnit.Framework.Constraints
{
	public abstract class StringConstraint : Constraint
	{
		protected string expected;

		protected bool caseInsensitive;

		public StringConstraint IgnoreCase
		{
			get
			{
				caseInsensitive = true;
				return this;
			}
		}

		public StringConstraint(string expected)
			: base(expected)
		{
			this.expected = expected;
		}
	}
}
