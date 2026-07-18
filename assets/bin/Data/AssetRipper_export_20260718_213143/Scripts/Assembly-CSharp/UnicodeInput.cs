using System;
using System.Collections;
using UnityEngine;

public class UnicodeInput : MonoBehaviour
{
	private TextMeshController _textMeshController;

	private tk2dUITextInput _tk2dInput;

	private Transform _cursor;

	private void Awake()
	{
		_tk2dInput = GetComponent<tk2dUITextInput>();
		StartCoroutine(SetValues());
	}

	private IEnumerator SetValues()
	{
		_cursor = _tk2dInput.cursor.transform;
		yield return StartCoroutine(SetTextMeshController());
		_textMeshController.OnTextMeshSwitch += CheckIfUnicode;
	}

	private IEnumerator SetTextMeshController()
	{
		while (_tk2dInput.inputLabel.textMeshController == null)
		{
			yield return 0;
		}
		_textMeshController = _tk2dInput.inputLabel.textMeshController;
	}

	private void CheckIfUnicode(bool isUnicode)
	{
		if (isUnicode)
		{
			_textMeshController.ucTextMesh.orderInLayer = _tk2dInput.inputLabel.SortingOrder + 1;
			FixUcTextPosition();
			tk2dUITextInput tk2dInput = _tk2dInput;
			tk2dInput.OnTextChangeReady = (Action)Delegate.Combine(tk2dInput.OnTextChangeReady, new Action(FixUcTextPosition));
			tk2dUITextInput tk2dInput2 = _tk2dInput;
			tk2dInput2.OnCursorChange = (Action)Delegate.Combine(tk2dInput2.OnCursorChange, new Action(UpdateUcCursorPosition));
		}
		else
		{
			tk2dUITextInput tk2dInput3 = _tk2dInput;
			tk2dInput3.OnTextChangeReady = (Action)Delegate.Remove(tk2dInput3.OnTextChangeReady, new Action(FixUcTextPosition));
			tk2dUITextInput tk2dInput4 = _tk2dInput;
			tk2dInput4.OnCursorChange = (Action)Delegate.Remove(tk2dInput4.OnCursorChange, new Action(UpdateUcCursorPosition));
		}
	}

	private void FixUcTextPosition()
	{
		StopCoroutine("FixUcTextPositionCoroutine");
		StartCoroutine("FixUcTextPositionCoroutine");
	}

	private IEnumerator FixUcTextPositionCoroutine()
	{
		yield return 0;
		while (_textMeshController.ucTextMesh.renderer.bounds.extents.x * 2f > _tk2dInput.fieldLength)
		{
			string modifiedText = _tk2dInput.inputLabel.text;
			modifiedText = modifiedText.Substring(1, modifiedText.Length - 1);
			_tk2dInput.inputLabel.text = modifiedText;
			_tk2dInput.inputLabel.Commit();
			yield return 0;
		}
		UpdateUcCursorPosition();
	}

	private void UpdateUcCursorPosition()
	{
		Vector3 position = _cursor.position;
		position.x = _textMeshController.ucTextMesh.renderer.bounds.max.x;
		_cursor.position = position;
	}

	private void OnDestroy()
	{
		tk2dUITextInput tk2dInput = _tk2dInput;
		tk2dInput.OnTextChangeReady = (Action)Delegate.Remove(tk2dInput.OnTextChangeReady, new Action(FixUcTextPosition));
		tk2dUITextInput tk2dInput2 = _tk2dInput;
		tk2dInput2.OnCursorChange = (Action)Delegate.Remove(tk2dInput2.OnCursorChange, new Action(UpdateUcCursorPosition));
	}
}
