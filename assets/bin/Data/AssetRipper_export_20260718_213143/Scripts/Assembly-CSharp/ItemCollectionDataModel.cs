using System.Collections.Generic;
using LitJson0;

public class ItemCollectionDataModel
{
	public class Item
	{
		public int amount;

		public UserInventory.ItemType itemType;

		public int itemId;

		public UnitLevelProgressionDataModel Unit
		{
			get
			{
				if (itemType != UserInventory.ItemType.Unit)
				{
					return null;
				}
				UnitLevelProgressionDataModel single = UnitLevelProgressionDataModel.GetSingle(itemId);
				if (single != null)
				{
					return single;
				}
				UnitDataModel single2 = UnitDataModel.GetSingle(itemId);
				if (single2 != null)
				{
					return single2.Levels[0];
				}
				Log.Error("A unit by ID '" + itemId + "' does not exist");
				return null;
			}
		}

		public UnitPartTypesDataModel Part
		{
			get
			{
				if (itemType != UserInventory.ItemType.Parts)
				{
					return null;
				}
				return UnitPartTypesDataModel.GetSingle(itemId);
			}
		}

		public Item()
		{
		}

		public Item(UserInventory.ItemType type, int itemId, int amount)
		{
			itemType = type;
			this.itemId = itemId;
			this.amount = amount;
		}
	}

	public List<Item> items;

	public ItemCollectionDataModel()
	{
		items = new List<Item>();
	}

	public ItemCollectionDataModel(UserInventory.ItemType itemType, int amount)
		: this(new Item
		{
			amount = amount,
			itemType = itemType
		})
	{
	}

	public ItemCollectionDataModel(Item item)
		: this(new List<Item> { item })
	{
	}

	public ItemCollectionDataModel(List<Item> items)
	{
		this.items = items;
	}

	public static ItemCollectionDataModel CopyCollectionDataModel(ItemCollectionDataModel model)
	{
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		itemCollectionDataModel.items = new List<Item>(model.items);
		return itemCollectionDataModel;
	}

	public string PrintItems()
	{
		string text = string.Empty;
		for (int i = 0; i < items.Count; i++)
		{
			string text2 = text;
			text = string.Concat(text2, "{", items[i].amount, ", ", items[i].itemType, ", ", items[i].itemId, "}\n");
		}
		return text;
	}

	public void AddItem(Item item)
	{
		items.Add(item);
	}

	public void AddItem(UserInventory.ItemType ItemType, int itemId, int amount)
	{
		items.Add(new Item
		{
			itemType = ItemType,
			itemId = itemId,
			amount = amount
		});
	}

	public bool HasItemOfType(UserInventory.ItemType ItemType)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].itemType == ItemType)
			{
				return true;
			}
		}
		return false;
	}

	public int GetItemCountOfType(UserInventory.ItemType ItemType)
	{
		int num = 0;
		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].itemType == ItemType)
			{
				num += items[i].amount;
			}
		}
		return num;
	}

	public static ItemCollectionDataModel FromJSON(JsonObject json)
	{
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		List<JsonObject> objectList = json.GetObjectList("items");
		if (objectList != null)
		{
			foreach (JsonObject item2 in objectList)
			{
				Item item = new Item();
				item.itemId = item2.GetInt("itemId");
				item.itemType = (UserInventory.ItemType)item2.GetInt("itemType");
				item.amount = item2.GetInt("amount");
				itemCollectionDataModel.AddItem(item);
			}
		}
		return itemCollectionDataModel;
	}
}
