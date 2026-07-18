using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UIScore : MonoBehaviour
{
	private const string _digitPrefix = "BattleDigit_";

	private const string _digitAnim = "BattleDigit_Focus";

	[SerializeField]
	private List<tk2dSprite> _digits;

	[SerializeField]
	private List<tk2dSprite> _commas;

	[SerializeField]
	private int _score;

	private int _lastScore = -1;

	private void Awake()
	{
		SetScore(0);
	}

	public void SetScore(int score)
	{
		if (_lastScore != score)
		{
			if (score < 0)
			{
				score = 0;
			}
			else if ((float)score >= Mathf.Pow(10f, _digits.Count))
			{
				score = (int)(Mathf.Pow(10f, _digits.Count) - 1f);
			}
			_lastScore = score;
			char[] array = score.ToString().ToCharArray();
			int num = Mathf.FloorToInt((array.Length - 1) / 3);
			for (int i = 0; i < _commas.Count; i++)
			{
				_commas[i].gameObject.SetActive(i < num);
			}
			for (int j = 0; j < _digits.Count; j++)
			{
				_digits[j].gameObject.SetActive(j < array.Length);
			}
			for (int k = 0; k < array.Length; k++)
			{
				_digits[k].spriteId = _digits[k].GetSpriteIdByName("BattleDigit_" + array[array.Length - 1 - k]);
				_digits[k].gameObject.animation.Play("BattleDigit_Focus");
			}
		}
	}
}
