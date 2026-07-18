using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class BlueprintsClaimTankPopUp : PopupController
{
	[SerializeField]
	private UnitInfoView unitInfoView;

	[SerializeField]
	private tk2dUIItem closeButton;

	[SerializeField]
	private PrefabProxy blueprintsPrefabProxy;

	[SerializeField]
	private GameObject unitRevealEffect;

	[SerializeField]
	private FireworksAnimation fireworks;

	[SerializeField]
	private tk2dTextMesh itemName;

	[SerializeField]
	private tk2dTextMesh itemRarity;

	[SerializeField]
	private PriceLabelController priceLabel;

	private Tweener tempTweener;

	private UnitDataModel unitDataModel;

	private UnitPartTypesDataModel partTypeDataModel;

	private Transform unitInfoViewTransform;

	private Transform priceLabelTransform;

	protected override void Start()
	{
		base.Start();
		unitInfoViewTransform = unitInfoView.transform;
		unitInfoViewTransform.localScale = Vector3.zero;
		priceLabelTransform = priceLabel.transform;
		priceLabelTransform.localScale = Vector3.one;
		Log.DebugTag(string.Concat("Type of ", model.payload.GetType(), " Vs ", typeof(UnitDataModel), " = ", model.payload.GetType() == typeof(UnitDataModel)), null, "BluePrintsClaim");
		if (model.payload.GetType() == typeof(UnitDataModel))
		{
			unitDataModel = (UnitDataModel)model.payload;
		}
		if (unitDataModel == null)
		{
			partTypeDataModel = (UnitPartTypesDataModel)model.payload;
		}
		tk2dUIManager.Instance.ForceRefresh();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.BootReady, delegate
		{
			OnBootReady();
		});
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void OnBootReady()
	{
	}

	private void Init()
	{
		if (unitDataModel != null)
		{
			StartCoroutine(ClaimUnitSequence());
		}
		else
		{
			StartCoroutine(ClaimPartSequence());
		}
	}

	private IEnumerator ClaimPartSequence()
	{
		if (partTypeDataModel == null)
		{
			yield break;
		}
		if ((bool)unitInfoView)
		{
			unitInfoView.gameObject.SetActive(false);
		}
		ItemCollectionDataModel t = new ItemCollectionDataModel(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, int.Parse(partTypeDataModel.id), 1));
		priceLabel.ConfigurePriceLabel(t);
		if ((bool)priceLabelTransform)
		{
			priceLabelTransform.localScale = Vector3.zero;
		}
		closeButton.gameObject.SetActive(false);
		unitRevealEffect.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		itemName.text = partTypeDataModel.Name;
		itemRarity.text = partTypeDataModel.Rarity.Name;
		tempTweener = SimpleTween.Start(0f, 1.5f, 1.5f, EaseType.EaseOutElastic, delegate(float val)
		{
			if ((bool)priceLabelTransform)
			{
				priceLabelTransform.localScale = Vector3.one * val;
			}
		});
		AudioTrigger.CrowdCheering.Play();
		yield return new WaitForSeconds(0.25f);
		blueprintsPrefabProxy.gameObject.SetActive(false);
		fireworks.PlayEffect();
		yield return new WaitForSeconds(1f);
		closeButton.gameObject.SetActive(true);
	}

	private IEnumerator ClaimUnitSequence()
	{
		if (unitDataModel == null)
		{
			yield break;
		}
		closeButton.gameObject.SetActive(false);
		yield return StartCoroutine(blueprintsPrefabProxy.ChangeAssetCoroutine(unitDataModel.BlueprintLinkage));
		unitRevealEffect.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.5f);
		if ((bool)unitInfoView)
		{
			unitInfoView.ConfigureUnitView(unitDataModel, 1);
			tempTweener = SimpleTween.Start(0f, 1.5f, 1.5f, EaseType.EaseOutElastic, delegate(float val)
			{
				if ((bool)unitInfoViewTransform)
				{
					unitInfoViewTransform.localScale = Vector3.one * val;
				}
			});
		}
		AudioTrigger.CrowdCheering.Play();
		yield return new WaitForSeconds(0.25f);
		blueprintsPrefabProxy.gameObject.SetActive(false);
		fireworks.PlayEffect();
		yield return new WaitForSeconds(1f);
		closeButton.gameObject.SetActive(true);
	}

	public override void OnBackButtonPressed()
	{
		if (allowBackButton && closeButton.gameObject.activeSelf)
		{
			OnCloseButton();
		}
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
		if (tempTweener != null)
		{
			tempTweener.Kill();
			tempTweener = null;
		}
	}

	private void ClosePopUpButton()
	{
		PopupManager.DestroyPopup(model);
	}
}
