using System.Globalization;

namespace System.ComponentModel
{
	public class ArrayConverter : CollectionConverter
	{
		internal class ArrayPropertyDescriptor : PropertyDescriptor
		{
			private int index;

			private Type array_type;

			public override Type ComponentType
			{
				get
				{
					return array_type;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return array_type.GetElementType();
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return false;
				}
			}

			public ArrayPropertyDescriptor(int index, Type array_type)
				: base(string.Format("[{0}]", index), null)
			{
				this.index = index;
				this.array_type = array_type;
			}

			public override object GetValue(object component)
			{
				if (component == null)
				{
					return null;
				}
				return ((Array)component).GetValue(index);
			}

			public override void SetValue(object component, object value)
			{
				if (component != null)
				{
					((Array)component).SetValue(value, index);
				}
			}

			public override void ResetValue(object component)
			{
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(string) && value is Array)
			{
				return value.GetType().Name + " Array";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (value == null)
			{
				throw new NullReferenceException();
			}
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			if (value is Array)
			{
				Array array = (Array)value;
				for (int i = 0; i < array.Length; i++)
				{
					propertyDescriptorCollection.Add(new ArrayPropertyDescriptor(i, array.GetType()));
				}
			}
			return propertyDescriptorCollection;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
