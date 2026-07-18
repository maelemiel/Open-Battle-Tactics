using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class EnumConverter : TypeConverter
	{
		private class EnumComparer : IComparer
		{
			int IComparer.Compare(object compareObject1, object compareObject2)
			{
				string text = compareObject1 as string;
				string text2 = compareObject2 as string;
				if (text == null || text2 == null)
				{
					return System.Collections.Comparer.Default.Compare(compareObject1, compareObject2);
				}
				return CultureInfo.InvariantCulture.CompareInfo.Compare(text, text2);
			}
		}

		private Type type;

		private StandardValuesCollection stdValues;

		protected virtual IComparer Comparer
		{
			get
			{
				return new EnumComparer();
			}
		}

		protected Type EnumType
		{
			get
			{
				return type;
			}
		}

		protected StandardValuesCollection Values
		{
			get
			{
				return stdValues;
			}
			set
			{
				stdValues = value;
			}
		}

		private bool IsFlags
		{
			get
			{
				return type.IsDefined(typeof(FlagsAttribute), false);
			}
		}

		public EnumConverter(Type type)
		{
			this.type = type;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			if (destinationType == typeof(Enum[]))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value != null)
			{
				if (value is IConvertible)
				{
					Type underlyingType = Enum.GetUnderlyingType(type);
					if (underlyingType != value.GetType())
					{
						value = ((IConvertible)value).ToType(underlyingType, culture);
					}
				}
				if (!IsFlags && !IsValid(context, value))
				{
					throw CreateValueNotValidException(value);
				}
				return Enum.Format(type, value, "G");
			}
			if (destinationType == typeof(InstanceDescriptor) && value != null)
			{
				string text = ConvertToString(context, culture, value);
				if (IsFlags && text.IndexOf(",") != -1)
				{
					if (value is IConvertible)
					{
						Type underlyingType2 = Enum.GetUnderlyingType(type);
						object obj = ((IConvertible)value).ToType(underlyingType2, culture);
						MethodInfo method = typeof(Enum).GetMethod("ToObject", new Type[2]
						{
							typeof(Type),
							underlyingType2
						});
						return new InstanceDescriptor(method, new object[2] { type, obj });
					}
				}
				else
				{
					FieldInfo field = type.GetField(text);
					if (field != null)
					{
						return new InstanceDescriptor(field, null);
					}
				}
			}
			else if (destinationType == typeof(Enum[]) && value != null)
			{
				if (!IsFlags)
				{
					return new Enum[1] { (Enum)Enum.ToObject(type, value) };
				}
				long num = Convert.ToInt64((Enum)value, culture);
				Array values = Enum.GetValues(type);
				long[] array = new long[values.Length];
				for (int i = 0; i < values.Length; i++)
				{
					array[i] = Convert.ToInt64(values.GetValue(i));
				}
				ArrayList arrayList = new ArrayList();
				bool flag = false;
				while (!flag)
				{
					flag = true;
					long[] array2 = array;
					foreach (long num2 in array2)
					{
						if ((num2 != 0L && (num2 & num) == num2) || num2 == num)
						{
							arrayList.Add(Enum.ToObject(type, num2));
							num &= ~num2;
							flag = false;
						}
					}
					if (num == 0L)
					{
						flag = true;
					}
				}
				if (num != 0L)
				{
					arrayList.Add(Enum.ToObject(type, num));
				}
				return arrayList.ToArray(typeof(Enum));
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			if (sourceType == typeof(Enum[]))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string text = value as string;
				try
				{
					if (text.IndexOf(',') == -1)
					{
						return Enum.Parse(type, text, true);
					}
					long num = 0L;
					string[] array = text.Split(',');
					string[] array2 = array;
					foreach (string value2 in array2)
					{
						Enum value3 = (Enum)Enum.Parse(type, value2, true);
						num |= Convert.ToInt64(value3, culture);
					}
					return Enum.ToObject(type, num);
				}
				catch (Exception innerException)
				{
					throw new FormatException(text + " is not a valid value for " + type.Name, innerException);
				}
			}
			if (value is Enum[])
			{
				long num2 = 0L;
				Enum[] array3 = (Enum[])value;
				foreach (Enum value4 in array3)
				{
					num2 |= Convert.ToInt64(value4, culture);
				}
				return Enum.ToObject(type, num2);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return Enum.IsDefined(type, value);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return !IsFlags;
		}

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (stdValues == null)
			{
				Array values = Enum.GetValues(type);
				Array.Sort(values);
				stdValues = new StandardValuesCollection(values);
			}
			return stdValues;
		}

		private ArgumentException CreateValueNotValidException(object value)
		{
			string message = string.Format(CultureInfo.InvariantCulture, "The value '{0}' is not a valid value for the enum '{1}'", value, type.Name);
			return new ArgumentException(message);
		}
	}
}
