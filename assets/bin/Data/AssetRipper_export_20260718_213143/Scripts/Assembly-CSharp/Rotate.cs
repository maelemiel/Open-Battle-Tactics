using UnityEngine;

public class Rotate : MonoBehaviour
{
	public Vector3 rotationOffset;

	private Transform cachedTransform;

	private void Awake()
	{
		cachedTransform = base.transform;
	}

	private void Update()
	{
		cachedTransform.Rotate(rotationOffset * Time.deltaTime);
	}
}
