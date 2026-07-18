using System.Collections.Generic;
using UnityEngine;

public class LucyTankRig : MonoBehaviour
{
	public tk2dSprite tankSprite;

	[SerializeField]
	private List<UnitAnimationLayer> animationLayers;

	private void Start()
	{
		Initialize();
	}

	[ContextMenu("Reinitialize")]
	private void Initialize()
	{
		tk2dSprite component = tankSprite.GetComponent<tk2dSprite>();
		string baseSpriteName = component.GetCurrentSpriteDef().name;
		foreach (UnitAnimationLayer animationLayer in animationLayers)
		{
			animationLayer.Activate(component.Collection, baseSpriteName);
		}
	}

	private void Update()
	{
	}
}
