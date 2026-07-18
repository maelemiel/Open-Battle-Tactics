namespace MobageEditor
{
	public class ItemData
	{
		public string itemId;

		public string name;

		public int price;

		public string longDescription;

		public string imageUrl;

		public string originPriceLabel;

		public string originCurrencyLabel;

		public bool itemForCash;

		public double originPrice;

		public string currency;

		public string id
		{
			get
			{
				return itemId;
			}
			set
			{
				itemId = value;
			}
		}

		public string value
		{
			get
			{
				return price.ToString();
			}
			set
			{
				price = int.Parse(value);
			}
		}

		public string description
		{
			get
			{
				return longDescription;
			}
			set
			{
				longDescription = value;
			}
		}
	}
}
