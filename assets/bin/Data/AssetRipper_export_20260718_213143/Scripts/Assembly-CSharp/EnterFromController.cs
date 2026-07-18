using System.Collections;
using UnityEngine;

public class EnterFromController : MonoBehaviour
{
	[SerializeField]
	private bool _autoStart;

	[SerializeField]
	private float _delayStart;

	[SerializeField]
	private EnterPosition _enterFrom = EnterPosition.Top;

	[SerializeField]
	private int _distanceMovement;

	private Vector3 _initialPosition;

	private void Awake()
	{
		_initialPosition = base.gameObject.transform.localPosition;
		Vector3 localPosition = _initialPosition;
		switch (_enterFrom)
		{
		case EnterPosition.Top:
			localPosition = new Vector3(localPosition.x, localPosition.y + (float)_distanceMovement, localPosition.z);
			break;
		case EnterPosition.Left:
			localPosition = new Vector3(localPosition.x - (float)_distanceMovement, localPosition.y, localPosition.z);
			break;
		case EnterPosition.Right:
			localPosition = new Vector3(localPosition.x + (float)_distanceMovement, localPosition.y, localPosition.z);
			break;
		case EnterPosition.Bottom:
			localPosition = new Vector3(localPosition.x, localPosition.y - (float)_distanceMovement, localPosition.z);
			break;
		}
		base.gameObject.transform.localPosition = localPosition;
	}

	private void Start()
	{
		if (_autoStart)
		{
			StartCoroutine(Init());
		}
	}

	public IEnumerator Init()
	{
		yield return new WaitForSeconds(_delayStart);
		base.gameObject.transform.TweenLocalPosition(_initialPosition, 0.5f);
		yield return new WaitForSeconds(0.5f);
	}
}
