using System.ComponentModel;

namespace System.Data
{
	[AttributeUsage(AttributeTargets.All)]
	[Obsolete("DataSysDescriptionAttribute has been deprecated")]
	public class DataSysDescriptionAttribute : DescriptionAttribute
	{
		private string description;

		public override string Description
		{
			get
			{
				return description;
			}
		}

		[Obsolete("DataSysDescriptionAttribute has been deprecated")]
		public DataSysDescriptionAttribute(string description)
			: base(description)
		{
			this.description = description;
		}
	}
}
