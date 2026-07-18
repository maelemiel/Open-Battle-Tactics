using System;
using System.Collections.Generic;
using LitJson0;

public class UINetworkErrorHandler
{
	private static string UNKNOWN_ERROR = "__unknown_error__";

	private static Dictionary<string, Action<TypedRestResponse<JsonObject>>> ERROR_ACTIONS = new Dictionary<string, Action<TypedRestResponse<JsonObject>>>
	{
		{ "DataModelUpdateRequired", DataModelUpdateRequired },
		{ "BinaryUpdateRequired", BinaryUpdateRequired },
		{ "Maintenance", Maintenance },
		{ "SessionExpired", SessionExpired },
		{ "PCLoadLetter", PCLoadLetter },
		{ "communicationError", CommunicationError },
		{ UNKNOWN_ERROR, UnknownError }
	};

	public static void DisplayMessage(string error)
	{
		if (!ERROR_ACTIONS.ContainsKey(error))
		{
			Log.DebugTag("No error was defined for {0}, showing unknown", null, "UINetworkHandler", error);
			error = UNKNOWN_ERROR;
		}
		ERROR_ACTIONS[error](null);
	}

	public static void DisplayMessage(TypedRestResponse<JsonObject> response)
	{
		string text = (string)response.Error;
		if (!ERROR_ACTIONS.ContainsKey(text))
		{
			Log.DebugTag("No error was defined for {0}, showing unknown", null, "UINetworkHandler", text);
			text = UNKNOWN_ERROR;
		}
		ERROR_ACTIONS[text](response);
	}

	private static void DataModelUpdateRequired(TypedRestResponse<JsonObject> response)
	{
		PopupManager.highDepThPopup = true;
		PopupManager.ShowPopup(PopupDataModel.DataModelUpdateRequired());
	}

	private static void BinaryUpdateRequired(TypedRestResponse<JsonObject> response)
	{
		PopupManager.highDepThPopup = true;
		PopupManager.ShowPopup(PopupDataModel.BinaryUpdateRequired());
	}

	private static void Maintenance(TypedRestResponse<JsonObject> response)
	{
		PopupManager.highDepThPopup = true;
		PopupManager.ShowPopup(PopupDataModel.Maintenance());
	}

	private static void SessionExpired(TypedRestResponse<JsonObject> response)
	{
		PopupManager.ShowPopup(PopupDataModel.SessionExpiredError(QuitUtility.Restart));
	}

	private static void PCLoadLetter(TypedRestResponse<JsonObject> response)
	{
		PopupManager.ShowPopup(PopupDataModel.NetworkError(QuitUtility.Restart));
	}

	private static void CommunicationError(TypedRestResponse<JsonObject> response)
	{
		PopupManager.ShowPopup(PopupDataModel.NetworkError(QuitUtility.Restart));
	}

	private static void UnknownError(TypedRestResponse<JsonObject> response)
	{
		PopupManager.highDepThPopup = true;
		if (!AppConfig.verboseErrors)
		{
			PopupManager.ShowPopup(PopupDataModel.OutOfSync(QuitUtility.Restart));
		}
		else
		{
			PopupManager.ShowPopup(PopupDataModel.NetworkError(QuitUtility.Restart));
		}
	}
}
