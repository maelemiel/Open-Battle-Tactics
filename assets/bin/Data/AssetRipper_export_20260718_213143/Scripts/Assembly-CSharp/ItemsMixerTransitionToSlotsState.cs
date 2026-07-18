using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemsMixerTransitionToSlotsState : GachaPlinkoBaseState
{
	[SerializeField]
	private GameObject itemMixerBoard;

	[SerializeField]
	private GameObject instructionsBubble;

	[SerializeField]
	private GameObject bouncingSlotArrows;

	[SerializeField]
	private GameObject bubblePrices;

	[SerializeField]
	private GameObject prizesScrollableAreaGameObject;

	[SerializeField]
	private GameObject pricesScrollableAreaGameObject;

	public float initialPricesScrollableAreaPosition = -265f;

	private ScrollableAreaController pricesScrollableArea;

	public ItemsMixerItemSlotViewController[] itemsSlots;

	public ItemsMixerItemSlotViewController playerItemSlot;

	private bool rewardsAvailable;

	public override IEnumerator StartStateSequence()
	{
		prizesScrollableAreaGameObject.transform.TweenLocalXPosition(600f, 1.25f);
		instructionsBubble.transform.TweenLocalScale(0f, 1f);
		if (base.ItemsMixer.gachaResultsPayload.gachaRewards != null)
		{
			yield return StartCoroutine(SetupItemsSequenceCoroutine());
		}
		else
		{
			base.ItemsMixer.gachaResultsPayload.OnRewardsAvailable += delegate
			{
				rewardsAvailable = true;
			};
			while (!rewardsAvailable)
			{
				yield return 0;
			}
			yield return StartCoroutine(SetupItemsSequenceCoroutine());
		}
		yield return new WaitForSeconds(1f);
		itemMixerBoard.transform.TweenLocalYPosition(base.ItemsMixer.finalBoardYPosition, 1f);
		SetupPricesScrollableArea();
		PopulatePricesScrollableArea();
		pricesScrollableAreaGameObject.transform.TweenLocalXPosition(initialPricesScrollableAreaPosition, 1f);
		if ((bool)bubblePrices)
		{
			bubblePrices.transform.localScale = Vector3.zero;
			bubblePrices.SetActive(true);
			bubblePrices.transform.TweenLocalScale(1f, 1f);
		}
		yield return new WaitForSeconds(1f);
		base.ItemsMixer.SetState(GachaPlinkoStates.SELECT_SLOT);
	}

	private void PopulatePricesScrollableArea()
	{
		if (base.ItemsMixer.selectedPrize != null)
		{
			UserProfile playerProfile = UserProfile.player;
			List<GachaPlinkoPrizePriceDataModel> source = GachaPlinkoPrizePriceDataModel.GetAll().FindAll((GachaPlinkoPrizePriceDataModel x) => x.priceId == base.ItemsMixer.selectedPrize.priceId);
			source = source.OrderByDescending((GachaPlinkoPrizePriceDataModel x) => x.orderNumber).Reverse().ToList();
			ItemCollectionDataModel.Item item1;
			ItemCollectionDataModel.Item item2;
			source.Sort(delegate(GachaPlinkoPrizePriceDataModel x, GachaPlinkoPrizePriceDataModel y)
			{
				item1 = new ItemCollectionDataModel.Item((UserInventory.ItemType)x.itemType, x.itemId, x.amount);
				item2 = new ItemCollectionDataModel.Item((UserInventory.ItemType)y.itemType, y.itemId, y.amount);
				bool flag = playerProfile.CanAffordItem(item1);
				bool flag2 = playerProfile.CanAffordItem(item2);
				if (!flag && flag2)
				{
					return 1;
				}
				return (flag && !flag2) ? (-1) : (x.orderNumber - y.orderNumber);
			});
			pricesScrollableArea.InitializeWithData(source);
		}
		else
		{
			Log.Error("Trying to populate the Prize prices list with null data");
		}
	}

	private void SetupPricesScrollableArea()
	{
		if ((bool)pricesScrollableAreaGameObject)
		{
			pricesScrollableArea = pricesScrollableAreaGameObject.GetComponent<ScrollableAreaController>();
			if (!pricesScrollableArea)
			{
				Log.Error("[ItemsMixerScene] ScrollableAreaController not found", base.gameObject);
			}
		}
	}

	private IEnumerator SetupItemsSequenceCoroutine()
	{
		yield return StartCoroutine(SetPartsMixerItemSet(base.ItemsMixer.gachaResultsPayload.gachaRewards));
	}

	private IEnumerator SetPartsMixerItemSet(ItemCollectionDataModel itemSet)
	{
		if (itemSet.items.Count != itemsSlots.Length)
		{
			Log.Error("Input Items Set and Visual slots lengths are different");
			yield break;
		}
		for (int i = 0; i < itemSet.items.Count; i++)
		{
			yield return StartCoroutine(itemsSlots[i].Init(itemSet.items[i]));
		}
		for (int j = 0; j < itemSet.items.Count; j++)
		{
			yield return StartCoroutine(itemsSlots[j].ConfigureView(itemSet.items[j]));
		}
		if ((bool)playerItemSlot)
		{
			ItemCollectionDataModel.Item selectedItem = new ItemCollectionDataModel.Item((UserInventory.ItemType)base.ItemsMixer.selectedPrize.itemType, base.ItemsMixer.selectedPrize.itemId, 1);
			yield return StartCoroutine(playerItemSlot.Init(selectedItem));
			yield return StartCoroutine(playerItemSlot.ConfigureView(selectedItem));
		}
	}
}
