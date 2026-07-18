using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotBattlePopUpController : PopupController
{
	public class PayloadData
	{
		public UnitPartTypesDataModel partInfo;

		public PayloadData(UnitPartTypesDataModel partInfo)
		{
			this.partInfo = partInfo;
		}
	}

	[SerializeField]
	private Camera _popUpCamera;

	[SerializeField]
	private Camera _hudCamera;

	[SerializeField]
	private Camera _playerCamera;

	[SerializeField]
	private Camera _enemyCamera;

	[SerializeField]
	private tk2dTextMesh _dailyLimitText;

	[SerializeField]
	private BackgroundsController _backgroundController;

	[SerializeField]
	private CooldownsController cooldownsController;

	[SerializeField]
	private UnitInfoView[] _playerUnits;

	[SerializeField]
	private UnitInfoView[] _enemyUnits;

	[SerializeField]
	private PriceLabelController _priceLableController;

	[SerializeField]
	private tk2dUIItem autoBattleButton;

	private PayloadData payloadData;

	protected AiArmyDataModel _botOpponent;

	private bool _autoBattle;

	protected override void Start()
	{
		base.Start();
		payloadData = (PayloadData)model.payload;
		if (SceneTransitionManager.CurrentSceneDM._scene == SceneTransitionManager.Scene.EditTeamUnitsScene)
		{
			((EditTeamUnitsSceneController)SceneTransitionManager.CurrentSceneDM.controller).TriggerSaveToProfile();
		}
		ConfigureCameras(true);
		List<AiArmyPartsDataModel> list = (from x in AiArmyPartsDataModel.GetAll()
			where x.partId.ToString() == payloadData.partInfo.id
			select x).ToList();
		if (list.Count > 0)
		{
			_botOpponent = AiArmyDataModel.GetSingle(list[0].aiArmyId);
		}
		else
		{
			_botOpponent = UserProfile.player.NextAIArmies[0];
		}
		cooldownsController.Init();
		cooldownsController.SelectedIndex = UserProfile.player.currentTeamIndex;
		cooldownsController.OnControllerCooldownFinished += OnControllerCooldownFinished;
		ConfigureParts();
		_backgroundController.SetBackgroundsRandom();
		ConfigureTeamPlayer(UserProfile.player.CurrentTeam);
		StartCoroutine(ConfigureTeamEnemy());
		ConfigureDailyLimit();
		StartCoroutine(AnnouncerController.DialogTrigger("FirstBotBattleScreen"));
	}

	private void ConfigureCameras(bool show)
	{
		if (show)
		{
			_playerCamera.depth = _popUpCamera.depth + 2f;
			_enemyCamera.depth = _popUpCamera.depth + 1f;
			_hudCamera.depth = _popUpCamera.depth + 3f;
		}
		else
		{
			_playerCamera.depth = _popUpCamera.depth;
			_enemyCamera.depth = _popUpCamera.depth;
			_hudCamera.depth = _popUpCamera.depth;
		}
	}

	private void ConfigureTeamPlayer(UserTeam team)
	{
		for (int i = 0; i < team.units.Count; i++)
		{
			UserUnit userUnit = team.units[i];
			if (userUnit != null)
			{
				_playerUnits[i].SetState(true);
				_playerUnits[i].ConfigureUnitView(userUnit.UnitDataModel, userUnit.level, 0, userUnit.IsOnCooldown);
			}
			else
			{
				_playerUnits[i].SetState(false);
			}
		}
	}

	private IEnumerator ConfigureTeamEnemy()
	{
		List<UserUnit> unitsList = _botOpponent.GetUnitList();
		for (int i = 0; i < unitsList.Count; i++)
		{
			_enemyUnits[i].SetState(true);
			_enemyUnits[i].ConfigureUnitView(unitsList[i].UnitDataModel, unitsList[i].level, 0, unitsList[i].IsOnCooldown);
		}
		for (int j = 0; j < unitsList.Count; j++)
		{
			while (!_enemyUnits[j].unitProxy.AssetReady)
			{
				yield return 0;
			}
			_enemyUnits[j].unitProxy.transform.SetLocalXScale(-1f);
		}
	}

	private void ConfigureDailyLimit()
	{
		_dailyLimitText.text = string.Format("ui_botBattle_dailyLimit".Localize("Daily Limit: {0}/{1}"), Constants.MaxBostBattlesPerDay - UserProfile.player.botBattleCount, Constants.MaxBostBattlesPerDay);
	}

	private void ConfigureParts()
	{
		if ((bool)_priceLableController)
		{
			_priceLableController.ConfigurePriceLabel(GetAvailableParts(_botOpponent.GetUnitList()));
		}
	}

	private void OnBattlePressed()
	{
		if (UserProfile.player.botBattleCount >= Constants.MaxBostBattlesPerDay)
		{
			ConfigureCameras(false);
			int cost = UserProfile.player.botBattleRestoreCount * Constants.BotBattleDailyCapRestoreCost;
			int count = UserProfile.player.botBattleRestoreCount;
			PopupManager.ShowPopup(PopupDataModel.ResetBotBattleCountPopUp(delegate
			{
				ConfigureCameras(true);
				ConfigureDailyLimit();
				Reporting.BotTeamQuotaHit("Purchased", cost, count);
				StartCoroutine(OnBattlePressedPostPopup());
			}, delegate
			{
				autoBattleButton.enabled = true;
			}, delegate
			{
				ConfigureDailyLimit();
				Reporting.BotTeamQuotaHit("Cancelled", cost, count);
				ConfigureCameras(true);
				autoBattleButton.enabled = true;
			}));
		}
		else if (UserProfile.player.CurrentTeam.IsOnCooldown)
		{
			ConfigureCameras(false);
			PopupManager.ShowPopup(PopupDataModel.SkipCooldownPopup(UserProfile.player.CurrentTeam, delegate
			{
				StartCoroutine(GoToBattlePostPopup());
			}, delegate
			{
				autoBattleButton.enabled = true;
			}, delegate
			{
				ConfigureCameras(true);
				autoBattleButton.enabled = true;
			}));
		}
		else
		{
			GoToBattle();
		}
	}

	private IEnumerator OnBattlePressedPostPopup()
	{
		yield return new WaitForEndOfFrame();
		OnBattlePressed();
	}

	private void OnAutoBattlePressed()
	{
		_autoBattle = true;
		autoBattleButton.enabled = false;
		OnBattlePressed();
	}

	private void OnToggleButtonGroupChanged(tk2dUIToggleButtonGroup toggleButtonGroup)
	{
		SetTeamIndex(toggleButtonGroup.SelectedIndex);
	}

	private void SetTeamIndex(int index)
	{
		if (UserProfile.player.teams[index].GetUnitCount() < Constants.MinUnitsPerTeam)
		{
			ConfigureCameras(false);
			EditTeamUnitsSceneModel sceneModel = new EditTeamUnitsSceneModel
			{
				selectedTeamIndex = UserProfile.player.currentTeamIndex
			};
			PopupManager.ShowPopup(PopupDataModel.Full("ui_popup_notenoughunits_title".Localize("Not enough units!"), string.Format("ui_popup_notenoughunits_desc".Localize("You must have {0} units to battle!"), Constants.MinUnitsPerTeam), "ui_topbar_team_button".Localize("Edit Team"), delegate
			{
				PopupManager.DestroyAllPopups();
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.EditTeamUnitsScene, sceneModel);
			}, "ui_popup_OK".Localize("OK"), delegate
			{
				for (int i = 0; i < UserProfile.player.teams.Count; i++)
				{
					if (UserProfile.player.teams[i].GetUnitCount() >= Constants.MinUnitsPerTeam)
					{
						UserProfile.player.currentTeamIndex = i;
						cooldownsController.SelectedIndex = UserProfile.player.currentTeamIndex;
						ConfigureTeamPlayer(UserProfile.player.CurrentTeam);
						ConfigureCameras(true);
						return;
					}
				}
				PopupManager.DestroyAllPopups();
			}, PopUpTypes.GENERIC, delegate
			{
				for (int i = 0; i < UserProfile.player.teams.Count; i++)
				{
					if (UserProfile.player.teams[i].GetUnitCount() >= Constants.MinUnitsPerTeam)
					{
						UserProfile.player.currentTeamIndex = i;
						cooldownsController.SelectedIndex = UserProfile.player.currentTeamIndex;
						ConfigureTeamPlayer(UserProfile.player.CurrentTeam);
						ConfigureCameras(true);
						return;
					}
				}
				PopupManager.DestroyAllPopups();
			}));
		}
		else
		{
			UserProfile.player.currentTeamIndex = index;
			cooldownsController.SelectedIndex = UserProfile.player.currentTeamIndex;
			ConfigureTeamPlayer(UserProfile.player.CurrentTeam);
		}
	}

	private IEnumerator GoToBattlePostPopup()
	{
		yield return new WaitForEndOfFrame();
		GoToBattle();
	}

	private void GoToBattle()
	{
		if (UserProfile.player.divisionInt >= Constants.MinTierForShowBoostSelection)
		{
			ConfigureCameras(false);
			PopupManager.ShowPopup(PopupDataModel.NormalTicketBoostPopUp(delegate(BoostType boostType)
			{
				BattleButtonLogic(boostType);
			}, delegate
			{
				ConfigureCameras(true);
				autoBattleButton.enabled = true;
			}));
		}
		else if (UserProfile.player.energy >= 1)
		{
			BattleButtonLogic(BoostType.NoBoost);
		}
		else
		{
			ConfigureCameras(false);
			TopBarController.instance.BuyEnergyThenDoAction("ui_tickets_depleted_title", "ui_buy_tickets_message", delegate
			{
				BattleButtonLogic(BoostType.NoBoost);
			}, delegate
			{
				ConfigureCameras(true);
				autoBattleButton.enabled = true;
			});
		}
	}

	private void BattleButtonLogic(BoostType boostType)
	{
		int loadingPopupId = LoadingPopupManager.ShowLoadingPopup(0f);
		if (boostType != BoostType.NoBoost)
		{
			Singleton<SessionManager>.instance.PurchaseBoost(BoostDataModel.GetBoostByType(boostType), false, "auto_bot_battle", delegate
			{
				LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
				loadingPopupId = -1;
				StartBattleLogic(boostType);
			});
		}
		else
		{
			LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
			loadingPopupId = -1;
			StartBattleLogic(boostType);
		}
	}

	private void StartBattleLogic(BoostType boostType)
	{
		Reporting.TargetBotTeam("FoughtBotTeam", boostType.ToString());
		base.OnCloseButton();
		PopupManager.BackupState(true);
		BattleSceneModel battleSceneModel = new BattleSceneModel();
		battleSceneModel.matchType = (_autoBattle ? MatchData.Type.AUTO_BOT_BATTLE : MatchData.Type.AI);
		battleSceneModel.botPartId = payloadData.partInfo.id;
		Singleton<SessionManager>.instance.UpdateTeam();
		UserProfile.player.UpdateUnitEventItems();
		if (_autoBattle)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.AutoBotBattleCalculationScene, battleSceneModel);
			return;
		}
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.AudiencePanShotScene, new SceneModel
		{
			payload = battleSceneModel
		});
	}

	private void OnControllerCooldownFinished(UserTeam team)
	{
		if (team != null)
		{
			UserTeam userTeam = UserProfile.player.teams[cooldownsController.SelectedIndex];
			if (userTeam != null && userTeam == team)
			{
				ConfigureTeamPlayer(team);
			}
		}
	}

	private ItemCollectionDataModel GetAvailableParts(List<UserUnit> units)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		ItemCollectionDataModel itemCollectionDataModel = new ItemCollectionDataModel();
		foreach (UserUnit unit in units)
		{
			for (int i = 0; i < unit.PartDrops.Length; i++)
			{
				if (!dictionary.ContainsKey(unit.PartDrops[i].ID))
				{
					dictionary.Add(unit.PartDrops[i].ID, 1);
					itemCollectionDataModel.items.Add(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, int.Parse(unit.PartDrops[i].ID), 0));
				}
			}
		}
		return itemCollectionDataModel;
	}

	public override void Close()
	{
		base.Close();
		Reporting.TargetBotTeam("Exited", "NoBoost_exited");
	}
}
