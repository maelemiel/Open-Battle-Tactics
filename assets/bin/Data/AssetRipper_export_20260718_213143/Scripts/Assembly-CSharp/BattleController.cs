using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;

public class BattleController : SceneController
{
	[SerializeField]
	private UnitPositionSet[] unitPositionSets;

	[SerializeField]
	private List<UnitView> playerUnitViews;

	[SerializeField]
	private List<UnitView> enemyUnitViews;

	private List<UnitView> activePlayerUnitViews = new List<UnitView>();

	private List<UnitView> activeEnemyUnitViews = new List<UnitView>();

	private List<UnitView> allActiveUnitViews;

	private AudioManager.RepeatingSfx crowdRepeatingSound;

	public BattleState battleState;

	public TeamState playerTeam;

	public TeamState enemyTeam;

	public BattleAnimationHandler animationHandler;

	public BattleHooks battleHooks;

	public BattleField playerField;

	public BattleField enemyField;

	public HUDController hud;

	public tk2dCamera mainCamera;

	public BattleTunables tunables;

	public AnimationCurve driveOffCurve;

	public MatchManager matchManager;

	public PhaseManager phaseManager;

	public TargetSelectionManager targetSelectionManager;

	public DesaturationManager desaturationManager;

	public AbilityManager abilityManager;

	public ViewportManager viewportManager;

	public RaidBossManager raidBossManager;

	public BackgroundsController backgroundsController;

	public EventLogoController eventLogoController;

	public MiniBossManager miniBossManager;

	public IntroPhase introPhase;

	public RoundStartPhase roundStartPhase;

	public DecisionPhase decisionPhase;

	public InitiativePhase initiativePhase;

	public WaitForOpponentPhase waitForOpponentPhase;

	public ResolutionPhase resolutionPhase;

	public OutroPhase outroPhase;

	public List<UnitView> PlayerUnits
	{
		get
		{
			return activePlayerUnitViews;
		}
	}

	public List<UnitView> EnemyUnits
	{
		get
		{
			return activeEnemyUnitViews;
		}
	}

	public List<UnitView> AllUnits
	{
		get
		{
			return allActiveUnitViews;
		}
	}

	public CubeBarController CubeBar
	{
		get
		{
			return hud.cubeBar;
		}
	}

	public MatchData MatchData
	{
		get
		{
			return matchManager.MatchData;
		}
	}

	public MatchData.Type MatchType
	{
		get
		{
			return SceneModel.matchType;
		}
	}

	public BaseMatchHandler MatchHandler
	{
		get
		{
			return matchManager.MatchHandler;
		}
	}

	public BattleSceneModel SceneModel
	{
		get
		{
			return sceneModel as BattleSceneModel;
		}
	}

	public BattleHooks BattleHooks
	{
		get
		{
			return battleHooks;
		}
		set
		{
			battleHooks = value;
			battleHooks.Init(this);
		}
	}

	public List<UnitView> GetAllEnemyUnitViews()
	{
		return enemyUnitViews;
	}

	public List<UnitView> GetAllPlayerUnitViews()
	{
		return playerUnitViews;
	}

	public override void Awake()
	{
		_showTopBar = false;
		base.Awake();
	}

	private void Start()
	{
		BattleHooks = new BattleHooks();
		if (SceneModel == null)
		{
			sceneModel = new BattleSceneModel(MatchData.Type.TEST);
		}
		allActiveUnitViews = new List<UnitView>(playerUnitViews.Concat(enemyUnitViews));
		foreach (UnitView allUnit in AllUnits)
		{
			allUnit.DriveOffScreen(0f);
		}
		InitializePhaseManager();
		abilityManager = new AbilityManager(this);
		InitializeMatchManager();
		Singleton<InitializationManager>.instance.ExecuteIfStateEquals(InitializationManager.State.OnlineReady, delegate
		{
			StartCoroutine(Init());
			phaseManager.SwitchPhase(Phase.INTRO);
		});
	}

	private IEnumerator Init()
	{
		InitBattleState();
		matchManager.Init(this);
		InitPlayer();
		hud.Init(this);
		if (backgroundsController != null)
		{
			yield return StartCoroutine(backgroundsController.InitBackgrounds(this));
		}
		if (eventLogoController != null && UserProfile.player.divisionInt >= Constants.MinTierEventContent)
		{
			StartCoroutine(eventLogoController.InitBattleEventLogo(this));
		}
		SceneTransitionManager.readyToTransitionIn = true;
		MatchHandler.CreateMatch(delegate
		{
			if (MatchHandler.IsRaidBossEventActive && MatchHandler.GetOpponentTeam().raidbossStatus == "dead")
			{
				DefeatedBossHandler();
			}
			else
			{
				InitEnemy();
				if (enemyTeam.type != TeamType.RaidBoss)
				{
					AudioTrigger.BattleBackground_Music.PlayMusic();
				}
				BeginBattle();
				if (raidBossManager != null)
				{
					raidBossManager.Init(this);
				}
				if (miniBossManager != null)
				{
					miniBossManager.Init(this);
				}
			}
		});
	}

	private void InitBattleState()
	{
		battleState = new BattleState();
		animationHandler = new BattleAnimationHandler(this);
		battleState.animationHandler = animationHandler;
		StartCoroutine(animationHandler.RunSequences());
		battleState.constantsProvider = new BattleConstantsProvider();
	}

	private void DefeatedBossHandler()
	{
		PopupManager.ShowPopup(PopupDataModel.RaidBossDefeatedPopUp());
	}

	public void BeginBattle()
	{
		battleState.randomSeed = MatchData.battleSeed;
		playerTeam.randomSeed = MatchData.playerTeam.randomSeed;
		enemyTeam.randomSeed = MatchData.opponentTeam.randomSeed;
		battleState.hostTeam = ((!MatchData.playerIsHost) ? enemyTeam : playerTeam);
		battleHooks.ModifyPreBeginBattleStep();
		BattleSetupUtils.InitTeamRandom(playerTeam, playerTeam.randomSeed);
		BattleSetupUtils.InitTeamRandom(enemyTeam, enemyTeam.randomSeed);
		BattleSetupUtils.InitBattleRandom(battleState, battleState.randomSeed);
		BattleLogic.BeginBattle(battleState);
		ServerAbilityHandler[] allBattleAbilities = battleState.teamOne.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			AbilityState abilityState = serverAbilityHandler.abilityState as AbilityState;
			abilityState.animationHandler = AbilityAnimationHandlerFactory.Create(abilityState.metadata.Type, abilityState, this);
		}
		battleHooks.ModifyBeginBattleStep();
	}

	public void InitPlayer()
	{
		OpponentData opponentData = MatchHandler.GetPlayerTeam();
		battleState.teamOne = CreateTeamState(opponentData);
		battleState.teamOne.battleField = playerField;
		playerTeam = battleState.teamOne;
		playerTeam.IsPlayer = true;
		playerTeam.type = TeamType.Player;
		battleHooks.ModifyTeamStep(battleState.teamOne);
		activePlayerUnitViews = new List<UnitView>(playerUnitViews);
		AssignUnitViews(battleState.teamOne, activePlayerUnitViews);
		hud.cubeBar.UpdatePlayerAbilities(playerTeam.abilities);
	}

	public void InitEnemy()
	{
		OpponentData opponentTeam = MatchHandler.GetOpponentTeam();
		battleState.teamTwo = CreateTeamState(opponentTeam);
		battleState.teamTwo.battleField = enemyField;
		enemyTeam = battleState.teamTwo;
		enemyTeam.IsPlayer = false;
		battleHooks.ModifyTeamStep(battleState.teamTwo);
		for (int i = 0; i < enemyUnitViews.Count; i++)
		{
			enemyUnitViews[i].yOffset = UnityEngine.Random.Range(0f - tunables.enemyYOffsetRange, tunables.enemyYOffsetRange);
		}
		activeEnemyUnitViews = new List<UnitView>(enemyUnitViews);
		AssignUnitViews(battleState.teamTwo, activeEnemyUnitViews);
	}

	private void Update()
	{
		if (phaseManager != null)
		{
			phaseManager.Update();
		}
	}

	protected void InitializeMatchManager()
	{
		matchManager = new MatchManager();
		matchManager.Register(MatchData.Type.TUTORIAL, () => new TutorialMatchHandler());
		matchManager.Register(MatchData.Type.AI, () => new OnlineMatchHandler());
		matchManager.Register(MatchData.Type.PVP, () => new OnlineMatchHandler());
		matchManager.Register(MatchData.Type.RAIDBOSS, () => new OnlineMatchHandler());
		matchManager.Register(MatchData.Type.AUTO_BOT_BATTLE, () => new AutoBotBattleMatchHandler());
	}

	private void InitializePhaseManager()
	{
		phaseManager = new PhaseManager(this);
		phaseManager.Register(Phase.INTRO, introPhase = new IntroPhase());
		phaseManager.Register(Phase.ROUNDSTART, roundStartPhase = new RoundStartPhase());
		phaseManager.Register(Phase.DECISION, decisionPhase = new DecisionPhase());
		phaseManager.Register(Phase.INITIATIVE, initiativePhase = new InitiativePhase());
		phaseManager.Register(Phase.WAIT_FOR_OPPONENT, waitForOpponentPhase = new WaitForOpponentPhase());
		phaseManager.Register(Phase.RESOLUTION, resolutionPhase = new ResolutionPhase());
		phaseManager.Register(Phase.OUTRO, outroPhase = new OutroPhase());
	}

	public List<UnitView> GetUnitsByTeam(TeamState team)
	{
		return (team != playerTeam) ? EnemyUnits.ToList() : PlayerUnits.ToList();
	}

	public void RemoveUnit(UnitView unitToRemove)
	{
		List<UnitView> list = ((unitToRemove.Team != playerTeam) ? activeEnemyUnitViews : activePlayerUnitViews);
		list.Remove(unitToRemove);
		allActiveUnitViews.Remove(unitToRemove);
	}

	public override void OnBeginTransitionOut()
	{
		base.OnBeginTransitionOut();
		Time.timeScale = 1f;
		if (crowdRepeatingSound != null)
		{
			crowdRepeatingSound.Stop();
		}
	}

	public void OnSelectUnit(UnitView unit)
	{
		if (targetSelectionManager.isActive)
		{
			if (battleHooks.OnTargetUnit(unit))
			{
				targetSelectionManager.SelectUnit(unit);
			}
		}
		else if (phaseManager.currentPhase == Phase.DECISION)
		{
			if (battleHooks.OnTapUnit(unit))
			{
				unit.OnUnitTapped();
			}
		}
		else if (!battleHooks.OnInvalidTapUnit(unit))
		{
		}
	}

	public void SetReadyForTurn()
	{
		if (phaseManager.currentPhase == Phase.DECISION && battleHooks.OnClickBattle())
		{
			phaseManager.SwitchPhase(Phase.WAIT_FOR_OPPONENT);
		}
	}

	public void Forfeit()
	{
		ForfeitAction forfeitAction = new ForfeitAction();
		matchManager.playerActions.Clear();
		matchManager.playerActions.Add(forfeitAction);
		BattleLogic.ApplyBattleAction(playerTeam, forfeitAction);
		SetReadyForTurn();
	}

	public void ReAssignUnitViews(List<UnitView> teamUnitViews, TeamState teamState, List<UnitView> activeTeamUnitViews)
	{
		activeTeamUnitViews.Clear();
		activeTeamUnitViews.AddRange(teamUnitViews);
		AssignUnitViews(teamState, activeTeamUnitViews);
		ServerAbilityHandler[] allBattleAbilities = battleState.teamOne.allBattleAbilities;
		foreach (ServerAbilityHandler serverAbilityHandler in allBattleAbilities)
		{
			AbilityState abilityState = serverAbilityHandler.abilityState as AbilityState;
			if (abilityState != null && abilityState.team == teamState && abilityState.animationHandler != null)
			{
				StartCoroutine(abilityState.animationHandler.DestroyAnimation());
				abilityState.animationHandler = AbilityAnimationHandlerFactory.Create(abilityState.metadata.Type, abilityState, this);
			}
		}
	}

	private void AssignUnitViews(TeamState team, List<UnitView> unitViews)
	{
		if (team.units.Length > unitViews.Count)
		{
			throw new Exception("Too many units! Not enough UnitViews!");
		}
		for (int i = 0; i < unitViews.Count; i++)
		{
			if (i < team.units.Length)
			{
				unitViews[i].gameObject.SetActive(true);
				unitViews[i].Init(this, team.units[i]);
				continue;
			}
			unitViews[i].gameObject.SetActive(false);
			allActiveUnitViews.Remove(unitViews[i]);
			unitViews.RemoveAt(i);
			i--;
		}
		RepositionTeam(team);
	}

	public void RepositionTeam(TeamState team)
	{
		List<UnitView> unitsByTeam = GetUnitsByTeam(team);
		float num = ((!team.IsPlayer) ? tunables.enemyPositionOffset : 0f);
		bool isEnemy = team.IsEnemy;
		int count = unitsByTeam.Count;
		List<Transform> transformList = unitPositionSets[count - 1].transformList;
		object.Equals(count, transformList.Count);
		float num2 = mainCamera.TargetResolution.x / mainCamera.TargetResolution.y / 2f;
		float num3 = ((!isEnemy) ? num2 : (0f - num2));
		for (int i = 0; i < count; i++)
		{
			UnitView unitView = unitsByTeam[i];
			Transform transform = transformList[i];
			unitView.SetOriginalPosition(new Vector2(transform.localPosition.x * num3 + num, transform.localPosition.y + unitView.yOffset));
		}
	}

	public void OffsetWinnerTeamOutOfScreen(float time)
	{
		List<UnitView> unitsByTeam = GetUnitsByTeam(battleState.winningTeam);
		int count = unitsByTeam.Count;
		for (int i = 0; i < count; i++)
		{
			StartCoroutine(MoveUnitOffScreen(unitsByTeam[i].transform, time));
		}
	}

	private IEnumerator MoveUnitOffScreen(Transform unit, float time)
	{
		float timer = 0f;
		float customTime = time + UnityEngine.Random.value * 0.1f;
		float xOffset = mainCamera.TargetResolution.x * (battleState.winningTeam.IsPlayer ? 1f : (-1f));
		Vector3 startPosition = unit.position;
		Vector3 endPosition = unit.position + new Vector3(xOffset, 0f, 0f);
		while (timer < customTime)
		{
			unit.position = startPosition.UnclampedLerp(endPosition, driveOffCurve.Evaluate(timer / customTime));
			yield return new WaitForEndOfFrame();
			timer += Time.deltaTime;
			xOffset = mainCamera.TargetResolution.x * (battleState.winningTeam.IsPlayer ? 1f : (-1f));
			endPosition = unit.position + new Vector3(xOffset, 0f, 0f);
		}
		unit.position = endPosition;
	}

	protected TeamState CreateTeamState(OpponentData opponentData)
	{
		return BattleSetupUtils.CreateTeamState<TeamState, UnitState, AbilityState>(opponentData);
	}

	public IEnumerator PlayViewportWinAnimation()
	{
		SimpleTween.Start(toVal: (!playerTeam.IsBattleWinner) ? (-0.1f) : 1.1f, fromVal: viewportManager.ViewportCenter, duration: 0.9f, ease: EaseType.EaseInBack, onUpdate: delegate(float val)
		{
			viewportManager.ViewportCenter = val;
		});
		OffsetWinnerTeamOutOfScreen(1f);
		yield return new WaitForSeconds(1.3f);
	}
}
