public class BaseLocalizationDataModel : BaseDataModel
{
	public string keyName;

	public string keyDescription;

	public string name
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(keyName);
		}
	}

	public string description
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(keyDescription);
		}
	}
}
