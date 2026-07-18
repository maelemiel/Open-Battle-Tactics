using UnityEngine;

public struct tk2dUITouch
{
	public const int MOUSE_POINTER_FINGER_ID = 9999;

	public TouchPhase phase { get; private set; }

	public int fingerId { get; private set; }

	public Vector2 position { get; private set; }

	public Vector2 deltaPosition { get; private set; }

	public float deltaTime { get; private set; }

	public tk2dUITouch(TouchPhase _phase, int _fingerId, Vector2 _position, Vector2 _deltaPosition, float _deltaTime)
	{
		phase = _phase;
		fingerId = _fingerId;
		position = _position;
		deltaPosition = _deltaPosition;
		deltaTime = _deltaTime;
	}

	public tk2dUITouch(Touch touch)
	{
		phase = touch.phase;
		fingerId = touch.fingerId;
		position = touch.position;
		deltaPosition = deltaPosition;
		deltaTime = deltaTime;
	}

	public override string ToString()
	{
		return string.Concat(phase.ToString(), ",", fingerId, ",", position, ",", deltaPosition, ",", deltaTime);
	}
}
