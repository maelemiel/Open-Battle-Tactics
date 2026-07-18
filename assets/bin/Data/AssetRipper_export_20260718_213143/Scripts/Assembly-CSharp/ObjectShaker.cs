using UnityEngine;

public class ObjectShaker : MonoBehaviour
{
	public Transform shakeTarget;

	private Vector3 origin;

	public float initialIntensity;

	private float _intensity;

	public float decay;

	private void Start()
	{
		origin = shakeTarget.position;
	}

	private void Update()
	{
		if (_intensity > 0f)
		{
			shakeTarget.position = origin + new Vector3(Random.Range(0f - _intensity, _intensity), Random.Range(0f - _intensity, _intensity), 0f);
			_intensity -= decay;
		}
	}

	public void Shake(bool resetPosition = false)
	{
		if (resetPosition)
		{
			origin = shakeTarget.position;
		}
		_intensity = initialIntensity;
	}
}
