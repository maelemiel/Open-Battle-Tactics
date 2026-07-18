using UnityEngine;

public class AbilityCell : ScrollableCell
{
	private const string ACTIVE_BACKGROUND_SPRITE_NAME = "glowing_large_button";

	private const string UNACTIVE_BACKGROUND_SPRITE_NAME = "large_button";

	private const string LOCKED_ABILITY_ICON = "lock-icon";

	private const string EMPTY_ABILITY_ICON = "empty";

	private const string CLAIM_ABILITY_ICON = "plus";

	[SerializeField]
	private tk2dTextMesh abilityName;

	[SerializeField]
	private tk2dTextMesh abilityCost;

	[SerializeField]
	private tk2dSprite overlay;

	[SerializeField]
	private tk2dSprite cellBackground;

	[SerializeField]
	private tk2dSprite abilityIcon;

	[SerializeField]
	private tk2dUIProgressBar abilityProgressBar;

	[SerializeField]
	private tk2dSprite checkIcon;

	private AbilityCellState currentCellState;

	private UserResearcher researcher;

	private void Update()
	{
		if (currentCellState == AbilityCellState.RESEARCHING)
		{
			if (researcher.CanClaim)
			{
				currentCellState = AbilityCellState.RESEARCHED;
				SetCellState(currentCellState);
			}
			else
			{
				abilityProgressBar.Value = researcher.Progress;
				abilityName.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(researcher.finishTime, true);
			}
		}
	}

	public override void ConfigureCell()
	{
		if (dataObject == null)
		{
			return;
		}
		if (dataIndex > 0)
		{
			int num = 0;
			for (int i = 1; i <= dataIndex % controller.NUMBER_OF_COLUMNS; i++)
			{
				num = dataIndex - i;
				if (num < 0)
				{
					break;
				}
				if (controller.DataSource[num] == null)
				{
					base.transform.position += new Vector3((0f - controller.cellWidth) * 0.5f, 0f, 0f);
				}
			}
		}
		ConfigureCellData();
	}

	public override void ConfigureCellData()
	{
		if (dataObject != null)
		{
			AbilityDataModel abilityDataModel = (AbilityDataModel)dataObject;
			if (!UserProfile.player.HasUnlockedAbility(abilityDataModel))
			{
				currentCellState = AbilityCellState.LOCKED;
			}
			else if (UserProfile.player.HasUnlockedAbility(abilityDataModel.id))
			{
				currentCellState = AbilityCellState.CLAIMED;
			}
			else
			{
				currentCellState = AbilityCellState.UNLOCKED;
			}
			SetCellState(currentCellState);
		}
	}

	private void SetCellState(AbilityCellState cellState)
	{
		switch (cellState)
		{
		case AbilityCellState.LOCKED:
			SetCellLocked();
			break;
		case AbilityCellState.UNLOCKED:
			SetCellUnlocked();
			break;
		case AbilityCellState.CLAIMED:
			SetAbilityClaimed();
			break;
		case AbilityCellState.RESEARCHED:
			SetAbilityResearched();
			break;
		case AbilityCellState.RESEARCHING:
			SetAbilityResearching();
			break;
		}
	}

	private void OnTouch()
	{
		AbilityDataModel abilityDataModel = (AbilityDataModel)dataObject;
		switch (currentCellState)
		{
		case AbilityCellState.UNLOCKED:
			break;
		case AbilityCellState.CLAIMED:
			PopupManager.ShowPopup(PopupDataModel.InspectAbilityPopUp(abilityDataModel, null));
			break;
		case AbilityCellState.RESEARCHED:
			UserProfile.player.TryClaimResearch(researcher, ConfigureCellData);
			break;
		case AbilityCellState.RESEARCHING:
			PopupManager.ShowPopup(PopupDataModel.SkipWaitPopup(researcher, string.Empty, string.Empty, null, ConfigureCellData));
			break;
		}
	}

	private void SetCellLocked()
	{
		overlay.gameObject.SetActive(true);
		checkIcon.gameObject.SetActive(false);
		AbilityDataModel abilityDataModel = (AbilityDataModel)dataObject;
		abilityName.text = abilityDataModel.Name;
		abilityName.color = Color.red;
		abilityIcon.SetSprite("lock-icon");
		cellBackground.SetSprite("large_button");
		abilityCost.text = string.Empty;
		abilityProgressBar.gameObject.SetActive(false);
	}

	private void SetCellUnlocked()
	{
		overlay.gameObject.SetActive(false);
		checkIcon.gameObject.SetActive(false);
		AbilityDataModel abilityDataModel = (AbilityDataModel)dataObject;
		abilityName.color = Color.white;
		abilityName.text = abilityDataModel.Name;
		abilityIcon.SetSprite("lock-icon");
		cellBackground.SetSprite("glowing_large_button");
		abilityCost.text = string.Empty;
		abilityProgressBar.gameObject.SetActive(false);
	}

	private void SetAbilityClaimed()
	{
		overlay.gameObject.SetActive(false);
		checkIcon.gameObject.SetActive(true);
		AbilityDataModel abilityDataModel = (AbilityDataModel)dataObject;
		abilityName.color = Color.white;
		abilityName.text = abilityDataModel.Name;
		if (abilityDataModel.abilityType != 4)
		{
			abilityIcon.SetSprite(abilityDataModel.ButtonIconArtName);
		}
		else
		{
			abilityIcon.SetSprite("empty");
		}
		cellBackground.SetSprite("glowing_large_button");
		abilityCost.text = string.Empty;
		abilityProgressBar.gameObject.SetActive(false);
	}

	private void SetAbilityResearched()
	{
		overlay.gameObject.SetActive(false);
		checkIcon.gameObject.SetActive(false);
		abilityName.color = Color.white;
		abilityName.text = "Tap to claim!";
		abilityIcon.SetSprite("plus");
		cellBackground.SetSprite("glowing_large_button");
		abilityCost.text = string.Empty;
		abilityProgressBar.gameObject.SetActive(false);
	}

	private void SetAbilityResearching()
	{
		overlay.gameObject.SetActive(false);
		checkIcon.gameObject.SetActive(false);
		abilityName.color = Color.cyan;
		abilityProgressBar.slicedSpriteBar.color = Color.red;
		abilityName.text = NonUnitySingleton<TimeManager>.instance.GetCountdownString(researcher.finishTime);
		abilityProgressBar.gameObject.SetActive(true);
		abilityIcon.SetSprite("lock-icon");
	}
}
