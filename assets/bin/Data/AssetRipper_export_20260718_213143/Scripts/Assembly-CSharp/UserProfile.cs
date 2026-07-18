using System;
using System.Collections.Generic;
using System.Linq;
using LitJson0;
using UnityEngine;

public class UserProfile
{
	public const int TEAM_COUNT = 3;

	public const int RESEARCHER_COUNT = 1;

	public JsonObject jsonSource;

	public string id;

	public long version;

	public long lastLogin;

	public string username;

	public bool isAdmin;

	public List<string> activeLeaderboards;

	private string _nickname;

	public long mobageId;

	public string thumbnail;

	private UserInventory _inventory;

	private int _coins;

	private int _scrap;

	private int _lastEnergy = -1;

	public long energyRecoveryTime;

	public int energyOvercharge;

	public Dictionary<string, int> parts;

	public int unitsKilled;

	public int wins;

	public int losses;

	public int totalBattles;

	public int winStreak;

	public bool claimedFirstUnit;

	public int pvpRating;

	public int videoAdsCount;

	public long videoAdsLastShow;

	public int botBattleCount;

	public long botBattleLast;

	public int botBattleRestoreCount = 1;

	public long joinClubLastShow;

	public int soloEventPoints;

	public string locale;

	public Dictionary<string, UserUnit> unitInventory;

	public List<string> purchasedSkinIds = new List<string>();

	public List<UserTeam> teams;

	public int currentTeamIndex;

	public List<string> abilities;

	public List<UserAbilitySet> userAbilitySets;

	public List<string> abilitiesInventory;

	public List<string> newAbilities = new List<string>();

	public string divisionId = "1";

	public int points;

	public string promoSeriesId;

	public int promoSeriesWins;

	public int promoSeriesLosses;

	public string promoSeriesLastId;

	public int stadiumIndex;

	public int newBlueprintsCount;

	public int newAbilitiesCount;

	public List<int> divisionsWithRewardsClaimed;

	public long random_seed_contracts;

	public List<string> nextAIArmies = new List<string>();

	public List<int> nextContracts = new List<int>();

	public List<UserNotification> notifications = new List<UserNotification>();

	public Action notificationsCompleteCallback;

	private UserTutorialData _tutorial;

	private UserDialogTriggerData _dialogTriggers;

	private UserPreferenceData _preferences;

	private UserFacebookData _facebookData;

	public List<UserUnitStats> unitStats;

	public List<UserResearcher> researchers;

	public UserContract contract;

	public List<UserGachaPrize> userGachaPrizes;

	public List<UserBoost> boosts = new List<UserBoost>();

	public UserClub userClub;

	public string clubID;

	private int lastClubMsg;

	private int currentClubMsg;

	public int pendingClubCrateCount;

	public UserMultiTeamReport userMultiTeamReport;

	public ClaimedReportData lastClaimedReport;

	public static UserProfile player
	{
		get
		{
			return Singleton<UserProfileManager>.instance.UserProfile;
		}
		set
		{
			Singleton<UserProfileManager>.instance.UserProfile = value;
		}
	}

	public string nickname
	{
		get
		{
			if (!string.IsNullOrEmpty(username))
			{
				return username;
			}
			if (!string.IsNullOrEmpty(FacebookData.Name) && (_nickname.Substring(0, 4) == "FB:(" || _nickname.Substring(0, 4) == "Rival"))
			{
				return FacebookData.Name;
			}
			return _nickname;
		}
		set
		{
			_nickname = value;
		}
	}

	public UserInventory inventory
	{
		get
		{
			if (_inventory == null)
			{
				_inventory = new UserInventory(this);
			}
			return _inventory;
		}
	}

	public int coins
	{
		get
		{
			return _coins;
		}
		set
		{
			int arg = _coins;
			_coins = value;
			if (this.OnCoinsChanged != null)
			{
				this.OnCoinsChanged(arg, _coins);
			}
		}
	}

	public int gems
	{
		get
		{
			return _scrap;
		}
		set
		{
			int scrap = _scrap;
			_scrap = value;
			if (this.OnGemsChanged != null)
			{
				this.OnGemsChanged(scrap, _scrap);
			}
		}
	}

	private long normalizedEnergyRecoveryTime
	{
		get
		{
			return Math.Min(Math.Max(0L, energyRecoveryTime - TimeManager.ServerTime), Constants.EnergyRechargeSeconds * 1000 * Constants.MaxEnergy);
		}
	}

	public int energy
	{
		get
		{
			return energyOvercharge + GetRechargeableEnergy(normalizedEnergyRecoveryTime);
		}
		set
		{
			int num = value - energy;
			long num2 = normalizedEnergyRecoveryTime;
			if (num > 0)
			{
				num2 += -num * Constants.EnergyRechargeSeconds * 1000;
				int rechargeableEnergy = GetRechargeableEnergy(num2);
				if (rechargeableEnergy > Constants.MaxEnergy)
				{
					energyOvercharge += rechargeableEnergy - Constants.MaxEnergy;
				}
				energyRecoveryTime = TimeManager.ServerTime + Math.Max(0L, num2);
			}
			else if (num < 0)
			{
				if (energyOvercharge > 0)
				{
					int val = num + energyOvercharge;
					num += energyOvercharge;
					energyOvercharge = Math.Max(val, 0);
				}
				energyRecoveryTime = TimeManager.ServerTime + num2 + Mathf.FloorToInt((float)(-num) * ((float)Constants.EnergyRechargeSeconds * 1000f));
			}
			if (energy != _lastEnergy)
			{
				if (this.OnEnergyChanged != null)
				{
					this.OnEnergyChanged(_lastEnergy, energy);
				}
				_lastEnergy = energy;
			}
		}
	}

	public int maxEnergy
	{
		get
		{
			return Constants.MaxEnergy;
		}
	}

	public int NewContentCount
	{
		get
		{
			return newBlueprintsCount + newAbilitiesCount;
		}
	}

	public int divisionInt
	{
		get
		{
			return int.Parse(divisionId);
		}
	}

	public UserTutorialData tutorial
	{
		get
		{
			if (_tutorial == null)
			{
				_tutorial = new UserTutorialData(this);
			}
			return _tutorial;
		}
	}

	public UserDialogTriggerData dialogTriggers
	{
		get
		{
			if (_dialogTriggers == null)
			{
				_dialogTriggers = new UserDialogTriggerData(this);
			}
			return _dialogTriggers;
		}
	}

	public UserPreferenceData preferences
	{
		get
		{
			if (_preferences == null)
			{
				_preferences = new UserPreferenceData();
			}
			return _preferences;
		}
	}

	public UserFacebookData FacebookData
	{
		get
		{
			if (_facebookData == null)
			{
				_facebookData = new UserFacebookData();
			}
			return _facebookData;
		}
	}

	public UserResearcher NextAvailableResearcher
	{
		get
		{
			foreach (UserResearcher researcher in researchers)
			{
				if (researcher.IsIdle)
				{
					return researcher;
				}
			}
			return null;
		}
	}

	public UserResearcher SoonestAvailableResearcher
	{
		get
		{
			UserResearcher userResearcher = ((researchers.Count <= 0) ? null : researchers[0]);
			foreach (UserResearcher researcher in researchers)
			{
				userResearcher = ((researcher.RemainingSeconds >= userResearcher.RemainingSeconds) ? userResearcher : researcher);
			}
			return userResearcher;
		}
	}

	public UserResearcher ClaimableResearcher
	{
		get
		{
			foreach (UserResearcher researcher in researchers)
			{
				if (researcher.CanClaim)
				{
					return researcher;
				}
			}
			return null;
		}
	}

	public int LastClubMsg
	{
		get
		{
			return lastClubMsg;
		}
		set
		{
			lastClubMsg = value;
			if (this.OnClubChatChanged != null)
			{
				this.OnClubChatChanged();
			}
		}
	}

	public int CurrentClubMsg
	{
		get
		{
			return currentClubMsg;
		}
		set
		{
			currentClubMsg = value;
			if (this.OnClubChatChanged != null)
			{
				this.OnClubChatChanged();
			}
		}
	}

	public bool IsClubMember
	{
		get
		{
			return userClub != null || !string.IsNullOrEmpty(clubID);
		}
	}

	public bool HasSeenLastClubMessage
	{
		get
		{
			if (CurrentClubMsg == 0)
			{
				return true;
			}
			return CurrentClubMsg == LastClubMsg;
		}
	}

	public UserTeam CurrentTeam
	{
		get
		{
			if (teams == null)
			{
				return null;
			}
			if (currentTeamIndex >= teams.Count)
			{
				return null;
			}
			return teams[currentTeamIndex];
		}
	}

	public UserAbilitySet CurrentAbilitySet
	{
		get
		{
			if (userAbilitySets == null)
			{
				return null;
			}
			if (currentTeamIndex >= userAbilitySets.Count)
			{
				return null;
			}
			return userAbilitySets[currentTeamIndex];
		}
	}

	public ProgressionPromotionSeriesDataModel PromoSeries
	{
		get
		{
			if (IsInPromoSeries)
			{
				return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionPromotionSeriesDataModel>(promoSeriesId);
			}
			return null;
		}
	}

	public bool IsInPromoSeries
	{
		get
		{
			return !string.IsNullOrEmpty(promoSeriesId);
		}
	}

	public ProgressionDivisionDataModel CurrentDivision
	{
		get
		{
			return NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(divisionId);
		}
	}

	public List<AbilityDataModel> UnlockedAbilities
	{
		get
		{
			List<AbilityDataModel> list = new List<AbilityDataModel>();
			foreach (AbilityDataModel item in NonUnitySingleton<DMAccessManager>.instance.GetAll<AbilityDataModel>())
			{
				if (item.numKillUnit <= unitsKilled)
				{
					list.Add(item);
				}
			}
			return list;
		}
	}

	public List<AiArmyDataModel> NextAIArmies
	{
		get
		{
			List<AiArmyDataModel> list = new List<AiArmyDataModel>();
			AiArmyDataModel aiArmyDataModel = null;
			foreach (string nextAIArmy in nextAIArmies)
			{
				aiArmyDataModel = NonUnitySingleton<DMAccessManager>.instance.GetSingle<AiArmyDataModel>(nextAIArmy);
				list.Add(aiArmyDataModel);
			}
			return list;
		}
	}

	public event Action<int, int> OnCoinsChanged;

	public event Action<int, int> OnGemsChanged;

	public event Action<int, int> OnEnergyChanged;

	public event Action OnClubChatChanged;

	public event Action OnPartsChanged;

	public event Action OnResearchClaimed;

	public UserProfile()
	{
		activeLeaderboards = new List<string>();
		teams = new List<UserTeam>();
		userAbilitySets = new List<UserAbilitySet>();
		parts = new Dictionary<string, int>();
		researchers = new List<UserResearcher>(1);
		for (int i = 0; i < 1; i++)
		{
			researchers.Add(new UserResearcher());
		}
		divisionsWithRewardsClaimed = new List<int>();
		unitStats = new List<UserUnitStats>();
		userGachaPrizes = new List<UserGachaPrize>();
	}

	private int GetRechargeableEnergy(long normalizedRecoveryTime)
	{
		return Constants.MaxEnergy - (int)Mathf.Ceil((float)normalizedRecoveryTime / ((float)Constants.EnergyRechargeSeconds * 1000f));
	}

	public void SetPartCount(string partId, int amount)
	{
		parts[partId] = amount;
		if (this.OnPartsChanged != null)
		{
			this.OnPartsChanged();
		}
	}

	public void UpdateUnitEventItems()
	{
		EventDataModel activeEvent = GetActiveEvent();
		foreach (UserUnit value in unitInventory.Values)
		{
			EventUnitBoostDataModel boostModel = null;
			string partsID = string.Empty;
			if (activeEvent != null)
			{
				boostModel = EventUnitBoostDataModel.FindUnitBoost(value.UnitDataModel.id, value.level, activeEvent.id);
				EventPartsDataModel eventPartsDataModel = EventPartsDataModel.FindUnitEventPart(value.metadataId, activeEvent.id);
				if (eventPartsDataModel != null)
				{
					partsID = eventPartsDataModel.ID;
				}
			}
			value.UpdateEventParts(partsID);
			value.UpdateBoost(boostModel);
		}
	}

	public void ClearNewContentCount()
	{
		newBlueprintsCount = 0;
		newAbilitiesCount = 0;
	}

	public UserBoost GetOrCreateBoost(BoostDataModel boostData)
	{
		UserBoost userBoost = boosts.Find((UserBoost x) => x.metaID == int.Parse(boostData.id));
		if (userBoost == null)
		{
			userBoost = new UserBoost(int.Parse(boostData.id), -1L);
			boosts.Add(userBoost);
		}
		return userBoost;
	}

	public void AdvanceDivision()
	{
		int promoSeriesCompleteReward = Constants.PromoSeriesCompleteReward;
		if (promoSeriesCompleteReward != 0)
		{
			ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(promoSeriesCompleteReward);
			player.AddItems(giftPackage);
		}
		Reporting.TierUpEvent(PromoSeries.promotionDivisionId);
		AdvanceToDivision(PromoSeries.promotionDivisionId.ToString());
	}

	public void AdvanceToDivision(string destDivisionId)
	{
		divisionId = destDivisionId;
		ResetDivision();
		ResetPromotionSeries();
		notifications.Add(new UserNotification.DivisionPromotion(CurrentDivision));
		bool flag = false;
		for (int i = 0; i < abilitiesInventory.Count; i++)
		{
			if (AbilityDataModel.GetSingle(abilitiesInventory[i]).unlockTier == divisionInt)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			return;
		}
		IEnumerable<AbilityDataModel> abilitiesByDivision = AbilityDataModel.GetAbilitiesByDivision(divisionInt);
		foreach (AbilityDataModel item in abilitiesByDivision)
		{
			if (item.isActive == 1)
			{
				abilitiesInventory.Add(item.id);
				newAbilities.Add(item.id);
				notifications.Add(new UserNotification.AbilityEarned(item));
			}
		}
	}

	public void ResetDivision()
	{
		points = 0;
	}

	public void BeginPromoSeries(ProgressionPromotionSeriesDataModel promoSeries)
	{
		ResetPromotionSeries();
		promoSeriesId = promoSeries.id;
		promoSeriesLastId = promoSeries.id;
		promoSeriesWins = 0;
		promoSeriesLosses = 0;
		notifications.Add(new UserNotification.PromotionSeriesEarned(promoSeries));
	}

	public void FailPromoSeries()
	{
		notifications.Add(new UserNotification.PromotionSeriesFailure(PromoSeries));
		points = CurrentDivision.ResetPoints;
		ResetPromotionSeries();
	}

	private void ResetPromotionSeries()
	{
		promoSeriesId = null;
		promoSeriesWins = 0;
		promoSeriesLosses = 0;
	}

	public bool CanAffordItem(ItemCollectionDataModel.Item item)
	{
		int item2 = inventory.GetItem(item.itemType, item.itemId);
		return item2 >= item.amount;
	}

	public bool CanAfford(IList<ItemCollectionDataModel.Item> items)
	{
		foreach (ItemCollectionDataModel.Item item in items)
		{
			if (!CanAffordItem(item))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanAfford(UserPriceDataModel price)
	{
		for (int i = 0; i < price.items.Count; i++)
		{
			ItemCollectionDataModel.Item item = price.items[i];
			if (!CanAffordItem(item))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanAfford(int priceId)
	{
		return CanAfford(ItemPriceDataModel.GetPriceForID(priceId));
	}

	public bool CanAfford(IPurchasableDataModel purchasable)
	{
		return CanAfford(purchasable.GetPrice());
	}

	public void PayPrice(UserPriceDataModel price)
	{
		RemoveItems(price);
	}

	public void PayPrice(IPurchasableDataModel purchasable)
	{
		PayPrice(purchasable.GetPrice());
	}

	public void AddSkin(UnitLevelProgressionDataModel skinToPurchase)
	{
		if (!purchasedSkinIds.Contains(skinToPurchase.id))
		{
			purchasedSkinIds.Add(skinToPurchase.id);
		}
	}

	public List<UserUnit> AddNewUnitsToInventory(List<JsonObject> jsonNewUnits)
	{
		List<UserUnit> list = new List<UserUnit>();
		foreach (JsonObject jsonNewUnit in jsonNewUnits)
		{
			UserUnit userUnit = UserUnit.FromJSON(jsonNewUnit);
			unitInventory.Add(userUnit.id, userUnit);
			list.Add(userUnit);
		}
		return list;
	}

	public void AddItems(List<ItemCollectionDataModel> genericItem)
	{
		foreach (ItemCollectionDataModel item in genericItem)
		{
			AddItems(item);
		}
	}

	public void AddItems(ItemCollectionDataModel genericItem)
	{
		foreach (ItemCollectionDataModel.Item item in genericItem.items)
		{
			inventory.AddItem(item.itemType, item.itemId.ToString(), item.amount);
		}
	}

	public void RemoveItems(ItemCollectionDataModel genericItem)
	{
		foreach (ItemCollectionDataModel.Item item in genericItem.items)
		{
			inventory.RemoveItem(item.itemType, item.itemId.ToString(), item.amount);
		}
	}

	public bool HasUnlockedSkin(string skinID)
	{
		return purchasedSkinIds.Contains(skinID);
	}

	public bool HasUnlockedUnit(UnitDataModel unitDataModel)
	{
		return unitDataModel.unlockTier <= int.Parse(divisionId);
	}

	public bool HasUnlockedAbility(AbilityDataModel abilityDataModel)
	{
		return abilityDataModel.unlockTier <= int.Parse(divisionId);
	}

	public void RemoveUnitFromAllTeams(UserUnit unit)
	{
		for (int i = 0; i < teams.Count; i++)
		{
			teams[i].RemoveUnit(unit);
		}
	}

	public bool HasBuiltUnit(string unitId)
	{
		return TimesBuiltUnit(unitId) > 0;
	}

	public int TimesBuiltUnit(string unitId)
	{
		for (int i = 0; i < unitStats.Count; i++)
		{
			UserUnitStats userUnitStats = unitStats[i];
			if (userUnitStats.metaId == unitId)
			{
				return userUnitStats.built;
			}
		}
		return 0;
	}

	public void BuiltUnit(string metadataId)
	{
		foreach (UserUnitStats unitStat in unitStats)
		{
			if (unitStat.metaId == metadataId)
			{
				unitStat.AddBuilt();
				return;
			}
		}
		unitStats.Add(new UserUnitStats(metadataId, 1));
	}

	public bool HasUnlockedAbility(string abilityId)
	{
		if (abilitiesInventory == null)
		{
			return false;
		}
		return abilitiesInventory.Contains(abilityId);
	}

	public void TryStartResearch(UserResearcher.ResearchType type, string itemId, Action successCallback = null)
	{
		UserResearcher availableResearcher = NextAvailableResearcher;
		if (availableResearcher == null)
		{
			UserResearcher claimableResearcher = ClaimableResearcher;
			if (claimableResearcher != null)
			{
				PopupManager.ShowPopup(PopupDataModel.One("ui_skip_claim_tank_title".Localize("Existing Research"), "ui_skip_claim_tank_message".Localize("Before starting new research, you must collect your results!"), "ui_skip_claim_tank_button".Localize("Claim!!"), delegate
				{
					TryClaimResearch(claimableResearcher);
				}));
			}
			else
			{
				PopupManager.ShowPopup(PopupDataModel.SkipWaitPopup(SoonestAvailableResearcher, "ui_skip_build_tank_title".Localize("SKIP"), "ui_skip_build_tank_message".Localize("Speed up tank construction???"), SkipResearchFlags));
			}
			return;
		}
		if (type == UserResearcher.ResearchType.BuildTank)
		{
			UnitDataModel single = UnitDataModel.GetSingle(itemId);
			UserPriceDataModel buildPrice = single.GetBuildPrice(player.TimesBuiltUnit(single.id));
			if (!player.CanAfford(buildPrice))
			{
				buildPrice = UserPriceDataModel.GetPremiumPrice(buildPrice, player, Constants.CashToGemRateMaxUpgrade);
				int itemCountOfType = buildPrice.GetItemCountOfType(UserInventory.ItemType.PremiumCurrency);
				UserPriceDataModel GemCost = new UserPriceDataModel(UserInventory.ItemType.PremiumCurrency, itemCountOfType);
				PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(GemCost, "ui_not_enough_parts_title".Localize("not enough parts").ToUpper(), "ui_not_enough_parts_direct_purchase".Localize("Would you like to spend to buy them?"), delegate
				{
					if (player.CanAfford(GemCost))
					{
						Reporting.TankDirectPurchase("Purchased", GemCost.PrintItems(), itemId);
						DisplayResearchReward(itemId);
						Singleton<SessionManager>.instance.StartResearch(type, itemId, availableResearcher, UserPriceDataModel.PaymentType.UsePremiumForDifference);
						if (successCallback != null)
						{
							successCallback();
						}
					}
					else
					{
						PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
						{
							PopupManager.DestroyAllPopups();
							TopBarController.instance.LoadShop();
						}));
					}
				}, null, delegate
				{
					Reporting.TankDirectPurchase("Cancelled", GemCost.PrintItems(), itemId);
				}));
				return;
			}
			DisplayResearchReward(itemId);
		}
		Singleton<SessionManager>.instance.StartResearch(type, itemId, availableResearcher, UserPriceDataModel.PaymentType.Normal);
		if (successCallback != null)
		{
			successCallback();
		}
	}

	private void DisplayResearchReward(string itemId)
	{
		if (Constants.ResearchStartReward != 0 && player.TimesBuiltUnit(itemId) == 0)
		{
			ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(Constants.ResearchStartReward);
			CurrencyEffect.Create(UserInventory.ItemType.Energy, giftPackage.items[0].amount);
		}
	}

	public void TryClaimResearch(UserResearcher researcher, Action successCallback = null)
	{
		UnitDataModel unitDataModel = (UnitDataModel)researcher.ResearchItem;
		if (researcher.CanClaim)
		{
			if (researcher.researchType == UserResearcher.ResearchType.BuildTank)
			{
				Vector3 startPos = TopBarController.instance.TopBarCamera.ScreenCamera.ViewportToWorldPoint(new Vector3(0.33f, 0.5f, 0.1f));
				Vector3 startPos2 = TopBarController.instance.TopBarCamera.ScreenCamera.ViewportToWorldPoint(new Vector3(0.66f, 0.5f, 0.1f));
				CurrencyEffect currencyEffect = CurrencyEffect.Create((UserInventory.ItemType)unitDataModel.rewardTypeId, unitDataModel.rewardAmount, startPos);
				if (player.TimesBuiltUnit(unitDataModel.id) == 0)
				{
					ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(Constants.ResearchClaimReward);
					CurrencyEffect currencyEffect2 = CurrencyEffect.Create(UserInventory.ItemType.Energy, giftPackage.items[0].amount, startPos2);
					currencyEffect2.gameObject.SetLayerRecursively(LayerMask.NameToLayer("NotificationPopUp"));
				}
				currencyEffect.gameObject.SetLayerRecursively(LayerMask.NameToLayer("NotificationPopUp"));
			}
			Singleton<SessionManager>.instance.ClaimResearch(researcher, null);
			PopupManager.ShowPopup(PopupDataModel.ClaimUnitPopUp(unitDataModel, null, delegate
			{
				if (this.OnResearchClaimed != null)
				{
					this.OnResearchClaimed();
				}
				if (successCallback != null)
				{
					successCallback();
				}
			}));
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_already_claim_tank_title".Localize("Research Claimed"), "ui_already_claim_tank_message".Localize("Research has already been claimed.")));
		}
	}

	public UserResearcher GetResearcher(UserResearcher.ResearchType type, string itemId)
	{
		return researchers.Find((UserResearcher x) => x.researchType == type && x.itemID == itemId);
	}

	public bool HasClaimedDivisionReward(int divisionId)
	{
		return divisionsWithRewardsClaimed.IndexOf(divisionId) != -1;
	}

	public void TryClaimDivisionReward(int divisionId, Action successCallback = null)
	{
		Singleton<SessionManager>.instance.ClaimDivisionReward(divisionId, delegate
		{
		});
		if (successCallback != null)
		{
			successCallback();
		}
	}

	public void TryClaimFirstUnit(Action completeHandler)
	{
		if (!player.claimedFirstUnit)
		{
			player.claimedFirstUnit = true;
			Singleton<SessionManager>.instance.ClaimStarterUnit(delegate
			{
				Singleton<SessionManager>.instance.GetUserUnits(false, delegate
				{
					if (player.unitInventory.Count <= Constants.MinUnitsPerTeam)
					{
						UserTeam userTeam = player.teams[0];
						int num = 0;
						foreach (UserUnit value in player.unitInventory.Values)
						{
							userTeam.SetUnit(num, value);
							num++;
						}
						Singleton<SessionManager>.instance.UpdateTeam(delegate
						{
							if (completeHandler != null)
							{
								completeHandler();
							}
						});
					}
					else if (completeHandler != null)
					{
						completeHandler();
					}
				});
			});
		}
		else if (completeHandler != null)
		{
			completeHandler();
		}
	}

	public UserGachaPrize GetGachaPrizeData(int id)
	{
		UserGachaPrize userGachaPrize = player.userGachaPrizes.Find((UserGachaPrize x) => x.gachaPrizeID == id);
		if (userGachaPrize == null)
		{
			userGachaPrize = new UserGachaPrize(id, 0L);
			userGachaPrizes.Add(userGachaPrize);
		}
		return userGachaPrize;
	}

	private void SkipResearchFlags()
	{
		BlueprintCell.shouldSkipClaimAnimation = true;
	}

	public void ApplyStreakLogic(bool isVictory)
	{
		if ((0 > winStreak && isVictory) || (0 < winStreak && !isVictory))
		{
			winStreak = 0;
		}
		winStreak += (isVictory ? 1 : (-1));
	}

	public EventDataModel GetActiveEvent(bool userInClubCheck = false)
	{
		if (userInClubCheck && !IsClubMember)
		{
			return null;
		}
		return EventDataModel.GetActiveEvent();
	}

	public EventDataModel GetActiveOnCooldownEvent(bool userInClubCheck = false)
	{
		if (userInClubCheck && !IsClubMember)
		{
			return null;
		}
		EventDataModel activeEvent = GetActiveEvent(userInClubCheck);
		if (activeEvent != null)
		{
			return activeEvent;
		}
		return EventDataModel.GetOnCooldownEvent();
	}

	public EventDataModel GetNextEvent(bool userInClubCheck = false)
	{
		if (userInClubCheck && !IsClubMember)
		{
			return null;
		}
		return EventDataModel.GetNextEvent();
	}

	public int GetBuildCount()
	{
		EventDataModel activeOnCooldownEvent = player.GetActiveOnCooldownEvent();
		int num = 0;
		int num2 = player.divisionInt;
		List<UnitDataModel> all = UnitDataModel.GetAll();
		for (int i = 0; i < all.Count; i++)
		{
			UnitDataModel unitDataModel = all[i];
			if (unitDataModel.UnitType.IsExclusive())
			{
				continue;
			}
			if (unitDataModel.UnitType.IsEvent())
			{
				if (activeOnCooldownEvent == null || !activeOnCooldownEvent.UnitBelongsToEvent(unitDataModel.id))
				{
					continue;
				}
			}
			else if (num2 < unitDataModel.unlockTier || unitDataModel.unlockTier == 0)
			{
				continue;
			}
			int num3 = player.TimesBuiltUnit(unitDataModel.id);
			if (num3 <= 0)
			{
				UserPriceDataModel buildPrice = unitDataModel.GetBuildPrice(num3);
				if (player.CanAfford(buildPrice))
				{
					num++;
				}
			}
		}
		return num;
	}

	public List<UnitDataModel> GetBuildList()
	{
		int num = player.divisionInt;
		List<UnitDataModel> list = new List<UnitDataModel>();
		foreach (UnitDataModel item in UnitDataModel.GetAll())
		{
			if (player.HasBuiltUnit(item.id) || item.UnitType.IsExclusive())
			{
				continue;
			}
			if (item.UnitType.IsEvent())
			{
				EventDataModel activeOnCooldownEvent = player.GetActiveOnCooldownEvent();
				if (activeOnCooldownEvent == null || !activeOnCooldownEvent.UnitBelongsToEvent(item.id))
				{
					continue;
				}
			}
			else if (num < item.unlockTier || item.unlockTier == 0)
			{
				continue;
			}
			UserPriceDataModel buildPrice = item.GetBuildPrice(player.TimesBuiltUnit(item.id));
			if (player.CanAfford(buildPrice))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public void ResetToPVPDivision()
	{
	}

	public int ABTestingGroup(GachaAbTestingDataModel gachaABTesting)
	{
		long num = mobageId / 2;
		return (int)(num % gachaABTesting.groupsCount);
	}

	public string GetGachaABTestingAnalitics(int gachaId = 0)
	{
		string text = string.Empty;
		List<GachaPoolsDataModel> list = (from x in GachaPoolsDataModel.GetAll()
			where x.abTestingId != 0
			select x).ToList();
		if (gachaId != 0)
		{
			list = list.Where((GachaPoolsDataModel x) => x.id == gachaId.ToString()).ToList();
		}
		int num = 0;
		foreach (GachaPoolsDataModel item in list)
		{
			GachaAbTestingDataModel single = GachaAbTestingDataModel.GetSingle(item.abTestingId);
			if (single != null && single.UserIsInABTesting)
			{
				if (num > 0)
				{
					text += "|";
				}
				string text2 = text;
				text = text2 + single.id + "," + ABTestingGroup(single);
				num++;
			}
		}
		return text;
	}
}
