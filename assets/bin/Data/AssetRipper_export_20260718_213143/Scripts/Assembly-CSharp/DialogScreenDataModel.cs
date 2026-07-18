using System.Collections.Generic;

public class DialogScreenDataModel : BaseDataModel
{
	public int limitOne;

	public string screenId;

	public int sequenceId;

	public static DialogScreenDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<DialogScreenDataModel>(id.ToString());
	}

	public static DialogScreenDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<DialogScreenDataModel>(id);
	}

	public static List<DialogScreenDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<DialogScreenDataModel>();
	}

	public static DialogScreenDataModel GetDialogScreenDataModelWithScreenId(string screenId)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingleByQuery<DialogScreenDataModel>(" WHERE screen_id = \"" + screenId + "\"");
	}
}
