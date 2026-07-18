namespace System.Diagnostics
{
	[System.MonoLimitation("This attribute is not considered in trace support.")]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SwitchLevelAttribute : Attribute
	{
		private Type type;

		public Type SwitchLevelType
		{
			get
			{
				return type;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				type = value;
			}
		}

		public SwitchLevelAttribute(Type switchLevelType)
		{
			if (switchLevelType == null)
			{
				throw new ArgumentNullException("switchLevelType");
			}
			type = switchLevelType;
		}
	}
}
