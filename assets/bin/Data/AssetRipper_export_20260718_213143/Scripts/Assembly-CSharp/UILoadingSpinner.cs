using UnityEngine;

public class UILoadingSpinner : MonoBehaviour
{
	public float factor = 500f;

	private void Update()
	{
		base.transform.Rotate(Vector3.back * Time.deltaTime * factor);
	}
}
