using System.Collections;
using System.Globalization;

namespace System.ComponentModel
{
	public class CollectionConverter : TypeConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value != null && value is ICollection)
			{
				return "(Collection)";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return null;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false;
		}
	}
}
