using System.Collections.Generic;
using UnityEngine;

public class EditTeamAbilityCell : EditTeamBaseCell
{
	private const string LOCKED_ABILITY_ICON = "lock-icon";

	private const string ENERGY_ON = "HUD_button_energy_on";

	private const string ENERGY_OFF = "HUD_button_energy_off";

	[SerializeField]
	private tk2dTextMesh abilityNameLabel;

	[SerializeField]
	private tk2dTextMesh tierTextLabel;

	[SerializeField]
	private tk2dBaseSprite abilityButtonBackground;

	[SerializeField]
	private tk2dSprite iconSprite;

	[SerializeField]
	private tk2dSprite overlay;

	[SerializeField]
	private GameObject energyContainer;

	[SerializeField]
	private List<tk2dSprite> energySprites;

	[SerializeField]
	private tk2dSlicedSprite bannerSprite;

	private Vector3 emptyStateTextPosition = new Vector3(0f, -5f, 0f);

	private Vector3 lockedStateTextPosition = new Vector3(0f, -85f, 0f);

	private Dictionary<int, int> bannerSizes = new Dictionary<int, int>
	{
		{ 0, 0 },
		{ 1, 80 },
		{ 2, 105 },
		{ 3, 130 },
		{ 4, 155 }
	};

	public AbilityDataModel Ability
	{
		get
		{
			return (AbilityDataModel)base.DataObject;
		}
	}

	public override void ConfigureCellData()
	{
		if (Ability != null)
		{
			energyContainer.gameObject.SetActive(true);
			iconSprite.gameObject.SetActive(true);
			if ((bool)abilityButtonBackground)
			{
				abilityButtonBackground.gameObject.SetActive(true);
			}
			UserProfile player = UserProfile.player;
			ProgressionDivisionDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(Ability.unlockTier);
			if (single != null)
			{
				if (player.HasUnlockedAbility(Ability.ID))
				{
					SetCellUnlocked();
				}
				else
				{
					SetCellLocked(single);
				}
			}
			abilityNameLabel.text = Ability.Name;
			float num = 36f;
			Vector3 vector = new Vector3(0f - (float)(Ability.Cost - 1) * (num * 0.5f), 0f, -1f);
			for (int i = 0; i < energySprites.Count; i++)
			{
				tk2dSprite tk2dSprite2 = energySprites[i];
				tk2dSprite2.transform.localPosition = vector + i * Vector3.right * num;
				tk2dSprite2.transform.localRotation = Quaternion.identity;
				if (i < Ability.actionPoint)
				{
					tk2dSprite2.gameObject.SetActive(true);
				}
				else
				{
					tk2dSprite2.gameObject.SetActive(false);
				}
			}
			if ((bool)bannerSprite)
			{
				bannerSprite.gameObject.SetActive(true);
				Vector2 dimensions = new Vector2(bannerSprite.dimensions.x, bannerSizes[Ability.actionPoint]);
				bannerSprite.dimensions = dimensions;
			}
		}
		else
		{
			if ((bool)abilityButtonBackground)
			{
				abilityButtonBackground.gameObject.SetActive(false);
			}
			energyContainer.gameObject.SetActive(false);
			iconSprite.gameObject.SetActive(false);
			if ((bool)overlay)
			{
				overlay.gameObject.SetActive(false);
			}
			if ((bool)bannerSprite)
			{
				bannerSprite.gameObject.SetActive(false);
			}
			abilityNameLabel.text = string.Empty;
			tierTextLabel.text = "ui_editabilities_dragabilityhere".Localize("Drag ability here");
			tierTextLabel.color = Color.white;
			tierTextLabel.transform.localPosition = emptyStateTextPosition;
		}
		base.ConfigureCellData();
	}

	private void SetCellLocked(ProgressionDivisionDataModel division)
	{
		iconSprite.SetSprite("lock-icon");
		if (division != null)
		{
			tierTextLabel.color = Color.red;
			tierTextLabel.text = string.Format("ui_editabilities_unlocked".Localize("Unlocked at {0}"), division.name);
			tierTextLabel.transform.localPosition = lockedStateTextPosition;
		}
		else
		{
			tierTextLabel.text = string.Empty;
		}
		base.DragItem.Locked = true;
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(true);
		}
	}

	private void SetCellUnlocked()
	{
		iconSprite.SetSprite(Ability.ButtonIconArtName);
		tierTextLabel.text = string.Empty;
		base.DragItem.Locked = false;
		base.DragItem.collider.enabled = true;
		if ((bool)overlay)
		{
			overlay.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (tierTextLabel.gameObject.activeSelf && !string.IsNullOrEmpty(tierTextLabel.text))
		{
			tierTextLabel.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, 2f));
		}
	}
}
