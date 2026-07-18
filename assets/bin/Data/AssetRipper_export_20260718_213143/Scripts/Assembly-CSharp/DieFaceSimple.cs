using System.Collections;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class DieFaceSimple : MonoBehaviour
{
	private const float _DieFaceWidth = 36f;

	private const float _DieFaceHeight_closed = 12f;

	private const float _DieFaceHeight_opened = 30f;

	[SerializeField]
	private tk2dSlicedSprite _die;

	[SerializeField]
	private tk2dTextMesh _value;

	[SerializeField]
	private tk2dTextMesh _plusNumber;

	private int _rollValue;

	private DieFaceType _rollAction;

	private GameObject _activationEffect;

	private Sequence animateLabelSequence;

	private IHOTweenComponent anim1;

	private IHOTweenComponent anim2;

	private ABSTweenComponent openCloseSequence;

	private bool plusTextActive;

	private EventUnitBoostDataModel unitBoost;

	private Vector3 origLocalPos;

	private Vector3 originalScale;

	public Vector3 origExpandPos;

	public int RollValue
	{
		get
		{
			return _rollValue;
		}
		set
		{
			_rollValue = value;
		}
	}

	public DieFaceType RollAction
	{
		get
		{
			return _rollAction;
		}
		set
		{
			_rollAction = value;
		}
	}

	public tk2dSlicedSprite DieFace
	{
		get
		{
			return _die;
		}
	}

	private void Start()
	{
		originalScale = base.transform.localScale;
		origLocalPos = base.transform.localPosition;
	}

	private void UpdateFaceSprite(DieFaceType action, bool open, bool ignoreBoost = false)
	{
		string text = "DieFace_" + (int)action;
		if (action == DieFaceType.Special && !open)
		{
			text += "_NoStar";
		}
		if (!ignoreBoost && unitBoost != null)
		{
			if (action == DieFaceType.ArmourPiercing && unitBoost.dieBoostArmourPiercing > 0)
			{
				text += "G";
			}
			else if (action == DieFaceType.AcidStrike && unitBoost.dieBoostAcidStrike > 0)
			{
				text += "G";
			}
			else if (action == DieFaceType.DirectDamage && unitBoost.dieBoostDamage > 0)
			{
				text += "G";
			}
			else if (action == DieFaceType.Initiative && unitBoost.dieBoostInitiative > 0)
			{
				text += "G";
			}
			else if (action == DieFaceType.Special && (unitBoost.ability1BoostA > 0 || unitBoost.ability1BoostB > 0))
			{
				text += "G";
			}
		}
		_die.SetSprite(text);
	}

	public void UpdateTexture(DieFaceType rollAction, int rollValue, bool ignoreBoost = false)
	{
		RollAction = rollAction;
		RollValue = rollValue;
		UpdateFaceSprite(rollAction, true, ignoreBoost);
		_value.text = ((RollAction != DieFaceType.Special) ? _rollValue.ToString() : string.Empty);
		_value.Commit();
	}

	public void HideLabel()
	{
		_value.gameObject.SetActive(false);
		if (plusTextActive)
		{
			_plusNumber.gameObject.SetActive(false);
		}
	}

	public void ShowLabel()
	{
		_value.gameObject.SetActive(true);
		if (plusTextActive)
		{
			_plusNumber.gameObject.SetActive(true);
		}
	}

	public void SetBoost(EventUnitBoostDataModel unitBoost)
	{
		this.unitBoost = unitBoost;
	}

	public bool IsOpen()
	{
		return _die.dimensions.y >= 30f;
	}

	public bool IsClosed()
	{
		return _die.dimensions.y <= 12f;
	}

	public void OpenFace(float time = 0f)
	{
		StopAllCoroutines();
		if (openCloseSequence != null)
		{
			openCloseSequence.Complete();
			openCloseSequence = null;
		}
		UpdateFaceSprite(RollAction, true);
		ShowLabel();
		if (!IsOpen())
		{
			SimpleTween.Start(0f, 1f, time, delegate(float val)
			{
				float y = Mathf.Lerp(12f, 30f, val);
				_die.dimensions = new Vector2(36f, y);
			});
		}
	}

	public void CloseFace(float time = 0f)
	{
		StopAllCoroutines();
		if (openCloseSequence != null)
		{
			openCloseSequence.Complete();
			openCloseSequence = null;
		}
		UpdateFaceSprite(RollAction, false);
		HideLabel();
		if (!IsClosed())
		{
			SimpleTween.Start(0f, 1f, time, delegate(float val)
			{
				float y = Mathf.Lerp(30f, 12f, val);
				_die.dimensions = new Vector2(36f, y);
			});
			Vector3 localPosition = base.transform.localPosition;
			localPosition.z = origLocalPos.z;
			base.transform.localPosition = localPosition;
			HOTween.To(base.transform, time, new TweenParms().Prop("localScale", Vector3.one));
		}
	}

	public void OpenAndCloseFace(float time = 0f)
	{
		_die.dimensions = new Vector2(36f, 45f);
		base.transform.localScale = originalScale;
		openCloseSequence = SimpleTween.Start(0f, 1f, 0.25f, 0.03f, EaseType.EaseOutSine, delegate(float val)
		{
			float y = Mathf.Lerp(45f, 12f, val);
			_die.dimensions = new Vector2(36f, y);
		});
	}

	public void ResetActivationEffect()
	{
		if ((bool)_activationEffect)
		{
			Object.Destroy(_activationEffect);
			_activationEffect = null;
		}
	}

	public void ResetPlusText()
	{
		_plusNumber.gameObject.SetActive(false);
		_plusNumber.text = string.Empty;
		_plusNumber.Commit();
		plusTextActive = false;
	}

	public void ResetPlusText(float time)
	{
		StartCoroutine(ResetPlusTextDelay(time));
	}

	public IEnumerator ResetPlusTextDelay(float time)
	{
		yield return new WaitForSeconds(time);
		ResetPlusText();
	}

	public void UpdatePlusText(int buffValue)
	{
		if ((bool)_plusNumber)
		{
			if (buffValue > 0)
			{
				_plusNumber.gameObject.SetActive(_value.gameObject.activeSelf);
				_plusNumber.text = "+" + buffValue;
				plusTextActive = true;
			}
			else
			{
				ResetPlusText();
			}
		}
	}

	public void ExpandContract(float scaleAmount = 1.2f)
	{
		anim1 = HOTween.To(_die.transform, 0.6f, new TweenParms().Ease(EaseType.EaseOutBack, 0.4f).Prop("localScale", Vector3.one * scaleAmount).Prop("localPosition", new Vector3(_die.transform.localPosition.x, _die.transform.localPosition.y - 32f, _die.transform.localPosition.z - 15f)));
		anim2 = HOTween.To(_die.transform, 0.4f, new TweenParms().Ease(EaseType.EaseOutBack).Prop("localScale", Vector3.one).Prop("localPosition", _die.transform.localPosition)
			.OnComplete(ShowLabel));
		animateLabelSequence = new Sequence();
		animateLabelSequence.Append(anim1);
		animateLabelSequence.Append(anim2);
		animateLabelSequence.Play();
	}
}
