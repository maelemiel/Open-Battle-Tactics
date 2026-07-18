using NUnit.Framework;

namespace BattleDiceTests
{
	[TestFixture]
	public class UserProfileTests
	{
		private const int PART_ONE = 1;

		private const int PART_TWO = 2;

		private const int PART_THREE = 3;

		private const int PART_FOUR = 4;

		private UserProfile GetUser()
		{
			UserProfile userProfile = new UserProfile();
			userProfile.coins += 1200;
			userProfile.gems += 200;
			userProfile.inventory.AddParts(1.ToString(), 10);
			userProfile.inventory.AddParts(2.ToString(), 5);
			userProfile.inventory.AddParts(3.ToString(), 1);
			return userProfile;
		}

		[Test]
		public void TestSingleItemPrices()
		{
			UserProfile user = GetUser();
			Assert.True(user.CanAfford(new UserPriceDataModel(UserInventory.ItemType.Coins, 1000)));
			Assert.False(user.CanAfford(new UserPriceDataModel(UserInventory.ItemType.Coins, 1400)));
			int coins = user.coins;
			user.PayPrice(new UserPriceDataModel(UserInventory.ItemType.Coins, 1000));
			Assert.True(user.coins == coins - 1000);
		}

		[Test]
		public void TestMultiItemPrices()
		{
			UserProfile user = GetUser();
			Assert.True(user.CanAfford(new UserPriceDataModel(UserInventory.ItemType.Coins, 1000, UserInventory.ItemType.PremiumCurrency, 100)));
			Assert.True(user.CanAfford(new UserPriceDataModel(UserInventory.ItemType.Coins, 0, UserInventory.ItemType.PremiumCurrency, 0)));
			Assert.False(user.CanAfford(new UserPriceDataModel(UserInventory.ItemType.Coins, 1400, UserInventory.ItemType.PremiumCurrency, 500)));
			Assert.False(user.CanAfford(new UserPriceDataModel(UserInventory.ItemType.Coins, 1400, UserInventory.ItemType.PremiumCurrency, 0)));
			Assert.False(user.CanAfford(new UserPriceDataModel(UserInventory.ItemType.Coins, 0, UserInventory.ItemType.PremiumCurrency, 500)));
			int coins = user.coins;
			int gems = user.gems;
			user.PayPrice(new UserPriceDataModel(UserInventory.ItemType.Coins, 1000, UserInventory.ItemType.PremiumCurrency, 100));
			Assert.True(user.coins == coins - 1000);
			Assert.True(user.gems == gems - 100);
		}

		[Test]
		public void TestPartsPrices()
		{
			UserProfile user = GetUser();
			UserPriceDataModel userPriceDataModel = new UserPriceDataModel();
			userPriceDataModel.AddItem(UserInventory.ItemType.Parts, 1, 1);
			userPriceDataModel.AddItem(UserInventory.ItemType.Parts, 2, 1);
			userPriceDataModel.AddItem(UserInventory.ItemType.Parts, 3, 1);
			Assert.True(user.CanAfford(userPriceDataModel));
			UserPriceDataModel userPriceDataModel2 = new UserPriceDataModel();
			userPriceDataModel2.AddItem(UserInventory.ItemType.Parts, 1, 100);
			userPriceDataModel2.AddItem(UserInventory.ItemType.Parts, 2, 100);
			userPriceDataModel2.AddItem(UserInventory.ItemType.Parts, 3, 100);
			userPriceDataModel2.AddItem(UserInventory.ItemType.Parts, 4, 100);
			Assert.False(user.CanAfford(userPriceDataModel2));
			int parts = user.inventory.GetParts(1.ToString());
			int parts2 = user.inventory.GetParts(2.ToString());
			int parts3 = user.inventory.GetParts(3.ToString());
			int parts4 = user.inventory.GetParts(4.ToString());
			Assert.True(user.inventory.GetParts(1.ToString()) == 10);
			user.PayPrice(userPriceDataModel);
			Assert.True(user.inventory.GetParts(1.ToString()) == parts - 1);
			Assert.True(user.inventory.GetParts(2.ToString()) == parts2 - 1);
			Assert.True(user.inventory.GetParts(3.ToString()) == parts3 - 1);
			Assert.True(user.inventory.GetParts(4.ToString()) == parts4);
		}

		[Test]
		public void TestEnoughCash()
		{
			UserPriceDataModel userPriceDataModel = new UserPriceDataModel(UserInventory.ItemType.Coins, 200);
			UserProfile userProfile = new UserProfile();
			userProfile.inventory.AddItem(UserInventory.ItemType.Coins, null, 500);
			Assert.IsTrue(userProfile.CanAfford(userPriceDataModel.items));
			userProfile.PayPrice(userPriceDataModel);
			Assert.AreEqual(300, userProfile.coins);
		}

		[Test]
		public void TestNotEnoughCash()
		{
			UserPriceDataModel userPriceDataModel = new UserPriceDataModel(UserInventory.ItemType.Coins, 200);
			UserProfile userProfile = new UserProfile();
			userProfile.inventory.AddItem(UserInventory.ItemType.Coins, null, 100);
			Assert.IsFalse(userProfile.CanAfford(userPriceDataModel.items));
		}

		[Test]
		public void TestNotEnoughCashUseGems()
		{
			UserPriceDataModel userPriceDataModel = new UserPriceDataModel(UserInventory.ItemType.Coins, 200);
			UserProfile userProfile = new UserProfile();
			userProfile.inventory.AddItem(UserInventory.ItemType.Coins, null, 100);
			userProfile.inventory.AddItem(UserInventory.ItemType.PremiumCurrency, null, 2);
			Assert.IsFalse(userProfile.CanAfford(userPriceDataModel.items));
			UserPriceDataModel premiumPrice = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, userProfile, 200);
			int expected = -1;
			int actual = -1;
			foreach (ItemCollectionDataModel.Item item in premiumPrice.items)
			{
				if (item.itemType == UserInventory.ItemType.Coins)
				{
					expected = item.amount;
				}
				else if (item.itemType == UserInventory.ItemType.PremiumCurrency)
				{
					actual = item.amount;
				}
			}
			Assert.AreEqual(expected, userProfile.coins);
			Assert.AreEqual(1, actual);
			Assert.IsTrue(userProfile.CanAfford(premiumPrice.items));
			userProfile.PayPrice(premiumPrice);
			Assert.AreEqual(0, userProfile.coins);
			Assert.AreEqual(1, userProfile.gems);
		}

		[Test]
		public void TestNotEnoughCashOrGems()
		{
			UserPriceDataModel userPriceDataModel = new UserPriceDataModel(UserInventory.ItemType.Coins, 1000);
			UserProfile userProfile = new UserProfile();
			userProfile.inventory.AddItem(UserInventory.ItemType.Coins, null, 100);
			userProfile.inventory.AddItem(UserInventory.ItemType.PremiumCurrency, null, 2);
			Assert.IsFalse(userProfile.CanAfford(userPriceDataModel.items));
			UserPriceDataModel premiumPrice = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, userProfile, 200);
			int expected = -1;
			int actual = -1;
			foreach (ItemCollectionDataModel.Item item in premiumPrice.items)
			{
				if (item.itemType == UserInventory.ItemType.Coins)
				{
					expected = item.amount;
				}
				else if (item.itemType == UserInventory.ItemType.PremiumCurrency)
				{
					actual = item.amount;
				}
			}
			Assert.AreEqual(expected, userProfile.coins);
			Assert.AreEqual(5, actual);
			Assert.IsFalse(userProfile.CanAfford(premiumPrice.items));
		}
	}
}
