using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class AspectRatioEnforcer : MonoBehaviour
{
	private static GameObject _cameraBoxing;

	private void Awake()
	{
		if (Application.isPlaying && _cameraBoxing == null)
		{
			_cameraBoxing = new GameObject("-Camera Boxing");
			_cameraBoxing.AddComponent("Camera");
			Camera camera = (Camera)_cameraBoxing.GetComponent("Camera");
			camera.depth = -100f;
			camera.cullingMask = 0;
			camera.backgroundColor = Color.black;
		}
	}

	private void Start()
	{
		float num = 2f / 3f;
		float num2 = (float)Screen.width / (float)Screen.height;
		float num3 = num2 / num;
		Camera component = GetComponent<Camera>();
		if (num3 < 1f)
		{
			Rect rect = component.rect;
			rect.width = 1f;
			rect.height = num3;
			rect.x = 0f;
			rect.y = (1f - num3) / 2f;
			component.rect = rect;
		}
		else
		{
			float num4 = 1f / num3;
			Rect rect2 = component.rect;
			rect2.width = num4;
			rect2.height = 1f;
			rect2.x = (1f - num4) / 2f;
			rect2.y = 0f;
			component.rect = rect2;
		}
	}
}
