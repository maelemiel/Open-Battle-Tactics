using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameLoopTitles : MonoBehaviour
{
	public enum Title
	{
		BattleLost = 0,
		BattleWon = 1,
		BossBattle = 2,
		Draw = 3,
		FinalRound = 4,
		OutOfMoves = 5,
		Round1 = 6,
		Round2 = 7,
		RoundLost = 8,
		RoundWon = 9,
		TimeUp = 10,
		Fight = 11
	}

	[SerializeField]
	private List<MixedAnimationPlayer> _titlesAnimations;

	private void Awake()
	{
		for (int i = 0; i < _titlesAnimations.Count; i++)
		{
		}
	}

	public void StartTitleAnimation(Title title, Action cbFinish)
	{
		_titlesAnimations[(int)title].gameObject.SetActive(true);
		_titlesAnimations[(int)title].StartAnimation(delegate
		{
			_titlesAnimations[(int)title].gameObject.SetActive(false);
			if (cbFinish != null)
			{
				cbFinish();
			}
		});
	}

	public IEnumerator StartTitleAnimationCoroutine(Title title)
	{
		bool finish = false;
		_titlesAnimations[(int)title].gameObject.SetActive(true);
		Title title2 = default(Title);
		_titlesAnimations[(int)title].StartAnimation(delegate
		{
			_titlesAnimations[(int)title2].gameObject.SetActive(false);
			finish = true;
		});
		while (!finish)
		{
			yield return 0;
		}
		yield return 1;
	}
}
