namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public sealed class CurrencyWrapper
	{
		private decimal currency;

		public decimal WrappedObject
		{
			get
			{
				return currency;
			}
		}

		public CurrencyWrapper(decimal obj)
		{
			currency = obj;
		}

		public CurrencyWrapper(object obj)
		{
			if (obj.GetType() != typeof(decimal))
			{
				throw new ArgumentException("obj has to be a Decimal type");
			}
			currency = (decimal)obj;
		}
	}
}
