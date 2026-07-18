using System.Collections.Generic;
using UnityEngine;

public class UnitPositionSet : MonoBehaviour
{
	public List<Transform> transformList;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		foreach (Transform transform in transformList)
		{
			Gizmos.DrawSphere(transform.position, 25f);
		}
	}
}
