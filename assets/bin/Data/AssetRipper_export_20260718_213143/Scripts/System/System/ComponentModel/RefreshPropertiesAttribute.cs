namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class RefreshPropertiesAttribute : Attribute
	{
		private RefreshProperties refresh;

		public static readonly RefreshPropertiesAttribute All = new RefreshPropertiesAttribute(RefreshProperties.All);

		public static readonly RefreshPropertiesAttribute Default = new RefreshPropertiesAttribute(RefreshProperties.None);

		public static readonly RefreshPropertiesAttribute Repaint = new RefreshPropertiesAttribute(RefreshProperties.Repaint);

		public RefreshProperties RefreshProperties
		{
			get
			{
				return refresh;
			}
		}

		public RefreshPropertiesAttribute(RefreshProperties refresh)
		{
			this.refresh = refresh;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RefreshPropertiesAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((RefreshPropertiesAttribute)obj).RefreshProperties == refresh;
		}

		public override int GetHashCode()
		{
			return refresh.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return this == Default;
		}
	}
}
