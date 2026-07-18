using System.Collections.Generic;

public class AnnouncerDialogSequencesDataModel : BaseDataModel
{
	public string action;

	public string announcerType;

	public string keyName;

	public string orientation;

	public int sequenceId;

	public int subsequenceOrder;

	public static AnnouncerDialogSequencesDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AnnouncerDialogSequencesDataModel>(id.ToString());
	}

	public static AnnouncerDialogSequencesDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AnnouncerDialogSequencesDataModel>(id);
	}

	public static List<AnnouncerDialogSequencesDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AnnouncerDialogSequencesDataModel>();
	}

	public static List<AnnouncerDialogSequencesDataModel> GetSequenceDataForSequenceId(int sequenceId)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<AnnouncerDialogSequencesDataModel>(" WHERE sequence_id = " + sequenceId);
	}
}
