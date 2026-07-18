using UnityEngine;

[RequireComponent(typeof(tk2dBaseSprite))]
public class tk2dSpriteColorAnimator : MonoBehaviour
{
	private tk2dBaseSprite sprite;

	public Color newColor = Color.white;

	private Color previousColor;

	private void Start()
	{
		sprite = GetComponent<tk2dBaseSprite>();
		previousColor = newColor;
	}

	private void Update()
	{
		if (newColor != previousColor)
		{
			previousColor = newColor;
			sprite.color = newColor;
		}
	}
}
