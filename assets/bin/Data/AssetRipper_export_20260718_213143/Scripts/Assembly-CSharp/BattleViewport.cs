using UnityEngine;

[ExecuteInEditMode]
public class BattleViewport : MonoBehaviour
{
	private void OnPreCull()
	{
		float height = base.camera.rect.height;
		Rect rect = base.camera.rect;
		rect.height = 1f;
		base.camera.rect = rect;
		Matrix4x4 projectionMatrix = base.camera.projectionMatrix;
		projectionMatrix.m11 *= height / rect.height;
		base.camera.projectionMatrix = projectionMatrix;
	}
}
