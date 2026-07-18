using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, Inherited = false)]
	public sealed class DecimalConstantAttribute : Attribute
	{
		private byte scale;

		private bool sign;

		private int hi;

		private int mid;

		private int low;

		public decimal Value
		{
			get
			{
				return new decimal(low, mid, hi, sign, scale);
			}
		}

		[CLSCompliant(false)]
		public DecimalConstantAttribute(byte scale, byte sign, uint hi, uint mid, uint low)
		{
			this.scale = scale;
			this.sign = Convert.ToBoolean(sign);
			this.hi = (int)hi;
			this.mid = (int)mid;
			this.low = (int)low;
		}

		public DecimalConstantAttribute(byte scale, byte sign, int hi, int mid, int low)
		{
			this.scale = scale;
			this.sign = Convert.ToBoolean(sign);
			this.hi = hi;
			this.mid = mid;
			this.low = low;
		}
	}
}
