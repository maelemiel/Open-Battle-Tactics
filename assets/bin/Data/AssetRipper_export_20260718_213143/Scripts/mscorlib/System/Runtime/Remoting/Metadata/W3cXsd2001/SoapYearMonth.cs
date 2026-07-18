using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapYearMonth : ISoapXsd
	{
		private static readonly string[] _datetimeFormats = new string[6] { "yyyy-MM", "'+'yyyy-MM", "'-'yyyy-MM", "yyyy-MMzzz", "'+'yyyy-MMzzz", "'-'yyyy-MMzzz" };

		private int _sign;

		private DateTime _value;

		public int Sign
		{
			get
			{
				return _sign;
			}
			set
			{
				_sign = value;
			}
		}

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
				return "gYearMonth";
			}
		}

		public SoapYearMonth()
		{
		}

		public SoapYearMonth(DateTime value)
		{
			_value = value;
		}

		public SoapYearMonth(DateTime value, int sign)
		{
			_value = value;
			_sign = sign;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapYearMonth Parse(string value)
		{
			DateTime value2 = DateTime.ParseExact(value, _datetimeFormats, null, DateTimeStyles.None);
			SoapYearMonth soapYearMonth = new SoapYearMonth(value2);
			if (value.StartsWith("-"))
			{
				soapYearMonth.Sign = -1;
			}
			else
			{
				soapYearMonth.Sign = 0;
			}
			return soapYearMonth;
		}

		public override string ToString()
		{
			if (_sign >= 0)
			{
				return _value.ToString("yyyy-MM", CultureInfo.InvariantCulture);
			}
			return _value.ToString("'-'yyyy-MM", CultureInfo.InvariantCulture);
		}
	}
}
