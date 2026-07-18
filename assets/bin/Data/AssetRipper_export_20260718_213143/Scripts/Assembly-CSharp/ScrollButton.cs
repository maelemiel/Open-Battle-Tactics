using Holoville.HOTween;
using UnityEngine;

public class ScrollButton : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite openButtonGameObject;

	[SerializeField]
	private tk2dSprite closeButtonGameObject;

	[SerializeField]
	private tk2dUIItem openButton;

	[SerializeField]
	private LocateObjectTypes openDirection;

	[SerializeField]
	private float movementOffset = 125f;

	private Vector3 initialPosition;

	private Vector3 finalPosition;

	private bool state;

	public float openCloseTime = 0.25f;

	public bool IsOpen
	{
		get
		{
			return state;
		}
	}

	private void Awake()
	{
		initialPosition = base.transform.localPosition;
		Vector3 zero = Vector3.zero;
		switch (openDirection)
		{
		case LocateObjectTypes.HORIZONTAL:
		case LocateObjectTypes.HORIZONTAL_AND_VERTICAL:
			zero.x = movementOffset;
			break;
		case LocateObjectTypes.VERTICAL:
			zero.y = movementOffset;
			break;
		}
		finalPosition = initialPosition + zero;
		CloseButton();
	}

	public void ToggleButton()
	{
		if (state)
		{
			CloseButton();
		}
		else
		{
			OpenButton();
		}
	}

	private void OpenButton()
	{
		if (!state)
		{
			state = true;
			SetButtonState(false);
			Sequence sequence = new Sequence();
			sequence.Append(HOTween.To(base.transform, openCloseTime, new TweenParms().Prop("localPosition", finalPosition).Ease(EaseType.Linear).OnComplete(OnButtonEndsMovement)));
			sequence.Play();
		}
	}

	private void CloseButton()
	{
		if (state)
		{
			state = false;
			SetButtonState(false);
			Sequence sequence = new Sequence();
			sequence.Append(HOTween.To(base.transform, openCloseTime, new TweenParms().Prop("localPosition", initialPosition).Ease(EaseType.Linear).OnComplete(OnButtonEndsMovement)));
			sequence.Play();
		}
	}

	private void SetButtonState(bool state)
	{
		if ((bool)openButton)
		{
			openButton.enabled = state;
		}
	}

	private void SetButtonGraphicState(bool state)
	{
		openButtonGameObject.gameObject.SetActive(!state);
		closeButtonGameObject.gameObject.SetActive(state);
	}

	private void OnButtonEndsMovement()
	{
		SetButtonState(true);
		SetButtonGraphicState(state);
	}

	private void OnButtonClicked()
	{
		ToggleButton();
	}
}
