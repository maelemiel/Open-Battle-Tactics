using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class AnimScript_WithDelay : MonoBehaviour
{
	private float delay;

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(delay);
		base.animation.Play();
	}
}
