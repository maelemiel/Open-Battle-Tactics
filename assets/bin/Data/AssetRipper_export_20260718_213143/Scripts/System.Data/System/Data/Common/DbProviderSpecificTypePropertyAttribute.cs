namespace System.Data.Common
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class DbProviderSpecificTypePropertyAttribute : Attribute
	{
		private bool isProviderSpecificTypeProperty;

		public bool IsProviderSpecificTypeProperty
		{
			get
			{
				return isProviderSpecificTypeProperty;
			}
		}

		public DbProviderSpecificTypePropertyAttribute(bool isProviderSpecificTypeProperty)
		{
			this.isProviderSpecificTypeProperty = isProviderSpecificTypeProperty;
		}
	}
}
