using System.Globalization;

namespace System.ComponentModel
{
	public abstract class BaseNumberConverter : TypeConverter
	{
		internal Type InnerType;

		internal abstract bool SupportHex { get; }

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type t)
		{
			return t.IsPrimitive || base.CanConvertTo(context, t);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			string text = value as string;
			if (text != null)
			{
				try
				{
					if (SupportHex)
					{
						if (text.Length >= 1 && text[0] == '#')
						{
							return ConvertFromString(text.Substring(1), 16);
						}
						if (text.StartsWith("0x") || text.StartsWith("0X"))
						{
							return ConvertFromString(text, 16);
						}
					}
					NumberFormatInfo format = (NumberFormatInfo)culture.GetFormat(typeof(NumberFormatInfo));
					return ConvertFromString(text, format);
				}
				catch (Exception innerException)
				{
					throw new Exception(value.ToString() + " is not a valid value for " + InnerType.Name + ".", innerException);
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			if (destinationType == typeof(string) && value is IConvertible)
			{
				return ((IConvertible)value).ToType(destinationType, culture);
			}
			if (destinationType.IsPrimitive)
			{
				return Convert.ChangeType(value, destinationType, culture);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		internal abstract string ConvertToString(object value, NumberFormatInfo format);

		internal abstract object ConvertFromString(string value, NumberFormatInfo format);

		internal virtual object ConvertFromString(string value, int fromBase)
		{
			if (SupportHex)
			{
				throw new NotImplementedException();
			}
			throw new InvalidOperationException();
		}
	}
}
