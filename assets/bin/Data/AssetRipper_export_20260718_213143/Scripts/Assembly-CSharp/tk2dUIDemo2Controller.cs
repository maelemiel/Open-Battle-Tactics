using System.Collections;
using UnityEngine;

public class tk2dUIDemo2Controller : tk2dUIBaseDemoController
{
	public tk2dUILayout windowLayout;

	private Vector3[] rectMin = new Vector3[6]
	{
		Vector3.zero,
		new Vector3(-0.8f, -0.7f, 0f),
		new Vector3(-0.9f, -0.9f, 0f),
		new Vector3(-1f, -0.9f, 0f),
		new Vector3(-1f, -1f, 0f),
		Vector3.zero
	};

	private Vector3[] rectMax = new Vector3[6]
	{
		Vector3.one,
		new Vector3(0.8f, 0.7f, 0f),
		new Vector3(0.9f, 0.9f, 0f),
		new Vector3(0.6f, 0.7f, 0f),
		new Vector3(1f, 1f, 0f),
		Vector3.one
	};

	private int currRect;

	private bool allowButtonPress = true;

	private void Start()
	{
		rectMin[0] = windowLayout.GetMinBounds();
		rectMax[0] = windowLayout.GetMaxBounds();
	}

	private IEnumerator NextButtonPressed()
	{
		if (allowButtonPress)
		{
			allowButtonPress = false;
			currRect = (currRect + 1) % rectMin.Length;
			yield return StartCoroutine(coResizeLayout(min: rectMin[currRect], max: rectMax[currRect], layout: windowLayout, time: 0.15f));
			allowButtonPress = true;
		}
	}

	private void LateUpdate()
	{
		int num = rectMin.Length - 1;
		rectMin[num].Set(tk2dCamera.Instance.ScreenExtents.xMin, tk2dCamera.Instance.ScreenExtents.yMin, 0f);
		rectMax[num].Set(tk2dCamera.Instance.ScreenExtents.xMax, tk2dCamera.Instance.ScreenExtents.yMax, 0f);
	}
}
