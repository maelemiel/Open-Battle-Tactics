using System.Collections;
using UnityEngine;

public class TargetIndicator : MonoBehaviour
{
	public enum TargetIndicatorType
	{
		WHITE = 0,
		RED = 1,
		ADVANCED = 2
	}

	private const string ANIMATION_NAME_TARGET_RED = "Target Red";

	private const string ANIMATION_NAME_TARGET_WHITE = "Target White";

	private const string ANIMATION_NAME_DISABLE_TARGET_RED = "Target Red Hologram";

	private const string ANIMATION_NAME_DISABLE_TARGET_WHITE = "Target White Hologram";

	private const string ANIMATION_NAME_TARGET_ADVANCED = "Advanced Targeting";

	private const string ANIMATION_NAME_DISABLE_TARGET_ADVANCED = "Advanced Targeting Hologram";

	private tk2dSpineAnimation spineAnimation;

	private void Awake()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
		if (!spineAnimation)
		{
			Debug.LogError("No reference to Spine Animation!", this);
		}
	}

	public void SetTarget(UnitView _target, TargetIndicatorType type)
	{
		string animationName = string.Empty;
		switch (type)
		{
		case TargetIndicatorType.RED:
			animationName = "Target Red";
			break;
		case TargetIndicatorType.WHITE:
			animationName = "Target White";
			break;
		case TargetIndicatorType.ADVANCED:
			animationName = "Advanced Targeting";
			break;
		}
		Vector3 position = new Vector3(_target.TankSpritesTransform.position.x, _target.TankSpritesTransform.position.y - 60f, _target.TankSpritesTransform.position.z + 1f);
		base.transform.position = position;
		if ((bool)spineAnimation)
		{
			spineAnimation.loop = false;
			spineAnimation.AnimationName = animationName;
			spineAnimation.animationSpeed = 1f;
		}
	}

	public IEnumerator AnimateBack(TargetIndicatorType type)
	{
		string animationName = string.Empty;
		switch (type)
		{
		case TargetIndicatorType.RED:
			animationName = "Target Red Hologram";
			break;
		case TargetIndicatorType.WHITE:
			animationName = "Target White Hologram";
			break;
		case TargetIndicatorType.ADVANCED:
			animationName = "Advanced Targeting Hologram";
			break;
		}
		spineAnimation.AnimationName = animationName;
		yield return new WaitForSeconds(0.5f);
	}
}
