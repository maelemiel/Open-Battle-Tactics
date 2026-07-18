using UnityEngine;

public class SceneTransition : MonoBehaviour
{
	public enum Direction
	{
		Out = 0,
		In = 1,
		Both = 2
	}

	public enum Type
	{
		Push = 0,
		Pop = 1,
		Overlay = 2,
		None = 3,
		All = 4
	}

	public Direction allowedDirections = Direction.Both;

	public Type allowedTypes = Type.All;

	public float delayOut;

	public float delayIn;

	public float duration = 0.5f;

	public float timeScale = 1f;

	private float durationScaled;

	private Direction direction;

	private float remainingDelay;

	private float remainingDuration;

	private bool running;

	private SceneController root;

	public void SetSceneRoot(SceneController root)
	{
		this.root = root;
	}

	public void SetDirection(Direction direction)
	{
		this.direction = direction;
	}

	public virtual void PrepareTransition()
	{
		durationScaled = timeScale * duration;
		remainingDuration = durationScaled;
		switch (direction)
		{
		case Direction.Out:
			remainingDelay = timeScale * delayOut;
			UpdateTransition(0f);
			break;
		case Direction.In:
			remainingDelay = timeScale * delayIn;
			UpdateTransition(1f);
			break;
		}
	}

	public virtual void BeginTransition()
	{
		running = true;
	}

	public virtual void EndTransition()
	{
	}

	public void Pump()
	{
		if (!running)
		{
			return;
		}
		if (remainingDelay >= 0f)
		{
			remainingDelay -= Time.deltaTime;
			if (remainingDelay >= 0f)
			{
				return;
			}
			remainingDuration += remainingDelay;
			remainingDelay = 0f;
		}
		else
		{
			remainingDuration -= Time.deltaTime;
		}
		if (remainingDuration <= 0f)
		{
			switch (direction)
			{
			case Direction.Out:
				UpdateTransition(1f);
				break;
			case Direction.In:
				UpdateTransition(0f);
				break;
			}
			running = false;
			root.TransitionDone(this);
			return;
		}
		float num = 0f;
		switch (direction)
		{
		case Direction.Out:
			num = (durationScaled - remainingDuration) / durationScaled;
			break;
		case Direction.In:
			num = remainingDuration / durationScaled;
			break;
		}
		float num2 = num * num;
		float num3 = num2 * num;
		num = -2f * num3 + 3f * num2;
		UpdateTransition(num);
	}

	public virtual void UpdateTransition(float t)
	{
	}
}
