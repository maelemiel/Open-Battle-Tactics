using System.Collections;
using UnityEngine;

public class tk2dUIDemo3Controller : tk2dUIBaseDemoController
{
	public Transform perspectiveCamera;

	public Transform overlayInterface;

	private Vector3 overlayRestPosition = Vector3.zero;

	public Transform instructions;

	private IEnumerator Start()
	{
		overlayRestPosition = overlayInterface.position;
		HideOverlay();
		Vector3 instructionsRestPos = instructions.position;
		instructions.position += instructions.up * 10f;
		StartCoroutine(coMove(instructions, instructionsRestPos, 1f));
		yield return new WaitForSeconds(3f);
		StartCoroutine(coMove(instructions, instructionsRestPos - instructions.up * 10f, 1f));
	}

	public void ToggleCase(tk2dUIToggleButton button)
	{
		float xAngle = (button.IsOn ? (-66) : 0);
		StartCoroutine(coTweenAngle(button.transform, xAngle, 0.5f));
	}

	private IEnumerator coRedButtonPressed()
	{
		StartCoroutine(coShake(perspectiveCamera, Vector3.one, Vector3.one, 1f));
		yield return new WaitForSeconds(0.3f);
		ShowOverlay();
	}

	private void ShowOverlay()
	{
		overlayInterface.gameObject.SetActive(true);
		Vector3 position = overlayRestPosition;
		position.y = -2.5f;
		overlayInterface.position = position;
		StartCoroutine(coMove(overlayInterface, overlayRestPosition, 0.15f));
	}

	private IEnumerator coHideOverlay()
	{
		Vector3 v = overlayRestPosition;
		v.y = -2.5f;
		yield return StartCoroutine(coMove(overlayInterface, v, 0.15f));
		HideOverlay();
	}

	private void HideOverlay()
	{
		overlayInterface.gameObject.SetActive(false);
	}
}
