using System.Globalization;

namespace System.ComponentModel
{
	public class DoubleConverter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return false;
			}
		}

		public DoubleConverter()
		{
			InnerType = typeof(double);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((double)value).ToString("R", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return double.Parse(value, NumberStyles.Float, format);
		}
	}
}
