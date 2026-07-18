namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DefaultPropertyAttribute : Attribute
	{
		private string property_name;

		public static readonly DefaultPropertyAttribute Default = new DefaultPropertyAttribute(null);

		public string Name
		{
			get
			{
				return property_name;
			}
		}

		public DefaultPropertyAttribute(string name)
		{
			property_name = name;
		}

		public override bool Equals(object o)
		{
			if (!(o is DefaultPropertyAttribute))
			{
				return false;
			}
			return ((DefaultPropertyAttribute)o).Name == property_name;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
