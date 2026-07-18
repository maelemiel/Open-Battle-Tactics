using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class ItemsMixerInProgressState : GachaPlinkoBaseState
{
	private const float DEFAULT_SCREEN_WIDTH = 960f;

	private const float DEFAULT_SCREEN_HEIGHT = 640f;

	private const int PLAYER_REWARD_SLOT_INDEX = 4;

	[SerializeField]
	private GameObject itemMixerBoard;

	[SerializeField]
	private GameObject pricesScrollableAreaGameObject;

	[SerializeField]
	private GameObject bubbleText;

	[SerializeField]
	private GameObject background;

	[SerializeField]
	private GameObject[] playerSlotsArrows;

	[SerializeField]
	private GameObject bouncersGameObject;

	[SerializeField]
	private FireworksAnimation jackpotFireworks;

	[SerializeField]
	private FireworksAnimation megaJackpotFireworks;

	private GameObject playerSlotChip;

	[SerializeField]
	private ItemsMixerTrajectories itemMixerTrajectories;

	public float originalSampleScale = 0.2f;

	public float initialItemsBoardScale = 0.5f;

	public float finalItemsBoardScale = 0.7f;

	public Color jackpotStarBurstColor = Color.white;

	public Color megaJackpotStarBurstColor = Color.white;

	private float finalItemsBoardRelativeScale;

	private float scaleTrajectoryMultiplier = 1f;

	private Transform itemMixerBoardTransform;

	private float defaultAspectRatio;

	private float currentAspectRatio;

	private float aspectRatiosFactor;

	private float verticalDelta;

	private Tweener starbustTweener;

	public ItemsMixerItemSlotViewController[] itemsSlots;

	private void Awake()
	{
		defaultAspectRatio = 1.5f;
		currentAspectRatio = (float)Screen.width / (float)Screen.height;
		aspectRatiosFactor = currentAspectRatio / defaultAspectRatio;
		finalItemsBoardRelativeScale = aspectRatiosFactor * finalItemsBoardScale;
	}

	public override IEnumerator StartStateSequence()
	{
		float finalBoardHeight = (float)base.ItemsMixer.itemsMixerHeight * (finalItemsBoardScale / initialItemsBoardScale);
		verticalDelta = finalBoardHeight * (0.575f * aspectRatiosFactor);
		if ((bool)bubbleText)
		{
			bubbleText.gameObject.SetActive(false);
		}
		if ((bool)pricesScrollableAreaGameObject)
		{
			pricesScrollableAreaGameObject.transform.TweenLocalXPosition(600f, 1f);
		}
		if ((bool)itemMixerBoard)
		{
			itemMixerBoard.transform.TweenLocalXPosition(base.ItemsMixer.finalBoardXPosition, 1f);
			scaleTrajectoryMultiplier = finalItemsBoardRelativeScale / originalSampleScale;
			itemMixerBoardTransform = itemMixerBoard.transform;
		}
		else
		{
			Log.Error("[ItemsMixerInProgressState] ItemsMixerBoardController not found", base.gameObject);
		}
		if ((bool)base.ItemsMixer.playerSlotChip)
		{
			playerSlotChip = base.ItemsMixer.playerSlotChip;
		}
		else
		{
			Log.Error("[ItemsMixerInProgressState] Game chip GameObject, not found", base.gameObject);
		}
		if (playerSlotsArrows != null)
		{
			GameObject[] array = playerSlotsArrows;
			foreach (GameObject arrow in array)
			{
				arrow.SetActive(false);
			}
		}
		if ((bool)bouncersGameObject)
		{
			bouncersGameObject.SetActive(true);
		}
		base.ItemsMixer.ShowText("ui_items_drop_it_game".Localize("DROP IT!"));
		Vector3 newPosition = new Vector3(y: 0f - verticalDelta, x: base.ItemsMixer.finalBoardXPosition, z: 0f);
		AudioTrigger.ChaChing.Play();
		itemMixerBoard.transform.TweenLocalPosition(newPosition, 1f);
		itemMixerBoard.transform.TweenLocalScale(finalItemsBoardRelativeScale, 1f);
		yield return new WaitForSeconds(1f);
		if (base.ItemsMixer.HasResults())
		{
			SimulateItemsMixerLogic();
			yield break;
		}
		base.ItemsMixer.OnResultAvailable += delegate
		{
			SimulateItemsMixerLogic();
		};
	}

	private void SimulateItemsMixerLogic()
	{
		List<Vector3> trajectory = itemMixerTrajectories.GetTrajectory(base.ItemsMixer.SelectedPlayerSlot.slotIndex, base.ItemsMixer.ItemsMixerSlotResult);
		StartCoroutine(SimulateMovement(trajectory));
		itemMixerBoard.transform.TweenLocalYPosition(base.ItemsMixer.initialBoardYPosition, (float)trajectory.Count * 0.017f, EaseType.Linear);
		if ((bool)background)
		{
			background.transform.TweenLocalYPosition(base.ItemsMixer.finalBackgroundYPosition, (float)trajectory.Count * 0.017f, EaseType.Linear);
		}
	}

	private IEnumerator SimulateMovement(List<Vector3> positions)
	{
		if ((bool)playerSlotChip)
		{
			Transform chipTransform = playerSlotChip.transform;
			Vector3 nextPosition = Vector2.zero;
			float nextRotation = 0f;
			for (int i = 0; i < positions.Count; i++)
			{
				nextRotation = positions[i].z;
				chipTransform.rotation = Quaternion.Euler(0f, 0f, nextRotation);
				nextPosition = positions[i] * scaleTrajectoryMultiplier + itemMixerBoardTransform.position;
				playerSlotChip.transform.position = new Vector3(nextPosition.x, nextPosition.y, itemMixerBoardTransform.position.z);
				yield return new WaitForFixedUpdate();
			}
			EndMovement();
			yield return new WaitForSeconds(2f);
			OnFinishSimulate();
		}
	}

	private void EndMovement()
	{
		base.ItemsMixer.ShowText("ui_items_mixer_finish_game".Localize("FINISH!"));
		int itemsMixerSlotResult = base.ItemsMixer.ItemsMixerSlotResult;
		ItemsMixerItemSlotViewController itemsMixerItemSlotViewController = itemsSlots[itemsMixerSlotResult];
		bool flag = itemsMixerItemSlotViewController == itemsSlots[4];
		AudioTrigger audioName = ((!flag) ? AudioTrigger.MiniJackpot : AudioTrigger.MegaJackpot);
		audioName.Play();
		EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.GACHA_STARBUST, itemsMixerItemSlotViewController.transform.position, itemsMixerItemSlotViewController.transform);
		effectInstance.transform.localScale = Vector3.zero;
		starbustTweener = effectInstance.transform.TweenLocalScale(1f, 0.5f);
		Color color = ((!flag) ? jackpotStarBurstColor : megaJackpotStarBurstColor);
		tk2dBaseSprite componentInChildren = effectInstance.GetComponentInChildren<tk2dBaseSprite>();
		if ((bool)componentInChildren)
		{
			componentInChildren.color = color;
		}
		FireworksAnimation fireworksAnimation = ((!flag) ? jackpotFireworks : megaJackpotFireworks);
		if ((bool)fireworksAnimation)
		{
			fireworksAnimation.PlayEffect();
		}
		AudioTrigger.Fireworks_2.Play();
		playerSlotChip.transform.TweenLocalScale(0f, 1f);
		for (int i = 0; i < itemsSlots.Length; i++)
		{
			if (i != itemsMixerSlotResult)
			{
				itemsSlots[i].transform.TweenLocalScale(0f, 1f);
			}
		}
		if (!flag && (bool)base.ItemsMixer.playerRewardCell)
		{
			base.ItemsMixer.playerRewardCell.transform.TweenLocalScale(0f, 1f);
		}
	}

	private void OnDestroy()
	{
		if (starbustTweener != null)
		{
			starbustTweener.Kill();
		}
	}

	private void OnFinishSimulate()
	{
		base.ItemsMixer.SetState(GachaPlinkoStates.FINISHED);
	}
}
