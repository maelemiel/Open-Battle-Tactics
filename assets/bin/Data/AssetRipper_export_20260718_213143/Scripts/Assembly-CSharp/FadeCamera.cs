using System.Collections;
using UnityEngine;

public class FadeCamera : MonoBehaviour
{
	public float time = 0.9f;

	private Camera cam1;

	private Texture tex;

	private RenderTexture renderTex;

	private bool useCurve;

	private AnimationCurve curve;

	private float alpha;

	private void Start()
	{
		cam1 = GetComponent<Camera>();
	}

	public IEnumerator BeginCameraTransition()
	{
		if (!renderTex)
		{
			renderTex = new RenderTexture(Screen.width, Screen.height, 24);
		}
		cam1.targetTexture = renderTex;
		tex = renderTex;
		CameraSetup();
		yield return StartCoroutine(AlphaTimer());
		cam1.targetTexture = null;
		renderTex.Release();
		base.enabled = false;
	}

	private void CameraSetup()
	{
		base.enabled = true;
	}

	private IEnumerator AlphaTimer()
	{
		float rate = 1f / time;
		if (useCurve)
		{
			float t = 0f;
			while (t < 1f)
			{
				alpha = curve.Evaluate(t);
				t += Time.deltaTime * rate;
				yield return 0;
			}
		}
		else
		{
			for (alpha = 1f; alpha > 0f; alpha -= Time.deltaTime * rate)
			{
				yield return 0;
			}
		}
	}

	private void OnGUI()
	{
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		if ((bool)tex)
		{
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), tex);
		}
	}
}
