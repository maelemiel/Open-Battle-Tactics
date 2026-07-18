using System.Collections.Generic;

public class EventAssetsDataModel : BaseDataModel
{
	public int backgroundAssetId;

	public string eventInfoKeyStringBody1;

	public string eventInfoKeyStringBody2;

	public string eventInfoKeyStringBody3;

	public string eventInfoKeyStringBody4;

	public string eventInfoKeyStringBody5;

	public string eventInfoKeyStringEventH1;

	public string eventInfoKeyStringH2;

	public string eventInfoKeyStringUnitsSet;

	public int gachaAssetBundle1;

	public int gachaAssetBundle2;

	public int gachaInfoAssetBundle1;

	public int gachaInfoAssetBundle2;

	public int gachaInfoAssetBundle3;

	public int gachaInfoAssetBundle4;

	public int homeScreenUnitId;

	public string keyDescriptionAlreadyMemberEventPopup;

	public string keyDescriptionJoinClubEventPopup;

	public string keyTitleAlreadyMemberEventPopup;

	public string keyTitleJoinClubEventPopup;

	public int leaderboardsAssetId;

	public int leftUnitId;

	public int logoAssetId;

	public int panshotAssetId;

	public int rightUnitId;

	public static EventAssetsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventAssetsDataModel>(id.ToString());
	}

	public static EventAssetsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventAssetsDataModel>(id);
	}

	public static List<EventAssetsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventAssetsDataModel>();
	}
}
