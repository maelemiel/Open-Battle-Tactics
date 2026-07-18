using System.Collections;
using UnityEngine;

public class SkipWaitPopupController : PopupController
{
	[SerializeField]
	private PriceLabelController skipPriceLabel;

	[SerializeField]
	private GameObject objectContainer;

	[SerializeField]
	private tk2dBaseSprite disabledButton;

	private UserPriceDataModel skipPrice;

	private long priceCalculationTime;

	private UserResearcher researcher;

	private bool topbarWasShowing;

	[SerializeField]
	private tk2dTextMesh rightDownLabel;

	[SerializeField]
	private tk2dTextMesh leftDownLabel;

	protected override void Start()
	{
		base.Start();
		topbarWasShowing = TopBarController.instance.Visible;
		if (!topbarWasShowing)
		{
			TopBarController.instance.ShowHomeButton = false;
			TopBarController.instance.Visible = true;
		}
		if (model.viewObject != null)
		{
			objectContainer.transform.MakeChild(model.viewObject.transform);
			objectContainer.FitWithinBounds(objectContainer.collider.bounds);
			PrefabProxy component = model.viewObject.GetComponent<PrefabProxy>();
			if (component != null)
			{
				component.FitWithinBounds(objectContainer.collider.bounds);
			}
		}
		if ((bool)_title)
		{
			_title.text = model.title;
		}
		if ((bool)_message)
		{
			_message.text = model.message;
		}
		if ((bool)_rightLabel && (bool)rightDownLabel)
		{
			rightDownLabel.text = _rightLabel.text;
		}
		if ((bool)_leftLabel && (bool)leftDownLabel)
		{
			leftDownLabel.text = _leftLabel.text;
		}
		priceCalculationTime = TimeManager.ServerTime;
		researcher = model.payload as UserResearcher;
		UpdatePrice();
		StartCoroutine(UpdateLoop());
	}

	private IEnumerator UpdateLoop()
	{
		while (!researcher.CanClaim)
		{
			yield return new WaitForSeconds(1f);
		}
		OnCloseButton();
	}

	private void UpdatePrice()
	{
		priceCalculationTime = TimeManager.ServerTime;
		skipPrice = researcher.GetHurryCost(priceCalculationTime);
		skipPriceLabel.ConfigurePriceLabel(skipPrice);
	}

	private void OnClickYes()
	{
		if (!researcher.CanClaim)
		{
			if (UserProfile.player.CanAfford(skipPrice))
			{
				Singleton<SessionManager>.instance.SkipResearch(researcher, priceCalculationTime);
				if (model.rightAction != null)
				{
					model.rightAction();
				}
			}
			else
			{
				PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
				{
					PopupManager.DestroyAllPopups();
					TopBarController.instance.LoadShop();
				}));
			}
		}
		Close();
	}

	private void OnClickNo()
	{
		OnCloseButton();
	}

	public override void Close()
	{
		base.Close();
		TopBarController.instance.Visible = topbarWasShowing;
	}
}
