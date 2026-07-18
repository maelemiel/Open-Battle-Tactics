using UnityEngine;

public class Shaker : MonoBehaviour
{
	private const float IDEAL_FPS = 60f;

	[SerializeField]
	private BattleController battleController;

	[SerializeField]
	private float viewportShakeScaler = 0.0003f;

	[SerializeField]
	private float viewportShakeThreshold = 2f;

	private float _intensity;

	private Vector3 origin;

	private float _decay;

	private bool isShaking;

	private void Awake()
	{
		origin = base.gameObject.transform.position;
	}

	private void Update()
	{
		if (_intensity > 0f && isShaking)
		{
			base.gameObject.transform.position = origin + new Vector3(Random.Range(0f - _intensity, _intensity), Random.Range(0f - _intensity, _intensity), 0f);
			if (_intensity > viewportShakeThreshold)
			{
				battleController.viewportManager.ViewportCenter = 0.5f + Random.Range(0f - _intensity, _intensity) * viewportShakeScaler;
			}
			_intensity -= _decay * Time.deltaTime * 60f;
		}
		else if (isShaking)
		{
			base.gameObject.transform.position = origin;
			isShaking = false;
		}
	}

	public void Shake(float initialIntensity, float decay)
	{
		_intensity = initialIntensity;
		_decay = decay;
		isShaking = true;
	}
}
