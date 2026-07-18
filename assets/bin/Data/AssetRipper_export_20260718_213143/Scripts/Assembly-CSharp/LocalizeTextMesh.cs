using UnityEngine;

public class LocalizeTextMesh : MonoBehaviour
{
	[SerializeField]
	private string textKey;

	public string TextKey
	{
		get
		{
			return textKey;
		}
		set
		{
			textKey = value;
			Localize();
		}
	}

	private void OnEnable()
	{
		if (Singleton<LocalizationManager>.instance.IsInitialized())
		{
			Localize();
		}
		else
		{
			Singleton<LocalizationManager>.instance.OnInitializeListener += Localize;
		}
	}

	private void OnDisable()
	{
		Singleton<LocalizationManager>.instance.OnInitializeListener -= Localize;
	}

	private void Localize()
	{
		tk2dTextMesh component = GetComponent<tk2dTextMesh>();
		string text = component.text;
		component.text = textKey.Localize(text);
		Singleton<LocalizationManager>.instance.OnInitializeListener -= Localize;
	}
}
