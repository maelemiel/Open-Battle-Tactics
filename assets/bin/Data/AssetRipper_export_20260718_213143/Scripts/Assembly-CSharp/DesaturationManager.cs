using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class DesaturationManager : MonoBehaviour
{
	private Dictionary<tk2dBaseSprite, Material> spriteMaterialBackupTable = new Dictionary<tk2dBaseSprite, Material>();

	private List<tk2dBaseSprite> desaturatedSprites = new List<tk2dBaseSprite>();

	private List<Tweener> animations = new List<Tweener>();

	private bool canDesaturate = true;

	[SerializeField]
	private Shader desaturationShader;

	public void Desaturate(List<tk2dBaseSprite> sprites)
	{
		if (canDesaturate)
		{
			canDesaturate = false;
			{
				foreach (tk2dBaseSprite sprite in sprites)
				{
					if (sprite == null)
					{
						continue;
					}
					Material spriteMaterial = sprite.renderer.sharedMaterial;
					if (!spriteMaterialBackupTable.ContainsKey(sprite) && spriteMaterial != null)
					{
						spriteMaterialBackupTable[sprite] = spriteMaterial;
						spriteMaterial = new Material(spriteMaterial);
						spriteMaterial.shader = desaturationShader;
						sprite.renderer.material = spriteMaterial;
						animations.Add(SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
						{
							spriteMaterial.SetFloat("_Desaturation", val);
						}));
					}
					desaturatedSprites.Add(sprite);
					if (sprite is tk2dSprite)
					{
						(sprite as tk2dSprite).ignoreCustomMaterial = true;
					}
				}
				return;
			}
		}
		Debug.LogError("Cannot Desaturate Sprites Twice. Please call the method ResetSaturation() before.");
	}

	public void ResetSaturation()
	{
		canDesaturate = true;
		foreach (Tweener animation in animations)
		{
			animation.Kill();
		}
		animations.Clear();
		foreach (tk2dBaseSprite key in spriteMaterialBackupTable.Keys)
		{
			key.renderer.sharedMaterial = spriteMaterialBackupTable[key];
		}
		foreach (tk2dBaseSprite desaturatedSprite in desaturatedSprites)
		{
			if (desaturatedSprite is tk2dSprite)
			{
				(desaturatedSprite as tk2dSprite).ignoreCustomMaterial = false;
			}
		}
		spriteMaterialBackupTable.Clear();
		desaturatedSprites.Clear();
	}
}
