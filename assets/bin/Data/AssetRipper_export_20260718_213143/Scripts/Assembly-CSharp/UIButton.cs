using UnityEngine;

[RequireComponent(typeof(Animation))]
[RequireComponent(typeof(BoxCollider))]
public class UIButton : tk2dButton
{
	private Animation _animation;

	public override void Start()
	{
		base.Start();
		base.ButtonPressedEvent += buttonCallback;
		_animation = GetComponent<Animation>();
	}

	public override void Update()
	{
		if (IsVisibleFromCamera())
		{
			base.Update();
		}
	}

	private bool IsVisibleFromCamera()
	{
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(viewCamera);
		return GeometryUtility.TestPlanesAABB(planes, base.gameObject.collider.bounds);
	}

	private void buttonCallback(tk2dButton source)
	{
		_animation.Play();
	}
}
