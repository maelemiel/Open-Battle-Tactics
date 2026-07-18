using UnityEngine;

public class TextMeshFixedLength : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh textFieldLabel;

	public float characterWidth = 19.16f;

	private string currentValue;

	[SerializeField]
	private Color highlightColor;

	[SerializeField]
	private Color transparentColor;

	private string highlightColorCode;

	private string transparentColorColorCode;

	public int defaultSize = 4;

	public string Text
	{
		get
		{
			return currentValue;
		}
		set
		{
			currentValue = value;
			int length = value.Length;
			if (length > textFieldLabel.maxChars)
			{
				textFieldLabel.maxChars = length;
			}
			int num = 0;
			int num2 = defaultSize - length;
			string text = ((num2 <= 0) ? string.Empty : num.ToString("D" + num2));
			textFieldLabel.text = transparentColorColorCode + text + highlightColorCode + currentValue;
		}
	}

	private void Awake()
	{
		highlightColorCode = highlightColor.InlineStyleCode();
		transparentColorColorCode = transparentColor.InlineStyleCode();
	}

	public void SetLength(int newLength)
	{
		textFieldLabel.maxChars = newLength;
		Text = "0";
	}
}
