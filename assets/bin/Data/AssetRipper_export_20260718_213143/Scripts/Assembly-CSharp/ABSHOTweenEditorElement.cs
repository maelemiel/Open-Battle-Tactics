using System.Collections.Generic;
using UnityEngine;

public class ABSHOTweenEditorElement : MonoBehaviour
{
	public List<HOTweenManager.HOTweenData> tweenDatas = new List<HOTweenManager.HOTweenData>();

	public float globalDelay;

	public float globalTimeScale = 1f;

	public int creationTime;

	protected bool destroyed;

	protected virtual void DoDestroy()
	{
		if (!destroyed)
		{
			destroyed = true;
			tweenDatas = null;
		}
	}

	public int TotEmptyTweens()
	{
		if (tweenDatas == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < tweenDatas.Count; i++)
		{
			if (tweenDatas[i].propDatas.Count == 0)
			{
				num++;
				continue;
			}
			foreach (HOTweenManager.HOPropData propData in tweenDatas[i].propDatas)
			{
				if (!propData.isActive)
				{
					continue;
				}
				goto IL_0094;
			}
			num++;
			IL_0094:;
		}
		return num;
	}

	public int TotReadyTweens()
	{
		if (tweenDatas == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < tweenDatas.Count; i++)
		{
			if (tweenDatas[i].propDatas.Count <= 0)
			{
				continue;
			}
			foreach (HOTweenManager.HOPropData propData in tweenDatas[i].propDatas)
			{
				if (propData.isActive)
				{
					num++;
					break;
				}
			}
		}
		return num;
	}
}
