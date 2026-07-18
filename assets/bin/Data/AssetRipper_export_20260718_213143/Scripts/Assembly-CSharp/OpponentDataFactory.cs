using System.Collections.Generic;
using System.Linq;
using LitJson0;

public class OpponentDataFactory
{
	public static OpponentData FromUserJson(JsonObject userJson, List<JsonObject> userUnits)
	{
		UserProfile userProfile = UserProfileManager.ParseUserProfile(userJson);
		OpponentData opponentData = new OpponentData();
		opponentData.id = userProfile.id;
		opponentData.type = TeamType.Player;
		opponentData.name = userProfile.nickname;
		opponentData.division = userProfile.CurrentDivision;
		opponentData.winStreak = userProfile.winStreak;
		opponentData.pvpRating = userProfile.pvpRating;
		opponentData.thumbnailURL = userProfile.thumbnail;
		opponentData.abilities = OpponentDataUtils.GetAbilityListFromStrings(userProfile.CurrentAbilitySet.abilities);
		opponentData = OpponentDataUtils.FillOpponentUnits(opponentData, userUnits);
		foreach (UserBoost boost in userProfile.boosts)
		{
			opponentData.boosts.Add(boost.metaID);
		}
		return opponentData;
	}

	public static OpponentData FromUserProfile(UserProfile user)
	{
		OpponentData opponentData = new OpponentData();
		opponentData.id = user.id;
		opponentData.type = TeamType.Player;
		opponentData.name = user.nickname;
		opponentData.division = user.CurrentDivision;
		opponentData.pvpRating = user.pvpRating;
		opponentData.winStreak = user.winStreak;
		opponentData.thumbnailURL = user.thumbnail;
		opponentData.abilities = OpponentDataUtils.GetAbilityListFromStrings(user.CurrentAbilitySet.abilities);
		if (user.CurrentTeam != null)
		{
			opponentData.units = user.CurrentTeam.units.Cast<IUnitMetadata>().ToList();
		}
		foreach (UserBoost boost in user.boosts)
		{
			opponentData.boosts.Add(boost.metaID);
		}
		return opponentData;
	}

	public static OpponentData CreateTutorialOpponent()
	{
		OpponentData opponentData = new OpponentData();
		opponentData.id = string.Empty;
		opponentData.type = TeamType.Bot;
		opponentData.name = "AI";
		opponentData.division = ProgressionDivisionDataModel.GetAll()[0];
		return opponentData;
	}

	public static OpponentData FromAIArmy(string id, IDivisionMetadata division)
	{
		AiArmyDataModel single = AiArmyDataModel.GetSingle(id);
		OpponentData opponentData = new OpponentData();
		opponentData.id = single.id;
		opponentData.type = TeamType.Bot;
		opponentData.name = single.Name;
		opponentData.division = division;
		opponentData.units = single.GetUnitList().Cast<IUnitMetadata>().ToList();
		opponentData.abilities = OpponentDataUtils.GetAbilityListFromStrings(single.GetAbilityIDs());
		AiHandlerDataModel single2 = AiHandlerDataModel.GetSingle(single.aiStrategyId);
		if (single2 != null)
		{
			opponentData.ai = AIHandlerFactory.Create(single2.handler);
		}
		return opponentData;
	}
}
