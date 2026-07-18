using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
	[SerializeField]
	private tk2dCamera battlefieldCamera;

	public GameObject background;

	public Shaker shaker;

	public float directionScaler = 1f;

	public List<UnitView> unitViews;

	public bool isEnemy;

	public float leftViewportOffset;

	public float rightViewportOffset = 30f;

	[HideInInspector]
	public Camera unityCamera;

	private void Start()
	{
		unityCamera = battlefieldCamera.camera;
	}

	public IEnumerable<tk2dBaseSprite> GetSprites()
	{
		return background.GetComponentsInChildren<tk2dBaseSprite>();
	}

	public void UpdateViewportSize(float startXOffset, float endXOffset)
	{
		tk2dCamera tk2dCamera2 = ((!(battlefieldCamera.InheritConfig != null)) ? battlefieldCamera : battlefieldCamera.InheritConfig);
		float x = tk2dCamera2.TargetResolution.x;
		float num = tk2dCamera2.nativeResolutionWidth;
		float num2 = num * startXOffset + leftViewportOffset;
		float num3 = num * endXOffset + rightViewportOffset;
		float num4 = Mathf.Min(tk2dCamera2.TargetResolution.y / (float)tk2dCamera2.nativeResolutionHeight, tk2dCamera2.TargetResolution.x / (float)tk2dCamera2.nativeResolutionWidth);
		float num5 = num * num4;
		float num6 = x - num5;
		num3 -= num2;
		num2 -= num6 * 0.5f;
		num2 += num6 * startXOffset;
		num3 += num6 * endXOffset;
		num3 = Mathf.Max((0f - num6) * 0.5f, num2 + num3) - num2;
		num3 = Mathf.Min(num + num6, num2 + num3) - num2;
		num2 = Mathf.Max((0f - num6) * 0.5f, num2);
		num2 = Mathf.Min(num + num6, num2);
		if (startXOffset >= endXOffset)
		{
			battlefieldCamera.ScreenCamera.enabled = false;
		}
		else
		{
			battlefieldCamera.ScreenCamera.enabled = true;
		}
		battlefieldCamera.viewportRegion.x = num2;
		battlefieldCamera.viewportRegion.z = num3;
	}
}
