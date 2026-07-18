namespace Mono.Data.Tds.Protocol
{
	public class TdsBigDecimal
	{
		private bool isNegative;

		private byte precision;

		private byte scale;

		private int[] data;

		public int[] Data
		{
			get
			{
				return data;
			}
		}

		public byte Precision
		{
			get
			{
				return precision;
			}
		}

		public byte Scale
		{
			get
			{
				return scale;
			}
		}

		public bool IsNegative
		{
			get
			{
				return isNegative;
			}
		}

		public TdsBigDecimal(byte precision, byte scale, bool isNegative, int[] data)
		{
			this.isNegative = isNegative;
			this.precision = precision;
			this.scale = scale;
			this.data = data;
		}
	}
}
