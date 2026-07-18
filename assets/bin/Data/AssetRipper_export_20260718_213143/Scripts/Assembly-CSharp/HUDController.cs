using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class HUDController : MonoBehaviour
{
	[HideInInspector]
	public BattleController battleController;

	public static Dictionary<EmoticonTypes, string> emoticonsData = new Dictionary<EmoticonTypes, string>
	{
		{
			EmoticonTypes.NONE,
			"empty"
		},
		{
			EmoticonTypes.HAPPY,
			"emoticon_happy"
		},
		{
			EmoticonTypes.SAD,
			"emoticon_sad"
		},
		{
			EmoticonTypes.ANGRY,
			"emoticon_angry"
		}
	};

	private static Color READY_BUTTON_YELLOW = new Color(0.988f, 0.827f, 0f);

	private static Color READY_BUTTON_ORANGE_FROM = new Color(0.941f, 0.478f, 0.129f);

	private static Color READY_BUTTON_ORANGE_TO = new Color(0.941f, 0.65f, 0.129f);

	private PopupDataModel forfeitPopup;

	public tk2dCamera uiCamera;

	public CubeBarController cubeBar;

	public BattleText battleText;

	public CelebrationKill celebrationKill;

	public PhaseText phaseText;

	public Announcer announcer;

	public ActionPointText actionPointText;

	public BouncingArrow bouncingArrowAction;

	public BouncingArrow bouncingArrowCommit;

	public BattleTimer timer;

	public EmoticonsHUDController emoticonController;

	public AbilitiesReporterController abilityReporter;

	public PVPMomentView pvpMomentView;

	public ScrollButton scrollButton;

	public GameObject searchingText;

	public GameObject botButton;

	public bool playerForcedBot;

	private Tweener battleButtonReadyTween;

	private void Start()
	{
		timer.HideTimer();
		abilityReporter.Hide();
		searchingText.SetActive(false);
		botButton.SetActive(false);
	}

	public void Init(BattleController battleController)
	{
		this.battleController = battleController;
		cubeBar.Init(battleController);
		emoticonController.Init();
		battleButtonReadyTween = HOTween.To(cubeBar, 1f, new TweenParms().Prop("BattleButtonTint", READY_BUTTON_ORANGE_TO).Loops(-1, LoopType.Yoyo).Ease(EaseType.EaseInQuad));
		battleButtonReadyTween.Pause();
	}

	public IEnumerator ShowAnnouncerText(AnnouncerType announcerType, string text, bool useLargeText = true, float duration = 3f)
	{
		announcer.Move(announcerType, new Vector3(0f, -250f, 100f), new Vector3(0f, 215f, 100f), new Vector3(-1f, 1f, 1f), duration);
		if (useLargeText)
		{
			phaseText.Present(text, null, duration);
		}
		else
		{
			phaseText.PresentSmall(text, null, duration);
		}
		yield return new WaitForSeconds(duration);
	}

	private void ClearForfeitPopup()
	{
		if (forfeitPopup != null)
		{
			PopupManager.DestroyPopup(forfeitPopup);
			forfeitPopup = null;
		}
	}

	private void Update()
	{
		CheckBackButton();
	}

	private void CheckBackButton()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			OnBackButton();
		}
	}

	public void OnClickReady()
	{
		ClearForfeitPopup();
		battleController.SetReadyForTurn();
	}

	public void OnClickForfeit()
	{
		forfeitPopup = PopupDataModel.NoYes("ui_popup_forfeit_title".Localize("Are you sure?"), "ui_popup_forfeit_desc".Localize("Forfeiting will destroy all of your remaining units."), battleController.Forfeit);
		PopupManager.ShowPopup(forfeitPopup);
	}

	public void OnClickCancel()
	{
		if (battleController.battleHooks.OnClickCancel())
		{
			battleController.targetSelectionManager.ExitTargettingMode();
			cubeBar.GoToMainState();
		}
	}

	public void OnClickBot()
	{
		playerForcedBot = true;
		battleController.MatchHandler.GiveUpPVPSearch();
		searchingText.SetActive(false);
		botButton.SetActive(false);
	}

	public void OnBackButton()
	{
		if (cubeBar.CurrentState == CubeBarController.State.Action)
		{
			OnClickCancel();
		}
		else if (scrollButton.IsOpen)
		{
			scrollButton.ToggleButton();
		}
	}

	public void ActivateTimer()
	{
		if ((bool)timer)
		{
			int roundTimeLimit = battleController.MatchHandler.RoundTimeLimit;
			if (roundTimeLimit > 0)
			{
				timer.ConfigureNewTimer(roundTimeLimit, OnTimerFinished, true);
			}
		}
	}

	public void HideTimer()
	{
		if ((bool)timer)
		{
			timer.StopTimer();
			timer.HideTimer();
		}
	}

	public void OnTimerFinished()
	{
		ClearForfeitPopup();
		cubeBar.FakeReadyButtonPress();
	}

	public IEnumerator ShowMessageSequence(string text, Color color, InBattleMessageType msgAnimationType = InBattleMessageType.NONE)
	{
		yield return StartCoroutine(battleText.ShowMessageSequence(text, color, msgAnimationType));
	}

	public IEnumerator ShowMessageSequence(string text, InBattleMessageType msgAnimationType = InBattleMessageType.NONE)
	{
		yield return StartCoroutine(battleText.ShowMessageSequence(text, msgAnimationType));
	}

	public void UpdateBattleButton()
	{
		if (battleController.playerTeam.energy > 0)
		{
			cubeBar.BattleButtonTint = READY_BUTTON_YELLOW;
			if (!battleButtonReadyTween.isPaused)
			{
				battleButtonReadyTween.Pause();
			}
		}
		else if (battleButtonReadyTween.isPaused)
		{
			cubeBar.BattleButtonTint = READY_BUTTON_ORANGE_FROM;
			battleButtonReadyTween.Play();
		}
	}

	public void ShowPVPMomentAnimation(OpponentData player1, OpponentData player2)
	{
		if ((bool)pvpMomentView)
		{
			pvpMomentView.gameObject.SetActive(true);
			pvpMomentView.ShowPVPMoment(player1, player2);
		}
	}

	public IEnumerator HidePVPMomentAnimation()
	{
		pvpMomentView.HidePVPMoment();
		yield break;
	}

	public void ShowSearchingUI()
	{
		if (!botButton.activeSelf)
		{
			searchingText.SetActive(true);
			botButton.SetActive(true);
		}
	}

	public void HideSearchingUI()
	{
		searchingText.SetActive(false);
		botButton.SetActive(false);
	}
}
