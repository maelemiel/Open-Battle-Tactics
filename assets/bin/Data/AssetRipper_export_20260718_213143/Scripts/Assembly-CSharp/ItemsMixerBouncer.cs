using System;
using UnityEngine;

public class ItemsMixerBouncer : MonoBehaviour
{
	public float forceAmount = 0.1f;

	public float minAngle = -20f;

	public float maxAngle = 20f;

	private void OnCollisionEnter2D(Collision2D coll)
	{
		if ((bool)coll.rigidbody)
		{
			Vector3 position = coll.transform.position;
			position -= base.transform.position;
			float angle = UnityEngine.Random.Range(minAngle, maxAngle);
			coll.rigidbody.AddForceAtPosition(GetRotatedVector(position, angle) * forceAmount, coll.contacts[0].point);
		}
	}

	private Vector3 GetRotatedVector(Vector3 initialVector, float angle)
	{
		angle *= (float)Math.PI / 180f;
		float num = Mathf.Cos(angle);
		float num2 = Mathf.Sin(angle);
		float x = initialVector.x * num - initialVector.y * num2;
		float y = initialVector.x * num2 + initialVector.y * num;
		return new Vector2(x, y);
	}

	private void OnDrawGizmosSelected()
	{
	}
}
