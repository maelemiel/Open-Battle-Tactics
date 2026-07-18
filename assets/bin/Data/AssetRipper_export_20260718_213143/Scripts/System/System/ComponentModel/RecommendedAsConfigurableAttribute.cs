namespace System.ComponentModel
{
	[Obsolete("Use SettingsBindableAttribute instead of RecommendedAsConfigurableAttribute")]
	[AttributeUsage(AttributeTargets.Property)]
	public class RecommendedAsConfigurableAttribute : Attribute
	{
		private bool recommendedAsConfigurable;

		public static readonly RecommendedAsConfigurableAttribute Default = new RecommendedAsConfigurableAttribute(false);

		public static readonly RecommendedAsConfigurableAttribute No = new RecommendedAsConfigurableAttribute(false);

		public static readonly RecommendedAsConfigurableAttribute Yes = new RecommendedAsConfigurableAttribute(true);

		public bool RecommendedAsConfigurable
		{
			get
			{
				return recommendedAsConfigurable;
			}
		}

		public RecommendedAsConfigurableAttribute(bool recommendedAsConfigurable)
		{
			this.recommendedAsConfigurable = recommendedAsConfigurable;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RecommendedAsConfigurableAttribute))
			{
				return false;
			}
			return ((RecommendedAsConfigurableAttribute)obj).RecommendedAsConfigurable == recommendedAsConfigurable;
		}

		public override int GetHashCode()
		{
			return recommendedAsConfigurable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return recommendedAsConfigurable == Default.RecommendedAsConfigurable;
		}
	}
}
