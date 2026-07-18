namespace NUnit.Framework.Constraints
{
	public class AndConstraint : BinaryConstraint
	{
		private enum FailurePoint
		{
			None = 0,
			Left = 1,
			Right = 2
		}

		private FailurePoint failurePoint;

		public AndConstraint(Constraint left, Constraint right)
			: base(left, right)
		{
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			failurePoint = ((!left.Matches(actual)) ? FailurePoint.Left : ((!right.Matches(actual)) ? FailurePoint.Right : FailurePoint.None));
			return failurePoint == FailurePoint.None;
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			left.WriteDescriptionTo(writer);
			writer.WriteConnector("and");
			right.WriteDescriptionTo(writer);
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			switch (failurePoint)
			{
			case FailurePoint.Left:
				left.WriteActualValueTo(writer);
				break;
			case FailurePoint.Right:
				right.WriteActualValueTo(writer);
				break;
			default:
				base.WriteActualValueTo(writer);
				break;
			}
		}
	}
}
