using System.Collections;
using System.Globalization;

namespace System.ComponentModel
{
	public class NullableConverter : TypeConverter
	{
		private Type nullableType;

		private Type underlyingType;

		private TypeConverter underlyingTypeConverter;

		public Type NullableType
		{
			get
			{
				return nullableType;
			}
		}

		public Type UnderlyingType
		{
			get
			{
				return underlyingType;
			}
		}

		public TypeConverter UnderlyingTypeConverter
		{
			get
			{
				return underlyingTypeConverter;
			}
		}

		public NullableConverter(Type nullableType)
		{
			if (nullableType == null)
			{
				throw new ArgumentNullException("nullableType");
			}
			this.nullableType = nullableType;
			underlyingType = Nullable.GetUnderlyingType(nullableType);
			underlyingTypeConverter = TypeDescriptor.GetConverter(underlyingType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == underlyingType)
			{
				return true;
			}
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.CanConvertFrom(context, sourceType);
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == underlyingType)
			{
				return true;
			}
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.CanConvertTo(context, destinationType);
			}
			return base.CanConvertFrom(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null || value.GetType() == underlyingType)
			{
				return value;
			}
			if (value is string && string.IsNullOrEmpty((string)value))
			{
				return null;
			}
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.ConvertFrom(context, culture, value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == underlyingType && value.GetType() == nullableType)
			{
				return value;
			}
			if (underlyingTypeConverter != null && value != null)
			{
				return underlyingTypeConverter.ConvertTo(context, culture, value, destinationType);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.CreateInstance(context, propertyValues);
			}
			return base.CreateInstance(context, propertyValues);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.GetCreateInstanceSupported(context);
			}
			return base.GetCreateInstanceSupported(context);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.GetProperties(context, value, attributes);
			}
			return base.GetProperties(context, value, attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.GetCreateInstanceSupported(context);
			}
			return base.GetCreateInstanceSupported(context);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (underlyingTypeConverter != null && underlyingTypeConverter.GetStandardValuesSupported(context))
			{
				StandardValuesCollection standardValues = underlyingTypeConverter.GetStandardValues(context);
				if (standardValues != null)
				{
					ArrayList arrayList = new ArrayList(standardValues);
					arrayList.Add(null);
					return new StandardValuesCollection(arrayList);
				}
			}
			return base.GetStandardValues(context);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.GetStandardValuesExclusive(context);
			}
			return base.GetStandardValuesExclusive(context);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.GetStandardValuesSupported(context);
			}
			return base.GetStandardValuesSupported(context);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (underlyingTypeConverter != null)
			{
				return underlyingTypeConverter.IsValid(context, value);
			}
			return base.IsValid(context, value);
		}
	}
}
