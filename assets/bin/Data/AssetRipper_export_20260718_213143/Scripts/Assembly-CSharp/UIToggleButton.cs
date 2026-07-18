using UnityEngine;

public class UIToggleButton : UIButton
{
	[SerializeField]
	private GameObject _selectedSprite;

	[SerializeField]
	private GameObject _unselectedSprite;

	private bool _selected;

	public bool Selected
	{
		get
		{
			return _selected;
		}
	}

	public override void Start()
	{
		base.Start();
		base.ButtonPressedEvent += buttonCallback;
	}

	public void SelectButton(bool selected = false)
	{
		_selected = selected;
		ChangeSelectedImage(selected);
	}

	private void ChangeSelectedImage(bool selected)
	{
		if (selected)
		{
			_selectedSprite.SetActive(true);
			_unselectedSprite.SetActive(false);
		}
		else
		{
			_selectedSprite.SetActive(false);
			_unselectedSprite.SetActive(true);
		}
	}

	private void buttonCallback(tk2dButton source)
	{
		_selected = !_selected;
		ChangeSelectedImage(_selected);
	}
}
