using UnityEngine;

public class EffectInstance : MonoBehaviour
{
	public EffectType type;

	public new GameObject gameObject;

	public new Transform transform;

	private Vector3 originalPosition;

	private Quaternion originalRotation;

	private float originalAnimationSpeed;

	private bool _autoDestroy;

	private tk2dSpineAnimation _spineAnimation;

	public Vector3 originalScale { get; private set; }

	public tk2dSpineAnimation SpineAnimation
	{
		get
		{
			if (_spineAnimation == null)
			{
				_spineAnimation = gameObject.GetComponentInChildren<tk2dSpineAnimation>();
				if (_spineAnimation != null)
				{
					originalAnimationSpeed = _spineAnimation.animationSpeed;
				}
			}
			return _spineAnimation;
		}
	}

	public EffectInstance AutoDestroy()
	{
		if ((bool)SpineAnimation)
		{
			SpineAnimation.AnimationComplete += OnSpineAnimComplete;
		}
		_autoDestroy = true;
		return this;
	}

	public void Delay(float time)
	{
		gameObject.SetActive(false);
		Invoke("_DelayStart", time);
	}

	private void _DelayStart()
	{
		if (SpineAnimation != null)
		{
			SpineAnimation.state.Time = 0f;
		}
		gameObject.SetActive(true);
	}

	public EffectInstance SetLayer(int layer, bool recursively = true)
	{
		if (recursively)
		{
			gameObject.SetLayerRecursively(layer);
		}
		else
		{
			gameObject.layer = layer;
		}
		return this;
	}

	public void Reset()
	{
		if (SpineAnimation != null)
		{
			SpineAnimation.Reset();
			SpineAnimation.animationSpeed = originalAnimationSpeed;
		}
		ResetTransform();
	}

	public void ResetTransform(bool position = true, bool rotation = true, bool scale = true)
	{
		if (position)
		{
			transform.localPosition = originalPosition;
		}
		if (rotation)
		{
			transform.localRotation = originalRotation;
		}
		if (scale)
		{
			transform.localScale = originalScale;
		}
	}

	public void Destroy()
	{
		if (_autoDestroy && SpineAnimation != null)
		{
			SpineAnimation.AnimationComplete -= OnSpineAnimComplete;
		}
		GlobalEffectsManager.Return(this);
	}

	private void OnSpineAnimComplete(tk2dSpineAnimation anim)
	{
		Destroy();
	}

	private void OnDestroy()
	{
		Singleton<GlobalEffectsManager>.instance.Pool.Remove(this);
	}

	public static EffectInstance Create(EffectType type, GameObject obj)
	{
		EffectInstance effectInstance = obj.AddComponent<EffectInstance>();
		effectInstance.type = type;
		effectInstance.gameObject = obj;
		effectInstance.transform = obj.transform;
		effectInstance.originalPosition = obj.transform.localPosition;
		effectInstance.originalRotation = obj.transform.localRotation;
		effectInstance.originalScale = obj.transform.localScale;
		return effectInstance;
	}
}
