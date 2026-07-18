using System.Collections.Generic;

public class OpponentData
{
	public string name;

	public string id;

	public IDivisionMetadata division;

	public TeamType type;

	public List<IUnitMetadata> units;

	public List<IAbilityMetadata> abilities;

	public BaseAI ai;

	public int winStreak;

	public int pvpRating;

	public string thumbnailURL;

	public List<int> boosts = new List<int>();

	public bool isTeamOne;

	public EventRaidbossDamageDropRateDataModel[] rewardDropRate;

	public int startingBossHealth;

	public string raidbossId;

	public string raidbossStatus;

	public int randomSeed;
}
