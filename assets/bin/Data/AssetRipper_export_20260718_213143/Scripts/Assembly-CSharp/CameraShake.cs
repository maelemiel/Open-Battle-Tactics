using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public new Camera camera;

	private Vector3 camOrigin;

	public float initialIntensity;

	private float _intensity;

	public float decay;

	public bool useOwnTransform;

	public float Intensity
	{
		get
		{
			return _intensity;
		}
	}

	private void Awake()
	{
		if (useOwnTransform)
		{
			camOrigin = base.transform.position;
		}
		else if (!camera)
		{
			Debug.LogError("No camera assigned: CameraShake on " + base.name);
		}
		else
		{
			camOrigin = camera.transform.position;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			base.animation.Play();
		}
		if (useOwnTransform)
		{
			if (_intensity > 0f)
			{
				base.transform.position = camOrigin + new Vector3(Random.Range(0f - _intensity, _intensity), Random.Range(0f - _intensity, _intensity), 0f);
				_intensity -= decay;
			}
		}
		else if (_intensity > 0f)
		{
			camera.transform.position = camOrigin + new Vector3(Random.Range(0f - _intensity, _intensity), Random.Range(0f - _intensity, _intensity), 0f);
			_intensity -= decay;
		}
	}

	public void Shake()
	{
		_intensity = initialIntensity;
	}
}
