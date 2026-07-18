using System.Globalization;

namespace System.ComponentModel
{
	public class Int16Converter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return true;
			}
		}

		public Int16Converter()
		{
			InnerType = typeof(short);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((short)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return short.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToInt16(value, fromBase);
		}
	}
}
