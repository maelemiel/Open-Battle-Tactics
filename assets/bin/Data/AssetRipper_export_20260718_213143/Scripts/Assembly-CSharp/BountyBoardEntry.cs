using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BountyBoardEntry : ScrollableCell
{
	private const string COLLECT_RAID_BOSS_BACKGROUND_COLOR = "BG_Edit_BorderHolder_BlacknGreen";

	private const string DISSMISS_RAID_BOSS_BACKGROUND_COLOR = "BG_Edit_BorderHolder_Grey";

	private const string FIGHT_RAID_BOSS_BACKGROUND_COLOR = "BG_Edit_BorderHolder_BlacknRed";

	[SerializeField]
	private List<GameObject> levelIcons;

	[SerializeField]
	private UnitInfoView unitInfoview;

	[SerializeField]
	private tk2dTextMesh teamName;

	[SerializeField]
	private tk2dTextMesh bossCurrentHealth;

	[SerializeField]
	private tk2dUIProgressBar healthBar;

	[SerializeField]
	private tk2dBaseSprite cellBackground;

	[SerializeField]
	private PrefabProxy ownerBadgeProxy;

	[SerializeField]
	private PrefabProxy ownerBadgeProxySolo;

	[SerializeField]
	private tk2dTextMesh ownerclubNameText;

	[SerializeField]
	private tk2dTextMesh ownerUserNameText;

	[SerializeField]
	private tk2dTextMesh ownerUserNameTextSolo;

	[SerializeField]
	private tk2dTextMesh yourDamageDealt;

	[SerializeField]
	private PrefabProxy mvpOwnerBadgeProxy;

	[SerializeField]
	private PrefabProxy mvpOwnerBadgeProxySolo;

	[SerializeField]
	private tk2dTextMesh mvpClubName;

	[SerializeField]
	private tk2dTextMesh mvpUserName;

	[SerializeField]
	private tk2dTextMesh mvpUserNameSolo;

	[SerializeField]
	private tk2dTextMesh damageDealt;

	[SerializeField]
	private GameObject battleButton;

	[SerializeField]
	private GameObject collectButton;

	[SerializeField]
	private GameObject discardButton;

	[SerializeField]
	private GameObject fightButton;

	[SerializeField]
	private GameObject searchBoss;

	[SerializeField]
	private GameObject mvpGameObject;

	[SerializeField]
	private GameObject playerGameObject;

	[SerializeField]
	private GameObject bossInfoGameObject;

	[SerializeField]
	private GameObject huntForABoss;

	[SerializeField]
	private GameObject regularBossFrame;

	[SerializeField]
	private GameObject maxLevelBossFrame;

	[SerializeField]
	private GameObject progressBarGameObject;

	[SerializeField]
	private GameObject timerGameObject;

	[SerializeField]
	private GameObject victoryLogo;

	[SerializeField]
	private GameObject defeatLogo;

	[SerializeField]
	private GameObject newRibbonRegular;

	[SerializeField]
	private GameObject newRibbonGold;

	[SerializeField]
	private tk2dTextMesh timeRemaining;

	private BountyBoardDataEntry data;

	private float startingHealth;

	private EffectInstance explosionEffect;

	private bool waitForResponse;

	private bool playingPreCollect;

	private bool playingPreDiscard;

	private UserUnit raidBossUnit;

	public override void ConfigureCellData()
	{
		if (base.DataObject != null)
		{
			data = base.DataObject as BountyBoardDataEntry;
			if (data.fightCell)
			{
				battleButton.SetActive(false);
				collectButton.SetActive(false);
				discardButton.SetActive(false);
				mvpGameObject.SetActive(false);
				playerGameObject.SetActive(false);
				bossInfoGameObject.SetActive(false);
				fightButton.SetActive(true);
				searchBoss.SetActive(true);
				huntForABoss.SetActive(true);
				ownerclubNameText.gameObject.SetActive(false);
				ownerUserNameText.gameObject.SetActive(false);
				ownerBadgeProxy.gameObject.SetActive(false);
				ownerBadgeProxySolo.gameObject.SetActive(false);
				timerGameObject.SetActive(false);
				defeatLogo.SetActive(false);
				victoryLogo.SetActive(false);
				ownerUserNameTextSolo.gameObject.SetActive(true);
				ownerUserNameTextSolo.text = LocalizationManager.GetString("ui_arenaselection_search_boss_title", "HUNT FOR A BOSS");
				if ((bool)cellBackground)
				{
					cellBackground.SetSprite("BG_Edit_BorderHolder_BlacknRed");
				}
				return;
			}
			UserProfile player = UserProfile.player;
			for (int i = 0; i < levelIcons.Count; i++)
			{
				if ((bool)levelIcons[i])
				{
					levelIcons[i].SetActive(false);
				}
			}
			AiArmyDataModel single = AiArmyDataModel.GetSingle(data.armyId);
			teamName.text = single.Name;
			List<UserUnit> unitList = single.GetUnitList();
			for (int j = 0; j < unitList.Count; j++)
			{
				if (unitList[j].UnitType == UnitType.RAID_BOSS)
				{
					startingHealth = unitList[j].CurrentLevelDataModel.hp;
					unitInfoview.gameObject.SetActive(true);
					unitInfoview.ConfigureUnitView(unitList[j].UnitDataModel, unitList[j].level, unitList[j].partialLevel);
					raidBossUnit = unitList[j];
					break;
				}
			}
			SetupLevel(data.raidBossLevel);
			healthBar.Value = (float)data.bossHealth / startingHealth;
			yourDamageDealt.text = data.damage.ToString();
			bossCurrentHealth.text = data.bossHealth.ToString();
			bool flag = string.IsNullOrEmpty(data.owner_club_name);
			ownerUserNameText.gameObject.SetActive(!flag);
			ownerUserNameTextSolo.gameObject.SetActive(flag);
			ownerclubNameText.gameObject.SetActive(!flag);
			mvpUserName.gameObject.SetActive(!flag);
			mvpUserNameSolo.gameObject.SetActive(flag);
			mvpClubName.gameObject.SetActive(!flag);
			ownerBadgeProxySolo.gameObject.SetActive(flag);
			mvpOwnerBadgeProxySolo.gameObject.SetActive(flag);
			ownerBadgeProxy.gameObject.SetActive(!flag);
			mvpOwnerBadgeProxy.gameObject.SetActive(!flag);
			if (flag)
			{
				ownerUserNameTextSolo.text = ((!string.IsNullOrEmpty(data.owner_user_name)) ? data.owner_user_name : string.Empty);
				mvpUserNameSolo.text = ((!string.IsNullOrEmpty(data.mvp_user_name)) ? data.mvp_user_name : string.Empty);
				StartCoroutine(SetBadge(ownerBadgeProxySolo, player.CurrentDivision.badgeLinkageId));
				StartCoroutine(SetBadge(mvpOwnerBadgeProxySolo, player.CurrentDivision.badgeLinkageId));
			}
			else
			{
				ownerUserNameText.text = ((!string.IsNullOrEmpty(data.owner_user_name)) ? data.owner_user_name : string.Empty);
				ownerclubNameText.text = ((!flag) ? data.owner_club_name : string.Empty);
				mvpUserName.text = ((!string.IsNullOrEmpty(data.mvp_user_name)) ? data.mvp_user_name : string.Empty);
				mvpClubName.text = ((!string.IsNullOrEmpty(data.mvp_club_name)) ? data.mvp_club_name : string.Empty);
				if (!string.IsNullOrEmpty(data.mvp_club_badge))
				{
					StartCoroutine(SetBadge(mvpOwnerBadgeProxy, int.Parse(data.mvp_club_badge)));
				}
				if (!string.IsNullOrEmpty(data.owner_club_badge))
				{
					StartCoroutine(SetBadge(ownerBadgeProxy, int.Parse(data.owner_club_badge)));
				}
			}
			damageDealt.text = data.mvp_damage.ToString();
			timeRemaining.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(data.timeToLive, true);
			switch (data.State)
			{
			case BountyBoardDataEntry.RaidBossState.ALIVE:
				battleButton.SetActive(true);
				collectButton.SetActive(false);
				discardButton.SetActive(false);
				fightButton.SetActive(false);
				searchBoss.SetActive(false);
				mvpGameObject.SetActive(true);
				playerGameObject.SetActive(true);
				bossInfoGameObject.SetActive(true);
				huntForABoss.SetActive(false);
				if ((bool)timerGameObject)
				{
					timerGameObject.SetActive(true);
				}
				if ((bool)progressBarGameObject)
				{
					progressBarGameObject.SetActive(true);
				}
				if ((bool)cellBackground)
				{
					cellBackground.SetSprite("BG_Edit_BorderHolder_BlacknRed");
				}
				if ((bool)victoryLogo)
				{
					victoryLogo.SetActive(false);
				}
				if ((bool)defeatLogo)
				{
					defeatLogo.SetActive(false);
				}
				break;
			case BountyBoardDataEntry.RaidBossState.DEAD:
				battleButton.SetActive(false);
				collectButton.SetActive(true);
				discardButton.SetActive(false);
				fightButton.SetActive(false);
				searchBoss.SetActive(false);
				mvpGameObject.SetActive(true);
				playerGameObject.SetActive(true);
				bossInfoGameObject.SetActive(true);
				huntForABoss.SetActive(false);
				if ((bool)progressBarGameObject)
				{
					progressBarGameObject.SetActive(false);
				}
				if ((bool)timerGameObject)
				{
					timerGameObject.SetActive(false);
				}
				if ((bool)cellBackground)
				{
					cellBackground.SetSprite("BG_Edit_BorderHolder_BlacknGreen");
				}
				if ((bool)victoryLogo)
				{
					victoryLogo.SetActive(true);
				}
				if ((bool)defeatLogo)
				{
					defeatLogo.SetActive(false);
				}
				break;
			case BountyBoardDataEntry.RaidBossState.EXPIRED:
				battleButton.SetActive(false);
				collectButton.SetActive(false);
				discardButton.SetActive(true);
				fightButton.SetActive(false);
				searchBoss.SetActive(false);
				mvpGameObject.SetActive(true);
				playerGameObject.SetActive(true);
				bossInfoGameObject.SetActive(true);
				huntForABoss.SetActive(false);
				if ((bool)timerGameObject)
				{
					timerGameObject.SetActive(false);
				}
				if ((bool)progressBarGameObject)
				{
					progressBarGameObject.SetActive(false);
				}
				if ((bool)cellBackground)
				{
					cellBackground.SetSprite("BG_Edit_BorderHolder_Grey");
				}
				if ((bool)victoryLogo)
				{
					victoryLogo.SetActive(false);
				}
				if ((bool)defeatLogo)
				{
					defeatLogo.SetActive(true);
				}
				break;
			}
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	private void SetupLevel(int level)
	{
		bool frame = level == Constants.RaidBossMaxLevel;
		if (levelIcons.Count <= Constants.RaidBossMaxLevel && (bool)levelIcons[level - 1])
		{
			levelIcons[level - 1].SetActive(true);
		}
		SetFrame(frame);
	}

	private void SetFrame(bool isMaxLevel)
	{
		bool flag = data.damage == 0 && data.State == BountyBoardDataEntry.RaidBossState.ALIVE;
		if ((bool)regularBossFrame)
		{
			regularBossFrame.SetActive(!isMaxLevel);
		}
		if ((bool)newRibbonRegular)
		{
			newRibbonRegular.SetActive(flag && !isMaxLevel);
		}
		if ((bool)maxLevelBossFrame)
		{
			maxLevelBossFrame.SetActive(isMaxLevel);
		}
		if ((bool)newRibbonGold)
		{
			newRibbonGold.SetActive(flag && isMaxLevel);
		}
	}

	private void Update()
	{
		if (data.State == BountyBoardDataEntry.RaidBossState.ALIVE)
		{
			timeRemaining.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(data.timeToLive, true);
			if (NonUnitySingleton<TimeManager>.instance.GetTimeDelta(data.timeToLive) <= 0)
			{
				data.state = BountyBoardDataEntry.RaidBossState.EXPIRED.GetRaidBossStateString();
				ConfigureCellData();
			}
		}
	}

	public IEnumerator SetBadge(PrefabProxy prefabProxy, int clubBadgeId)
	{
		AssetLinkageDataModel clubBadgeAssetLinkage = AssetLinkageDataModel.GetSingle(clubBadgeId.ToString());
		if (clubBadgeAssetLinkage != null)
		{
			yield return StartCoroutine(prefabProxy.ChangeAssetCoroutine(clubBadgeAssetLinkage));
		}
	}

	public void BattleBossButton()
	{
		if (!SceneTransitionManager.transitionActive)
		{
			BattleSceneModel battleSceneModel = new BattleSceneModel();
			battleSceneModel.matchType = MatchData.Type.RAIDBOSS;
			battleSceneModel.raidbossId = data.raidBossId;
			if (data.battleBoss != null)
			{
				data.battleBoss(battleSceneModel);
			}
		}
	}

	public void FigthButton()
	{
		data.battleBoss(null);
	}

	public void CollectButton()
	{
		if (waitForResponse || BountyBoardController.IsUpdating)
		{
			return;
		}
		waitForResponse = true;
		BountyBoardController.IsUpdating = true;
		controller.ScrollableArea.allowSwipeScrolling = false;
		Singleton<SessionManager>.instance.CompleteRaidBoss(true, data.raidBossId, data, delegate(RewardLabelTypeCollection rewards)
		{
			UserProfile.player.AddItems(rewards.itemCollections);
			if (base.gameObject.activeInHierarchy)
			{
				StartCoroutine(CollectCoroutine(rewards));
			}
		});
		StartCoroutine(PreCollectCoroutine());
	}

	public void DiscardButton()
	{
		if (!waitForResponse && !BountyBoardController.IsUpdating)
		{
			waitForResponse = true;
			BountyBoardController.IsUpdating = true;
			controller.ScrollableArea.allowSwipeScrolling = false;
			Singleton<SessionManager>.instance.CompleteRaidBoss(false, data.raidBossId, data, delegate
			{
				Reporting.RaidBossDismissed(data.armyId, data.raidBossId, (int)startingHealth, data.bossHealth, "Manual");
				waitForResponse = false;
				StartCoroutine(DiscardCoroutine());
			});
			StartCoroutine(PreDiscardCoroutine());
		}
	}

	private IEnumerator PreDiscardCoroutine()
	{
		playingPreDiscard = true;
		base.gameObject.transform.TweenLocalXPosition(base.transform.localPosition.x + 2000f, 1f);
		yield return new WaitForSeconds(1f);
		playingPreDiscard = false;
	}

	private IEnumerator DiscardCoroutine()
	{
		while (playingPreDiscard)
		{
			yield return 0;
		}
		if ((bool)controller)
		{
			controller.DataSource.Remove(base.DataObject);
			controller.OnDataChanged();
		}
		BountyBoardController.IsUpdating = false;
		controller.ScrollableArea.allowSwipeScrolling = true;
	}

	private IEnumerator PreCollectCoroutine()
	{
		playingPreCollect = true;
		LayerMask topBarLayer = LayerMask.NameToLayer("TopBar");
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.BOSS_EXPLOSION, unitInfoview.unitProxy.transform.position, base.gameObject, delegate(EffectInstance x)
		{
			explosionEffect = x;
		}));
		explosionEffect.Reset();
		explosionEffect.transform.position = unitInfoview.unitProxy.transform.position;
		AudioTrigger.BigBossExplosion.Play();
		explosionEffect.SetLayer(topBarLayer);
		explosionEffect.SpineAnimation.Skeleton.SortOrder = 101;
		yield return new WaitForSeconds(1.4f);
		CurrencyEffect cashEffect = GlobalEffectsManager.Create(EffectType.CASH, unitInfoview.unitProxy.transform.position, base.gameObject).SetLayer(topBarLayer).GetComponent<CurrencyEffect>();
		cashEffect.ShowText = false;
		cashEffect.ConfigureEffect(UserInventory.ItemType.RaidBossEventPoint, raidBossUnit.DestroyEventPoints);
		cashEffect.gameObject.SetLayerRecursively(topBarLayer);
		cashEffect.SortingOrder = 102;
		unitInfoview.gameObject.SetActive(false);
		yield return new WaitForSeconds(1.6f);
		playingPreCollect = false;
	}

	private IEnumerator CollectCoroutine(RewardLabelTypeCollection collectRewards)
	{
		while (playingPreCollect)
		{
			yield return 0;
		}
		GachaRewardsSceneModel raidBossResults = new GachaRewardsSceneModel(GachaTypes.RAID_BOSS);
		ItemCollectionDataModel rewards = new ItemCollectionDataModel();
		List<string> itemBonusLabels = new List<string>();
		for (int i = 0; i < collectRewards.itemCollections.Count; i++)
		{
			foreach (ItemCollectionDataModel.Item item in collectRewards.itemCollections[i].items)
			{
				rewards.AddItem(item);
				itemBonusLabels.Add(collectRewards.labelType[i]);
			}
		}
		ItemCollectionDataModel.Item eventPoints = new ItemCollectionDataModel.Item(UserInventory.ItemType.RaidBossEventPoint, 0, collectRewards.bonusEventPoints);
		rewards.AddItem(eventPoints);
		itemBonusLabels.Add("rewards_loc_event_points");
		explosionEffect.Destroy();
		raidBossResults.gachaRewards = rewards;
		raidBossResults.itemBonusLabels = itemBonusLabels;
		PopupManager.ShowPopup(PopupDataModel.GachaResult(raidBossResults, "ui_raidboss_rewards"));
		yield return new WaitForSeconds(0.2f);
		if ((bool)controller)
		{
			controller.DataSource.Remove(base.DataObject);
		}
		controller.OnDataChanged();
		waitForResponse = false;
		BountyBoardController.IsUpdating = false;
		controller.ScrollableArea.allowSwipeScrolling = true;
	}

	private void OnDisable()
	{
		if ((bool)explosionEffect)
		{
			explosionEffect.Destroy();
		}
		StopAllCoroutines();
	}
}
