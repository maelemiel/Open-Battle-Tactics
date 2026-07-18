using System.Globalization;

namespace System.ComponentModel
{
	public class Int32Converter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return true;
			}
		}

		public Int32Converter()
		{
			InnerType = typeof(int);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((int)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return int.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToInt32(value, fromBase);
		}
	}
}
