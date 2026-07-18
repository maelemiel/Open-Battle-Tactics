namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DesignerCategoryAttribute : Attribute
	{
		private string category;

		public static readonly DesignerCategoryAttribute Component = new DesignerCategoryAttribute("Component");

		public static readonly DesignerCategoryAttribute Form = new DesignerCategoryAttribute("Form");

		public static readonly DesignerCategoryAttribute Generic = new DesignerCategoryAttribute("Designer");

		public static readonly DesignerCategoryAttribute Default = new DesignerCategoryAttribute(string.Empty);

		public override object TypeId
		{
			get
			{
				return GetType();
			}
		}

		public string Category
		{
			get
			{
				return category;
			}
		}

		public DesignerCategoryAttribute()
		{
			category = string.Empty;
		}

		public DesignerCategoryAttribute(string category)
		{
			this.category = category;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DesignerCategoryAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((DesignerCategoryAttribute)obj).Category == category;
		}

		public override int GetHashCode()
		{
			return category.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return category == Default.Category;
		}
	}
}
