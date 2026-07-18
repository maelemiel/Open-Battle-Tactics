using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using LitJson0;
using UnityEngine;

public class SessionManager : Singleton<SessionManager>
{
	public class LeaderboardRewardResponse
	{
		public string leaderboardId;

		public int finalRank;

		public int finalPoints;

		public List<int> rewardGiftPackageIds;

		public IList topRanked;

		public LeaderboardRewardResponse()
		{
			rewardGiftPackageIds = new List<int>();
		}
	}

	public enum ConnectedState
	{
		Offline = 0,
		Connecting = 1,
		Connected = 2,
		Tutorial = 3
	}

	public enum QueuePriority
	{
		Low = 0,
		Normal = 1,
		High = 2
	}

	public delegate void ConnectionCallback(ConnectedState connectedState);

	private const int MAX_CONNECT_RETRIES = 5;

	private bool requestResult;

	private Action<bool> parentCallback;

	private static string PERSISTENT_DATA_PATH = Application.persistentDataPath + "/";

	private ConnectedState _connectedState = ConnectedState.Connecting;

	private ServerUtilities.Session Session;

	private static int requestId = UnityEngine.Random.Range(1, 2147473647);

	private DeNetwork network;

	private int _loadingPopupNetworkConnectionId;

	public ConnectedState connectedState
	{
		get
		{
			return _connectedState;
		}
	}

	public DeNetwork DeNetwork
	{
		get
		{
			return network;
		}
	}

	public ServerUtilities.Session UserSession
	{
		get
		{
			return Session;
		}
	}

	public event ConnectionCallback ConnectionEvent;

	public void PostAnalytics(List<JsonObject> messages, Action cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetObjectList("messages", messages);
		DeNetwork.Server().At("/analytics/report").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb();
				}
			});
	}

	public void BattleHeartbeat(string matchID)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("match_id", matchID);
		DeNetwork.Server().At("/api/setBattleUserHeartbeat").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
			});
	}

	public void PurchaseBoost(BoostDataModel boost, bool usePremium, string matchType, Action cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["boost_id"] = boost.id;
		jsonObject["use_premium"] = usePremium;
		jsonObject["match_type"] = matchType;
		DeNetwork.Server().At("/api/purchaseboost").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (response != null)
				{
					if (response.Error != null)
					{
						Debug.LogError("----------------    can't buy boost");
						if (cb != null)
						{
							cb();
						}
					}
					else
					{
						if (jsonObject2.Contains("Error"))
						{
							Debug.LogError("----------------    can't buy boost   " + jsonObject2.GetObject("Error").ToString());
						}
						else
						{
							PurchaseBoostLogic(boost);
						}
						if (cb != null)
						{
							cb();
						}
					}
				}
			});
	}

	private void PurchaseBoostLogic(BoostDataModel boost)
	{
		UserProfile.player.PayPrice(boost.Price);
		UserProfile.player.GetOrCreateBoost(boost);
		Reporting.CurrencyTransactionEvent(PurchaseSource.PreBattleBoost, boost.Price);
	}

	public void BuyBulkEnergy(int energyAmount)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["energy_amount"] = energyAmount;
		BuyBulkEnergyLogic(energyAmount);
		DeNetwork.Server().At("/api/buyBulkEnergy").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
			});
	}

	private bool BuyBulkEnergyLogic(int energyAmount)
	{
		bool result = false;
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel(UserInventory.ItemType.PremiumCurrency, Constants.EnergyRestoreCost * energyAmount);
		if (UserProfile.player.CanAfford(userPriceDataModel))
		{
			Reporting.BuyTicketsEvent(energyAmount, userPriceDataModel);
			UserProfile.player.PayPrice(userPriceDataModel);
			UserProfile.player.energy += energyAmount;
			result = true;
			Reporting.CurrencyTransactionEvent(PurchaseSource.BuyItemPopup, userPriceDataModel);
		}
		return result;
	}

	public void BuyEnergy()
	{
		JsonObject o = new JsonObject();
		BuyTicketsLogic();
		DeNetwork.Server().At("/api/buyenergy").Post(o)
			.Response<JsonObject>(delegate
			{
			});
	}

	public void ClaimEnergy()
	{
		int num = 1;
		if (UserProfile.player.boosts.Count > 0)
		{
			for (int i = 0; i < UserProfile.player.boosts.Count; i++)
			{
				BoostDataModel single = BoostDataModel.GetSingle(UserProfile.player.boosts[i].metaID);
				if (single.Type != BoostType.TicketRBDmgBoost_1 && single.Type != BoostType.TicketRBDmgBoost_2)
				{
					continue;
				}
				UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(single.priceId);
				foreach (ItemCollectionDataModel.Item item in priceForID.items)
				{
					if (item.itemType == UserInventory.ItemType.Energy)
					{
						num += item.amount;
					}
				}
			}
		}
		UserProfile.player.energy += num;
	}

	private bool BuyTicketsLogic()
	{
		bool result = false;
		UserPriceDataModel userPriceDataModel = new UserPriceDataModel(UserInventory.ItemType.PremiumCurrency, Constants.EnergyRestoreCost);
		if (UserProfile.player.CanAfford(userPriceDataModel))
		{
			Reporting.BuyTicketsEvent(1, userPriceDataModel);
			UserProfile.player.PayPrice(userPriceDataModel);
			UserProfile.player.energy++;
			result = true;
			Reporting.CurrencyTransactionEvent(PurchaseSource.BuyItemPopup, userPriceDataModel);
			CurrencyEffect.Create(UserInventory.ItemType.Energy, 1);
		}
		return result;
	}

	public void BuyPrizeGacha(GachaPoolsDataModel prizeGachaMetadata, ItemsMixerModel itemsMixerModel = null, Action<GachaResult> cb = null)
	{
		long serverTime = TimeManager.ServerTime;
		JsonObject jsonObject = new JsonObject();
		jsonObject["prize_gacha_id"] = prizeGachaMetadata.ID;
		jsonObject["claim_timestamp"] = serverTime;
		if (itemsMixerModel != null)
		{
			jsonObject["parts_mixer_item_type"] = itemsMixerModel.itemType;
			jsonObject["parts_mixer_item_id"] = itemsMixerModel.itemId;
			jsonObject["parts_mixer_price_item_type"] = itemsMixerModel.priceItemType;
			jsonObject["parts_mixer_price_item_id"] = itemsMixerModel.priceItemId;
			jsonObject["selected_slot"] = itemsMixerModel.selectedSlot;
			Reporting.PartsMixerEvent(prizeGachaMetadata.ID, itemsMixerModel.itemId.ToString(), itemsMixerModel.priceItemId.ToString());
		}
		BuyPrizeGachaLogic(prizeGachaMetadata, itemsMixerModel, serverTime);
		DeNetwork.Server().At("/api/buyPrizeGacha").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				ItemCollectionDataModel itemCollectionDataModel = null;
				int slot = -1;
				if (jsonObject2.Contains("itemCollection"))
				{
					JsonObject json = jsonObject2.GetObject("itemCollection");
					itemCollectionDataModel = ItemCollectionDataModel.FromJSON(json);
				}
				if (jsonObject2.Contains("bundleCollection"))
				{
					JsonObject json2 = jsonObject2.GetObject("bundleCollection");
					ItemCollectionDataModel genericItem = ItemCollectionDataModel.FromJSON(json2);
					UserProfile.player.AddItems(genericItem);
				}
				else
				{
					UserProfile.player.AddItems(itemCollectionDataModel);
				}
				if (jsonObject2.Contains("slot"))
				{
					slot = jsonObject2.GetInt("slot");
				}
				if (jsonObject2.Contains("newUnits"))
				{
					UserProfile.player.AddNewUnitsToInventory(jsonObject2.GetObjectList("newUnits"));
				}
				GachaResult gachaResult = new GachaResult();
				gachaResult = new GachaResult
				{
					itemCollection = itemCollectionDataModel,
					slot = slot
				};
				if (cb != null)
				{
					cb(gachaResult);
				}
			});
	}

	private void BuyPrizeGachaLogic(GachaPoolsDataModel prizeGachaMetadata, ItemsMixerModel itemsMixeModel, long timestamp)
	{
		UserGachaPrize userGachaPrize = null;
		userGachaPrize = UserProfile.player.GetGachaPrizeData(int.Parse(prizeGachaMetadata.id));
		if (userGachaPrize.gachaType != 3)
		{
			userGachaPrize.startTime = timestamp;
		}
		UserProfile.player.PayPrice(prizeGachaMetadata.GetPrice());
		TopBarController.instance.UpdateNotifications();
	}

	public void FastForwardGacha(string gachaId, Action cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["gacha_id"] = gachaId;
		DeNetwork.Server().At("/api/fastForwardGacha").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				UserProfile.player.videoAdsCount++;
				if (cb != null)
				{
					cb();
				}
			});
	}

	private void FastForwardLogic(string gachaId)
	{
		foreach (UserGachaPrize userGachaPrize in UserProfile.player.userGachaPrizes)
		{
			if (userGachaPrize.id == gachaId)
			{
				userGachaPrize.startTime -= Constants.VideoAdsDecreaseGachaTime;
			}
		}
	}

	public void BuyShopItem(IShopItemMetadata shopItemMetadata, Action<bool> cb)
	{
		if (!BuyShopItemLogic(shopItemMetadata) && cb != null)
		{
			cb(false);
		}
		if (cb != null)
		{
			cb(true);
		}
	}

	private bool BuyShopItemLogic(IShopItemMetadata shopItemMetadata)
	{
		return true;
	}

	public void ChangeUserName(string userName, UserPriceDataModel.PaymentType paymentType, Action<ServerUtilities.BaseResponse> cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["user_name"] = userName;
		jsonObject["use_premium"] = paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference;
		ChangeUserNameLogic(userName, paymentType);
		DeNetwork.Server().At("/api/changeUserName").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					cb(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	private void ChangeUserNameLogic(string userName, UserPriceDataModel.PaymentType paymentType)
	{
		if (!string.IsNullOrEmpty(UserProfile.player.username))
		{
			UserPriceDataModel userPriceDataModel = ItemPriceDataModel.GetPriceForID(Constants.ChangeNamePriceId);
			if (paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference)
			{
				userPriceDataModel = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, UserProfile.player, Constants.SkinGemToCashConversion);
			}
			UserProfile.player.PayPrice(userPriceDataModel);
			Reporting.CurrencyTransactionEvent(PurchaseSource.PickNamePayment, userPriceDataModel);
		}
	}

	public void SetChatMessage(string message, bool isClubMsg, Action cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["body"] = message;
		jsonObject.SetBoolean("is_club_msg", isClubMsg);
		DeNetwork.Server().At("/chat/set_msg").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				cb();
			});
	}

	public void GetChatMessage(string lastSequence, bool isClubMsg, Action<List<ChatMessage>, string, bool> cb)
	{
		Action<TypedRestResponse<JsonObject>> callback = delegate(TypedRestResponse<JsonObject> response)
		{
			JsonObject jsonObject = response.Resource().GetObject("result");
			if (jsonObject.Contains("error"))
			{
				string text = jsonObject.GetString("error");
				if (text == "UserKicked" && UserProfile.player != null)
				{
					UserProfile.player.clubID = string.Empty;
					UserProfile.player.userClub = null;
				}
				PopupManager.ShowPopup(PopupDataModel.Ok("ui_popup_warning".Localize("Warning"), text.Localize(text)));
				cb(null, null, false);
			}
			else
			{
				List<ChatMessage> list = null;
				foreach (JsonObject @object in jsonObject.GetObjectList("messages"))
				{
					ChatMessage chatMessage = new ChatMessage();
					JsonObject jsonObject2 = @object.GetObject("data");
					chatMessage.id = @object.GetInt("id");
					chatMessage.message = @object.GetString("body");
					chatMessage.nickname = @object.GetString("nickname");
					if (jsonObject2 != null)
					{
						chatMessage.tier = jsonObject2.GetIntOrDefault("tier", 0);
						chatMessage.pvpRating = jsonObject2.GetIntOrDefault("pvp_rating", 0);
						if (jsonObject2.Contains("is_admin"))
						{
							chatMessage.isAdmin = jsonObject2.GetBoolean("is_admin");
						}
						else
						{
							chatMessage.isAdmin = false;
						}
					}
					chatMessage.userID = @object.GetString("user_id");
					chatMessage.createdAt = @object.GetLong("created_at");
					if (list == null)
					{
						list = new List<ChatMessage>();
					}
					list.Add(chatMessage);
					if (isClubMsg)
					{
						UserProfile.player.CurrentClubMsg = chatMessage.id;
					}
				}
				if (cb != null)
				{
					cb(list, jsonObject["last_seq"].ToString(), true);
				}
			}
		};
		DeNetwork.Server().At("/chat/get_msg").Param("last_seq", lastSequence)
			.Param("is_club_msg", (!isClubMsg) ? "false" : "true")
			.Get()
			.Response(callback);
	}

	public void SetChatMessageViewed(int msgId, bool isClubMsg, Action cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["msg_id"] = msgId;
		jsonObject.SetBoolean("is_club_msg", isClubMsg);
		DeNetwork.Server().At("/chat/set_msg_viewed").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb();
				}
			});
	}

	public void CheatCreateUnits(Dictionary<string, int> unitsToCreate, Action<List<UserUnit>> cb)
	{
	}

	public void CheatGiveCurrency(UserInventory.ItemType itemType, int amount, Action cb)
	{
	}

	public void CheatUnlockAbilities(List<string> abilityIds, Action cb)
	{
	}

	public void CheatSetDivision(int divisionID, Action cb)
	{
	}

	public void CheatGiveParts(Dictionary<int, int> partTypeAmountDictionary, Action cb)
	{
	}

	public void CheatRestoreEnergy(Action cb)
	{
	}

	public void CheatTestError()
	{
	}

	public void CheatEmptyWallet(Action cb)
	{
	}

	public void CheatResetFreeGacha(Action cb)
	{
	}

	public void CheatMakeUserAdmin(Action cb)
	{
	}

	public void CheatResetUserPVP(Action cb)
	{
	}

	public void SkipTutorialStep(Action<ServerUtilities.BaseResponse> cb)
	{
	}

	public void ClientLogging(string text)
	{
	}

	public void CheatResetCrateDate(Action cb)
	{
	}

	public void CheatGetCrates(Action cb)
	{
	}

	public void CheatGive100EventPoints(Action cb)
	{
	}

	public void CheatRemoveAllUnits(Action cb)
	{
	}

	public void CheatClearParts(Action cb)
	{
	}

	public void CheatClearTokens(List<int> tokenList, Action cb)
	{
	}

	public void ClaimStarterUnit(Action cb)
	{
		if (UserProfile.player.unitInventory != null && UserProfile.player.unitInventory.Count > 3)
		{
			cb();
			return;
		}
		JsonObject jsonObject = new JsonObject();
		jsonObject["index"] = UserProfile.player.tutorial.FirstUnitIndex;
		DeNetwork.Server().At("/api/claimstarterunit").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				cb();
			});
	}

	public void CompleteRaidBoss(bool collect, string raidBossId, BountyBoardDataEntry data, Action<RewardLabelTypeCollection> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetInt("collect", collect ? 1 : 0);
		jsonObject.SetString("raidboss_id", raidBossId);
		DeNetwork.Server().At("/api/completeRaidBossReward").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				Log.DebugTag("Received Respeonse " + jsonObject2.ToJson(), null, "CompleteRaidBoss");
				RewardLabelTypeCollection rewardLabelTypeCollection = new RewardLabelTypeCollection();
				if (jsonObject2.Contains("error"))
				{
					string text = jsonObject2.GetString("error");
					PopupManager.ShowPopup(PopupDataModel.Ok("Error", text.Localize(text)));
					cb(rewardLabelTypeCollection);
				}
				else
				{
					int intOrDefault = jsonObject2.GetIntOrDefault("bonusPts", 0);
					List<ItemCollectionDataModel> list = new List<ItemCollectionDataModel>();
					List<string> list2 = new List<string>();
					List<int> list3 = new List<int>();
					if (jsonObject2.Contains("giftid"))
					{
						foreach (object item in jsonObject2.GetList("giftid"))
						{
							list3.Add(Convert.ToInt32(item));
							list.Add(ItemGiftDataModel.GetGiftPackage(Convert.ToInt32(item)));
						}
						foreach (object item2 in jsonObject2.GetList("awards"))
						{
							list2.Add(item2.ToString());
						}
					}
					rewardLabelTypeCollection.itemCollections = list;
					rewardLabelTypeCollection.labelType = list2;
					rewardLabelTypeCollection.bonusEventPoints = intOrDefault;
					if (intOrDefault > 0)
					{
						int prizes = 0;
						int discoveryBonus = 0;
						int socialBonus = 0;
						int socialAmount = 0;
						int mvpBonus = 0;
						for (int i = 0; i < list3.Count; i++)
						{
							switch (list2[i])
							{
							case "rewards_loc_rb_mvp":
								mvpBonus = list3[i];
								break;
							case "rewards_loc_rb_owner":
								discoveryBonus = list3[i];
								break;
							case "rewards_loc_rb_killed":
								prizes = list3[i];
								break;
							case "rewards_loc_rb_members_2":
								socialBonus = list3[i];
								socialAmount = 2;
								break;
							case "rewards_loc_rb_members_3":
								socialBonus = list3[i];
								socialAmount = 3;
								break;
							case "rewards_loc_rb_members_4":
								socialBonus = list3[i];
								socialAmount = 4;
								break;
							}
						}
						Reporting.RaidBossRewardClaimed(data.armyId, data.raidBossId, prizes, data.mvp_user, mvpBonus, socialBonus, socialAmount, discoveryBonus, data.damage, intOrDefault);
					}
					if (jsonObject2.Contains("newUnits"))
					{
						UserProfile.player.AddNewUnitsToInventory(jsonObject2.GetObjectList("newUnits"));
					}
					if (cb != null)
					{
						cb(rewardLabelTypeCollection);
					}
				}
			});
	}

	public void CreateClub(UserClub clubData, Action<UserClub> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("name", clubData.name.Trim());
		jsonObject.SetString("description", clubData.description);
		jsonObject.SetString("password", clubData.password);
		jsonObject.SetInt("badge", int.Parse(clubData.teamBadge));
		jsonObject.SetInt("type", (int)clubData.teamType);
		jsonObject.SetInt("tier_requirement", clubData.minTier);
		DeNetwork.Server().At("/api/clubs/createClub").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (cb != null)
				{
					bool boolean = jsonObject2.GetBoolean("success");
					UserClub userClub = null;
					if (boolean)
					{
						if (jsonObject2.Contains("club"))
						{
							userClub = UserClub.FromJSON(jsonObject2.GetObject("club"));
							Reporting.CreateClubEvent(userClub.clubID, userClub.name, userClub.teamType);
						}
					}
					else
					{
						string text = jsonObject2.GetString("reason");
						Log.DebugTag("Creating a club " + text + " " + text.Localize(text), null, "Default");
						PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("Error"), "Reason: " + text.Localize(text)).ShowCloseButton(false));
					}
					cb(userClub);
				}
			});
	}

	public void CreateMatch(MatchData.Type matchType, bool giveup, string matchID, int difficulty, string botPartId, string raidBossId, Action<CreateMatchResponse> cb)
	{
		JsonObject jsonObject = new JsonObject();
		string value = ((matchType == MatchData.Type.AI) ? "bot" : ((matchType == MatchData.Type.AUTO_BOT_BATTLE) ? "auto_bot_battle" : ((matchType == MatchData.Type.RAIDBOSS) ? "eventRaidboss" : (giveup ? "pvpThenBot" : "pvp"))));
		jsonObject.SetString("type", value);
		jsonObject.SetInt("difficulty", difficulty);
		if (matchID != null)
		{
			jsonObject.SetString("id", matchID);
		}
		if (!string.IsNullOrEmpty(raidBossId))
		{
			jsonObject.SetString("raidbossid", raidBossId);
		}
		if (!string.IsNullOrEmpty(botPartId))
		{
			jsonObject.SetString("botpartid", botPartId);
		}
		Action<TypedRestResponse<JsonObject>> callback = delegate(TypedRestResponse<JsonObject> httpResponse)
		{
			JsonObject jsonObject2 = httpResponse.Resource().GetObject("result");
			CreateMatchResponse createMatchResponse = new CreateMatchResponse
			{
				matchID = jsonObject2.GetString("_id")
			};
			if (jsonObject2.GetList("players").Count < 2)
			{
				if (jsonObject2.GetInt("semaphore") <= -1)
				{
					createMatchResponse.abortSequence = true;
				}
				createMatchResponse.success = false;
			}
			else
			{
				createMatchResponse.matchData = MatchParser.ParseMatchJSON(new MatchParser.Input
				{
					json = jsonObject2,
					localPlayerID = UserProfile.player.id,
					UnitParser = MatchParserUtils.GetUnitMetadata,
					AbilityParser = MatchParserUtils.GetAbilityMetadata,
					DivisionParser = MatchParserUtils.GetDivisionMetadata
				});
				Log.DebugTag("Raw JSON: " + jsonObject2.ToJson(), null, "CreateMatch");
				UserProfile.player.energy -= Constants.BattleEnergyCost;
				OpponentData opponentTeam = createMatchResponse.matchData.opponentTeam;
				Log.DebugTag("opponent Type " + opponentTeam.type, null, "CreateMatch");
				Log.DebugTag("opponent ID " + opponentTeam.id, null, "CreateMatch");
				Log.DebugTag("opponent Name " + opponentTeam.name, null, "CreateMatch");
				Log.DebugTag("player seed " + createMatchResponse.matchData.playerTeam.randomSeed, null, "CreateMatch");
				Log.DebugTag("enemy seed " + createMatchResponse.matchData.opponentTeam.randomSeed, null, "CreateMatch");
				Log.DebugTag("battle seed " + createMatchResponse.matchData.battleSeed, null, "CreateMatch");
				if (matchType == MatchData.Type.AUTO_BOT_BATTLE)
				{
					createMatchResponse.matchData.playerTeam.ai = createMatchResponse.matchData.opponentTeam.ai;
				}
				createMatchResponse.success = true;
			}
			cb(createMatchResponse);
		};
		DeNetwork.Server().At("/api/creatematch").Post(jsonObject)
			.Response(callback);
	}

	public void ClaimDivisionReward(int divisionId, Action<ServerUtilities.BaseResponse> callback)
	{
		ClaimDivisionRewardLogic(divisionId);
		JsonObject jsonObject = new JsonObject();
		jsonObject["division_id"] = divisionId;
		DeNetwork.Server().At("/api/claimdivisionreward").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (jsonObject2.Contains("newUnits"))
				{
					UserProfile.player.AddNewUnitsToInventory(jsonObject2.GetObjectList("newUnits"));
				}
				if (callback != null)
				{
					callback(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	private void ClaimDivisionRewardLogic(int divisionId)
	{
		UserProfile player = UserProfile.player;
		player.divisionsWithRewardsClaimed.Add(divisionId);
		ProgressionDivisionDataModel single = ProgressionDivisionDataModel.GetSingle(divisionId.ToString());
		player.AddItems(single.CompletionClaimReward);
	}

	public void EditClub(UserClub clubData, Action<UserClub> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("name", clubData.name.Trim());
		jsonObject.SetString("description", clubData.description);
		jsonObject.SetString("password", clubData.password);
		jsonObject.SetInt("badge", int.Parse(clubData.teamBadge));
		jsonObject.SetInt("type", (int)clubData.teamType);
		jsonObject.SetInt("tier_requirement", clubData.minTier);
		DeNetwork.Server().At("/api/clubs/editClub").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (cb != null)
				{
					if (jsonObject2.GetBoolean("success"))
					{
						UserClub userClub = UserProfile.player.userClub;
						UserClub obj = null;
						if (jsonObject2.Contains("club"))
						{
							obj = UserClub.FromJSON(jsonObject2.GetObject("club"), userClub, false);
						}
						cb(obj);
					}
					else
					{
						string text = jsonObject2.GetString("reason");
						PopupManager.ShowPopup(PopupDataModel.Ok("Error", "Reason: " + text.Localize(text)));
						cb(null);
					}
				}
			});
	}

	public void GetClub(Action<UserClub> cb = null)
	{
		JsonObject o = new JsonObject();
		DeNetwork.Server().At("/api/clubs/getClub").Post(o)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				if (cb != null)
				{
					UserClub obj = null;
					if (jsonObject.Contains("club"))
					{
						JsonObject jsonObject2 = jsonObject.GetObject("club");
						if (jsonObject2 != null && jsonObject2.Dictionary != null)
						{
							bool flag = UserProfile.player.GetActiveEvent() != null;
							if (jsonObject2.Contains("error"))
							{
								string text = jsonObject2.GetString("error");
								if (text == "UserKicked" && UserProfile.player != null)
								{
									UserProfile.player.clubID = string.Empty;
									UserProfile.player.userClub = null;
								}
								if (flag && text == "UserKicked")
								{
									int num = 0;
									if (UserProfile.player.userClub != null)
									{
										num = UserProfile.player.userClub.GetCurrentEventUserClubPoints(UserProfile.player.id);
									}
									num = (int)Math.Floor((double)num * ((double)Constants.EventPointsKeptOnKick / 100.0));
									if (num != 0)
									{
										string message = string.Format("UserKickedEvent".Localize("You have been kicked out of the club\n You will retain {0} event ponts!"), num);
										PopupManager.ShowPopup(PopupDataModel.Ok("Error", message));
									}
									else
									{
										PopupManager.ShowPopup(PopupDataModel.Ok("Error", text.Localize(text)));
									}
								}
								else
								{
									PopupManager.ShowPopup(PopupDataModel.Ok("Error", text.Localize(text)));
								}
							}
							else
							{
								obj = UserClub.FromJSON(jsonObject2);
							}
						}
					}
					cb(obj);
				}
			});
	}

	public void GetClubWithID(string clubID, Action<UserClub> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("club_id", clubID);
		DeNetwork.Server().At("/api/clubs/getClub").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (cb != null)
				{
					UserClub obj = null;
					if (jsonObject2.Contains("club"))
					{
						obj = UserClub.FromJSON(jsonObject2.GetObject("club"));
					}
					cb(obj);
				}
			});
	}

	public void GetClubCrates(Action<List<ClubCrateDataModel>> cb = null)
	{
		DeNetwork.Server().At("/api/openclubcrates").Post()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				if (cb != null)
				{
					List<JsonObject> objectList = jsonObject.GetObjectList("crateResults");
					List<ClubCrateDataModel> list = new List<ClubCrateDataModel>();
					if (objectList != null)
					{
						for (int i = 0; i < objectList.Count; i++)
						{
							JsonObject jsonObject2 = objectList[i];
							if (jsonObject2.Contains("from_user") && jsonObject2.Contains("rewards"))
							{
								string fromUser = jsonObject2.GetString("from_user");
								ItemCollectionDataModel itemCollectionDataModel = null;
								itemCollectionDataModel = ItemCollectionDataModel.FromJSON(jsonObject2.GetObject("rewards"));
								UserProfile.player.AddItems(itemCollectionDataModel);
								ClubCrateDataModel item = new ClubCrateDataModel(fromUser, itemCollectionDataModel);
								list.Add(item);
							}
						}
					}
					cb(list);
				}
			});
	}

	public void GetPendingClubCrateCount(Action<int> cb = null)
	{
		DeNetwork.Server().At("/api/pendingcratecount").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				if (cb != null)
				{
					int intOrDefault = jsonObject.GetIntOrDefault("crate_count", 0);
					cb(intOrDefault);
				}
			});
	}

	public void GetEventClub(string clubID, string eventID, Action<JsonObject> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("club_id", clubID);
		jsonObject.SetString("event_id", clubID);
		DeNetwork.Server().At("/api/clubevent/getclubevent").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject obj = response.Resource().GetObject("result");
				if (cb != null)
				{
					cb(obj);
				}
			});
	}

	public void GetNextItemsMixerSet(int itemsMixerDetailsId, Action<ItemCollectionDataModel> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["items_mixer_id"] = itemsMixerDetailsId;
		DeNetwork.Server().At("/api/getNextItemsMixerSet").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject itemsMixerSet = response.Resource().GetObject("result");
				ItemCollectionDataModel obj = FromJSON(itemsMixerSet);
				if (cb != null)
				{
					cb(obj);
				}
			});
	}

	private ItemCollectionDataModel FromJSON(JsonObject itemsMixerSet)
	{
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		if (itemsMixerSet != null && itemsMixerSet.Contains("next_parts"))
		{
			List<JsonObject> objectList = itemsMixerSet.GetObjectList("next_parts");
			foreach (JsonObject item2 in objectList)
			{
				ItemCollectionDataModel.Item item = new ItemCollectionDataModel.Item();
				item.itemId = item2.GetInt("itemId");
				item.itemType = (UserInventory.ItemType)item2.GetInt("itemType");
				item.amount = 1;
				itemCollectionDataModel.AddItem(item);
			}
		}
		return itemCollectionDataModel;
	}

	public void GetRaidBossEvents(Action<List<BountyBoardDataEntry>> cb = null)
	{
		DeNetwork.Server().At("/api/getUserActiveRaidBosses").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				Log.DebugTag("Received Respeonse " + jsonObject.ToJson(), null, "GetRaidBoss");
				List<BountyBoardDataEntry> obj = new List<BountyBoardDataEntry>();
				if (jsonObject.Contains("error"))
				{
					string text = jsonObject.GetString("error");
					PopupManager.ShowPopup(PopupDataModel.Ok("Error", text.Localize(text)));
					cb(obj);
				}
				else
				{
					JsonObject jsonObject2 = jsonObject.GetObject("entries");
					if (jsonObject2 != null && jsonObject2.Dictionary != null)
					{
						obj = BountyBoardDataEntry.CreateEntryFromJson(jsonObject2);
					}
					if (cb != null)
					{
						cb(obj);
					}
				}
			});
	}

	public void GetSoloEventPoints()
	{
		DeNetwork.Server().At("/api/event/getSoloEventPoints").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				if (jsonObject.Contains("user"))
				{
					int soloEventPoints = jsonObject.GetInt("user");
					UserProfile.player.soloEventPoints = soloEventPoints;
				}
			});
	}

	public void GetStepGachas(Action<Dictionary<int, UserStepGacha>> cb = null)
	{
		DeNetwork.Server().At("/api/stepgacha/getByUser").Post()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					Dictionary<int, UserStepGacha> obj = new Dictionary<int, UserStepGacha>();
					JsonObject jsonObject = response.Resource().GetObject("result");
					if (jsonObject.Contains("stepGachas"))
					{
						obj = UserStepGacha.FromJSON(jsonObject.GetObjectList("stepGachas"));
					}
					cb(obj);
				}
			});
	}

	public void GetUserUnits(bool reparseUserProfile, Action cb)
	{
		Action<TypedRestResponse<JsonObject>> callback = delegate(TypedRestResponse<JsonObject> response)
		{
			JsonObject jsonObject = response.Resource().GetObject("result");
			if (!jsonObject.Contains("units"))
			{
				cb();
			}
			else
			{
				Log.Debug("Raw JSON: " + jsonObject.ToJson());
				List<UserUnit> list = new List<UserUnit>();
				foreach (JsonObject @object in jsonObject.GetObjectList("units"))
				{
					try
					{
						list.Add(UserUnit.FromJSON(@object));
					}
					catch (Exception ex)
					{
						Log.Error(ex.ToString());
					}
				}
				list.RemoveAll((UserUnit unit) => unit == null);
				Dictionary<string, UserUnit> dictionary = new Dictionary<string, UserUnit>();
				foreach (UserUnit item in list)
				{
					dictionary[item.ID] = item;
				}
				UserProfile.player.unitInventory = dictionary;
				if (reparseUserProfile)
				{
					Singleton<UserProfileManager>.instance.UpdateUserProfile(UserProfile.player.jsonSource, delegate
					{
						cb();
					});
				}
				else
				{
					cb();
				}
			}
		};
		DeNetwork.Server().At("/api/getuserunits").Get()
			.Response(callback);
	}

	public void GetUser(Action cb)
	{
		DeNetwork.Server().At("/api/getuser").Get()
			.Response<JsonObject>(delegate
			{
				cb();
			});
	}

	public void JoinClub(UserClub club, string password, Action<UserClub> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("id", club.clubID);
		if (!string.IsNullOrEmpty(password))
		{
			jsonObject.SetString("password", password);
		}
		DeNetwork.Server().At("/api/clubs/joinClub").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (cb != null)
				{
					if (jsonObject2.GetBoolean("success"))
					{
						UserClub obj = UserProfile.player.userClub;
						if (jsonObject2.Contains("club"))
						{
							obj = UserClub.FromJSON(jsonObject2.GetObject("club"));
						}
						cb(obj);
					}
					else
					{
						string text = jsonObject2.GetString("reason");
						PopupManager.ShowPopup(PopupDataModel.Ok("Error", "Reason: " + text.Localize(text)));
					}
				}
			});
	}

	public bool JoinClubLogic(UserClub club)
	{
		UserProfile player = UserProfile.player;
		if (player == null)
		{
			return false;
		}
		if (!string.IsNullOrEmpty(player.clubID))
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("Error"), "ui_clubs_error_already_on_club".Localize("You are already part of a club")));
			return false;
		}
		if (club.minTier > int.Parse(player.divisionId))
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("Error"), "ui_clubs_error_need_higher_tier".Localize("You need to be at a higher tier to join this club")));
			return false;
		}
		return true;
	}

	public void KickFromClub(string kickMemberID, Action<bool> cb = null)
	{
		if (!KickOutUserLogic(kickMemberID))
		{
			return;
		}
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("kick_member_id", kickMemberID);
		DeNetwork.Server().At("/api/clubs/kickFromClub").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (cb != null)
				{
					bool boolean = jsonObject2.GetBoolean("success");
					if (boolean)
					{
						cb(boolean);
					}
					else
					{
						string text = jsonObject2.GetString("reason");
						PopupManager.ShowPopup(PopupDataModel.Ok("Error", "Reason: " + text.Localize(text), QuitUtility.Restart).ShowCloseButton(false));
					}
				}
			});
	}

	private bool KickOutUserLogic(string memberToKickID)
	{
		UserProfile player = UserProfile.player;
		if (player == null)
		{
			Log.Error("No player profile found");
			return false;
		}
		if (player.userClub == null)
		{
			Log.Error("Player is trying to leave a club without being part of any");
			return false;
		}
		if (!player.userClub.UserIsMember(memberToKickID))
		{
			Log.Error("Player is trying to kick out a non-existent member");
			return false;
		}
		if (!player.userClub.UserIsLeader(player.id))
		{
			Log.Error("Player is not the club leader. Only leaders can kick out other members ");
			return false;
		}
		ClubMember clubMember = player.userClub.members.Find((ClubMember x) => x.ID == memberToKickID);
		if (clubMember != null)
		{
			player.userClub.members.Remove(clubMember);
		}
		return true;
	}

	private LeaderboardEntryData ParseLeaderboardEntryData(JsonObject rawEntry)
	{
		LeaderboardEntryData leaderboardEntryData = new LeaderboardEntryData();
		leaderboardEntryData.playerName = rawEntry.GetString("nickname");
		leaderboardEntryData.pvpRating = rawEntry.GetInt("pvp_rating");
		leaderboardEntryData.rank = rawEntry.GetInt("rank");
		leaderboardEntryData.tier = rawEntry.GetString("tier");
		leaderboardEntryData.userId = rawEntry.GetString("user_id");
		leaderboardEntryData.tankID = rawEntry.GetString("tank_id");
		leaderboardEntryData.tankLevel = rawEntry.GetIntOrDefault("tank_lvl", 1);
		return leaderboardEntryData;
	}

	private EventLeaderboardEntryData ParseEventLeaderboardEntryData(JsonObject rawEntry)
	{
		EventLeaderboardEntryData eventLeaderboardEntryData = new EventLeaderboardEntryData();
		if (!rawEntry.Contains("name"))
		{
			return eventLeaderboardEntryData;
		}
		eventLeaderboardEntryData.clubName = rawEntry.GetString("name");
		eventLeaderboardEntryData.error = eventLeaderboardEntryData.clubName == null;
		if (eventLeaderboardEntryData.error)
		{
			Log.ErrorTag("Errored Club data in leaderboard, null Name Raw Json: " + rawEntry.ToJson(), null, "Leaderboards");
			eventLeaderboardEntryData.clubName = "Error";
		}
		eventLeaderboardEntryData.winCount = rawEntry.GetIntOrDefault("score", 0);
		eventLeaderboardEntryData.rank = rawEntry.GetIntOrDefault("rank", 1000000);
		eventLeaderboardEntryData.badgeId = rawEntry.GetIntOrDefault("badge_id", 11001);
		eventLeaderboardEntryData.clubId = rawEntry.GetString("_id");
		return eventLeaderboardEntryData;
	}

	private SoloLeaderboardEntryData ParseSoloLeaderboardEntryData(JsonObject rawEntry)
	{
		SoloLeaderboardEntryData soloLeaderboardEntryData = new SoloLeaderboardEntryData();
		soloLeaderboardEntryData.playerName = rawEntry.GetString("nickname");
		soloLeaderboardEntryData.pvpRating = rawEntry.GetInt("pvp_rating");
		soloLeaderboardEntryData.rank = rawEntry.GetInt("rank");
		soloLeaderboardEntryData.tier = rawEntry.GetString("tier");
		soloLeaderboardEntryData.userId = rawEntry.GetString("user_id");
		soloLeaderboardEntryData.tankID = rawEntry.GetString("tank_id");
		soloLeaderboardEntryData.tankLevel = rawEntry.GetIntOrDefault("tank_lvl", 1);
		return soloLeaderboardEntryData;
	}

	public void GetLeaderboardTopEntries(Action<List<LeaderboardEntryData>, bool> cb)
	{
		DeNetwork.Server().At("/api/getleaderboardtop").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericEntriesResponse(response, cb, ParseLeaderboardEntryData);
			});
	}

	public void GetEventLeaderboardTopEntries(Action<List<EventLeaderboardEntryData>, bool> cb)
	{
		DeNetwork.Server().At("/api/club/getleaderboardtop").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericEntriesResponse(response, cb, ParseEventLeaderboardEntryData);
			});
	}

	public void GetSoloLeaderboardTopEntries(Action<List<SoloLeaderboardEntryData>, bool> cb)
	{
		DeNetwork.Server().At("/api/event_single/getleaderboardtop").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericEntriesResponse(response, cb, ParseSoloLeaderboardEntryData);
			});
	}

	public void GetLeaderboardSelfEntries(Action<List<LeaderboardEntryData>, bool> cb)
	{
		DeNetwork.Server().At("/api/getleaderboardself").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericEntriesResponse(response, cb, ParseLeaderboardEntryData);
			});
	}

	public void GetEventLeaderboardSelfEntries(Action<List<EventLeaderboardEntryData>, bool> cb)
	{
		DeNetwork.Server().At("/api/club/getleaderboardself").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericEntriesResponse(response, cb, ParseEventLeaderboardEntryData);
			});
	}

	public void GetSoloLeaderboardSelfEntries(Action<List<SoloLeaderboardEntryData>, bool> cb)
	{
		DeNetwork.Server().At("/api/event_single/getleaderboardself").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericEntriesResponse(response, cb, ParseSoloLeaderboardEntryData);
			});
	}

	public void CheckActiveLeaderboards(Action<List<LeaderboardRewardResponse>, List<string>> callback)
	{
		DeNetwork.Server().At("/api/checkActiveLeaderboards").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericLeaderBoardResponse(response, callback, ParseLeaderboardEntryData);
			});
	}

	public void CheckActiveEventLeaderboards(Action<List<LeaderboardRewardResponse>, List<string>> callback)
	{
		DeNetwork.Server().At("/api/event/checkactiveleaderboards").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericLeaderBoardResponse(response, callback, ParseEventLeaderboardEntryData);
			});
	}

	public void CheckSoloLeaderboards(Action<List<LeaderboardRewardResponse>, List<string>> callback)
	{
		DeNetwork.Server().At("/api/event_single/checkactiveleaderboards").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				GenericLeaderBoardResponse(response, callback, ParseLeaderboardEntryData);
			});
	}

	private void GenericEntriesResponse<T>(TypedRestResponse<JsonObject> response, Action<List<T>, bool> cb, Func<JsonObject, T> ParseEntry)
	{
		JsonObject jsonObject = response.Resource().GetObject("result");
		List<T> list = new List<T>();
		if (jsonObject.Contains("error"))
		{
			string text = jsonObject.GetString("error");
			if (!(text == "UserKicked"))
			{
				PopupManager.ShowPopup(PopupDataModel.Ok("Error", text.Localize(text)));
				cb(null, false);
				return;
			}
			if (UserProfile.player != null && UserProfile.player.userClub != null)
			{
				UserProfile.player.clubID = string.Empty;
				UserProfile.player.userClub = null;
				PopupManager.ShowPopup(PopupDataModel.Ok("ui_popup_warning".Localize("Warning"), text.Localize(text)));
			}
		}
		List<JsonObject> objectList = jsonObject.GetObjectList("entries");
		if (objectList != null)
		{
			for (int i = 0; i < objectList.Count; i++)
			{
				JsonObject arg = objectList[i];
				T item = ParseEntry(arg);
				list.Add(item);
			}
		}
		if (cb != null)
		{
			cb(list, true);
		}
	}

	private LeaderboardRewardResponse ParseLeaderboardGeneric<T>(JsonObject rawEntries, Func<JsonObject, T> ParseEntry)
	{
		LeaderboardRewardResponse leaderboardRewardResponse = new LeaderboardRewardResponse();
		leaderboardRewardResponse.leaderboardId = rawEntries.GetString("leaderboard");
		foreach (object item in rawEntries.GetList("rewardPackageIds"))
		{
			leaderboardRewardResponse.rewardGiftPackageIds.Add(Convert.ToInt32(item));
		}
		leaderboardRewardResponse.finalRank = rawEntries.GetIntOrDefault("finalRank", -1);
		leaderboardRewardResponse.finalPoints = rawEntries.GetIntOrDefault("finalScore", -1);
		leaderboardRewardResponse.topRanked = new List<T>();
		if (rawEntries.Contains("top3Users"))
		{
			foreach (JsonObject @object in rawEntries.GetObjectList("top3Users"))
			{
				leaderboardRewardResponse.topRanked.Add(ParseEntry(@object));
			}
		}
		return leaderboardRewardResponse;
	}

	private void GenericLeaderBoardResponse<T>(TypedRestResponse<JsonObject> response, Action<List<LeaderboardRewardResponse>, List<string>> callback, Func<JsonObject, T> ParseEntry)
	{
		JsonObject jsonObject = response.Resource().GetObject("result");
		List<LeaderboardRewardResponse> list = new List<LeaderboardRewardResponse>();
		if (jsonObject.Contains("results"))
		{
			foreach (JsonObject @object in jsonObject.GetObjectList("results"))
			{
				list.Add(ParseLeaderboardGeneric(@object, ParseEntry));
			}
		}
		List<string> list2 = null;
		if (jsonObject.Contains("active_leaderboards"))
		{
			list2 = new List<string>();
			foreach (object item in jsonObject.GetList("active_leaderboards"))
			{
				list2.Add((string)item);
			}
		}
		if (jsonObject.Contains("newUnits"))
		{
			UserProfile.player.AddNewUnitsToInventory(jsonObject.GetObjectList("newUnits"));
		}
		callback(list, list2);
	}

	public void SyncLeaderboards(Action cb)
	{
	}

	public void LeaveClub(Action<bool> cb = null)
	{
		if (!LeaveClubLogic())
		{
			return;
		}
		parentCallback = cb;
		JsonObject o = new JsonObject();
		DeNetwork.Server().At("/api/clubs/leaveClub").Post(o)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				if (cb != null)
				{
					requestResult = jsonObject.GetBoolean("success");
					if (!requestResult)
					{
						string text = jsonObject.GetString("reason");
						PopupManager.ShowPopup(PopupDataModel.Ok("Error", "Reason: " + text.Localize(text), OnError).ShowCloseButton(false));
					}
					else
					{
						cb(requestResult);
					}
				}
			});
	}

	private bool LeaveClubLogic()
	{
		UserProfile player = UserProfile.player;
		if (player == null)
		{
			Log.Error("No player profile found");
			return false;
		}
		if (player.userClub == null)
		{
			Log.Error("Player is trying to leave a club without being part of any");
			return false;
		}
		if (!player.userClub.UserIsMember(player.id))
		{
			Log.Error("Player is trying to leave a club without being part of it");
			return false;
		}
		ClubMember clubMember = player.userClub.members.Find((ClubMember x) => x.ID == player.id);
		if (clubMember != null)
		{
			player.userClub.members.Remove(clubMember);
		}
		Reporting.LeaveClubEvent(player.id, player.clubID);
		player.userClub = null;
		player.clubID = string.Empty;
		return true;
	}

	private void OnError()
	{
		if (parentCallback != null)
		{
			parentCallback(requestResult);
		}
	}

	public void SendLocale(string locale, Action cb)
	{
		Debug.Log("Locale.SendLocale: " + locale);
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("locale", locale);
		DeNetwork.Server().At("/api/setlocale").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb();
				}
			});
	}

	public void GetAllPushNotifEnableStatus(Action<ServerUtilities.BaseResponse> callback = null)
	{
		DeNetwork.Server().At("/api/getAllPushNotifEnableStatus").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.TANKS_BUILD.ToString()))
				{
					UserProfile.player.preferences.NotifTanksBuild = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.TANKS_BUILD.ToString());
				}
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.TANKS_REPAIRED.ToString()))
				{
					UserProfile.player.preferences.NotifTanksRepaired = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.TANKS_REPAIRED.ToString());
				}
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.TICKET_RECHARGED.ToString()))
				{
					UserProfile.player.preferences.NotifTicketsRecharged = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.TICKET_RECHARGED.ToString());
				}
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.PRIZES_READY.ToString()))
				{
					UserProfile.player.preferences.NotifPrizesReady = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.PRIZES_READY.ToString());
				}
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.CLUB_CRATES.ToString()))
				{
					UserProfile.player.preferences.NotifClubCrates = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.CLUB_CRATES.ToString());
				}
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.EVENT_REWARDS.ToString()))
				{
					UserProfile.player.preferences.NotifEventRewards = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.EVENT_REWARDS.ToString());
				}
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.TEAM_REPORT_CARDS.ToString()))
				{
					UserProfile.player.preferences.NotifTeamReportCards = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.TEAM_REPORT_CARDS.ToString());
				}
				if (jsonObject.Contains(UserPreferenceData.PUSH_NOTIF_TYPE.ALL.ToString()))
				{
					UserProfile.player.preferences.NotifAll = jsonObject.GetBoolean(UserPreferenceData.PUSH_NOTIF_TYPE.ALL.ToString());
				}
			});
	}

	public void SetPushNotifEnableStatus(UserPreferenceData.PUSH_NOTIF_TYPE notifType, bool value, Action<ServerUtilities.BaseResponse> callback = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["notification_type"] = notifType.ToString();
		jsonObject["enabled"] = value;
		Singleton<SessionManager>.instance.DeNetwork.Server().At("/api/setPushNotifEnableStatus").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (callback != null)
				{
					callback(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	public void SubmitPartialUpgrade(string unitId, UnitPartialLevelDataModel partial, Action cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["unit_id"] = unitId;
		jsonObject["part_index"] = partial.partIndex;
		DeNetwork.Server().At("/api/submituserunitparts").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				int partialLevel = UserProfile.player.unitInventory[unitId].partialLevel | (1 << partial.partIndex);
				for (int i = 0; i < UserProfile.player.CurrentTeam.units.Count; i++)
				{
					if (UserProfile.player.CurrentTeam.units[i] != null && UserProfile.player.CurrentTeam.units[i].id == unitId)
					{
						UserProfile.player.CurrentTeam.units[i].partialLevel = partialLevel;
					}
				}
				UserProfile.player.unitInventory[unitId].partialLevel = partialLevel;
				UserProfile.player.PayPrice(ItemPriceDataModel.GetPriceForID(partial.requirementPriceId));
				if (cb != null)
				{
					cb();
				}
			});
	}

	public void PrizeWheelReward(Action cb)
	{
		JsonObject o = new JsonObject();
		DeNetwork.Server().At("/api/spinprizewheel").Post(o)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject = response.Resource().GetObject("result");
				UserProfile.player.inventory.AddItem(UserInventory.ItemType.Coins, null, jsonObject.GetInt("4"));
				if (cb != null)
				{
					cb();
				}
			});
	}

	public void PurchaseSkin(UnitLevelProgressionDataModel skinToPurchase, UserPriceDataModel.PaymentType paymentType, Action<bool> cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["skin_id"] = skinToPurchase.id;
		jsonObject["use_premium"] = paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference;
		PurchaseSkinLogic(skinToPurchase, paymentType);
		DeNetwork.Server().At("/api/purchaseskin").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb(true);
				}
			});
	}

	private bool PurchaseSkinLogic(UnitLevelProgressionDataModel skinToPurchase, UserPriceDataModel.PaymentType paymentType)
	{
		if (UserProfile.player.HasUnlockedSkin(skinToPurchase.id))
		{
			Log.Error("Someone is trying to purchase a skin that is already purchased", base.gameObject);
			return false;
		}
		UserPriceDataModel userPriceDataModel = ItemPriceDataModel.GetPriceForID(skinToPurchase.priceId);
		if (paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference)
		{
			userPriceDataModel = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, UserProfile.player, Constants.SkinGemToCashConversion);
		}
		if (!UserProfile.player.CanAfford(userPriceDataModel))
		{
			return false;
		}
		UserProfile.player.PayPrice(userPriceDataModel);
		Reporting.CurrencyTransactionEvent(PurchaseSource.SkinPurchase, userPriceDataModel);
		UserProfile.player.AddSkin(skinToPurchase);
		return true;
	}

	public void ReceiveActions(string matchId, int roundId, ServerTeamState forTeam, Action<ServerUtilities.ReceiveActionsResponse> cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["match_id"] = matchId;
		jsonObject["round_id"] = roundId;
		DeNetwork.Server().At("/api/receiveActions").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				ServerUtilities.ReceiveActionsResponse receiveActionsResponse = new ServerUtilities.ReceiveActionsResponse
				{
					success = jsonObject2.GetBoolean("success")
				};
				if (receiveActionsResponse.success)
				{
					List<BattleAction> list = new List<BattleAction>();
					foreach (JsonObject @object in jsonObject2.GetObjectList("actions"))
					{
						list.Add(BattleAction.DeserializeAction(@object));
					}
					receiveActionsResponse.actions = list;
				}
				cb(receiveActionsResponse);
			});
	}

	public void StartResearch(UserResearcher.ResearchType researchType, string itemID, UserResearcher researcher, UserPriceDataModel.PaymentType paymentType, Action<ServerUtilities.BaseResponse> callback = null)
	{
		if (!StartResearchValidation(researchType, itemID))
		{
			return;
		}
		StartResearchLogic(researchType, itemID, researcher, paymentType);
		Reporting.ResearchStartEvent(researcher);
		JsonObject jsonObject = new JsonObject();
		jsonObject["research_type"] = researchType.ToString();
		jsonObject["item_id"] = itemID;
		jsonObject["researcher_index"] = UserProfile.player.researchers.IndexOf(researcher);
		jsonObject["gem_price"] = paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference;
		DeNetwork.Server().At("/api/startresearch").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (callback != null)
				{
					callback(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	public void SkipResearch(UserResearcher researcher, long currentTime, Action<ServerUtilities.BaseResponse> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["researcher_index"] = UserProfile.player.researchers.IndexOf(researcher);
		jsonObject["time_units"] = researcher.GetHurryTimeUnits(currentTime);
		SkipResearchLogic(researcher, currentTime);
		DeNetwork.Server().At("/api/skipresearch").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					cb(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	public void ClaimResearch(UserResearcher researcher, Action<ServerUtilities.BaseResponse> callback)
	{
		UserResearcher.ResearchType researchType = researcher.researchType;
		string tempUnitID = null;
		UserUnit unit;
		if (researchType == UserResearcher.ResearchType.BuildTank)
		{
			tempUnitID = researcher.itemID + TimeManager.ServerTime;
			unit = new UserUnit(tempUnitID, researcher.itemID, 1, 0, string.Empty, string.Empty);
			UserProfile.player.unitInventory.Add(unit.id, unit);
			UserProfile.player.BuiltUnit(unit.metadataId);
		}
		ClaimResearchLogic(researcher);
		JsonObject jsonObject = new JsonObject();
		jsonObject["researcher_index"] = UserProfile.player.researchers.IndexOf(researcher);
		DeNetwork.Server().At("/api/claimresearch").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (researchType == UserResearcher.ResearchType.BuildTank)
				{
					UserProfile player = UserProfile.player;
					player.unitInventory.Remove(tempUnitID);
					unit = UserUnit.FromJSON(jsonObject2.GetObject("unit"));
					player.unitInventory.Add(unit.id, unit);
					Reporting.ResearchCompleteEvent(researcher, unit);
					UserTeam userTeam = player.teams[0];
					userTeam.TryInsertUnit(unit);
				}
				if (callback != null)
				{
					callback(new ServerUtilities.BaseResponse(Session, jsonObject2));
				}
			});
	}

	private void StartResearchLogic(UserResearcher.ResearchType researchType, string itemID, UserResearcher researcher, UserPriceDataModel.PaymentType paymentType)
	{
		researcher.StartResearch(researchType, itemID);
		UserPriceDataModel userPriceDataModel = researcher.ResearchItem.GetResearchCost(UserProfile.player);
		if (paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference)
		{
			userPriceDataModel = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, UserProfile.player, Constants.CashToGemRateMaxUpgrade);
		}
		UserProfile.player.PayPrice(userPriceDataModel);
		int researchStartReward = Constants.ResearchStartReward;
		if (researchStartReward != 0 && !UserProfile.player.HasBuiltUnit(itemID))
		{
			ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(researchStartReward);
			UserProfile.player.AddItems(giftPackage);
		}
	}

	private bool StartResearchValidation(UserResearcher.ResearchType researchType, string itemID)
	{
		if (researchType == UserResearcher.ResearchType.BuildTank)
		{
			UnitDataModel single = UnitDataModel.GetSingle(itemID);
			if (single.UnitType.IsEvent())
			{
				EventDataModel onCooldownEvent = EventDataModel.GetOnCooldownEvent();
				if (onCooldownEvent == null)
				{
					PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("ERROR"), "ui_buildpopup_event_unit_error".Localize("An Event Unit cannot be built if the Event is not active")));
					return false;
				}
				if (!onCooldownEvent.UnitBelongsToEvent(single.id))
				{
					PopupManager.ShowPopup(PopupDataModel.Ok("ui_mobage_transaction_error_title".Localize("ERROR"), "ui_buildpopup_event_unit_error".Localize("An Event Unit cannot be built if the Event is not active")));
					return false;
				}
			}
		}
		return true;
	}

	private void SkipResearchLogic(UserResearcher researcher, long currentTime)
	{
		UserPriceDataModel hurryCost = researcher.GetHurryCost(currentTime);
		UserProfile.player.PayPrice(hurryCost);
		Reporting.CurrencyTransactionEvent(PurchaseSource.SkipResearchPopup, hurryCost);
		Reporting.ResearchSkippedEvent(researcher);
		researcher.SkipResearch();
	}

	private void ClaimResearchLogic(UserResearcher researcher)
	{
		string itemID = researcher.itemID;
		researcher.ClaimResearch();
		int researchClaimReward = Constants.ResearchClaimReward;
		if (researchClaimReward != 0 && UserProfile.player.TimesBuiltUnit(itemID) == 1)
		{
			ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(researchClaimReward);
			UserProfile.player.AddItems(giftPackage);
		}
	}

	public void ResetBotBattleDailyCount(UserPriceDataModel price)
	{
		JsonObject o = new JsonObject();
		ResetBotBattleDailyCountLogic(price);
		DeNetwork.Server().At("/api/resetBotBattleDailyCount").Post(o)
			.Response<JsonObject>(delegate
			{
			});
	}

	private void ResetBotBattleDailyCountLogic(UserPriceDataModel price)
	{
		UserProfile.player.PayPrice(price);
		UserProfile.player.botBattleCount = 0;
		UserProfile.player.botBattleRestoreCount++;
	}

	public bool SaveDialogTrigger(int triggerID)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["trigger_id"] = triggerID;
		DeNetwork.Server().At("/api/savedialogtrigger").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
			});
		return true;
	}

	public bool ScrapUnit(UserUnit unitToScrap, Action<bool> cb)
	{
		if (!ScrapUnitLogic(unitToScrap))
		{
			cb(false);
			return false;
		}
		JsonObject jsonObject = new JsonObject();
		jsonObject["unit_id"] = unitToScrap.ID;
		DeNetwork.Server().At("/api/scrapuserunit").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb(true);
				}
			});
		return true;
	}

	private bool ScrapUnitLogic(UserUnit unitToScrap)
	{
		UserProfile player = UserProfile.player;
		if (!player.unitInventory.ContainsKey(unitToScrap.ID))
		{
			Log.Error("User is trying to scrap a unit which is not in his inventory");
			return false;
		}
		ItemCollectionDataModel scrap = unitToScrap.GetScrap();
		player.AddItems(scrap);
		UserTeam team = unitToScrap.Team;
		if (team != null)
		{
			if (team.IsOnCooldown)
			{
				Log.Error("User is trying to scrap a unit on Cooldown");
				return false;
			}
			team.units.Remove(unitToScrap);
			team.units.Add(null);
		}
		player.unitInventory.Remove(unitToScrap.ID);
		return true;
	}

	public void SearchClub(string clubName = "", Action<List<UserClub>> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		if (!string.IsNullOrEmpty(clubName))
		{
			jsonObject.SetString("term", clubName);
		}
		DeNetwork.Server().At("/api/clubs/searchClub").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					cb(UserClub.FromJSONList(response.Resource().GetObject("result")));
				}
			});
	}

	public void SendActions(string matchId, int roundId, List<BattleAction> actions, Action<TypedRestResponse<JsonObject>> cb)
	{
		JsonObject jsonObject = new JsonObject();
		List<JsonObject> list = new List<JsonObject>();
		foreach (BattleAction action in actions)
		{
			list.Add(action.Serialize());
		}
		jsonObject.SetInt("round_id", roundId);
		jsonObject.SetString("match_id", matchId);
		jsonObject.SetObjectList("action", list);
		DeNetwork.Server().At("/api/sendActions").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				cb(response);
			});
	}

	private void SetConnectedState(ConnectedState connectedState)
	{
		if (connectedState != _connectedState)
		{
			_connectedState = connectedState;
			if (this.ConnectionEvent != null)
			{
				this.ConnectionEvent(connectedState);
			}
		}
	}

	public void Disconnect()
	{
		SetConnectedState(ConnectedState.Offline);
	}

	public void Init()
	{
		ClientLogging("call Init()");
		Connect(null);
	}

	public void Connect(Action cb, int connectionAttempt = 0)
	{
		Log.DebugTag("Connect", null, "SessionManager");
		if (connectionAttempt > 5)
		{
			Log.Error("Max login attempts reached");
			return;
		}
		ClientLogging("call Connect()");
		SetConnectedState(ConnectedState.Connecting);
		UserProfileManager.UpdatedCallback userProfileUpdatedHandler = null;
		userProfileUpdatedHandler = delegate
		{
			Singleton<UserProfileManager>.instance.Updated -= userProfileUpdatedHandler;
		};
		Singleton<UserProfileManager>.instance.Updated += userProfileUpdatedHandler;
		if (network == null)
		{
			SetupDeNetwork();
		}
		Log.DebugTag("StartLoginProcess", null, "SessionManager");
		LoginController.StartLoginProcess(delegate
		{
			Reporting.StartupFunnelEvent(InitializationManager.StartupStep.Login);
			CheckAndUpdateDataModel(delegate
			{
				Singleton<BankService>.instance.GetCurrencyBalance(delegate(int balance)
				{
					UserProfile.player.gems = balance;
					_connectedState = ConnectedState.Connected;
					if (cb != null)
					{
						cb();
					}
				});
			});
		});
	}

	private void CheckAndUpdateDataModel(Action done)
	{
		if (!AppConfig.keepLocalDataModelChanges)
		{
			bool flag = Singleton<InitializationManager>.instance.dataModelVersion != Session.dataModelVersion;
			bool flag2 = Singleton<InitializationManager>.instance.dataModelHash != Session.dataModelHash;
			Reporting.StartupFunnelEvent(InitializationManager.StartupStep.UpdateDataModel);
			if (flag2 || flag)
			{
				UpdateDataModelFile(done);
				Log.DebugTag("SessionManager.UpdateDataModelFileDone", null, "SessionManager");
				return;
			}
		}
		else
		{
			Log.WarningTag("USING POSSIBLY MODIFIED STATIC DATA", null, "SessionManager");
		}
		done();
	}

	private void SetupDeNetwork()
	{
		network = new DeNetwork(1).SetupServer(AppConfig.Server, "/OAuth/request_temp_token", "/OAuth/req_token").SetupAssetBundles(PERSISTENT_DATA_PATH, Singleton<InitializationManager>.instance.dataModelAssetUrl, new ThreadPoolManager(2));
		network.Server().SetDefaultRetries(3);
		network.Server().SetErrorHandler(new ErrorHandler());
		network.Assets().SetMediaType("text/plain", new BinaryDownloadMediaTypeNoIcloud(PERSISTENT_DATA_PATH));
		network.Server().SetMediaType("application/json", new SignedLitJsonMediaType());
		network.Server().BeforeRequest(delegate(RestRequest request)
		{
			request.Header("Accept-Encoding", "gzip");
			request.Header("superbattletactics-client-ver", AppConfig.clientVersion);
			request.Header("superbattletactics-ndk-ver", AppConfig.ndkVersion);
			request.Header("ngserver-platform", AppConfig.platform.ToString());
			request.Header("superbattletactics-client-ver", AppConfig.clientVersion);
			request.Header("superbattletactics-ndk-ver", AppConfig.ndkVersion);
			request.Header("ngserver-platform", AppConfig.platform.ToString());
			request.Header("lcd-token", Singleton<LCDController>.instance.AccessToken);
			Log.DebugTag("LCD-TOKEN " + Singleton<LCDController>.instance.AccessToken, null, "SessionManager");
			if (Session != null)
			{
				request.Header("superbattletactics-config-ver", Session.dataModelVersion.ToString());
				request.Header("superbattletactics-config-hash", Session.dataModelHash);
				Interlocked.Increment(ref requestId);
				request.Header("superbattletactics-request-id", TimeManager.ServerTime.ToString() + requestId);
			}
		});
		SetupHTTPErrorMiddleware();
		SetupUserMiddleware();
		SetupSessionMiddleware();
		SetupErrorMiddleware();
		network.Server().OnNetworkLost(delegate
		{
			_loadingPopupNetworkConnectionId = LoadingPopupManager.ShowLoadingPopup(0f);
		});
		network.Server().OnNetworkRecovered(delegate
		{
			LoadingPopupManager.ClearLoadingPopup(_loadingPopupNetworkConnectionId);
		});
	}

	private void SetupHTTPErrorMiddleware()
	{
		network.Server().AfterResponse(delegate(RestResponse response)
		{
			if (response.Exception != null)
			{
				response.ThreadPoolManager.RunOnMainThread(delegate
				{
					try
					{
						CrittercismUtil.LogHandledException(response.Exception);
					}
					catch (Exception ex)
					{
						Log.ErrorTag("Something went wrong when reporting network error to crittercism. Error: " + ex.Message, null, "SessionManager");
					}
				});
			}
			if (IsErrorCode(response.StatusCode) && response.StatusCode != 401)
			{
				Log.ErrorTag(string.Format("Server response had errors, statusCode {0}", response.StatusCode.ToString()), null, "SessionManager");
				response.Error = "HTTPError";
			}
		});
	}

	private bool IsErrorCode(int code)
	{
		if (code >= 100 && code < 200)
		{
			return false;
		}
		if (code >= 200 && code < 300)
		{
			return false;
		}
		if (code >= 300 && code < 400)
		{
			return false;
		}
		if (code >= 400 && code < 500)
		{
			return true;
		}
		if (code >= 500 && code < 600)
		{
			return true;
		}
		return false;
	}

	private void SetupUserMiddleware()
	{
		network.Server().AfterResourceOfType(delegate(TypedRestResponse<JsonObject> response)
		{
			response.ThreadPoolManager.RunOnMainThread(delegate
			{
				JsonObject jsonObject = response.Resource();
				JsonObject jsonObject2 = jsonObject.GetObject("result");
				JsonObject jsonObject3 = jsonObject.GetObject("user");
				if (jsonObject3 != null)
				{
					if (UserProfile.player == null)
					{
						Singleton<UserProfileManager>.instance.UpdateUserProfile(jsonObject3, delegate
						{
							Log.Debug("Setting profile");
						});
					}
					else
					{
						Singleton<UserProfileManager>.instance.UpdateUserProfileServerData(jsonObject3, UserProfile.player);
						Singleton<UserProfileManager>.instance.SyncCheck(UserProfileManager.ParseUserProfile(jsonObject3));
					}
				}
				if (jsonObject2 != null && jsonObject2.Contains("asc_giftcode"))
				{
					string giftCode = jsonObject2.GetString("asc_giftcode");
					Singleton<BankService>.instance.GiftASC(giftCode, delegate
					{
						Singleton<BankService>.instance.GetCurrencyBalance(delegate(int balance)
						{
							UserProfile.player.gems = balance;
						});
					});
				}
			});
		});
	}

	private void SetupSessionMiddleware()
	{
		network.Server().AfterResourceOfType(delegate(TypedRestResponse<JsonObject> response)
		{
			JsonObject jsonObject = response.Resource();
			if (jsonObject.Contains("current_version") && jsonObject.Contains("current_hash") && jsonObject.Contains("server_ts"))
			{
				ServerUtilities.Session session = new ServerUtilities.Session
				{
					dataModelVersion = jsonObject.GetInt("current_version"),
					dataModelHash = jsonObject.GetString("current_hash"),
					serverTime = jsonObject.GetLong("server_ts")
				};
				TimeManager.ServerTime = jsonObject.GetLong("server_ts");
				if (jsonObject.Contains("client_md_url"))
				{
					session.dataModelFullUrl = jsonObject.GetString("client_md_url");
				}
				Session = session;
			}
		});
	}

	private void SetupErrorMiddleware()
	{
		network.Server().AfterResourceOfType(delegate(TypedRestResponse<JsonObject> response)
		{
			JsonObject jsonObject = response.Resource();
			if (jsonObject != null && !jsonObject.GetBoolean("success"))
			{
				JsonObject jsonObject2 = jsonObject.GetObject("reason");
				if (jsonObject2 != null)
				{
					response.Error = jsonObject2.GetString("code");
					Log.Error("Resposne Error: {0}, {1}", response.Error, response.Body);
				}
			}
		});
	}

	public void UpdateAssetBundleHost(string host)
	{
		if (network != null)
		{
			network.Assets().Host(host);
		}
	}

	private void UpdateDataModelFile(Action action)
	{
		Log.Debug("SessionManager.UpdateDataModelFile");
		int retries = AppConfig.networkRetries;
		DataModelFile.DownloadCallback cb = delegate(string error)
		{
			if (error != null)
			{
				if (--retries >= 0)
				{
					Log.Warning("UpdateDataModelFile failed, will retry {0} more times. Error: {1}", retries + 1, error);
					Singleton<DataModelFile>.instance.DownloadDynamic(Session.dataModelFullUrl, cb);
				}
				else
				{
					Log.Error("UpdateDataModelFile failed: " + error);
					action();
				}
			}
			else
			{
				Log.Debug("SessionManager.UpdateDataModelFileDone");
				ExecuteAfterCoroutine(Singleton<InitializationManager>.instance.DataModelConnect(), action);
			}
		};
		Singleton<DataModelFile>.instance.DownloadDynamic(Session.dataModelFullUrl, cb);
	}

	public void UpdateAbilities(Action<TypedRestResponse<JsonObject>> cb = null)
	{
		List<JsonObject> list = new List<JsonObject>();
		foreach (UserAbilitySet userAbilitySet in UserProfile.player.userAbilitySets)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject.SetList("abilities", userAbilitySet.abilities.ToArray());
			list.Add(jsonObject);
		}
		JsonObject jsonObject2 = new JsonObject();
		jsonObject2.SetObjectList("ability_sets", list);
		DeNetwork.Server().At("/api/setuserabilities").Post(jsonObject2)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					cb(response);
				}
			});
	}

	public void RecordBattleResult(MatchData matchData, ServerTeamState playerTeam, Action<TypedRestResponse<JsonObject>> cb)
	{
		RecordBattleResultLogic(matchData, playerTeam.stats);
		JsonObject jsonObject = new JsonObject();
		jsonObject["battle_result"] = BattleOutputUtils.CreateJsonBattleResult(playerTeam);
		jsonObject.SetString("match_id", matchData.matchId);
		for (int i = 0; i < playerTeam.stats.giftDrops.Count; i++)
		{
			UserProfile.player.AddItems(ItemGiftDataModel.GetGiftPackage(playerTeam.stats.giftDrops[i]));
		}
		DeNetwork.Server().At("/api/battleResult").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (cb != null)
				{
					cb(response);
				}
			});
	}

	private void RecordBattleResultLogic(MatchData matchData, ServerTeamStatsState playerStats)
	{
		UserProfile player = UserProfile.player;
		player.coins += playerStats.coinsEarned;
		player.gems += playerStats.gemsEarned;
		player.unitsKilled += playerStats.unitsDestroyed;
		foreach (IPartMetadata item in playerStats.partsEarned)
		{
			player.inventory.AddParts(item.ID, 1);
		}
		player.totalBattles++;
		if (playerStats.isWinner)
		{
			player.wins++;
		}
		else
		{
			player.losses++;
			player.CurrentTeam.cooldownFinishTime = TimeManager.ServerTime + player.CurrentTeam.CooldownLength * 1000;
		}
		if (player.IsInPromoSeries)
		{
			if (matchData.type == MatchData.Type.PVP)
			{
				if (playerStats.isWinner)
				{
					player.promoSeriesWins++;
				}
				else
				{
					player.promoSeriesLosses++;
				}
				int maxLosses = player.PromoSeries.MaxLosses;
				if (player.promoSeriesWins >= player.PromoSeries.totalWin)
				{
					player.AdvanceDivision();
				}
				else if (player.promoSeriesLosses >= maxLosses)
				{
					player.FailPromoSeries();
				}
			}
			return;
		}
		player.points += playerStats.pointsEarned;
		if (player.points < player.CurrentDivision.totalPointToPromotionSeries)
		{
			return;
		}
		string id = player.CurrentDivision.promotionSeriesId.ToString();
		ProgressionPromotionSeriesDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionPromotionSeriesDataModel>(id);
		if (single != null)
		{
			if (single.totalWin > 0)
			{
				if (player.promoSeriesLastId != single.id && Constants.PromoSeriesStartReward != 0)
				{
					ItemCollectionDataModel giftPackage = ItemGiftDataModel.GetGiftPackage(Constants.PromoSeriesStartReward);
					UserProfile.player.AddItems(giftPackage);
				}
				player.BeginPromoSeries(single);
			}
			else
			{
				player.AdvanceToDivision(single.promotionDivisionId.ToString());
				TopBarController.instance.UpdateNotifications();
			}
		}
		else
		{
			player.ResetDivision();
		}
	}

	public void UpdateTeam(Action<TypedRestResponse<JsonObject>> cb = null)
	{
		List<JsonObject> list = new List<JsonObject>();
		foreach (UserTeam team in UserProfile.player.teams)
		{
			JsonObject jsonObject = new JsonObject();
			jsonObject.SetList("units", team.GetUnitIDs());
			list.Add(jsonObject);
		}
		JsonObject jsonObject2 = new JsonObject();
		jsonObject2["current_team_index"] = UserProfile.player.currentTeamIndex;
		jsonObject2.SetObjectList("teams", list);
		JsonObject jsonObject3 = new JsonObject();
		jsonObject3["team_details"] = jsonObject2;
		DeNetwork.Server().At("/api/setuserteams").Post(jsonObject3)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					cb(response);
				}
			});
	}

	public void SetTutorialStep(int levelToSet, Action<RestResponse> cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["next_step"] = levelToSet;
		DeNetwork.Server().At("/api/setTutorialStep").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					cb(response);
				}
			});
	}

	public void SetUnitSkin(UserUnit unitToChange, UnitLevelProgressionDataModel skinToSet, Action<ServerUtilities.BaseResponse> cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["unit_id"] = unitToChange.id;
		jsonObject["skin_id"] = skinToSet.id;
		DeNetwork.Server().At("/api/setunitskin").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					cb(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	public void SkipRepairTime(int teamIndex, long timestamp, UserPriceDataModel.PaymentType paymentType, Action cb = null)
	{
		UserTeam userTeam = UserProfile.player.teams[teamIndex];
		JsonObject jsonObject = new JsonObject();
		jsonObject["team_index"] = teamIndex;
		jsonObject["time_units"] = userTeam.GetCooldownTimeUnits(timestamp);
		jsonObject["use_premium"] = paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference;
		SkipRepairTimeLogic(userTeam, timestamp, paymentType);
		DeNetwork.Server().At("/api/skiprepairtime").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb();
				}
			});
	}

	private void SkipRepairTimeLogic(UserTeam team, long timestamp, UserPriceDataModel.PaymentType paymentType)
	{
		UserPriceDataModel userPriceDataModel = team.GetPriceToSkipRepair(timestamp);
		if (paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference)
		{
			userPriceDataModel = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, UserProfile.player, Constants.CooldownGemToCashExchangeRate);
		}
		UserProfile.player.PayPrice(userPriceDataModel);
		team.cooldownFinishTime = TimeManager.ServerTime;
	}

	public void SendFacebookCredentials(string fbUserId, string fbAccessToken, Action<ServerUtilities.BaseResponse, JsonObject> cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["id"] = fbUserId;
		jsonObject["token"] = fbAccessToken;
		Debug.Log("SocialSession.SendFacebookCredentials");
		DeNetwork.Server().At("/user/setfbcredentials").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (cb != null)
				{
					ServerUtilities.BaseResponse baseResponse = new ServerUtilities.BaseResponse(Session, response.Resource());
					cb(baseResponse, baseResponse.json.GetObject("result"));
				}
			});
	}

	public void UpdateEventReports(string eventId, Action<ServerMultiteamReport> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["event_id"] = eventId;
		DeNetwork.Server().At("/api/event/updateEventReport").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				ServerMultiteamReport serverMultiteamReport = new ServerMultiteamReport();
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				if (jsonObject2.Contains("report"))
				{
					serverMultiteamReport.userMultiTeamReport = UserMultiTeamReport.FromJSON(jsonObject2.GetObject("report"));
				}
				if (jsonObject2.Contains("lastClaimed"))
				{
					serverMultiteamReport.claimedReportData = ClaimedReportData.FromJSON(jsonObject2.GetObject("lastClaimed"));
				}
				if (cb != null)
				{
					cb(serverMultiteamReport);
				}
			});
	}

	public void GetEventReport(string eventId, Action<ServerMultiteamReport> cb = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["event_id"] = eventId;
		DeNetwork.Server().At("/api/event/getEventReport").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				JsonObject jsonObject2 = response.Resource().GetObject("result");
				ServerMultiteamReport serverMultiteamReport = new ServerMultiteamReport();
				if (jsonObject2 != null && jsonObject2.Contains("report"))
				{
					serverMultiteamReport.userMultiTeamReport = UserMultiTeamReport.FromJSON(jsonObject2.GetObject("report"));
				}
				if (cb != null)
				{
					cb(serverMultiteamReport);
				}
			});
	}

	public void UpgradeUnit(UserUnit unitToUpgrade, UserPriceDataModel.PaymentType paymentType, Action<bool> cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["unit_id"] = unitToUpgrade.ID;
		jsonObject["use_premium"] = paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference;
		UpgradeUnitLogic(unitToUpgrade, paymentType);
		DeNetwork.Server().At("/api/upgradeuserunit").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb(true);
				}
			});
	}

	private bool UpgradeUnitLogic(UserUnit unitToUpgrade, UserPriceDataModel.PaymentType paymentType)
	{
		if (unitToUpgrade.IsMaxLevel)
		{
			Log.Error("Someone is trying to promote a unit which is already in MAX LEVEL", base.gameObject);
			return false;
		}
		UserPriceDataModel userPriceDataModel = unitToUpgrade.GetUpgradePrice();
		if (paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference)
		{
			userPriceDataModel = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, UserProfile.player, unitToUpgrade.GetUpgradeGemExchangeRate());
		}
		if (!UserProfile.player.CanAfford(userPriceDataModel))
		{
			return false;
		}
		UserProfile.player.PayPrice(userPriceDataModel);
		Reporting.CurrencyTransactionEvent(PurchaseSource.UnitPromotionPopup, userPriceDataModel);
		unitToUpgrade.IncreaseLevel();
		ItemCollectionDataModel itemCollectionDataModel = ((unitToUpgrade.CurrentLevelDataModel.onLevelGiftId == -1) ? null : ItemGiftDataModel.GetGiftPackage(unitToUpgrade.CurrentLevelDataModel.onLevelGiftId));
		string gifts = string.Empty;
		if (itemCollectionDataModel != null)
		{
			UserProfile.player.AddItems(itemCollectionDataModel);
			gifts = itemCollectionDataModel.PrintItems();
		}
		Reporting.TankLevelUpEvent(unitToUpgrade.metadataId, unitToUpgrade.level, userPriceDataModel.PrintItems(), gifts);
		return true;
	}

	public void UpgradeUnitMax(UserUnit unitToUpgrade, UserPriceDataModel.PaymentType paymentType, Action cb)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["unit_id"] = unitToUpgrade.ID;
		jsonObject["use_premium"] = paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference;
		UpgradeUnitMaxLogic(unitToUpgrade, paymentType);
		DeNetwork.Server().At("/api/upgradeuserunitMax").Post(jsonObject)
			.Response<JsonObject>(delegate
			{
				if (cb != null)
				{
					cb();
				}
			});
	}

	private bool UpgradeUnitMaxLogic(UserUnit unitToUpgrade, UserPriceDataModel.PaymentType paymentType)
	{
		if (unitToUpgrade.IsMaxLevel)
		{
			Log.Error("Someone is trying to promote a unit which is already in MAX LEVEL", base.gameObject);
			return false;
		}
		UserPriceDataModel userPriceDataModel = unitToUpgrade.GetMaxUpgradePrice();
		if (paymentType == UserPriceDataModel.PaymentType.UsePremiumForDifference)
		{
			userPriceDataModel = UserPriceDataModel.GetPremiumPrice(userPriceDataModel, UserProfile.player, Constants.CashToGemRateMaxUpgrade);
		}
		if (!UserProfile.player.CanAfford(userPriceDataModel))
		{
			Log.Error("Can't afford Max Price " + UserProfile.player.mobageId);
			return false;
		}
		UserProfile.player.PayPrice(userPriceDataModel);
		Reporting.CurrencyTransactionEvent(PurchaseSource.UnitMaxPromotionPopup, userPriceDataModel);
		unitToUpgrade.IncreaseMaxCashLevel();
		ItemCollectionDataModel itemCollectionDataModel = ((unitToUpgrade.CurrentLevelDataModel.onLevelGiftId == -1) ? null : ItemGiftDataModel.GetGiftPackage(unitToUpgrade.CurrentLevelDataModel.onLevelGiftId));
		string gifts = string.Empty;
		if (itemCollectionDataModel != null)
		{
			gifts = itemCollectionDataModel.PrintItems();
			UserProfile.player.AddItems(itemCollectionDataModel);
		}
		Reporting.TankLevelUpEvent(unitToUpgrade.metadataId, unitToUpgrade.level, userPriceDataModel.PrintItems(), gifts);
		return true;
	}

	public void useLocalSQLite(Action cb = null)
	{
	}

	public void CheckUsername(string userName, Action<ServerUtilities.BaseResponse> callback = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["username"] = userName;
		DeNetwork.Server().At("/api/checkUserName").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (callback != null)
				{
					callback(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	public void SetUsername(string userName, Action<ServerUtilities.BaseResponse> callback = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["username"] = userName;
		DeNetwork.Server().At("/api/setusername").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				if (callback != null)
				{
					callback(new ServerUtilities.BaseResponse(Session, response.Resource()));
				}
			});
	}

	public void PostLoginTasks(Action<ServerUtilities.BaseResponse> callback = null)
	{
		DeNetwork.Server().At("/api/postlogintasks").Post()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				Log.DebugTag("Post login info " + response.Resource().ToJson(), null, "PostLogin");
				JsonObject jsonObject = response.Resource().GetObject("result");
				if (jsonObject != null)
				{
					Log.Debug("User.PostLoginTasks >>> success: " + jsonObject["success"]);
				}
				if (jsonObject.Contains("did_update") && jsonObject.GetBoolean("did_update"))
				{
					Log.Debug("User.PostLoginTasks updated, updating balance");
					Singleton<BankService>.instance.GetCurrencyBalance(delegate(int balance)
					{
						UserProfile.player.gems = balance;
					});
				}
				if (callback == null)
				{
				}
			});
	}

	public void LoginLCD(string id, string fbToken, Action<ServerUtilities.BaseResponse> callback = null)
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject["_id"] = id;
		jsonObject["fbToken"] = fbToken;
		DeNetwork.Server().At("/api/lcd/login").Post(jsonObject)
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				ServerUtilities.BaseResponse obj = null;
				if (response != null)
				{
					if (response.Error != null)
					{
						Log.ErrorTag((string)response.Error, null, "LCD Error");
						UINetworkErrorHandler.DisplayMessage(response);
					}
					else
					{
						obj = new ServerUtilities.BaseResponse(Session, response.Resource());
					}
				}
				if (callback != null)
				{
					callback(obj);
				}
			});
	}

	public void PreLoginLCD(string id, Action<ServerUtilities.BaseResponse> callback = null)
	{
		DeNetwork.Server().At("/api/lcd/check_user").Get()
			.Response(delegate(TypedRestResponse<JsonObject> response)
			{
				ServerUtilities.BaseResponse obj = null;
				if (response != null)
				{
					if (response.Error != null)
					{
						Log.ErrorTag((string)response.Error, null, "LCD Error");
						UINetworkErrorHandler.DisplayMessage(response);
					}
					else
					{
						obj = new ServerUtilities.BaseResponse(Session, response.Resource());
					}
				}
				if (callback != null)
				{
					callback(obj);
				}
			});
	}
}
