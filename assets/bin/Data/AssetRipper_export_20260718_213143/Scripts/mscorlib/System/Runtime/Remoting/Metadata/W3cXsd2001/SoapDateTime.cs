using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[ComVisible(true)]
	public sealed class SoapDateTime
	{
		private static readonly string[] _datetimeFormats = new string[24]
		{
			"yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.f", "yyyy-MM-ddTHH:mm:ss.ff", "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss.ffff", "yyyy-MM-ddTHH:mm:ss.fffff", "yyyy-MM-ddTHH:mm:ss.ffffff", "yyyy-MM-ddTHH:mm:ss.fffffff", "yyyy-MM-ddTHH:mm:sszzz", "yyyy-MM-ddTHH:mm:ss.fzzz",
			"yyyy-MM-ddTHH:mm:ss.ffzzz", "yyyy-MM-ddTHH:mm:ss.fffzzz", "yyyy-MM-ddTHH:mm:ss.ffffzzz", "yyyy-MM-ddTHH:mm:ss.fffffzzz", "yyyy-MM-ddTHH:mm:ss.ffffffzzz", "yyyy-MM-ddTHH:mm:ss.fffffffzzz", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:ss.fZ", "yyyy-MM-ddTHH:mm:ss.ffZ", "yyyy-MM-ddTHH:mm:ss.fffZ",
			"yyyy-MM-ddTHH:mm:ss.ffffZ", "yyyy-MM-ddTHH:mm:ss.fffffZ", "yyyy-MM-ddTHH:mm:ss.ffffffZ", "yyyy-MM-ddTHH:mm:ss.fffffffZ"
		};

		public static string XsdType
		{
			get
			{
				return "dateTime";
			}
		}

		public static DateTime Parse(string value)
		{
			return DateTime.ParseExact(value, _datetimeFormats, null, DateTimeStyles.None);
		}

		public static string ToString(DateTime value)
		{
			return value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
		}
	}
}
