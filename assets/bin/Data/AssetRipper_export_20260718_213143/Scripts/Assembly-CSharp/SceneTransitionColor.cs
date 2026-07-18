using System.Collections.Generic;
using UnityEngine;
using tk2dRuntime;

public class SceneTransitionColor : SceneTransition
{
	public Color color = new Color(1f, 1f, 1f, 1f);

	private List<IRenderable> childrenSprites = new List<IRenderable>();

	private List<Renderer> childrenRenderers = new List<Renderer>();

	private Color[] originalColors;

	public override void PrepareTransition()
	{
		tk2dBaseSprite[] componentsInChildren = GetComponentsInChildren<tk2dBaseSprite>();
		childrenSprites.AddRange(componentsInChildren);
		tk2dTextMesh[] componentsInChildren2 = GetComponentsInChildren<tk2dTextMesh>();
		childrenSprites.AddRange(componentsInChildren2);
		originalColors = new Color[childrenSprites.Count];
		if (childrenSprites == null || childrenSprites.Count == 0)
		{
			Log.Error("No children sprites found to make the transition", base.gameObject);
		}
		for (int i = 0; i < childrenSprites.Count; i++)
		{
			originalColors[i] = childrenSprites[i].color;
			childrenRenderers.Add((childrenSprites[i] as Component).renderer);
		}
		base.PrepareTransition();
	}

	public override void EndTransition()
	{
		for (int i = 0; i < childrenSprites.Count; i++)
		{
			childrenSprites[i].color = originalColors[i];
		}
		base.EndTransition();
	}

	public override void UpdateTransition(float t)
	{
		Color white = Color.white;
		for (int i = 0; i < childrenSprites.Count; i++)
		{
			white = originalColors[i] * (1f - t) + color * t;
			childrenSprites[i].color = white;
			childrenRenderers[i].enabled = white.a > 0f;
		}
	}
}
