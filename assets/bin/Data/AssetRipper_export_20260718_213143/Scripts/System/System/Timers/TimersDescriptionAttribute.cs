using System.ComponentModel;

namespace System.Timers
{
	[AttributeUsage(AttributeTargets.All)]
	public class TimersDescriptionAttribute : DescriptionAttribute
	{
		public override string Description
		{
			get
			{
				return base.Description;
			}
		}

		public TimersDescriptionAttribute(string description)
			: base(description)
		{
		}
	}
}
