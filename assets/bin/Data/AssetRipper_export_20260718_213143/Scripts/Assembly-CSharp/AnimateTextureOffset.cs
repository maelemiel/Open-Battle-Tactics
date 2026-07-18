using UnityEngine;

public class AnimateTextureOffset : MonoBehaviour
{
	[SerializeField]
	private Vector2 velocityOffset;

	private Renderer myRenderer;

	[SerializeField]
	private Vector2 currentOffset;

	private void Awake()
	{
		myRenderer = base.renderer;
	}

	private void Update()
	{
		currentOffset += velocityOffset * Time.deltaTime;
		currentOffset.x %= 1f;
		currentOffset.y %= 1f;
		myRenderer.material.mainTextureOffset = currentOffset;
	}

	public void SetHorizontalSpeed(float speed)
	{
		velocityOffset.x = speed;
	}

	public void SetVerticalSpeed(float speed)
	{
		velocityOffset.y = speed;
	}

	public void Stop()
	{
		velocityOffset = Vector2.zero;
	}
}
