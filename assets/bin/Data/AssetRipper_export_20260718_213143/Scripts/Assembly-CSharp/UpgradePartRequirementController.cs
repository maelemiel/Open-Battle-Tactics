using System;
using UnityEngine;

public class UpgradePartRequirementController : MonoBehaviour
{
	[SerializeField]
	public PriceLabelController priceLabel;

	[SerializeField]
	public UnitPartialLevelDataModel partialData;

	[SerializeField]
	public UpgradePartRequirementView requirementView;

	[SerializeField]
	private string unitId;

	[SerializeField]
	private GameObject plusIcon;

	[SerializeField]
	private tk2dSprite outlinedSprite;

	[SerializeField]
	private tk2dTextMesh amountRequired;

	[SerializeField]
	private tk2dSprite upSprite;

	[SerializeField]
	private tk2dSprite downSprite;

	public bool hideRequirementOnComplete;

	public Action<int> OnPartsCommitted;

	private int index;

	public UserPriceDataModel priceData;

	public void ConfigureWithPrice(UserPriceDataModel priceDataModel, UnitPartialLevelDataModel partialLevel, string unitId, bool completed, int index)
	{
		priceData = priceDataModel;
		partialData = partialLevel;
		this.unitId = unitId;
		this.index = index;
		priceLabel.ConfigurePriceLabel(priceDataModel);
		if (upSprite == null || downSprite == null)
		{
			tk2dUIUpDownButton component = base.gameObject.GetComponent<tk2dUIUpDownButton>();
			upSprite = component.upStateGO.GetComponent<tk2dSprite>();
			downSprite = component.downStateGO.GetComponent<tk2dSprite>();
		}
		if (amountRequired != null)
		{
			amountRequired.gameObject.SetActive(true);
			amountRequired.text = UserProfile.player.inventory.GetParts(priceDataModel.items[0].Part.id) + "/" + priceDataModel.items[0].amount;
			if (UserProfile.player.CanAfford(priceDataModel))
			{
				amountRequired.color = Color.green;
			}
			else
			{
				amountRequired.color = Color.red;
			}
			if (hideRequirementOnComplete && completed)
			{
				amountRequired.gameObject.SetActive(false);
			}
		}
		if (completed)
		{
			SetSprites(true);
			if ((bool)plusIcon)
			{
				plusIcon.SetActive(false);
			}
			if (requirementView != null)
			{
				requirementView.DisableIncompleteItems();
				requirementView.StopAnimations();
			}
			priceLabel.gameObject.SetActive(true);
			if (outlinedSprite != null)
			{
				outlinedSprite.gameObject.SetActive(false);
			}
			return;
		}
		if (outlinedSprite != null)
		{
			priceLabel.gameObject.SetActive(false);
			outlinedSprite.SetSprite(ConvertToOutlinedName(priceDataModel));
			outlinedSprite.gameObject.SetActive(true);
		}
		if (!UserProfile.player.CanAffordItem(priceDataModel.items[0]))
		{
			SetSprites(false);
			if ((bool)plusIcon)
			{
				plusIcon.SetActive(true);
			}
			if (requirementView != null)
			{
				requirementView.partGlow.gameObject.SetActive(false);
			}
		}
		else
		{
			SetSprites(false);
			if ((bool)plusIcon)
			{
				plusIcon.SetActive(false);
			}
			if (requirementView != null)
			{
				requirementView.StartReadyAnimation();
			}
		}
		tk2dUIItem component2 = base.gameObject.GetComponent<tk2dUIItem>();
		if ((bool)component2)
		{
			component2.enabled = !completed;
		}
	}

	public void SetSprites(bool active)
	{
		if (active)
		{
			upSprite.SetSprite("LevelUp_Green_Up");
			downSprite.SetSprite("LevelUp_Green_Down");
		}
		else
		{
			upSprite.SetSprite("LevelUp_Blue_Up");
			downSprite.SetSprite("LevelUp_Blue_Down");
		}
	}

	private string ConvertToOutlinedName(UserPriceDataModel dataModel)
	{
		UnitPartTypesDataModel part = dataModel.items[0].Part;
		switch (int.Parse(part.Rarity.id))
		{
		case 1:
			return "Common";
		case 2:
			return "Uncommon";
		case 3:
			if (part.IsToken)
			{
				return "ChipRare";
			}
			switch (int.Parse(part.id))
			{
			case 99:
				return "ExclusiveRare";
			case 14:
				return "RareAir";
			default:
				return "RareTier";
			}
		case 4:
		{
			if (part.IsToken)
			{
				return "ChipSuperRare";
			}
			UnitDataModel firstAssociatedUnit = part.FirstAssociatedUnit;
			if (firstAssociatedUnit != null)
			{
				if (firstAssociatedUnit.type <= 4)
				{
					return "SRTier";
				}
				return "SREExclusive";
			}
			return "SRTier";
		}
		default:
			return "SRTier";
		}
	}

	public void SetCommitted(Action onComplete = null)
	{
		tk2dUIItem component = base.gameObject.GetComponent<tk2dUIItem>();
		if ((bool)component)
		{
			component.enabled = false;
		}
		if (requirementView != null)
		{
			requirementView.StopAnimations();
			StartCoroutine(requirementView.PlayUpgradePartAnimation(delegate
			{
				SetCommittedComponents(onComplete);
			}));
		}
		else
		{
			SetCommittedComponents(onComplete);
		}
	}

	private void SetCommittedComponents(Action onComplete)
	{
		if (outlinedSprite != null)
		{
			priceLabel.gameObject.SetActive(true);
			outlinedSprite.gameObject.SetActive(false);
		}
		if (onComplete != null)
		{
			onComplete();
		}
	}

	public void SetPartsCommitted()
	{
		if (OnPartsCommitted != null)
		{
			OnPartsCommitted(index);
		}
	}
}
