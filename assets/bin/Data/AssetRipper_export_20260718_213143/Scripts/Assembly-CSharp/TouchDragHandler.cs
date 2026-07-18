using UnityEngine;

public abstract class TouchDragHandler : MonoBehaviour
{
	public abstract void OnTap();

	public abstract void OnDragStarted();

	public abstract void OnDragUpdate(Vector2 touchPosition);

	public abstract void OnDragEnded();
}
