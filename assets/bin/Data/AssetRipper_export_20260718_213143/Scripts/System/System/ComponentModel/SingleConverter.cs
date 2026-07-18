using System.Globalization;

namespace System.ComponentModel
{
	public class SingleConverter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return false;
			}
		}

		public SingleConverter()
		{
			InnerType = typeof(float);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((float)value).ToString("R", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return float.Parse(value, NumberStyles.Float, format);
		}
	}
}
