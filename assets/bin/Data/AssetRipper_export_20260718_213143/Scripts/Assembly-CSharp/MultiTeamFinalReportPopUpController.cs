using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class MultiTeamFinalReportPopUpController : PopupController
{
	public class TeamReportResult
	{
		public int totalEventPoints;

		public int totalBonusPoints;

		public int teamIndex;

		public TeamReportResult(int teamIndex)
		{
			this.teamIndex = teamIndex;
		}
	}

	[SerializeField]
	private MovingObjectController popupContentController;

	[SerializeField]
	private MultiTeamFinalReportItemViewController[] results;

	[SerializeField]
	private MultiTeamFinalReportItemViewController totalResult;

	private UserMultiTeamReport userMultiTeamReport;

	private List<TeamReportResult> teamReportResultsSummary;

	private int totalEventPointsEarned;

	private int totalBonusPointsEarned;

	public float initialResultsLocalPosition = 800f;

	public float finalResultsLocalPosition = -400f;

	public ObjectShaker objectShaker;

	private Tweener totalPointsTween;

	protected override void Start()
	{
		base.Start();
		ClearSequence();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		userMultiTeamReport = (UserMultiTeamReport)model.payload;
		if (userMultiTeamReport == null)
		{
			Log.Error("UserMultiTeamReport not found", base.gameObject);
			OnCloseButton();
		}
		if ((bool)popupContentController)
		{
			popupContentController.IsOpen = true;
			popupContentController.OnClosed += DestroyPopUp;
		}
		for (int i = 0; i < results.Length; i++)
		{
			results[i].Init(i + 1);
		}
		if ((bool)totalResult)
		{
			totalResult.Init(-1);
		}
		if ((bool)_closeButton)
		{
			_closeButton.gameObject.SetActive(false);
		}
		ProcessResults();
		ClearSequence();
		StartCoroutine(PlaySequence());
	}

	private IEnumerator PlaySequence()
	{
		yield return new WaitForSeconds(0.5f);
		MultiTeamFinalReportItemViewController[] array = results;
		foreach (MultiTeamFinalReportItemViewController unit in array)
		{
			unit.transform.TweenLocalXPosition(finalResultsLocalPosition, 0.5f);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.5f);
		if (results.Length != teamReportResultsSummary.Count)
		{
			Log.Error("Final Report Model and View are out of sync. The view count is different than the data count: " + results.Length + " != " + teamReportResultsSummary.Count, base.gameObject);
			yield break;
		}
		for (int j = 0; j < results.Length; j++)
		{
			yield return StartCoroutine(results[j].ShowValues(teamReportResultsSummary[j].totalEventPoints, teamReportResultsSummary[j].totalBonusPoints));
		}
		if ((bool)totalResult)
		{
			totalResult.transform.TweenLocalXPosition(finalResultsLocalPosition, 0.25f);
			yield return new WaitForSeconds(0.3f);
			StartCoroutine(totalResult.ShowValues(totalEventPointsEarned + totalBonusPointsEarned, 0, false));
		}
		teamReportResultsSummary.Sort((TeamReportResult x, TeamReportResult y) => (x.totalEventPoints + x.totalBonusPoints).CompareTo(y.totalEventPoints + y.totalBonusPoints));
		int rank = teamReportResultsSummary.Count - 1;
		foreach (TeamReportResult orderedResult in teamReportResultsSummary)
		{
			StartCoroutine(results[orderedResult.teamIndex].ShowRanking(rank));
			rank--;
		}
		if ((bool)objectShaker)
		{
			objectShaker.Shake(true);
		}
		if ((bool)_closeButton)
		{
			_closeButton.gameObject.SetActive(true);
		}
	}

	private void ProcessResults()
	{
		teamReportResultsSummary = new List<TeamReportResult>
		{
			new TeamReportResult(0),
			new TeamReportResult(1),
			new TeamReportResult(2)
		};
		int eventPoints = 0;
		int bonusPoints = 0;
		userMultiTeamReport.CalculateEventPoints();
		for (int i = 0; i < userMultiTeamReport.teamReports.Count; i++)
		{
			for (int j = 0; j < userMultiTeamReport.teamReports[i].Teams.Count; j++)
			{
				userMultiTeamReport.teamReports[i].GetPointsWithTeamIndex(j, out eventPoints, out bonusPoints);
				teamReportResultsSummary[j].totalEventPoints += eventPoints;
				teamReportResultsSummary[j].totalBonusPoints += bonusPoints;
				totalEventPointsEarned += eventPoints;
				totalBonusPointsEarned += bonusPoints;
			}
		}
	}

	private void ClearSequence()
	{
		MultiTeamFinalReportItemViewController[] array = results;
		foreach (MultiTeamFinalReportItemViewController multiTeamFinalReportItemViewController in array)
		{
			multiTeamFinalReportItemViewController.transform.SetLocalXPosition(initialResultsLocalPosition);
		}
		if ((bool)totalResult)
		{
			totalResult.transform.SetLocalXPosition(initialResultsLocalPosition);
		}
	}

	public override void OnCloseButton()
	{
		if ((bool)popupContentController)
		{
			popupContentController.IsOpen = false;
		}
		base.OnCloseButton();
	}

	private void DestroyPopUp()
	{
		PopupManager.DestroyPopup(model);
	}
}
