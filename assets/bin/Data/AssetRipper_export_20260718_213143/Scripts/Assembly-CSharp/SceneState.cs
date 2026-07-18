using System;
using System.Collections;
using UnityEngine;

public class SceneState : MonoBehaviour
{
	protected object dataObject;

	protected Action<PostBattleRewardsStates, object> callback;

	protected bool isFinished;

	[SerializeField]
	protected tk2dTextMesh tapToContinueText;

	public virtual void InitSequence(object dataObject)
	{
		this.dataObject = dataObject;
		if ((bool)tapToContinueText)
		{
			tapToContinueText.Alpha = 0f;
		}
	}

	public virtual IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		yield break;
	}

	public virtual void SkipToEnd()
	{
		StopAllCoroutines();
	}

	public virtual IEnumerator EndSequence(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		isFinished = false;
		if ((bool)tapToContinueText)
		{
			tapToContinueText.Alpha = 0f;
			AudioTrigger.TapToContinue.Play();
		}
	}

	private void Update()
	{
		if (isFinished && (bool)tapToContinueText)
		{
			tapToContinueText.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, 1.5f));
		}
	}
}
