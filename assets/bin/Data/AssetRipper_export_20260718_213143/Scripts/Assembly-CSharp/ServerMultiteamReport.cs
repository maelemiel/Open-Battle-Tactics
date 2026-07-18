public class ServerMultiteamReport
{
	public UserMultiTeamReport userMultiTeamReport;

	public ClaimedReportData claimedReportData;

	public bool IsFinalReport
	{
		get
		{
			return claimedReportData == null;
		}
	}
}
