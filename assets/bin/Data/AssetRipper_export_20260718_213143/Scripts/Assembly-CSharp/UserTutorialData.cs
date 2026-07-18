using System;
using System.Collections.Generic;

public class UserTutorialData
{
	private const string KEY_TUTORIAL_STEP = "current_step";

	private const string KEY_FIRST_UNIT_INDEX = "first_unit_index";

	private const string KEY_LAST_TIER_ASKED_RATING = "last_tier_asked_rating";

	private const string KEY_HAS_RATED = "has_rated";

	private static Dictionary<TutorialStep, Func<TutorialHooks>> tutorialHandlers;

	private UserProfile user;

	private KeyValueStorage tutorialKVS;

	private KeyValueStorage dialogTriggerVisitsKVS;

	private KeyValueStorage askForRatingKVS;

	private static bool skippingTutorial;

	private bool _isComplete;

	private TutorialStep _currentStep;

	private int _lastTierAskedRating;

	private bool _hasRated;

	private int _firstUnitIndex;

	public bool IsComplete
	{
		get
		{
			return _isComplete;
		}
		set
		{
			_isComplete = value;
			if (skippingTutorial)
			{
				PerformSkipTutorial();
			}
		}
	}

	public TutorialStep CurrentStep
	{
		get
		{
			if (IsComplete)
			{
				return TutorialStep.Complete;
			}
			return _currentStep;
		}
		set
		{
			if (!IsComplete && _currentStep != value)
			{
				Log.InfoTag("TutorialStep is now " + value, null, "USERTUTORIALDATA");
				_currentStep = value;
				if (!IsComplete)
				{
					Singleton<SessionManager>.instance.SetTutorialStep((int)value, null);
					Reporting.TutorialFunnelEvent(_currentStep);
				}
				if (_currentStep == TutorialStep.Complete)
				{
					IsComplete = true;
				}
			}
		}
	}

	public int LastTierAskedRating
	{
		get
		{
			return _lastTierAskedRating;
		}
		set
		{
			_lastTierAskedRating = value;
			askForRatingKVS.SetValueAsync("last_tier_asked_rating", value);
		}
	}

	public bool HasRated
	{
		get
		{
			return _hasRated;
		}
		set
		{
			_hasRated = value;
			askForRatingKVS.SetValueAsync("has_rated", value);
		}
	}

	public int FirstUnitIndex
	{
		get
		{
			return _firstUnitIndex;
		}
		set
		{
			_firstUnitIndex = value;
			Log.InfoTag("First Unit Selected: " + FirstUnitID, null, "USERTUTORIALDATA");
			tutorialKVS.SetValueAsync("first_unit_index", value);
			Reporting.TutorialAction("PickTank_" + value);
		}
	}

	public int FirstUnitID
	{
		get
		{
			return TutorialConstants.FIRST_UNIT_OPTIONS[_firstUnitIndex];
		}
	}

	public bool IsInTutorial
	{
		get
		{
			return CurrentStep < TutorialStep.Complete;
		}
	}

	public UserTutorialData(UserProfile user)
	{
		this.user = user;
		tutorialKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.TUTORIAL);
		if (tutorialKVS.ContainsKey("current_step"))
		{
			tutorialKVS.Remove("current_step");
		}
		_currentStep = TutorialStep.PickTank;
		if (tutorialKVS.ContainsKey("first_unit_index"))
		{
			_firstUnitIndex = tutorialKVS.GetValue<int>("first_unit_index");
		}
		else
		{
			_firstUnitIndex = 0;
		}
		askForRatingKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.ASK_FOR_RATING);
		if (askForRatingKVS.ContainsKey("last_tier_asked_rating"))
		{
			_lastTierAskedRating = askForRatingKVS.GetValue<int>("last_tier_asked_rating");
		}
		else
		{
			_lastTierAskedRating = 0;
		}
		if (askForRatingKVS.ContainsKey("has_rated"))
		{
			_hasRated = askForRatingKVS.GetValue<bool>("has_rated");
		}
		else
		{
			_hasRated = false;
		}
		if (tutorialHandlers == null)
		{
			tutorialHandlers = new Dictionary<TutorialStep, Func<TutorialHooks>>
			{
				{
					TutorialStep.FirstBattle,
					() => new TutorialHooksFirstBattle()
				},
				{
					TutorialStep.SecondBattle,
					() => new TutorialHooksSecondBattle()
				},
				{
					TutorialStep.ThirdBattle,
					() => new TutorialHooksThirdBattle()
				},
				{
					TutorialStep.FourthBattle,
					() => new TutorialHooksFourthBattle()
				},
				{
					TutorialStep.Complete,
					() => (TutorialHooks)null
				}
			};
		}
		if (skippingTutorial)
		{
			PerformSkipTutorial();
		}
		dialogTriggerVisitsKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.DIALOG_TRIGGER_VISITS);
	}

	public int GetDialogTriggerVisitCount(string triggerKey)
	{
		return dialogTriggerVisitsKVS.GetValue<int>(triggerKey);
	}

	public void DialogTriggerVisit(string triggerKey)
	{
		int dialogTriggerVisitCount = GetDialogTriggerVisitCount(triggerKey);
		dialogTriggerVisitsKVS.SetValueAsync(triggerKey, dialogTriggerVisitCount + 1);
	}

	public TutorialHooks GetTutorialHandler()
	{
		if (tutorialHandlers.ContainsKey(CurrentStep))
		{
			return tutorialHandlers[CurrentStep]();
		}
		return null;
	}

	internal void SkipTutorial()
	{
		_isComplete = true;
		skippingTutorial = true;
	}

	public void PerformSkipTutorial()
	{
	}

	public bool ShouldSkipCurrentStep()
	{
		if (CurrentStep == TutorialStep.BuildFirstTank)
		{
			return user.unitInventory.Count >= Constants.MinUnitsPerTeam + 1 || user.GetBuildCount() > 0;
		}
		return false;
	}

	public bool ShouldSkipTutorial()
	{
		if (CurrentStep != TutorialStep.Complete)
		{
			return user.unitInventory.Count >= Constants.MinUnitsPerTeam;
		}
		return true;
	}

	public void GotoTutorialScene()
	{
		if (UserProfile.player.tutorial.ShouldSkipTutorial())
		{
			UserProfile.player.tutorial.CurrentStep = TutorialStep.Complete;
		}
		if (UserProfile.player.tutorial.CurrentStep == TutorialStep.PickTank)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.FirstUnitSelectScene);
			return;
		}
		if (UserProfile.player.tutorial.CurrentStep < TutorialStep.ChangeName)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BattleScene, new BattleSceneModel(MatchData.Type.TUTORIAL));
			return;
		}
		if (UserProfile.player.tutorial.CurrentStep == TutorialStep.BuildFirstTank || UserProfile.player.tutorial.CurrentStep == TutorialStep.ChangeName)
		{
			if (!UserProfile.player.tutorial.ShouldSkipCurrentStep())
			{
				SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene);
				return;
			}
			UserProfile.player.tutorial.CurrentStep = TutorialStep.Complete;
		}
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}
}
