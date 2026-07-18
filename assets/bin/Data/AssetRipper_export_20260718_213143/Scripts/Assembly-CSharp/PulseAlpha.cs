using UnityEngine;

public class PulseAlpha : MonoBehaviour
{
	public float minScale = 0.5f;

	public float maxScale = 1f;

	public float speed = 1f;

	private tk2dBaseSprite cachedSprite;

	private void Awake()
	{
		cachedSprite = GetComponent<tk2dBaseSprite>();
	}

	private void Update()
	{
		cachedSprite.Alpha = 1f * Mathf.Max(minScale, Mathf.PingPong(Time.time * speed, maxScale));
	}
}
