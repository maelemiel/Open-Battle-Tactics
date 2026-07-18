using UnityEngine;

[RequireComponent(typeof(tk2dSprite))]
public class EmoticonViewController : MonoBehaviour
{
	public EmoticonTypes emoticonType;

	private tk2dSprite emoticonSprite;

	private void Awake()
	{
		emoticonSprite = GetComponent<tk2dSprite>();
		SetEmoticonType(emoticonType);
	}

	public void SetEmoticonType(EmoticonTypes emoticonType)
	{
		this.emoticonType = emoticonType;
		UpdateEmoticonSprite();
	}

	[ContextMenu("Update sprite")]
	private void UpdateEmoticonSprite()
	{
		string sprite = HUDController.emoticonsData[emoticonType];
		if ((bool)emoticonSprite)
		{
			emoticonSprite.SetSprite(sprite);
		}
		else
		{
			GetComponent<tk2dSprite>().SetSprite(sprite);
		}
	}
}
