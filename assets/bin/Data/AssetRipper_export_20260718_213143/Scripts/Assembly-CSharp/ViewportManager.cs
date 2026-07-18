using UnityEngine;

public class ViewportManager : MonoBehaviour
{
	[SerializeField]
	private float viewportCenter = 0.5f;

	[SerializeField]
	private BattleController battleController;

	private float screenOffset;

	private float screenRatio = 1f;

	private Vector3 midlineStartPos;

	private Vector3 maskStartPos;

	public GameObject midline;

	public GameObject separationMasks;

	public float ViewportCenter
	{
		get
		{
			return viewportCenter;
		}
		set
		{
			viewportCenter = value;
			battleController.playerField.UpdateViewportSize(0f, value);
			battleController.enemyField.UpdateViewportSize(value, 1f);
			float num = battleController.mainCamera.NativeResolution.x * value - battleController.mainCamera.NativeResolution.x * 0.5f;
			num += screenOffset * -0.5f + screenOffset * value;
			midline.transform.localPosition = midlineStartPos + new Vector3(num, 0f, 0f);
			separationMasks.transform.localPosition = maskStartPos + new Vector3(num, 0f, 0f);
		}
	}

	private void Awake()
	{
		midlineStartPos = midline.transform.localPosition;
		maskStartPos = separationMasks.transform.localPosition;
		tk2dCamera mainCamera = battleController.mainCamera;
		float x = mainCamera.TargetResolution.x;
		float num = mainCamera.nativeResolutionWidth;
		screenRatio = Mathf.Min(mainCamera.TargetResolution.y / (float)mainCamera.nativeResolutionHeight, mainCamera.TargetResolution.x / (float)mainCamera.nativeResolutionWidth);
		float num2 = num * screenRatio;
		screenOffset = x - num2;
	}

	[ContextMenu("Force Update")]
	private void ForceViewportUpdate()
	{
		ViewportCenter = ViewportCenter;
	}
}
