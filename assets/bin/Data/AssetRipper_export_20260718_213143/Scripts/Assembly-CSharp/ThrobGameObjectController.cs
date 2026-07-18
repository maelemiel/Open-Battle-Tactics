using System.Collections;
using UnityEngine;

public class ThrobGameObjectController : MonoBehaviour
{
	[SerializeField]
	private bool _autoInit = true;

	[SerializeField]
	private float _timeFactor = 1f;

	[SerializeField]
	private float _deltaScale;

	private void Start()
	{
		if (_autoInit)
		{
			StartCoroutine(InitThrob());
		}
	}

	public IEnumerator InitThrob()
	{
		if (_deltaScale != 0f && _timeFactor != 0f)
		{
			while (true)
			{
				float scale = Mathf.Min(1f + _deltaScale, Mathf.PingPong(Time.time / _timeFactor, _deltaScale) + 1f);
				base.transform.localScale = new Vector3(scale, scale);
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
