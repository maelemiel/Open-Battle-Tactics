using LitJson0;

public class ClaimedReportData
{
	public string eventId;

	public int reportId;

	public bool IsMoreRecentThan(ClaimedReportData otherData)
	{
		return eventId != otherData.eventId || reportId > otherData.reportId;
	}

	public static ClaimedReportData FromJSON(JsonObject jsonMultiTeamReport)
	{
		ClaimedReportData claimedReportData = new ClaimedReportData();
		if (jsonMultiTeamReport.Contains("event_id"))
		{
			claimedReportData.eventId = jsonMultiTeamReport.GetString("event_id");
		}
		if (jsonMultiTeamReport.Contains("report_id"))
		{
			claimedReportData.reportId = jsonMultiTeamReport.GetInt("report_id");
		}
		return claimedReportData;
	}
}
