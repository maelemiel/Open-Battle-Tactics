using UnityEngine;

public class ButtonEnergy : MonoBehaviour
{
	private string[] energyAmount = new string[3] { "off", "off", null };

	private tk2dSprite[] energyOnList;

	private tk2dSprite sdf;

	private void Start()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < energyAmount.Length; i++)
		{
			if (energyAmount[i] != null)
			{
				num++;
			}
			if (energyAmount[i] == "on")
			{
				num2++;
			}
		}
		for (int j = 0; j < num2; j++)
		{
		}
	}

	private void Update()
	{
	}
}
