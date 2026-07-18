using System.Collections;
using UnityEngine;

public class UnitItemPopUpView : UnitItemView
{
	[SerializeField]
	private tk2dSprite userPopUpBackground;

	[SerializeField]
	private GameObject unitEnvironment;

	[SerializeField]
	private GameObject addButton;

	[SerializeField]
	private GameObject removeButton;

	[SerializeField]
	private GameObject overlayObject;

	[SerializeField]
	private PriceLabelController priceLabel;

	private void Start()
	{
		SetOverlayState(false);
	}

	public void ConfigurePopUp(UserUnit userUnit)
	{
		ConfigureUnitView(userUnit);
		if ((bool)unitEnvironment)
		{
			unitEnvironment.SetActive(true);
		}
	}

	public void DeactivateButtons()
	{
		if ((bool)addButton)
		{
			addButton.SetActive(false);
		}
		if ((bool)removeButton)
		{
			removeButton.SetActive(false);
		}
	}

	public void SetPopUpState(bool unitState)
	{
		if ((bool)addButton)
		{
			addButton.SetActive(!unitState);
		}
		if ((bool)removeButton)
		{
			removeButton.SetActive(unitState);
		}
		SetState(unitState);
		SetBackgroundState(unitState);
	}

	public override void ConfigureUnitView(UserUnit unit)
	{
		base.ConfigureUnitView(unit);
		if ((bool)priceLabel)
		{
			priceLabel.ConfigurePriceLabel(unit.GetScrap());
		}
	}

	public override void SetUnitLevel(int level)
	{
		if ((bool)levelLabel)
		{
			levelLabel.text = "[" + level + "]";
		}
	}

	public void SetOverlayState(bool state)
	{
		if ((bool)overlayObject)
		{
			overlayObject.SetActive(state);
		}
	}

	public void SetBackgroundState(bool state)
	{
		if ((bool)userPopUpBackground)
		{
			string sprite = ((!state) ? "btn_square_196" : "btn_square_196_active");
			userPopUpBackground.SetSprite(sprite);
		}
	}

	public override IEnumerator SetAssetBundle(int assetBundleID)
	{
		if ((bool)unitProxy)
		{
			yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", assetBundleID));
		}
		tk2dSprite tankSprite = unitProxy.GetComponentInChildren<tk2dSprite>();
		if ((bool)tankSprite)
		{
			tankSprite.SortingOrder = 14;
			tankSprite.gameObject.layer = unitProxy.gameObject.layer;
		}
	}
}
