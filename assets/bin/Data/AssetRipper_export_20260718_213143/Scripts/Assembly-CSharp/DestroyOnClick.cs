using UnityEngine;

public class DestroyOnClick : MonoBehaviour
{
	private void OnMouseDown()
	{
		Object.Destroy(base.gameObject);
	}
}
