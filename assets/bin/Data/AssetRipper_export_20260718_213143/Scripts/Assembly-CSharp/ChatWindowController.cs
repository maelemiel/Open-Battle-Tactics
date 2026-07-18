using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatWindowController : MonoBehaviour
{
	private const int MAX_MESSAGES = 30;

	[SerializeField]
	private tk2dUIItem sendButton;

	[SerializeField]
	private tk2dUITextInput inputText;

	[SerializeField]
	private ScrollableAreaController chatScrollableArea;

	[SerializeField]
	private float openY;

	[SerializeField]
	private float closedY;

	[SerializeField]
	private GameObject loadingIcon;

	[SerializeField]
	private tk2dUIToggleButtonGroup chatButtons;

	[SerializeField]
	private GameObject globalChatButton;

	[SerializeField]
	private GameObject clubChatButton;

	[SerializeField]
	private GameObject disabledClubChatButton;

	[SerializeField]
	private GameObject clubChatNotificationGreen;

	private Collider globalChatCollider;

	private Collider clubChatCollider;

	private ChatData globalChatData;

	private ChatData clubChatData;

	private ChatData currentChatData;

	private string playerClubID;

	private bool isClubChat;

	private bool fetching;

	private int lastClubChat = -1;

	private bool _isOpen;

	private bool Fetching
	{
		get
		{
			return fetching;
		}
		set
		{
			fetching = value;
			if ((bool)globalChatCollider)
			{
				globalChatCollider.enabled = !value;
			}
			if ((bool)clubChatCollider)
			{
				clubChatCollider.enabled = !value;
			}
		}
	}

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

	public event Action OnChatOpened;

	public event Action OnChatClosed;

	public event Action GoToFBCommunity;

	private void Awake()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, closedY, base.transform.localPosition.z);
		globalChatData = new ChatData();
		clubChatData = new ChatData();
		if ((bool)globalChatButton)
		{
			globalChatButton.SetActive(true);
		}
		if ((bool)globalChatButton)
		{
			globalChatCollider = globalChatButton.collider;
		}
		if ((bool)clubChatButton)
		{
			clubChatCollider = clubChatButton.collider;
		}
	}

	private void OnDestroy()
	{
		UserProfile.player.OnClubChatChanged -= UpdateNotifications;
		if (lastClubChat >= 0)
		{
			Singleton<SessionManager>.instance.SetChatMessageViewed(lastClubChat, true, delegate
			{
			});
		}
	}

	private void OnEnable()
	{
		if ((bool)loadingIcon)
		{
			loadingIcon.gameObject.SetActive(true);
		}
		if (currentChatData == null)
		{
			currentChatData = globalChatData;
		}
		chatScrollableArea.DataSource = currentChatData.chatMessages;
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	public void Init(string userClubID)
	{
		Log.DebugTag("Initing again " + userClubID, null, "Chat window");
		playerClubID = userClubID;
		bool flag = !string.IsNullOrEmpty(userClubID);
		if ((bool)clubChatButton)
		{
			clubChatButton.SetActive(flag);
		}
		if ((bool)disabledClubChatButton)
		{
			disabledClubChatButton.SetActive(!flag);
		}
		if (flag)
		{
			UpdateClubChat();
			if (!UserProfile.player.HasSeenLastClubMessage)
			{
				OnClickClub();
				chatButtons.SelectedIndex = 1;
			}
			else
			{
				UpdateNotifications();
			}
			UserProfile.player.OnClubChatChanged += UpdateNotifications;
		}
		else
		{
			UpdateNotifications();
		}
	}

	private void UpdateClubChat()
	{
		if (!isClubChat && clubChatData != null && UserProfile.player.IsClubMember)
		{
			Singleton<SessionManager>.instance.GetChatMessage(clubChatData.lastChatSequence, true, delegate(List<ChatMessage> newMessages, string lastSequence, bool success)
			{
				if (!success)
				{
					CancelInvoke("UpdateClubChat");
					Init(string.Empty);
				}
				else
				{
					UpdateNotifications();
				}
			});
			Invoke("UpdateClubChat", Constants.PopupClubChatRefresh);
		}
		else if (!UserProfile.player.IsClubMember)
		{
			Init(string.Empty);
		}
	}

	private void UpdateNotifications()
	{
		if (!(this == null) && UserProfile.player != null)
		{
			if (UserProfile.player.HasSeenLastClubMessage || isClubChat)
			{
				clubChatNotificationGreen.SetActive(false);
			}
			else
			{
				clubChatNotificationGreen.SetActive(true);
			}
		}
	}

	private IEnumerator FetchMessageSequence()
	{
		while (true)
		{
			Fetching = true;
			Singleton<SessionManager>.instance.GetChatMessage(currentChatData.lastChatSequence, isClubChat, delegate(List<ChatMessage> newMessages, string lastSequence, bool success)
			{
				if ((bool)this)
				{
					if (!success)
					{
						Init(string.Empty);
					}
					else
					{
						currentChatData.lastChatSequence = lastSequence;
						if (isClubChat)
						{
							lastClubChat = int.Parse(lastSequence);
							UserProfile.player.LastClubMsg = lastClubChat;
							clubChatNotificationGreen.SetActive(false);
						}
						Fetching = false;
						if (newMessages != null && newMessages.Count > 0)
						{
							bool flag = false;
							foreach (ChatMessage newMessage in newMessages)
							{
								foreach (ChatMessage chatMessage in currentChatData.chatMessages)
								{
									if (chatMessage.id == newMessage.id)
									{
										flag = true;
									}
								}
								if (!flag)
								{
									currentChatData.chatMessages.Insert(0, newMessage);
								}
							}
							while (currentChatData.chatMessages.Count > 30)
							{
								currentChatData.chatMessages.RemoveAt(currentChatData.chatMessages.Count - 1);
							}
							float contentPosition = chatScrollableArea.GetContentPosition();
							chatScrollableArea.OnDataChanged();
							if (contentPosition != 0f)
							{
								chatScrollableArea.ContentToPosition(0f - (contentPosition + chatScrollableArea.cellHeight));
							}
						}
					}
				}
			});
			while (fetching)
			{
				yield return 0;
			}
			currentChatData.lastFetchTime = Time.time;
			chatScrollableArea.Refresh();
			if ((bool)loadingIcon)
			{
				loadingIcon.gameObject.SetActive(false);
			}
			while (Time.time - currentChatData.lastFetchTime < 2f)
			{
				yield return 0;
			}
		}
	}

	public void SetOpen(bool isOpen)
	{
		base.gameObject.SetActive(true);
		if (!_isOpen && isOpen)
		{
			chatScrollableArea.Refresh();
		}
		StopAllCoroutines();
		StartCoroutine(OpenCloseSequence(isOpen));
		if (isOpen)
		{
			StartCoroutine(FetchMessageSequence());
		}
	}

	private IEnumerator OpenCloseSequence(bool isOpen)
	{
		Extensions.TweenLocalYPosition(newLocalYPosition: (!isOpen) ? closedY : openY, transform: base.transform, duration: 0.5f);
		yield return new WaitForSeconds(0.5f);
		if (isOpen)
		{
			if (this.OnChatOpened != null)
			{
				this.OnChatOpened();
			}
		}
		else if (this.OnChatClosed != null)
		{
			this.OnChatClosed();
		}
	}

	public void OnClickSend()
	{
		string text = inputText.Text;
		if (!string.IsNullOrEmpty(text))
		{
			SendChatMessage(text);
			inputText.Text = string.Empty;
		}
	}

	private void SendChatMessage(string message)
	{
		Singleton<SessionManager>.instance.SetChatMessage(message, isClubChat, delegate
		{
		});
		currentChatData.lastFetchTime = 0f;
	}

	private void OnClickGlobal()
	{
		if (currentChatData != globalChatData)
		{
			StopAllCoroutines();
			if ((bool)loadingIcon)
			{
				loadingIcon.gameObject.SetActive(true);
			}
			currentChatData = globalChatData;
			isClubChat = false;
			StartCoroutine(FetchMessageSequence());
			chatScrollableArea.DataSource = currentChatData.chatMessages;
			chatScrollableArea.Refresh();
		}
	}

	public void OnClickClub()
	{
		if (currentChatData != clubChatData)
		{
			StopAllCoroutines();
			if ((bool)loadingIcon)
			{
				loadingIcon.gameObject.SetActive(true);
			}
			currentChatData = clubChatData;
			isClubChat = true;
			StartCoroutine(FetchMessageSequence());
			chatScrollableArea.DataSource = currentChatData.chatMessages;
			chatScrollableArea.Refresh();
		}
	}

	public void OnClickCommunity()
	{
		PopupManager.ShowPopup(PopupDataModel.CancelCustom(Singleton<LocalizationManager>.instance.Get("ui_community_title"), Singleton<LocalizationManager>.instance.Get("ui_community_body"), Singleton<LocalizationManager>.instance.Get("ui_community_button"), delegate
		{
			OpenCommunityPage();
		}));
	}

	private void OpenCommunityPage()
	{
		Application.OpenURL("https://www.facebook.com/SuperBattleTactics");
	}
}
