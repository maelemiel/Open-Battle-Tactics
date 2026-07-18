using UnityEngine;

public class SceneModel
{
	public SceneTransitionManager.Scene _scene;

	public SceneModel _previous;

	public SceneController controller;

	public string title;

	public float scrollPosition = float.NaN;

	public object payload;

	public SceneModel()
	{
	}

	public SceneModel(object payload)
	{
		this.payload = payload;
	}

	public void RestoreScrollPosition(Transform table)
	{
		if (!float.IsNaN(scrollPosition))
		{
			table.localPosition = new Vector3(table.localPosition.x, scrollPosition, table.localPosition.z);
			scrollPosition = float.NaN;
		}
	}

	public void SaveScrollPosition(Transform table)
	{
		SceneTransitionManager.TransitionOutBegin += delegate
		{
			scrollPosition = table.localPosition.y;
		};
	}

	public void SetupScrollPosition(Transform table)
	{
		RestoreScrollPosition(table);
		SaveScrollPosition(table);
	}
}
