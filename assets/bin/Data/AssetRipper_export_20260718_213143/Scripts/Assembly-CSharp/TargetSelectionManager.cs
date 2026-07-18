using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionManager : MonoBehaviour
{
	public delegate void TargetSelectHandler(UnitView unit);

	private BattleController battleController;

	private List<UnitView> validTargetsList;

	private TargetSelectHandler selectHandler;

	[HideInInspector]
	public bool isActive;

	private List<EffectInstance> createdEffects = new List<EffectInstance>();

	private AbilityState ability;

	private bool inTargettingMode;

	public AbilityState CurrentAbility
	{
		get
		{
			return ability;
		}
	}

	private void Start()
	{
		battleController = GetComponent<BattleController>();
		isActive = false;
	}

	public void EnterTargetingMode(AbilityState abilityState, string instructionText, TargetSelectHandler selectHandler)
	{
		if (!inTargettingMode)
		{
			ability = abilityState;
			this.selectHandler = selectHandler;
			battleController.CubeBar.GoToActionState(instructionText, delegate
			{
				ExitTargettingMode();
			});
			ShowValidTargets(abilityState);
			isActive = true;
			inTargettingMode = true;
		}
	}

	public void ExitTargettingMode()
	{
		if (!inTargettingMode)
		{
			return;
		}
		battleController.desaturationManager.ResetSaturation();
		battleController.CubeBar.GoToMainState();
		isActive = false;
		foreach (UnitView validTargets in validTargetsList)
		{
			validTargets.PossibleRollsSimple.CloseDieBox();
		}
		if (battleController.MatchType == MatchData.Type.TUTORIAL)
		{
			foreach (EffectInstance createdEffect in createdEffects)
			{
				createdEffect.Destroy();
			}
		}
		inTargettingMode = false;
	}

	private void ShowValidTargets(AbilityState abilityState)
	{
		validTargetsList = new List<UnitView>();
		List<tk2dBaseSprite> list = new List<tk2dBaseSprite>();
		List<UnitView> list2 = new List<UnitView>();
		list2.AddRange(battleController.EnemyUnits);
		list2.AddRange(battleController.PlayerUnits);
		foreach (UnitView item2 in list2)
		{
			if (!BattleLogic.IsUnitTargetValid(battleController.playerTeam, item2.state, abilityState))
			{
				foreach (tk2dSprite sprite in item2.GetSprites())
				{
					list.Add(sprite);
				}
			}
			else
			{
				validTargetsList.Add(item2);
			}
		}
		foreach (UnitView validTargets in validTargetsList)
		{
			validTargets.PossibleRollsSimple.OpenDieHUD();
		}
		TargetType targetSelectType = abilityState.metadata.TargetSelectType;
		switch (targetSelectType)
		{
		case TargetType.EnemyUnit:
			list.AddRange(battleController.playerField.GetSprites());
			break;
		case TargetType.PlayerUnit:
			list.AddRange(battleController.enemyField.GetSprites());
			break;
		}
		if (battleController.MatchType == MatchData.Type.TUTORIAL)
		{
			EffectInstance effectInstance = null;
			foreach (UnitView validTargets2 in validTargetsList)
			{
				if (targetSelectType != TargetType.PlayerUnit || validTargets2.CurrentRoll != validTargets2.DiceValues.Length - 1)
				{
					effectInstance = GlobalEffectsManager.Create((targetSelectType != TargetType.EnemyUnit) ? EffectType.GREEN_ARROW : EffectType.RED_ARROW, validTargets2.transform.position, validTargets2.gameObject);
					createdEffects.Add(effectInstance);
				}
			}
		}
		battleController.desaturationManager.Desaturate(list);
	}

	public bool IsTargetValid(UnitView unit)
	{
		return validTargetsList.Contains(unit);
	}

	public void SelectUnit(UnitView unit)
	{
		if (IsTargetValid(unit))
		{
			ExitTargettingMode();
			selectHandler(unit);
		}
	}

	public void Cancel()
	{
		ExitTargettingMode();
		selectHandler(null);
	}
}
