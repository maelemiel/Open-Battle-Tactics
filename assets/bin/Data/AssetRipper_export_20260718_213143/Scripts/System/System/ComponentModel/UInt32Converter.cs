using System.Globalization;

namespace System.ComponentModel
{
	public class UInt32Converter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return true;
			}
		}

		public UInt32Converter()
		{
			InnerType = typeof(uint);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((uint)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return uint.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToUInt32(value, fromBase);
		}
	}
}
