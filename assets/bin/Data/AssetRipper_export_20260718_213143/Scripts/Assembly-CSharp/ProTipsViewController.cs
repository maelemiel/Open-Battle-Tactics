using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProTipsViewController : MonoBehaviour
{
	public float timePerCharacter = 0.1f;

	public float textFadeTime = 1f;

	public string titleStringID = string.Empty;

	public bool randomItems = true;

	[SerializeField]
	private tk2dTextMesh proTipsLabel;

	[SerializeField]
	private int textPosition;

	private Dictionary<string, string> proTipsLocalized = new Dictionary<string, string>();

	private List<string> dictionaryKeys = new List<string>();

	public Color proTipColor = Color.white;

	public Color textColor = Color.white;

	private string prefixText = string.Empty;

	private string proTipColorInlinceCode = string.Empty;

	private string textColorInlineCode = string.Empty;

	private int currentIndex;

	public void Init(LinesType type)
	{
		if (!proTipsLabel)
		{
			Log.Error("No Label found on ProTipsViewController", base.gameObject);
			return;
		}
		switch (type)
		{
		case LinesType.NEWS:
		{
			List<LinesNewsDataModel> all2 = LinesNewsDataModel.GetAll();
			foreach (LinesNewsDataModel item in all2)
			{
				proTipsLocalized.Add(item.newsKeyString, item.newsKeyString.Localize(string.Empty));
			}
			break;
		}
		case LinesType.PRO_TIPS:
		{
			List<LinesProTipsDataModel> all = LinesProTipsDataModel.GetAll();
			foreach (LinesProTipsDataModel item2 in all)
			{
				proTipsLocalized.Add(item2.proTipsKeyString, item2.proTipsKeyString.Localize(string.Empty));
			}
			break;
		}
		}
		if (proTipsLocalized.Keys.Count > 0)
		{
			dictionaryKeys = new List<string>(proTipsLocalized.Keys);
			prefixText = titleStringID.Localize("HEADER");
			proTipColorInlinceCode = proTipColor.InlineStyleCode();
			textColorInlineCode = textColor.InlineStyleCode();
		}
		else
		{
			base.gameObject.SetActive(false);
			Log.Warning("No ProTips have been found in Metadata", base.gameObject);
		}
	}

	private void OnEnable()
	{
		StartCoroutine(BeginProTipsSequence());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private IEnumerator BeginProTipsSequence()
	{
		while (true)
		{
			yield return StartCoroutine(ShowProTip());
		}
	}

	private IEnumerator ShowProTip()
	{
		if (dictionaryKeys.Count > 0)
		{
			int randomIndex = ((!randomItems) ? (currentIndex++ % dictionaryKeys.Count) : Random.Range(0, dictionaryKeys.Count));
			string key = dictionaryKeys[randomIndex];
			proTipsLabel.text = textColorInlineCode + proTipsLocalized[key];
			proTipsLabel.transform.SetLocalXPosition(-1280f);
			float waitTime = timePerCharacter * (float)proTipsLabel.text.Length;
			proTipsLabel.transform.TweenLocalXPosition(textPosition, 1f);
			yield return new WaitForSeconds(waitTime);
			proTipsLabel.transform.TweenLocalXPosition(1280f, 1f);
			yield return new WaitForSeconds(textFadeTime);
		}
	}
}
