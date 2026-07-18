using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class TimeZoneNotFoundException : Exception
	{
		public TimeZoneNotFoundException()
		{
		}

		public TimeZoneNotFoundException(string message)
			: base(message)
		{
		}

		public TimeZoneNotFoundException(string message, Exception e)
			: base(message, e)
		{
		}

		protected TimeZoneNotFoundException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
		}
	}
}
