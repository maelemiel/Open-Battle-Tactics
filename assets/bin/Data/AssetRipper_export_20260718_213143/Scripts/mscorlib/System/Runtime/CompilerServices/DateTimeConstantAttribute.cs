using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	[ComVisible(true)]
	public sealed class DateTimeConstantAttribute : CustomConstantAttribute
	{
		private long ticks;

		internal long Ticks
		{
			get
			{
				return ticks;
			}
		}

		public override object Value
		{
			get
			{
				return ticks;
			}
		}

		public DateTimeConstantAttribute(long ticks)
		{
			this.ticks = ticks;
		}
	}
}
