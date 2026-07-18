using System;
using System.Collections;
using UnityEngine;

public class CubeBarController : MonoBehaviour
{
	public enum State
	{
		Main = 0,
		Text = 1,
		Action = 2
	}

	[HideInInspector]
	public BattleController battleController;

	private float hudDelayTime;

	private Action cancelCallback;

	private State currentState;

	[SerializeField]
	private CubeRotator[] cubeRotators;

	[SerializeField]
	private tk2dTextMesh actionText;

	[SerializeField]
	private tk2dTextMesh fullText;

	[SerializeField]
	private Transform[] cubeTransforms;

	[SerializeField]
	private AbilityButton[] abilityButtons;

	[SerializeField]
	private float rotationTime = 0.25f;

	[SerializeField]
	private HUDEnergyWidget _energyWidget;

	[SerializeField]
	private tk2dUIItem readyButton;

	[SerializeField]
	private tk2dBaseSprite readyButtonIcon;

	[SerializeField]
	private tk2dTextMesh readyButtonText;

	private bool isRotating;

	private int lastEnergy;

	public HUDEnergyWidget EnergyWidget
	{
		get
		{
			return _energyWidget;
		}
	}

	public bool IsRotating
	{
		get
		{
			return isRotating;
		}
	}

	public State CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public bool BattleButtonEnabled
	{
		get
		{
			return readyButton.enabled;
		}
		set
		{
			bool flag = readyButton.enabled;
			readyButton.enabled = value;
			readyButtonText.gameObject.SetActive(value);
			readyButtonIcon.renderer.enabled = value;
			if (flag && value)
			{
				SimpleTween.Start(0f, 1f, 1f, delegate(float alphaVal)
				{
					BattleButtonAlpha = alphaVal;
				});
			}
		}
	}

	public float BattleButtonAlpha
	{
		get
		{
			return readyButtonIcon.Alpha;
		}
		set
		{
			readyButtonIcon.Alpha = value;
			readyButtonText.Alpha = value;
		}
	}

	public Color BattleButtonTint
	{
		get
		{
			if ((bool)readyButtonIcon)
			{
				return readyButtonIcon.color;
			}
			return Color.black;
		}
		set
		{
			if ((bool)readyButtonIcon)
			{
				readyButtonIcon.color = value;
			}
		}
	}

	public void Init(BattleController battleController)
	{
		this.battleController = battleController;
		cubeRotators = new CubeRotator[cubeTransforms.Length];
		for (int i = 0; i < cubeTransforms.Length; i++)
		{
			cubeRotators[i] = cubeTransforms[i].GetComponent<CubeRotator>();
		}
		for (int j = 0; j < 4; j++)
		{
			abilityButtons[j].hudController = battleController.hud;
		}
		hudDelayTime = 0.015f;
		for (int k = 0; k < cubeRotators.Length; k++)
		{
			cubeRotators[k].RotationTime = rotationTime;
		}
		lastEnergy = battleController.playerTeam.energy;
	}

	public void UpdatePlayerAbilities(AbilityState[] abilities, bool updateActive = false)
	{
		for (int i = 0; i < 4; i++)
		{
			AbilityState ability = ((i >= abilities.Length) ? null : abilities[i]);
			abilityButtons[i].SetAbility(ability);
			if (updateActive)
			{
				abilityButtons[i].UpdateButtonActive();
			}
		}
		if (battleController != null)
		{
			lastEnergy = battleController.playerTeam.energy;
		}
	}

	public void RefreshAbilities()
	{
		AbilityButton[] array = abilityButtons;
		foreach (AbilityButton abilityButton in array)
		{
			abilityButton.SetLEDStates();
		}
	}

	public void OnEnergyUpdated(int energyBefore, int energyAfter)
	{
		UpdateAbilityButtonState();
		int num = energyAfter - energyBefore;
		if (num < 0)
		{
			battleController.hud.actionPointText.ShowActionPointText(string.Format("ui_action_point_used".Localize("{0} Ability Point"), num));
		}
	}

	public void Update()
	{
		if (battleController != null && battleController.playerTeam != null && lastEnergy != battleController.playerTeam.energy)
		{
			OnEnergyUpdated(lastEnergy, battleController.playerTeam.energy);
			lastEnergy = battleController.playerTeam.energy;
		}
	}

	public void GoToMainState()
	{
		if (currentState != State.Main)
		{
			isRotating = true;
			currentState = State.Main;
			StartCoroutine(RotateMainStateCubes());
			ResetText();
			UpdateAbilityButtonState();
			AudioTrigger.RotateBottomBar.Play();
			isRotating = false;
		}
	}

	public void GoToTextState(string newFullText)
	{
		if (currentState == State.Text)
		{
			UpdateTextState(newFullText);
			return;
		}
		isRotating = true;
		currentState = State.Text;
		ResetText();
		StartCoroutine(RotateTextStateCubes(newFullText));
		AudioTrigger.RotateBottomBar.Play();
		isRotating = false;
	}

	public void GoToActionState(string newActionText, Action cancelCallback)
	{
		if (currentState != State.Action)
		{
			isRotating = true;
			currentState = State.Action;
			StartCoroutine(RotateActionStateCubes(newActionText));
			this.cancelCallback = cancelCallback;
			AudioTrigger.RotateBottomBar.Play();
			isRotating = false;
		}
	}

	public void UpdateTextState(string newFullText)
	{
		fullText.text = newFullText.ToUpper();
		fullText.Commit();
	}

	private void ResetText()
	{
		actionText.text = string.Empty;
		actionText.Commit();
		fullText.text = string.Empty;
		fullText.Commit();
	}

	private IEnumerator RotateMainStateCubes()
	{
		for (int i = 0; i < cubeRotators.Length; i++)
		{
			cubeRotators[i].DoAllRotations(CubeFace.FRONT);
			yield return new WaitForSeconds(hudDelayTime);
		}
	}

	private IEnumerator RotateActionStateCubes(string newActionText)
	{
		actionText.gameObject.SetActive(false);
		actionText.text = newActionText.ToUpper();
		actionText.Commit();
		for (int i = 0; i < cubeRotators.Length; i++)
		{
			cubeRotators[i].DoAllRotations(CubeFace.BACK);
			yield return new WaitForSeconds(hudDelayTime);
		}
		yield return new WaitForSeconds(rotationTime);
		actionText.gameObject.SetActive(true);
	}

	private IEnumerator RotateTextStateCubes(string newFullText)
	{
		fullText.gameObject.SetActive(false);
		UpdateTextState(newFullText);
		for (int i = 0; i < cubeRotators.Length; i++)
		{
			cubeRotators[i].DoAllRotations(CubeFace.TOP);
			yield return new WaitForSeconds(hudDelayTime);
		}
		yield return new WaitForSeconds(rotationTime);
		fullText.gameObject.SetActive(true);
	}

	private void UpdateAbilityButtonState()
	{
		battleController.hud.UpdateBattleButton();
		AbilityButton[] array = abilityButtons;
		foreach (AbilityButton abilityButton in array)
		{
			abilityButton.UpdateButtonActive();
		}
	}

	private void CancelAction()
	{
		if (isRotating || CurrentState != State.Action)
		{
			Log.Warning("Cannot press cancel button while rotating, or not in action state.");
		}
		else if (cancelCallback != null)
		{
			cancelCallback();
		}
	}

	public void FakeReadyButtonPress()
	{
		if ((bool)readyButton)
		{
			readyButton.Press(default(tk2dUITouch));
			readyButton.Release();
		}
		else
		{
			Log.Warning("Button not found on HUDManager");
		}
	}

	public void SetAbilityEnabled(int index, bool val)
	{
		abilityButtons[index].Toggle(val);
		SetAbilityViewLockState(index, !val);
	}

	public void SetAbilityViewLockState(int abilityIndex, bool state)
	{
		if (abilityIndex >= abilityButtons.Length)
		{
			Log.Error("Cannot lock ability view: " + abilityIndex);
		}
		else
		{
			abilityButtons[abilityIndex].Locked = state;
		}
	}
}
