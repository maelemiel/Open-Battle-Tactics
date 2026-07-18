using System;
using UnityEngine;

public class NewsBackgroundController : MonoBehaviour
{
	public enum Type
	{
		None = 100,
		field = 0,
		desert = 1,
		blue = 2,
		grass = 3,
		red = 4,
		snow = 5,
		steel = 6,
		tron = 7
	}

	[SerializeField]
	private tk2dBaseSprite _background;

	[SerializeField]
	private AnimateTextureOffset _backgroundAnimation;

	[SerializeField]
	private NewsBackgroundModel[] _backgroundConfigurations;

	[SerializeField]
	private Type _defaultBackground = Type.None;

	private void Awake()
	{
		if (_defaultBackground != Type.None)
		{
			_SetBackgroundsWithIndex((int)_defaultBackground);
		}
	}

	public void SetBackground(string backgroundType, float speed = 0.4f)
	{
		Type index = (Type)(int)Enum.Parse(typeof(Type), backgroundType);
		_SetBackgroundsWithIndex((int)index);
		_backgroundAnimation.SetHorizontalSpeed(speed);
	}

	public void SetBackgroundSpeed(float speed)
	{
		_backgroundAnimation.SetHorizontalSpeed(speed);
	}

	public void SetBackgroundSpeed(int speedPercentage)
	{
		_backgroundAnimation.SetHorizontalSpeed((float)speedPercentage / 100f);
	}

	private void _SetBackgroundsWithIndex(int index)
	{
		NewsBackgroundModel newsBackgroundModel = _backgroundConfigurations[index];
		tk2dSpriteCollectionData newCollection = Resources.Load<tk2dSpriteCollectionData>(newsBackgroundModel.backgroundSpriteCollectionName);
		_background.SetSprite(newCollection, newsBackgroundModel.backgroundName);
		Resources.UnloadUnusedAssets();
	}
}
