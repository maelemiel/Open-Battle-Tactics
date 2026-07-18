using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[RequireComponent(typeof(EasyFontTextMesh))]
public class UnicodeTextMesh : MonoBehaviour
{
	private static readonly string CUSTOM_UNICODE_FONT_PATH = "UnicodeFonts/DFGBZY9_pro";

	private static Material _sharedMaterial;

	private static Font _arialFont;

	private static Font _customFont;

	private EasyFontTextMesh _textMesh;

	private Renderer _renderer;

	private Transform _transform;

	private bool _initReady;

	private List<Color> _colorList = new List<Color>();

	private Color _color = Color.white;

	private bool _inlineStyling;

	private int _maxChars;

	public string Text
	{
		get
		{
			return _textMesh.Text;
		}
		set
		{
			if (value != null && !value.Equals(_textMesh.Text))
			{
				string text = FormatText(value);
				_textMesh.Text = text;
			}
		}
	}

	public TextAnchor anchor
	{
		get
		{
			return ConvertAnchorFromEasyText(_textMesh.Textanchor);
		}
		set
		{
			_textMesh.Textanchor = ConvertAnchorToEasyText(value);
			_textMesh.Textalignment = GetAlignmentFromAcnhor(value);
		}
	}

	public Font font
	{
		get
		{
			return _textMesh.FontType;
		}
		set
		{
			if (value != _textMesh.FontType)
			{
				_textMesh.FontType = value;
				_sharedMaterial = null;
				SetMaterial();
			}
		}
	}

	public int orderInLayer
	{
		get
		{
			return _textMesh.SortingLayerOrder;
		}
		set
		{
			_textMesh.SortingLayerOrder = value;
		}
	}

	public bool outlined
	{
		get
		{
			return _textMesh.EnableOutline;
		}
		set
		{
			if (_textMesh.EnableOutline != value)
			{
				if (value)
				{
					_textMesh.EnableOutline = true;
					_textMesh.OutLineWidth = 0.7f;
					_textMesh.OutlineQuality = EasyFontTextMesh.OUTLINE_QUALITY.low;
					Color outlineColor = _textMesh.OutlineColor;
					outlineColor.a = _color.a;
					_textMesh.OutlineColor = outlineColor;
				}
				else
				{
					_textMesh.EnableOutline = false;
				}
			}
		}
	}

	public Color color
	{
		get
		{
			return _color;
		}
		set
		{
			if (_color != value)
			{
				_color = value;
				_textMesh.FontColorBottom = _color;
				_textMesh.FontColorTop = _color;
				if (_textMesh.EnableOutline)
				{
					Color outlineColor = _textMesh.OutlineColor;
					outlineColor.a = _color.a;
					_textMesh.OutlineColor = outlineColor;
				}
			}
		}
	}

	public int fontSize
	{
		get
		{
			return _textMesh.FontSize;
		}
		set
		{
			_textMesh.FontSize = value;
		}
	}

	public float size
	{
		get
		{
			return _textMesh.Size;
		}
		set
		{
			if (value >= 60f && fontSize != 80)
			{
				fontSize = 80;
			}
			if (value < 60f && fontSize != 36)
			{
				fontSize = 36;
			}
			_textMesh.Size = value;
		}
	}

	public bool inlineStyling
	{
		get
		{
			return _inlineStyling;
		}
		set
		{
			_inlineStyling = value;
			if (_initReady)
			{
				FormatSelfText();
			}
		}
	}

	public int maxChars
	{
		get
		{
			return _maxChars;
		}
		set
		{
			_maxChars = value;
			if (_initReady)
			{
				FormatSelfText();
			}
		}
	}

	private void Awake()
	{
		_textMesh = GetComponent<EasyFontTextMesh>();
		_textMesh.Text = string.Empty;
		_renderer = base.renderer;
		_transform = base.transform;
	}

	public void Init()
	{
		_textMesh.FontType = GetCustomFont();
		_textMesh.FillColorStyle = EasyFontTextMesh.FILL_COLOR_STYLE.single;
		SetMaterial();
		_initReady = true;
	}

	private void SetMaterial()
	{
		if (_sharedMaterial == null)
		{
			Shader shader = Shader.Find("GUI/EasyFont Text Detail");
			_sharedMaterial = _renderer.material;
			_sharedMaterial.shader = shader;
		}
		else
		{
			_renderer.material = _sharedMaterial;
		}
	}

	private void FormatSelfText()
	{
		_textMesh.Text = FormatText(_textMesh.Text);
	}

	private string FormatText(string text)
	{
		if (_inlineStyling)
		{
			string text2 = string.Empty;
			bool flag = false;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (c == '^')
				{
					int num = GetInlineStyleCommandLength(text[i + 1]) + 1;
					flag = CreateColor(text.Substring(i + 1, num - 1)).a < 1f;
					i += num - 1;
					continue;
				}
				if (flag)
				{
					c = ' ';
				}
				text2 += c;
			}
			text = text2;
		}
		if (_maxChars > 0 && _maxChars < text.Length)
		{
			text = text.Substring(0, _maxChars);
		}
		return text;
	}

	public void FixBoundsIfNecessary(Bounds targetBound)
	{
		float num = -1f;
		Bounds bounds = _renderer.bounds;
		switch (_textMesh.Textalignment)
		{
		case EasyFontTextMesh.TEXT_ALIGNMENT.center:
			num = targetBound.center.x - bounds.center.x;
			break;
		case EasyFontTextMesh.TEXT_ALIGNMENT.left:
			num = targetBound.center.x - 0.5f * targetBound.size.x - (bounds.center.x - 0.5f * bounds.size.x);
			break;
		case EasyFontTextMesh.TEXT_ALIGNMENT.right:
			num = targetBound.center.x + 0.5f * targetBound.size.x - (bounds.center.x + 0.5f * bounds.size.x);
			break;
		}
		if (!(Mathf.Abs(num) < 0.01f))
		{
			Vector3 position = _transform.position;
			position.x += num;
			position.y += targetBound.center.y - bounds.center.y;
			position.z += targetBound.center.z - bounds.center.z;
			_transform.position = position;
		}
	}

	public static Font GetDefaultFont()
	{
		if (_arialFont == null)
		{
			_arialFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
		}
		return _arialFont;
	}

	public static Font GetCustomFont()
	{
		if (_customFont == null)
		{
			_customFont = Resources.Load<Font>(CUSTOM_UNICODE_FONT_PATH);
			if (_customFont == null)
			{
				Log.Error("Custom font not found. Path: " + CUSTOM_UNICODE_FONT_PATH + ". The default one will be used instead");
				_customFont = GetDefaultFont();
			}
		}
		return _customFont;
	}

	private static TextAnchor ConvertAnchorFromEasyText(EasyFontTextMesh.TEXT_ANCHOR easyAnchor)
	{
		switch (easyAnchor)
		{
		case EasyFontTextMesh.TEXT_ANCHOR.LowerCenter:
			return TextAnchor.LowerCenter;
		case EasyFontTextMesh.TEXT_ANCHOR.LowerLeft:
			return TextAnchor.LowerLeft;
		case EasyFontTextMesh.TEXT_ANCHOR.LowerRight:
			return TextAnchor.LowerRight;
		case EasyFontTextMesh.TEXT_ANCHOR.MiddleCenter:
			return TextAnchor.MiddleCenter;
		case EasyFontTextMesh.TEXT_ANCHOR.MiddleLeft:
			return TextAnchor.MiddleLeft;
		case EasyFontTextMesh.TEXT_ANCHOR.MiddleRight:
			return TextAnchor.MiddleRight;
		case EasyFontTextMesh.TEXT_ANCHOR.UpperCenter:
			return TextAnchor.UpperCenter;
		case EasyFontTextMesh.TEXT_ANCHOR.UpperLeft:
			return TextAnchor.UpperLeft;
		case EasyFontTextMesh.TEXT_ANCHOR.UpperRight:
			return TextAnchor.UpperRight;
		default:
			return TextAnchor.UpperRight;
		}
	}

	private static EasyFontTextMesh.TEXT_ANCHOR ConvertAnchorToEasyText(TextAnchor textAnchor)
	{
		switch (textAnchor)
		{
		case TextAnchor.LowerCenter:
			return EasyFontTextMesh.TEXT_ANCHOR.LowerCenter;
		case TextAnchor.LowerLeft:
			return EasyFontTextMesh.TEXT_ANCHOR.LowerLeft;
		case TextAnchor.LowerRight:
			return EasyFontTextMesh.TEXT_ANCHOR.LowerRight;
		case TextAnchor.MiddleCenter:
			return EasyFontTextMesh.TEXT_ANCHOR.MiddleCenter;
		case TextAnchor.MiddleLeft:
			return EasyFontTextMesh.TEXT_ANCHOR.MiddleLeft;
		case TextAnchor.MiddleRight:
			return EasyFontTextMesh.TEXT_ANCHOR.MiddleRight;
		case TextAnchor.UpperCenter:
			return EasyFontTextMesh.TEXT_ANCHOR.UpperCenter;
		case TextAnchor.UpperLeft:
			return EasyFontTextMesh.TEXT_ANCHOR.UpperLeft;
		case TextAnchor.UpperRight:
			return EasyFontTextMesh.TEXT_ANCHOR.UpperRight;
		default:
			return EasyFontTextMesh.TEXT_ANCHOR.UpperRight;
		}
	}

	private static EasyFontTextMesh.TEXT_ALIGNMENT GetAlignmentFromAcnhor(TextAnchor textAnchor)
	{
		switch (textAnchor)
		{
		case TextAnchor.UpperCenter:
		case TextAnchor.MiddleCenter:
		case TextAnchor.LowerCenter:
			return EasyFontTextMesh.TEXT_ALIGNMENT.center;
		case TextAnchor.UpperLeft:
		case TextAnchor.MiddleLeft:
		case TextAnchor.LowerLeft:
			return EasyFontTextMesh.TEXT_ALIGNMENT.left;
		case TextAnchor.UpperRight:
		case TextAnchor.MiddleRight:
		case TextAnchor.LowerRight:
			return EasyFontTextMesh.TEXT_ALIGNMENT.right;
		default:
			return EasyFontTextMesh.TEXT_ALIGNMENT.center;
		}
	}

	private static Color CreateColor(string colorStr)
	{
		if (string.IsNullOrEmpty(colorStr))
		{
			return Color.blue;
		}
		switch (colorStr[0])
		{
		case 'c':
			return Color.blue;
		case 'C':
			return new Color(GetColorFromHexString(colorStr.Substring(1, 2)), GetColorFromHexString(colorStr.Substring(3, 2)), GetColorFromHexString(colorStr.Substring(5, 2)), GetColorFromHexString(colorStr.Substring(7, 2)));
		case 'g':
			return Color.blue;
		case 'G':
			return Color.blue;
		default:
			return Color.blue;
		}
	}

	private static float GetColorFromHexString(string hexStr)
	{
		return (float)int.Parse(hexStr, NumberStyles.HexNumber) / 255f;
	}

	public static int GetInlineStyleCommandLength(char cmdSymbol)
	{
		int result = 0;
		switch (cmdSymbol)
		{
		case 'c':
			result = 5;
			break;
		case 'C':
			result = 9;
			break;
		case 'g':
			result = 9;
			break;
		case 'G':
			result = 17;
			break;
		}
		return result;
	}
}
