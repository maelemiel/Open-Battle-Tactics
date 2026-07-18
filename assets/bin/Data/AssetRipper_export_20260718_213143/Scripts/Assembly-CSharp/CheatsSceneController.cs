using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatsSceneController : SceneController
{
	public override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		base.SectionTitle = "Cheats";
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
	}

	private void AddRandomUnits(int numUnits, int level)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		List<UnitDataModel> all = NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitDataModel>();
		all = all.FindAll(delegate(UnitDataModel unit)
		{
			UnitType type = (UnitType)unit.type;
			return type != UnitType.BOSS && type != UnitType.RAID_BOSS;
		});
		all.ShuffleList();
		numUnits = Math.Min(all.Count, numUnits);
		for (int num = 0; num < numUnits; num++)
		{
			UnitDataModel unitDataModel = all[num];
			dictionary[unitDataModel.id.ToString()] = Mathf.Min(level, unitDataModel.MaxLevel);
		}
		Singleton<SessionManager>.instance.CheatCreateUnits(dictionary, delegate(List<UserUnit> resultUnits)
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Added " + resultUnits.Count + " units."));
		});
	}

	public void OnClickAddAllUnits()
	{
		List<UnitDataModel> all = NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitDataModel>();
		AddRandomUnits(all.Count, 1);
	}

	public void OnClickAddAllUnitsMaxLevel()
	{
		List<UnitDataModel> all = NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitDataModel>();
		AddRandomUnits(all.Count, 100);
	}

	public void OnClickRemoveAllUnits()
	{
		Singleton<SessionManager>.instance.CheatRemoveAllUnits(delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Remove all units."));
		});
	}

	public void OnClickAdd5Units()
	{
		AddRandomUnits(5, 1);
	}

	public void OnClickAddCurrency()
	{
		Singleton<SessionManager>.instance.CheatGiveCurrency(UserInventory.ItemType.Coins, 20000, delegate
		{
		});
	}

	public void OnClickAddScrap()
	{
		Singleton<SessionManager>.instance.CheatGiveCurrency(UserInventory.ItemType.PremiumCurrency, 1000, delegate
		{
		});
	}

	public void OnClickResetFreeGacha()
	{
		Singleton<SessionManager>.instance.CheatResetFreeGacha(delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Free gacha is available"));
		});
	}

	public void OnClickMakeUserAdmin()
	{
		Singleton<SessionManager>.instance.CheatMakeUserAdmin(delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "You are an admin now"));
		});
	}

	public void OnClickResetPVP()
	{
		Singleton<SessionManager>.instance.CheatResetUserPVP(delegate
		{
			UserProfile.player.ResetDivision();
			UserProfile.player.pvpRating = 1400;
			UserProfile.player.winStreak = 0;
			UserProfile.player.ResetToPVPDivision();
			TopBarController.instance.RefreshProgress();
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "You're PVP stats have been reset."));
		});
	}

	private void DivisionIncreaseCheat()
	{
		int divisionId = int.Parse(UserProfile.player.divisionId);
		ProgressionDivisionDataModel single = ProgressionDivisionDataModel.GetSingle(divisionId + 1);
		if (single != null)
		{
			Singleton<SessionManager>.instance.CheatSetDivision(divisionId + 1, delegate
			{
				TopBarController.instance.RefreshProgress();
				PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Set division to: " + (divisionId + 1) + "."));
			});
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat fail", "Next division: [" + (divisionId + 1) + "] doesn't exist yet"));
		}
	}

	private void DivisionDecreaseCheat()
	{
		int divisionId = int.Parse(UserProfile.player.divisionId);
		if (divisionId > 1)
		{
			Singleton<SessionManager>.instance.CheatSetDivision(divisionId - 1, delegate
			{
				TopBarController.instance.RefreshProgress();
				PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Set division to: " + (divisionId - 1) + "."));
			});
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat fail", "Division is already 1."));
		}
	}

	private void AbilitiesCheat()
	{
		List<string> list = new List<string>();
		foreach (AbilityDataModel item in AbilityDataModel.GetAll())
		{
			list.Add(item.id);
		}
		Singleton<SessionManager>.instance.CheatUnlockAbilities(list, delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Added abilities."));
		});
	}

	private void PartsCheat()
	{
		PartsCheatLogic(100);
	}

	private void PartsCheat1000()
	{
		PartsCheatLogic(1000);
	}

	private void PartsCheatLogic(int numParts)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		foreach (UnitPartTypesDataModel item in UnitPartTypesDataModel.GetAll())
		{
			dictionary[int.Parse(item.id)] = numParts;
		}
		dictionary[2] = numParts;
		dictionary[3] = numParts;
		dictionary[4] = numParts;
		Singleton<SessionManager>.instance.CheatGiveParts(dictionary, delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", string.Format("Added {0} parts of each type.", numParts)));
		});
	}

	private void ClearPartsCheat()
	{
		Singleton<SessionManager>.instance.CheatClearParts(delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Cleared all parts and tokens."));
		});
	}

	private void ClearTokensCheat()
	{
		List<int> list = new List<int>();
		foreach (UnitPartTypesDataModel item in UnitPartTypesDataModel.GetAll())
		{
			if (item.IsToken)
			{
				list.Add(int.Parse(item.id));
			}
		}
		Singleton<SessionManager>.instance.CheatClearTokens(list, delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Cleared all tokens"));
		});
	}

	private void TicketsCheat()
	{
		if (UserProfile.player.energy <= 0)
		{
			Singleton<SessionManager>.instance.CheatRestoreEnergy(delegate
			{
			});
		}
	}

	private void ClientError()
	{
		throw new Exception("TestException");
	}

	private void ServerError()
	{
		Singleton<SessionManager>.instance.CheatTestError();
	}

	private void RuinGame()
	{
		Application.Quit();
	}

	public void ExitButtonPressed()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.SettingsScene);
	}

	public void EmptyWalletCheat()
	{
		Singleton<SessionManager>.instance.CheatEmptyWallet(delegate
		{
			Singleton<BankService>.instance.GetCurrencyBalance(delegate(int balance)
			{
				UserProfile.player.gems = balance;
			});
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Emptied your wallet!"));
		});
	}

	public void OnClickResetCrateDate()
	{
		Singleton<SessionManager>.instance.CheatResetCrateDate(delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Club crate drop date reset"));
		});
	}

	public void OnClickGetCrates()
	{
		Singleton<SessionManager>.instance.CheatGetCrates(delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Club crates received"));
		});
	}

	public void OnClickUserServerLocalSQL()
	{
		Singleton<SessionManager>.instance.useLocalSQLite(delegate
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "Server Using Local SQL", QuitUtility.Restart));
		});
	}

	public void OnClickedEventPoints()
	{
		if (UserProfile.player.GetActiveEvent() != null)
		{
			Singleton<SessionManager>.instance.CheatGive100EventPoints(delegate
			{
				PopupManager.ShowPopup(PopupDataModel.Ok("Cheat success", "+100 Event Points"));
			});
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("Cheat failure", "No event running, come on!"));
		}
	}
}
