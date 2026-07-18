using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public class InvalidFilterCriteriaException : ApplicationException
	{
		public InvalidFilterCriteriaException()
			: base(Locale.GetText("Filter Criteria is not valid."))
		{
		}

		public InvalidFilterCriteriaException(string message)
			: base(message)
		{
		}

		public InvalidFilterCriteriaException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected InvalidFilterCriteriaException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
