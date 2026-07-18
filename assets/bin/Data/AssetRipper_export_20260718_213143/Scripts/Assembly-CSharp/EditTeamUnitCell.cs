using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditTeamUnitCell : EditTeamBaseCell
{
	private Dictionary<int, string> unityRarityBanners = new Dictionary<int, string>
	{
		{ 1, "banner_bronze" },
		{ 2, "banner_silver" },
		{ 3, "banner_gold" },
		{ 4, "banner_purple" },
		{ 5, "banner_red" }
	};

	private Dictionary<int, int> bannerSizes = new Dictionary<int, int>
	{
		{ 1, 80 },
		{ 2, 105 },
		{ 3, 130 },
		{ 4, 145 },
		{ 5, 160 }
	};

	[SerializeField]
	private UnitInfoView unitInfoView;

	[SerializeField]
	private tk2dSlicedSprite rarityBanner;

	[SerializeField]
	private tk2dTextMesh unitRarity;

	[SerializeField]
	private List<Color> unitRarityColors;

	[SerializeField]
	private tk2dTextMesh dragUnitHere;

	[SerializeField]
	private tk2dBaseSprite cellBackground;

	private bool _isCooldownLocked;

	public UserUnit Unit
	{
		get
		{
			return base.DataObject as UserUnit;
		}
	}

	public override void ConfigureCellData()
	{
		if (Unit != null)
		{
			unitInfoView.gameObject.SetActive(true);
			unitInfoView.ConfigureUnitView(Unit.UnitDataModel, Unit.level);
			if ((bool)rarityBanner)
			{
				Vector2 dimensions = new Vector2(rarityBanner.dimensions.x, bannerSizes[Unit.Rarity]);
				rarityBanner.dimensions = dimensions;
				rarityBanner.SetSprite(unityRarityBanners[Unit.Rarity]);
			}
			if ((bool)cellBackground)
			{
				cellBackground.gameObject.SetActive(true);
			}
			if ((bool)dragUnitHere)
			{
				dragUnitHere.gameObject.SetActive(false);
			}
		}
		else
		{
			if ((bool)unitInfoView)
			{
				unitInfoView.gameObject.SetActive(false);
			}
			if ((bool)dragUnitHere)
			{
				dragUnitHere.gameObject.SetActive(true);
			}
			if ((bool)cellBackground)
			{
				cellBackground.gameObject.SetActive(false);
			}
		}
		UpdateTeamState();
		base.ConfigureCellData();
	}

	private IEnumerator FitUnitProxy(UnitInfoView uiv)
	{
		yield return StartCoroutine(uiv.unitProxy.WaitForAssetReady());
		uiv.UnitGameObject.FitWithinBounds(uiv.unitProxy.collider.bounds);
	}

	public void UpdateTeamState()
	{
		if (!(teamCellGroup != null))
		{
			return;
		}
		bool isCooldownLocked = _isCooldownLocked;
		_isCooldownLocked = UserProfile.player.teams[teamCellGroup.index].IsOnCooldown;
		if (isCooldownLocked == _isCooldownLocked)
		{
			return;
		}
		if (_isCooldownLocked)
		{
			if ((bool)base.Background)
			{
				SetBackgroundState(false);
			}
		}
		else if ((bool)base.Background)
		{
			SetBackgroundState(true);
		}
		base.DragItem.Locked = _isCooldownLocked;
	}

	private void Update()
	{
		if ((bool)dragUnitHere && dragUnitHere.gameObject.activeSelf && !string.IsNullOrEmpty(dragUnitHere.text))
		{
			dragUnitHere.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, 2f));
		}
	}
}
