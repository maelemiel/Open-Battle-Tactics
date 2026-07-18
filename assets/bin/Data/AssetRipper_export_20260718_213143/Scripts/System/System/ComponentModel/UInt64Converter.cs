using System.Globalization;

namespace System.ComponentModel
{
	public class UInt64Converter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return true;
			}
		}

		public UInt64Converter()
		{
			InnerType = typeof(ulong);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((ulong)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return ulong.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToUInt64(value, fromBase);
		}
	}
}
