using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlueprintsPopUpController : PopupController
{
	[SerializeField]
	private tk2dTextMesh popUpMessage;

	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private tk2dUIItem buildButton;

	[SerializeField]
	private tk2dUIItem partsButton;

	[SerializeField]
	private tk2dTextMesh toggleViewButtonLabelUp;

	[SerializeField]
	private tk2dTextMesh toggleViewButtonLabelDown;

	[SerializeField]
	private UpgradePartRequirementController[] partRequirementControllers;

	[SerializeField]
	private UnitInfoView unitInfoView;

	[SerializeField]
	private UnitAnimationController unitAnimationController;

	[SerializeField]
	private AnimateTextureOffset backgroundAnimation;

	private bool viewingMax;

	protected override void Start()
	{
		base.Start();
		UnitDataModel unitDataModel = (UnitDataModel)model.payload;
		UserPriceDataModel buildPrice = unitDataModel.GetBuildPrice(UserProfile.player.TimesBuiltUnit(unitDataModel.id));
		if (!UserProfile.player.CanAfford(buildPrice) && !unitDataModel.CanBuyDirect)
		{
			SetNotReadyToBuildState(unitDataModel.researchTime);
		}
		else
		{
			SetReadyToBuildState(unitDataModel.researchTime);
		}
		if ((bool)unitInfoView)
		{
			unitInfoView.ConfigureUnitView(unitDataModel, 1);
		}
		SetupPriceLabel(buildPrice);
		ToggleView();
	}

	private void SetupPriceLabel(UserPriceDataModel price)
	{
		int num = 0;
		for (int i = 0; i < partRequirementControllers.Length; i++)
		{
			if (i < price.items.Count)
			{
				partRequirementControllers[i].gameObject.SetActive(true);
				if (price.items[i].Part != null)
				{
					List<ItemCollectionDataModel.Item> list = new List<ItemCollectionDataModel.Item>();
					list.Add(price.items[i]);
					UserPriceDataModel priceDataModel = new UserPriceDataModel(list);
					partRequirementControllers[num].ConfigureWithPrice(priceDataModel, null, string.Empty, UserProfile.player.CanAffordItem(price.items[i]), i);
					partRequirementControllers[num].OnPartsCommitted = OnPartsCommitted;
					num++;
				}
			}
			else
			{
				partRequirementControllers[i].gameObject.SetActive(false);
				partRequirementControllers[i].GetComponent<tk2dUIItem>().enabled = false;
			}
		}
	}

	private void OnPartsCommitted(int index)
	{
		ItemCollectionDataModel.Item item = partRequirementControllers[index].priceData.items[0];
		UnitDataModel unitDataModel = (UnitDataModel)model.payload;
		PopupManager.ShowPopup(PopupDataModel.UpgradeUnitPartDetailsPopUp(item.Part, item.amount, index, partRequirementControllers[index].partialData, unitDataModel.id, 0, null, null));
	}

	private void SetNotReadyToBuildState(int researchTime)
	{
		buildButton.gameObject.SetActive(false);
		partsButton.gameObject.SetActive(true);
		popUpMessage.text = TimeFormats.GetTimeShortStringComplete(TimeUtility.ConvertSecondsToMiliseconds(researchTime)).Trim();
	}

	private void SetReadyToBuildState(int researchTime)
	{
		buildButton.gameObject.SetActive(true);
		partsButton.gameObject.SetActive(false);
		popUpMessage.text = TimeFormats.GetTimeShortStringComplete(TimeUtility.ConvertSecondsToMiliseconds(researchTime)).Trim();
	}

	private void ToggleView()
	{
		viewingMax = !viewingMax;
		SetUnitImage();
		SetToggleButtonText();
	}

	private void SetUnitImage()
	{
		if (!unitInfoView)
		{
			return;
		}
		bool flag = false;
		UnitDataModel unitDataModel = (UnitDataModel)model.payload;
		if (UserProfile.player.HasBuiltUnit(unitDataModel.id))
		{
			List<UserUnit> list = UserProfile.player.unitInventory.Values.Where((UserUnit x) => x.metadataId == unitDataModel.id && x.IsMaxLevel).ToList();
			flag = list.Count > 0;
		}
		if (viewingMax)
		{
			unitInfoView.ConfigureUnitView(unitDataModel);
		}
		else
		{
			unitInfoView.ConfigureUnitView(unitDataModel, 1);
		}
	}

	private void SetToggleButtonText()
	{
		if ((bool)toggleViewButtonLabelUp && (bool)toggleViewButtonLabelDown)
		{
			if (viewingMax)
			{
				toggleViewButtonLabelUp.text = "ui_buildpopup_viewlvl1".Localize("VIEW LVL 1");
				toggleViewButtonLabelDown.text = "ui_buildpopup_viewlvl1".Localize("VIEW LVL 1");
			}
			else
			{
				UnitDataModel unitDataModel = (UnitDataModel)model.payload;
				toggleViewButtonLabelUp.text = string.Format("ui_buildpopup_viewlvlnum".Localize("VIEW LVL {0}"), unitDataModel.MaxLevel);
				toggleViewButtonLabelDown.text = string.Format("ui_buildpopup_viewlvlnum".Localize("VIEW LVL {0}"), unitDataModel.MaxLevel);
			}
		}
	}

	private void BuildUnitButton()
	{
		UnitDataModel unitDataModel = (UnitDataModel)model.payload;
		UserProfile.player.TryStartResearch(UserResearcher.ResearchType.BuildTank, unitDataModel.id, UnitResearchStarted);
	}

	private void PartsButton()
	{
		PopupManager.DestroyPopup(model);
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
	}

	private void UnitResearchStarted()
	{
		AudioTrigger.BuildTank.Play();
		ClosePopUpButton();
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
