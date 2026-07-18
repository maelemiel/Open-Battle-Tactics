using UnityEngine;

public class ServerSelectController : MonoBehaviour
{
	public ServersSelectionPopUpController _serverSelectionPopUp;

	public tk2dUIToggleButtonGroup _toggleGroup;

	public tk2dTextMesh[] _buttonTextFields;

	private void Start()
	{
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.BootReady, BootInit);
	}

	private void BootInit()
	{
		int num = 0;
		AppConfig.LoadCurrentEnvironment();
		_toggleGroup.SelectedIndex = 0;
		foreach (AppConfig.EnvironmentType key in AppConfig.AvailableServers.Keys)
		{
			_buttonTextFields[num].text = key.ToString();
			if (AppConfig.currentEnvironmentType == key)
			{
				_toggleGroup.SelectedIndex = num;
			}
			num++;
		}
	}

	public void SetButtonText(int buttonIndex, string text)
	{
		_buttonTextFields[buttonIndex].text = text;
	}

	public void OnSelectionChange()
	{
		string text = _buttonTextFields[_toggleGroup.SelectedIndex].text;
		AppConfig.EnvironmentType environment = ParseUtility.ParseEnum(text, AppConfig.EnvironmentType.Dev);
		AppConfig.SetEnvironment(environment);
	}
}
