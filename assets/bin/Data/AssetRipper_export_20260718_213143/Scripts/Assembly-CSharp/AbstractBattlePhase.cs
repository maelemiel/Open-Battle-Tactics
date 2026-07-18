using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractBattlePhase
{
	public BattleController battleController;

	public float timeInPhase;

	public Phase phaseType;

	public virtual void OnEnterPhase()
	{
	}

	public virtual void OnExitPhase()
	{
	}

	public virtual void OnUpdate()
	{
	}

	protected Coroutine StartCoroutine(IEnumerator coroutine)
	{
		return battleController.StartCoroutine(coroutine);
	}

	protected IEnumerator PlaySequences(List<Func<IEnumerator>> list)
	{
		foreach (Func<IEnumerator> func in list)
		{
			yield return StartCoroutine(func());
		}
	}
}
