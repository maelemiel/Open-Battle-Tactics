using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class ArgumentOutOfRangeException : ArgumentException
	{
		private const int Result = -2146233086;

		private object actual_value;

		public virtual object ActualValue
		{
			get
			{
				return actual_value;
			}
		}

		public override string Message
		{
			get
			{
				string text = base.Message;
				if (actual_value == null)
				{
					return text;
				}
				return text + Environment.NewLine + actual_value;
			}
		}

		public ArgumentOutOfRangeException()
			: base(Locale.GetText("Argument is out of range."))
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName)
			: base(Locale.GetText("Argument is out of range."), paramName)
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName, string message)
			: base(message, paramName)
		{
			base.HResult = -2146233086;
		}

		public ArgumentOutOfRangeException(string paramName, object actualValue, string message)
			: base(message, paramName)
		{
			actual_value = actualValue;
			base.HResult = -2146233086;
		}

		protected ArgumentOutOfRangeException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			actual_value = info.GetString("ActualValue");
		}

		public ArgumentOutOfRangeException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.HResult = -2146233086;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ActualValue", actual_value);
		}
	}
}
