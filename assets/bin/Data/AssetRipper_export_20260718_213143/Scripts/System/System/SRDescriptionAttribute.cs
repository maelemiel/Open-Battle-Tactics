using System.ComponentModel;

namespace System
{
	[AttributeUsage(AttributeTargets.All)]
	internal class SRDescriptionAttribute : DescriptionAttribute
	{
		private bool isReplaced;

		public override string Description
		{
			get
			{
				if (!isReplaced)
				{
					isReplaced = true;
					base.DescriptionValue = Locale.GetText(base.DescriptionValue);
				}
				return base.DescriptionValue;
			}
		}

		public SRDescriptionAttribute(string description)
			: base(description)
		{
		}
	}
}
