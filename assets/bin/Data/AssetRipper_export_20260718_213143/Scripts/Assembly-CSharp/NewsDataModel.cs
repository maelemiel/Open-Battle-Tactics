using System;
using System.Collections.Generic;
using System.Globalization;

public class NewsDataModel : BaseDataModel
{
	public string announcerType;

	public int assetLinkageId;

	public string background;

	public int backgroundSpeed;

	public string endTime;

	public int gachaId;

	public int orderNumber;

	public int showNumber;

	public string startTime;

	public string textDescription1;

	public string textDescription2;

	public string textDescription3;

	public string textDescription4;

	public string textDescription5;

	public string title;

	public int unitId;

	public bool Enabled
	{
		get
		{
			if (startTime == "0")
			{
				return false;
			}
			if (startTime == "1")
			{
				return true;
			}
			DateTime t = DateTime.Parse(startTime, CultureInfo.InvariantCulture).ToUniversalTime();
			if (DateTime.Compare(t, DateTime.UtcNow) > 0)
			{
				return false;
			}
			if (endTime == "1")
			{
				return true;
			}
			t = DateTime.Parse(endTime, CultureInfo.InvariantCulture).ToUniversalTime();
			return DateTime.Compare(t, DateTime.UtcNow) > 0;
		}
	}

	public static NewsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<NewsDataModel>(id.ToString());
	}

	public static NewsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<NewsDataModel>(id);
	}

	public static List<NewsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<NewsDataModel>();
	}
}
