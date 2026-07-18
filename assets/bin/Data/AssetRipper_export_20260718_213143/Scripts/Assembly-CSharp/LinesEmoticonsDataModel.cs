using System.Collections.Generic;

public class LinesEmoticonsDataModel : BaseDataModel
{
	public string emoticonKeyString;

	public int emoticonType;

	public static LinesEmoticonsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LinesEmoticonsDataModel>(id.ToString());
	}

	public static LinesEmoticonsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<LinesEmoticonsDataModel>(id);
	}

	public static List<LinesEmoticonsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<LinesEmoticonsDataModel>();
	}
}
