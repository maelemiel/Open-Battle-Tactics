using System;
using UnityEngine;

public class DiceEffectsTunables : MonoBehaviour
{
	[Serializable]
	public class DiceEffectTunableValues
	{
		public float opacity;

		public float scaleMultiplier;
	}

	public DiceEffectTunableValues[] diceEffectConfigurations;

	public float diceUITimeSpin = 0.06f;
}
