using System.Collections;
using UnityEngine;

public class UnitItemView : MonoBehaviour
{
	protected const string BACKGROUND_ACTIVE_SPRITE_NAME = "btn_square_196_active";

	protected const string BACKGROUND_UNACTIVE_SPRITE_NAME = "btn_square_196";

	protected const string BUTTON_ACTIVE_SPRITE_NAME = "btn_blue_add";

	protected const string BUTTON_UNACTIVE_SPRITE_NAME = "btn_red_add";

	protected const string DIE_FACE_PREFIX = "DieFace_";

	protected const string NO_SPECIAL_TEXT = "No special";

	protected const string SPECIAL_TEXT = "Special: ";

	protected const string MAX_LEVEL_REACHED_TEXT = "Max Level";

	protected const string MAX_LEVEL_BUTTON_SPRITE_NAME = "HUD_button_cancel_inactive";

	protected const string PROMOTE_TEXT = "Promote";

	protected const string PROMOTE_BUTTON_SPRITE_NAME = "HUD_button_confirm_active";

	public const int TANK_SPRITE_SORTING_ORDER = 7;

	public const int TANK_POPUP_SPRITE_SORTING_ORDER = 14;

	public const int BOTTOM_BAR_TANK_SPRITE_SORTING_ORDER = 7;

	[SerializeField]
	protected tk2dSprite background;

	[SerializeField]
	protected tk2dSprite icon;

	[SerializeField]
	protected tk2dTextMesh unitNameLabel;

	[SerializeField]
	protected tk2dTextMesh levelTitleLabel;

	[SerializeField]
	protected tk2dTextMesh levelLabel;

	[SerializeField]
	protected tk2dTextMesh hpLabel;

	[SerializeField]
	protected tk2dTextMesh abilityLabel;

	[SerializeField]
	protected tk2dTextMesh abilityDescriptionLabel;

	[SerializeField]
	protected tk2dSprite[] dieFaces;

	[SerializeField]
	protected tk2dTextMesh[] dieValues;

	[SerializeField]
	protected UnitProxy unitProxy;

	[SerializeField]
	protected tk2dSprite maxLevelButtonSprite;

	[SerializeField]
	protected tk2dTextMesh promoteButtonLabel;

	[SerializeField]
	protected ChartBarView rarityStarsView;

	[SerializeField]
	private Color dieFacesActiveColor;

	[SerializeField]
	private Color dieFacesUnactiveColor;

	public tk2dSprite[] DieFaces
	{
		get
		{
			return dieFaces;
		}
	}

	public GameObject HealthLabel
	{
		get
		{
			if ((bool)hpLabel)
			{
				return hpLabel.gameObject;
			}
			return null;
		}
	}

	public GameObject UnitGameObject
	{
		get
		{
			if ((bool)unitProxy)
			{
				return unitProxy.gameObject;
			}
			return null;
		}
	}

	public void SetState(bool state)
	{
		string sprite = ((!state) ? "btn_square_196" : "btn_square_196_active");
		if (background != null)
		{
			background.SetSprite(sprite);
		}
		SetDieFacesState(state);
	}

	public virtual void SetUnitName(string unitName)
	{
		if (!string.IsNullOrEmpty(unitName))
		{
			unitNameLabel.text = unitName;
		}
	}

	public void SetUnitAbilityDescription(UserUnit unit)
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (unit.HasSpecial)
		{
			text = unit.AbilityName;
			text2 = "Special: ";
		}
		if ((bool)abilityLabel)
		{
			abilityLabel.text = text2;
		}
		if ((bool)abilityDescriptionLabel)
		{
			abilityDescriptionLabel.text = text;
		}
	}

	public virtual IEnumerator SetAssetBundle(int assetBundleID)
	{
		if ((bool)unitProxy)
		{
			unitProxy.gameObject.SetActive(true);
			yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", assetBundleID));
		}
		tk2dSprite tankSprite = unitProxy.GetComponentInChildren<tk2dSprite>();
		if ((bool)tankSprite)
		{
			tankSprite.SortingOrder = 7;
			tankSprite.gameObject.layer = unitProxy.gameObject.layer;
		}
	}

	public virtual void SetUnitLevel(int level)
	{
		if ((bool)levelLabel)
		{
			levelLabel.text = level.ToString();
		}
	}

	public void SetUnitHP(int hp)
	{
		if ((bool)hpLabel)
		{
			hpLabel.text = hp.ToString();
		}
	}

	public void SetPromoteButtonStateView(bool state)
	{
		if ((bool)maxLevelButtonSprite && (bool)promoteButtonLabel)
		{
			if (state)
			{
				maxLevelButtonSprite.SetSprite("HUD_button_confirm_active");
				promoteButtonLabel.text = "Promote";
			}
			else
			{
				maxLevelButtonSprite.SetSprite("HUD_button_cancel_inactive");
			}
			promoteButtonLabel.text = "Promote";
		}
	}

	public void SetPromoteButtonMaxLevel()
	{
		if ((bool)maxLevelButtonSprite && (bool)promoteButtonLabel)
		{
			maxLevelButtonSprite.SetSprite("HUD_button_cancel_inactive");
			promoteButtonLabel.text = "Max Level";
		}
	}

	public void SetDieFaces(DieFaceType[] dieFaceTypes, int[] dieFaceValues)
	{
		if (dieFaces.Length == 0 || dieValues.Length == 0)
		{
			return;
		}
		if (dieFaces != null && dieFaces.Length < dieFaceTypes.Length)
		{
			Log.Error("UnitView dieFaces/dieFaceTypes are not configured correctly", base.gameObject);
		}
		if (dieValues != null && dieValues.Length < dieFaceValues.Length)
		{
			Log.Error("UnitView dieFaces/dieFaceValues are not configured correctly", base.gameObject);
		}
		for (int i = 0; i < dieFaceTypes.Length; i++)
		{
			dieFaces[i].SetSprite("DieFace_" + (int)dieFaceTypes[i]);
		}
		int num = 0;
		for (int j = 0; j < dieFaceValues.Length; j++)
		{
			num = dieFaceValues[j];
			if (num > 0)
			{
				dieValues[j].text = num.ToString();
			}
			else
			{
				dieValues[j].text = string.Empty;
			}
		}
	}

	public void SetDieFace(int index, DieFaceType[] dieFaceTypes, int[] dieFaceValues)
	{
		if (dieFaces.Length > index && dieValues.Length > index)
		{
			dieFaces[index].SetSprite("DieFace_" + (int)dieFaceTypes[index]);
			int num = dieFaceValues[index];
			if (num > 0)
			{
				dieValues[index].text = num.ToString();
			}
			else
			{
				dieValues[index].text = string.Empty;
			}
		}
	}

	public void SetDieFacesState(bool state)
	{
		Color color = ((!state) ? dieFacesUnactiveColor : dieFacesActiveColor);
		for (int i = 0; i < dieFaces.Length; i++)
		{
			dieFaces[i].color = color;
		}
		for (int j = 0; j < dieValues.Length; j++)
		{
			dieValues[j].color = color;
		}
	}

	public void SetRarity(int rarity)
	{
		if ((bool)rarityStarsView)
		{
			rarityStarsView.SetBarLevel(rarity);
		}
	}

	public virtual void ConfigureUnitView(UserUnit unit)
	{
		SetUnitName(unit.Name);
		SetUnitAbilityDescription(unit);
		SetUnitLevel(unit.level);
		SetUnitHP(unit.StartingHealth);
		SetDieFaces(unit.RollTypes, unit.RollValues);
		SetRarity(unit.Rarity - 1);
		unitProxy.gameObject.SetActive(true);
		StartCoroutine(SetAssetBundle(unit.AssetBundleID));
	}
}
