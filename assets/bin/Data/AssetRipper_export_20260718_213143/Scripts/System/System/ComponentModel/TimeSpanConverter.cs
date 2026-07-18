using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class TimeSpanConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string text = (string)value;
				try
				{
					return TimeSpan.Parse(text);
				}
				catch
				{
					throw new FormatException(text + "is not valid for a TimeSpan.");
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is TimeSpan)
			{
				TimeSpan timeSpan = (TimeSpan)value;
				if (destinationType == typeof(string) && value != null)
				{
					return timeSpan.ToString();
				}
				if (destinationType == typeof(InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(TimeSpan).GetConstructor(new Type[1] { typeof(long) });
					return new InstanceDescriptor(constructor, new object[1] { timeSpan.Ticks });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
