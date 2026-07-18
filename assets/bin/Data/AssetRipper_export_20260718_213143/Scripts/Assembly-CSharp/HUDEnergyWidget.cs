using System.Collections;
using UnityEngine;

public class HUDEnergyWidget : MonoBehaviour
{
	private const string SPRITE_NAME_ENERGY_ON = "BigLight_ON_v2";

	private const string SPRITE_NAME_ENERGY_OFF = "BigLight_Off_v2";

	[SerializeField]
	private tk2dSprite[] energyUnits;

	public int currentEnergy;

	public void UpdateEnergy(int currentEnergy)
	{
		this.currentEnergy = Mathf.Min(energyUnits.Length, currentEnergy);
		for (int i = 0; i < energyUnits.Length; i++)
		{
			energyUnits[i].SetSprite((i + 1 > currentEnergy) ? "BigLight_Off_v2" : "BigLight_ON_v2");
		}
	}

	public IEnumerator SpendLastTicketAnimation(float time)
	{
		if (energyUnits.Length != 0 && currentEnergy != 0)
		{
			Vector3 initialScale = energyUnits[0].transform.localScale;
			SimpleTween.Start(0f, 1f, time, delegate(float val)
			{
				Color color = energyUnits[currentEnergy - 1].color;
				color.a = 1f - val;
				energyUnits[currentEnergy - 1].color = color;
				energyUnits[currentEnergy - 1].transform.localScale = initialScale + initialScale * val;
			});
			yield return new WaitForSeconds(time);
		}
	}
}
