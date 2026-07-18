using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UICounter : MonoBehaviour
{
	private const string _digitPrefix = "BattleDigit_";

	private const string _digitAnim = "BattleDigit_Focus";

	[SerializeField]
	private List<tk2dSprite> _digits;

	private int _lastCount = -1;

	public int Value
	{
		get
		{
			return _lastCount;
		}
	}

	public void Init(int count)
	{
		SetCount(count);
	}

	public void Increment()
	{
		if (_lastCount < 0)
		{
			Debug.LogError("Counter has bad value " + _lastCount + ". Did you call Init?");
		}
		SetCount(_lastCount + 1);
	}

	public void Decrement()
	{
		if (_lastCount < 0)
		{
			Debug.LogError("Counter has bad value " + _lastCount + ". Did you call Init?");
		}
		SetCount(_lastCount - 1);
	}

	public void SetCount(int count)
	{
		if (_lastCount != count)
		{
			Debug.Log("Setting count to " + count + " and lastCount was " + _lastCount);
			if (count < 0)
			{
				count = 0;
			}
			else if ((float)count >= Mathf.Pow(10f, _digits.Count))
			{
				count = (int)(Mathf.Pow(10f, _digits.Count) - 1f);
			}
			_lastCount = count;
			char[] array = count.ToString().ToCharArray();
			for (int i = 0; i < _digits.Count; i++)
			{
				_digits[i].gameObject.SetActive(i < array.Length);
			}
			for (int j = 0; j < array.Length; j++)
			{
				_digits[j].spriteId = _digits[j].GetSpriteIdByName("BattleDigit_" + array[array.Length - 1 - j]);
				_digits[j].gameObject.animation.Play("BattleDigit_Focus");
			}
		}
	}
}
