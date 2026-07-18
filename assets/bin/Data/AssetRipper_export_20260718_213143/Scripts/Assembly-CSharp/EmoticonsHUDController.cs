using System.Collections.Generic;
using UnityEngine;

public class EmoticonsHUDController : MonoBehaviour
{
	private const float VERTICAL_LETTERBOX_OFFSET = -155f;

	[SerializeField]
	private SpeechBubbleHUDController speechController;

	[SerializeField]
	private tk2dBaseSprite spriteMarker;

	[SerializeField]
	private ScrollButton emoticonButtonScrollComponent;

	private EmoticonTypes currentEmoticonType;

	private static Dictionary<EmoticonTypes, List<string>> fakeBubbleMessages;

	public void Init()
	{
		if (!spriteMarker)
		{
			Log.Error("Sprite marker not found on Emoticon HUD Controller", base.gameObject);
		}
		if ((bool)emoticonButtonScrollComponent)
		{
			emoticonButtonScrollComponent.gameObject.SetActive(false);
		}
		if (fakeBubbleMessages == null)
		{
			Dictionary<EmoticonTypes, List<string>> dictionary = new Dictionary<EmoticonTypes, List<string>>();
			dictionary.Add(EmoticonTypes.ANGRY, GetEmoticonLinesWithType(EmoticonTypes.ANGRY));
			dictionary.Add(EmoticonTypes.HAPPY, GetEmoticonLinesWithType(EmoticonTypes.HAPPY));
			dictionary.Add(EmoticonTypes.SAD, GetEmoticonLinesWithType(EmoticonTypes.SAD));
			fakeBubbleMessages = dictionary;
		}
	}

	private List<string> GetEmoticonLinesWithType(EmoticonTypes emoticonType)
	{
		string emoticonLinePrefix = emoticonType.GetEmoticonLinePrefix();
		List<LinesEmoticonsDataModel> all = LinesEmoticonsDataModel.GetAll();
		all = all.FindAll((LinesEmoticonsDataModel x) => x.emoticonType == (int)emoticonType);
		List<string> list = new List<string>();
		foreach (LinesEmoticonsDataModel item in all)
		{
			list.Add(item.emoticonKeyString.Localize(string.Empty));
		}
		if (list.Count <= 0)
		{
			Log.Warning("No Emoticon Lines of type: [" + emoticonType.ToString() + "] have been found in Metadata", base.gameObject);
		}
		return list;
	}

	public void ShowToggleButton()
	{
		if ((bool)emoticonButtonScrollComponent)
		{
			emoticonButtonScrollComponent.gameObject.SetActive(true);
		}
	}

	public void HideToggleButton()
	{
		if ((bool)emoticonButtonScrollComponent)
		{
			emoticonButtonScrollComponent.gameObject.SetActive(false);
		}
	}

	private void OnEmoticonButtonPressed(tk2dUIItem item)
	{
		EmoticonViewController componentInChildren = item.GetComponentInChildren<EmoticonViewController>();
		if ((bool)componentInChildren)
		{
			currentEmoticonType = componentInChildren.emoticonType;
			spriteMarker.transform.position = componentInChildren.transform.position + Vector3.forward;
			spriteMarker.gameObject.SetActive(true);
			if ((bool)emoticonButtonScrollComponent)
			{
				emoticonButtonScrollComponent.ToggleButton();
			}
		}
	}

	public void ResetToggleEmoticon()
	{
		spriteMarker.gameObject.SetActive(false);
		currentEmoticonType = EmoticonTypes.NONE;
	}

	public EmoticonTypes GetCurrentSelectedEmoticon()
	{
		return currentEmoticonType;
	}

	public void ShowSpeechBubble(EmoticonTypes emoticonType, Transform target)
	{
		if (emoticonType != EmoticonTypes.NONE && (bool)speechController)
		{
			speechController.ShowSpeechBubble(target, GetBubbleMessage(emoticonType));
		}
	}

	private string GetBubbleMessage(EmoticonTypes emoticonType)
	{
		string empty = string.Empty;
		if (!fakeBubbleMessages.ContainsKey(emoticonType))
		{
			return empty;
		}
		List<string> list = fakeBubbleMessages[emoticonType];
		return list[Random.Range(0, list.Count)];
	}
}
