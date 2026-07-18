public class BattleSceneModel : SceneModel
{
	public MatchData.Type matchType = MatchData.Type.TEST;

	public int difficulty;

	public string raidbossId = string.Empty;

	public EventDataModel activeEvent;

	public string botPartId = string.Empty;

	public BattleSceneModel()
	{
	}

	public BattleSceneModel(MatchData.Type matchType, int difficulty = 0)
	{
		this.matchType = matchType;
		this.difficulty = difficulty;
	}
}
