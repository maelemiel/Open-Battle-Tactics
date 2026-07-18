using UnityEngine;

public class ArrowsEffect : MonoBehaviour
{
	[SerializeField]
	private Color initialColor;

	[SerializeField]
	private Color finalColor;

	public tk2dSprite[] arrowSprites;

	private float currentPercentage;

	public float speed = 7f;

	public float period = 0.35f;

	private void UpdateColors()
	{
		for (int i = 0; i < arrowSprites.Length; i++)
		{
			arrowSprites[i].color = Color.Lerp(initialColor, finalColor, (Mathf.Sin((float)i * period + currentPercentage) + 1f) * 0.5f);
		}
	}

	private void Update()
	{
		currentPercentage += (0f - Time.deltaTime) * speed;
		UpdateColors();
	}
}
