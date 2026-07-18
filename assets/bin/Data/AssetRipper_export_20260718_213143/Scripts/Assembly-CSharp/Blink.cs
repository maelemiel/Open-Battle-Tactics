using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Blink : MonoBehaviour
{
	public float period = 0.5f;

	private bool state;

	private Renderer myRenderer;

	private void Awake()
	{
		myRenderer = base.renderer;
	}

	private void OnEnable()
	{
		StartCoroutine(StartBlink());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator StartBlink()
	{
		while (true)
		{
			state = !state;
			myRenderer.enabled = state;
			yield return new WaitForSeconds(period);
		}
	}
}
