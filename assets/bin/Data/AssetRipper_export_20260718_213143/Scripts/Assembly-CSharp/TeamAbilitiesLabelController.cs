using UnityEngine;

public class TeamAbilitiesLabelController : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh abilityIndexLabel;

	[SerializeField]
	private tk2dTextMesh abilityIndexLabelDisabled;

	public void SetLocalizedLabels(int number)
	{
		if ((bool)abilityIndexLabel && (bool)abilityIndexLabelDisabled)
		{
			abilityIndexLabel.Alpha = 1f;
			abilityIndexLabel.text = string.Format("ui_editabilities_ability_number".Localize("TEAM {0}"), number);
			abilityIndexLabelDisabled.Alpha = 1f;
			abilityIndexLabelDisabled.text = abilityIndexLabel.text;
		}
	}
}
