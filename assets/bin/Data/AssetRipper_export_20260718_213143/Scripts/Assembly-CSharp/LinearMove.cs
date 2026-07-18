using System;
using UnityEngine;

public class LinearMove : MonoBehaviour
{
	public float speed = 3f;

	public float offset = 10f;

	public int direction = -1;

	private float initialSpeed;

	private Vector3 initialPosition;

	public float InitialSpeed
	{
		get
		{
			return initialSpeed;
		}
		private set
		{
			initialSpeed = value;
		}
	}

	private void Start()
	{
		InitialSpeed = speed;
		initialPosition = base.transform.localPosition;
		if (direction >= 0)
		{
			direction = 1;
		}
		else
		{
			direction = -1;
		}
	}

	private void Update()
	{
		Vector3 vector = new Vector3(direction, 0f, 0f);
		base.transform.Translate(vector * Time.deltaTime * speed);
		float num = Math.Abs(initialPosition.x - base.transform.localPosition.x);
		if (num >= offset)
		{
			base.transform.localPosition = initialPosition;
		}
	}
}
