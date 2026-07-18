using System;
using UnityEngine;

public class GCMReceiver : MonoBehaviour
{
	public static Action<string> _onError;

	public static Action<string> _onMessage;

	public static Action<string> _onRegistered;

	public static Action<string> _onUnregistered;

	public static Action<int> _onDeleteMessages;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
	}

	private void OnError(string errorId)
	{
		Debug.Log("Error: " + errorId);
		if (_onError != null)
		{
			_onError(errorId);
		}
	}

	private void OnMessage(string message)
	{
		Debug.Log("MetaData: " + message);
		if (_onMessage != null)
		{
			_onMessage(message);
		}
	}

	private void pMessage(string message)
	{
		Debug.Log("pMessage: " + message);
		OtherLevelsSDK.PushPhashForTracking(message);
		OtherLevelsSDK.TrackLastPhashOpen();
	}

	private void OnRegistered(string registrationId)
	{
		Debug.Log("Registered: " + registrationId);
		PlayerPrefs.SetString("OL_AndroidToken", registrationId);
		OtherLevelsSDK.RegisterDevice(string.Empty, registrationId);
		if (_onRegistered != null)
		{
			_onRegistered(registrationId);
		}
	}

	private void OnUnregistered(string registrationId)
	{
		Debug.Log("Unregistered: " + registrationId);
		if (_onUnregistered != null)
		{
			_onUnregistered(registrationId);
		}
	}

	private void OnDeleteMessages(string total)
	{
		Debug.Log("DeleteMessages: " + total);
		if (_onDeleteMessages != null)
		{
			int obj = Convert.ToInt32(total);
			_onDeleteMessages(obj);
		}
	}
}
