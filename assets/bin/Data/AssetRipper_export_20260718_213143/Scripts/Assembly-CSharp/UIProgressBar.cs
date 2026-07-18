using UnityEngine;

public class UIProgressBar : MonoBehaviour
{
	[SerializeField]
	private tk2dSlicedSprite _bar;

	private float _totalWidth;

	private void Awake()
	{
		if (_bar == null)
		{
			_bar = base.gameObject.GetComponent<tk2dSlicedSprite>();
		}
		_totalWidth = _bar.dimensions.x;
		SetValue(0);
	}

	public void SetValue(int barValue)
	{
		_bar.dimensions = new Vector2(barValue, _bar.dimensions.y);
	}

	public void SetPercentage(float percentage)
	{
		float num = percentage * _totalWidth;
		SetValue((int)num);
	}

	public void SetBarColor(Color c)
	{
		_bar.color = c;
	}

	public Color GetBarColor()
	{
		return _bar.color;
	}
}
