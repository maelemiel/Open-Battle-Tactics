using NUnit.Framework;

namespace BattleDiceTests
{
	[TestFixture]
	public class PriceTests
	{
		[Test]
		public void TestPriceMultiplication()
		{
			_TestPriceMultiplication(new UserPriceDataModel(UserInventory.ItemType.Coins, 10), 5);
			_TestPriceMultiplication(new UserPriceDataModel(UserInventory.ItemType.Coins, 0), 100);
			_TestPriceMultiplication(new UserPriceDataModel(UserInventory.ItemType.Coins, 100), 0);
			_TestPriceMultiplication(new UserPriceDataModel(UserInventory.ItemType.Coins, 100), 1);
			_TestPriceMultiplication(new UserPriceDataModel(UserInventory.ItemType.Coins, 100, UserInventory.ItemType.PremiumCurrency, 10), 1);
		}

		private void _TestPriceMultiplication(UserPriceDataModel price, int multiplier)
		{
			UserPriceDataModel userPriceDataModel = price.Multiply(multiplier);
			Assert.AreEqual(userPriceDataModel.items[0].amount, price.items[0].amount * multiplier);
			Assert.AreEqual(userPriceDataModel.items[0].itemType, price.items[0].itemType);
			Assert.AreEqual(userPriceDataModel.items.Count, price.items.Count);
		}
	}
}
