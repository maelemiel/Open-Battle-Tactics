using Holoville.HOTween;
using UnityEngine;

public class UnitItemPromotePopUpController : MonoBehaviour
{
	private const float SHOW_POPUP_OFFSET = 300f;

	private const float SHOW_POPUP_TIME = 0.25f;

	private const float HIDE_POPUP_TIME = 0.25f;

	private Vector3 initialPosition = new Vector3(0f, 5f, 1f);

	private Vector3 finalPosition = new Vector3(300f, 5f, 1f);

	public PriceLabelController priceLabelController;

	private bool state;

	public void ConfigurePopUp(UserPriceDataModel priceDataModel)
	{
		if ((bool)priceLabelController)
		{
			priceLabelController.ConfigurePriceLabel(priceDataModel);
		}
	}

	public void ResetPopUp()
	{
		SetPopUpInitialPosition();
		state = false;
	}

	public void SetPopUpInitialPosition()
	{
		base.transform.localPosition = initialPosition;
	}

	public void SetPopUpFinalPosition()
	{
		base.transform.localPosition = finalPosition;
	}

	public void ShowPopUp()
	{
		if (!state)
		{
			state = true;
			SetPopUpInitialPosition();
			Sequence sequence = new Sequence();
			sequence.Append(HOTween.To(base.gameObject.transform, 0.25f, new TweenParms().Prop("localPosition", new Vector3(base.gameObject.transform.localPosition.x + 300f, base.gameObject.transform.localPosition.y, base.gameObject.transform.localPosition.z)).Ease(EaseType.Linear)));
			sequence.Play();
		}
	}

	public void HidePopUp()
	{
		if (state)
		{
			state = false;
			SetPopUpFinalPosition();
			Sequence sequence = new Sequence();
			sequence.Append(HOTween.To(base.gameObject.transform, 0.25f, new TweenParms().Prop("localPosition", new Vector3(base.gameObject.transform.localPosition.x - 300f, base.gameObject.transform.localPosition.y, base.gameObject.transform.localPosition.z)).Ease(EaseType.Linear)));
			sequence.Play();
		}
	}
}
