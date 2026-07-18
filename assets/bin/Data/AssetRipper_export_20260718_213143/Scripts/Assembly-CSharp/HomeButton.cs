using UnityEngine;

public class HomeButton : MonoBehaviour
{
	protected void Awake()
	{
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
	}
}
