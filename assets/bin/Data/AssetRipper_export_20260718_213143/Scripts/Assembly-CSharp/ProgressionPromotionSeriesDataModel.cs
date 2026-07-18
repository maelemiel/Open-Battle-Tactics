using System.Collections.Generic;

public class ProgressionPromotionSeriesDataModel : BaseDataModel
{
	public int currentDivisionId;

	public int giftId;

	public int promotionDivisionId;

	public int totalBattle;

	public int totalWin;

	public ProgressionDivisionDataModel CurrentDivision
	{
		get
		{
			return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(currentDivisionId);
		}
	}

	public ProgressionDivisionDataModel PromotionDivision
	{
		get
		{
			return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(promotionDivisionId);
		}
	}

	public int MaxLosses
	{
		get
		{
			return totalBattle - totalWin + 1;
		}
	}

	public static ProgressionPromotionSeriesDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionPromotionSeriesDataModel>(id.ToString());
	}

	public static ProgressionPromotionSeriesDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionPromotionSeriesDataModel>(id);
	}

	public static List<ProgressionPromotionSeriesDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<ProgressionPromotionSeriesDataModel>();
	}
}
