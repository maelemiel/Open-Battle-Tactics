using System.Collections;
using UnityEngine;

public class ChartBarController : MonoBehaviour
{
	private ChartBarView[] chartBars;

	private int barCount;

	private int currentLevel;

	public int BarCount
	{
		get
		{
			return barCount;
		}
	}

	private void Awake()
	{
		chartBars = GetComponentsInChildren<ChartBarView>();
		if (chartBars != null)
		{
			barCount = chartBars.Length;
		}
	}

	public void SetAllBarsToLevel(int level)
	{
		for (int i = 0; i < chartBars.Length; i++)
		{
			chartBars[i].SetBarLevel(level);
		}
	}

	public void SetAllBarsToRandomLevel(int maximumLevel)
	{
		for (int i = 0; i < chartBars.Length; i++)
		{
			chartBars[i].SetBarLevel(Random.Range(0, maximumLevel + 1));
		}
	}

	public void SetChartFromMinToMaxInTime(float timeToActivate)
	{
		StopAllCoroutines();
		StartCoroutine(SetAllBarsBetweenLevelsWithTime(0, barCount, timeToActivate));
	}

	public void SetChartFromMaxToMinInTime(float timeToActivate)
	{
		StopAllCoroutines();
		StartCoroutine(SetAllBarsBetweenLevelsWithTime(barCount, 0, timeToActivate));
	}

	public IEnumerator SetAllBarsBetweenLevelsWithTime(int minimumLevel, int maximumLevel, float timeToDeactivateBars)
	{
		if (timeToDeactivateBars <= 0f)
		{
			SetAllBarsToLevel(maximumLevel);
			yield break;
		}
		float elapsedTime = 0f;
		StartCoroutine(SetAllBarsToRandomLevelRepeat(0.02f));
		while (elapsedTime <= timeToDeactivateBars)
		{
			currentLevel = (int)Mathf.Lerp(minimumLevel, maximumLevel, elapsedTime);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		SetAllBarsToLevel(maximumLevel);
		StopAllCoroutines();
		currentLevel = 0;
	}

	public IEnumerator SetAllBarsToRandomLevelRepeat(float timePeriod)
	{
		while (true)
		{
			SetAllBarsToRandomLevel(currentLevel);
			yield return new WaitForSeconds(timePeriod);
		}
	}

	public void SetAllBarsToMax()
	{
		SetAllBarsToLevel(barCount);
	}

	public void SetAllBarsToMin()
	{
		SetAllBarsToLevel(0);
	}
}
