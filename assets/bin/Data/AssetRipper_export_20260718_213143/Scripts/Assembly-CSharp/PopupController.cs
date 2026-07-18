using System.Collections;
using UnityEngine;

public class PopupController : MonoBehaviour
{
	[SerializeField]
	protected tk2dTextMesh _title;

	[SerializeField]
	protected tk2dTextMesh _message;

	[SerializeField]
	protected float _destroyAfterSeconds;

	[SerializeField]
	protected GameObject _closeButton;

	[SerializeField]
	protected GameObject _leftButton;

	[SerializeField]
	protected tk2dTextMesh _leftLabel;

	[SerializeField]
	protected GameObject _rightButton;

	[SerializeField]
	protected tk2dTextMesh _rightLabel;

	[SerializeField]
	protected bool allowBackButton;

	public bool behindTopBar;

	protected PopupDataModel model;

	protected virtual void Awake()
	{
		if (PopupManager.PopupCount != 0)
		{
			model = PopupManager.CurrentPopupDM;
		}
		if (model == null)
		{
			model = new PopupDataModel
			{
				id = 1
			};
		}
		else
		{
			PopupManager.RegisterPopupController(this);
		}
		model.controller = this;
		AdjustCameraAndPosition();
	}

	protected virtual void Start()
	{
		if (_destroyAfterSeconds > 0f)
		{
			StartCoroutine(DestroyAfterSeconds());
		}
		if (_title != null)
		{
			_title.text = (string.IsNullOrEmpty(model.title) ? _title.text : model.title);
		}
		if (_message != null)
		{
			_message.text = (string.IsNullOrEmpty(model.message) ? _message.text : model.message);
		}
		if (_leftLabel != null)
		{
			_leftLabel.text = (string.IsNullOrEmpty(model.leftLabel) ? _leftLabel.text : model.leftLabel);
		}
		if (_rightLabel != null)
		{
			_rightLabel.text = (string.IsNullOrEmpty(model.rightLabel) ? _rightLabel.text : model.rightLabel);
		}
		if (_leftButton != null && string.IsNullOrEmpty(model.leftLabel))
		{
			_leftButton.SetActive(false);
			_rightButton.transform.localPosition = new Vector3(0f, _rightButton.transform.localPosition.y, _rightButton.transform.localPosition.z);
		}
		if (_closeButton != null && !model.showCloseButton)
		{
			_closeButton.SetActive(false);
		}
	}

	protected virtual void Update()
	{
	}

	public virtual void OnBackButtonPressed()
	{
		if (allowBackButton && model.showCloseButton && !LoadingPopupManager.ShouldBlockInput)
		{
			OnCloseButton();
		}
	}

	public virtual void OnLeftButton()
	{
		if (_leftButton.activeInHierarchy)
		{
			if (model.destroyWhenClicked)
			{
				PopupManager.DestroyPopup(model);
			}
			if (model.leftAction != null)
			{
				model.leftAction();
			}
		}
	}

	public virtual void OnRightButton()
	{
		if (model.destroyWhenClicked)
		{
			PopupManager.DestroyPopup(model);
		}
		PopupManager.CallRightActionObject(model.rightActionObject, model);
		PopupManager.CallRightAction(model.rightAction, model);
	}

	public virtual void OnCloseButton()
	{
		Close();
		if (model.closeButtonAction != null)
		{
			model.closeButtonAction();
		}
	}

	public virtual void Close()
	{
		PopupManager.DestroyPopup(model);
	}

	protected virtual void AdjustCameraAndPosition()
	{
		float num = 25f;
		Camera componentInChildren = base.transform.GetComponentInChildren<Camera>();
		if ((bool)TopBarController.instance && !behindTopBar)
		{
			num = TopBarController.instance.TopBarCamera.ScreenCamera.depth + 10f;
			componentInChildren.depth = (float)model.id + num;
		}
		if (PopupManager.highDepThPopup)
		{
			componentInChildren.depth = 101f;
			PopupManager.highDepThPopup = false;
		}
		base.transform.AddLocalPosition(new Vector3(0f, 0f, -model.id * 1000));
	}

	public virtual void Dispose()
	{
		if (base.gameObject != null)
		{
			Object.Destroy(base.gameObject);
		}
		PopupManager.RegisterPopupDisposed();
	}

	private IEnumerator DestroyAfterSeconds()
	{
		yield return new WaitForSeconds(_destroyAfterSeconds);
		PopupManager.DestroyPopup(model);
	}
}
