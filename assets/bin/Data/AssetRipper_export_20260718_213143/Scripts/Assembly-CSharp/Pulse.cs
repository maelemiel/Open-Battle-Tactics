using UnityEngine;

public class Pulse : MonoBehaviour
{
	public float minScale = 0.5f;

	public float maxScale = 1f;

	public float speed = 1f;

	private Transform cachedTransform;

	private void Awake()
	{
		cachedTransform = base.transform;
	}

	private void Update()
	{
		cachedTransform.localScale = Vector3.one * Mathf.Max(minScale, Mathf.PingPong(Time.time * speed, maxScale));
	}
}
