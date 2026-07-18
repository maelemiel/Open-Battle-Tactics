using UnityEngine;

[RequireComponent(typeof(tk2dTextMesh))]
public class TextfieldAutosizer : MonoBehaviour
{
	private tk2dTextMesh _textMesh;

	private Bounds originalBounds;

	private Vector3 originalLocalPosition;

	private string originalText;

	private Vector3 originalScale;

	private bool leftAnchored;

	private bool rightAnchored;

	private bool topAnchored;

	private bool bottomAnchored;

	private string lastTextValue;

	public float width = 100f;

	public float height = 40f;

	public bool limitByHeightToo;

	public Bounds LimitBounds
	{
		get
		{
			Bounds result = new Bounds
			{
				size = new Vector3(width, height, 0f)
			};
			float x = 0f;
			if (leftAnchored)
			{
				x = width * 0.5f;
			}
			else if (rightAnchored)
			{
				x = width * -0.5f;
			}
			float y = 0f;
			if (bottomAnchored)
			{
				y = height * 0.5f;
			}
			else if (topAnchored)
			{
				y = height * -0.5f;
			}
			result.center = new Vector3(x, y, 0f);
			return result;
		}
	}

	private void Awake()
	{
		_textMesh = GetComponent<tk2dTextMesh>();
		if (_textMesh == null)
		{
			Log.Error("TextfieldAutosizer does not have a tk2dTextMesh to operate on.", this);
		}
		else
		{
			Initialize();
			_textMesh.OnTextWillChange += OnTextWillChange;
			OnTextWillChange();
		}
	}

	public void SetScale(Vector3 scale)
	{
		originalScale = scale;
	}

	private void Initialize()
	{
		originalBounds = _textMesh.GetEstimatedMeshBoundsForString(_textMesh.text);
		originalLocalPosition = _textMesh.transform.localPosition;
		originalScale = _textMesh.scale;
		UpdateAnchors();
	}

	private void UpdateAnchors()
	{
		leftAnchored = _textMesh.anchor == TextAnchor.LowerLeft || _textMesh.anchor == TextAnchor.MiddleLeft || _textMesh.anchor == TextAnchor.UpperLeft;
		rightAnchored = _textMesh.anchor == TextAnchor.LowerRight || _textMesh.anchor == TextAnchor.MiddleRight || _textMesh.anchor == TextAnchor.UpperRight;
		bottomAnchored = _textMesh.anchor == TextAnchor.LowerLeft || _textMesh.anchor == TextAnchor.LowerCenter || _textMesh.anchor == TextAnchor.LowerRight;
		topAnchored = _textMesh.anchor == TextAnchor.UpperLeft || _textMesh.anchor == TextAnchor.UpperCenter || _textMesh.anchor == TextAnchor.UpperRight;
	}

	[ContextMenu("Reset Scale")]
	private void OnTextWillChange()
	{
		Bounds estimatedMeshBoundsForString = _textMesh.GetEstimatedMeshBoundsForString(_textMesh.text);
		float num = width / estimatedMeshBoundsForString.size.x;
		float a = height / estimatedMeshBoundsForString.size.y;
		float num2 = ((!limitByHeightToo) ? num : Mathf.Min(a, num));
		Vector3 scale = _textMesh.scale;
		_textMesh.scale = new Vector3(Mathf.Min(originalScale.x, num2 * scale.x), Mathf.Min(originalScale.y, num2 * scale.y), 1f);
		if (topAnchored || bottomAnchored)
		{
			float num3 = originalBounds.size.y - estimatedMeshBoundsForString.size.y;
			Vector3 localPosition = _textMesh.transform.localPosition;
			if (topAnchored)
			{
				localPosition.y = originalLocalPosition.y + num3 * -0.5f;
			}
			else if (bottomAnchored)
			{
				localPosition.y = originalLocalPosition.y + num3 * 0.5f;
			}
			_textMesh.transform.localPosition = localPosition;
		}
	}

	public void FitWidth()
	{
		width = _textMesh.GetComponent<MeshFilter>().sharedMesh.bounds.size.x;
	}

	public void FitHeight()
	{
		height = _textMesh.GetComponent<MeshFilter>().sharedMesh.bounds.size.y;
	}

	private void OnDrawGizmosSelected()
	{
		_textMesh = GetComponent<tk2dTextMesh>();
		UpdateAnchors();
		Bounds limitBounds = LimitBounds;
		Vector3 vector = new Vector3(limitBounds.center.x, limitBounds.center.y, 0f);
		Vector3 vector2 = new Vector3(limitBounds.extents.x * 2f, limitBounds.extents.y * 2f, 0.1f);
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color32(byte.MaxValue, 22, 145, byte.MaxValue);
		Gizmos.DrawLine(new Vector3(vector.x - vector2.x * 0.5f, vector.y, vector.z), new Vector3(vector.x + vector2.x * 0.5f, vector.y, vector.z));
		Gizmos.DrawLine(new Vector3(vector.x - vector2.x * 0.5f, vector.y - height * 0.5f, vector.z), new Vector3(vector.x - vector2.x * 0.5f, vector.y + height * 0.5f, vector.z));
		Gizmos.DrawLine(new Vector3(vector.x + vector2.x * 0.5f, vector.y - height * 0.5f, vector.z), new Vector3(vector.x + vector2.x * 0.5f, vector.y + height * 0.5f, vector.z));
	}
}
