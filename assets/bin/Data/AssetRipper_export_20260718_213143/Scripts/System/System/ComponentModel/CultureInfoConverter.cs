using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class CultureInfoConverter : TypeConverter
	{
		private class CultureInfoComparer : IComparer
		{
			public int Compare(object first, object second)
			{
				if (first == null)
				{
					if (second == null)
					{
						return 0;
					}
					return -1;
				}
				if (second == null)
				{
					return 1;
				}
				return string.Compare(((CultureInfo)first).DisplayName, ((CultureInfo)second).DisplayName, false, CultureInfo.CurrentCulture);
			}
		}

		private StandardValuesCollection _standardValues;

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
			string text = value as string;
			if (text != null)
			{
				if (string.Compare(text, "(Default)", false) == 0)
				{
					return CultureInfo.InvariantCulture;
				}
				try
				{
					return new CultureInfo(text);
				}
				catch
				{
					CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
					foreach (CultureInfo cultureInfo in cultures)
					{
						if (string.Compare(cultureInfo.DisplayName, 0, text, 0, text.Length, true) == 0)
						{
							return cultureInfo;
						}
					}
				}
				throw new ArgumentException(string.Format("Culture {0} cannot be converted to a CultureInfo or is not available in this environment.", value));
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value != null && value is CultureInfo)
				{
					if (value == CultureInfo.InvariantCulture)
					{
						return "(Default)";
					}
					return ((CultureInfo)value).DisplayName;
				}
				return "(Default)";
			}
			if (destinationType == typeof(InstanceDescriptor) && value is CultureInfo)
			{
				CultureInfo cultureInfo = (CultureInfo)value;
				ConstructorInfo constructor = typeof(CultureInfo).GetConstructor(new Type[1] { typeof(int) });
				return new InstanceDescriptor(constructor, new object[1] { cultureInfo.LCID });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (_standardValues == null)
			{
				CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				Array.Sort(cultures, new CultureInfoComparer());
				CultureInfo[] array = new CultureInfo[cultures.Length + 1];
				array[0] = CultureInfo.InvariantCulture;
				Array.Copy(cultures, 0, array, 1, cultures.Length);
				_standardValues = new StandardValuesCollection(array);
			}
			return _standardValues;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
