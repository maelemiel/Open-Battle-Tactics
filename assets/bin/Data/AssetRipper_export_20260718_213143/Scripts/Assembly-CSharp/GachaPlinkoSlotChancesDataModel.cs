using System.Collections.Generic;

public class GachaPlinkoSlotChancesDataModel : BaseDataModel
{
	public int chancePrizeSlot0;

	public int chancePrizeSlot1;

	public int chancePrizeSlot2;

	public int chancePrizeSlot3;

	public int chancePrizeSlot4;

	public int chancePrizeSlot5;

	public int chancePrizeSlot6;

	public int chancePrizeSlot7;

	public int chancePrizeSlot8;

	public int itemsMixerId;

	public int slotIndex;

	public static GachaPlinkoSlotChancesDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoSlotChancesDataModel>(id.ToString());
	}

	public static GachaPlinkoSlotChancesDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPlinkoSlotChancesDataModel>(id);
	}

	public static List<GachaPlinkoSlotChancesDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaPlinkoSlotChancesDataModel>();
	}
}
