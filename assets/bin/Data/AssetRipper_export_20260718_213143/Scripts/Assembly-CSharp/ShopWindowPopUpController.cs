using UnityEngine;

public class ShopWindowPopUpController : PopupController
{
	[SerializeField]
	private ShopWindowController shopWindowController;

	[SerializeField]
	private GameObject shopButton;

	protected override void Start()
	{
		base.Start();
		SceneController.resumeCallbackEnable = false;
		OtherLevelsHelper.UsePlacement("open_store", "Store Launch", false);
		if ((bool)shopWindowController)
		{
			shopWindowController.IsOpen = true;
		}
		if ((bool)shopButton && (bool)TopBarController.instance)
		{
			shopButton.transform.position = TopBarController.instance.ShopButton.transform.position;
		}
	}

	public override void OnCloseButton()
	{
		if ((bool)shopButton)
		{
			shopButton.SetActive(false);
		}
		if ((bool)shopWindowController)
		{
			shopWindowController.OnShopClosed += DestroyPopUp;
			shopWindowController.IsOpen = false;
		}
	}

	private void DestroyPopUp()
	{
		SceneController.resumeCallbackEnable = true;
		PopupManager.DestroyPopup(model);
	}
}
