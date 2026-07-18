using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class DataModelAccess : AccessBase
{
	public delegate Error GetAllDelegate<T>(out List<T> list);

	private const string ASSET_BUNDLE_SELECT_SQL = "SELECT id,priority,iphone_hash,android_hash,osxeditor_hash FROM ASSET_BUNDLES ";

	private const string CONFIG_SELECT_SQL = "SELECT KEY, VALUE FROM CONFIG";

	private const string TUTORIAL_KEY_VALUE_SELECT_SQL = "SELECT key,value FROM TUTORIAL_KEY_VALUE ";

	private static Dictionary<Type, object> TYPE_TO_READER;

	private static DataModelReader<AbilityDataModel> ABILITY_GENERATED_READER;

	private static DataModelReader<AbilityHandlerDataModel> ABILITY_HANDLER_GENERATED_READER;

	private static DataModelReader<AbilityTypeDataModel> ABILITY_TYPE_GENERATED_READER;

	private static DataModelReader<AiArmyDataModel> AI_ARMY_GENERATED_READER;

	private static DataModelReader<AiArmyPartsDataModel> AI_ARMY_PARTS_GENERATED_READER;

	private static DataModelReader<AiHandlerDataModel> AI_HANDLER_GENERATED_READER;

	private static DataModelReader<AnnouncerDialogSequencesDataModel> ANNOUNCER_DIALOG_SEQUENCES_GENERATED_READER;

	private static DataModelReader<AssetBundlesDataModel> ASSET_BUNDLES_GENERATED_READER;

	private static DataModelReader<AssetLinkageDataModel> ASSET_LINKAGE_GENERATED_READER;

	private static DataModelReader<AudioTriggersDataModel> AUDIO_TRIGGERS_GENERATED_READER;

	private static DataModelReader<BoostAbilityMultiplierDataModel> BOOST_ABILITY_MULTIPLIER_GENERATED_READER;

	private static DataModelReader<BoostDataModel> BOOST_GENERATED_READER;

	private static DataModelReader<ConfigDataModel> CONFIG_GENERATED_READER;

	private static DataModelReader<ConstantDataModel> CONSTANT_GENERATED_READER;

	private static DataModelReader<ContractDetailsDataModel> CONTRACT_DETAILS_GENERATED_READER;

	private static DataModelReader<ContractsDataModel> CONTRACTS_GENERATED_READER;

	private static DataModelReader<DialogScreenDataModel> DIALOG_SCREEN_GENERATED_READER;

	private static DataModelReader<EffectsDataModel> EFFECTS_GENERATED_READER;

	private static DataModelReader<EventDataModel> EVENT_GENERATED_READER;

	private static DataModelReader<EventAssetsDataModel> EVENT_ASSETS_GENERATED_READER;

	private static DataModelReader<EventMultiTeamEffectivenessDataModel> EVENT_MULTI_TEAM_EFFECTIVENESS_GENERATED_READER;

	private static DataModelReader<EventMultiTeamResultThreshoDataModel> EVENT_MULTI_TEAM_RESULT_THRESHO_GENERATED_READER;

	private static DataModelReader<EventPartsDataModel> EVENT_PARTS_GENERATED_READER;

	private static DataModelReader<EventPointsBucketsDataModel> EVENT_POINTS_BUCKETS_GENERATED_READER;

	private static DataModelReader<EventRaidbossDamageDropRateDataModel> EVENT_RAIDBOSS_DAMAGE_DROP_RATE_GENERATED_READER;

	private static DataModelReader<EventUnitBoostDataModel> EVENT_UNIT_BOOST_GENERATED_READER;

	private static DataModelReader<EventUnitsDataModel> EVENT_UNITS_GENERATED_READER;

	private static DataModelReader<GachaAbTestingDataModel> GACHA_AB_TESTING_GENERATED_READER;

	private static DataModelReader<GachaAbTestingGroupEnableDataModel> GACHA_AB_TESTING_GROUP_ENABLE_GENERATED_READER;

	private static DataModelReader<GachaInfoDetailsDataModel> GACHA_INFO_DETAILS_GENERATED_READER;

	private static DataModelReader<GachaInfoItemsDataModel> GACHA_INFO_ITEMS_GENERATED_READER;

	private static DataModelReader<GachaPlinkoDetailsDataModel> GACHA_PLINKO_DETAILS_GENERATED_READER;

	private static DataModelReader<GachaPlinkoPrizePriceDataModel> GACHA_PLINKO_PRIZE_PRICE_GENERATED_READER;

	private static DataModelReader<GachaPlinkoPrizesDataModel> GACHA_PLINKO_PRIZES_GENERATED_READER;

	private static DataModelReader<GachaPlinkoSlotChancesDataModel> GACHA_PLINKO_SLOT_CHANCES_GENERATED_READER;

	private static DataModelReader<GachaPoolsDataModel> GACHA_POOLS_GENERATED_READER;

	private static DataModelReader<HelpRegistersDataModel> HELP_REGISTERS_GENERATED_READER;

	private static DataModelReader<HelpTopicDataModel> HELP_TOPIC_GENERATED_READER;

	private static DataModelReader<ItemDataModel> ITEM_GENERATED_READER;

	private static DataModelReader<ItemGiftDataModel> ITEM_GIFT_GENERATED_READER;

	private static DataModelReader<ItemPriceDataModel> ITEM_PRICE_GENERATED_READER;

	private static DataModelReader<LeaderboardRewardsDataModel> LEADERBOARD_REWARDS_GENERATED_READER;

	private static DataModelReader<LeaderboardsDataModel> LEADERBOARDS_GENERATED_READER;

	private static DataModelReader<LinesEmoticonsDataModel> LINES_EMOTICONS_GENERATED_READER;

	private static DataModelReader<LinesNewsDataModel> LINES_NEWS_GENERATED_READER;

	private static DataModelReader<LinesProTipsDataModel> LINES_PRO_TIPS_GENERATED_READER;

	private static DataModelReader<NewsDataModel> NEWS_GENERATED_READER;

	private static DataModelReader<ProgressionDivisionDataModel> PROGRESSION_DIVISION_GENERATED_READER;

	private static DataModelReader<ProgressionPromotionSeriesDataModel> PROGRESSION_PROMOTION_SERIES_GENERATED_READER;

	private static DataModelReader<UnitDataModel> UNIT_GENERATED_READER;

	private static DataModelReader<UnitActionDataModel> UNIT_ACTION_GENERATED_READER;

	private static DataModelReader<UnitCooldownDataModel> UNIT_COOLDOWN_GENERATED_READER;

	private static DataModelReader<UnitDestroyGemDropDataModel> UNIT_DESTROY_GEM_DROP_GENERATED_READER;

	private static DataModelReader<UnitLevelProgressionDataModel> UNIT_LEVEL_PROGRESSION_GENERATED_READER;

	private static DataModelReader<UnitLevelUpRequirementDataModel> UNIT_LEVEL_UP_REQUIREMENT_GENERATED_READER;

	private static DataModelReader<UnitPartTypesDataModel> UNIT_PART_TYPES_GENERATED_READER;

	private static DataModelReader<UnitPartialLevelDataModel> UNIT_PARTIAL_LEVEL_GENERATED_READER;

	private static DataModelReader<UnitPartsDataModel> UNIT_PARTS_GENERATED_READER;

	private static DataModelReader<UnitRarityDataModel> UNIT_RARITY_GENERATED_READER;

	private static DataModelReader<UnitScrapValueDataModel> UNIT_SCRAP_VALUE_GENERATED_READER;

	private static DataModelReader<UnitSpecialDataModel> UNIT_SPECIAL_GENERATED_READER;

	private static DataModelReader<UnitSpecialHandlerDataModel> UNIT_SPECIAL_HANDLER_GENERATED_READER;

	private static DataModelReader<UnitTypeDataModel> UNIT_TYPE_GENERATED_READER;

	private static DataModelReader<UnitWeaponAnimDataModel> UNIT_WEAPON_ANIM_GENERATED_READER;

	private DataModelReader<LocalizationItemDataModel> LOCALIZATION_UI_SELECT_SQL = new DataModelReader<LocalizationItemDataModel>("localization", CreateLocalizationItemReader);

	static DataModelAccess()
	{
		TYPE_TO_READER = new Dictionary<Type, object>();
		ABILITY_GENERATED_READER = new DataModelReader<AbilityDataModel>("ability", CreateAbilityReader);
		ABILITY_HANDLER_GENERATED_READER = new DataModelReader<AbilityHandlerDataModel>("ability_handler", CreateAbilityHandlerReader);
		ABILITY_TYPE_GENERATED_READER = new DataModelReader<AbilityTypeDataModel>("ability_type", CreateAbilityTypeReader);
		AI_ARMY_GENERATED_READER = new DataModelReader<AiArmyDataModel>("ai_army", CreateAiArmyReader);
		AI_ARMY_PARTS_GENERATED_READER = new DataModelReader<AiArmyPartsDataModel>("ai_army_parts", CreateAiArmyPartsReader);
		AI_HANDLER_GENERATED_READER = new DataModelReader<AiHandlerDataModel>("ai_handler", CreateAiHandlerReader);
		ANNOUNCER_DIALOG_SEQUENCES_GENERATED_READER = new DataModelReader<AnnouncerDialogSequencesDataModel>("announcer_dialog_sequences", CreateAnnouncerDialogSequencesReader);
		ASSET_BUNDLES_GENERATED_READER = new DataModelReader<AssetBundlesDataModel>("asset_bundles", CreateAssetBundlesReader);
		ASSET_LINKAGE_GENERATED_READER = new DataModelReader<AssetLinkageDataModel>("asset_linkage", CreateAssetLinkageReader);
		AUDIO_TRIGGERS_GENERATED_READER = new DataModelReader<AudioTriggersDataModel>("audio_triggers", CreateAudioTriggersReader);
		BOOST_ABILITY_MULTIPLIER_GENERATED_READER = new DataModelReader<BoostAbilityMultiplierDataModel>("boost_ability_multiplier", CreateBoostAbilityMultiplierReader);
		BOOST_GENERATED_READER = new DataModelReader<BoostDataModel>("boost", CreateBoostReader);
		CONFIG_GENERATED_READER = new DataModelReader<ConfigDataModel>("config", CreateConfigReader);
		CONSTANT_GENERATED_READER = new DataModelReader<ConstantDataModel>("constant", CreateConstantReader);
		CONTRACT_DETAILS_GENERATED_READER = new DataModelReader<ContractDetailsDataModel>("contract_details", CreateContractDetailsReader);
		CONTRACTS_GENERATED_READER = new DataModelReader<ContractsDataModel>("contracts", CreateContractsReader);
		DIALOG_SCREEN_GENERATED_READER = new DataModelReader<DialogScreenDataModel>("dialog_screen", CreateDialogScreenReader);
		EFFECTS_GENERATED_READER = new DataModelReader<EffectsDataModel>("effects", CreateEffectsReader);
		EVENT_GENERATED_READER = new DataModelReader<EventDataModel>("event", CreateEventReader);
		EVENT_ASSETS_GENERATED_READER = new DataModelReader<EventAssetsDataModel>("event_assets", CreateEventAssetsReader);
		EVENT_MULTI_TEAM_EFFECTIVENESS_GENERATED_READER = new DataModelReader<EventMultiTeamEffectivenessDataModel>("event_multi_team_effectiveness", CreateEventMultiTeamEffectivenessReader);
		EVENT_MULTI_TEAM_RESULT_THRESHO_GENERATED_READER = new DataModelReader<EventMultiTeamResultThreshoDataModel>("event_multi_team_result_thresho", CreateEventMultiTeamResultThreshoReader);
		EVENT_PARTS_GENERATED_READER = new DataModelReader<EventPartsDataModel>("event_parts", CreateEventPartsReader);
		EVENT_POINTS_BUCKETS_GENERATED_READER = new DataModelReader<EventPointsBucketsDataModel>("event_points_buckets", CreateEventPointsBucketsReader);
		EVENT_RAIDBOSS_DAMAGE_DROP_RATE_GENERATED_READER = new DataModelReader<EventRaidbossDamageDropRateDataModel>("event_raidboss_damage_drop_rate", CreateEventRaidbossDamageDropRateReader);
		EVENT_UNIT_BOOST_GENERATED_READER = new DataModelReader<EventUnitBoostDataModel>("event_unit_boost", CreateEventUnitBoostReader);
		EVENT_UNITS_GENERATED_READER = new DataModelReader<EventUnitsDataModel>("event_units", CreateEventUnitsReader);
		GACHA_AB_TESTING_GENERATED_READER = new DataModelReader<GachaAbTestingDataModel>("gacha_ab_testing", CreateGachaAbTestingReader);
		GACHA_AB_TESTING_GROUP_ENABLE_GENERATED_READER = new DataModelReader<GachaAbTestingGroupEnableDataModel>("gacha_ab_testing_group_enable", CreateGachaAbTestingGroupEnableReader);
		GACHA_INFO_DETAILS_GENERATED_READER = new DataModelReader<GachaInfoDetailsDataModel>("gacha_info_details", CreateGachaInfoDetailsReader);
		GACHA_INFO_ITEMS_GENERATED_READER = new DataModelReader<GachaInfoItemsDataModel>("gacha_info_items", CreateGachaInfoItemsReader);
		GACHA_PLINKO_DETAILS_GENERATED_READER = new DataModelReader<GachaPlinkoDetailsDataModel>("gacha_plinko_details", CreateGachaPlinkoDetailsReader);
		GACHA_PLINKO_PRIZE_PRICE_GENERATED_READER = new DataModelReader<GachaPlinkoPrizePriceDataModel>("gacha_plinko_prize_price", CreateGachaPlinkoPrizePriceReader);
		GACHA_PLINKO_PRIZES_GENERATED_READER = new DataModelReader<GachaPlinkoPrizesDataModel>("gacha_plinko_prizes", CreateGachaPlinkoPrizesReader);
		GACHA_PLINKO_SLOT_CHANCES_GENERATED_READER = new DataModelReader<GachaPlinkoSlotChancesDataModel>("gacha_plinko_slot_chances", CreateGachaPlinkoSlotChancesReader);
		GACHA_POOLS_GENERATED_READER = new DataModelReader<GachaPoolsDataModel>("gacha_pools", CreateGachaPoolsReader);
		HELP_REGISTERS_GENERATED_READER = new DataModelReader<HelpRegistersDataModel>("help_registers", CreateHelpRegistersReader);
		HELP_TOPIC_GENERATED_READER = new DataModelReader<HelpTopicDataModel>("help_topic", CreateHelpTopicReader);
		ITEM_GENERATED_READER = new DataModelReader<ItemDataModel>("item", CreateItemReader);
		ITEM_GIFT_GENERATED_READER = new DataModelReader<ItemGiftDataModel>("item_gift", CreateItemGiftReader);
		ITEM_PRICE_GENERATED_READER = new DataModelReader<ItemPriceDataModel>("item_price", CreateItemPriceReader);
		LEADERBOARD_REWARDS_GENERATED_READER = new DataModelReader<LeaderboardRewardsDataModel>("leaderboard_rewards", CreateLeaderboardRewardsReader);
		LEADERBOARDS_GENERATED_READER = new DataModelReader<LeaderboardsDataModel>("leaderboards", CreateLeaderboardsReader);
		LINES_EMOTICONS_GENERATED_READER = new DataModelReader<LinesEmoticonsDataModel>("lines_emoticons", CreateLinesEmoticonsReader);
		LINES_NEWS_GENERATED_READER = new DataModelReader<LinesNewsDataModel>("lines_news", CreateLinesNewsReader);
		LINES_PRO_TIPS_GENERATED_READER = new DataModelReader<LinesProTipsDataModel>("lines_pro_tips", CreateLinesProTipsReader);
		NEWS_GENERATED_READER = new DataModelReader<NewsDataModel>("news", CreateNewsReader);
		PROGRESSION_DIVISION_GENERATED_READER = new DataModelReader<ProgressionDivisionDataModel>("progression_division", CreateProgressionDivisionReader);
		PROGRESSION_PROMOTION_SERIES_GENERATED_READER = new DataModelReader<ProgressionPromotionSeriesDataModel>("progression_promotion_series", CreateProgressionPromotionSeriesReader);
		UNIT_GENERATED_READER = new DataModelReader<UnitDataModel>("unit", CreateUnitReader);
		UNIT_ACTION_GENERATED_READER = new DataModelReader<UnitActionDataModel>("unit_action", CreateUnitActionReader);
		UNIT_COOLDOWN_GENERATED_READER = new DataModelReader<UnitCooldownDataModel>("unit_cooldown", CreateUnitCooldownReader);
		UNIT_DESTROY_GEM_DROP_GENERATED_READER = new DataModelReader<UnitDestroyGemDropDataModel>("unit_destroy_gem_drop", CreateUnitDestroyGemDropReader);
		UNIT_LEVEL_PROGRESSION_GENERATED_READER = new DataModelReader<UnitLevelProgressionDataModel>("unit_level_progression", CreateUnitLevelProgressionReader);
		UNIT_LEVEL_UP_REQUIREMENT_GENERATED_READER = new DataModelReader<UnitLevelUpRequirementDataModel>("unit_level_up_requirement", CreateUnitLevelUpRequirementReader);
		UNIT_PART_TYPES_GENERATED_READER = new DataModelReader<UnitPartTypesDataModel>("unit_part_types", CreateUnitPartTypesReader);
		UNIT_PARTIAL_LEVEL_GENERATED_READER = new DataModelReader<UnitPartialLevelDataModel>("unit_partial_level", CreateUnitPartialLevelReader);
		UNIT_PARTS_GENERATED_READER = new DataModelReader<UnitPartsDataModel>("unit_parts", CreateUnitPartsReader);
		UNIT_RARITY_GENERATED_READER = new DataModelReader<UnitRarityDataModel>("unit_rarity", CreateUnitRarityReader);
		UNIT_SCRAP_VALUE_GENERATED_READER = new DataModelReader<UnitScrapValueDataModel>("unit_scrap_value", CreateUnitScrapValueReader);
		UNIT_SPECIAL_GENERATED_READER = new DataModelReader<UnitSpecialDataModel>("unit_special", CreateUnitSpecialReader);
		UNIT_SPECIAL_HANDLER_GENERATED_READER = new DataModelReader<UnitSpecialHandlerDataModel>("unit_special_handler", CreateUnitSpecialHandlerReader);
		UNIT_TYPE_GENERATED_READER = new DataModelReader<UnitTypeDataModel>("unit_type", CreateUnitTypeReader);
		UNIT_WEAPON_ANIM_GENERATED_READER = new DataModelReader<UnitWeaponAnimDataModel>("unit_weapon_anim", CreateUnitWeaponAnimReader);
		InitAbilityDataModelReader();
		InitAbilityHandlerDataModelReader();
		InitAbilityTypeDataModelReader();
		InitAiArmyDataModelReader();
		InitAiArmyPartsDataModelReader();
		InitAiHandlerDataModelReader();
		InitAnnouncerDialogSequencesDataModelReader();
		InitAssetBundlesDataModelReader();
		InitAssetLinkageDataModelReader();
		InitAudioTriggersDataModelReader();
		InitBoostDataModelReader();
		InitBoostAbilityMultiplierDataModelReader();
		InitConfigDataModelReader();
		InitConstantDataModelReader();
		InitContractDetailsDataModelReader();
		InitContractsDataModelReader();
		InitDialogScreenDataModelReader();
		InitEffectsDataModelReader();
		InitEventDataModelReader();
		InitEventAssetsDataModelReader();
		InitEventMultiTeamEffectivenessDataModelReader();
		InitEventMultiTeamResultThreshoDataModelReader();
		InitEventPartsDataModelReader();
		InitEventPointsBucketsDataModelReader();
		InitEventRaidbossDamageDropRateDataModelReader();
		InitEventUnitBoostDataModelReader();
		InitEventUnitsDataModelReader();
		InitGachaAbTestingDataModelReader();
		InitGachaAbTestingGroupEnableDataModelReader();
		InitGachaInfoDetailsDataModelReader();
		InitGachaInfoItemsDataModelReader();
		InitGachaPlinkoDetailsDataModelReader();
		InitGachaPlinkoPrizePriceDataModelReader();
		InitGachaPlinkoPrizesDataModelReader();
		InitGachaPlinkoSlotChancesDataModelReader();
		InitGachaPoolsDataModelReader();
		InitHelpRegistersDataModelReader();
		InitHelpTopicDataModelReader();
		InitItemDataModelReader();
		InitItemGiftDataModelReader();
		InitItemPriceDataModelReader();
		InitLeaderboardRewardsDataModelReader();
		InitLeaderboardsDataModelReader();
		InitLinesEmoticonsDataModelReader();
		InitLinesNewsDataModelReader();
		InitLinesProTipsDataModelReader();
		InitNewsDataModelReader();
		InitProgressionDivisionDataModelReader();
		InitProgressionPromotionSeriesDataModelReader();
		InitUnitDataModelReader();
		InitUnitActionDataModelReader();
		InitUnitCooldownDataModelReader();
		InitUnitDestroyGemDropDataModelReader();
		InitUnitLevelProgressionDataModelReader();
		InitUnitLevelUpRequirementDataModelReader();
		InitUnitPartTypesDataModelReader();
		InitUnitPartialLevelDataModelReader();
		InitUnitPartsDataModelReader();
		InitUnitRarityDataModelReader();
		InitUnitScrapValueDataModelReader();
		InitUnitSpecialDataModelReader();
		InitUnitSpecialHandlerDataModelReader();
		InitUnitTypeDataModelReader();
		InitUnitWeaponAnimDataModelReader();
	}

	private AssetBundleDataModel ReadAssetBundleDataModel(SqliteDataReader reader)
	{
		AssetBundleDataModel assetBundleDataModel = new AssetBundleDataModel();
		int num = 0;
		assetBundleDataModel.id = reader.GetInt32(num++);
		assetBundleDataModel.priority = reader.GetInt32(num++);
		assetBundleDataModel.iPhoneHash = reader.GetString(num++);
		assetBundleDataModel.androidHash = reader.GetString(num++);
		return assetBundleDataModel;
	}

	public Error GetSingleAssetBundle(int id, out AssetBundleDataModel m)
	{
		return GetSingle("SELECT id,priority,iphone_hash,android_hash,osxeditor_hash FROM ASSET_BUNDLES WHERE id=" + id, out m, ReadAssetBundleDataModel);
	}

	public Error GetAllAssetBundles(out List<AssetBundleDataModel> assetBundles)
	{
		return GetList("SELECT id,priority,iphone_hash,android_hash,osxeditor_hash FROM ASSET_BUNDLES ORDER BY priority,id", out assetBundles, ReadAssetBundleDataModel);
	}

	public Error InsertAssetBundle(AssetBundleDataModel m)
	{
		try
		{
			if (!IsConnected())
			{
				return Error.NotConnected();
			}
			OpenReader("select id from ASSET_BUNDLES where id=" + m.id, delegate(SqliteDataReader reader)
			{
				if (reader.Read())
				{
					SqliteCommand sqliteCommand = db.CreateCommand();
					try
					{
						sqliteCommand.CommandText = "update asset_bundles set ";
						if (m.androidHash != null)
						{
							sqliteCommand.CommandText = sqliteCommand.CommandText + "android_hash='" + m.androidHash + "'";
						}
						else if (m.iPhoneHash != null)
						{
							sqliteCommand.CommandText = sqliteCommand.CommandText + "iphone_hash='" + m.iPhoneHash + "'";
						}
						sqliteCommand.CommandText = sqliteCommand.CommandText + " where id=" + m.id;
						Log.Info(sqliteCommand.CommandText);
						sqliteCommand.ExecuteNonQuery();
						return;
					}
					finally
					{
						if (sqliteCommand != null)
						{
							((IDisposable)(object)sqliteCommand).Dispose();
						}
					}
				}
				SqliteCommand sqliteCommand2 = db.CreateCommand();
				try
				{
					sqliteCommand2.CommandText = "INSERT INTO ASSET_BUNDLES(id,priority,iphone_hash,android_hash) VALUES(?,?,?,?)";
					SqliteParameter sqliteParameter = sqliteCommand2.CreateParameter();
					sqliteParameter.Value = m.id;
					sqliteCommand2.Parameters.Add(sqliteParameter);
					sqliteParameter = sqliteCommand2.CreateParameter();
					sqliteParameter.Value = m.priority;
					sqliteCommand2.Parameters.Add(sqliteParameter);
					sqliteParameter = sqliteCommand2.CreateParameter();
					if (m.iPhoneHash != null)
					{
						sqliteParameter.Value = m.iPhoneHash;
					}
					else
					{
						sqliteParameter.Value = string.Empty;
					}
					sqliteCommand2.Parameters.Add(sqliteParameter);
					sqliteParameter = sqliteCommand2.CreateParameter();
					if (m.androidHash != null)
					{
						sqliteParameter.Value = m.androidHash;
					}
					else
					{
						sqliteParameter.Value = string.Empty;
					}
					sqliteCommand2.Parameters.Add(sqliteParameter);
					sqliteCommand2.ExecuteNonQuery();
				}
				finally
				{
					if (sqliteCommand2 != null)
					{
						((IDisposable)(object)sqliteCommand2).Dispose();
					}
				}
			});
			return null;
		}
		catch (Exception ex)
		{
			return new Error(Error.Code.Exception, ex.ToString());
		}
	}

	public Error _GetAllAssetBundles(out List<AssetBundleDataModel> assetBundles)
	{
		DataModelAccess dataModelAccess = NonUnitySingleton<DMAccessManager>.instance.DataModelAccess;
		Error error = null;
		lock (dataModelAccess)
		{
			return dataModelAccess.GetAllAssetBundles(out assetBundles);
		}
	}

	public Error _GetSingleAssetBundle(int id, out AssetBundleDataModel m)
	{
		DataModelAccess dataModelAccess = NonUnitySingleton<DMAccessManager>.instance.DataModelAccess;
		Error result = null;
		Dictionary<string, object> singleValueCache = CacheManager.GetSingleValueCache<AssetBundleDataModel>();
		object value = null;
		if (singleValueCache.TryGetValue(id.ToString(), out value))
		{
			m = value as AssetBundleDataModel;
		}
		else
		{
			lock (dataModelAccess)
			{
				result = dataModelAccess.GetSingleAssetBundle(id, out m);
			}
		}
		return result;
	}

	public Error _InsertAssetBundle(AssetBundleDataModel m)
	{
		DataModelAccess dataModelAccess = NonUnitySingleton<DMAccessManager>.instance.DataModelAccess;
		Error error = null;
		lock (dataModelAccess)
		{
			return dataModelAccess.InsertAssetBundle(m);
		}
	}

	public Error Connect(string path, out ConnectDataModel m)
	{
		Log.Debug("DataModelAccess.Connect path:" + path);
		m = null;
		try
		{
			Error error = Connect(path);
			if (error != null)
			{
				return error;
			}
			string version = null;
			string assetUrl = null;
			OpenReader("SELECT KEY, VALUE FROM CONFIG", delegate(SqliteDataReader reader)
			{
				while (reader.Read())
				{
					int num = 0;
					string text = reader.GetString(num++);
					switch (text)
					{
					case "VERSION":
						version = reader.GetString(num++);
						break;
					case "ASSET_URL":
						assetUrl = reader.GetString(num++);
						break;
					default:
						throw new Exception("Unknown key in config table: " + text);
					}
				}
			});
			if (version == null || assetUrl == null)
			{
				throw new Exception("Could not read required fields out of the config table");
			}
			m = new ConnectDataModel();
			m.hash = HashUtility.MD5(path);
			m.version = int.Parse(version);
			m.assetUrl = assetUrl;
			return null;
		}
		catch (Exception e)
		{
			db.Close();
			db = null;
			return Error.Exception(e);
		}
	}

	private Dictionary<string, T> GenerateCacheDictionary<T>(GetAllDelegate<T> getAllCb, out Error error) where T : BaseDataModel
	{
		List<T> list = new List<T>();
		error = getAllCb(out list);
		Dictionary<string, T> dictionary = new Dictionary<string, T>();
		foreach (T item in list)
		{
			dictionary.Add(item.id, item);
		}
		return dictionary;
	}

	private void ForceCache<T>() where T : BaseDataModel, new()
	{
		NonUnitySingleton<DMAccessManager>.instance.GetAll<T>();
	}

	public void CacheConstants()
	{
		foreach (ConstantDataModel item in NonUnitySingleton<DMAccessManager>.instance.GetAll<ConstantDataModel>())
		{
			CacheManager.constantsCache[item.key] = item.value;
		}
	}

	public Error CacheRequiredTables()
	{
		CacheManager.PurgeCache();
		Log.Info("Caching Constants Data Model");
		CacheConstants();
		Log.Info("Caching Common Data Models");
		ForceCache<AbilityHandlerDataModel>();
		ForceCache<AbilityDataModel>();
		ForceCache<UnitSpecialDataModel>();
		ForceCache<UnitSpecialHandlerDataModel>();
		ForceCache<UnitRarityDataModel>();
		ForceCache<UnitTypeDataModel>();
		ForceCache<UnitDestroyGemDropDataModel>();
		return null;
	}

	public T GetSingle<T>(string id) where T : BaseDataModel, new()
	{
		return GetSingleByQuery<T>(" WHERE id = " + id);
	}

	public T GetSingleByQuery<T>(string whereClause) where T : BaseDataModel, new()
	{
		Type typeFromHandle = typeof(T);
		DataModelReader<T> dataModelReader = TYPE_TO_READER[typeFromHandle] as DataModelReader<T>;
		return GetSingleObject(dataModelReader.sqlRequest + " " + whereClause, dataModelReader.MakeSqlRequest);
	}

	public List<T> GetMultiByQuery<T>(string whereClause) where T : BaseDataModel, new()
	{
		Type typeFromHandle = typeof(T);
		DataModelReader<T> dataModelReader = TYPE_TO_READER[typeFromHandle] as DataModelReader<T>;
		return GetMultiObject(dataModelReader.sqlRequest + " " + whereClause, dataModelReader.MakeSqlRequest);
	}

	public List<T> GetAll<T>() where T : BaseDataModel, new()
	{
		Type typeFromHandle = typeof(T);
		DataModelReader<T> dataModelReader = TYPE_TO_READER[typeFromHandle] as DataModelReader<T>;
		return GetMultiObject(dataModelReader.sqlRequest, dataModelReader.MakeSqlRequest);
	}

	private static void CreateAbilityReader(DataModelReader<AbilityDataModel> dmReader)
	{
		dmReader.AddItem("ability_type", delegate
		{
			dmReader.model.abilityType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("action_boost_type", delegate
		{
			dmReader.model.actionBoostType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("action_boost_value_a", delegate
		{
			dmReader.model.actionBoostValueA = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("action_boost_value_b", delegate
		{
			dmReader.model.actionBoostValueB = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("action_point", delegate
		{
			dmReader.model.actionPoint = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("action_type", delegate
		{
			dmReader.model.actionType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("execution_order", delegate
		{
			dmReader.model.executionOrder = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("handler_id", delegate
		{
			dmReader.model.handlerId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("icon_linkage_id", delegate
		{
			dmReader.model.iconLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("is_active", delegate
		{
			dmReader.model.isActive = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("is_announcer", delegate
		{
			dmReader.model.isAnnouncer = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("limit_one_per_battle", delegate
		{
			dmReader.model.limitOnePerBattle = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("limit_one_per_round", delegate
		{
			dmReader.model.limitOnePerRound = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("num_kill_unit", delegate
		{
			dmReader.model.numKillUnit = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("research_time", delegate
		{
			dmReader.model.researchTime = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("selection_text", delegate
		{
			dmReader.model.selectionText = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("target_group", delegate
		{
			dmReader.model.targetGroup = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unlock_order", delegate
		{
			dmReader.model.unlockOrder = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unlock_tier", delegate
		{
			dmReader.model.unlockTier = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleAbility(string id, out AbilityDataModel m)
	{
		return GetSingle(ABILITY_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, ABILITY_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAbility(out List<AbilityDataModel> data)
	{
		return GetList(ABILITY_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, ABILITY_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAbilityDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AbilityDataModel), ABILITY_GENERATED_READER);
	}

	private static void CreateAbilityHandlerReader(DataModelReader<AbilityHandlerDataModel> dmReader)
	{
		dmReader.AddItem("handler", delegate
		{
			dmReader.model.handler = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
	}

	private Error GetSingleAbilityHandler(string id, out AbilityHandlerDataModel m)
	{
		return GetSingle(ABILITY_HANDLER_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, ABILITY_HANDLER_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAbilityHandler(out List<AbilityHandlerDataModel> data)
	{
		return GetList(ABILITY_HANDLER_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, ABILITY_HANDLER_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAbilityHandlerDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AbilityHandlerDataModel), ABILITY_HANDLER_GENERATED_READER);
	}

	private static void CreateAbilityTypeReader(DataModelReader<AbilityTypeDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleAbilityType(string id, out AbilityTypeDataModel m)
	{
		return GetSingle(ABILITY_TYPE_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, ABILITY_TYPE_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAbilityType(out List<AbilityTypeDataModel> data)
	{
		return GetList(ABILITY_TYPE_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, ABILITY_TYPE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAbilityTypeDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AbilityTypeDataModel), ABILITY_TYPE_GENERATED_READER);
	}

	private static void CreateAiArmyReader(DataModelReader<AiArmyDataModel> dmReader)
	{
		dmReader.AddItem("ability_id_0", delegate
		{
			dmReader.model.abilityId0 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability_id_1", delegate
		{
			dmReader.model.abilityId1 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability_id_2", delegate
		{
			dmReader.model.abilityId2 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability_id_3", delegate
		{
			dmReader.model.abilityId3 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ai_strategy_id", delegate
		{
			dmReader.model.aiStrategyId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("difficulty_id", delegate
		{
			dmReader.model.difficultyId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("division_max_id", delegate
		{
			dmReader.model.divisionMaxId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("division_min_id", delegate
		{
			dmReader.model.divisionMinId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_id", delegate
		{
			dmReader.model.eventId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_time_active_end", delegate
		{
			dmReader.model.eventTimeActiveEnd = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_time_active_start", delegate
		{
			dmReader.model.eventTimeActiveStart = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("promo_use", delegate
		{
			dmReader.model.promoUse = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("streak", delegate
		{
			dmReader.model.streak = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("team_type", delegate
		{
			dmReader.model.teamType = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("unit_level_id_1", delegate
		{
			dmReader.model.unitLevelId1 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_level_id_2", delegate
		{
			dmReader.model.unitLevelId2 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_level_id_3", delegate
		{
			dmReader.model.unitLevelId3 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_level_id_4", delegate
		{
			dmReader.model.unitLevelId4 = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleAiArmy(string id, out AiArmyDataModel m)
	{
		return GetSingle(AI_ARMY_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, AI_ARMY_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAiArmy(out List<AiArmyDataModel> data)
	{
		return GetList(AI_ARMY_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, AI_ARMY_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAiArmyDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AiArmyDataModel), AI_ARMY_GENERATED_READER);
	}

	private static void CreateAiArmyPartsReader(DataModelReader<AiArmyPartsDataModel> dmReader)
	{
		dmReader.AddItem("ai_army_id", delegate
		{
			dmReader.model.aiArmyId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("part_id", delegate
		{
			dmReader.model.partId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleAiArmyParts(string id, out AiArmyPartsDataModel m)
	{
		return GetSingle(AI_ARMY_PARTS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, AI_ARMY_PARTS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAiArmyParts(out List<AiArmyPartsDataModel> data)
	{
		return GetList(AI_ARMY_PARTS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, AI_ARMY_PARTS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAiArmyPartsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AiArmyPartsDataModel), AI_ARMY_PARTS_GENERATED_READER);
	}

	private static void CreateAiHandlerReader(DataModelReader<AiHandlerDataModel> dmReader)
	{
		dmReader.AddItem("handler", delegate
		{
			dmReader.model.handler = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
	}

	private Error GetSingleAiHandler(string id, out AiHandlerDataModel m)
	{
		return GetSingle(AI_HANDLER_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, AI_HANDLER_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAiHandler(out List<AiHandlerDataModel> data)
	{
		return GetList(AI_HANDLER_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, AI_HANDLER_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAiHandlerDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AiHandlerDataModel), AI_HANDLER_GENERATED_READER);
	}

	private static void CreateAnnouncerDialogSequencesReader(DataModelReader<AnnouncerDialogSequencesDataModel> dmReader)
	{
		dmReader.AddItem("action", delegate
		{
			dmReader.model.action = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("announcer_type", delegate
		{
			dmReader.model.announcerType = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("orientation", delegate
		{
			dmReader.model.orientation = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("sequence_id", delegate
		{
			dmReader.model.sequenceId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("subsequence_order", delegate
		{
			dmReader.model.subsequenceOrder = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllAnnouncerDialogSequences(out List<AnnouncerDialogSequencesDataModel> data)
	{
		return GetList(ANNOUNCER_DIALOG_SEQUENCES_GENERATED_READER.sqlRequest + " ", out data, ANNOUNCER_DIALOG_SEQUENCES_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAnnouncerDialogSequencesDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AnnouncerDialogSequencesDataModel), ANNOUNCER_DIALOG_SEQUENCES_GENERATED_READER);
	}

	private static void CreateAssetBundlesReader(DataModelReader<AssetBundlesDataModel> dmReader)
	{
		dmReader.AddItem("android_hash", delegate
		{
			dmReader.model.androidHash = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("iphone_hash", delegate
		{
			dmReader.model.iphoneHash = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("osxeditor_hash", delegate
		{
			dmReader.model.osxeditorHash = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("priority", delegate
		{
			dmReader.model.priority = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleAssetBundles(string id, out AssetBundlesDataModel m)
	{
		return GetSingle(ASSET_BUNDLES_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, ASSET_BUNDLES_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAssetBundles(out List<AssetBundlesDataModel> data)
	{
		return GetList(ASSET_BUNDLES_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, ASSET_BUNDLES_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAssetBundlesDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AssetBundlesDataModel), ASSET_BUNDLES_GENERATED_READER);
	}

	private static void CreateAssetLinkageReader(DataModelReader<AssetLinkageDataModel> dmReader)
	{
		dmReader.AddItem("asset_name", delegate
		{
			dmReader.model.assetName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("bundle_id", delegate
		{
			dmReader.model.bundleId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("priority", delegate
		{
			dmReader.model.priority = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleAssetLinkage(string id, out AssetLinkageDataModel m)
	{
		return GetSingle(ASSET_LINKAGE_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, ASSET_LINKAGE_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllAssetLinkage(out List<AssetLinkageDataModel> data)
	{
		return GetList(ASSET_LINKAGE_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, ASSET_LINKAGE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAssetLinkageDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AssetLinkageDataModel), ASSET_LINKAGE_GENERATED_READER);
	}

	private static void CreateAudioTriggersReader(DataModelReader<AudioTriggersDataModel> dmReader)
	{
		dmReader.AddItem("asset_linkage_id", delegate
		{
			dmReader.model.assetLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("audio_trigger_name", delegate
		{
			dmReader.model.audioTriggerName = dmReader.ReadOneValue<string>();
		});
	}

	public Error GetAllAudioTriggers(out List<AudioTriggersDataModel> data)
	{
		return GetList(AUDIO_TRIGGERS_GENERATED_READER.sqlRequest + " ", out data, AUDIO_TRIGGERS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitAudioTriggersDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(AudioTriggersDataModel), AUDIO_TRIGGERS_GENERATED_READER);
	}

	private static void CreateBoostAbilityMultiplierReader(DataModelReader<BoostAbilityMultiplierDataModel> dmReader)
	{
		dmReader.AddItem("ability_id", delegate
		{
			dmReader.model.abilityId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("boost_id", delegate
		{
			dmReader.model.boostId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("multiplier", delegate
		{
			dmReader.model.multiplier = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleBoostAbilityMultiplier(string id, out BoostAbilityMultiplierDataModel m)
	{
		return GetSingle(BOOST_ABILITY_MULTIPLIER_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, BOOST_ABILITY_MULTIPLIER_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllBoostAbilityMultiplier(out List<BoostAbilityMultiplierDataModel> data)
	{
		return GetList(BOOST_ABILITY_MULTIPLIER_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, BOOST_ABILITY_MULTIPLIER_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitBoostAbilityMultiplierDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(BoostAbilityMultiplierDataModel), BOOST_ABILITY_MULTIPLIER_GENERATED_READER);
	}

	private static void CreateBoostReader(DataModelReader<BoostDataModel> dmReader)
	{
		dmReader.AddItem("boost_type", delegate
		{
			dmReader.model.boostType = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("cash_multiplier", delegate
		{
			dmReader.model.cashMultiplier = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("duration_minutes", delegate
		{
			dmReader.model.durationMinutes = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_type", delegate
		{
			dmReader.model.eventType = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("gem_multiplier", delegate
		{
			dmReader.model.gemMultiplier = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("is_active", delegate
		{
			dmReader.model.isActive = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_multiplier_1", delegate
		{
			dmReader.model.keyMultiplier1 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_multiplier_2", delegate
		{
			dmReader.model.keyMultiplier2 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_short_description", delegate
		{
			dmReader.model.keyShortDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("multiplier_1", delegate
		{
			dmReader.model.multiplier1 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("multiplier_2", delegate
		{
			dmReader.model.multiplier2 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("parts_multiplier", delegate
		{
			dmReader.model.partsMultiplier = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("price_id", delegate
		{
			dmReader.model.priceId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("tier_multiplier", delegate
		{
			dmReader.model.tierMultiplier = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleBoost(string id, out BoostDataModel m)
	{
		return GetSingle(BOOST_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, BOOST_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllBoost(out List<BoostDataModel> data)
	{
		return GetList(BOOST_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, BOOST_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitBoostDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(BoostDataModel), BOOST_GENERATED_READER);
	}

	private static void CreateConfigReader(DataModelReader<ConfigDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key", delegate
		{
			dmReader.model.key = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("value", delegate
		{
			dmReader.model.value = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleConfig(string id, out ConfigDataModel m)
	{
		return GetSingle(CONFIG_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, CONFIG_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllConfig(out List<ConfigDataModel> data)
	{
		return GetList(CONFIG_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, CONFIG_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitConfigDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ConfigDataModel), CONFIG_GENERATED_READER);
	}

	private static void CreateConstantReader(DataModelReader<ConstantDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key", delegate
		{
			dmReader.model.key = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("value", delegate
		{
			dmReader.model.value = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleConstant(string id, out ConstantDataModel m)
	{
		return GetSingle(CONSTANT_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, CONSTANT_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllConstant(out List<ConstantDataModel> data)
	{
		return GetList(CONSTANT_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, CONSTANT_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitConstantDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ConstantDataModel), CONSTANT_GENERATED_READER);
	}

	private static void CreateContractDetailsReader(DataModelReader<ContractDetailsDataModel> dmReader)
	{
		dmReader.AddItem("contract_id", delegate
		{
			dmReader.model.contractId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_max", delegate
		{
			dmReader.model.dropMax = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_min", delegate
		{
			dmReader.model.dropMin = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_rate", delegate
		{
			dmReader.model.dropRate = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_id", delegate
		{
			dmReader.model.itemId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("part_type", delegate
		{
			dmReader.model.partType = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllContractDetails(out List<ContractDetailsDataModel> data)
	{
		return GetList(CONTRACT_DETAILS_GENERATED_READER.sqlRequest + " ", out data, CONTRACT_DETAILS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitContractDetailsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ContractDetailsDataModel), CONTRACT_DETAILS_GENERATED_READER);
	}

	private static void CreateContractsReader(DataModelReader<ContractsDataModel> dmReader)
	{
		dmReader.AddItem("asset_linkage_id", delegate
		{
			dmReader.model.assetLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("contract_duration", delegate
		{
			dmReader.model.contractDuration = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("contract_id", delegate
		{
			dmReader.model.contractId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("contract_pool", delegate
		{
			dmReader.model.contractPool = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("contract_rarity", delegate
		{
			dmReader.model.contractRarity = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("contract_weight", delegate
		{
			dmReader.model.contractWeight = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
	}

	public Error GetAllContracts(out List<ContractsDataModel> data)
	{
		return GetList(CONTRACTS_GENERATED_READER.sqlRequest + " ", out data, CONTRACTS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitContractsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ContractsDataModel), CONTRACTS_GENERATED_READER);
	}

	private static void CreateDialogScreenReader(DataModelReader<DialogScreenDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("limit_one", delegate
		{
			dmReader.model.limitOne = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("screen_id", delegate
		{
			dmReader.model.screenId = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("sequence_id", delegate
		{
			dmReader.model.sequenceId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleDialogScreen(string id, out DialogScreenDataModel m)
	{
		return GetSingle(DIALOG_SCREEN_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, DIALOG_SCREEN_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllDialogScreen(out List<DialogScreenDataModel> data)
	{
		return GetList(DIALOG_SCREEN_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, DIALOG_SCREEN_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitDialogScreenDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(DialogScreenDataModel), DIALOG_SCREEN_GENERATED_READER);
	}

	private static void CreateEffectsReader(DataModelReader<EffectsDataModel> dmReader)
	{
		dmReader.AddItem("asset_linkage_id", delegate
		{
			dmReader.model.assetLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("effect_name", delegate
		{
			dmReader.model.effectName = dmReader.ReadOneValue<string>();
		});
	}

	public Error GetAllEffects(out List<EffectsDataModel> data)
	{
		return GetList(EFFECTS_GENERATED_READER.sqlRequest + " ", out data, EFFECTS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEffectsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EffectsDataModel), EFFECTS_GENERATED_READER);
	}

	private static void CreateEventReader(DataModelReader<EventDataModel> dmReader)
	{
		dmReader.AddItem("cooldown_time", delegate
		{
			dmReader.model.cooldownTime = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("date_end", delegate
		{
			dmReader.model.dateEnd = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("date_start", delegate
		{
			dmReader.model.dateStart = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_assets_id", delegate
		{
			dmReader.model.eventAssetsId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_type", delegate
		{
			dmReader.model.eventType = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("force_bot_matching", delegate
		{
			dmReader.model.forceBotMatching = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("rewards_id", delegate
		{
			dmReader.model.rewardsId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("solo_reward_id", delegate
		{
			dmReader.model.soloRewardId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleEvent(string id, out EventDataModel m)
	{
		return GetSingle(EVENT_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, EVENT_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllEvent(out List<EventDataModel> data)
	{
		return GetList(EVENT_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, EVENT_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventDataModel), EVENT_GENERATED_READER);
	}

	private static void CreateEventAssetsReader(DataModelReader<EventAssetsDataModel> dmReader)
	{
		dmReader.AddItem("background_asset_id", delegate
		{
			dmReader.model.backgroundAssetId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_info_key_string_body_1", delegate
		{
			dmReader.model.eventInfoKeyStringBody1 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_info_key_string_body_2", delegate
		{
			dmReader.model.eventInfoKeyStringBody2 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_info_key_string_body_3", delegate
		{
			dmReader.model.eventInfoKeyStringBody3 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_info_key_string_body_4", delegate
		{
			dmReader.model.eventInfoKeyStringBody4 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_info_key_string_body_5", delegate
		{
			dmReader.model.eventInfoKeyStringBody5 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_info_key_string_event_h1", delegate
		{
			dmReader.model.eventInfoKeyStringEventH1 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_info_key_string_h2", delegate
		{
			dmReader.model.eventInfoKeyStringH2 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_info_key_string_units_set", delegate
		{
			dmReader.model.eventInfoKeyStringUnitsSet = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("gacha_asset_bundle_1", delegate
		{
			dmReader.model.gachaAssetBundle1 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_asset_bundle_2", delegate
		{
			dmReader.model.gachaAssetBundle2 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_info_asset_bundle_1", delegate
		{
			dmReader.model.gachaInfoAssetBundle1 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_info_asset_bundle_2", delegate
		{
			dmReader.model.gachaInfoAssetBundle2 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_info_asset_bundle_3", delegate
		{
			dmReader.model.gachaInfoAssetBundle3 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_info_asset_bundle_4", delegate
		{
			dmReader.model.gachaInfoAssetBundle4 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("home_screen_unit_id", delegate
		{
			dmReader.model.homeScreenUnitId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_description_already_member_event_popup", delegate
		{
			dmReader.model.keyDescriptionAlreadyMemberEventPopup = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_description_join_club_event_popup", delegate
		{
			dmReader.model.keyDescriptionJoinClubEventPopup = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_title_already_member_event_popup", delegate
		{
			dmReader.model.keyTitleAlreadyMemberEventPopup = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_title_join_club_event_popup", delegate
		{
			dmReader.model.keyTitleJoinClubEventPopup = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("leaderboards_asset_id", delegate
		{
			dmReader.model.leaderboardsAssetId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("left_unit_id", delegate
		{
			dmReader.model.leftUnitId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("logo_asset_id", delegate
		{
			dmReader.model.logoAssetId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("panshot_asset_id", delegate
		{
			dmReader.model.panshotAssetId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("right_unit_id", delegate
		{
			dmReader.model.rightUnitId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleEventAssets(string id, out EventAssetsDataModel m)
	{
		return GetSingle(EVENT_ASSETS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, EVENT_ASSETS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllEventAssets(out List<EventAssetsDataModel> data)
	{
		return GetList(EVENT_ASSETS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, EVENT_ASSETS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventAssetsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventAssetsDataModel), EVENT_ASSETS_GENERATED_READER);
	}

	private static void CreateEventMultiTeamEffectivenessReader(DataModelReader<EventMultiTeamEffectivenessDataModel> dmReader)
	{
		dmReader.AddItem("event_unit_rank_max", delegate
		{
			dmReader.model.eventUnitRankMax = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_unit_rank_min", delegate
		{
			dmReader.model.eventUnitRankMin = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("non_event_unit_rank_max", delegate
		{
			dmReader.model.nonEventUnitRankMax = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("non_event_unit_rank_min", delegate
		{
			dmReader.model.nonEventUnitRankMin = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("post_event_unit_max", delegate
		{
			dmReader.model.postEventUnitMax = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("post_event_unit_min", delegate
		{
			dmReader.model.postEventUnitMin = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_rarity", delegate
		{
			dmReader.model.unitRarity = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllEventMultiTeamEffectiveness(out List<EventMultiTeamEffectivenessDataModel> data)
	{
		return GetList(EVENT_MULTI_TEAM_EFFECTIVENESS_GENERATED_READER.sqlRequest + " ", out data, EVENT_MULTI_TEAM_EFFECTIVENESS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventMultiTeamEffectivenessDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventMultiTeamEffectivenessDataModel), EVENT_MULTI_TEAM_EFFECTIVENESS_GENERATED_READER);
	}

	private static void CreateEventMultiTeamResultThreshoReader(DataModelReader<EventMultiTeamResultThreshoDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("sprite_name", delegate
		{
			dmReader.model.spriteName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("threshold_value", delegate
		{
			dmReader.model.thresholdValue = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleEventMultiTeamResultThresho(string id, out EventMultiTeamResultThreshoDataModel m)
	{
		return GetSingle(EVENT_MULTI_TEAM_RESULT_THRESHO_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, EVENT_MULTI_TEAM_RESULT_THRESHO_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllEventMultiTeamResultThresho(out List<EventMultiTeamResultThreshoDataModel> data)
	{
		return GetList(EVENT_MULTI_TEAM_RESULT_THRESHO_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, EVENT_MULTI_TEAM_RESULT_THRESHO_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventMultiTeamResultThreshoDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventMultiTeamResultThreshoDataModel), EVENT_MULTI_TEAM_RESULT_THRESHO_GENERATED_READER);
	}

	private static void CreateEventPartsReader(DataModelReader<EventPartsDataModel> dmReader)
	{
		dmReader.AddItem("drop_max", delegate
		{
			dmReader.model.dropMax = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_min", delegate
		{
			dmReader.model.dropMin = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_rate", delegate
		{
			dmReader.model.dropRate = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_id", delegate
		{
			dmReader.model.eventId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("part_type", delegate
		{
			dmReader.model.partType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleEventParts(string id, out EventPartsDataModel m)
	{
		return GetSingle(EVENT_PARTS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, EVENT_PARTS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllEventParts(out List<EventPartsDataModel> data)
	{
		return GetList(EVENT_PARTS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, EVENT_PARTS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventPartsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventPartsDataModel), EVENT_PARTS_GENERATED_READER);
	}

	private static void CreateEventPointsBucketsReader(DataModelReader<EventPointsBucketsDataModel> dmReader)
	{
		dmReader.AddItem("event_id", delegate
		{
			dmReader.model.eventId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("points_end", delegate
		{
			dmReader.model.pointsEnd = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("points_start", delegate
		{
			dmReader.model.pointsStart = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllEventPointsBuckets(out List<EventPointsBucketsDataModel> data)
	{
		return GetList(EVENT_POINTS_BUCKETS_GENERATED_READER.sqlRequest + " ", out data, EVENT_POINTS_BUCKETS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventPointsBucketsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventPointsBucketsDataModel), EVENT_POINTS_BUCKETS_GENERATED_READER);
	}

	private static void CreateEventRaidbossDamageDropRateReader(DataModelReader<EventRaidbossDamageDropRateDataModel> dmReader)
	{
		dmReader.AddItem("boxtype", delegate
		{
			dmReader.model.boxtype = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("giftid", delegate
		{
			dmReader.model.giftid = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("teamid", delegate
		{
			dmReader.model.teamid = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("threshold", delegate
		{
			dmReader.model.threshold = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllEventRaidbossDamageDropRate(out List<EventRaidbossDamageDropRateDataModel> data)
	{
		return GetList(EVENT_RAIDBOSS_DAMAGE_DROP_RATE_GENERATED_READER.sqlRequest + " ", out data, EVENT_RAIDBOSS_DAMAGE_DROP_RATE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventRaidbossDamageDropRateDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventRaidbossDamageDropRateDataModel), EVENT_RAIDBOSS_DAMAGE_DROP_RATE_GENERATED_READER);
	}

	private static void CreateEventUnitBoostReader(DataModelReader<EventUnitBoostDataModel> dmReader)
	{
		dmReader.AddItem("ability1_boost_a", delegate
		{
			dmReader.model.ability1BoostA = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability1_boost_b", delegate
		{
			dmReader.model.ability1BoostB = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability1_override", delegate
		{
			dmReader.model.ability1Override = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability2_boost_a", delegate
		{
			dmReader.model.ability2BoostA = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability2_boost_b", delegate
		{
			dmReader.model.ability2BoostB = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("ability2_override", delegate
		{
			dmReader.model.ability2Override = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("bonus_points_boost", delegate
		{
			dmReader.model.bonusPointsBoost = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("die_boost_acid_strike", delegate
		{
			dmReader.model.dieBoostAcidStrike = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("die_boost_armour_piercing", delegate
		{
			dmReader.model.dieBoostArmourPiercing = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("die_boost_damage", delegate
		{
			dmReader.model.dieBoostDamage = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("die_boost_initiative", delegate
		{
			dmReader.model.dieBoostInitiative = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_id", delegate
		{
			dmReader.model.eventId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("hp_boost", delegate
		{
			dmReader.model.hpBoost = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("level", delegate
		{
			dmReader.model.level = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleEventUnitBoost(string id, out EventUnitBoostDataModel m)
	{
		return GetSingle(EVENT_UNIT_BOOST_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, EVENT_UNIT_BOOST_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllEventUnitBoost(out List<EventUnitBoostDataModel> data)
	{
		return GetList(EVENT_UNIT_BOOST_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, EVENT_UNIT_BOOST_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventUnitBoostDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventUnitBoostDataModel), EVENT_UNIT_BOOST_GENERATED_READER);
	}

	private static void CreateEventUnitsReader(DataModelReader<EventUnitsDataModel> dmReader)
	{
		dmReader.AddItem("can_build", delegate
		{
			dmReader.model.canBuild = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_id", delegate
		{
			dmReader.model.eventId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleEventUnits(string id, out EventUnitsDataModel m)
	{
		return GetSingle(EVENT_UNITS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, EVENT_UNITS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllEventUnits(out List<EventUnitsDataModel> data)
	{
		return GetList(EVENT_UNITS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, EVENT_UNITS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitEventUnitsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(EventUnitsDataModel), EVENT_UNITS_GENERATED_READER);
	}

	private static void CreateGachaAbTestingReader(DataModelReader<GachaAbTestingDataModel> dmReader)
	{
		dmReader.AddItem("enable", delegate
		{
			dmReader.model.enable = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("end_date", delegate
		{
			dmReader.model.endDate = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("groups_count", delegate
		{
			dmReader.model.groupsCount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("start_date", delegate
		{
			dmReader.model.startDate = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleGachaAbTesting(string id, out GachaAbTestingDataModel m)
	{
		return GetSingle(GACHA_AB_TESTING_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, GACHA_AB_TESTING_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllGachaAbTesting(out List<GachaAbTestingDataModel> data)
	{
		return GetList(GACHA_AB_TESTING_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, GACHA_AB_TESTING_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaAbTestingDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaAbTestingDataModel), GACHA_AB_TESTING_GENERATED_READER);
	}

	private static void CreateGachaAbTestingGroupEnableReader(DataModelReader<GachaAbTestingGroupEnableDataModel> dmReader)
	{
		dmReader.AddItem("ab_testing_id", delegate
		{
			dmReader.model.abTestingId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("group_number", delegate
		{
			dmReader.model.groupNumber = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
	}

	private Error GetSingleGachaAbTestingGroupEnable(string id, out GachaAbTestingGroupEnableDataModel m)
	{
		return GetSingle(GACHA_AB_TESTING_GROUP_ENABLE_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, GACHA_AB_TESTING_GROUP_ENABLE_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllGachaAbTestingGroupEnable(out List<GachaAbTestingGroupEnableDataModel> data)
	{
		return GetList(GACHA_AB_TESTING_GROUP_ENABLE_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, GACHA_AB_TESTING_GROUP_ENABLE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaAbTestingGroupEnableDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaAbTestingGroupEnableDataModel), GACHA_AB_TESTING_GROUP_ENABLE_GENERATED_READER);
	}

	private static void CreateGachaInfoDetailsReader(DataModelReader<GachaInfoDetailsDataModel> dmReader)
	{
		dmReader.AddItem("asset_id", delegate
		{
			dmReader.model.assetId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("display_order", delegate
		{
			dmReader.model.displayOrder = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_id", delegate
		{
			dmReader.model.gachaId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_text", delegate
		{
			dmReader.model.keyText = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("spacing", delegate
		{
			dmReader.model.spacing = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleGachaInfoDetails(string id, out GachaInfoDetailsDataModel m)
	{
		return GetSingle(GACHA_INFO_DETAILS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, GACHA_INFO_DETAILS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllGachaInfoDetails(out List<GachaInfoDetailsDataModel> data)
	{
		return GetList(GACHA_INFO_DETAILS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, GACHA_INFO_DETAILS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaInfoDetailsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaInfoDetailsDataModel), GACHA_INFO_DETAILS_GENERATED_READER);
	}

	private static void CreateGachaInfoItemsReader(DataModelReader<GachaInfoItemsDataModel> dmReader)
	{
		dmReader.AddItem("asset_linkage", delegate
		{
			dmReader.model.assetLinkage = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_asset_linkage_index", delegate
		{
			dmReader.model.eventAssetLinkageIndex = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_pool_id", delegate
		{
			dmReader.model.gachaPoolId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleGachaInfoItems(string id, out GachaInfoItemsDataModel m)
	{
		return GetSingle(GACHA_INFO_ITEMS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, GACHA_INFO_ITEMS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllGachaInfoItems(out List<GachaInfoItemsDataModel> data)
	{
		return GetList(GACHA_INFO_ITEMS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, GACHA_INFO_ITEMS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaInfoItemsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaInfoItemsDataModel), GACHA_INFO_ITEMS_GENERATED_READER);
	}

	private static void CreateGachaPlinkoDetailsReader(DataModelReader<GachaPlinkoDetailsDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleGachaPlinkoDetails(string id, out GachaPlinkoDetailsDataModel m)
	{
		return GetSingle(GACHA_PLINKO_DETAILS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, GACHA_PLINKO_DETAILS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllGachaPlinkoDetails(out List<GachaPlinkoDetailsDataModel> data)
	{
		return GetList(GACHA_PLINKO_DETAILS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, GACHA_PLINKO_DETAILS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaPlinkoDetailsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaPlinkoDetailsDataModel), GACHA_PLINKO_DETAILS_GENERATED_READER);
	}

	private static void CreateGachaPlinkoPrizePriceReader(DataModelReader<GachaPlinkoPrizePriceDataModel> dmReader)
	{
		dmReader.AddItem("amount", delegate
		{
			dmReader.model.amount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_id", delegate
		{
			dmReader.model.itemId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_type", delegate
		{
			dmReader.model.itemType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("order_number", delegate
		{
			dmReader.model.orderNumber = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("price_id", delegate
		{
			dmReader.model.priceId = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllGachaPlinkoPrizePrice(out List<GachaPlinkoPrizePriceDataModel> data)
	{
		return GetList(GACHA_PLINKO_PRIZE_PRICE_GENERATED_READER.sqlRequest + " ", out data, GACHA_PLINKO_PRIZE_PRICE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaPlinkoPrizePriceDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaPlinkoPrizePriceDataModel), GACHA_PLINKO_PRIZE_PRICE_GENERATED_READER);
	}

	private static void CreateGachaPlinkoPrizesReader(DataModelReader<GachaPlinkoPrizesDataModel> dmReader)
	{
		dmReader.AddItem("item_id", delegate
		{
			dmReader.model.itemId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_type", delegate
		{
			dmReader.model.itemType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("items_mixer_id", delegate
		{
			dmReader.model.itemsMixerId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("order_number", delegate
		{
			dmReader.model.orderNumber = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("price_id", delegate
		{
			dmReader.model.priceId = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllGachaPlinkoPrizes(out List<GachaPlinkoPrizesDataModel> data)
	{
		return GetList(GACHA_PLINKO_PRIZES_GENERATED_READER.sqlRequest + " ", out data, GACHA_PLINKO_PRIZES_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaPlinkoPrizesDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaPlinkoPrizesDataModel), GACHA_PLINKO_PRIZES_GENERATED_READER);
	}

	private static void CreateGachaPlinkoSlotChancesReader(DataModelReader<GachaPlinkoSlotChancesDataModel> dmReader)
	{
		dmReader.AddItem("chance_prize_slot_0", delegate
		{
			dmReader.model.chancePrizeSlot0 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_1", delegate
		{
			dmReader.model.chancePrizeSlot1 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_2", delegate
		{
			dmReader.model.chancePrizeSlot2 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_3", delegate
		{
			dmReader.model.chancePrizeSlot3 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_4", delegate
		{
			dmReader.model.chancePrizeSlot4 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_5", delegate
		{
			dmReader.model.chancePrizeSlot5 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_6", delegate
		{
			dmReader.model.chancePrizeSlot6 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_7", delegate
		{
			dmReader.model.chancePrizeSlot7 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("chance_prize_slot_8", delegate
		{
			dmReader.model.chancePrizeSlot8 = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("items_mixer_id", delegate
		{
			dmReader.model.itemsMixerId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("slot_index", delegate
		{
			dmReader.model.slotIndex = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllGachaPlinkoSlotChances(out List<GachaPlinkoSlotChancesDataModel> data)
	{
		return GetList(GACHA_PLINKO_SLOT_CHANCES_GENERATED_READER.sqlRequest + " ", out data, GACHA_PLINKO_SLOT_CHANCES_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaPlinkoSlotChancesDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaPlinkoSlotChancesDataModel), GACHA_PLINKO_SLOT_CHANCES_GENERATED_READER);
	}

	private static void CreateGachaPoolsReader(DataModelReader<GachaPoolsDataModel> dmReader)
	{
		dmReader.AddItem("ab_testing_id", delegate
		{
			dmReader.model.abTestingId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("asset_linkage_id", delegate
		{
			dmReader.model.assetLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("available_countdown", delegate
		{
			dmReader.model.availableCountdown = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("box_count", delegate
		{
			dmReader.model.boxCount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("bundle_gift_id", delegate
		{
			dmReader.model.bundleGiftId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("date_end", delegate
		{
			dmReader.model.dateEnd = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("date_start", delegate
		{
			dmReader.model.dateStart = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("event_id", delegate
		{
			dmReader.model.eventId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("free_cooldown", delegate
		{
			dmReader.model.freeCooldown = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gacha_type", delegate
		{
			dmReader.model.gachaType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("order_position", delegate
		{
			dmReader.model.orderPosition = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("price_id", delegate
		{
			dmReader.model.priceId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("size", delegate
		{
			dmReader.model.size = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("step_up_draw_limit", delegate
		{
			dmReader.model.stepUpDrawLimit = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("step_up_id", delegate
		{
			dmReader.model.stepUpId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("step_up_num", delegate
		{
			dmReader.model.stepUpNum = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unlock_division", delegate
		{
			dmReader.model.unlockDivision = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleGachaPools(string id, out GachaPoolsDataModel m)
	{
		return GetSingle(GACHA_POOLS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, GACHA_POOLS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllGachaPools(out List<GachaPoolsDataModel> data)
	{
		return GetList(GACHA_POOLS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, GACHA_POOLS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitGachaPoolsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(GachaPoolsDataModel), GACHA_POOLS_GENERATED_READER);
	}

	private static void CreateHelpRegistersReader(DataModelReader<HelpRegistersDataModel> dmReader)
	{
		dmReader.AddItem("description_key", delegate
		{
			dmReader.model.descriptionKey = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("name", delegate
		{
			dmReader.model.name = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("order_number", delegate
		{
			dmReader.model.orderNumber = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("title_key", delegate
		{
			dmReader.model.titleKey = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("topic_id", delegate
		{
			dmReader.model.topicId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleHelpRegisters(string id, out HelpRegistersDataModel m)
	{
		return GetSingle(HELP_REGISTERS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, HELP_REGISTERS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllHelpRegisters(out List<HelpRegistersDataModel> data)
	{
		return GetList(HELP_REGISTERS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, HELP_REGISTERS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitHelpRegistersDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(HelpRegistersDataModel), HELP_REGISTERS_GENERATED_READER);
	}

	private static void CreateHelpTopicReader(DataModelReader<HelpTopicDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("name", delegate
		{
			dmReader.model.name = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("title_key", delegate
		{
			dmReader.model.titleKey = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleHelpTopic(string id, out HelpTopicDataModel m)
	{
		return GetSingle(HELP_TOPIC_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, HELP_TOPIC_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllHelpTopic(out List<HelpTopicDataModel> data)
	{
		return GetList(HELP_TOPIC_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, HELP_TOPIC_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitHelpTopicDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(HelpTopicDataModel), HELP_TOPIC_GENERATED_READER);
	}

	private static void CreateItemReader(DataModelReader<ItemDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name_singular", delegate
		{
			dmReader.model.keyNameSingular = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleItem(string id, out ItemDataModel m)
	{
		return GetSingle(ITEM_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, ITEM_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllItem(out List<ItemDataModel> data)
	{
		return GetList(ITEM_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, ITEM_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitItemDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ItemDataModel), ITEM_GENERATED_READER);
	}

	private static void CreateItemGiftReader(DataModelReader<ItemGiftDataModel> dmReader)
	{
		dmReader.AddItem("amount", delegate
		{
			dmReader.model.amount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gift_id", delegate
		{
			dmReader.model.giftId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_id", delegate
		{
			dmReader.model.itemId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_type", delegate
		{
			dmReader.model.itemType = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllItemGift(out List<ItemGiftDataModel> data)
	{
		return GetList(ITEM_GIFT_GENERATED_READER.sqlRequest + " ", out data, ITEM_GIFT_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitItemGiftDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ItemGiftDataModel), ITEM_GIFT_GENERATED_READER);
	}

	private static void CreateItemPriceReader(DataModelReader<ItemPriceDataModel> dmReader)
	{
		dmReader.AddItem("amount", delegate
		{
			dmReader.model.amount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_id", delegate
		{
			dmReader.model.itemId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("item_type", delegate
		{
			dmReader.model.itemType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("price_id", delegate
		{
			dmReader.model.priceId = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllItemPrice(out List<ItemPriceDataModel> data)
	{
		return GetList(ITEM_PRICE_GENERATED_READER.sqlRequest + " ", out data, ITEM_PRICE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitItemPriceDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ItemPriceDataModel), ITEM_PRICE_GENERATED_READER);
	}

	private static void CreateLeaderboardRewardsReader(DataModelReader<LeaderboardRewardsDataModel> dmReader)
	{
		dmReader.AddItem("gift_package_id", delegate
		{
			dmReader.model.giftPackageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("rank_end", delegate
		{
			dmReader.model.rankEnd = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("rank_start", delegate
		{
			dmReader.model.rankStart = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("rewards_id", delegate
		{
			dmReader.model.rewardsId = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllLeaderboardRewards(out List<LeaderboardRewardsDataModel> data)
	{
		return GetList(LEADERBOARD_REWARDS_GENERATED_READER.sqlRequest + " ", out data, LEADERBOARD_REWARDS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitLeaderboardRewardsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(LeaderboardRewardsDataModel), LEADERBOARD_REWARDS_GENERATED_READER);
	}

	private static void CreateLeaderboardsReader(DataModelReader<LeaderboardsDataModel> dmReader)
	{
		dmReader.AddItem("date_end", delegate
		{
			dmReader.model.dateEnd = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("date_start", delegate
		{
			dmReader.model.dateStart = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("group_id", delegate
		{
			dmReader.model.groupId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("rewards_id", delegate
		{
			dmReader.model.rewardsId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("tier_end", delegate
		{
			dmReader.model.tierEnd = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("tier_start", delegate
		{
			dmReader.model.tierStart = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleLeaderboards(string id, out LeaderboardsDataModel m)
	{
		return GetSingle(LEADERBOARDS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, LEADERBOARDS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllLeaderboards(out List<LeaderboardsDataModel> data)
	{
		return GetList(LEADERBOARDS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, LEADERBOARDS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitLeaderboardsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(LeaderboardsDataModel), LEADERBOARDS_GENERATED_READER);
	}

	private static void CreateLinesEmoticonsReader(DataModelReader<LinesEmoticonsDataModel> dmReader)
	{
		dmReader.AddItem("emoticon_key_string", delegate
		{
			dmReader.model.emoticonKeyString = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("emoticon_type", delegate
		{
			dmReader.model.emoticonType = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllLinesEmoticons(out List<LinesEmoticonsDataModel> data)
	{
		return GetList(LINES_EMOTICONS_GENERATED_READER.sqlRequest + " ", out data, LINES_EMOTICONS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitLinesEmoticonsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(LinesEmoticonsDataModel), LINES_EMOTICONS_GENERATED_READER);
	}

	private static void CreateLinesNewsReader(DataModelReader<LinesNewsDataModel> dmReader)
	{
		dmReader.AddItem("news_key_string", delegate
		{
			dmReader.model.newsKeyString = dmReader.ReadOneValue<string>();
		});
	}

	public Error GetAllLinesNews(out List<LinesNewsDataModel> data)
	{
		return GetList(LINES_NEWS_GENERATED_READER.sqlRequest + " ", out data, LINES_NEWS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitLinesNewsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(LinesNewsDataModel), LINES_NEWS_GENERATED_READER);
	}

	private static void CreateLinesProTipsReader(DataModelReader<LinesProTipsDataModel> dmReader)
	{
		dmReader.AddItem("pro_tips_key_string", delegate
		{
			dmReader.model.proTipsKeyString = dmReader.ReadOneValue<string>();
		});
	}

	public Error GetAllLinesProTips(out List<LinesProTipsDataModel> data)
	{
		return GetList(LINES_PRO_TIPS_GENERATED_READER.sqlRequest + " ", out data, LINES_PRO_TIPS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitLinesProTipsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(LinesProTipsDataModel), LINES_PRO_TIPS_GENERATED_READER);
	}

	private static void CreateNewsReader(DataModelReader<NewsDataModel> dmReader)
	{
		dmReader.AddItem("announcer_type", delegate
		{
			dmReader.model.announcerType = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("asset_linkage_id", delegate
		{
			dmReader.model.assetLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("background", delegate
		{
			dmReader.model.background = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("background_speed", delegate
		{
			dmReader.model.backgroundSpeed = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("end_time", delegate
		{
			dmReader.model.endTime = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("gacha_id", delegate
		{
			dmReader.model.gachaId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("order_number", delegate
		{
			dmReader.model.orderNumber = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("show_number", delegate
		{
			dmReader.model.showNumber = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("start_time", delegate
		{
			dmReader.model.startTime = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("text_description_1", delegate
		{
			dmReader.model.textDescription1 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("text_description_2", delegate
		{
			dmReader.model.textDescription2 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("text_description_3", delegate
		{
			dmReader.model.textDescription3 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("text_description_4", delegate
		{
			dmReader.model.textDescription4 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("text_description_5", delegate
		{
			dmReader.model.textDescription5 = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("title", delegate
		{
			dmReader.model.title = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleNews(string id, out NewsDataModel m)
	{
		return GetSingle(NEWS_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, NEWS_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllNews(out List<NewsDataModel> data)
	{
		return GetList(NEWS_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, NEWS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitNewsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(NewsDataModel), NEWS_GENERATED_READER);
	}

	private static void CreateProgressionDivisionReader(DataModelReader<ProgressionDivisionDataModel> dmReader)
	{
		dmReader.AddItem("badge_linkage_id", delegate
		{
			dmReader.model.badgeLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("base_lose_coin_reward", delegate
		{
			dmReader.model.baseLoseCoinReward = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("base_win_coin_reward", delegate
		{
			dmReader.model.baseWinCoinReward = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("event_point_pvp_bonus", delegate
		{
			dmReader.model.eventPointPvpBonus = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gift_package_id", delegate
		{
			dmReader.model.giftPackageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("has_promotion_series", delegate
		{
			dmReader.model.hasPromotionSeries = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("is_hidden", delegate
		{
			dmReader.model.isHidden = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("lose_point", delegate
		{
			dmReader.model.losePoint = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("promotion_series_id", delegate
		{
			dmReader.model.promotionSeriesId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("reward_amount", delegate
		{
			dmReader.model.rewardAmount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("reward_type_id", delegate
		{
			dmReader.model.rewardTypeId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("total_point_to_promotion_series", delegate
		{
			dmReader.model.totalPointToPromotionSeries = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("total_point_to_reward", delegate
		{
			dmReader.model.totalPointToReward = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_destroy_coin_reward", delegate
		{
			dmReader.model.unitDestroyCoinReward = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_survive_coin_reward", delegate
		{
			dmReader.model.unitSurviveCoinReward = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("win_event_point", delegate
		{
			dmReader.model.winEventPoint = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("win_point", delegate
		{
			dmReader.model.winPoint = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleProgressionDivision(string id, out ProgressionDivisionDataModel m)
	{
		return GetSingle(PROGRESSION_DIVISION_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, PROGRESSION_DIVISION_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllProgressionDivision(out List<ProgressionDivisionDataModel> data)
	{
		return GetList(PROGRESSION_DIVISION_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, PROGRESSION_DIVISION_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitProgressionDivisionDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ProgressionDivisionDataModel), PROGRESSION_DIVISION_GENERATED_READER);
	}

	private static void CreateProgressionPromotionSeriesReader(DataModelReader<ProgressionPromotionSeriesDataModel> dmReader)
	{
		dmReader.AddItem("current_division_id", delegate
		{
			dmReader.model.currentDivisionId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gift_id", delegate
		{
			dmReader.model.giftId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("promotion_division_id", delegate
		{
			dmReader.model.promotionDivisionId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("total_battle", delegate
		{
			dmReader.model.totalBattle = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("total_win", delegate
		{
			dmReader.model.totalWin = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleProgressionPromotionSeries(string id, out ProgressionPromotionSeriesDataModel m)
	{
		return GetSingle(PROGRESSION_PROMOTION_SERIES_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, PROGRESSION_PROMOTION_SERIES_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllProgressionPromotionSeries(out List<ProgressionPromotionSeriesDataModel> data)
	{
		return GetList(PROGRESSION_PROMOTION_SERIES_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, PROGRESSION_PROMOTION_SERIES_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitProgressionPromotionSeriesDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(ProgressionPromotionSeriesDataModel), PROGRESSION_PROMOTION_SERIES_GENERATED_READER);
	}

	private static void CreateUnitReader(DataModelReader<UnitDataModel> dmReader)
	{
		dmReader.AddItem("blueprint_linkage_id", delegate
		{
			dmReader.model.blueprintLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("can_buy_direct", delegate
		{
			dmReader.model.canBuyDirect = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("found_in_gacha", delegate
		{
			dmReader.model.foundInGacha = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("rarity", delegate
		{
			dmReader.model.rarity = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("research_time", delegate
		{
			dmReader.model.researchTime = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("reward_amount", delegate
		{
			dmReader.model.rewardAmount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("reward_type_id", delegate
		{
			dmReader.model.rewardTypeId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("type", delegate
		{
			dmReader.model.type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_index", delegate
		{
			dmReader.model.unitIndex = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unlock_tier", delegate
		{
			dmReader.model.unlockTier = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("weapon_anim", delegate
		{
			dmReader.model.weaponAnim = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnit(string id, out UnitDataModel m)
	{
		return GetSingle(UNIT_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnit(out List<UnitDataModel> data)
	{
		return GetList(UNIT_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitDataModel), UNIT_GENERATED_READER);
	}

	private static void CreateUnitActionReader(DataModelReader<UnitActionDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
	}

	private Error GetSingleUnitAction(string id, out UnitActionDataModel m)
	{
		return GetSingle(UNIT_ACTION_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_ACTION_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitAction(out List<UnitActionDataModel> data)
	{
		return GetList(UNIT_ACTION_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_ACTION_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitActionDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitActionDataModel), UNIT_ACTION_GENERATED_READER);
	}

	private static void CreateUnitCooldownReader(DataModelReader<UnitCooldownDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("rarity", delegate
		{
			dmReader.model.rarity = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("seconds", delegate
		{
			dmReader.model.seconds = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_level", delegate
		{
			dmReader.model.unitLevel = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnitCooldown(string id, out UnitCooldownDataModel m)
	{
		return GetSingle(UNIT_COOLDOWN_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_COOLDOWN_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitCooldown(out List<UnitCooldownDataModel> data)
	{
		return GetList(UNIT_COOLDOWN_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_COOLDOWN_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitCooldownDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitCooldownDataModel), UNIT_COOLDOWN_GENERATED_READER);
	}

	private static void CreateUnitDestroyGemDropReader(DataModelReader<UnitDestroyGemDropDataModel> dmReader)
	{
		dmReader.AddItem("gem_proc_rate", delegate
		{
			dmReader.model.gemProcRate = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("max_gem_num", delegate
		{
			dmReader.model.maxGemNum = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("min_gem_num", delegate
		{
			dmReader.model.minGemNum = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllUnitDestroyGemDrop(out List<UnitDestroyGemDropDataModel> data)
	{
		return GetList(UNIT_DESTROY_GEM_DROP_GENERATED_READER.sqlRequest + " ", out data, UNIT_DESTROY_GEM_DROP_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitDestroyGemDropDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitDestroyGemDropDataModel), UNIT_DESTROY_GEM_DROP_GENERATED_READER);
	}

	private static void CreateUnitLevelProgressionReader(DataModelReader<UnitLevelProgressionDataModel> dmReader)
	{
		dmReader.AddItem("alternative_weapon", delegate
		{
			dmReader.model.alternativeWeapon = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("asset_bundle_id", delegate
		{
			dmReader.model.assetBundleId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("evolution_stage", delegate
		{
			dmReader.model.evolutionStage = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_1_type", delegate
		{
			dmReader.model.face1Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_1_value", delegate
		{
			dmReader.model.face1Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_2_type", delegate
		{
			dmReader.model.face2Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_2_value", delegate
		{
			dmReader.model.face2Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_3_type", delegate
		{
			dmReader.model.face3Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_3_value", delegate
		{
			dmReader.model.face3Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_4_type", delegate
		{
			dmReader.model.face4Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_4_value", delegate
		{
			dmReader.model.face4Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_5_type", delegate
		{
			dmReader.model.face5Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_5_value", delegate
		{
			dmReader.model.face5Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("hp", delegate
		{
			dmReader.model.hp = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("is_skin", delegate
		{
			dmReader.model.isSkin = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("killed_cash", delegate
		{
			dmReader.model.killedCash = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("killed_event_point", delegate
		{
			dmReader.model.killedEventPoint = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("killed_point", delegate
		{
			dmReader.model.killedPoint = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("level", delegate
		{
			dmReader.model.level = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("level_up_requirement_id", delegate
		{
			dmReader.model.levelUpRequirementId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("on_level_gift_id", delegate
		{
			dmReader.model.onLevelGiftId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("passive_boost_value_a", delegate
		{
			dmReader.model.passiveBoostValueA = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("passive_boost_value_b", delegate
		{
			dmReader.model.passiveBoostValueB = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("passive_id", delegate
		{
			dmReader.model.passiveId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("price_id", delegate
		{
			dmReader.model.priceId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("rarity", delegate
		{
			dmReader.model.rarity = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("special_boost_value_a", delegate
		{
			dmReader.model.specialBoostValueA = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("special_boost_value_b", delegate
		{
			dmReader.model.specialBoostValueB = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("special_id", delegate
		{
			dmReader.model.specialId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("survived_cash", delegate
		{
			dmReader.model.survivedCash = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("survived_point", delegate
		{
			dmReader.model.survivedPoint = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnitLevelProgression(string id, out UnitLevelProgressionDataModel m)
	{
		return GetSingle(UNIT_LEVEL_PROGRESSION_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_LEVEL_PROGRESSION_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitLevelProgression(out List<UnitLevelProgressionDataModel> data)
	{
		return GetList(UNIT_LEVEL_PROGRESSION_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_LEVEL_PROGRESSION_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitLevelProgressionDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitLevelProgressionDataModel), UNIT_LEVEL_PROGRESSION_GENERATED_READER);
	}

	private static void CreateUnitLevelUpRequirementReader(DataModelReader<UnitLevelUpRequirementDataModel> dmReader)
	{
		dmReader.AddItem("current_level", delegate
		{
			dmReader.model.currentLevel = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gem_to_cash_rate", delegate
		{
			dmReader.model.gemToCashRate = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("killed_cash", delegate
		{
			dmReader.model.killedCash = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("killed_event_points", delegate
		{
			dmReader.model.killedEventPoints = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("killed_points", delegate
		{
			dmReader.model.killedPoints = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("next_level", delegate
		{
			dmReader.model.nextLevel = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("price_id", delegate
		{
			dmReader.model.priceId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("rarity", delegate
		{
			dmReader.model.rarity = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("survived_cash", delegate
		{
			dmReader.model.survivedCash = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("survived_points", delegate
		{
			dmReader.model.survivedPoints = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnitLevelUpRequirement(string id, out UnitLevelUpRequirementDataModel m)
	{
		return GetSingle(UNIT_LEVEL_UP_REQUIREMENT_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_LEVEL_UP_REQUIREMENT_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitLevelUpRequirement(out List<UnitLevelUpRequirementDataModel> data)
	{
		return GetList(UNIT_LEVEL_UP_REQUIREMENT_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_LEVEL_UP_REQUIREMENT_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitLevelUpRequirementDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitLevelUpRequirementDataModel), UNIT_LEVEL_UP_REQUIREMENT_GENERATED_READER);
	}

	private static void CreateUnitPartTypesReader(DataModelReader<UnitPartTypesDataModel> dmReader)
	{
		dmReader.AddItem("asset_linkage_id", delegate
		{
			dmReader.model.assetLinkageId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("gem_value", delegate
		{
			dmReader.model.gemValue = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("is_hidden", delegate
		{
			dmReader.model.isHidden = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("name", delegate
		{
			dmReader.model.name = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("order_by", delegate
		{
			dmReader.model.orderBy = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("rarity", delegate
		{
			dmReader.model.rarity = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnitPartTypes(string id, out UnitPartTypesDataModel m)
	{
		return GetSingle(UNIT_PART_TYPES_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_PART_TYPES_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitPartTypes(out List<UnitPartTypesDataModel> data)
	{
		return GetList(UNIT_PART_TYPES_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_PART_TYPES_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitPartTypesDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitPartTypesDataModel), UNIT_PART_TYPES_GENERATED_READER);
	}

	private static void CreateUnitPartialLevelReader(DataModelReader<UnitPartialLevelDataModel> dmReader)
	{
		dmReader.AddItem("face_1_type", delegate
		{
			dmReader.model.face1Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_1_value", delegate
		{
			dmReader.model.face1Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_2_type", delegate
		{
			dmReader.model.face2Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_2_value", delegate
		{
			dmReader.model.face2Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_3_type", delegate
		{
			dmReader.model.face3Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_3_value", delegate
		{
			dmReader.model.face3Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_4_type", delegate
		{
			dmReader.model.face4Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_4_value", delegate
		{
			dmReader.model.face4Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_5_type", delegate
		{
			dmReader.model.face5Type = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("face_5_value", delegate
		{
			dmReader.model.face5Value = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("hp", delegate
		{
			dmReader.model.hp = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("level", delegate
		{
			dmReader.model.level = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("part_index", delegate
		{
			dmReader.model.partIndex = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("passive_boost_value_a", delegate
		{
			dmReader.model.passiveBoostValueA = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("passive_boost_value_b", delegate
		{
			dmReader.model.passiveBoostValueB = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("passive_id", delegate
		{
			dmReader.model.passiveId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("requirement_price_id", delegate
		{
			dmReader.model.requirementPriceId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("requirement_tier", delegate
		{
			dmReader.model.requirementTier = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("special_boost_value_a", delegate
		{
			dmReader.model.specialBoostValueA = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("special_boost_value_b", delegate
		{
			dmReader.model.specialBoostValueB = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("special_id", delegate
		{
			dmReader.model.specialId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnitPartialLevel(string id, out UnitPartialLevelDataModel m)
	{
		return GetSingle(UNIT_PARTIAL_LEVEL_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_PARTIAL_LEVEL_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitPartialLevel(out List<UnitPartialLevelDataModel> data)
	{
		return GetList(UNIT_PARTIAL_LEVEL_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_PARTIAL_LEVEL_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitPartialLevelDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitPartialLevelDataModel), UNIT_PARTIAL_LEVEL_GENERATED_READER);
	}

	private static void CreateUnitPartsReader(DataModelReader<UnitPartsDataModel> dmReader)
	{
		dmReader.AddItem("amount", delegate
		{
			dmReader.model.amount = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_max", delegate
		{
			dmReader.model.dropMax = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_min", delegate
		{
			dmReader.model.dropMin = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("drop_rate", delegate
		{
			dmReader.model.dropRate = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("part_type", delegate
		{
			dmReader.model.partType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_id", delegate
		{
			dmReader.model.unitId = dmReader.ReadOneValue<int>();
		});
	}

	public Error GetAllUnitParts(out List<UnitPartsDataModel> data)
	{
		return GetList(UNIT_PARTS_GENERATED_READER.sqlRequest + " ", out data, UNIT_PARTS_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitPartsDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitPartsDataModel), UNIT_PARTS_GENERATED_READER);
	}

	private static void CreateUnitRarityReader(DataModelReader<UnitRarityDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleUnitRarity(string id, out UnitRarityDataModel m)
	{
		return GetSingle(UNIT_RARITY_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_RARITY_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitRarity(out List<UnitRarityDataModel> data)
	{
		return GetList(UNIT_RARITY_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_RARITY_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitRarityDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitRarityDataModel), UNIT_RARITY_GENERATED_READER);
	}

	private static void CreateUnitScrapValueReader(DataModelReader<UnitScrapValueDataModel> dmReader)
	{
		dmReader.AddItem("gift_id", delegate
		{
			dmReader.model.giftId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("rarity", delegate
		{
			dmReader.model.rarity = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("unit_level", delegate
		{
			dmReader.model.unitLevel = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnitScrapValue(string id, out UnitScrapValueDataModel m)
	{
		return GetSingle(UNIT_SCRAP_VALUE_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_SCRAP_VALUE_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitScrapValue(out List<UnitScrapValueDataModel> data)
	{
		return GetList(UNIT_SCRAP_VALUE_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_SCRAP_VALUE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitScrapValueDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitScrapValueDataModel), UNIT_SCRAP_VALUE_GENERATED_READER);
	}

	private static void CreateUnitSpecialReader(DataModelReader<UnitSpecialDataModel> dmReader)
	{
		dmReader.AddItem("action_boost_type", delegate
		{
			dmReader.model.actionBoostType = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("asset_bundle_id", delegate
		{
			dmReader.model.assetBundleId = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("execution_order", delegate
		{
			dmReader.model.executionOrder = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("handler_id", delegate
		{
			dmReader.model.handlerId = dmReader.ReadOneValue<int>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_description", delegate
		{
			dmReader.model.keyDescription = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("related_action_id", delegate
		{
			dmReader.model.relatedActionId = dmReader.ReadOneValue<int>();
		});
	}

	private Error GetSingleUnitSpecial(string id, out UnitSpecialDataModel m)
	{
		return GetSingle(UNIT_SPECIAL_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_SPECIAL_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitSpecial(out List<UnitSpecialDataModel> data)
	{
		return GetList(UNIT_SPECIAL_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_SPECIAL_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitSpecialDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitSpecialDataModel), UNIT_SPECIAL_GENERATED_READER);
	}

	private static void CreateUnitSpecialHandlerReader(DataModelReader<UnitSpecialHandlerDataModel> dmReader)
	{
		dmReader.AddItem("handler", delegate
		{
			dmReader.model.handler = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
	}

	private Error GetSingleUnitSpecialHandler(string id, out UnitSpecialHandlerDataModel m)
	{
		return GetSingle(UNIT_SPECIAL_HANDLER_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_SPECIAL_HANDLER_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitSpecialHandler(out List<UnitSpecialHandlerDataModel> data)
	{
		return GetList(UNIT_SPECIAL_HANDLER_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_SPECIAL_HANDLER_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitSpecialHandlerDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitSpecialHandlerDataModel), UNIT_SPECIAL_HANDLER_GENERATED_READER);
	}

	private static void CreateUnitTypeReader(DataModelReader<UnitTypeDataModel> dmReader)
	{
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
		dmReader.AddItem("key_name", delegate
		{
			dmReader.model.keyName = dmReader.ReadOneValue<string>();
		});
	}

	private Error GetSingleUnitType(string id, out UnitTypeDataModel m)
	{
		return GetSingle(UNIT_TYPE_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_TYPE_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitType(out List<UnitTypeDataModel> data)
	{
		return GetList(UNIT_TYPE_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_TYPE_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitTypeDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitTypeDataModel), UNIT_TYPE_GENERATED_READER);
	}

	private static void CreateUnitWeaponAnimReader(DataModelReader<UnitWeaponAnimDataModel> dmReader)
	{
		dmReader.AddItem("anim_name", delegate
		{
			dmReader.model.animName = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("id", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<int>().ToString();
		});
	}

	private Error GetSingleUnitWeaponAnim(string id, out UnitWeaponAnimDataModel m)
	{
		return GetSingle(UNIT_WEAPON_ANIM_GENERATED_READER.sqlRequest + " WHERE id='" + id + "'", out m, UNIT_WEAPON_ANIM_GENERATED_READER.MakeSqlRequest);
	}

	public Error GetAllUnitWeaponAnim(out List<UnitWeaponAnimDataModel> data)
	{
		return GetList(UNIT_WEAPON_ANIM_GENERATED_READER.sqlRequest + " ORDER By id DESC", out data, UNIT_WEAPON_ANIM_GENERATED_READER.MakeSqlRequest);
	}

	private static void InitUnitWeaponAnimDataModelReader()
	{
		TYPE_TO_READER.Add(typeof(UnitWeaponAnimDataModel), UNIT_WEAPON_ANIM_GENERATED_READER);
	}

	private static void CreateLocalizationItemReader(DataModelReader<LocalizationItemDataModel> dmReader)
	{
		dmReader.AddItem("key", delegate
		{
			dmReader.model.id = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("en", delegate
		{
			dmReader.model.strings[0] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("fr", delegate
		{
			dmReader.model.strings[1] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("it", delegate
		{
			dmReader.model.strings[2] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("de", delegate
		{
			dmReader.model.strings[3] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("es", delegate
		{
			dmReader.model.strings[4] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("pt", delegate
		{
			dmReader.model.strings[5] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("jp", delegate
		{
			dmReader.model.strings[6] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("sch", delegate
		{
			dmReader.model.strings[7] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("tch", delegate
		{
			dmReader.model.strings[11] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("tr", delegate
		{
			dmReader.model.strings[8] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("ko", delegate
		{
			dmReader.model.strings[10] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("ru", delegate
		{
			dmReader.model.strings[9] = dmReader.ReadOneValue<string>();
		});
		dmReader.AddItem("test_longest", delegate
		{
			dmReader.model.strings[12] = dmReader.ReadOneValue<string>();
		});
	}

	public Error GetAllLocalizationItems(out List<LocalizationItemDataModel> localizationItems)
	{
		List<DataModelReader<LocalizationItemDataModel>> list = new List<DataModelReader<LocalizationItemDataModel>>();
		list.Add(LOCALIZATION_UI_SELECT_SQL);
		localizationItems = new List<LocalizationItemDataModel>();
		foreach (DataModelReader<LocalizationItemDataModel> item in list)
		{
			List<LocalizationItemDataModel> list3;
			Error list2 = GetList(item.sqlRequest, out list3, item.MakeSqlRequest);
			if (list2 != null)
			{
				return list2;
			}
			localizationItems.AddRange(list3);
		}
		return null;
	}

	private Dictionary<string, string> ReadTutorialKeyValueDataModel(SqliteDataReader reader)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		do
		{
			dictionary.Add(reader.GetString(0), reader.GetString(1));
		}
		while (reader.Read());
		return dictionary;
	}
}
