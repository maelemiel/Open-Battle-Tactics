using System.Collections.Generic;
using UnityEngine;

public class AbilityItemBottomBarController : MonoBehaviour
{
	private const string EMPTY_ICON_SPRITE_NAME = "fake";

	public tk2dSprite[] abilityIconSprites;

	public void UpdateAbilityIcons(List<string> abilityIDs)
	{
		if (abilityIDs != null && abilityIDs.Count > abilityIconSprites.Length)
		{
			Log.Error(abilityIDs.Count + ">" + abilityIconSprites.Length);
			Log.Error("Abilities and their icons are inconsistent", base.gameObject);
			return;
		}
		for (int i = 0; i < abilityIDs.Count; i++)
		{
			AbilityDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityDataModel>(abilityIDs[i]);
			if (single != null)
			{
				abilityIconSprites[i].SetSprite(single.ButtonIconArtName);
			}
			else if (single == null)
			{
				abilityIconSprites[i].SetSprite("fake");
			}
		}
		for (int j = abilityIDs.Count; j < abilityIconSprites.Length; j++)
		{
			abilityIconSprites[j].SetSprite("fake");
		}
	}
}
