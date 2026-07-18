using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
	[SerializeField]
	private GameObject _prefab;

	[SerializeField]
	private int _capacity;

	[SerializeField]
	private bool _manualInit;

	private Queue<GameObject> _objectPool;

	private void Start()
	{
		if (!_manualInit)
		{
			Init();
		}
	}

	public void Init()
	{
		_objectPool = new Queue<GameObject>(_capacity);
		for (int i = 0; i < _capacity; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(_prefab);
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.name = "-";
			gameObject.SetActive(false);
			_objectPool.Enqueue(gameObject);
		}
	}

	public void Init(int capacity)
	{
		_capacity = capacity;
		Init();
	}

	public GameObject GetGameObject()
	{
		return _objectPool.Dequeue();
	}

	public void Release(GameObject go)
	{
		go.name = "-";
		go.SetActive(false);
		_objectPool.Enqueue(go);
	}

	public void DestroyPool()
	{
		while (_objectPool.Count > 0)
		{
			Object.Destroy(_objectPool.Dequeue());
		}
	}
}
