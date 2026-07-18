using System.Collections;
using UnityEngine;

public class TestAutosizeTextfields : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh[] textfields;

	public float addInterval = 1f;

	public int maxChars = 10;

	public int dir = 1;

	private int chars;

	private void Start()
	{
		StartCoroutine(AddSequence());
	}

	private IEnumerator AddSequence()
	{
		while (true)
		{
			tk2dTextMesh[] array = textfields;
			foreach (tk2dTextMesh field in array)
			{
				if (dir == 1)
				{
					field.text += Random.Range(0, 10);
				}
				else
				{
					field.text = field.text.Substring(0, field.text.Length - 1);
				}
			}
			chars++;
			if (chars >= maxChars)
			{
				if (dir == 1)
				{
					dir = -1;
				}
				else
				{
					dir = 1;
				}
				chars = 0;
			}
			yield return new WaitForSeconds(addInterval);
		}
	}

	private void Update()
	{
	}
}
