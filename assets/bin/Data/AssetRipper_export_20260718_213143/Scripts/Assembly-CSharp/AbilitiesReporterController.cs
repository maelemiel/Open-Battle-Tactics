using Holoville.HOTween;
using UnityEngine;

public class AbilitiesReporterController : MonoBehaviour
{
	public GameObject _openMarker;

	public GameObject _closeMarker;

	public tk2dSprite[] _abilityIcons;

	private bool _isShowing;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void DisableAbilityIcons()
	{
		tk2dSprite[] abilityIcons = _abilityIcons;
		foreach (tk2dSprite tk2dSprite2 in abilityIcons)
		{
			tk2dSprite2.gameObject.SetActive(false);
		}
	}

	public void Show(float time = 0f)
	{
		if (!_isShowing)
		{
			HOTween.To(base.gameObject.transform, time, "localPosition", _openMarker.transform.localPosition);
			_isShowing = true;
		}
	}

	public void Hide(float time = 0f)
	{
		if (_isShowing)
		{
			HOTween.To(base.gameObject.transform, time, "localPosition", _closeMarker.transform.localPosition);
			_isShowing = false;
		}
	}

	public void SetAbilities(AbilityState[] abilities)
	{
		int num = 0;
		DisableAbilityIcons();
		foreach (AbilityState abilityState in abilities)
		{
			string buttonIconArtName = abilityState.metadata.ButtonIconArtName;
			int spriteIdByName = _abilityIcons[num].GetSpriteIdByName(buttonIconArtName);
			_abilityIcons[num].spriteId = spriteIdByName;
			_abilityIcons[num].gameObject.SetActive(true);
			num++;
		}
	}
}
