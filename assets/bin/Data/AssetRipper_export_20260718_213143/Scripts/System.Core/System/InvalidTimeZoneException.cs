using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class InvalidTimeZoneException : Exception
	{
		public InvalidTimeZoneException()
		{
		}

		public InvalidTimeZoneException(string message)
			: base(message)
		{
		}

		public InvalidTimeZoneException(string message, Exception e)
			: base(message, e)
		{
		}

		protected InvalidTimeZoneException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
		}
	}
}
