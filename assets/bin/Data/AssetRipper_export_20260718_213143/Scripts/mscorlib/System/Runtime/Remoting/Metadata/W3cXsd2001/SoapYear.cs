using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SoapYear : ISoapXsd
	{
		private static readonly string[] _datetimeFormats = new string[6] { "yyyy", "'+'yyyy", "'-'yyyy", "yyyyzzz", "'+'yyyyzzz", "'-'yyyyzzz" };

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
				return "gYear";
			}
		}

		public SoapYear()
		{
		}

		public SoapYear(DateTime value)
		{
			_value = value;
		}

		public SoapYear(DateTime value, int sign)
		{
			_value = value;
			_sign = sign;
		}

		public string GetXsdType()
		{
			return XsdType;
		}

		public static SoapYear Parse(string value)
		{
			DateTime value2 = DateTime.ParseExact(value, _datetimeFormats, null, DateTimeStyles.None);
			SoapYear soapYear = new SoapYear(value2);
			if (value.StartsWith("-"))
			{
				soapYear.Sign = -1;
			}
			else
			{
				soapYear.Sign = 0;
			}
			return soapYear;
		}

		public override string ToString()
		{
			if (_sign >= 0)
			{
				return _value.ToString("yyyy", CultureInfo.InvariantCulture);
			}
			return _value.ToString("'-'yyyy", CultureInfo.InvariantCulture);
		}
	}
}
