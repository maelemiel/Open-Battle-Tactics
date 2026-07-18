using Holoville.HOTween;
using UnityEngine;

public class UIComboFlyUp : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _digit0;

	[SerializeField]
	private tk2dSprite _digit1;

	[SerializeField]
	private tk2dSprite _digit2;

	[SerializeField]
	private tk2dSprite _combo;

	private float _staticTime = 0.25f;

	private float _fadeOutTime = 0.5f;

	private Vector3 _deltaUpPosition = new Vector3(0f, 50f, 0f);

	private void Awake()
	{
		_digit0.gameObject.SetActive(false);
		_digit1.gameObject.SetActive(false);
		_digit2.gameObject.SetActive(false);
		_combo.gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.A))
		{
			Play(new Vector3(0f, 0f, 0f), 23);
		}
	}

	public void SetCombo(int combo)
	{
		if (combo < 10)
		{
			_digit0.gameObject.SetActive(true);
			_digit1.gameObject.SetActive(false);
			_digit2.gameObject.SetActive(false);
			_digit0.spriteId = _digit0.GetSpriteIdByName("BattleDigit_" + combo);
		}
		else
		{
			_digit0.gameObject.SetActive(false);
			_digit1.gameObject.SetActive(true);
			_digit2.gameObject.SetActive(true);
			_digit1.spriteId = _digit0.GetSpriteIdByName("BattleDigit_" + combo.ToString()[0]);
			_digit2.spriteId = _digit0.GetSpriteIdByName("BattleDigit_" + combo.ToString()[1]);
		}
	}

	public void Play(Vector3 position, int combo)
	{
		_digit0.Alpha = 1f;
		_digit1.Alpha = 1f;
		_digit2.Alpha = 1f;
		_combo.Alpha = 1f;
		_combo.gameObject.SetActive(true);
		base.gameObject.transform.localPosition = position;
		SetCombo(combo);
		FlyUpSequence().Play();
	}

	private Sequence FlyUpSequence()
	{
		Vector3 vector = base.gameObject.transform.localPosition + _deltaUpPosition;
		SequenceParms p_parms = new SequenceParms().OnComplete(FlyUpCompleteCallBack);
		Sequence sequence = new Sequence(p_parms);
		sequence.Insert(_staticTime, HOTween.To(base.gameObject.transform, _fadeOutTime, "localPosition", vector));
		sequence.Insert(_staticTime, HOTween.To(_digit0, _fadeOutTime, "Alpha", 0));
		sequence.Insert(_staticTime, HOTween.To(_digit1, _fadeOutTime, "Alpha", 0));
		sequence.Insert(_staticTime, HOTween.To(_digit2, _fadeOutTime, "Alpha", 0));
		sequence.Insert(_staticTime, HOTween.To(_combo, _fadeOutTime, "Alpha", 0));
		return sequence;
	}

	private void FlyUpCompleteCallBack()
	{
		Object.Destroy(base.gameObject);
	}
}
