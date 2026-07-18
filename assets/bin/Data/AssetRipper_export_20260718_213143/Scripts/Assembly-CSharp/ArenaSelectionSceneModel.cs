public class ArenaSelectionSceneModel : SceneModel
{
	public EventDataModel eventDataModel;

	public int forceTabIndex = -1;

	public ArenaSelectionSceneModel()
	{
	}

	public ArenaSelectionSceneModel(EventDataModel eventDataModel)
	{
		this.eventDataModel = eventDataModel;
	}

	public ArenaSelectionSceneModel(EventDataModel eventDataModel, int forceTabIndex)
	{
		this.eventDataModel = eventDataModel;
		this.forceTabIndex = forceTabIndex;
	}
}
