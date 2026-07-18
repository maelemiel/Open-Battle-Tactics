using UnityEngine;

public class PersonalPartsPopUpController : PopupController
{
	[SerializeField]
	private PersonalPartsSceneController personalPartsController;

	[SerializeField]
	private GameObject personalPartsButton;

	protected override void Start()
	{
		base.Start();
		if ((bool)personalPartsController)
		{
			personalPartsController.IsOpen = true;
			personalPartsController.OnPersonalPartsClosed += DestroyPopUp;
		}
		if ((bool)personalPartsButton && (bool)TopBarController.instance)
		{
			personalPartsButton.transform.position = TopBarController.instance.PersonalPartsButton.transform.position;
		}
	}

	public override void OnCloseButton()
	{
		if ((bool)personalPartsButton)
		{
			personalPartsButton.SetActive(false);
		}
		if ((bool)personalPartsController)
		{
			personalPartsController.IsOpen = false;
		}
	}

	private void OnGetPartsClicked()
	{
		if (SceneTransitionManager.CurrentSceneDM._scene != SceneTransitionManager.Scene.ShopItemsSuppliesScene)
		{
			Reporting.MenuNavigation("contracts");
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
		}
		personalPartsController.IsOpen = false;
	}

	private void DestroyPopUp()
	{
		PopupManager.DestroyPopup(model);
	}
}
