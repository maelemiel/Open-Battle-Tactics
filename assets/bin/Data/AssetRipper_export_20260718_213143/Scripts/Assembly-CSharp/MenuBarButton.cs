using UnityEngine;

public class MenuBarButton : ScrollableCell
{
	public SceneTransitionManager.Scene scene;

	public Transform itemTransform;

	[SerializeField]
	private GameObject enabledGameObject;

	[SerializeField]
	private GameObject disabledGameObject;

	[SerializeField]
	private tk2dUIItem enabledButton;

	[SerializeField]
	private tk2dSprite itemIcon;

	[SerializeField]
	private tk2dTextMesh iconText;

	[SerializeField]
	private tk2dTextMesh iconTextDisabled;

	[SerializeField]
	private string buttonID;

	[SerializeField]
	private tk2dBaseSprite regularButtonSprite;

	[SerializeField]
	private tk2dBaseSprite lockedButtonSprite;

	public GameObject[] notificationGameObjects;

	public tk2dTextMesh[] notificationTextMeshs;

	private MenuBarController.MenuButtonOption cellData;

	public string ButtonID
	{
		get
		{
			return buttonID;
		}
		private set
		{
			buttonID = value;
		}
	}

	public override void Init(ScrollableAreaController controller, object data, int index, float cellHeight = 0f, float cellWidth = 0f, ScrollableCell parentCell = null)
	{
		base.Init(controller, data, index, cellHeight, cellWidth);
		cellData = (MenuBarController.MenuButtonOption)data;
		if (cellData != null)
		{
			buttonID = cellData.buttonID;
			scene = cellData.scene;
			enabledButton.SendMessageOnClickMethodName = cellData.functionToCall;
			enabledButton.sendMessageTarget = TopBarController.instance.gameObject;
			itemIcon.SetSprite(cellData.iconName);
			iconText.color = cellData.textColour;
			iconText.text = cellData.textLocalization.Localize();
			if (cellData.textSize != 0f)
			{
				iconText.scale = new Vector3(cellData.textSize, cellData.textSize, 1f);
				iconTextDisabled.scale = new Vector3(cellData.textSize, cellData.textSize, 1f);
				iconText.GetComponent<TextfieldAutosizer>().SetScale(new Vector3(cellData.textSize, cellData.textSize, 1f));
				iconTextDisabled.GetComponent<TextfieldAutosizer>().SetScale(new Vector3(cellData.textSize, cellData.textSize, 1f));
			}
			iconTextDisabled.text = cellData.textLocalization.Localize();
		}
	}

	public void SetButtonState(bool isEnabled)
	{
		if ((bool)enabledGameObject)
		{
			enabledGameObject.SetActive(isEnabled);
		}
		if ((bool)disabledGameObject)
		{
			disabledGameObject.SetActive(!isEnabled);
		}
		if (!isEnabled)
		{
			GameObject[] array = notificationGameObjects;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(isEnabled);
			}
		}
		if ((bool)regularButtonSprite)
		{
			regularButtonSprite.gameObject.SetActive(isEnabled);
		}
		if ((bool)lockedButtonSprite)
		{
			lockedButtonSprite.gameObject.SetActive(!isEnabled);
		}
	}
}
