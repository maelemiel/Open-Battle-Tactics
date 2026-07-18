using System.Globalization;

namespace System.ComponentModel
{
	public class Int64Converter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return true;
			}
		}

		public Int64Converter()
		{
			InnerType = typeof(long);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((long)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return long.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToInt64(value, fromBase);
		}
	}
}
