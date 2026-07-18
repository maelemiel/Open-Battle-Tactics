using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class DateTimeConverter : TypeConverter
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
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string text = (string)value;
				try
				{
					if (text != null && text.Trim().Length == 0)
					{
						return DateTime.MinValue;
					}
					if (culture == null)
					{
						return DateTime.Parse(text);
					}
					DateTimeFormatInfo provider = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
					return DateTime.Parse(text, provider);
				}
				catch
				{
					throw new FormatException(text + " is not a valid DateTime value.");
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is DateTime)
			{
				DateTime dateTime = (DateTime)value;
				if (destinationType == typeof(string))
				{
					if (culture == null)
					{
						culture = CultureInfo.CurrentCulture;
					}
					if (dateTime == DateTime.MinValue)
					{
						return string.Empty;
					}
					DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
					if (culture == CultureInfo.InvariantCulture)
					{
						if (dateTime.Equals(dateTime.Date))
						{
							return dateTime.ToString("yyyy-MM-dd", culture);
						}
						return dateTime.ToString(culture);
					}
					if (dateTime == dateTime.Date)
					{
						return dateTime.ToString(dateTimeFormatInfo.ShortDatePattern, culture);
					}
					return dateTime.ToString(dateTimeFormatInfo.ShortDatePattern + " " + dateTimeFormatInfo.ShortTimePattern, culture);
				}
				if (destinationType == typeof(InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(DateTime).GetConstructor(new Type[1] { typeof(long) });
					return new InstanceDescriptor(constructor, new object[1] { dateTime.Ticks });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
