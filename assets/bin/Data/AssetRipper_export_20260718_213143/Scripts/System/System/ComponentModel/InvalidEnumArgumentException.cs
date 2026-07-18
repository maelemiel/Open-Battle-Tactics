using System.Globalization;
using System.Runtime.Serialization;

namespace System.ComponentModel
{
	[Serializable]
	public class InvalidEnumArgumentException : ArgumentException
	{
		public InvalidEnumArgumentException()
			: this(null)
		{
		}

		public InvalidEnumArgumentException(string message)
			: base(message)
		{
		}

		public InvalidEnumArgumentException(string argumentName, int invalidValue, Type enumClass)
			: base(string.Format(CultureInfo.CurrentCulture, "The value of argument '{0}' ({1}) is invalid for Enum type '{2}'.", argumentName, invalidValue, enumClass.Name), argumentName)
		{
		}

		public InvalidEnumArgumentException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected InvalidEnumArgumentException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
