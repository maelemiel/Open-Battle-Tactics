using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButton : MonoBehaviour
{
	private const string SPRITE_NAME_ENABLED_ABILITY_HOLDER = "Ability_button_up";

	private const string SPRITE_NAME_DISABLED_ABILITY_HOLDER = "Ability_button_up_grey";

	private const string SPRITE_NAME_ENABLED_ABILITY_POINT = "BigLight_ON_v2";

	private const string SPRITE_NAME_DISABLED_ABILITY_POINT = "BigLight_Off_v2";

	private AbilityState ability;

	private List<tk2dSprite> _energyLEDs = new List<tk2dSprite>();

	private int litLEDIndex;

	[SerializeField]
	private GameObject _energyObject;

	[SerializeField]
	private tk2dSprite[] abilityIcons;

	[SerializeField]
	private tk2dSpineAnimation abilityPointAnimation;

	[SerializeField]
	private tk2dTextMesh[] abilityNames;

	[SerializeField]
	private tk2dUIUpDownButton upDownButton;

	public HUDController hudController;

	private bool _disabled;

	private tk2dUIItem button;

	[SerializeField]
	private tk2dBaseSprite buttonSprite;

	public bool Disabled
	{
		get
		{
			return _disabled;
		}
	}

	public bool Locked { get; set; }

	private void EnergyLEDPlacement()
	{
		foreach (tk2dSprite energyLED in _energyLEDs)
		{
			Object.Destroy(energyLED.gameObject);
		}
		_energyLEDs = new List<tk2dSprite>();
		int num = ((ability != null) ? ability.metadata.Cost : 0);
		Vector3 vector = new Vector3(0f - (float)(num - 1) * 12f, 45f, -1f);
		GameObject gameObject = null;
		for (int i = 0; i < num; i++)
		{
			gameObject = (GameObject)Object.Instantiate(_energyObject);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = vector + i * Vector3.right * 24f;
			gameObject.transform.localRotation = Quaternion.identity;
			_energyLEDs.Add(gameObject.GetComponent<tk2dSprite>());
			_energyLEDs[i].SetSprite("BigLight_Off_v2");
		}
	}

	private void Awake()
	{
		button = GetComponent<tk2dUIItem>();
		if (!abilityPointAnimation)
		{
			Log.Warning("Ability points animation not found", base.gameObject);
		}
		SetAbility(null);
	}

	public void SetAbility(AbilityState ability)
	{
		this.ability = ability;
		if (ability != null)
		{
			EnergyLEDPlacement();
			SetAbilityName(ability.metadata.Name);
			SetAbilityIcon(this.ability.metadata.ButtonIconArtName);
		}
		else
		{
			EnergyLEDPlacement();
			SetAbilityName(string.Empty);
			SetAbilityIcon(string.Empty);
		}
	}

	public void UpdateButtonActive()
	{
		if (ability == null)
		{
			Toggle(false);
		}
		else if (!Locked)
		{
			Toggle(ability.CanActivate);
		}
	}

	public void Toggle(bool enabled)
	{
		_disabled = !enabled;
		SetButtonVisualState(!_disabled);
		buttonSprite.SetSprite((!_disabled) ? "Ability_button_up" : "Ability_button_up_grey");
		button.enabled = !_disabled;
		if (_disabled)
		{
			StartCoroutine(HackingButtonTiming());
		}
	}

	public void SetLEDStates()
	{
		int num = (litLEDIndex = ((ability != null) ? ability.energy : 0));
		foreach (tk2dSprite energyLED in _energyLEDs)
		{
			energyLED.SetSprite((num <= 0) ? "BigLight_Off_v2" : "BigLight_ON_v2");
			num--;
		}
	}

	public void OnClicked()
	{
		if (hudController.battleController.playerTeam.energy == 0)
		{
			Log.Warning("Player tried to click ability button without enough energy.");
		}
		else if (hudController.cubeBar.IsRotating)
		{
			Log.Warning("Ability cannot be clicked when cube bar is rotating.");
		}
		else if (hudController.cubeBar.CurrentState != CubeBarController.State.Main)
		{
			Log.Warning("Ability cannot be clicked when cube bar is not in main state.");
		}
		else
		{
			if (!hudController.battleController.battleHooks.OnTapAbility(ability) || ability == null)
			{
				return;
			}
			AudioTrigger.AbilityPointSpent.Play();
			int num = ability.metadata.Cost - ability.energy;
			hudController.bouncingArrowAction.Hide();
			hudController.battleController.abilityManager.DepositOneEnergy(ability);
			if (num <= 1)
			{
				if (!_disabled)
				{
					AudioTrigger.AbilityActivated.Play();
				}
			}
			else
			{
				abilityPointAnimation.gameObject.SetActive(true);
				abilityPointAnimation.transform.position = _energyLEDs[litLEDIndex].transform.position;
				abilityPointAnimation.Reset();
				abilityPointAnimation.AnimationComplete += OnAnimationComplete;
			}
			if (litLEDIndex < _energyLEDs.Count)
			{
				_energyLEDs[litLEDIndex].SetSprite("BigLight_ON_v2");
			}
			StartCoroutine(UpdateLEDAfterSec(0f));
		}
	}

	private void OnAnimationComplete(tk2dSpineAnimation spineAnimation)
	{
		abilityPointAnimation.AnimationComplete -= OnAnimationComplete;
		spineAnimation.gameObject.SetActive(false);
	}

	private IEnumerator UpdateLEDAfterSec(float delay)
	{
		yield return new WaitForSeconds(delay);
		SetLEDStates();
	}

	private IEnumerator HackingButtonTiming()
	{
		yield return new WaitForSeconds(0.1f);
		SetLEDStates();
		buttonSprite.SetSprite("Ability_button_up_grey");
	}

	private void SetButtonVisualState(bool state)
	{
		if ((bool)buttonSprite)
		{
			buttonSprite.SetSprite((!state) ? "Ability_button_up_grey" : "Ability_button_up");
		}
		for (int i = 0; i < abilityNames.Length; i++)
		{
			abilityNames[i].color = ((!state) ? Color.gray : Color.white);
		}
		if ((bool)upDownButton)
		{
			upDownButton.IsDown = !state;
		}
	}

	private void SetAbilityName(string name)
	{
		if (abilityNames.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < abilityNames.Length; i++)
		{
			if ((bool)abilityNames[i])
			{
				abilityNames[i].text = name;
			}
		}
	}

	private void SetAbilityIcon(string iconName)
	{
		if (abilityIcons.Length <= 0)
		{
			return;
		}
		if (string.IsNullOrEmpty(iconName))
		{
			for (int i = 0; i < abilityIcons.Length; i++)
			{
				abilityIcons[i].renderer.enabled = false;
			}
			return;
		}
		for (int j = 0; j < abilityIcons.Length; j++)
		{
			abilityIcons[j].renderer.enabled = true;
			abilityIcons[j].SetSprite(iconName);
		}
	}
}
