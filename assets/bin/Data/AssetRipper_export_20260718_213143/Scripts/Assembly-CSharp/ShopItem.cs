public class ShopItem
{
	public string title;

	public string sku;

	public string detail;

	public int value;

	public string currency;

	public string priceCode;

	public double price;

	public double usdPrice;

	public string displayPrice;

	public ShopItem(string title, string sku, string detail, int value, string currency, string priceCode, double price, double usdPrice, string displayPrice)
	{
		this.title = title;
		this.sku = sku;
		this.detail = detail;
		this.value = value;
		this.currency = currency;
		this.priceCode = priceCode;
		this.price = price;
		this.usdPrice = usdPrice;
		this.displayPrice = displayPrice;
	}

	public override string ToString()
	{
		return string.Format("title: {0}\n sku: {1}\n detail: {2}\n value: {3}\n currency: {4}\n priceCode: {5}\n price: {6}\n usdPrice: {7}\n displayPrice: {8}", title, sku, detail, value, currency, priceCode, price, usdPrice, displayPrice);
	}
}
