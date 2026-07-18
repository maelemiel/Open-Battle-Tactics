using System.Collections;
using UnityEngine;

public class AutoBotBattleCalculationSceneController : BattleController
{
	[SerializeField]
	private float panDuration = 4f;

	public override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		if (base.SceneModel == null)
		{
			sceneModel = new BattleSceneModel(MatchData.Type.TEST);
		}
		InitializeMatchManager();
		TopBarController.instance.Visible = false;
		AudioTrigger.StandardCrowd.PlayMusic();
		Singleton<InitializationManager>.instance.ExecuteIfStateEquals(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		battleState = new BattleState();
		battleState.constantsProvider = new BattleConstantsProvider();
		matchManager.Init(this);
		OpponentData opponentData = base.MatchHandler.GetPlayerTeam();
		battleState.teamOne = CreateTeamState(opponentData);
		playerTeam = battleState.teamOne;
		playerTeam.IsPlayer = true;
		playerTeam.type = TeamType.Player;
		SceneTransitionManager.readyToTransitionIn = true;
		base.MatchHandler.CreateMatch(delegate
		{
			OpponentData opponentTeam = base.MatchHandler.GetOpponentTeam();
			battleState.teamTwo = CreateTeamState(opponentTeam);
			enemyTeam = battleState.teamTwo;
			enemyTeam.IsPlayer = false;
			battleState.randomSeed = base.MatchData.battleSeed;
			playerTeam.randomSeed = base.MatchData.playerTeam.randomSeed;
			enemyTeam.randomSeed = base.MatchData.opponentTeam.randomSeed;
			battleState.hostTeam = ((!base.MatchData.playerIsHost) ? enemyTeam : playerTeam);
			BattleSetupUtils.InitTeamRandom(playerTeam, playerTeam.randomSeed);
			BattleSetupUtils.InitTeamRandom(enemyTeam, enemyTeam.randomSeed);
			BattleSetupUtils.InitBattleRandom(battleState, battleState.randomSeed);
			BattleLogic.BeginBattle(battleState);
			AutoBattleHandler();
		});
	}

	private void AutoBattleHandler()
	{
		StartCoroutine(AutoBattleHandlerCoroutine());
	}

	private IEnumerator AutoBattleHandlerCoroutine()
	{
		yield return StartCoroutine(AutoBattleMatchComplete());
		yield return new WaitForSeconds(panDuration);
		base.MatchHandler.GotoPostBattleScene();
	}

	private IEnumerator AutoBattleMatchComplete()
	{
		bool complete = false;
		base.MatchHandler.MatchComplete(playerTeam.stats, delegate
		{
			complete = true;
		});
		while (!complete)
		{
			yield return 0;
		}
	}
}
