using System.ComponentModel;

namespace System.Data
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class DataCategoryAttribute : CategoryAttribute
	{
		public DataCategoryAttribute(string category)
		{
		}

		[System.MonoTODO]
		protected override string GetLocalizedString(string value)
		{
			throw new NotImplementedException();
		}
	}
}
