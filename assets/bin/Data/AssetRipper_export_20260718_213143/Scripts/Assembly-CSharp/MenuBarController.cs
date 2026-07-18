using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBarController : MonoBehaviour
{
	[Serializable]
	public class MenuButtonOption
	{
		public SceneTransitionManager.Scene scene;

		public string buttonID;

		public string functionToCall;

		public string iconName;

		public Color textColour;

		public float textSize;

		public string textLocalization;
	}

	[SerializeField]
	private float openY;

	[SerializeField]
	private float closedY;

	[SerializeField]
	private float buttonOpenedX;

	[SerializeField]
	private float buttonClosedX;

	[SerializeField]
	private tk2dSlicedSprite overlaySprite;

	[SerializeField]
	private tk2dSprite selectedItemSprite;

	[SerializeField]
	private ScrollableAreaController scrollController;

	private SceneTransitionManager.Scene[] sceneIndexes;

	public List<MenuButtonOption> menuOptions;

	public float timeToOpenButton = 0.15f;

	public float timeBetweenButtons = 0.075f;

	public float timeToOpenMenu = 0.5f;

	public int highlightRepetitions = 2;

	public float timeBetweenRepetitions = 0.2f;

	public float alphaOverlay = 0.35f;

	private Dictionary<string, int> buttonTiers = new Dictionary<string, int>();

	private bool _isOpen;

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

	public int ButtonCount
	{
		get
		{
			return menuOptions.Count;
		}
	}

	private void Awake()
	{
		overlaySprite.gameObject.SetActive(false);
		sceneIndexes = new SceneTransitionManager.Scene[menuOptions.Count];
		for (int i = 0; i < menuOptions.Count; i++)
		{
			sceneIndexes[i] = menuOptions[i].scene;
		}
	}

	public void Init()
	{
		scrollController.InitializeWithData(menuOptions);
		MenuBarNotificationsController component = GetComponent<MenuBarNotificationsController>();
		LinkedListNode<GameObject> linkedListNode = scrollController.CellsInUse.First;
		while (linkedListNode.Next != null)
		{
			MenuBarButton component2 = linkedListNode.Value.GetComponent<MenuBarButton>();
			if (component2 != null)
			{
				List<NotificationObject> list = new List<NotificationObject>();
				NotificationObject notificationObject = new NotificationObject();
				notificationObject.notificationType = NotificationType.RED;
				notificationObject.notificationText = component2.notificationTextMeshs[0];
				notificationObject.notificationSprite = component2.notificationGameObjects[0];
				list.Add(notificationObject);
				notificationObject = new NotificationObject();
				notificationObject.notificationType = NotificationType.GREEN;
				notificationObject.notificationText = component2.notificationTextMeshs[1];
				notificationObject.notificationSprite = component2.notificationGameObjects[1];
				list.Add(notificationObject);
				component.UpdateGameObjects(component2.scene, list);
				if (!buttonTiers.ContainsKey(component2.ButtonID))
				{
					buttonTiers.Add(component2.ButtonID, Constants.GetIntConstantWithID(component2.ButtonID));
				}
				else
				{
					buttonTiers[component2.ButtonID] = Constants.GetIntConstantWithID(component2.ButtonID);
				}
				linkedListNode = linkedListNode.Next;
				continue;
			}
			break;
		}
		component.SetupHandlers();
		openY = 38.5f;
		SetStateImmediate(false);
	}

	internal NotificationType UpdateNotificationForScene(SceneTransitionManager.Scene scene)
	{
		MenuBarNotificationsController component = GetComponent<MenuBarNotificationsController>();
		return component.UpdateNotificationForScene(scene);
	}

	public void SetMenuButtonStates()
	{
		if (buttonTiers.Count == 0)
		{
			return;
		}
		UserProfile player = UserProfile.player;
		LinkedListNode<GameObject> linkedListNode = scrollController.CellsInUse.First;
		while (linkedListNode.Next != null)
		{
			MenuBarButton component = linkedListNode.Value.GetComponent<MenuBarButton>();
			if (component != null)
			{
				SetButtonState(component, buttonTiers[component.ButtonID] <= player.divisionInt);
			}
			linkedListNode = linkedListNode.Next;
		}
	}

	public void SetOpen(bool isOpen)
	{
		base.gameObject.SetActive(true);
		StopAllCoroutines();
		StartCoroutine(OpenCloseSequence(isOpen));
	}

	public void SetStateImmediate(bool isOpen)
	{
		StopAllCoroutines();
		_isOpen = isOpen;
		OpenCloseImmediate(isOpen);
	}

	private void OpenCloseImmediate(bool isOpen)
	{
		overlaySprite.gameObject.SetActive(isOpen);
		FadeOverlay(isOpen, 1f);
		float xPosition = ((!isOpen) ? closedY : openY);
		base.transform.SetLocalXPosition(xPosition);
		OpenMenuButtonsImmediate(isOpen);
		if (!isOpen)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OpenMenuButtonsImmediate(bool state)
	{
		LinkedListNode<GameObject> linkedListNode = scrollController.CellsInUse.First;
		while (linkedListNode.Next != null)
		{
			MenuBarButton component = linkedListNode.Value.GetComponent<MenuBarButton>();
			if (component != null)
			{
				OpenCloseButtonImmediate(component, state);
			}
			linkedListNode = linkedListNode.Next;
		}
	}

	private void OpenCloseButtonImmediate(MenuBarButton menuButton, bool isOpen)
	{
		float xPosition = ((!isOpen) ? buttonClosedX : buttonOpenedX);
		if ((bool)menuButton)
		{
			menuButton.itemTransform.SetLocalXPosition(xPosition);
		}
	}

	private IEnumerator OpenCloseSequence(bool isOpen)
	{
		overlaySprite.gameObject.SetActive(isOpen);
		FadeOverlay(isOpen, 1f);
		Extensions.TweenLocalXPosition(newLocalXPosition: (!isOpen) ? closedY : openY, transform: base.transform, duration: timeToOpenMenu);
		if (isOpen)
		{
			if ((bool)selectedItemSprite)
			{
				selectedItemSprite.transform.localPosition = new Vector3(0f, -500f, 0f);
			}
			StartCoroutine(OpenMenuButtons(timeBetweenButtons));
		}
		else
		{
			StartCoroutine(CloseMenuButtons(0f));
		}
		yield return new WaitForSeconds(0.5f);
		if (!isOpen)
		{
			base.gameObject.SetActive(false);
		}
	}

	private void OpenCloseButton(MenuBarButton menuButton, bool isOpen)
	{
		float newLocalXPosition = ((!isOpen) ? buttonClosedX : buttonOpenedX);
		if ((bool)menuButton)
		{
			menuButton.itemTransform.TweenLocalXPosition(newLocalXPosition, timeToOpenButton);
		}
	}

	private IEnumerator HighlightButton(Transform itemTransform, int repetitions, float timeBetweenRepetitions)
	{
		if ((bool)selectedItemSprite)
		{
			selectedItemSprite.transform.position = itemTransform.position;
		}
		for (int i = 0; i < repetitions; i++)
		{
			if ((bool)selectedItemSprite)
			{
				selectedItemSprite.gameObject.SetActive(true);
				yield return new WaitForSeconds(timeBetweenRepetitions * 0.5f);
				selectedItemSprite.gameObject.SetActive(false);
				yield return new WaitForSeconds(timeBetweenRepetitions * 0.5f);
			}
		}
		selectedItemSprite.gameObject.SetActive(true);
	}

	public IEnumerator OpenMenuButtons(float timeBetweenButtons)
	{
		LinkedListNode<GameObject> currentNode = scrollController.CellsInUse.First;
		while (currentNode.Next != null)
		{
			MenuBarButton temp = currentNode.Value.GetComponent<MenuBarButton>();
			if (temp != null)
			{
				OpenCloseButton(temp, true);
				yield return new WaitForSeconds(timeBetweenButtons);
			}
			currentNode = currentNode.Next;
		}
	}

	public IEnumerator CloseMenuButtons(float timeBetweenButtons, Transform selectedItem = null)
	{
		int index = -1;
		if ((bool)selectedItem)
		{
			StartCoroutine(HighlightButton(selectedItem, highlightRepetitions, timeBetweenRepetitions));
		}
		MenuBarButton stored = null;
		LinkedListNode<GameObject> currentNode = scrollController.CellsInUse.First;
		while (currentNode.Next != null)
		{
			MenuBarButton temp = currentNode.Value.GetComponent<MenuBarButton>();
			if (temp != null)
			{
				if (temp.itemTransform.parent != selectedItem)
				{
					OpenCloseButton(temp, false);
					if (timeBetweenButtons > 0f)
					{
						yield return new WaitForSeconds(timeBetweenButtons);
					}
				}
				else
				{
					stored = temp;
				}
			}
			currentNode = currentNode.Next;
		}
		if (stored != null)
		{
			yield return new WaitForSeconds(0.5f);
			OpenCloseButton(stored, false);
		}
	}

	public void SetButtonState(int index, bool state)
	{
		int num = 0;
		LinkedListNode<GameObject> linkedListNode = scrollController.CellsInUse.First;
		while (linkedListNode.Next != null)
		{
			MenuBarButton component = linkedListNode.Value.GetComponent<MenuBarButton>();
			if (component != null)
			{
				if (num == index)
				{
					SetButtonState(component, state);
					break;
				}
				num++;
			}
			linkedListNode = linkedListNode.Next;
		}
	}

	public void SetButtonState(MenuBarButton menuButton, bool state)
	{
		menuButton.SetButtonState(state);
	}

	public void FadeOverlay(bool state, float time)
	{
		float fromVal = ((!state) ? alphaOverlay : 0f);
		float toVal = ((!state) ? 0f : alphaOverlay);
		SimpleTween.Start(fromVal, toVal, time, delegate(float val)
		{
			overlaySprite.Alpha = val;
		});
	}

	public void SetSelectedItemMarker(SceneTransitionManager.Scene currentScene)
	{
		LinkedListNode<GameObject> linkedListNode = scrollController.CellsInUse.First;
		while (linkedListNode.Next != null)
		{
			MenuBarButton component = linkedListNode.Value.GetComponent<MenuBarButton>();
			if (component != null && currentScene == component.scene && (bool)selectedItemSprite)
			{
				selectedItemSprite.gameObject.SetActive(true);
				selectedItemSprite.transform.position = component.itemTransform.parent.position;
			}
			linkedListNode = linkedListNode.Next;
		}
	}

	public SceneTransitionManager.Scene GetSceneButtonByIndex(int index)
	{
		if (index < 0 || index >= sceneIndexes.Length)
		{
			return SceneTransitionManager.Scene._NULL;
		}
		return sceneIndexes[index];
	}
}
