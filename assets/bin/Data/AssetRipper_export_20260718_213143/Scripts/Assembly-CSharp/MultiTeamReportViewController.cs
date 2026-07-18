using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class MultiTeamReportViewController : MonoBehaviour
{
	public enum ReportViewState
	{
		NONE = 0,
		IN_PROGRESS = 1,
		COMPLETED = 2,
		EMPTY_TEAM = 3
	}

	private class TeamResults
	{
		public int totalEventPoints;

		public int totalBonusPoints;

		public string gradeSpriteName;

		public TeamResults(int totalEventPoints, int totalBonusPoints, string gradeSpriteName)
		{
			this.totalEventPoints = totalEventPoints;
			this.totalBonusPoints = totalBonusPoints;
			this.gradeSpriteName = gradeSpriteName;
		}
	}

	[SerializeField]
	private UnitMultiTeamReportView[] reportUnitViews;

	private PopupController controller;

	private UserMultiTeamReport.MultiTeamReport multiTeamReport;

	[SerializeField]
	private ObjectShaker shaker;

	[SerializeField]
	private tk2dTextMesh teamLabel;

	[SerializeField]
	private tk2dTextMesh battlesLabel;

	[SerializeField]
	private tk2dTextMesh battlesLabelValue;

	[SerializeField]
	private tk2dBaseSprite battlesSprite;

	[SerializeField]
	private tk2dTextMesh eventPointsLabel;

	[SerializeField]
	private tk2dTextMesh eventPointsValue;

	[SerializeField]
	private tk2dBaseSprite eventPointsSprite;

	[SerializeField]
	private tk2dTextMesh eventPointsLabelSmall;

	[SerializeField]
	private tk2dTextMesh eventPointsValueSmall;

	[SerializeField]
	private tk2dBaseSprite eventPointsSpriteSmall;

	[SerializeField]
	private tk2dTextMesh bonusPointsLabel;

	[SerializeField]
	private tk2dTextMesh bonusEventPointsValue;

	[SerializeField]
	private tk2dBaseSprite bonusEventPointsSprite;

	[SerializeField]
	private tk2dTextMesh bonusPointsLabelSmall;

	[SerializeField]
	private tk2dTextMesh bonusPointsValueSmall;

	[SerializeField]
	private tk2dBaseSprite bonusPointsSpriteSmall;

	[SerializeField]
	private tk2dBaseSprite headerBackgroundSprite;

	[SerializeField]
	private GameObject totalResultGameObject;

	[SerializeField]
	private tk2dTextMesh totalPointsEarnedLabel;

	[SerializeField]
	private tk2dTextMesh totalPointsEarnedValue;

	[SerializeField]
	private GameObject totalGradeGameObject;

	[SerializeField]
	private tk2dBaseSprite totalGradeSprite;

	[SerializeField]
	private GameObject tapToContinue;

	[SerializeField]
	private GameObject incompleteTeam;

	public int finalTeamLabelXPosition = 10;

	public int initialTeamLabelXPosition = -400;

	public int totalResultInitialYPosition = 200;

	public int totalResultFinalYPosition = 350;

	private Tweener teamLabelTween;

	private Tweener battlesLabelTween;

	private Tweener eventPointsLabelTween;

	private Tweener eventPointsSpriteTween;

	private Tweener eventPointsSmallLabelTween;

	private Tweener bonusEventPointsLabelTween;

	private Tweener bonusEventPointsSmallLabelTween;

	private Tweener bonusEventPointsSpriteTween;

	private Tweener headerBackgroundTween;

	private Tweener totalResultTween;

	private Tweener totalGradeTween;

	private Tweener incompleteTeamTween;

	private int currentTeamIndex;

	private TeamResults currentTeamResults;

	[Range(1f, 10f)]
	public int eventPointsUpdateSpeed = 3;

	[Range(1f, 10f)]
	public int bonusPointsUpdateSpeed = 3;

	[Range(1f, 200f)]
	public int totalEventPointsSpeed = 2;

	public Vector3 initialEventPointsPosition;

	public Vector3 initialBonusPointsPosition;

	private float currentGoalPoints;

	private float totalPointsEarned;

	private float previousTotalPoints;

	private ReportViewState currentState;

	private AudioManager.RepeatingSfx loopSound;

	public List<UserMultiTeamReportUnit> CurrentTeam
	{
		get
		{
			if (currentTeamIndex < multiTeamReport.Teams.Count)
			{
				return multiTeamReport.Teams[currentTeamIndex];
			}
			return null;
		}
	}

	public void Init(PopupController popUpController, UserMultiTeamReport.MultiTeamReport report)
	{
		multiTeamReport = report;
		controller = popUpController;
		initialEventPointsPosition.z = base.transform.position.z;
		initialBonusPointsPosition.z = base.transform.position.z;
		ClearSequence();
	}

	private void ProcessTeam()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < CurrentTeam.Count; i++)
		{
			num += CurrentTeam[i].eventPointsEarned;
			num2 += CurrentTeam[i].bonusEventPointsEarned;
		}
		if (num < 0)
		{
			num = 0;
		}
		if (num2 < 0)
		{
			num2 = 0;
		}
		string empty = string.Empty;
		EventMultiTeamResultThreshoDataModel eventMultiTeamResultThreshoDataModel = null;
		List<EventMultiTeamResultThreshoDataModel> list = new List<EventMultiTeamResultThreshoDataModel>(EventMultiTeamResultThreshoDataModel.GetAll());
		if (list != null && list.Count > 0)
		{
			list.Sort((EventMultiTeamResultThreshoDataModel x, EventMultiTeamResultThreshoDataModel y) => x.thresholdValue.CompareTo(y.thresholdValue));
			for (int num3 = 1; num3 < list.Count; num3++)
			{
				if (num >= list[num3 - 1].thresholdValue && num < list[num3].thresholdValue)
				{
					eventMultiTeamResultThreshoDataModel = list[num3 - 1];
					break;
				}
			}
		}
		if (eventMultiTeamResultThreshoDataModel == null)
		{
			eventMultiTeamResultThreshoDataModel = list[list.Count - 1];
		}
		empty = eventMultiTeamResultThreshoDataModel.spriteName;
		currentTeamResults = new TeamResults(num, num2, empty);
	}

	public IEnumerator StartSequence()
	{
		ClearSequence();
		yield return StartCoroutine(PlaySequence());
	}

	private IEnumerator PlaySequence()
	{
		if (CurrentTeam == null)
		{
			yield break;
		}
		ShowTeamIndex();
		if (CurrentTeam.Count < Constants.MinUnitsPerTeam)
		{
			currentState = ReportViewState.EMPTY_TEAM;
			if (incompleteTeamTween != null)
			{
				incompleteTeamTween.Kill();
			}
			incompleteTeam.gameObject.SetActive(true);
			incompleteTeamTween = incompleteTeam.transform.TweenLocalScale(1f, 0.5f, EaseType.Linear);
			yield return new WaitForSeconds(0.5f);
			SetCurrentTeamSequenceAsCompleted();
		}
		else
		{
			yield return new WaitForSeconds(0.1f);
			currentState = ReportViewState.IN_PROGRESS;
			ProcessTeam();
			yield return StartCoroutine(ShowUnits());
			yield return StartCoroutine(ShowBattles());
			StartCoroutine(UpdateTotalPoints());
			yield return StartCoroutine(ShowEventPoints());
			yield return StartCoroutine(ShowBonusPoints());
			yield return StartCoroutine(ShowGrade());
			SetCurrentTeamSequenceAsCompleted();
		}
	}

	private void ClearSequence()
	{
		currentState = ReportViewState.NONE;
		incompleteTeam.SetActive(false);
		incompleteTeam.transform.localScale = Vector3.zero;
		teamLabel.transform.SetLocalXPosition(initialTeamLabelXPosition);
		battlesLabel.Alpha = 0f;
		battlesLabelValue.Alpha = 0f;
		battlesSprite.Alpha = 0f;
		eventPointsLabel.gameObject.SetActive(true);
		eventPointsLabel.transform.localScale = Vector3.one;
		eventPointsLabel.Alpha = 0f;
		eventPointsValue.Alpha = 0f;
		eventPointsSprite.Alpha = 0f;
		eventPointsSprite.scale = Vector3.one;
		eventPointsLabel.transform.position = initialEventPointsPosition;
		eventPointsLabelSmall.gameObject.SetActive(false);
		eventPointsLabelSmall.Alpha = 0f;
		eventPointsValueSmall.Alpha = 0f;
		eventPointsSpriteSmall.Alpha = 0f;
		bonusPointsLabel.gameObject.SetActive(false);
		bonusPointsLabel.transform.localScale = Vector3.one;
		bonusPointsLabel.Alpha = 0f;
		bonusEventPointsValue.Alpha = 0f;
		bonusEventPointsSprite.Alpha = 0f;
		bonusEventPointsSprite.scale = Vector3.one;
		bonusPointsLabel.transform.position = initialBonusPointsPosition;
		bonusPointsLabelSmall.gameObject.SetActive(false);
		bonusPointsLabelSmall.Alpha = 0f;
		bonusPointsValueSmall.Alpha = 0f;
		bonusPointsSpriteSmall.Alpha = 0f;
		headerBackgroundSprite.Alpha = 0f;
		totalGradeGameObject.gameObject.SetActive(false);
		totalGradeSprite.Alpha = 0f;
		totalGradeSprite.scale = Vector3.one * 3f;
		previousTotalPoints = totalPointsEarned;
		for (int i = 0; i < reportUnitViews.Length; i++)
		{
			StartCoroutine(reportUnitViews[i].HideUnit());
		}
		tapToContinue.SetActive(false);
		if (loopSound != null)
		{
			loopSound.Stop();
			loopSound = null;
		}
	}

	private void SkipSequence(bool isEmptyTeam = false)
	{
		StopAllCoroutines();
		if (teamLabelTween != null)
		{
			teamLabelTween.Kill();
		}
		teamLabel.transform.SetLocalXPosition(finalTeamLabelXPosition);
		if (headerBackgroundTween != null)
		{
			headerBackgroundTween.Kill();
		}
		headerBackgroundSprite.Alpha = 0.5f;
		if (isEmptyTeam)
		{
			if (incompleteTeamTween != null)
			{
				incompleteTeamTween.Kill();
			}
			incompleteTeam.transform.localScale = Vector3.one;
			return;
		}
		for (int i = 0; i < CurrentTeam.Count; i++)
		{
			reportUnitViews[i].ShowUnitImmediate(CurrentTeam[i]);
		}
		if (battlesLabelTween != null)
		{
			battlesLabelTween.Kill();
		}
		battlesLabel.Alpha = 1f;
		battlesLabelValue.Alpha = 1f;
		battlesSprite.Alpha = 1f;
		battlesLabelValue.text = Constants.MultiTeamReportBattleCount.ToString();
		if (eventPointsLabelTween != null)
		{
			eventPointsLabelTween.Kill();
		}
		if (eventPointsLabelTween != null)
		{
			eventPointsLabelTween.Kill();
		}
		if (eventPointsSmallLabelTween != null)
		{
			eventPointsSmallLabelTween.Kill();
		}
		if (eventPointsSpriteTween != null)
		{
			eventPointsSpriteTween.Kill();
		}
		eventPointsLabel.gameObject.SetActive(false);
		eventPointsLabelSmall.gameObject.SetActive(true);
		eventPointsLabelSmall.Alpha = 1f;
		eventPointsValueSmall.Alpha = 1f;
		eventPointsSpriteSmall.Alpha = 1f;
		eventPointsLabelSmall.text = "ui_multiteam_eventpoints".Localize("Event Points:");
		eventPointsValueSmall.text = "+" + currentTeamResults.totalEventPoints;
		if (bonusEventPointsLabelTween != null)
		{
			bonusEventPointsLabelTween.Kill();
		}
		if (bonusEventPointsSmallLabelTween != null)
		{
			bonusEventPointsSmallLabelTween.Kill();
		}
		if (bonusEventPointsSpriteTween != null)
		{
			bonusEventPointsSpriteTween.Kill();
		}
		bonusPointsLabel.gameObject.SetActive(false);
		bonusPointsLabelSmall.gameObject.SetActive(true);
		bonusPointsLabelSmall.Alpha = 1f;
		bonusPointsValueSmall.Alpha = 1f;
		bonusPointsSpriteSmall.Alpha = 1f;
		bonusPointsLabelSmall.text = "ui_multiteam_bonuspoints".Localize("Bonus Points:");
		bonusPointsValueSmall.text = "+" + currentTeamResults.totalBonusPoints;
		totalGradeGameObject.gameObject.SetActive(true);
		totalGradeSprite.SetSprite(currentTeamResults.gradeSpriteName);
		totalGradeSprite.Alpha = 1f;
		totalGradeSprite.scale = Vector3.one;
		if (totalGradeTween != null)
		{
			totalGradeTween.Kill();
		}
		AudioTrigger.CrateLand.Play();
		if (totalResultTween != null)
		{
			totalResultTween.Kill();
		}
		totalResultGameObject.transform.SetLocalYPosition(totalResultFinalYPosition);
		totalPointsEarned = previousTotalPoints + (float)currentTeamResults.totalEventPoints + (float)currentTeamResults.totalBonusPoints;
		currentGoalPoints = totalPointsEarned;
		totalPointsEarnedValue.text = "+" + totalPointsEarned;
		if (loopSound != null)
		{
			loopSound.Stop();
			loopSound = null;
		}
	}

	private void SetCurrentTeamSequenceAsCompleted()
	{
		if (currentState == ReportViewState.COMPLETED)
		{
			Log.Error("Somebody is trying to set a sequence as complete, while it was already completed", base.gameObject);
		}
		tapToContinue.SetActive(true);
		currentState = ReportViewState.COMPLETED;
		currentTeamIndex++;
	}

	private void ShowTeamIndex()
	{
		if (teamLabelTween != null)
		{
			teamLabelTween.Kill();
		}
		teamLabel.text = string.Format("ui_multiteam_popup_title".Localize("TEAM {0} REPORT CARDS"), currentTeamIndex + 1);
		teamLabelTween = teamLabel.transform.TweenLocalXPosition(finalTeamLabelXPosition, 0.5f);
	}

	private IEnumerator ShowUnits()
	{
		int reportUnitsCount = CurrentTeam.Count;
		ShowTeamIndex();
		yield return new WaitForSeconds(reportUnitViews[0].AnimationTime);
		if (reportUnitsCount != reportUnitViews.Length)
		{
			Log.Error("UserMultiTeamReportUnits: " + reportUnitsCount + " != " + reportUnitViews.Length + ". This shouldn't ever happen", base.gameObject);
			yield break;
		}
		List<int> randomIndices = new List<int>(reportUnitsCount);
		for (int i = 0; i < reportUnitsCount; i++)
		{
			randomIndices.Add(i);
		}
		randomIndices.ShuffleList();
		for (int j = 0; j < reportUnitsCount; j++)
		{
			StartCoroutine(reportUnitViews[randomIndices[j]].ShowUnitWithMultiTeamReportUnit(CurrentTeam[randomIndices[j]]));
			yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
		}
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator ShowBattles()
	{
		if (battlesLabelTween != null)
		{
			battlesLabelTween.Kill();
		}
		headerBackgroundTween = headerBackgroundSprite.TweenAlpha(0.5f, 0.5f);
		yield return new WaitForSeconds(0.1f);
		battlesLabelValue.text = Constants.MultiTeamReportBattleCount.ToString();
		battlesLabelTween = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)battlesLabel)
			{
				battlesLabel.Alpha = val;
			}
			if ((bool)battlesLabelValue)
			{
				battlesLabelValue.Alpha = val;
			}
			if ((bool)battlesSprite)
			{
				battlesSprite.Alpha = val;
			}
		});
		if (headerBackgroundTween != null)
		{
			headerBackgroundTween.Kill();
		}
		AudioTrigger.PlayerUnEquipItem.Play();
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ShowEventPoints()
	{
		if (eventPointsLabelTween != null)
		{
			eventPointsLabelTween.Kill();
		}
		eventPointsLabelTween = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)eventPointsLabel)
			{
				eventPointsLabel.Alpha = val;
			}
			if ((bool)eventPointsValue)
			{
				eventPointsValue.Alpha = val;
			}
		});
		totalPointsEarned += currentTeamResults.totalEventPoints;
		int updateFrames = currentTeamResults.totalEventPoints / eventPointsUpdateSpeed;
		for (int i = 0; i < updateFrames; i++)
		{
			eventPointsValue.text = string.Format("+{0}", i * eventPointsUpdateSpeed);
			if (loopSound == null)
			{
				loopSound = AudioTrigger.DieFaceSpin.PlayRepeating();
			}
			yield return 0;
		}
		if (loopSound != null)
		{
			loopSound.Stop();
			loopSound = null;
		}
		eventPointsValue.text = string.Format("+{0}", currentTeamResults.totalEventPoints);
		if (eventPointsSpriteTween != null)
		{
			eventPointsSpriteTween.Kill();
		}
		Vector3 initialScale = Vector3.one * 2f;
		eventPointsSpriteTween = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)eventPointsSprite)
			{
				eventPointsSprite.Alpha = val;
				eventPointsSprite.scale = initialScale - Vector3.one * val;
			}
		});
		AudioTrigger.CoinsEarned.Play();
		yield return new WaitForSeconds(0.5f);
		initialScale = Vector3.one;
		eventPointsLabelTween = SimpleTween.Start(1f, 0f, 0.5f, delegate(float val)
		{
			if ((bool)eventPointsLabel)
			{
				eventPointsLabel.Alpha = val;
				eventPointsLabel.transform.position = Vector3.Lerp(initialEventPointsPosition, eventPointsLabelSmall.transform.position, 1f - val);
				eventPointsLabel.transform.localScale = initialScale - Vector3.one * (0.5f * (1f - val));
			}
			if ((bool)eventPointsValue)
			{
				eventPointsValue.Alpha = val;
			}
			if ((bool)eventPointsSprite)
			{
				eventPointsSprite.Alpha = val;
			}
		});
		eventPointsLabelSmall.text = "ui_multiteam_eventpoints".Localize("Event Points:");
		eventPointsValueSmall.text = "+" + currentTeamResults.totalEventPoints;
		eventPointsLabelSmall.gameObject.SetActive(true);
		if (eventPointsSmallLabelTween != null)
		{
			eventPointsSmallLabelTween.Kill();
		}
		eventPointsSmallLabelTween = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)eventPointsLabelSmall)
			{
				eventPointsLabelSmall.Alpha = val;
			}
			if ((bool)eventPointsValueSmall)
			{
				eventPointsValueSmall.Alpha = val;
			}
			if ((bool)eventPointsSpriteSmall)
			{
				eventPointsSpriteSmall.Alpha = val;
			}
		});
		AudioTrigger.PlayerUnEquipItem.Play();
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ShowBonusPoints()
	{
		if (bonusEventPointsLabelTween != null)
		{
			bonusEventPointsLabelTween.Kill();
		}
		bonusPointsLabel.gameObject.SetActive(true);
		bonusEventPointsLabelTween = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)bonusPointsLabel)
			{
				bonusPointsLabel.Alpha = val;
			}
			if ((bool)bonusEventPointsValue)
			{
				bonusEventPointsValue.Alpha = val;
			}
		});
		totalPointsEarned += currentTeamResults.totalBonusPoints;
		for (int i = 0; i < reportUnitViews.Length; i++)
		{
			if (CurrentTeam[i].bonusEventPointsEarned > 0)
			{
				StartCoroutine(reportUnitViews[i].ShowUnitPoints(CurrentTeam[i].bonusEventPointsEarned, 0.15f * (float)i));
			}
		}
		int updateFrames = currentTeamResults.totalBonusPoints / bonusPointsUpdateSpeed;
		for (int i2 = 0; i2 < updateFrames; i2++)
		{
			bonusEventPointsValue.text = string.Format("+{0}", i2 * eventPointsUpdateSpeed);
			if (loopSound == null)
			{
				loopSound = AudioTrigger.DieFaceSpin.PlayRepeating();
			}
			yield return 0;
		}
		if (loopSound != null)
		{
			loopSound.Stop();
			loopSound = null;
		}
		bonusEventPointsValue.text = string.Format("+{0}", currentTeamResults.totalBonusPoints);
		if (bonusEventPointsSpriteTween != null)
		{
			bonusEventPointsSpriteTween.Kill();
		}
		Vector3 initialScale = Vector3.one * 2f;
		bonusEventPointsSpriteTween = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)bonusEventPointsSprite)
			{
				bonusEventPointsSprite.Alpha = val;
				bonusEventPointsSprite.scale = initialScale - Vector3.one * val;
			}
		});
		AudioTrigger.CoinsEarned.Play();
		yield return new WaitForSeconds(0.6f);
		initialScale = Vector3.one;
		bonusEventPointsLabelTween = SimpleTween.Start(1f, 0f, 0.5f, delegate(float val)
		{
			if ((bool)bonusPointsLabel)
			{
				bonusPointsLabel.Alpha = val;
				bonusPointsLabel.transform.position = Vector3.Lerp(initialBonusPointsPosition, bonusPointsLabelSmall.transform.position, 1f - val);
				bonusPointsLabel.transform.localScale = initialScale - Vector3.one * (0.5f * (1f - val));
			}
			if ((bool)bonusEventPointsValue)
			{
				bonusEventPointsValue.Alpha = val;
			}
			if ((bool)bonusEventPointsSprite)
			{
				bonusEventPointsSprite.Alpha = val;
			}
		});
		bonusPointsLabelSmall.text = "ui_multiteam_bonuspoints".Localize("Bonus Points:");
		bonusPointsValueSmall.text = "+" + currentTeamResults.totalBonusPoints;
		bonusPointsLabelSmall.gameObject.SetActive(true);
		if (bonusEventPointsSmallLabelTween != null)
		{
			bonusEventPointsSmallLabelTween.Kill();
		}
		bonusEventPointsSmallLabelTween = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)bonusPointsLabelSmall)
			{
				bonusPointsLabelSmall.Alpha = val;
			}
			if ((bool)bonusPointsValueSmall)
			{
				bonusPointsValueSmall.Alpha = val;
			}
			if ((bool)bonusPointsSpriteSmall)
			{
				bonusPointsSpriteSmall.Alpha = val;
			}
		});
		AudioTrigger.PlayerUnEquipItem.Play();
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ShowGrade()
	{
		totalGradeGameObject.SetActive(true);
		if (totalGradeTween != null)
		{
			totalGradeTween.Kill();
		}
		totalGradeSprite.SetSprite(currentTeamResults.gradeSpriteName);
		totalGradeTween = totalGradeSprite.TweenAlpha(1f, 0.5f);
		yield return new WaitForSeconds(0.25f);
		Vector3 initialScale = Vector3.one * 2f;
		totalGradeTween = SimpleTween.Start(0f, 1f, 0.25f, delegate(float val)
		{
			if ((bool)totalGradeSprite)
			{
				totalGradeSprite.Alpha = val;
				totalGradeSprite.scale = initialScale - Vector3.one * val;
			}
		});
		if ((bool)shaker)
		{
			shaker.Shake(true);
		}
		AudioTrigger.CrateLand.Play();
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator UpdateTotalPoints()
	{
		if (totalResultTween != null)
		{
			totalResultTween.Kill();
		}
		totalResultTween = totalResultGameObject.transform.TweenLocalYPosition(totalResultFinalYPosition, 0.5f);
		while (true)
		{
			if (currentGoalPoints < totalPointsEarned)
			{
				currentGoalPoints = Mathf.Min(currentGoalPoints + (float)totalEventPointsSpeed * Time.deltaTime, totalPointsEarned);
				totalPointsEarnedValue.text = "+" + Mathf.FloorToInt(currentGoalPoints);
			}
			yield return 0;
		}
	}

	private void OnPressHandler()
	{
		switch (currentState)
		{
		case ReportViewState.IN_PROGRESS:
		case ReportViewState.EMPTY_TEAM:
			SkipSequence(currentState == ReportViewState.EMPTY_TEAM);
			SetCurrentTeamSequenceAsCompleted();
			break;
		case ReportViewState.COMPLETED:
			if (currentTeamIndex >= multiTeamReport.Teams.Count)
			{
				SendMultiTeamReportAnalytics();
				controller.OnCloseButton();
			}
			else
			{
				StartCoroutine(StartSequence());
			}
			break;
		}
	}

	private void SendMultiTeamReportAnalytics()
	{
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent();
		int activeEventId = ((activeOnCooldownEvent == null) ? (-1) : int.Parse(activeOnCooldownEvent.id));
		Reporting.MultiTeamReportClaimed(UserProfile.player.teams, activeEventId, totalPointsEarned);
	}

	private void OnButtonPressed(tk2dUIItem button)
	{
		OnPressHandler();
	}

	private void KillTweens()
	{
		if (teamLabelTween != null)
		{
			teamLabelTween.Kill();
		}
		if (battlesLabelTween != null)
		{
			battlesLabelTween.Kill();
		}
		if (eventPointsLabelTween != null)
		{
			eventPointsLabelTween.Kill();
		}
		if (eventPointsSpriteTween != null)
		{
			eventPointsSpriteTween.Kill();
		}
		if (eventPointsSmallLabelTween != null)
		{
			eventPointsSmallLabelTween.Kill();
		}
		if (bonusEventPointsLabelTween != null)
		{
			bonusEventPointsLabelTween.Kill();
		}
		if (bonusEventPointsSpriteTween != null)
		{
			bonusEventPointsSpriteTween.Kill();
		}
		if (bonusEventPointsSmallLabelTween != null)
		{
			bonusEventPointsSmallLabelTween.Kill();
		}
		if (headerBackgroundTween != null)
		{
			headerBackgroundTween.Kill();
		}
		if (totalResultTween != null)
		{
			totalResultTween.Kill();
		}
		if (totalGradeTween != null)
		{
			totalGradeTween.Kill();
		}
		if (incompleteTeamTween != null)
		{
			incompleteTeamTween.Kill();
		}
	}

	private void OnDestroy()
	{
		KillTweens();
	}
}
