namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public class DescriptionAttribute : Attribute
	{
		private string desc;

		public static readonly DescriptionAttribute Default = new DescriptionAttribute();

		public virtual string Description
		{
			get
			{
				return DescriptionValue;
			}
		}

		protected string DescriptionValue
		{
			get
			{
				return desc;
			}
			set
			{
				desc = value;
			}
		}

		public DescriptionAttribute()
		{
			desc = string.Empty;
		}

		public DescriptionAttribute(string name)
		{
			desc = name;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DescriptionAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((DescriptionAttribute)obj).Description == desc;
		}

		public override int GetHashCode()
		{
			return desc.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this == Default;
		}
	}
}
