namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event)]
	public class DisplayNameAttribute : Attribute
	{
		public static readonly DisplayNameAttribute Default = new DisplayNameAttribute();

		private string attributeDisplayName;

		public virtual string DisplayName
		{
			get
			{
				return attributeDisplayName;
			}
		}

		protected string DisplayNameValue
		{
			get
			{
				return attributeDisplayName;
			}
			set
			{
				attributeDisplayName = value;
			}
		}

		public DisplayNameAttribute()
		{
			attributeDisplayName = string.Empty;
		}

		public DisplayNameAttribute(string displayName)
		{
			attributeDisplayName = displayName;
		}

		public override bool IsDefaultAttribute()
		{
			if (attributeDisplayName != null)
			{
				return attributeDisplayName.Length == 0;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return attributeDisplayName.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
			{
				return true;
			}
			DisplayNameAttribute displayNameAttribute = obj as DisplayNameAttribute;
			if (displayNameAttribute == null)
			{
				return false;
			}
			return displayNameAttribute.DisplayName == attributeDisplayName;
		}
	}
}
