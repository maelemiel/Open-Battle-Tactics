using System;

namespace NUnit.Framework.Constraints
{
	public class Tolerance
	{
		private ToleranceMode mode;

		private object amount;

		private static readonly string ModeMustFollowTolerance = "Tolerance amount must be specified before setting mode";

		private static readonly string MultipleToleranceModes = "Tried to use multiple tolerance modes at the same time";

		private static readonly string NumericToleranceRequired = "A numeric tolerance is required";

		public static Tolerance Empty
		{
			get
			{
				return new Tolerance(0, ToleranceMode.None);
			}
		}

		public static Tolerance Zero
		{
			get
			{
				return new Tolerance(0, ToleranceMode.Linear);
			}
		}

		public ToleranceMode Mode
		{
			get
			{
				return mode;
			}
		}

		public object Value
		{
			get
			{
				return amount;
			}
		}

		public Tolerance Percent
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(amount, ToleranceMode.Percent);
			}
		}

		public Tolerance Ulps
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(amount, ToleranceMode.Ulps);
			}
		}

		public Tolerance Days
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromDays(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Hours
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromHours(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Minutes
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromMinutes(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Seconds
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromSeconds(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Milliseconds
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromMilliseconds(Convert.ToDouble(amount)));
			}
		}

		public Tolerance Ticks
		{
			get
			{
				CheckLinearAndNumeric();
				return new Tolerance(TimeSpan.FromTicks(Convert.ToInt64(amount)));
			}
		}

		public bool IsEmpty
		{
			get
			{
				return mode == ToleranceMode.None;
			}
		}

		public Tolerance(object amount)
			: this(amount, ToleranceMode.Linear)
		{
		}

		public Tolerance(object amount, ToleranceMode mode)
		{
			this.amount = amount;
			this.mode = mode;
		}

		private void CheckLinearAndNumeric()
		{
			if (mode != ToleranceMode.Linear)
			{
				throw new InvalidOperationException((mode == ToleranceMode.None) ? ModeMustFollowTolerance : MultipleToleranceModes);
			}
			if (!Numerics.IsNumericType(amount))
			{
				throw new InvalidOperationException(NumericToleranceRequired);
			}
		}
	}
}
