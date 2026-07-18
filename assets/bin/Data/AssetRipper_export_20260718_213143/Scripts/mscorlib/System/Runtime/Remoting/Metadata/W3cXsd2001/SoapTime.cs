using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapTime : ISoapXsd
	{
		private static readonly string[] _datetimeFormats = new string[24]
		{
			"HH:mm:ss", "HH:mm:ss.f", "HH:mm:ss.ff", "HH:mm:ss.fff", "HH:mm:ss.ffff", "HH:mm:ss.fffff", "HH:mm:ss.ffffff", "HH:mm:ss.fffffff", "HH:mm:sszzz", "HH:mm:ss.fzzz",
			"HH:mm:ss.ffzzz", "HH:mm:ss.fffzzz", "HH:mm:ss.ffffzzz", "HH:mm:ss.fffffzzz", "HH:mm:ss.ffffffzzz", "HH:mm:ss.fffffffzzz", "HH:mm:ssZ", "HH:mm:ss.fZ", "HH:mm:ss.ffZ", "HH:mm:ss.fffZ",
			"HH:mm:ss.ffffZ", "HH:mm:ss.fffffZ", "HH:mm:ss.ffffffZ", "HH:mm:ss.fffffffZ"
		};

		private DateTime _value;

		public DateTime Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public static string XsdType
		{
			get
			{
				return "time";
			}
		}

		public SoapTime()
		{
		}

		public SoapTime(DateTime value)
		{
			_value = value;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapTime Parse(string value)
		{
			return new SoapTime(DateTime.ParseExact(value, _datetimeFormats, null, DateTimeStyles.None));
		}

		public override string ToString()
		{
			return _value.ToString("HH:mm:ss.fffffffzzz", CultureInfo.InvariantCulture);
		}
	}
}
