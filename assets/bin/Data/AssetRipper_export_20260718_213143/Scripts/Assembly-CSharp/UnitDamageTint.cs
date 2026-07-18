using Holoville.HOTween;
using UnityEngine;

public class UnitDamageTint : MonoBehaviour
{
	private UnitView unitView;

	[SerializeField]
	private float blinkCutoffMinor = 0.95f;

	[SerializeField]
	private float blinkCutoffMajor = 0.85f;

	[SerializeField]
	private float blinkFrequencyMinor = 3f;

	[SerializeField]
	private float blinkFrequencyMajor = 30f;

	[SerializeField]
	private Color blinkColor = Color.red;

	private bool _animating;

	private int randomOffset;

	private void Start()
	{
		unitView = GetComponent<UnitView>();
		randomOffset = (int)(Random.value * 10000f);
	}

	public void Reset()
	{
		unitView.SetUnitColor(Color.white);
	}

	private void Update()
	{
		if (unitView.state == null)
		{
			return;
		}
		float num = (float)unitView.LocalHealth / (float)unitView.state.startingHp;
		bool flag = num < 0.25f;
		bool flag2 = num < 0.5f;
		float num2 = 0f;
		float num3 = 0f;
		if (flag)
		{
			num3 = blinkFrequencyMajor;
			num2 = blinkCutoffMajor;
		}
		else if (flag2)
		{
			num3 = blinkFrequencyMinor;
			num2 = blinkCutoffMinor;
		}
		if ((flag || flag2) && !_animating && unitView.HasUnitSprite())
		{
			if (Mathf.Sin(Time.time * num3 + (float)randomOffset) > num2)
			{
				unitView.SetUnitColor(blinkColor);
			}
			else
			{
				unitView.SetUnitColor(Color.white);
			}
		}
	}

	public void PlayReceiveDamageTint()
	{
		_animating = true;
		SimpleTween.Start(0f, 1f, 0.3f, EaseType.EaseInExpo, delegate(float val)
		{
			unitView.SetUnitColor(Color.Lerp(Color.red, Color.white, val));
		}, delegate
		{
			_animating = false;
		});
	}
}
