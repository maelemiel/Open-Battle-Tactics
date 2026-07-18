namespace NUnit.Framework.Constraints
{
	public abstract class BinaryConstraint : Constraint
	{
		protected Constraint left;

		protected Constraint right;

		public BinaryConstraint(Constraint left, Constraint right)
			: base(left, right)
		{
			this.left = left;
			this.right = right;
		}
	}
}
