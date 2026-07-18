using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(tk2dTextMesh))]
public class TextMeshController : MonoBehaviour
{
	private static char _unicodeCharacterRepresentation = 'W';

	private tk2dTextMesh _tk2DTextMesh;

	private UnicodeTextMesh _ucTextMesh;

	private GameObject _ucTextMeshGO;

	private MeshRenderer _tk2DTextMeshRenderer;

	private MeshRenderer _ucTextMeshRenderer;

	private bool _showingUcTextMesh;

	private bool _outlined;

	private bool _outlinedReady;

	private int _fontSize;

	public UnicodeTextMesh ucTextMesh
	{
		get
		{
			return _ucTextMesh;
		}
	}

	public event Action<bool> OnTextMeshSwitch;

	private unsafe void Awake()
	{
		_tk2DTextMesh = GetComponent<tk2dTextMesh>();
		_tk2DTextMeshRenderer = _tk2DTextMesh.GetComponent<MeshRenderer>();
		_fontSize = ((!_tk2DTextMesh.font.name.Contains("72")) ? 36 : 72);
		_outlined = _tk2DTextMesh.font.name.Contains("outlined");
		_tk2DTextMesh.OnTextChanged += new Action(this, (IntPtr)__ldftn(TextMeshController.UpdateTextComponent));
		_tk2DTextMesh.OnColorChanged += UpdateUcTextMeshColor;
	}

	private IEnumerator Start()
	{
		yield return 0;
		UpdateTextComponent();
	}

	private void UpdateTextComponent(bool forceSwitch = false)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (HaveUnicodeChar(_tk2DTextMesh.text))
		{
			if (!_showingUcTextMesh)
			{
				_tk2DTextMeshRenderer.enabled = false;
				if (_ucTextMeshRenderer != null)
				{
					_ucTextMeshRenderer.enabled = true;
				}
			}
			StartCoroutine(UpdateUcTextMesh());
			if (!_showingUcTextMesh || forceSwitch)
			{
				StopCoroutine("UpdateUcTextMeshProperties");
				StartCoroutine("UpdateUcTextMeshProperties");
				if (this.OnTextMeshSwitch != null)
				{
					this.OnTextMeshSwitch(true);
				}
			}
			_showingUcTextMesh = true;
			return;
		}
		if (_showingUcTextMesh || forceSwitch)
		{
			if (_ucTextMeshRenderer != null)
			{
				_ucTextMeshRenderer.enabled = false;
			}
			_tk2DTextMeshRenderer.enabled = true;
			StopCoroutine("UpdateUcTextMeshProperties");
			if (this.OnTextMeshSwitch != null)
			{
				this.OnTextMeshSwitch(false);
			}
		}
		_showingUcTextMesh = false;
	}

	private void CreateUcTextMeshIfNecessary()
	{
		if (_ucTextMesh == null)
		{
			_ucTextMeshGO = new GameObject();
			_ucTextMeshGO.name = base.gameObject.name + "-dynamicFont";
			_ucTextMeshGO.layer = base.gameObject.layer;
			_ucTextMesh = _ucTextMeshGO.AddComponent<UnicodeTextMesh>();
			_ucTextMeshRenderer = _ucTextMesh.GetComponent<MeshRenderer>();
			_ucTextMeshRenderer.enabled = false;
			_ucTextMesh.transform.parent = base.transform;
			_ucTextMesh.transform.localPosition = Vector3.zero;
			_ucTextMesh.transform.localScale = new Vector3(1f, 1f, 1f);
			_ucTextMesh.transform.localRotation = Quaternion.identity;
			_ucTextMesh.orderInLayer = _tk2DTextMesh.SortingOrder;
			_ucTextMesh.maxChars = _tk2DTextMesh.maxChars;
			_ucTextMesh.inlineStyling = _tk2DTextMesh.inlineStyling;
			_ucTextMesh.Init();
			_ucTextMesh.anchor = _tk2DTextMesh.anchor;
			_ucTextMesh.fontSize = 36;
		}
	}

	private IEnumerator UpdateUcTextMesh()
	{
		CreateUcTextMeshIfNecessary();
		_ucTextMesh.size = (float)_fontSize * _tk2DTextMesh.scale.x * 0.9f;
		_ucTextMesh.Text = SetFormatToUnicode(_tk2DTextMesh.text, _tk2DTextMesh.FormattedText);
		yield return 0;
		UpdateUcTextMeshColor();
		yield return 0;
		if (_showingUcTextMesh)
		{
			_ucTextMeshRenderer.enabled = true;
		}
	}

	private void UpdateUcTextMeshColor()
	{
		if (!_showingUcTextMesh)
		{
			return;
		}
		try
		{
			_ucTextMesh.color = _tk2DTextMesh.color;
			if (!_outlinedReady)
			{
				_ucTextMesh.outlined = _outlined;
				_outlinedReady = true;
			}
		}
		catch (Exception)
		{
		}
	}

	private IEnumerator UpdateUcTextMeshProperties()
	{
		while (true)
		{
			_ucTextMesh.FixBoundsIfNecessary(_tk2DTextMeshRenderer.bounds);
			yield return 0;
		}
	}

	private void OnEnable()
	{
		UpdateTextComponent(true);
	}

	private unsafe void OnDestroy()
	{
		_tk2DTextMesh.OnTextChanged -= new Action(this, (IntPtr)__ldftn(TextMeshController.UpdateTextComponent));
		_tk2DTextMesh.OnColorChanged -= UpdateUcTextMeshColor;
	}

	public bool HaveUnicodeChar(string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			if (IsCharUnicode(text[i]))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsCharUnicode(char c)
	{
		if (_tk2DTextMesh == null || _tk2DTextMesh.font == null || _tk2DTextMesh.font.inst == null || _tk2DTextMesh.font.inst.charDict == null)
		{
			return false;
		}
		return !_tk2DTextMesh.font.inst.charDict.ContainsKey(c);
	}

	public string GetTk2dRepresentationForUnicode(string text)
	{
		if (!HaveUnicodeChar(text))
		{
			return text;
		}
		string text2 = string.Empty;
		foreach (char c in text)
		{
			text2 = ((!IsCharUnicode(c)) ? (text2 + c) : (text2 + _unicodeCharacterRepresentation));
		}
		return text2;
	}

	private string SetFormatToUnicode(string unicodeText, string formatText)
	{
		if (!HaveUnicodeChar(unicodeText))
		{
			return formatText;
		}
		string text = string.Empty;
		foreach (char c in formatText)
		{
			if (c == _unicodeCharacterRepresentation)
			{
				for (int j = 0; j < unicodeText.Length; j++)
				{
					char c2 = unicodeText[j];
					if (IsCharUnicode(c2) || c2 == _unicodeCharacterRepresentation)
					{
						text += c2;
						unicodeText = ((j != unicodeText.Length - 1) ? unicodeText.Substring(j + 1) : string.Empty);
						break;
					}
				}
			}
			else
			{
				text += c;
			}
		}
		return text;
	}

	private static bool HasToReposition(Vector3 position1, Vector3 position2)
	{
		return Mathf.Abs(position1.x - position2.x) > 0.01f || Mathf.Abs(position1.y - position2.y) > 0.01f;
	}
}
