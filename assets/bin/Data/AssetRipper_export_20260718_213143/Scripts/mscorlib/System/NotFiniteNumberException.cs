using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class NotFiniteNumberException : ArithmeticException
	{
		private const int Result = -2146233048;

		private double offending_number;

		public double OffendingNumber
		{
			get
			{
				return offending_number;
			}
		}

		public NotFiniteNumberException()
			: base(Locale.GetText("The number encountered was not a finite quantity."))
		{
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(double offendingNumber)
		{
			offending_number = offendingNumber;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message)
			: base(message)
		{
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message, double offendingNumber)
			: base(message)
		{
			offending_number = offendingNumber;
			base.HResult = -2146233048;
		}

		public NotFiniteNumberException(string message, double offendingNumber, Exception innerException)
			: base(message, innerException)
		{
			offending_number = offendingNumber;
			base.HResult = -2146233048;
		}

		protected NotFiniteNumberException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			offending_number = info.GetDouble("OffendingNumber");
		}

		public NotFiniteNumberException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233048;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("OffendingNumber", offending_number);
		}
	}
}
