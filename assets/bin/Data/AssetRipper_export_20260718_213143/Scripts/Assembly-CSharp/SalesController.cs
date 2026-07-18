using System.Collections;
using UnityEngine;

public class SalesController : NewsController
{
	private const string OFFER_ATTACH_POINT_NAME = "offer";

	private const string DAILY_ATTACH_POINT_NAME = "daily";

	private const string CONTRACT_REGULAR_SPRITENAME = "BG_gacha_02";

	private const string CONTRACT_RARE_SPRITENAME = "BG_gacha_01";

	private const string CONTRACT_EXCLUSIVE_SPRITENAME = "BG_gacha_03";

	[SerializeField]
	private tk2dTextMesh _title;

	[SerializeField]
	private tk2dTextMesh _dealText;

	[SerializeField]
	private UnitProxy _unitProxy;

	[SerializeField]
	private tk2dSlicedSprite _spriteBackground;

	[SerializeField]
	private tk2dSlicedSprite _spriteShineBackground;

	[SerializeField]
	private tk2dSlicedSprite _spritePriceBar;

	[SerializeField]
	private tk2dSlicedSprite _shine;

	[SerializeField]
	private tk2dBaseSprite[] _screws;

	[SerializeField]
	private tk2dTextMesh offerLabel;

	[SerializeField]
	private PrefabProxy _gachaPrizeSprite;

	[SerializeField]
	private tk2dTextMesh _gachaTitle;

	[SerializeField]
	private tk2dTextMesh _gachaDescription;

	[SerializeField]
	private PriceLabelController _gachaPriceLabel;

	[SerializeField]
	private EnterFromController[] _movementGO;

	private GachaPoolsDataModel _prizeGachaDM;

	private BasicWeaponController _weaponController;

	private float _backgroundWidth;

	private float _ratio;

	public static bool CanShow(NewsDataModel newsDM)
	{
		GachaPoolsDataModel single = GachaPoolsDataModel.GetSingle(newsDM.gachaId);
		if (single == null)
		{
			return false;
		}
		if (single.DateStartTimeStamp > TimeUtility.ServerTs || TimeUtility.ServerTs > single.DateEndTimeStamp)
		{
			return false;
		}
		return true;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	protected override void Awake()
	{
		base.Awake();
		offerLabel.gameObject.SetActive(true);
		_backgroundWidth = _spriteBackground.dimensions.x;
	}

	public override IEnumerator Init(NewsDataModel newsDM = null)
	{
		yield return StartCoroutine(base.Init(newsDM));
		if (newsDM == null)
		{
			yield break;
		}
		_prizeGachaDM = GachaPoolsDataModel.GetSingle(newsDM.gachaId);
		_ratio = (float)_prizeGachaDM.size / 100f;
		AdjustUIElement();
		if ((bool)_title)
		{
			_title.text = newsDM.title.Localize();
			_title.Commit();
		}
		if ((bool)_dealText)
		{
			_dealText.text = newsDM.textDescription1.Localize();
			_dealText.Commit();
		}
		StartCoroutine(InitPrizeGacha(_prizeGachaDM));
		UnitDataModel unit = UnitDataModel.GetSingle(newsDM.unitId);
		yield return StartCoroutine(_unitProxy.ChangeAssetCoroutine("Prefab.prefab", unit.Levels[0].assetBundleId));
		if (_unitProxy.AssetReady)
		{
			_weaponController = _unitProxy.GetComponent<BasicWeaponController>();
			if (_weaponController != null)
			{
				_weaponController.Init(unit.rarity, _unitProxy.Prefab.GetComponent<tk2dSprite>().CurrentSprite);
			}
		}
	}

	private IEnumerator InitPrizeGacha(GachaPoolsDataModel prizeGachaDM)
	{
		if ((bool)_gachaTitle)
		{
			_gachaTitle.text = prizeGachaDM.Name;
		}
		if ((bool)_gachaDescription)
		{
			_gachaDescription.text = prizeGachaDM.Description;
		}
		if ((bool)_gachaPriceLabel)
		{
			_gachaPriceLabel.ConfigurePriceLabel(prizeGachaDM.GetPrice());
		}
		yield return StartCoroutine(_gachaPrizeSprite.ChangeAssetCoroutine(prizeGachaDM.AssetLinkage));
		while (!_gachaPrizeSprite.AssetReady)
		{
			yield return 0;
		}
		tk2dSprite gachaSprite = _gachaPrizeSprite.Prefab.GetComponentInChildren<tk2dSprite>();
		if ((bool)gachaSprite)
		{
			tk2dSpriteDefinition.AttachPoint setupAttachPoint = gachaSprite.GetAttachPointByName("offer");
			if (setupAttachPoint != null && (bool)offerLabel)
			{
				offerLabel.gameObject.SetActive(true);
				offerLabel.transform.localPosition = setupAttachPoint.position;
			}
			setupAttachPoint = gachaSprite.GetAttachPointByName("daily");
			ConfigureBackground(setupAttachPoint != null);
		}
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.StepUpGacha, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ShopItemsSuppliesScene.ToString());
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
	}

	public override void BeforeMovingInAction()
	{
		base.BeforeMovingInAction();
	}

	public override IEnumerator AfterMovingInAction()
	{
		yield return StartCoroutine(base.AfterMovingInAction());
		EnterFromController[] movementGO = _movementGO;
		foreach (EnterFromController enterFC in movementGO)
		{
			yield return StartCoroutine(enterFC.Init());
		}
		StartCoroutine(ShootCoroutine());
	}

	private IEnumerator ShootCoroutine()
	{
		while (true)
		{
			if (_weaponController != null && _unitProxy.Prefab != null)
			{
				yield return StartCoroutine(_weaponController.FiringAnimation(0, _unitProxy.Prefab.transform, true));
			}
			yield return new WaitForSeconds(1.5f);
		}
	}

	private void AdjustUIElement()
	{
		Vector2 dimensions = _spriteBackground.dimensions;
		dimensions.x += dimensions.x * _ratio;
		_spriteBackground.dimensions = dimensions;
		_spriteShineBackground.dimensions = dimensions + new Vector2(40f, 40f);
		_spritePriceBar.dimensions = new Vector2(dimensions.x - 18f, _spritePriceBar.dimensions.y);
		_shine.dimensions = new Vector2(dimensions.x, _shine.dimensions.y);
		int num = 40;
		float num2 = dimensions.x / 2f - (float)num;
		float xPosition = num2 * -1f;
		_screws[0].transform.SetLocalXPosition(xPosition);
		_screws[1].transform.SetLocalXPosition(num2);
	}

	private void ConfigureBackground(bool isExclusive)
	{
		if ((bool)_spriteBackground)
		{
			if (isExclusive)
			{
				_spriteBackground.SetSprite("BG_gacha_03");
				return;
			}
			bool flag = IsPremiumGacha(_prizeGachaDM.GachaType);
			_spriteBackground.SetSprite((!flag) ? "BG_gacha_02" : "BG_gacha_01");
		}
	}

	private bool IsPremiumGacha(GachaTypes gachaType)
	{
		return gachaType == GachaTypes.EVENT || gachaType == GachaTypes.PREMIUM || gachaType == GachaTypes.STEPGACHA || gachaType == GachaTypes.UNLOCKBYTIER;
	}
}
