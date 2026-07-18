using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class EasyFontTextMesh : MonoBehaviour
{
	public enum TEXT_ANCHOR
	{
		UpperLeft = 0,
		UpperRight = 1,
		UpperCenter = 2,
		MiddleLeft = 3,
		MiddleRight = 4,
		MiddleCenter = 5,
		LowerLeft = 6,
		LowerRight = 7,
		LowerCenter = 8
	}

	public enum TEXT_ALIGNMENT
	{
		left = 0,
		right = 1,
		center = 2
	}

	public enum OUTLINE_QUALITY
	{
		low = 0,
		medium = 1,
		high = 2
	}

	public enum FILL_COLOR_STYLE
	{
		single = 0,
		gradient = 1,
		textureGradient = 2
	}

	private enum TEXT_COMPONENT
	{
		Main = 0,
		Shadow = 1,
		Outline = 2
	}

	[Serializable]
	public class TextProperties
	{
		public string text = "Hello World!";

		public Font font;

		public Material customDetailMaterial;

		public int fontSize = 16;

		public float size = 16f;

		public TEXT_ANCHOR textAnchor;

		public TEXT_ALIGNMENT textAlignment;

		public float lineSpacing = 1f;

		public FILL_COLOR_STYLE fillColorStyle;

		public Color fontColorTop = new Color(1f, 1f, 1f, 1f);

		public Color fontColorBottom = new Color(1f, 1f, 1f, 1f);

		public Material fillMaterial;

		public bool enableShadow;

		public Color shadowColor = new Color(0f, 0f, 0f, 1f);

		public Vector3 shadowDistance = new Vector3(0f, -1f, 0f);

		public bool enableOutline;

		public Color outlineColor = new Color(0f, 0f, 0f, 1f);

		public float outLineWidth = 0.3f;

		public OUTLINE_QUALITY outlineQuality;

		public int sortingLayerIndex;

		public int sortingLayerOrder;

		public string sortingLayerName;
	}

	[HideInInspector]
	public TextProperties _privateProperties;

	public bool updateAlwaysOnEnable;

	public bool dontOverrideMaterials;

	private Mesh textMesh;

	private MeshFilter textMeshFilter;

	private Material fontMaterial;

	private Renderer textRenderer;

	private char[] textChars;

	private bool isDirty;

	private int currentLineBreak;

	private float heightSum;

	private List<int> lineBreakCharCounter = new List<int>();

	private List<float> lineBreakAccumulatedDistance = new List<float>();

	private Vector3[] vertices;

	private int[] triangles;

	private Vector2[] uv;

	private Vector2[] uv2;

	private Color[] colors;

	[HideInInspector]
	public bool GUIChanged;

	private char LINE_BREAK = Convert.ToChar(10);

	public string Text
	{
		get
		{
			return _privateProperties.text;
		}
		set
		{
			_privateProperties.text = value;
			isDirty = true;
		}
	}

	public Font FontType
	{
		get
		{
			return _privateProperties.font;
		}
		set
		{
			_privateProperties.font = value;
			ChangeFont();
		}
	}

	public Material CustomDetailMaterial
	{
		get
		{
			return _privateProperties.customDetailMaterial;
		}
		set
		{
			_privateProperties.customDetailMaterial = value;
			isDirty = true;
		}
	}

	public int FontSize
	{
		get
		{
			return _privateProperties.fontSize;
		}
		set
		{
			_privateProperties.fontSize = value;
			isDirty = true;
		}
	}

	public float Size
	{
		get
		{
			return _privateProperties.size;
		}
		set
		{
			_privateProperties.size = value;
			isDirty = true;
		}
	}

	public TEXT_ANCHOR Textanchor
	{
		get
		{
			return _privateProperties.textAnchor;
		}
		set
		{
			_privateProperties.textAnchor = value;
			isDirty = true;
		}
	}

	public TEXT_ALIGNMENT Textalignment
	{
		get
		{
			return _privateProperties.textAlignment;
		}
		set
		{
			_privateProperties.textAlignment = value;
			isDirty = true;
		}
	}

	public float LineSpacing
	{
		get
		{
			return _privateProperties.lineSpacing;
		}
		set
		{
			_privateProperties.lineSpacing = value;
			isDirty = true;
		}
	}

	public FILL_COLOR_STYLE FillColorStyle
	{
		get
		{
			return _privateProperties.fillColorStyle;
		}
		set
		{
			_privateProperties.fillColorStyle = value;
			SetColor(_privateProperties.fontColorTop, _privateProperties.fontColorBottom);
		}
	}

	public Color FontColorTop
	{
		get
		{
			return _privateProperties.fontColorTop;
		}
		set
		{
			_privateProperties.fontColorTop = value;
			SetColor(_privateProperties.fontColorTop, _privateProperties.fontColorBottom);
		}
	}

	public Color FontColorBottom
	{
		get
		{
			return _privateProperties.fontColorBottom;
		}
		set
		{
			_privateProperties.fontColorBottom = value;
			SetColor(_privateProperties.fontColorTop, _privateProperties.fontColorBottom);
		}
	}

	public Material FillMaterial
	{
		get
		{
			return _privateProperties.fillMaterial;
		}
		set
		{
			_privateProperties.fillMaterial = value;
			SetColor(_privateProperties.fontColorTop, _privateProperties.fontColorBottom);
		}
	}

	public bool EnableShadow
	{
		get
		{
			return _privateProperties.enableShadow;
		}
		set
		{
			_privateProperties.enableShadow = value;
			isDirty = true;
		}
	}

	public Color ShadowColor
	{
		get
		{
			return _privateProperties.shadowColor;
		}
		set
		{
			_privateProperties.shadowColor = value;
			SetShadowColor(_privateProperties.shadowColor);
		}
	}

	public Vector3 ShadowDistance
	{
		get
		{
			return _privateProperties.shadowDistance;
		}
		set
		{
			_privateProperties.shadowDistance = value;
			isDirty = true;
		}
	}

	public bool EnableOutline
	{
		get
		{
			return _privateProperties.enableOutline;
		}
		set
		{
			_privateProperties.enableOutline = value;
			isDirty = true;
		}
	}

	public Color OutlineColor
	{
		get
		{
			return _privateProperties.outlineColor;
		}
		set
		{
			_privateProperties.outlineColor = value;
			SetOutlineColor(_privateProperties.outlineColor);
		}
	}

	public float OutLineWidth
	{
		get
		{
			return _privateProperties.outLineWidth;
		}
		set
		{
			_privateProperties.outLineWidth = value;
			isDirty = true;
		}
	}

	public OUTLINE_QUALITY OutlineQuality
	{
		get
		{
			return _privateProperties.outlineQuality;
		}
		set
		{
			_privateProperties.outlineQuality = value;
			isDirty = true;
		}
	}

	public int SortingLayerOrder
	{
		get
		{
			return _privateProperties.sortingLayerOrder;
		}
		set
		{
			_privateProperties.sortingLayerOrder = value;
			RefreshRenderLayerSettings();
		}
	}

	public string SortingLayerName
	{
		get
		{
			return _privateProperties.sortingLayerName;
		}
		set
		{
			_privateProperties.sortingLayerName = value;
			RefreshRenderLayerSettings();
		}
	}

	private void Awake()
	{
		if (_privateProperties == null)
		{
			_privateProperties = new TextProperties();
		}
		if (_privateProperties.font == null)
		{
			_privateProperties.font = UnicodeTextMesh.GetCustomFont();
		}
		CacheTextVars();
		RefreshMesh(true);
	}

	private void OnEnable()
	{
		Font font = _privateProperties.font;
		font.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Combine(font.textureRebuildCallback, new Font.FontTextureRebuildCallback(FontTexureRebuild));
		if (updateAlwaysOnEnable)
		{
			RefreshMesh(true);
		}
		RefreshRenderLayerSettings();
	}

	public void CacheTextVars()
	{
		textMeshFilter = GetComponent<MeshFilter>();
		if (textMeshFilter == null)
		{
			textMeshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		textMesh = textMeshFilter.sharedMesh;
		if (textMesh == null)
		{
			textMesh = new Mesh();
			textMesh.name = base.gameObject.name + GetInstanceID();
			textMeshFilter.sharedMesh = textMesh;
		}
		textRenderer = base.renderer;
		if (textRenderer == null)
		{
			textRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}
		SetFontMaterial();
	}

	private void SetFontMaterial()
	{
		if (dontOverrideMaterials)
		{
			return;
		}
		if (_privateProperties.customDetailMaterial != null)
		{
			if (textRenderer.sharedMaterials.Length < 2)
			{
				textRenderer.sharedMaterials = new Material[2]
				{
					_privateProperties.font.material,
					_privateProperties.customDetailMaterial
				};
			}
			if (_privateProperties.customDetailMaterial.mainTexture != _privateProperties.font.material.mainTexture)
			{
				_privateProperties.customDetailMaterial.mainTexture = _privateProperties.font.material.mainTexture;
			}
			textRenderer.sharedMaterial = _privateProperties.font.material;
		}
		else if (_privateProperties.fillColorStyle == FILL_COLOR_STYLE.textureGradient && _privateProperties.fillMaterial != null)
		{
			if (_privateProperties.fillMaterial.mainTexture != _privateProperties.font.material.mainTexture)
			{
				_privateProperties.fillMaterial.mainTexture = _privateProperties.font.material.mainTexture;
			}
			if (_privateProperties.enableShadow || _privateProperties.enableOutline)
			{
				textRenderer.sharedMaterials = new Material[2]
				{
					_privateProperties.font.material,
					_privateProperties.fillMaterial
				};
			}
			else
			{
				textRenderer.sharedMaterials = new Material[1] { _privateProperties.fillMaterial };
			}
		}
		else
		{
			textRenderer.sharedMaterials = new Material[1] { _privateProperties.font.material };
		}
	}

	private void RefreshMesh(bool _updateTexureInfo)
	{
		if (_updateTexureInfo)
		{
			_privateProperties.font.RequestCharactersInTexture(_privateProperties.text, _privateProperties.fontSize);
		}
		textChars = null;
		textChars = _privateProperties.text.ToCharArray();
		AnalizeText();
		float num = 90f;
		int num2 = 1;
		if (_privateProperties.enableOutline)
		{
			switch (_privateProperties.outlineQuality)
			{
			case OUTLINE_QUALITY.medium:
				num2 += 8;
				num = 45f;
				break;
			case OUTLINE_QUALITY.high:
				num = 22.5f;
				num2 += 16;
				break;
			default:
				num = 90f;
				num2 += 4;
				break;
			}
		}
		if (_privateProperties.enableShadow)
		{
			num2++;
		}
		vertices = new Vector3[textChars.Length * 4 * num2];
		triangles = new int[textChars.Length * 6 * num2];
		uv = new Vector2[textChars.Length * 4 * num2];
		uv2 = new Vector2[textChars.Length * 4 * num2];
		colors = new Color[textChars.Length * 4 * num2];
		int num3 = 0;
		int num4 = 0;
		if (_privateProperties.enableShadow)
		{
			ResetHelperVariables();
			char[] array = textChars;
			foreach (char character in array)
			{
				CreateCharacter(character, num3, _privateProperties.shadowDistance, _privateProperties.shadowColor, _privateProperties.shadowColor);
				num3++;
			}
			SetAlignment(num4++);
		}
		if (_privateProperties.enableOutline)
		{
			for (float num5 = 0f; num5 < 360f; num5 += num)
			{
				Vector3 right = Vector3.right;
				right.x = Mathf.Cos(num5 * ((float)Math.PI / 180f));
				right.y = Mathf.Sin(num5 * ((float)Math.PI / 180f));
				ResetHelperVariables();
				char[] array2 = textChars;
				foreach (char character2 in array2)
				{
					CreateCharacter(character2, num3, right * _privateProperties.outLineWidth, _privateProperties.outlineColor, _privateProperties.outlineColor);
					num3++;
				}
				SetAlignment(num4++);
			}
		}
		ResetHelperVariables();
		char[] array3 = textChars;
		foreach (char character3 in array3)
		{
			CreateCharacter(character3, num3, Vector3.zero, _privateProperties.fontColorTop, _privateProperties.fontColorBottom);
			num3++;
		}
		SetAlignment(num4++);
		if (textMesh != null)
		{
			textMesh.Clear(true);
			SetAnchor();
			textMesh.vertices = vertices;
			textMesh.uv = uv;
			textMesh.uv2 = uv2;
			SetColor(_privateProperties.fontColorTop, _privateProperties.fontColorBottom);
			if ((_privateProperties.customDetailMaterial != null || _privateProperties.fillColorStyle == FILL_COLOR_STYLE.textureGradient) && (_privateProperties.enableShadow || _privateProperties.enableOutline))
			{
				SetTrianglesForMultimesh();
			}
			else
			{
				textMesh.triangles = triangles;
			}
		}
	}

	private void ResetHelperVariables()
	{
		lineBreakAccumulatedDistance.Clear();
		lineBreakCharCounter.Clear();
		currentLineBreak = 0;
		heightSum = 0f;
	}

	private void AnalizeText()
	{
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < textChars.Length; i++)
			{
				if (textChars[i] != '\\' || i + 1 >= textChars.Length || textChars[i + 1] != 'n')
				{
					continue;
				}
				char[] array = new char[textChars.Length - 1];
				int num = 0;
				for (int j = 0; j < textChars.Length; j++)
				{
					if (j == i)
					{
						array[num] = LINE_BREAK;
						num++;
						continue;
					}
					if (j == i + 1)
					{
						j++;
						if (j >= textChars.Length)
						{
							continue;
						}
					}
					array[num] = textChars[j];
					num++;
				}
				textChars = array;
				flag = true;
				break;
			}
		}
	}

	private void CreateCharacter(char _character, int _arrayPosition, Vector3 _offset, Color _colorTop, Color _colorBottom)
	{
		if (lineBreakAccumulatedDistance.Count == 0)
		{
			lineBreakAccumulatedDistance.Add(0f);
		}
		if (lineBreakCharCounter.Count == 0)
		{
			lineBreakCharCounter.Add(0);
		}
		CharacterInfo info = default(CharacterInfo);
		if (!_privateProperties.font.GetCharacterInfo(_character, out info, _privateProperties.fontSize))
		{
			lineBreakCharCounter.Add(lineBreakCharCounter[currentLineBreak]);
			lineBreakAccumulatedDistance.Add(0f);
			currentLineBreak++;
			return;
		}
		List<int> list2;
		List<int> list = (list2 = lineBreakCharCounter);
		int index2;
		int index = (index2 = currentLineBreak);
		index2 = list2[index2];
		list[index] = index2 + 1;
		float num = _privateProperties.size / (float)_privateProperties.fontSize;
		_offset *= _privateProperties.size * 0.1f;
		float num2 = info.vert.width * num;
		float num3 = info.vert.height * num;
		Vector2 vector = new Vector2(info.vert.x, info.vert.y) * num;
		if (_character != ' ')
		{
			heightSum += (info.vert.y + info.vert.height * 0.5f) * num;
		}
		Vector3 vector2 = new Vector3(lineBreakAccumulatedDistance[currentLineBreak] * num, (0f - _privateProperties.size) * (float)currentLineBreak * _privateProperties.lineSpacing, 0f);
		if (info.flipped)
		{
			vertices[4 * _arrayPosition] = new Vector3(vector.x + num2, num3 + vector.y, 0f) + _offset + vector2;
			vertices[4 * _arrayPosition + 1] = new Vector3(vector.x, num3 + vector.y, 0f) + _offset + vector2;
			vertices[4 * _arrayPosition + 2] = new Vector3(vector.x, vector.y, 0f) + _offset + vector2;
			vertices[4 * _arrayPosition + 3] = new Vector3(vector.x + num2, vector.y, 0f) + _offset + vector2;
		}
		else
		{
			vertices[4 * _arrayPosition] = new Vector3(vector.x + num2, num3 + vector.y, 0f) + _offset + vector2;
			vertices[4 * _arrayPosition + 1] = new Vector3(vector.x, num3 + vector.y, 0f) + _offset + vector2;
			vertices[4 * _arrayPosition + 2] = new Vector3(vector.x, vector.y, 0f) + _offset + vector2;
			vertices[4 * _arrayPosition + 3] = new Vector3(vector.x + num2, vector.y, 0f) + _offset + vector2;
		}
		List<float> list4;
		List<float> list3 = (list4 = lineBreakAccumulatedDistance);
		int index3 = (index2 = currentLineBreak);
		float num4 = list4[index2];
		list3[index3] = num4 + info.width;
		triangles[6 * _arrayPosition] = _arrayPosition * 4;
		triangles[6 * _arrayPosition + 1] = _arrayPosition * 4 + 1;
		triangles[6 * _arrayPosition + 2] = _arrayPosition * 4 + 2;
		triangles[6 * _arrayPosition + 3] = _arrayPosition * 4;
		triangles[6 * _arrayPosition + 4] = _arrayPosition * 4 + 2;
		triangles[6 * _arrayPosition + 5] = _arrayPosition * 4 + 3;
		if (info.flipped)
		{
			uv[4 * _arrayPosition] = new Vector2(info.uv.x, info.uv.y + info.uv.height);
			uv[4 * _arrayPosition + 1] = new Vector2(info.uv.x, info.uv.y);
			uv[4 * _arrayPosition + 2] = new Vector2(info.uv.x + info.uv.width, info.uv.y);
			uv[4 * _arrayPosition + 3] = new Vector2(info.uv.x + info.uv.width, info.uv.y + info.uv.height);
		}
		else
		{
			uv[4 * _arrayPosition] = new Vector2(info.uv.x + info.uv.width, info.uv.y);
			uv[4 * _arrayPosition + 1] = new Vector2(info.uv.x, info.uv.y);
			uv[4 * _arrayPosition + 2] = new Vector2(info.uv.x, info.uv.y + info.uv.height);
			uv[4 * _arrayPosition + 3] = new Vector2(info.uv.x + info.uv.width, info.uv.y + info.uv.height);
		}
		if (_privateProperties.customDetailMaterial != null && _privateProperties.fillColorStyle != FILL_COLOR_STYLE.textureGradient)
		{
			Vector2 vector3 = new Vector2(_offset.x, _offset.y);
			Vector2 vector4 = new Vector2(vector2.x, vector2.y);
			uv2[4 * _arrayPosition] = new Vector2(vector.x + num2, num3 + vector.y) + vector3 + vector4;
			uv2[4 * _arrayPosition + 1] = new Vector2(vector.x, num3 + vector.y) + vector3 + vector4;
			uv2[4 * _arrayPosition + 2] = new Vector2(vector.x, vector.y) + vector3 + vector4;
			uv2[4 * _arrayPosition + 3] = new Vector2(vector.x + num2, vector.y) + vector3 + vector4;
		}
		else
		{
			uv2[4 * _arrayPosition] = new Vector2(0f, 0f);
			uv2[4 * _arrayPosition + 1] = new Vector2(1f, 0f);
			uv2[4 * _arrayPosition + 2] = new Vector2(1f, 1f);
			uv2[4 * _arrayPosition + 3] = new Vector2(0f, 1f);
		}
		colors[4 * _arrayPosition] = _colorBottom;
		colors[4 * _arrayPosition + 1] = _colorBottom;
		colors[4 * _arrayPosition + 2] = _colorTop;
		colors[4 * _arrayPosition + 3] = _colorTop;
	}

	private void SetAnchor()
	{
		Vector2 zero = Vector2.zero;
		float num = 0f;
		for (int i = 0; i < lineBreakAccumulatedDistance.Count; i++)
		{
			if (lineBreakAccumulatedDistance[i] > num)
			{
				num = lineBreakAccumulatedDistance[i];
			}
		}
		switch (_privateProperties.textAnchor)
		{
		case TEXT_ANCHOR.UpperLeft:
		case TEXT_ANCHOR.MiddleLeft:
		case TEXT_ANCHOR.LowerLeft:
			switch (_privateProperties.textAlignment)
			{
			case TEXT_ALIGNMENT.left:
				zero.x = 0f;
				break;
			case TEXT_ALIGNMENT.right:
				zero.x = num * _privateProperties.size / (float)_privateProperties.fontSize;
				break;
			case TEXT_ALIGNMENT.center:
				zero.x += num * 0.5f * _privateProperties.size / (float)_privateProperties.fontSize;
				break;
			}
			break;
		case TEXT_ANCHOR.UpperRight:
		case TEXT_ANCHOR.MiddleRight:
		case TEXT_ANCHOR.LowerRight:
			switch (_privateProperties.textAlignment)
			{
			case TEXT_ALIGNMENT.left:
				zero.x -= num * _privateProperties.size / (float)_privateProperties.fontSize;
				break;
			case TEXT_ALIGNMENT.right:
				zero.x = 0f;
				break;
			case TEXT_ALIGNMENT.center:
				zero.x -= num * 0.5f * _privateProperties.size / (float)_privateProperties.fontSize;
				break;
			}
			break;
		case TEXT_ANCHOR.UpperCenter:
		case TEXT_ANCHOR.MiddleCenter:
		case TEXT_ANCHOR.LowerCenter:
			switch (_privateProperties.textAlignment)
			{
			case TEXT_ALIGNMENT.left:
				zero.x -= num * _privateProperties.size * 0.5f / (float)_privateProperties.fontSize;
				break;
			case TEXT_ALIGNMENT.right:
				zero.x = num * 0.5f * _privateProperties.size / (float)_privateProperties.fontSize;
				break;
			case TEXT_ALIGNMENT.center:
				zero.x = 0f;
				break;
			}
			break;
		}
		if (_privateProperties.textAnchor == TEXT_ANCHOR.UpperLeft || _privateProperties.textAnchor == TEXT_ANCHOR.UpperRight || _privateProperties.textAnchor == TEXT_ANCHOR.UpperCenter)
		{
			zero.y = (0f - heightSum) / (float)textChars.Length;
		}
		else if (_privateProperties.textAnchor == TEXT_ANCHOR.MiddleCenter || _privateProperties.textAnchor == TEXT_ANCHOR.MiddleLeft || _privateProperties.textAnchor == TEXT_ANCHOR.MiddleRight)
		{
			zero.y = 0f - heightSum / (float)textChars.Length + _privateProperties.size * (float)currentLineBreak * _privateProperties.lineSpacing * 0.5f;
		}
		else if (_privateProperties.textAnchor == TEXT_ANCHOR.LowerLeft || _privateProperties.textAnchor == TEXT_ANCHOR.LowerRight || _privateProperties.textAnchor == TEXT_ANCHOR.LowerCenter)
		{
			zero.y = (0f - heightSum) / (float)textChars.Length + _privateProperties.size * (float)currentLineBreak * _privateProperties.lineSpacing;
		}
		for (int j = 0; j < vertices.Length; j++)
		{
			vertices[j].x += zero.x;
			vertices[j].y += zero.y;
		}
	}

	private void SetAlignment(int _pass)
	{
		int num = _pass * textChars.Length * 4;
		float num2 = 0f;
		for (int i = 0; i < lineBreakCharCounter.Count; i++)
		{
			switch (_privateProperties.textAlignment)
			{
			case TEXT_ALIGNMENT.right:
				num2 = (0f - lineBreakAccumulatedDistance[i]) * _privateProperties.size / (float)_privateProperties.fontSize;
				break;
			case TEXT_ALIGNMENT.center:
				num2 = (0f - lineBreakAccumulatedDistance[i]) * 0.5f * _privateProperties.size / (float)_privateProperties.fontSize;
				break;
			}
			int num3 = ((i != 0) ? (lineBreakCharCounter[i - 1] * 4) : 0);
			int num4 = lineBreakCharCounter[i] * 4 - 1;
			for (int j = num3 + i * 4 + num; j <= num4 + i * 4 + num; j++)
			{
				vertices[j].x += num2;
			}
		}
	}

	private void SetTrianglesForMultimesh()
	{
		int num = 0;
		num += GetVertexIndexPosition();
		int num2 = num * 6 * textChars.Length;
		int[] array = new int[textChars.Length * 6];
		int num3 = 0;
		for (int i = num2; i < triangles.Length; i++)
		{
			array[num3] = triangles[i];
			num3++;
		}
		num3 = 0;
		int num4 = textChars.Length * num * 6;
		int[] array2 = new int[num4];
		for (int j = 0; j < num4; j++)
		{
			array2[num3] = triangles[j];
			num3++;
		}
		textMeshFilter.sharedMesh.subMeshCount = 2;
		textMeshFilter.sharedMesh.SetTriangles(array, 1);
		textMeshFilter.sharedMesh.SetTriangles(array2, 0);
	}

	private void FontTexureRebuild()
	{
		RefreshMesh(true);
	}

	private void OnDisable()
	{
		Font font = _privateProperties.font;
		font.textureRebuildCallback = (Font.FontTextureRebuildCallback)Delegate.Remove(font.textureRebuildCallback, new Font.FontTextureRebuildCallback(FontTexureRebuild));
	}

	public void RefreshMeshEditor()
	{
		CacheTextVars();
		UnityEngine.Object.DestroyImmediate(textMesh);
		textMesh = new Mesh();
		textMesh.name = GetInstanceID().ToString();
		MeshFilter component = GetComponent<MeshFilter>();
		if (component != null)
		{
			component.sharedMesh = textMesh;
			SetFontMaterial();
			RefreshRenderLayerSettings();
			RefreshMesh(true);
		}
	}

	private void RefreshRenderLayerSettings()
	{
		base.renderer.sortingOrder = _privateProperties.sortingLayerOrder;
		base.renderer.sortingLayerName = _privateProperties.sortingLayerName;
	}

	public int GetVertexCount()
	{
		if (vertices != null)
		{
			return vertices.Length;
		}
		return 0;
	}

	private void LateUpdate()
	{
		if (isDirty)
		{
			isDirty = false;
			RefreshMesh(true);
		}
	}

	private void SetColor(Color _topColor, Color _bottomColor)
	{
		if (colors == null || textMesh == null)
		{
			return;
		}
		switch (_privateProperties.fillColorStyle)
		{
		case FILL_COLOR_STYLE.single:
			_bottomColor = _topColor;
			break;
		case FILL_COLOR_STYLE.textureGradient:
			_topColor = Color.white;
			_bottomColor = Color.white;
			break;
		}
		int initialVertexToColorize = GetInitialVertexToColorize(TEXT_COMPONENT.Main);
		int num = 0;
		for (int i = initialVertexToColorize; i < GetFinalVertexToColorize(TEXT_COMPONENT.Main); i++)
		{
			if (num == 0 || num == 1)
			{
				colors[i] = _bottomColor;
			}
			else
			{
				colors[i] = _topColor;
			}
			num++;
			if (num > 3)
			{
				num = 0;
			}
		}
		textMesh.colors = colors;
	}

	public void SetColor(Color _color)
	{
		if (colors != null && !(textMesh == null))
		{
			int initialVertexToColorize = GetInitialVertexToColorize(TEXT_COMPONENT.Main);
			for (int i = initialVertexToColorize; i < GetFinalVertexToColorize(TEXT_COMPONENT.Main); i++)
			{
				colors[i] = _color;
			}
			textMesh.colors = colors;
		}
	}

	private void SetShadowColor(Color _color)
	{
		if (colors != null && !(textMesh == null))
		{
			int initialVertexToColorize = GetInitialVertexToColorize(TEXT_COMPONENT.Shadow);
			for (int i = initialVertexToColorize; i < GetFinalVertexToColorize(TEXT_COMPONENT.Shadow); i++)
			{
				colors[i] = _color;
			}
			textMesh.colors = colors;
		}
	}

	private void SetOutlineColor(Color _color)
	{
		if (colors != null && !(textMesh == null))
		{
			int initialVertexToColorize = GetInitialVertexToColorize(TEXT_COMPONENT.Outline);
			for (int i = initialVertexToColorize; i < GetFinalVertexToColorize(TEXT_COMPONENT.Outline); i++)
			{
				colors[i] = _color;
			}
			textMesh.colors = colors;
		}
	}

	private int GetInitialVertexToColorize(TEXT_COMPONENT _textComponent)
	{
		if (textChars == null)
		{
			textChars = _privateProperties.text.ToCharArray();
		}
		int num = 0;
		switch (_textComponent)
		{
		case TEXT_COMPONENT.Main:
			num += GetVertexIndexPosition();
			break;
		case TEXT_COMPONENT.Shadow:
			num = 0;
			break;
		case TEXT_COMPONENT.Outline:
			num = (_privateProperties.enableShadow ? 1 : 0);
			break;
		}
		return textChars.Length * 4 * num;
	}

	private int GetFinalVertexToColorize(TEXT_COMPONENT _textComponent)
	{
		if (textChars == null)
		{
			textChars = _privateProperties.text.ToCharArray();
		}
		int result = 0;
		int num = 1;
		switch (_textComponent)
		{
		case TEXT_COMPONENT.Main:
			num += GetVertexIndexPosition();
			result = textChars.Length * 4 * num;
			break;
		case TEXT_COMPONENT.Shadow:
			result = textChars.Length * 4;
			break;
		case TEXT_COMPONENT.Outline:
			num = (_privateProperties.enableShadow ? 1 : 0);
			result = textChars.Length * 4 * (num + 4);
			break;
		}
		return result;
	}

	private int GetVertexIndexPosition()
	{
		int num = 0;
		if (_privateProperties.enableOutline)
		{
			switch (_privateProperties.outlineQuality)
			{
			case OUTLINE_QUALITY.low:
				num += 4;
				break;
			case OUTLINE_QUALITY.medium:
				num += 8;
				break;
			case OUTLINE_QUALITY.high:
				num += 16;
				break;
			}
		}
		if (_privateProperties.enableShadow)
		{
			num++;
		}
		return num;
	}

	private void ChangeFont()
	{
		if (!dontOverrideMaterials && _privateProperties.customDetailMaterial == null)
		{
			textRenderer.sharedMaterial = _privateProperties.font.material;
		}
		isDirty = true;
	}
}
