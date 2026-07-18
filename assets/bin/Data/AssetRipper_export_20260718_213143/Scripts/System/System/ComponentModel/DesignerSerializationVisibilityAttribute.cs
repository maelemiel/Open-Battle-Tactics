namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event)]
	public sealed class DesignerSerializationVisibilityAttribute : Attribute
	{
		private DesignerSerializationVisibility visibility;

		public static readonly DesignerSerializationVisibilityAttribute Default = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible);

		public static readonly DesignerSerializationVisibilityAttribute Content = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content);

		public static readonly DesignerSerializationVisibilityAttribute Hidden = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden);

		public static readonly DesignerSerializationVisibilityAttribute Visible = new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible);

		public DesignerSerializationVisibility Visibility
		{
			get
			{
				return visibility;
			}
		}

		public DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility vis)
		{
			visibility = vis;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is DesignerSerializationVisibilityAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((DesignerSerializationVisibilityAttribute)obj).Visibility == visibility;
		}

		public override int GetHashCode()
		{
			return visibility.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return visibility == Default.Visibility;
		}
	}
}
