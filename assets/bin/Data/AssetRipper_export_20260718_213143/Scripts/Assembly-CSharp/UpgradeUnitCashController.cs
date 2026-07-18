using System;
using System.Collections;
using UnityEngine;

public class UpgradeUnitCashController : MonoBehaviour, IUpgradeUnitPartsContoller
{
	[SerializeField]
	private Transform unitAnchor;

	[SerializeField]
	private Transform centerButtonAnchor;

	[SerializeField]
	private PromoteUnitController promotionController;

	[SerializeField]
	private PromoteUnitMaxController promotionControllerMax;

	[SerializeField]
	private PriceLabelController promoteSinglePriceLabelController;

	[SerializeField]
	private PriceLabelController promoteFullPriceLabelController;

	[SerializeField]
	private tk2dUIItem promoteSingleButton;

	[SerializeField]
	private tk2dUIItem promoteFullButton;

	[SerializeField]
	private tk2dBaseSprite promoteSingleButtonSpriteDisabled;

	[SerializeField]
	private tk2dBaseSprite promoteFullButtonSpriteDisabled;

	[SerializeField]
	private tk2dTextMesh promoteSingleText;

	[SerializeField]
	private tk2dTextMesh promoteFullText;

	private UserUnit localUserUnit;

	private bool promotingUnit;

	private Action<UnitDataModel, int, int> unitViewChange;

	private Action<bool> scrapButtonUpdate;

	private Action closeScreen;

	private Action upgradeStart;

	public void SetupScreen(UserUnit localUserUnit, Action upgradeStart, Action closeScreen, Action<bool> scrapButtonUpdate, Action<UnitDataModel, int, int> unitViewChange)
	{
		this.closeScreen = closeScreen;
		this.upgradeStart = upgradeStart;
		this.localUserUnit = localUserUnit;
		this.unitViewChange = unitViewChange;
		this.scrapButtonUpdate = scrapButtonUpdate;
		if (localUserUnit.GetMaxCashLevel() == 1)
		{
			promoteFullButton.gameObject.SetActive(false);
		}
	}

	public void ToggleScreen(bool toggle)
	{
		if (toggle)
		{
			base.gameObject.SetActive(true);
			UpdatePriceLabel();
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void UpdatePriceLabel()
	{
		UserPriceDataModel upgradePrice = localUserUnit.GetUpgradePrice();
		promoteSinglePriceLabelController.ConfigurePriceLabel(upgradePrice);
		UserPriceDataModel maxUpgradePrice = localUserUnit.GetMaxUpgradePrice();
		promoteFullPriceLabelController.ConfigurePriceLabel(maxUpgradePrice);
	}

	private void UpdatePromotePopUpState(bool updateButton = true)
	{
		if (unitViewChange != null)
		{
			unitViewChange(localUserUnit.UnitDataModel, localUserUnit.level, localUserUnit.partialLevel);
		}
		if (updateButton)
		{
			SetPromoteButtonState(true);
		}
	}

	private void PromoteUnitMaxButton()
	{
		promotingUnit = true;
		if (upgradeStart != null)
		{
			upgradeStart();
		}
		SetPromoteButtonState(false);
		if (scrapButtonUpdate != null)
		{
			scrapButtonUpdate(false);
		}
		promotionControllerMax.PromoteUnit(localUserUnit, UnitPromoted, UnitPromotedCancelled);
	}

	private void PromoteUnitButton()
	{
		promotingUnit = true;
		if (upgradeStart != null)
		{
			upgradeStart();
		}
		SetPromoteButtonState(false);
		if (scrapButtonUpdate != null)
		{
			scrapButtonUpdate(false);
		}
		promotionController.PromoteUnit(localUserUnit, UnitPromoted, UnitPromotedCancelled);
	}

	public void SetPromoteButtonState(bool state)
	{
		if ((bool)promoteSingleButton)
		{
			promoteSingleButton.enabled = state;
			promoteFullButton.enabled = state;
			SetPromoteButtonStateView(state);
		}
	}

	private void SetPromoteButtonStateView(bool state)
	{
		if ((bool)promoteSingleButtonSpriteDisabled)
		{
			promoteSingleButtonSpriteDisabled.gameObject.SetActive(!state);
			promoteFullButtonSpriteDisabled.gameObject.SetActive(!state);
		}
	}

	private void UnitPromoted()
	{
		if (!localUserUnit.IsMaxLevel && localUserUnit.GetMaxCashLevel() == localUserUnit.level)
		{
			StartCoroutine(AnnouncerController.DialogTrigger("AfterCashLvlMaxWithParts"));
		}
		UpdatePriceLabel();
		UpdatePromotePopUpState(false);
	}

	private void UnitPromotedCancelled(bool forceClose)
	{
		if (forceClose)
		{
			if (closeScreen != null)
			{
				closeScreen();
			}
			return;
		}
		if (scrapButtonUpdate != null)
		{
			scrapButtonUpdate(true);
		}
		UpdatePromotePopUpState();
	}

	public bool IsPromoting()
	{
		return promotingUnit;
	}

	public Transform GetUnitAnchor()
	{
		return unitAnchor;
	}

	public void OnPartsCommittedComplete(object input)
	{
	}

	public IEnumerator AnimateControllerIn()
	{
		yield break;
	}

	public IEnumerator AnimateControllerOut()
	{
		promoteSinglePriceLabelController.gameObject.SetActive(false);
		promoteFullPriceLabelController.gameObject.SetActive(false);
		promoteSingleText.text = string.Empty;
		promoteFullText.text = string.Empty;
		Transform leftButton = promoteSingleButton.transform;
		Transform rightButton = promoteFullButton.transform;
		Vector3 leftStart = leftButton.localPosition;
		Vector3 rightStart = rightButton.localPosition;
		AnimationCurve easeInOut = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		float time;
		for (time = 1f; time > 0f; time -= Time.deltaTime)
		{
			time -= Time.deltaTime;
			leftButton.localPosition = Vector3.Lerp(leftStart, centerButtonAnchor.localPosition, easeInOut.Evaluate(1f - time));
			rightButton.localPosition = Vector3.Lerp(rightStart, centerButtonAnchor.localPosition, easeInOut.Evaluate(1f - time));
			yield return new WaitForEndOfFrame();
		}
		leftButton.localPosition = leftStart;
		rightButton.localPosition = rightStart;
	}
}
