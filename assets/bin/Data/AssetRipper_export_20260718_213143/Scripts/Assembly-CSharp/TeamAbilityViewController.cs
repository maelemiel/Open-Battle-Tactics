using UnityEngine;

public class TeamAbilityViewController : MonoBehaviour
{
	[SerializeField]
	private TeamAbilitiesLabelController[] tabLabels;

	private void Start()
	{
		for (int i = 0; i < tabLabels.Length; i++)
		{
			tabLabels[i].SetLocalizedLabels(i + 1);
		}
	}
}
