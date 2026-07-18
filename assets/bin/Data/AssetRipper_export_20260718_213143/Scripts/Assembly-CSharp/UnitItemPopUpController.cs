using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class UnitItemPopUpController : MonoBehaviour
{
	private const float SHOW_POPUP_OFFSET = 530f;

	private const float SHOW_POPUP_TIME = 0.2f;

	private const float HIDE_POPUP_TIME = 0.2f;

	private Vector3 initialPosition = new Vector3(0f, 5f, -2f);

	private Vector3 finalPosition = new Vector3(530f, 5f, -2f);

	[SerializeField]
	private UnitItemPopUpView popUpView;

	[SerializeField]
	private UnitItemPromotePopUpController secondaryPopUp;

	[SerializeField]
	private tk2dUIItem promoteButton;

	[SerializeField]
	private tk2dUIItem generalButton;

	private UnitItemController currentUnitItemController;

	private UnitSelectHUDController hudController;

	public UserUnit unit;

	public void Init(UnitSelectHUDController hudController)
	{
		this.hudController = hudController;
	}

	public void ConfigurePopUp(UnitItemController unitItemController, bool unitState, bool addRemoveButtonEnabled = true)
	{
		currentUnitItemController = unitItemController;
		unit = unitItemController.unit;
		if ((bool)popUpView)
		{
			popUpView.ConfigurePopUp(unitItemController.unit);
			popUpView.SetPopUpState(unitState);
		}
		if ((bool)generalButton)
		{
			generalButton.enabled = true;
		}
		UpdatePromoteButton();
		if (!addRemoveButtonEnabled)
		{
			popUpView.DeactivateButtons();
		}
		if ((bool)secondaryPopUp)
		{
			secondaryPopUp.ConfigurePopUp(unitItemController.unit.GetUpgradePrice());
		}
	}

	public void SetPromoteButtonState(bool state)
	{
		if ((bool)promoteButton)
		{
			promoteButton.enabled = state;
			popUpView.SetPromoteButtonStateView(state);
		}
	}

	private void SetPromoteButtonMaxLevel()
	{
		if ((bool)promoteButton)
		{
			promoteButton.enabled = false;
			popUpView.SetPromoteButtonMaxLevel();
		}
	}

	public void ShowPopUp()
	{
		if ((bool)secondaryPopUp)
		{
			secondaryPopUp.ResetPopUp();
		}
		base.transform.localPosition = initialPosition;
		popUpView.SetOverlayState(true);
		Sequence sequence = new Sequence();
		sequence.Append(HOTween.To(popUpView.gameObject.transform, 0.2f, new TweenParms().Prop("localPosition", new Vector3(popUpView.gameObject.transform.localPosition.x + 530f, popUpView.gameObject.transform.localPosition.y, popUpView.gameObject.transform.localPosition.z)).Ease(EaseType.Linear)));
		sequence.Play();
	}

	public void ShowSecondaryPopUp()
	{
		if ((bool)secondaryPopUp)
		{
			secondaryPopUp.ShowPopUp();
		}
	}

	public void HidePopUp()
	{
		base.transform.localPosition = finalPosition;
		Sequence sequence = new Sequence();
		sequence.Append(HOTween.To(popUpView.gameObject.transform, 0.2f, new TweenParms().Prop("localPosition", new Vector3(popUpView.gameObject.transform.localPosition.x - 530f, popUpView.gameObject.transform.localPosition.y, popUpView.gameObject.transform.localPosition.z)).Ease(EaseType.Linear)));
		sequence.Play();
		if ((bool)secondaryPopUp)
		{
			secondaryPopUp.HidePopUp();
		}
		StartCoroutine(SetOverlayStateAfterSequence(sequence, false));
	}

	public void HideSecondaryPopUp()
	{
		if ((bool)secondaryPopUp)
		{
			secondaryPopUp.HidePopUp();
		}
	}

	private IEnumerator SetOverlayStateAfterSequence(Sequence tweenSequence, bool state)
	{
		while (!tweenSequence.isComplete)
		{
			yield return new WaitForEndOfFrame();
		}
		popUpView.SetOverlayState(state);
	}

	private void PopUpMainButtonPressed()
	{
		hudController.UnitSelectedOnPopUp(currentUnitItemController);
	}

	private void PopUpOpenSecondaryButtonPressed()
	{
		ShowSecondaryPopUp();
	}

	public void SecondaryPopUpConfirmPressed()
	{
		hudController.UnitPromotedOnHUD(currentUnitItemController);
		HideSecondaryPopUp();
	}

	public void UpdatePromoteButton()
	{
		if (unit.IsMaxLevel)
		{
			SetPromoteButtonMaxLevel();
		}
		else
		{
			SetPromoteButtonState(true);
		}
	}

	public void UpdatePopUpView()
	{
		if ((bool)popUpView)
		{
			popUpView.ConfigurePopUp(unit);
			UpdatePromoteButton();
		}
	}
}
