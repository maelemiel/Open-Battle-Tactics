using System.Collections.Generic;

public class UserDialogTriggerData
{
	private KeyValueStorage dialogTriggerVisitsKVS;

	private List<int> dialogsTriggered = new List<int>();

	public UserDialogTriggerData(UserProfile user)
	{
		dialogTriggerVisitsKVS = KeyValueStorage.Instance(KeyValueStorage.Storage.DIALOG_TRIGGER_VISITS);
	}

	public bool ShouldTriggerDialog(string triggerName)
	{
		return _ShouldTriggerDialog(DialogScreenDataModel.GetDialogScreenDataModelWithScreenId(triggerName));
	}

	private bool _ShouldTriggerDialog(DialogScreenDataModel dialogScreen)
	{
		if (dialogScreen == null)
		{
			return false;
		}
		int value = dialogTriggerVisitsKVS.GetValue<int>(dialogScreen.screenId);
		if (value >= 1)
		{
			return false;
		}
		if (dialogScreen.limitOne == 1)
		{
			return !dialogsTriggered.Contains(int.Parse(dialogScreen.id));
		}
		return true;
	}

	private void _SetDialogTriggered(DialogScreenDataModel dialogScreen, bool persist)
	{
		if (dialogScreen == null)
		{
			return;
		}
		int num = int.Parse(dialogScreen.id);
		if (!dialogsTriggered.Contains(num))
		{
			dialogsTriggered.Add(num);
			if (persist)
			{
				Singleton<SessionManager>.instance.SaveDialogTrigger(num);
			}
		}
	}

	public void SetDialogTriggered(string triggerName, bool persist = true)
	{
		_SetDialogTriggered(DialogScreenDataModel.GetDialogScreenDataModelWithScreenId(triggerName), persist);
	}

	public void SetDialogTriggered(int triggerID, bool persist = true)
	{
		_SetDialogTriggered(DialogScreenDataModel.GetSingle(triggerID), persist);
	}

	public void ClearTriggers()
	{
		dialogTriggerVisitsKVS.ClearValues();
	}
}
