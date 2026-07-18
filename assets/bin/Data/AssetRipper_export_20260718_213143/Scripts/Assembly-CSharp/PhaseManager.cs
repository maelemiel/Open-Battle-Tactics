using System.Collections.Generic;
using UnityEngine;

public class PhaseManager
{
	private BattleController battleController;

	private Dictionary<Phase, AbstractBattlePhase> registeredPhases;

	private AbstractBattlePhase currentPhaseHandler;

	public Phase currentPhase
	{
		get
		{
			return (currentPhaseHandler != null) ? currentPhaseHandler.phaseType : Phase.NONE;
		}
	}

	public PhaseManager(BattleController battleController)
	{
		this.battleController = battleController;
		registeredPhases = new Dictionary<Phase, AbstractBattlePhase>();
	}

	public void Register(Phase phaseType, AbstractBattlePhase phaseInstance)
	{
		registeredPhases.Add(phaseType, phaseInstance);
		phaseInstance.battleController = battleController;
		phaseInstance.phaseType = phaseType;
	}

	public void Update()
	{
		if (currentPhaseHandler != null)
		{
			currentPhaseHandler.timeInPhase += Time.deltaTime;
			currentPhaseHandler.OnUpdate();
		}
	}

	public void SwitchPhase(Phase newPhaseType)
	{
		if (currentPhaseHandler != null)
		{
			currentPhaseHandler.OnExitPhase();
		}
		currentPhaseHandler = registeredPhases[newPhaseType];
		if (currentPhaseHandler != null)
		{
			currentPhaseHandler.timeInPhase = 0f;
			currentPhaseHandler.OnEnterPhase();
		}
	}
}
