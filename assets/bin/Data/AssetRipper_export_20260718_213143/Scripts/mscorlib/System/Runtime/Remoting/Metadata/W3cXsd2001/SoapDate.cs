using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapDate : ISoapXsd
	{
		private static readonly string[] _datetimeFormats = new string[6] { "yyyy-MM-dd", "'+'yyyy-MM-dd", "'-'yyyy-MM-dd", "yyyy-MM-ddzzz", "'+'yyyy-MM-ddzzz", "'-'yyyy-MM-ddzzz" };

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
				return "date";
			}
		}

		public SoapDate()
		{
		}

		public SoapDate(DateTime value)
		{
			_value = value;
		}

		public SoapDate(DateTime value, int sign)
		{
			_value = value;
			_sign = sign;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapDate Parse(string value)
		{
			DateTime value2 = DateTime.ParseExact(value, _datetimeFormats, null, DateTimeStyles.None);
			SoapDate soapDate = new SoapDate(value2);
			if (value.StartsWith("-"))
			{
				soapDate.Sign = -1;
			}
			else
			{
				soapDate.Sign = 0;
			}
			return soapDate;
		}

		public override string ToString()
		{
			if (_sign >= 0)
			{
				return _value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
			}
			return _value.ToString("'-'yyyy-MM-dd", CultureInfo.InvariantCulture);
		}
	}
}
