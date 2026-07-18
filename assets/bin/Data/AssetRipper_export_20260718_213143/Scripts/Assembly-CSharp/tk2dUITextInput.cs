using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/UI/tk2dUITextInput")]
public class tk2dUITextInput : MonoBehaviour
{
	public tk2dUIItem selectionBtn;

	public tk2dTextMesh inputLabel;

	public tk2dTextMesh emptyDisplayLabel;

	public GameObject unSelectedStateGO;

	public GameObject selectedStateGO;

	public GameObject cursor;

	public float fieldLength = 1f;

	public int maxCharacterLength = 30;

	public string emptyDisplayText;

	public string emptyDisplayTextLocKey;

	public bool isPasswordField;

	public string passwordChar = "*";

	[HideInInspector]
	[SerializeField]
	private tk2dUILayout layoutItem;

	private bool isSelected;

	private bool wasStartedCalled;

	private bool wasOnAnyPressEventAttached;

	private TouchScreenKeyboard keyboard;

	private bool listenForKeyboardText;

	private bool isDisplayTextShown;

	public Action<tk2dUITextInput> OnTextChange;

	public string SendMessageOnTextChangeMethodName = string.Empty;

	public Action OnTextChangeReady;

	public Action OnCursorChange;

	private string text = string.Empty;

	public tk2dUILayout LayoutItem
	{
		get
		{
			return layoutItem;
		}
		set
		{
			if (layoutItem != value)
			{
				if (layoutItem != null)
				{
					layoutItem.OnReshape -= LayoutReshaped;
				}
				layoutItem = value;
				if (layoutItem != null)
				{
					layoutItem.OnReshape += LayoutReshaped;
				}
			}
		}
	}

	public GameObject SendMessageTarget
	{
		get
		{
			if (selectionBtn != null)
			{
				return selectionBtn.sendMessageTarget;
			}
			return null;
		}
		set
		{
			if (selectionBtn != null && selectionBtn.sendMessageTarget != value)
			{
				selectionBtn.sendMessageTarget = value;
			}
		}
	}

	public bool IsFocus
	{
		get
		{
			return isSelected;
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (text != value)
			{
				text = value;
				if (text.Length > maxCharacterLength)
				{
					text = text.Substring(0, maxCharacterLength);
				}
				FormatTextForDisplay(text);
				if (isSelected)
				{
					SetCursorPosition();
				}
				if (OnTextChangeReady != null)
				{
					OnTextChangeReady();
				}
			}
		}
	}

	private void Awake()
	{
		SetState();
		ShowDisplayText();
	}

	private void Start()
	{
		base.gameObject.AddComponent<UnicodeInput>();
		wasStartedCalled = true;
		if (tk2dUIManager.Instance__NoCreate != null)
		{
			tk2dUIManager.Instance.OnAnyPress += AnyPress;
		}
		wasOnAnyPressEventAttached = true;
	}

	private void OnEnable()
	{
		if (wasStartedCalled && !wasOnAnyPressEventAttached && tk2dUIManager.Instance__NoCreate != null)
		{
			tk2dUIManager.Instance.OnAnyPress += AnyPress;
		}
		if (layoutItem != null)
		{
			layoutItem.OnReshape += LayoutReshaped;
		}
		selectionBtn.OnClick += InputSelected;
	}

	private void OnDisable()
	{
		if (tk2dUIManager.Instance__NoCreate != null)
		{
			tk2dUIManager.Instance.OnAnyPress -= AnyPress;
			if (listenForKeyboardText)
			{
				tk2dUIManager.Instance.OnInputUpdate -= ListenForKeyboardTextUpdate;
			}
		}
		wasOnAnyPressEventAttached = false;
		selectionBtn.OnClick -= InputSelected;
		listenForKeyboardText = false;
		if (layoutItem != null)
		{
			layoutItem.OnReshape -= LayoutReshaped;
		}
	}

	public void SetFocus()
	{
		if (!IsFocus)
		{
			InputSelected();
		}
	}

	private void FormatTextForDisplay(string modifiedText)
	{
		if (isPasswordField)
		{
			int length = modifiedText.Length;
			char paddingChar = ((passwordChar.Length <= 0) ? '*' : passwordChar[0]);
			modifiedText = string.Empty;
			modifiedText = modifiedText.PadRight(length, paddingChar);
		}
		inputLabel.text = modifiedText;
		inputLabel.Commit();
		while (inputLabel.renderer.bounds.extents.x * 2f > fieldLength)
		{
			modifiedText = modifiedText.Substring(1, modifiedText.Length - 1);
			inputLabel.text = modifiedText;
			inputLabel.Commit();
		}
		if (modifiedText.Length == 0 && !listenForKeyboardText)
		{
			ShowDisplayText();
		}
		else
		{
			HideDisplayText();
		}
	}

	private void ListenForKeyboardTextUpdate()
	{
		bool flag = false;
		string text = this.text;
		string inputString = Input.inputString;
		foreach (char c in inputString)
		{
			if (c == "\b"[0])
			{
				if (this.text.Length != 0)
				{
					text = this.text.Substring(0, this.text.Length - 1);
					flag = true;
				}
			}
			else if (c != "\n"[0] && c != "\r"[0] && c != '\t' && c != '\u001b')
			{
				text += c;
				flag = true;
			}
		}
		if (flag)
		{
			Text = text;
			if (OnTextChange != null)
			{
				OnTextChange(this);
			}
			if (SendMessageTarget != null && SendMessageOnTextChangeMethodName.Length > 0)
			{
				SendMessageTarget.SendMessage(SendMessageOnTextChangeMethodName, this, SendMessageOptions.RequireReceiver);
			}
		}
	}

	private void InputSelected()
	{
		if (text.Length == 0)
		{
			HideDisplayText();
		}
		isSelected = true;
		if (!listenForKeyboardText)
		{
			tk2dUIManager.Instance.OnInputUpdate += ListenForKeyboardTextUpdate;
		}
		listenForKeyboardText = true;
		SetState();
		SetCursorPosition();
		if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
		{
			TouchScreenKeyboard.hideInput = false;
			keyboard = TouchScreenKeyboard.Open(text, TouchScreenKeyboardType.ASCIICapable, false, false, false, false);
			StartCoroutine(TouchScreenKeyboardLoop());
		}
	}

	private IEnumerator TouchScreenKeyboardLoop()
	{
		while (keyboard != null && !keyboard.done && keyboard.active)
		{
			Text = keyboard.text;
			yield return null;
		}
		if (keyboard != null)
		{
			Text = keyboard.text;
		}
		if (isSelected)
		{
			InputDeselected();
		}
	}

	private void InputDeselected()
	{
		if (text.Length == 0)
		{
			ShowDisplayText();
		}
		isSelected = false;
		if (listenForKeyboardText)
		{
			tk2dUIManager.Instance.OnInputUpdate -= ListenForKeyboardTextUpdate;
		}
		listenForKeyboardText = false;
		SetState();
		if (keyboard != null && !keyboard.done)
		{
			keyboard.active = false;
		}
		keyboard = null;
	}

	private void AnyPress()
	{
		if (isSelected && tk2dUIManager.Instance.PressedUIItem != selectionBtn)
		{
			InputDeselected();
		}
	}

	private void SetState()
	{
		tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(unSelectedStateGO, !isSelected);
		tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(selectedStateGO, isSelected);
		tk2dUIBaseItemControl.ChangeGameObjectActiveState(cursor, isSelected);
	}

	private void SetCursorPosition()
	{
		float num = 1f;
		float num2 = 0.002f;
		if (inputLabel.anchor == TextAnchor.MiddleLeft || inputLabel.anchor == TextAnchor.LowerLeft || inputLabel.anchor == TextAnchor.UpperLeft)
		{
			num = 2f;
		}
		else if (inputLabel.anchor == TextAnchor.MiddleRight || inputLabel.anchor == TextAnchor.LowerRight || inputLabel.anchor == TextAnchor.UpperRight)
		{
			num = -2f;
			num2 = 0.012f;
		}
		if (text.EndsWith(" "))
		{
			tk2dFontChar tk2dFontChar2 = ((!inputLabel.font.useDictionary) ? inputLabel.font.chars[32] : inputLabel.font.charDict[32]);
			num2 += tk2dFontChar2.advance * inputLabel.scale.x / 2f;
		}
		cursor.transform.localPosition = new Vector3(inputLabel.transform.localPosition.x + (inputLabel.renderer.bounds.extents.x + num2) * num, cursor.transform.localPosition.y, cursor.transform.localPosition.z);
		if (OnCursorChange != null)
		{
			OnCursorChange();
		}
	}

	private void ShowDisplayText()
	{
		if (isDisplayTextShown)
		{
			return;
		}
		isDisplayTextShown = true;
		if (emptyDisplayLabel != null)
		{
			if (!string.IsNullOrEmpty(emptyDisplayTextLocKey))
			{
				emptyDisplayLabel.text = emptyDisplayTextLocKey.Localize(emptyDisplayText);
			}
			tk2dUIBaseItemControl.ChangeGameObjectActiveState(emptyDisplayLabel.gameObject, true);
		}
		tk2dUIBaseItemControl.ChangeGameObjectActiveState(inputLabel.gameObject, false);
	}

	private void HideDisplayText()
	{
		if (isDisplayTextShown)
		{
			isDisplayTextShown = false;
			tk2dUIBaseItemControl.ChangeGameObjectActiveStateWithNullCheck(emptyDisplayLabel.gameObject, false);
			tk2dUIBaseItemControl.ChangeGameObjectActiveState(inputLabel.gameObject, true);
		}
	}

	private void LayoutReshaped(Vector3 dMin, Vector3 dMax)
	{
		fieldLength += dMax.x - dMin.x;
		string text = this.text;
		this.text = string.Empty;
		Text = text;
	}

	public TouchScreenKeyboard KeyboardClone()
	{
		return keyboard;
	}
}
