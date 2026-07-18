using UnityEngine;

public class TestFitBounds : MonoBehaviour
{
	public GameObject fitObject;

	private void Start()
	{
		fitObject.FitWithinBounds(base.collider.bounds);
	}

	private void Update()
	{
	}
}
