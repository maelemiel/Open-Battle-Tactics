using System.Collections.Generic;
using LitJson0;

public class UserMultiTeamReport
{
	public class MultiTeamReport
	{
		public int reportId;

		public long randomSeed;

		private List<List<UserMultiTeamReportUnit>> teams = new List<List<UserMultiTeamReportUnit>>();

		public List<List<UserMultiTeamReportUnit>> Teams
		{
			get
			{
				return teams;
			}
		}

		public MultiTeamReport()
		{
		}

		public MultiTeamReport(int reportId, long randomSeed)
		{
			this.reportId = reportId;
			this.randomSeed = randomSeed;
		}

		private void SetTeams(List<List<UserMultiTeamReportUnit>> teams)
		{
			this.teams = teams;
			ProcessTeams();
		}

		private void ProcessTeams()
		{
			foreach (List<UserMultiTeamReportUnit> team in teams)
			{
				foreach (UserMultiTeamReportUnit item in team)
				{
					if (item.unitLevelProgressionDataModel == null)
					{
						item.unitLevelProgressionDataModel = UnitLevelProgressionDataModel.GetSingle(item.levelId);
					}
					if (item.unitDataModel == null)
					{
						item.unitDataModel = UnitDataModel.GetSingle(item.unitLevelProgressionDataModel.unitId);
					}
				}
			}
		}

		public void GetPointsWithTeamIndex(int teamIndex, out int eventPoints, out int bonusPoints)
		{
			eventPoints = 0;
			bonusPoints = 0;
			foreach (UserMultiTeamReportUnit item in teams[teamIndex])
			{
				eventPoints += item.eventPointsEarned;
				bonusPoints += item.bonusEventPointsEarned;
			}
		}

		public static MultiTeamReport FromJSON(JsonObject jsonReport)
		{
			MultiTeamReport multiTeamReport = new MultiTeamReport();
			if (jsonReport.Contains("report_id"))
			{
				multiTeamReport.reportId = jsonReport.GetInt("report_id");
			}
			if (jsonReport.Contains("random_seed"))
			{
				multiTeamReport.randomSeed = jsonReport.GetLong("random_seed");
			}
			if (jsonReport.Contains("teams"))
			{
				List<List<UserMultiTeamReportUnit>> list = new List<List<UserMultiTeamReportUnit>>();
				foreach (JsonObject @object in jsonReport.GetObjectList("teams"))
				{
					if (!@object.Contains("units"))
					{
						continue;
					}
					List<UserMultiTeamReportUnit> list2 = new List<UserMultiTeamReportUnit>();
					foreach (JsonObject object2 in @object.GetObjectList("units"))
					{
						if (object2.Contains("unit"))
						{
							list2.Add(new UserMultiTeamReportUnit(object2.GetInt("unit")));
						}
					}
					list.Add(list2);
				}
				multiTeamReport.SetTeams(list);
			}
			return multiTeamReport;
		}
	}

	public int eventId;

	public long finishTimestamp;

	public List<MultiTeamReport> teamReports = new List<MultiTeamReport>();

	public MultiTeamReport LastAvailableReport
	{
		get
		{
			return teamReports[teamReports.Count - 1];
		}
	}

	public bool IsClaimable
	{
		get
		{
			return NonUnitySingleton<TimeManager>.instance.IsTimestampInPast(finishTimestamp);
		}
	}

	public long RemainingTime
	{
		get
		{
			long num = finishTimestamp - TimeManager.ServerTime;
			return (num <= 0) ? 0 : num;
		}
	}

	public string RemainingTimeText
	{
		get
		{
			return TimeFormats.GetTimeString(RemainingTime, TimeFormat.NUMBER);
		}
	}

	public static UserMultiTeamReport FromJSON(JsonObject jsonMultiTeamReport)
	{
		if (jsonMultiTeamReport == null || jsonMultiTeamReport.Dictionary == null)
		{
			return null;
		}
		UserMultiTeamReport userMultiTeamReport = new UserMultiTeamReport();
		if (jsonMultiTeamReport.Contains("event_id"))
		{
			userMultiTeamReport.eventId = int.Parse(jsonMultiTeamReport.GetString("event_id"));
		}
		if (jsonMultiTeamReport.Contains("finish_timestamp"))
		{
			userMultiTeamReport.finishTimestamp = jsonMultiTeamReport.GetLong("finish_timestamp");
		}
		if (jsonMultiTeamReport.Contains("reports"))
		{
			List<MultiTeamReport> list = new List<MultiTeamReport>();
			foreach (JsonObject @object in jsonMultiTeamReport.GetObjectList("reports"))
			{
				list.Add(MultiTeamReport.FromJSON(@object));
			}
			userMultiTeamReport.teamReports = list;
		}
		return userMultiTeamReport;
	}

	public void CalculateEventPoints()
	{
		MersenneTwister mersenneTwister = null;
		foreach (MultiTeamReport teamReport in teamReports)
		{
			mersenneTwister = new MersenneTwister((uint)teamReport.randomSeed);
			foreach (List<UserMultiTeamReportUnit> team in teamReport.Teams)
			{
				if (team.Count == Constants.MinUnitsPerTeam)
				{
					CalculateTeamEventPoints(team, mersenneTwister, teamReport.randomSeed);
				}
			}
		}
	}

	private void CalculateTeamEventPoints(List<UserMultiTeamReportUnit> team, MersenneTwister randomProvider, long seed)
	{
		UserProfile player = UserProfile.player;
		int num = 0;
		int num2 = 0;
		EventMultiTeamEffectivenessDataModel eventMultiTeamEffectivenessDataModel = null;
		EventDataModel activeOnCooldownEvent = player.GetActiveOnCooldownEvent();
		foreach (UserMultiTeamReportUnit item in team)
		{
			eventMultiTeamEffectivenessDataModel = EventMultiTeamEffectivenessDataModel.GetSingle(item.unitLevelProgressionDataModel.Rarity);
			switch (GetUnitMultiTeamType(item.unitDataModel, activeOnCooldownEvent))
			{
			case MultiReportUnitType.REGULAR_UNIT:
				num = Constants.MultiTeamReportBattleCount * eventMultiTeamEffectivenessDataModel.nonEventUnitRankMin;
				num2 = Constants.MultiTeamReportBattleCount * eventMultiTeamEffectivenessDataModel.nonEventUnitRankMax;
				break;
			case MultiReportUnitType.POST_EVENT_UNIT:
				num = Constants.MultiTeamReportBattleCount * eventMultiTeamEffectivenessDataModel.postEventUnitMin;
				num2 = Constants.MultiTeamReportBattleCount * eventMultiTeamEffectivenessDataModel.postEventUnitMax;
				break;
			case MultiReportUnitType.EVENT_UNIT:
				num = Constants.MultiTeamReportBattleCount * eventMultiTeamEffectivenessDataModel.eventUnitRankMin;
				num2 = Constants.MultiTeamReportBattleCount * eventMultiTeamEffectivenessDataModel.eventUnitRankMax;
				break;
			}
			int num3 = randomProvider.Next(num2 - num);
			item.eventPointsEarned = num + num3;
			EventUnitBoostDataModel eventUnitBoostDataModel = null;
			if (activeOnCooldownEvent != null)
			{
				eventUnitBoostDataModel = EventUnitBoostDataModel.FindUnitBoost(item.unitDataModel.id, item.unitLevelProgressionDataModel.level, activeOnCooldownEvent.id);
			}
			if (eventUnitBoostDataModel != null)
			{
				float num4 = (float)eventUnitBoostDataModel.bonusPointsBoost * 0.01f;
				item.bonusEventPointsEarned = (int)((float)item.eventPointsEarned * num4);
			}
		}
	}

	private MultiReportUnitType GetUnitMultiTeamType(UnitDataModel unitDataModel, EventDataModel activeEvent)
	{
		return (!unitDataModel.UnitType.IsEvent()) ? MultiReportUnitType.REGULAR_UNIT : ((activeEvent == null || !activeEvent.UnitBelongsToEvent(unitDataModel.id)) ? MultiReportUnitType.POST_EVENT_UNIT : MultiReportUnitType.EVENT_UNIT);
	}
}
