using System;

namespace NUnit.Framework.Constraints
{
	public class PredicateConstraint<T> : Constraint
	{
		private Predicate<T> predicate;

		public PredicateConstraint(Predicate<T> predicate)
		{
			this.predicate = predicate;
		}

		public override bool Matches(object actual)
		{
			base.actual = actual;
			if (!(actual is T))
			{
				throw new ArgumentException("The actual value is not of type " + typeof(T).Name, "actual");
			}
			return predicate((T)actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("value matching");
			writer.Write(predicate.Method.Name.StartsWith("<") ? "lambda expression" : predicate.Method.Name);
		}
	}
}
