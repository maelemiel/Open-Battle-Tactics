using System;
using UnityEngine;

[Serializable]
public class NotificationObject
{
	public NotificationType notificationType;

	public GameObject notificationSprite;

	public tk2dTextMesh notificationText;

	public void SetNotificationState(bool state, int count = 0)
	{
		notificationSprite.gameObject.SetActive(state);
		if (state && (bool)notificationText)
		{
			notificationText.text = count.ToString();
		}
	}
}
