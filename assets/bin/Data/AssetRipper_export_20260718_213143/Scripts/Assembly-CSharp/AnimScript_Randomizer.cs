using UnityEngine;

public class AnimScript_Randomizer : MonoBehaviour
{
	public float minDuration = 0.5f;

	public float maxDuration = 1f;

	private float timer;

	public bool playAutomatically = true;

	private void Start()
	{
		timer = Random.Range(minDuration, maxDuration);
		if (playAutomatically)
		{
			base.animation.Play();
		}
	}

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			Reset();
			base.animation.Play();
		}
	}

	private void Reset()
	{
		timer = Random.Range(minDuration, maxDuration);
	}
}
