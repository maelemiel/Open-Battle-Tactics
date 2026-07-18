using UnityEngine;

public class PrefabInflator : MonoBehaviour
{
	private string resourcesStr = "Resources/";

	private T GetObjectFromResources<T>(string path) where T : class
	{
		string text = path.Substring(path.LastIndexOf(resourcesStr) + resourcesStr.Length);
		text = text.Substring(0, text.LastIndexOf('.'));
		return Resources.Load(text) as T;
	}

	public void Inflate(GameObject instance)
	{
		PlayButtonSound[] componentsInChildren = instance.GetComponentsInChildren<PlayButtonSound>(true);
		foreach (PlayButtonSound playButtonSound in componentsInChildren)
		{
			PrefabSound component = playButtonSound.GetComponent<PrefabSound>();
			if (!(component == null))
			{
				string savedName = component.savedName;
				Object.Destroy(component);
				playButtonSound.pressSound = GetObjectFromResources<Object>(savedName) as AudioClip;
			}
		}
	}
}
