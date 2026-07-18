using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class DecimalConverter : BaseNumberConverter
	{
		internal override bool SupportHex
		{
			get
			{
				return false;
			}
		}

		public DecimalConverter()
		{
			InnerType = typeof(decimal);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is decimal)
			{
				decimal d = (decimal)value;
				ConstructorInfo constructor = typeof(decimal).GetConstructor(new Type[1] { typeof(int[]) });
				return new InstanceDescriptor(constructor, new object[1] { decimal.GetBits(d) });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((decimal)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return decimal.Parse(value, NumberStyles.Float, format);
		}
	}
}
