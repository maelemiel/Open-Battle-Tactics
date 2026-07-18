using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class UserLocalNotificationsController : MonoBehaviour
{
	[SerializeField]
	private LocalNotificationController notificationController;

	[SerializeField]
	private float openY;

	[SerializeField]
	private float closedY;

	public float timeToOpen = 0.5f;

	public float timeToShowNotification = 3f;

	public float timeBetweenNotifications = 0.5f;

	public float updateTime = 1f;

	private UserProfile userProfile;

	private tk2dSpriteCollectionData spriteCollection;

	private string starSpriteName;

	private List<LocalUserNotificationModel> currentNotifications = new List<LocalUserNotificationModel>();

	private Tweener currentTweener;

	private List<INotificationChecker> notificationCheckers = new List<INotificationChecker>
	{
		new BuiltTankNotificationChecker(),
		new ReadyToBuildTanksNotificationChecker(),
		new PrizeGachaNotificationChecker()
	};

	private bool _isOpen = true;

	public bool IsOpen
	{
		get
		{
			return _isOpen;
		}
		set
		{
			SetOpen(value);
			_isOpen = value;
		}
	}

	public void EnableCheckingState()
	{
		StartCoroutine(CheckNotifications());
		StartCoroutine(RunNotificationsSequence());
	}

	public void DisableCheckingState()
	{
		StopAllCoroutines();
		if (currentTweener != null)
		{
			currentTweener.Kill();
		}
		notificationController.transform.SetLocalYPosition(closedY);
	}

	public void SetOpen(bool isOpen)
	{
		StopAllCoroutines();
		if (isOpen)
		{
			EnableCheckingState();
		}
		StartCoroutine(OpenCloseSequence(isOpen));
	}

	private IEnumerator OpenCloseSequence(bool isOpen)
	{
		currentTweener = Extensions.TweenLocalYPosition(newLocalYPosition: (!isOpen) ? closedY : openY, transform: notificationController.transform, duration: timeToOpen);
		yield break;
	}

	public void Toggle()
	{
		IsOpen = !IsOpen;
	}

	public void Init(TopBarController controller)
	{
		IsOpen = false;
		InitNotificationCheckers();
	}

	private void InitNotificationCheckers()
	{
		for (int i = 0; i < notificationCheckers.Count; i++)
		{
			notificationCheckers[i].Init();
		}
	}

	private IEnumerator CheckNotifications()
	{
		while (true)
		{
			for (int i = 0; i < notificationCheckers.Count; i++)
			{
				notificationCheckers[i].CheckConditions();
			}
			yield return new WaitForSeconds(updateTime);
		}
	}

	public void AddNotification(LocalUserNotificationModel localUserNotification)
	{
		currentNotifications.Add(localUserNotification);
	}

	private IEnumerator RunNotificationsSequence()
	{
		while (true)
		{
			if (currentNotifications.Count > 0 && base.enabled)
			{
				LocalUserNotificationModel userNotification = currentNotifications[0];
				yield return StartCoroutine(ShowNotification(userNotification));
			}
			yield return null;
		}
	}

	private IEnumerator ShowNotification(LocalUserNotificationModel notification)
	{
		if ((bool)notificationController)
		{
			notificationController.NotificationEnabled = true;
		}
		if (notificationController.ShowNotification(notification))
		{
			yield return StartCoroutine(OpenCloseSequence(true));
			yield return new WaitForSeconds(timeToShowNotification);
			yield return StartCoroutine(OpenCloseSequence(false));
		}
		if (currentNotifications.Count > 0)
		{
			currentNotifications.RemoveAt(0);
		}
		if ((bool)notificationController)
		{
			notificationController.NotificationEnabled = false;
		}
		yield return new WaitForSeconds(timeBetweenNotifications);
	}
}
