using System;
using System.Text;
using UnityEngine;
using tk2dRuntime;

[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("2D Toolkit/Text/tk2dTextMesh")]
public class tk2dTextMesh : MonoBehaviour, ISpriteCollectionForceBuild, IRenderable
{
	[Flags]
	private enum UpdateFlags
	{
		UpdateNone = 0,
		UpdateText = 1,
		UpdateColors = 2,
		UpdateBuffers = 4
	}

	private tk2dFontData _fontInst;

	private string _formattedText = string.Empty;

	[SerializeField]
	private tk2dFontData _font;

	[SerializeField]
	private string _text = string.Empty;

	[SerializeField]
	private Color _color = Color.white;

	[SerializeField]
	private Color _color2 = Color.white;

	[SerializeField]
	private bool _useGradient;

	[SerializeField]
	private int _textureGradient;

	[SerializeField]
	private TextAnchor _anchor = TextAnchor.LowerLeft;

	[SerializeField]
	private Vector3 _scale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	private bool _kerning;

	[SerializeField]
	private int _maxChars = 16;

	[SerializeField]
	private bool _inlineStyling;

	[SerializeField]
	private bool _formatting;

	[SerializeField]
	private int _wordWrapWidth;

	[SerializeField]
	private float spacing;

	[SerializeField]
	private float lineSpacing;

	[SerializeField]
	private tk2dTextMeshData data = new tk2dTextMeshData();

	private TextMeshController _textMeshController;

	private Vector3[] vertices;

	private Vector2[] uvs;

	private Vector2[] uv2;

	private Color32[] colors;

	private Color32[] untintedColors;

	private UpdateFlags updateFlags = UpdateFlags.UpdateBuffers;

	private Mesh mesh;

	private MeshFilter meshFilter;

	private Renderer _cachedRenderer;

	public TextMeshController textMeshController
	{
		get
		{
			return _textMeshController;
		}
	}

	public string FormattedText
	{
		get
		{
			return _formattedText;
		}
	}

	public tk2dFontData font
	{
		get
		{
			UpgradeData();
			return data.font;
		}
		set
		{
			UpgradeData();
			data.font = value;
			_fontInst = data.font.inst;
			SetNeedUpdate(UpdateFlags.UpdateText);
			UpdateMaterial();
		}
	}

	public bool formatting
	{
		get
		{
			UpgradeData();
			return data.formatting;
		}
		set
		{
			UpgradeData();
			if (data.formatting != value)
			{
				data.formatting = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public int wordWrapWidth
	{
		get
		{
			UpgradeData();
			return data.wordWrapWidth;
		}
		set
		{
			UpgradeData();
			if (data.wordWrapWidth != value)
			{
				data.wordWrapWidth = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public string text
	{
		get
		{
			UpgradeData();
			return data.text;
		}
		set
		{
			UpgradeData();
			data.text = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public Color color
	{
		get
		{
			UpgradeData();
			return data.color;
		}
		set
		{
			UpgradeData();
			data.color = value;
			SetNeedUpdate(UpdateFlags.UpdateColors);
		}
	}

	public Color color2
	{
		get
		{
			UpgradeData();
			return data.color2;
		}
		set
		{
			UpgradeData();
			data.color2 = value;
			SetNeedUpdate(UpdateFlags.UpdateColors);
		}
	}

	public bool useGradient
	{
		get
		{
			UpgradeData();
			return data.useGradient;
		}
		set
		{
			UpgradeData();
			data.useGradient = value;
			SetNeedUpdate(UpdateFlags.UpdateColors);
		}
	}

	public TextAnchor anchor
	{
		get
		{
			UpgradeData();
			return data.anchor;
		}
		set
		{
			UpgradeData();
			data.anchor = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public Vector3 scale
	{
		get
		{
			UpgradeData();
			return data.scale;
		}
		set
		{
			UpgradeData();
			data.scale = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public bool kerning
	{
		get
		{
			UpgradeData();
			return data.kerning;
		}
		set
		{
			UpgradeData();
			data.kerning = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public int maxChars
	{
		get
		{
			UpgradeData();
			return data.maxChars;
		}
		set
		{
			UpgradeData();
			data.maxChars = value;
			SetNeedUpdate(UpdateFlags.UpdateBuffers);
		}
	}

	public int textureGradient
	{
		get
		{
			UpgradeData();
			return data.textureGradient;
		}
		set
		{
			UpgradeData();
			data.textureGradient = value % font.gradientCount;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public bool inlineStyling
	{
		get
		{
			UpgradeData();
			return data.inlineStyling;
		}
		set
		{
			UpgradeData();
			data.inlineStyling = value;
			SetNeedUpdate(UpdateFlags.UpdateText);
		}
	}

	public float Spacing
	{
		get
		{
			UpgradeData();
			return data.spacing;
		}
		set
		{
			UpgradeData();
			if (data.spacing != value)
			{
				data.spacing = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public float LineSpacing
	{
		get
		{
			UpgradeData();
			return data.lineSpacing;
		}
		set
		{
			UpgradeData();
			if (data.lineSpacing != value)
			{
				data.lineSpacing = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public bool IsItalic
	{
		get
		{
			UpgradeData();
			return data.isItalic;
		}
		set
		{
			UpgradeData();
			if (data.isItalic != value)
			{
				data.isItalic = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public float ItalicOffset
	{
		get
		{
			UpgradeData();
			return data.italicOffset;
		}
		set
		{
			UpgradeData();
			if (data.italicOffset != value)
			{
				data.italicOffset = value;
				SetNeedUpdate(UpdateFlags.UpdateText);
			}
		}
	}

	public float Alpha
	{
		get
		{
			return color.a;
		}
		set
		{
			color = new Color(color.r, color.g, color.b, value);
			Commit();
		}
	}

	public int SortingOrder
	{
		get
		{
			return CachedRenderer.sortingOrder;
		}
		set
		{
			if (CachedRenderer.sortingOrder != value)
			{
				data.renderLayer = value;
				CachedRenderer.sortingOrder = value;
			}
		}
	}

	private Renderer CachedRenderer
	{
		get
		{
			if (_cachedRenderer == null)
			{
				_cachedRenderer = base.renderer;
			}
			return _cachedRenderer;
		}
	}

	private bool useInlineStyling
	{
		get
		{
			return inlineStyling && _fontInst.textureGradients;
		}
	}

	public event Action OnTextWillChange;

	public event Action OnTextChanged;

	public event Action OnColorChanged;

	private void UpgradeData()
	{
		if (data.version != 1)
		{
			data.font = _font;
			data.text = _text;
			data.color = _color;
			data.color2 = _color2;
			data.useGradient = _useGradient;
			data.textureGradient = _textureGradient;
			data.anchor = _anchor;
			data.scale = _scale;
			data.kerning = _kerning;
			data.maxChars = _maxChars;
			data.inlineStyling = _inlineStyling;
			data.formatting = _formatting;
			data.wordWrapWidth = _wordWrapWidth;
			data.spacing = spacing;
			data.lineSpacing = lineSpacing;
		}
		data.version = 1;
	}

	private static int GetInlineStyleCommandLength(int cmdSymbol)
	{
		int result = 0;
		switch (cmdSymbol)
		{
		case 99:
			result = 5;
			break;
		case 67:
			result = 9;
			break;
		case 103:
			result = 9;
			break;
		case 71:
			result = 17;
			break;
		}
		return result;
	}

	public string FormatText(string unformattedString)
	{
		string _targetString = string.Empty;
		FormatText(ref _targetString, unformattedString);
		return _targetString;
	}

	private void FormatText()
	{
		FormatText(ref _formattedText, data.text);
	}

	private void FormatText(ref string _targetString, string _source)
	{
		if ((bool)_textMeshController)
		{
			_source = _textMeshController.GetTk2dRepresentationForUnicode(_source);
		}
		InitInstance();
		if (!formatting || wordWrapWidth == 0 || _fontInst.texelSize == Vector2.zero)
		{
			_targetString = _source;
			return;
		}
		float num = _fontInst.texelSize.x * (float)wordWrapWidth;
		StringBuilder stringBuilder = new StringBuilder(_source.Length);
		float num2 = 0f;
		float num3 = 0f;
		int num4 = -1;
		int num5 = -1;
		bool flag = false;
		for (int i = 0; i < _source.Length; i++)
		{
			char c = _source[i];
			bool flag2 = c == '^';
			tk2dFontChar tk2dFontChar2;
			if (_fontInst.useDictionary)
			{
				if (!_fontInst.charDict.ContainsKey(c))
				{
					c = '\0';
				}
				tk2dFontChar2 = _fontInst.charDict[c];
			}
			else
			{
				if (c >= _fontInst.chars.Length)
				{
					c = '\0';
				}
				tk2dFontChar2 = _fontInst.chars[(uint)c];
			}
			if (flag2)
			{
				c = '^';
			}
			if (flag)
			{
				flag = false;
				continue;
			}
			if (data.inlineStyling && c == '^' && i + 1 < _source.Length)
			{
				if (_source[i + 1] != '^')
				{
					int inlineStyleCommandLength = GetInlineStyleCommandLength(_source[i + 1]);
					int num6 = 1 + inlineStyleCommandLength;
					for (int j = 0; j < num6; j++)
					{
						if (i + j < _source.Length)
						{
							stringBuilder.Append(_source[i + j]);
						}
					}
					i += num6 - 1;
					continue;
				}
				flag = true;
				stringBuilder.Append('^');
			}
			switch (c)
			{
			case '\n':
				num2 = 0f;
				num3 = 0f;
				num4 = stringBuilder.Length;
				num5 = i;
				break;
			case ' ':
				num2 += (tk2dFontChar2.advance + data.spacing) * data.scale.x;
				num3 = num2;
				num4 = stringBuilder.Length;
				num5 = i;
				break;
			default:
				if (num2 + tk2dFontChar2.p1.x * data.scale.x > num)
				{
					if (num3 > 0f)
					{
						num3 = 0f;
						num2 = 0f;
						stringBuilder.Remove(num4 + 1, stringBuilder.Length - num4 - 1);
						stringBuilder.Append('\n');
						i = num5;
						continue;
					}
					stringBuilder.Append('\n');
					num2 = (tk2dFontChar2.advance + data.spacing) * data.scale.x;
				}
				else
				{
					num2 += (tk2dFontChar2.advance + data.spacing) * data.scale.x;
				}
				break;
			}
			stringBuilder.Append(c);
		}
		_targetString = stringBuilder.ToString();
	}

	private void SetNeedUpdate(UpdateFlags uf)
	{
		if (updateFlags == UpdateFlags.UpdateNone)
		{
			updateFlags |= uf;
			tk2dUpdateManager.QueueCommit(this);
		}
		else
		{
			updateFlags |= uf;
		}
	}

	private void InitInstance()
	{
		if (data != null && data.font != null)
		{
			_fontInst = data.font.inst;
			_fontInst.InitDictionary();
		}
	}

	private void Awake()
	{
		_textMeshController = base.gameObject.AddComponent<TextMeshController>();
		UpgradeData();
		if (data.font != null)
		{
			_fontInst = data.font.inst;
		}
		updateFlags = UpdateFlags.UpdateBuffers;
		if (data.font != null)
		{
			Init();
			UpdateMaterial();
		}
		updateFlags = UpdateFlags.UpdateNone;
	}

	protected void OnDestroy()
	{
		if (meshFilter == null)
		{
			meshFilter = GetComponent<MeshFilter>();
		}
		if (meshFilter != null)
		{
			mesh = meshFilter.sharedMesh;
		}
		if ((bool)mesh)
		{
			UnityEngine.Object.DestroyImmediate(mesh, true);
			meshFilter.mesh = null;
		}
	}

	public int NumDrawnCharacters()
	{
		int num = NumTotalCharacters();
		if (num > data.maxChars)
		{
			num = data.maxChars;
		}
		return num;
	}

	public int NumTotalCharacters()
	{
		InitInstance();
		if ((updateFlags & (UpdateFlags.UpdateText | UpdateFlags.UpdateBuffers)) != UpdateFlags.UpdateNone)
		{
			FormatText();
		}
		int num = 0;
		for (int i = 0; i < _formattedText.Length; i++)
		{
			int num2 = _formattedText[i];
			bool flag = num2 == 94;
			if (_fontInst.useDictionary)
			{
				if (_fontInst.charDict == null || !_fontInst.charDict.ContainsKey(num2))
				{
					num2 = 0;
				}
			}
			else if (num2 >= _fontInst.chars.Length)
			{
				num2 = 0;
			}
			if (flag)
			{
				num2 = 94;
			}
			if (num2 == 10)
			{
				continue;
			}
			if (data.inlineStyling && num2 == 94 && i + 1 < _formattedText.Length)
			{
				if (_formattedText[i + 1] != '^')
				{
					i += GetInlineStyleCommandLength(_formattedText[i + 1]);
					continue;
				}
				i++;
			}
			num++;
		}
		return num;
	}

	[Obsolete("Use GetEstimatedMeshBoundsForString().size instead")]
	public Vector2 GetMeshDimensionsForString(string str)
	{
		return tk2dTextGeomGen.GetMeshDimensionsForString(str, tk2dTextGeomGen.Data(data, _fontInst, _formattedText));
	}

	public Bounds GetEstimatedMeshBoundsForString(string str)
	{
		InitInstance();
		tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(data, _fontInst, _formattedText);
		Vector2 meshDimensionsForString = tk2dTextGeomGen.GetMeshDimensionsForString(FormatText(str), geomData);
		float yAnchorForHeight = tk2dTextGeomGen.GetYAnchorForHeight(meshDimensionsForString.y, geomData);
		float xAnchorForWidth = tk2dTextGeomGen.GetXAnchorForWidth(meshDimensionsForString.x, geomData);
		float num = (_fontInst.lineHeight + data.lineSpacing) * data.scale.y;
		return new Bounds(new Vector3(xAnchorForWidth + meshDimensionsForString.x * 0.5f, yAnchorForHeight + meshDimensionsForString.y * 0.5f + num, 0f), Vector3.Scale(meshDimensionsForString, new Vector3(1f, -1f, 1f)));
	}

	public void Init(bool force)
	{
		if (force)
		{
			SetNeedUpdate(UpdateFlags.UpdateBuffers);
		}
		Init();
	}

	public void Init()
	{
		if (!_fontInst || ((updateFlags & UpdateFlags.UpdateBuffers) == 0 && !(mesh == null)))
		{
			return;
		}
		_fontInst.InitDictionary();
		FormatText();
		tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(data, _fontInst, _formattedText);
		int numVertices;
		int numIndices;
		tk2dTextGeomGen.GetTextMeshGeomDesc(out numVertices, out numIndices, geomData);
		vertices = new Vector3[numVertices];
		uvs = new Vector2[numVertices];
		colors = new Color32[numVertices];
		untintedColors = new Color32[numVertices];
		if (_fontInst.textureGradients)
		{
			uv2 = new Vector2[numVertices];
		}
		int[] array = new int[numIndices];
		int target = tk2dTextGeomGen.SetTextMeshGeom(vertices, uvs, uv2, untintedColors, 0, geomData);
		if (!_fontInst.isPacked)
		{
			Color32 color = data.color;
			Color32 color2 = ((!data.useGradient) ? data.color : data.color2);
			for (int i = 0; i < numVertices; i++)
			{
				Color32 color3 = ((i % 4 >= 2) ? color2 : color);
				byte b = (byte)(untintedColors[i].r * color3.r / 255);
				byte b2 = (byte)(untintedColors[i].g * color3.g / 255);
				byte b3 = (byte)(untintedColors[i].b * color3.b / 255);
				byte b4 = (byte)(untintedColors[i].a * color3.a / 255);
				if (_fontInst.premultipliedAlpha)
				{
					b = (byte)(b * b4 / 255);
					b2 = (byte)(b2 * b4 / 255);
					b3 = (byte)(b3 * b4 / 255);
				}
				colors[i] = new Color32(b, b2, b3, b4);
			}
		}
		else
		{
			colors = untintedColors;
		}
		tk2dTextGeomGen.SetTextMeshIndices(array, 0, 0, geomData, target);
		if (mesh == null)
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			meshFilter.mesh = mesh;
		}
		else
		{
			mesh.Clear();
		}
		mesh.vertices = vertices;
		mesh.uv = uvs;
		if (font.textureGradients)
		{
			mesh.uv2 = uv2;
		}
		mesh.triangles = array;
		mesh.colors32 = colors;
		mesh.RecalculateBounds();
		mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(mesh.bounds, data.renderLayer);
		updateFlags = UpdateFlags.UpdateNone;
	}

	public void Commit()
	{
		tk2dUpdateManager.FlushQueues();
	}

	public void DoNotUse__CommitInternal()
	{
		InitInstance();
		if (_fontInst == null)
		{
			return;
		}
		_fontInst.InitDictionary();
		bool flag = false;
		bool flag2 = false;
		if ((updateFlags & UpdateFlags.UpdateBuffers) != UpdateFlags.UpdateNone || mesh == null)
		{
			Init();
		}
		else
		{
			if ((updateFlags & UpdateFlags.UpdateText) != UpdateFlags.UpdateNone)
			{
				FormatText();
				if (this.OnTextWillChange != null)
				{
					this.OnTextWillChange();
				}
				tk2dTextGeomGen.GeomData geomData = tk2dTextGeomGen.Data(data, _fontInst, _formattedText);
				int num = tk2dTextGeomGen.SetTextMeshGeom(vertices, uvs, uv2, untintedColors, 0, geomData);
				for (int i = num; i < data.maxChars; i++)
				{
					vertices[i * 4] = (vertices[i * 4 + 1] = (vertices[i * 4 + 2] = (vertices[i * 4 + 3] = Vector3.zero)));
				}
				mesh.vertices = vertices;
				mesh.uv = uvs;
				if (_fontInst.textureGradients)
				{
					mesh.uv2 = uv2;
				}
				if (_fontInst.isPacked)
				{
					colors = untintedColors;
					mesh.colors32 = colors;
				}
				if (data.inlineStyling)
				{
					SetNeedUpdate(UpdateFlags.UpdateColors);
				}
				mesh.RecalculateBounds();
				mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(mesh.bounds, data.renderLayer);
				flag = true;
			}
			if (!font.isPacked && (updateFlags & UpdateFlags.UpdateColors) != UpdateFlags.UpdateNone)
			{
				Color32 color = data.color;
				Color32 color2 = ((!data.useGradient) ? data.color : data.color2);
				for (int j = 0; j < colors.Length; j++)
				{
					Color32 color3 = ((j % 4 >= 2) ? color2 : color);
					byte b = (byte)(untintedColors[j].r * color3.r / 255);
					byte b2 = (byte)(untintedColors[j].g * color3.g / 255);
					byte b3 = (byte)(untintedColors[j].b * color3.b / 255);
					byte b4 = (byte)(untintedColors[j].a * color3.a / 255);
					if (_fontInst.premultipliedAlpha)
					{
						b = (byte)(b * b4 / 255);
						b2 = (byte)(b2 * b4 / 255);
						b3 = (byte)(b3 * b4 / 255);
					}
					colors[j] = new Color32(b, b2, b3, b4);
				}
				mesh.colors32 = colors;
				flag2 = true;
			}
		}
		updateFlags = UpdateFlags.UpdateNone;
		if (flag && this.OnTextChanged != null)
		{
			this.OnTextChanged();
		}
		if (flag2 && this.OnColorChanged != null)
		{
			this.OnColorChanged();
		}
	}

	public void MakePixelPerfect()
	{
		float num = 1f;
		tk2dCamera tk2dCamera2 = tk2dCamera.CameraForLayer(base.gameObject.layer);
		if (tk2dCamera2 != null)
		{
			if (_fontInst.version < 1)
			{
				Debug.LogError("Need to rebuild font.");
			}
			float distance = base.transform.position.z - tk2dCamera2.transform.position.z;
			float num2 = _fontInst.invOrthoSize * _fontInst.halfTargetHeight;
			num = tk2dCamera2.GetSizeAtDistance(distance) * num2;
		}
		else if ((bool)Camera.main)
		{
			if (Camera.main.isOrthoGraphic)
			{
				num = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = base.transform.position.z - Camera.main.transform.position.z;
				num = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fieldOfView, zdist);
			}
			num *= _fontInst.invOrthoSize;
		}
		scale = new Vector3(Mathf.Sign(scale.x) * num, Mathf.Sign(scale.y) * num, Mathf.Sign(scale.z) * num);
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		if (data.font != null && data.font.spriteCollection != null)
		{
			return data.font.spriteCollection == spriteCollection;
		}
		return true;
	}

	private void UpdateMaterial()
	{
		if (base.renderer.sharedMaterial != _fontInst.materialInst)
		{
			base.renderer.material = _fontInst.materialInst;
		}
	}

	public void ForceBuild()
	{
		if (data.font != null)
		{
			_fontInst = data.font.inst;
			UpdateMaterial();
		}
		Init(true);
	}
}
