using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public class MissingFieldException : MissingMemberException
	{
		private const int Result = -2146233071;

		public override string Message
		{
			get
			{
				if (ClassName == null)
				{
					return base.Message;
				}
				string text = Locale.GetText("Field '{0}.{1}' not found.");
				return string.Format(text, ClassName, MemberName);
			}
		}

		public MissingFieldException()
			: base(Locale.GetText("Cannot find requested field."))
		{
			base.HResult = -2146233071;
		}

		public MissingFieldException(string message)
			: base(message)
		{
			base.HResult = -2146233071;
		}

		protected MissingFieldException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public MissingFieldException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233071;
		}

		public MissingFieldException(string className, string fieldName)
			: base(className, fieldName)
		{
			base.HResult = -2146233071;
		}
	}
}
