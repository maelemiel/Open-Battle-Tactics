using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class ItemsMixerIntroState : GachaPlinkoBaseState
{
	[SerializeField]
	private GameObject itemMixerBoard;

	[SerializeField]
	private GameObject bouncingArrow;

	[SerializeField]
	private GameObject prizesScrollableAreaGameObject;

	[SerializeField]
	private GameObject pricesScrollableAreaGameObject;

	[SerializeField]
	private GameObject chipSlotsGameObject;

	[SerializeField]
	private ObjectShaker itemsMixerShaker;

	[SerializeField]
	private tk2dBaseSprite itemsMixerLogo;

	private float initialItemsMixerLogoYPosition;

	public override IEnumerator StartStateSequence()
	{
		SetScrollableAreasOutOfScreen();
		if ((bool)itemsMixerLogo)
		{
			initialItemsMixerLogoYPosition = itemsMixerLogo.transform.localPosition.y;
			itemsMixerLogo.transform.TweenLocalYPosition(0f, 0.35f);
		}
		yield return new WaitForSeconds(0.25f);
		if ((bool)itemMixerBoard)
		{
			itemMixerBoard.transform.TweenLocalYPosition(base.ItemsMixer.initialBoardYPosition, 1f, EaseType.EaseInExpo);
			yield return new WaitForSeconds(0.9f);
			itemsMixerLogo.transform.TweenLocalYPosition(initialItemsMixerLogoYPosition, 0.5f);
			yield return new WaitForSeconds(0.1f);
			if ((bool)itemsMixerShaker)
			{
				itemsMixerShaker.Shake(true);
			}
			AudioTrigger.CrateLand.Play();
			AudioTrigger.ItemsMixer_Music.PlayMusic();
			yield return new WaitForSeconds(0.25f);
			base.ItemsMixer.ShowText("ui_items_play_game".Localize("LET'S PLAY!"));
			yield return new WaitForSeconds(1f);
			itemMixerBoard.transform.TweenLocalXPosition(base.ItemsMixer.initialBoardXPosition, 1f);
		}
		if ((bool)chipSlotsGameObject)
		{
			chipSlotsGameObject.SetActive(false);
		}
		base.ItemsMixer.SetState(GachaPlinkoStates.SELECT_ITEM);
	}

	private void SetScrollableAreasOutOfScreen()
	{
		if ((bool)prizesScrollableAreaGameObject)
		{
			prizesScrollableAreaGameObject.transform.SetLocalXPosition(600f);
		}
		if ((bool)pricesScrollableAreaGameObject)
		{
			pricesScrollableAreaGameObject.transform.SetLocalXPosition(600f);
		}
	}
}
