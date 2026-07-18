using UnityEngine;

public class PulseAlphaText : MonoBehaviour
{
	public float minScale = 0.5f;

	public float maxScale = 1f;

	public float speed = 1f;

	private tk2dTextMesh cachedTextMesh;

	private void Awake()
	{
		cachedTextMesh = GetComponent<tk2dTextMesh>();
	}

	private void Update()
	{
		cachedTextMesh.Alpha = 1f * Mathf.Max(minScale, Mathf.PingPong(Time.time * speed, maxScale));
	}
}
