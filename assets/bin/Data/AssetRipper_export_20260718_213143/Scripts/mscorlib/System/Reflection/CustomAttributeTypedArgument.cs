using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public struct CustomAttributeTypedArgument
	{
		private Type argumentType;

		private object value;

		public Type ArgumentType
		{
			get
			{
				return argumentType;
			}
		}

		public object Value
		{
			get
			{
				return value;
			}
		}

		internal CustomAttributeTypedArgument(Type argumentType, object value)
		{
			this.argumentType = argumentType;
			this.value = value;
			if (value is Array)
			{
				Array array = (Array)value;
				Type elementType = array.GetType().GetElementType();
				CustomAttributeTypedArgument[] array2 = new CustomAttributeTypedArgument[array.GetLength(0)];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = new CustomAttributeTypedArgument(elementType, array.GetValue(i));
				}
				this.value = new ReadOnlyCollection<CustomAttributeTypedArgument>(array2);
			}
		}

		public override string ToString()
		{
			string text = ((value == null) ? string.Empty : value.ToString());
			if (argumentType == typeof(string))
			{
				return "\"" + text + "\"";
			}
			if (argumentType == typeof(Type))
			{
				return "typeof (" + text + ")";
			}
			if (argumentType.IsEnum)
			{
				return "(" + argumentType.Name + ")" + text;
			}
			return text;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CustomAttributeTypedArgument))
			{
				return false;
			}
			CustomAttributeTypedArgument customAttributeTypedArgument = (CustomAttributeTypedArgument)obj;
			return (customAttributeTypedArgument.argumentType != argumentType || value == null) ? (customAttributeTypedArgument.value == null) : value.Equals(customAttributeTypedArgument.value);
		}

		public override int GetHashCode()
		{
			return (argumentType.GetHashCode() << 16) + ((value != null) ? value.GetHashCode() : 0);
		}

		public static bool operator ==(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(CustomAttributeTypedArgument left, CustomAttributeTypedArgument right)
		{
			return !left.Equals(right);
		}
	}
}
