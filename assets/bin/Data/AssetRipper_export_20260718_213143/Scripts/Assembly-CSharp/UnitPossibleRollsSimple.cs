using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class UnitPossibleRollsSimple : MonoBehaviour
{
	private enum State
	{
		Open = 1,
		Close = 2,
		OnSpin = 3
	}

	private const string ANIMATION_NAME_MOTION = "Spinner Motion Blur Speed lines";

	private const string ANIMATION_NAME_NORMAL_FACE = "White Particles";

	private const string ANIMATION_NAME_BEST_FACE = "Color Particles ";

	private const float _DieBoxWidth = 192f;

	private const float _DieBoxHeight_closed = 15f;

	private const float _DieBoxHeight_opened = 32f;

	private const float _DieFaceGapWidth = 20f;

	private const int VALUE_THRESHOLD_DAMAGE_ROLL = 0;

	private const int VALUE_THRESHOLD_FIRST_STRIKE_ROLL = 0;

	private const string ANIMATION_NAME_DAMAGE_DIEFACE = "Attack Effect";

	private const string ANIMATION_NAME_FIRST_STRIKE_DIEFACE = "First Strike Effect";

	private const string ANIMATION_NAME_SPECIAL_FACE = "Special Effect";

	private const float _kEXPANDEDSCALEVALUE = 1.5f;

	public const float TIME_OPEN_DIEBOX = 0.3f;

	public const float TIME_CLOSE_DIEBOX = 0.3f;

	[SerializeField]
	private tk2dSlicedSprite _dieFaceBox;

	[SerializeField]
	private DieFaceSimple[] _dieFaces;

	[SerializeField]
	private UnitView _unit;

	[SerializeField]
	private tk2dSpineAnimation spinnerMotion;

	private float _DieFaceWidth;

	private Vector3 EFFECT_OFFSET = Vector3.forward;

	private Vector3 _origPos;

	private Vector3 _origScale;

	private bool _isSpinning;

	private bool _wasOpen;

	private bool _wasOpenDecision;

	public tk2dSprite[] _dieFaceSprites;

	private int previousRoll;

	private State _currentState;

	public DieFaceSimple ActiveFace
	{
		get
		{
			return _dieFaces[_unit.CurrentRoll];
		}
	}

	private void Awake()
	{
		ResetAllPlusNumber();
	}

	public void Init()
	{
		UpdateDieFaceTexture(_unit.DiceSides, _unit.DiceValues);
		Transform transform = _dieFaces[0].transform;
		_origPos = transform.localPosition;
		_origScale = Vector3.one;
		_DieFaceWidth = (_dieFaces[_dieFaces.Length - 1].transform.localPosition.x - _dieFaces[0].transform.localPosition.x) / (float)(_dieFaces.Length - 1);
		ResetCurrentSpin();
		if ((bool)spinnerMotion)
		{
			spinnerMotion.gameObject.SetActive(false);
			spinnerMotion.AnimationComplete += OnSpinnerAnimationComplete;
		}
		_currentState = State.Close;
	}

	public void SetVisible(bool currentState)
	{
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>(true);
		foreach (MeshRenderer meshRenderer in componentsInChildren)
		{
			meshRenderer.enabled = currentState;
		}
	}

	public void OpenDieHUD(float animTime = 0.5f)
	{
		OpenDieBox();
	}

	public bool DieBoxIsOpen()
	{
		return _dieFaceBox.dimensions.y >= 32f;
	}

	public bool DieBoxIsClosed()
	{
		return _dieFaceBox.dimensions.y <= 15f;
	}

	private void ResizeDieFaces(float time, bool open)
	{
		DieFaceSimple[] dieFaces = _dieFaces;
		foreach (DieFaceSimple dieFaceSimple in dieFaces)
		{
			if (open)
			{
				dieFaceSimple.OpenFace(time);
			}
			else if (ActiveFace != dieFaceSimple || _unit.isEnemy)
			{
				dieFaceSimple.CloseFace(time);
			}
		}
	}

	public void OpenDieBox(float time = 0.3f)
	{
		_currentState = State.Open;
		if (!DieBoxIsOpen())
		{
			if (_isSpinning)
			{
				return;
			}
			SimpleTween.Start(0f, 1f, time, delegate(float val)
			{
				float y = Mathf.Lerp(15f, 32f, val);
				_dieFaceBox.dimensions = new Vector2(192f, y);
			});
		}
		ResizeDieFaces(time, true);
	}

	public void CloseDieBox(float time = 0.3f, bool force = false)
	{
		_currentState = State.Close;
		if (!DieBoxIsClosed() || force)
		{
			if (_isSpinning)
			{
				return;
			}
			SimpleTween.Start(0f, 1f, time, delegate(float val)
			{
				float y = Mathf.Lerp(32f, 15f, val);
				_dieFaceBox.dimensions = new Vector2(192f, y);
			});
		}
		ResizeDieFaces(time, false);
	}

	public void SetDiceValue(int index, int value)
	{
		_dieFaces[index].UpdateTexture(_dieFaces[index].RollAction, value);
	}

	public void SetDiceValues(int[] values, bool ignoreBoost)
	{
		for (int i = 0; i < values.Length; i++)
		{
			_dieFaces[i].UpdateTexture(_dieFaces[i].RollAction, values[i], ignoreBoost);
		}
	}

	public void CloseInfoBox()
	{
		_dieFaceBox.dimensions = new Vector2(0f, _dieFaceBox.dimensions.y);
		DieFaceSimple[] dieFaces = _dieFaces;
		foreach (DieFaceSimple dieFaceSimple in dieFaces)
		{
			dieFaceSimple.transform.localPosition = new Vector3(10f, dieFaceSimple.transform.localPosition.y, dieFaceSimple.transform.localPosition.z);
		}
	}

	public void ToggleOpenClose()
	{
		if (!_isSpinning)
		{
			if (DieBoxIsOpen())
			{
				CloseDieBox();
			}
			else if (DieBoxIsClosed())
			{
				OpenDieBox();
			}
		}
	}

	public void ResetAllPlusNumber()
	{
		DieFaceSimple[] dieFaces = _dieFaces;
		foreach (DieFaceSimple dieFaceSimple in dieFaces)
		{
			dieFaceSimple.ResetPlusText();
		}
	}

	public IEnumerator StartSpinAnimation(int currentRoll, int numRevolutions)
	{
		float dieFaceTime = _unit.BattleController.tunables.diceEffectTunables.diceUITimeSpin;
		int finalIndex = _dieFaces.Length * numRevolutions + currentRoll;
		float animTime = dieFaceTime * (float)finalIndex;
		float spaceBetweenFaces = 40f;
		float barWidth = (float)_dieFaces.Length * spaceBetweenFaces;
		float totalWidth = (float)finalIndex * spaceBetweenFaces;
		float offsetX = -12f;
		float initialX = (float)previousRoll * spaceBetweenFaces;
		int checkIndex = -1;
		AudioManager.RepeatingSfx spinSound = null;
		totalWidth -= initialX;
		ResetCurrentSpin();
		yield return new WaitForEndOfFrame();
		if ((bool)spinnerMotion)
		{
			spinnerMotion.animationSpeed = 2.22f;
			spinnerMotion.gameObject.SetActive(true);
			spinnerMotion.AnimationName = "Spinner Motion Blur Speed lines";
			spinnerMotion.loop = true;
		}
		int currentRoll2 = default(int);
		SimpleTween.Start(0f, 1f, animTime, EaseType.Linear, delegate(float val)
		{
			int num = (int)Mathf.Lerp(previousRoll, finalIndex, val);
			int num2 = num % _dieFaces.Length;
			if ((bool)spinnerMotion)
			{
				spinnerMotion.transform.SetLocalXPosition(offsetX + (totalWidth * val + initialX) % barWidth);
			}
			if (checkIndex != num)
			{
				if ((bool)spinnerMotion && num % _dieFaces.Length == 0)
				{
					spinnerMotion.state.Time = 0f;
				}
				checkIndex = num;
				if (num == finalIndex)
				{
					ActivateDie(currentRoll2);
					_isSpinning = false;
					CreateDiceEffectOnFace(currentRoll2);
					previousRoll = currentRoll2;
					if (spinSound != null)
					{
						spinSound.Stop();
						spinSound = null;
					}
					AudioTrigger.DieFaceSpinEnd.Play();
					spinnerMotion.transform.position = _dieFaces[currentRoll2].transform.position;
					if (_currentState == State.Open)
					{
						OpenDieBox();
					}
				}
				else
				{
					if ((bool)_dieFaces[num2])
					{
						_dieFaces[num2].OpenAndCloseFace(dieFaceTime * (float)(_dieFaces.Length - 2));
						if (spinSound == null)
						{
							spinSound = AudioTrigger.DieFaceSpin.PlayRepeating();
						}
					}
					_isSpinning = true;
				}
			}
		});
		yield return new WaitForSeconds(animTime);
	}

	private void UpdateDieFaceTexture(DieFaceType[] diceActions, int[] diceValues)
	{
		int num = 0;
		DieFaceSimple[] dieFaces = _dieFaces;
		foreach (DieFaceSimple dieFaceSimple in dieFaces)
		{
			dieFaceSimple.UpdateTexture(diceActions[num], diceValues[num]);
			num++;
		}
	}

	private void ResetCurrentSpin()
	{
		DieFaceSimple[] dieFaces = _dieFaces;
		foreach (DieFaceSimple dieFaceSimple in dieFaces)
		{
			dieFaceSimple.CloseFace();
			dieFaceSimple.transform.localScale = _origScale;
			dieFaceSimple.transform.localPosition = new Vector3(dieFaceSimple.transform.localPosition.x, _origPos.y, _origPos.z);
		}
	}

	public void ResetDieFaces()
	{
		ResetAllPlusNumber();
		ResetCurrentSpin();
		CloseDieBox();
	}

	public void ActivateDie(int index, float animTime = 0.25f)
	{
		DieFaceSimple dieFaceSimple = _dieFaces[index];
		Vector3 vector = _origScale * 1.5f;
		dieFaceSimple.transform.localScale = vector;
		Vector3 localPosition = dieFaceSimple.transform.localPosition;
		HOTween.To(dieFaceSimple.transform, animTime, new TweenParms().Ease(EaseType.EaseOutBack).Prop("localScale", vector * 1.5f).Prop("localPosition", new Vector3(localPosition.x, localPosition.y, -30f)));
		HOTween.To(dieFaceSimple.transform, animTime * 0.5f, new TweenParms().Prop("localScale", vector).Prop("localPosition", new Vector3(localPosition.x, localPosition.y, -30f)).Delay(animTime)
			.Ease(EaseType.EaseInSine));
		dieFaceSimple.OpenFace();
	}

	public void ActivateActiveDie(int currentRoll = -1)
	{
		ActivateDie((currentRoll < 0) ? _unit.CurrentRoll : currentRoll);
	}

	public void ExpandActiveDieFace(float scaleFactor, float animTime)
	{
		ActiveFace.OpenFace();
		ActiveFace.origExpandPos = ActiveFace.transform.localPosition;
		Vector3 vector = Vector3.one * scaleFactor;
		HOTween.To(ActiveFace.transform, animTime, new TweenParms().Ease(EaseType.EaseOutSine).Prop("localScale", vector).Prop("localPosition", ActiveFace.origExpandPos + new Vector3(0f, 0f, -30f)));
	}

	public void RestoreActiveDieFace(float animTime)
	{
		ActiveFace.OpenFace();
		HOTween.To(ActiveFace.transform, animTime, new TweenParms().Ease(EaseType.Linear).Prop("localScale", _origScale * 1.5f).Prop("localPosition", ActiveFace.origExpandPos));
	}

	public void DeactivateActiveDie()
	{
		ActiveFace.transform.localScale = Vector3.one;
		if (DieBoxIsOpen())
		{
			ActiveFace.CloseFace();
		}
	}

	public void RememberState()
	{
		_wasOpen = DieBoxIsOpen();
	}

	public void RestoreRememberedState()
	{
		if (_wasOpen)
		{
			OpenDieBox();
		}
		else
		{
			CloseDieBox();
		}
	}

	public void RememberDecisionState()
	{
		_wasOpenDecision = DieBoxIsOpen();
	}

	public void RestoreDecisionState()
	{
		if (_wasOpenDecision)
		{
			OpenDieBox();
		}
		else
		{
			CloseDieBox();
		}
	}

	public EffectInstance CreateDiceEffectOnActiveFace(bool allowBestRoll = true, bool tweakScaleAndOpacity = true)
	{
		return CreateDiceEffectOnFace(_unit.CurrentRoll, allowBestRoll, tweakScaleAndOpacity);
	}

	public EffectInstance CreateDiceEffectOnFace(int index, bool allowBestRoll = true, bool tweakScaleAndOpacity = true)
	{
		DieFaceSimple dieFaceSimple = _dieFaces[index];
		string animationName = string.Empty;
		switch (dieFaceSimple.RollAction)
		{
		case DieFaceType.DirectDamage:
			if (dieFaceSimple.RollValue >= 0)
			{
				animationName = "Attack Effect";
			}
			break;
		case DieFaceType.Initiative:
			if (dieFaceSimple.RollValue >= 0)
			{
				animationName = "First Strike Effect";
			}
			break;
		case DieFaceType.ArmourPiercing:
			if (dieFaceSimple.RollValue >= 0)
			{
				animationName = "Attack Effect";
			}
			break;
		case DieFaceType.Special:
			animationName = "Special Effect";
			break;
		}
		EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.DICE_EFFECT, dieFaceSimple.transform.position + EFFECT_OFFSET, dieFaceSimple.transform).AutoDestroy();
		effectInstance.SpineAnimation.AnimationName = animationName;
		if ((bool)spinnerMotion)
		{
			spinnerMotion.loop = false;
			spinnerMotion.animationSpeed = 1f;
		}
		if (IsBestRoll(index) && allowBestRoll)
		{
			GlobalEffectsManager.Create(EffectType.BEST_ROLL_SPARKS, dieFaceSimple.transform.position + EFFECT_OFFSET, dieFaceSimple.transform).AutoDestroy();
			if ((bool)spinnerMotion)
			{
				spinnerMotion.Reset();
				spinnerMotion.AnimationName = "Color Particles ";
			}
		}
		else if ((bool)spinnerMotion)
		{
			spinnerMotion.Reset();
			spinnerMotion.AnimationName = "White Particles";
		}
		if (dieFaceSimple.RollAction != DieFaceType.Special && tweakScaleAndOpacity)
		{
			ConfigureRollValueEffect(effectInstance);
		}
		if (index == 0)
		{
			if (dieFaceSimple.RollAction == DieFaceType.DirectDamage)
			{
				AudioTrigger.LowDamageResult.Play();
			}
			if (!_unit.isEnemy)
			{
				AudioTrigger.SpinWorstResult.Play();
			}
		}
		else
		{
			AudioTrigger.CrowdCheering.Play();
			if (dieFaceSimple.RollAction == DieFaceType.DirectDamage)
			{
				AudioTrigger.HighDamageResult.Play();
			}
			else if (dieFaceSimple.RollAction == DieFaceType.ArmourPiercing)
			{
				AudioTrigger.HighDamageResult.Play();
			}
			else if (dieFaceSimple.RollAction == DieFaceType.Initiative)
			{
				AudioTrigger.HighFirstStrikeResult.Play();
			}
			else if (dieFaceSimple.RollAction == DieFaceType.Special)
			{
				AudioTrigger.SpecialResult.Play();
			}
		}
		return effectInstance;
	}

	private bool IsBestRoll(int index)
	{
		if (index == _unit.state.rollValues.Length - 1)
		{
			return true;
		}
		int num = _unit.state.rollValues[_unit.state.rollValues.Length - 1];
		DieFaceType dieFaceType = _unit.state.rollTypes[_unit.state.rollTypes.Length - 1];
		if (_unit.state.rollValues[index] == num && _unit.state.rollTypes[index] == dieFaceType)
		{
			return true;
		}
		return false;
	}

	private void ConfigureRollValueEffect(EffectInstance effect)
	{
		if ((bool)effect)
		{
			float num = 1f;
			float a = 1f;
			DiceEffectsTunables diceEffectTunables = _unit.BattleController.tunables.diceEffectTunables;
			if (diceEffectTunables.diceEffectConfigurations.Length <= _unit.CurrentRoll)
			{
				Log.Warning("Dice effects configurations and dice effect values are not consistent", base.gameObject);
			}
			else
			{
				num = diceEffectTunables.diceEffectConfigurations[_unit.CurrentRoll].scaleMultiplier;
				a = diceEffectTunables.diceEffectConfigurations[_unit.CurrentRoll].opacity;
			}
			tk2dSpineSkeleton component = effect.GetComponent<tk2dSpineSkeleton>();
			if ((bool)component)
			{
				component.skeleton.A = a;
			}
			if ((bool)effect)
			{
				effect.transform.localScale *= num;
			}
		}
	}

	public void SetInfoBoxWidth(float s)
	{
		float num = s * 192f;
		int num2 = 0;
		_dieFaceBox.dimensions = new Vector2(num, _dieFaceBox.dimensions.y);
		DieFaceSimple[] dieFaces = _dieFaces;
		foreach (DieFaceSimple dieFaceSimple in dieFaces)
		{
			Vector3 localPosition = dieFaceSimple.transform.localPosition;
			localPosition.x = 0f - num + 20f + _DieFaceWidth * (float)num2;
			if (localPosition.x < 0f)
			{
				dieFaceSimple.transform.localPosition = localPosition;
			}
			else
			{
				localPosition.x = dieFaceSimple.transform.localPosition.x;
			}
			num2++;
		}
	}

	private void OnSpinnerAnimationComplete(tk2dSpineAnimation spineAnimation)
	{
		if (!spineAnimation.loop)
		{
			spineAnimation.gameObject.SetActive(false);
		}
	}
}
