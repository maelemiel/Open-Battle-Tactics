using UnityEngine;

public class FirstUnitSelectButton : MonoBehaviour
{
	[SerializeField]
	private GameObject _selectedEffect;

	[SerializeField]
	private tk2dTextMesh _textMesh;

	public void Init(string name)
	{
		_textMesh.text = name;
		_textMesh.gameObject.SetActive(false);
	}

	public void SetEffect(bool status)
	{
		if (!status)
		{
			_selectedEffect.GetComponent<tk2dSpineAnimation>().Reset();
		}
		_selectedEffect.SetActive(status);
		_textMesh.gameObject.SetActive(status);
	}

	private void OnDisable()
	{
		_selectedEffect.SetActive(false);
		_textMesh.gameObject.SetActive(false);
	}
}
