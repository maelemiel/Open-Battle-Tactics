using System.Collections;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Demo/tk2dUIDemoController")]
public class tk2dUIDemoController : tk2dUIBaseDemoController
{
	private const float TIME_TO_COMPLETE_PROGRESS_BAR = 2f;

	public tk2dUIItem nextPage;

	public GameObject window1;

	public tk2dUIItem prevPage;

	public GameObject window2;

	public tk2dUIProgressBar progressBar;

	private float timeSincePageStart;

	private float progressBarChaseVelocity;

	public tk2dUIScrollbar slider;

	private GameObject currWindow;

	private void Awake()
	{
		ShowWindow(window1.transform);
		HideWindow(window2.transform);
	}

	private void OnEnable()
	{
		nextPage.OnClick += GoToPage2;
		prevPage.OnClick += GoToPage1;
	}

	private void OnDisable()
	{
		nextPage.OnClick -= GoToPage2;
		prevPage.OnClick -= GoToPage1;
	}

	private void GoToPage1()
	{
		timeSincePageStart = 0f;
		AnimateHideWindow(window2.transform);
		AnimateShowWindow(window1.transform);
		currWindow = window1;
	}

	private void GoToPage2()
	{
		timeSincePageStart = 0f;
		if (currWindow != window2)
		{
			progressBar.Value = 0f;
			currWindow = window2;
			StartCoroutine(MoveProgressBar());
		}
		AnimateHideWindow(window1.transform);
		AnimateShowWindow(window2.transform);
	}

	private IEnumerator MoveProgressBar()
	{
		while (currWindow == window2 && progressBar.Value < 1f)
		{
			progressBar.Value = timeSincePageStart / 2f;
			yield return null;
			timeSincePageStart += tk2dUITime.deltaTime;
		}
		while (currWindow == window2)
		{
			float smoothTime = 0.5f;
			progressBar.Value = Mathf.SmoothDamp(progressBar.Value, slider.Value, ref progressBarChaseVelocity, smoothTime, float.PositiveInfinity, tk2dUITime.deltaTime);
			yield return 0;
		}
	}
}
