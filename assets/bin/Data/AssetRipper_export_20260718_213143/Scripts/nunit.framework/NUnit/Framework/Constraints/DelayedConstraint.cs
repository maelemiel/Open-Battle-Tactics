using System;
using System.Threading;

namespace NUnit.Framework.Constraints
{
	public class DelayedConstraint : PrefixConstraint
	{
		private readonly int delayInMilliseconds;

		private readonly int pollingInterval;

		public DelayedConstraint(Constraint baseConstraint, int delayInMilliseconds)
			: this(baseConstraint, delayInMilliseconds, 0)
		{
		}

		public DelayedConstraint(Constraint baseConstraint, int delayInMilliseconds, int pollingInterval)
			: base(baseConstraint)
		{
			if (delayInMilliseconds < 0)
			{
				throw new ArgumentException("Cannot check a condition in the past", "delayInMilliseconds");
			}
			this.delayInMilliseconds = delayInMilliseconds;
			this.pollingInterval = pollingInterval;
		}

		public override bool Matches(object actual)
		{
			int num = delayInMilliseconds;
			while (pollingInterval > 0 && pollingInterval < num)
			{
				num -= pollingInterval;
				Thread.Sleep(pollingInterval);
				base.actual = actual;
				if (baseConstraint.Matches(actual))
				{
					return true;
				}
			}
			if (num > 0)
			{
				Thread.Sleep(num);
			}
			base.actual = actual;
			return baseConstraint.Matches(actual);
		}

		public override bool Matches(ActualValueDelegate del)
		{
			int num = delayInMilliseconds;
			while (pollingInterval > 0 && pollingInterval < num)
			{
				num -= pollingInterval;
				Thread.Sleep(pollingInterval);
				actual = del();
				try
				{
					if (baseConstraint.Matches(actual))
					{
						return true;
					}
				}
				catch (Exception)
				{
				}
			}
			if (num > 0)
			{
				Thread.Sleep(num);
			}
			actual = del();
			return baseConstraint.Matches(actual);
		}

		public override bool Matches<T>(ref T actual)
		{
			int num = delayInMilliseconds;
			while (pollingInterval > 0 && pollingInterval < num)
			{
				num -= pollingInterval;
				Thread.Sleep(pollingInterval);
				base.actual = actual;
				try
				{
					if (baseConstraint.Matches(actual))
					{
						return true;
					}
				}
				catch (Exception)
				{
				}
			}
			if (num > 0)
			{
				Thread.Sleep(num);
			}
			base.actual = actual;
			return baseConstraint.Matches(actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			baseConstraint.WriteDescriptionTo(writer);
			writer.Write(string.Format(" after {0} millisecond delay", delayInMilliseconds));
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			baseConstraint.WriteActualValueTo(writer);
		}

		protected override string GetStringRepresentation()
		{
			return string.Format("<after {0} {1}>", delayInMilliseconds, baseConstraint);
		}
	}
}
