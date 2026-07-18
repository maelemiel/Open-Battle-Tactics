using System.Globalization;

namespace System.Data.Common
{
	internal sealed class DbConnectionStringBuilderHelper
	{
		public static int ConvertToInt32(object value)
		{
			return int.Parse(value.ToString(), CultureInfo.InvariantCulture);
		}

		public static bool ConvertToBoolean(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("null value cannot be converted to boolean");
			}
			switch (value.ToString().ToUpper().Trim())
			{
			case "YES":
			case "TRUE":
				return true;
			case "NO":
			case "FALSE":
				return false;
			default:
				throw new ArgumentException(string.Format("Invalid boolean value: {0}", value.ToString()));
			}
		}
	}
}
