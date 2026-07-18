namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class DefaultValueAttribute : Attribute
	{
		private object DefaultValue;

		public virtual object Value
		{
			get
			{
				return DefaultValue;
			}
		}

		public DefaultValueAttribute(bool value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(byte value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(char value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(double value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(short value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(int value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(long value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(object value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(float value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(string value)
		{
			DefaultValue = value;
		}

		public DefaultValueAttribute(Type type, string value)
		{
			try
			{
				if (type.IsEnum)
				{
					DefaultValue = Enum.Parse(type, value);
				}
				else if (type == typeof(TimeSpan))
				{
					DefaultValue = TimeSpan.Parse(value);
				}
				else
				{
					DefaultValue = Convert.ChangeType(value, type, null);
				}
			}
			catch
			{
			}
		}

		protected void SetValue(object value)
		{
			DefaultValue = value;
		}

		public override bool Equals(object obj)
		{
			DefaultValueAttribute defaultValueAttribute = obj as DefaultValueAttribute;
			if (defaultValueAttribute == null)
			{
				return false;
			}
			if (DefaultValue == null)
			{
				return defaultValueAttribute.Value == null;
			}
			return DefaultValue.Equals(defaultValueAttribute.Value);
		}

		public override int GetHashCode()
		{
			if (DefaultValue == null)
			{
				return base.GetHashCode();
			}
			return DefaultValue.GetHashCode();
		}
	}
}
