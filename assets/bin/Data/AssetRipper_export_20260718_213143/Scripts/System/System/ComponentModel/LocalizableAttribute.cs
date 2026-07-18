namespace System.ComponentModel
{
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizableAttribute : Attribute
	{
		private bool localizable;

		public static readonly LocalizableAttribute Default = new LocalizableAttribute(false);

		public static readonly LocalizableAttribute No = new LocalizableAttribute(false);

		public static readonly LocalizableAttribute Yes = new LocalizableAttribute(true);

		public bool IsLocalizable
		{
			get
			{
				return localizable;
			}
		}

		public LocalizableAttribute(bool localizable)
		{
			this.localizable = localizable;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is LocalizableAttribute))
			{
				return false;
			}
			if (obj == this)
			{
				return true;
			}
			return ((LocalizableAttribute)obj).IsLocalizable == localizable;
		}

		public override int GetHashCode()
		{
			return localizable.GetHashCode();
		}

		public override bool IsDefaultAttribute()
		{
			return localizable == Default.IsLocalizable;
		}
	}
}
