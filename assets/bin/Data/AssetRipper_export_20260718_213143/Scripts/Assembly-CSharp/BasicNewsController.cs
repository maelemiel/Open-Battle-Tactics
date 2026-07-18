using System.Collections;
using UnityEngine;

public class BasicNewsController : NewsController
{
	[SerializeField]
	private tk2dSpineAnimation _logoAnimation;

	[SerializeField]
	private GameObject _chineseLogo;

	[SerializeField]
	private tk2dTextMesh _tmText;

	[SerializeField]
	private GameObject _environmentalEffect;

	private Vector3 _announcerOnscreenPos;

	protected override void Awake()
	{
		base.Awake();
		if ((bool)_tmText)
		{
			_tmText.gameObject.SetActive(false);
		}
	}

	public override IEnumerator Init(NewsDataModel newsDM = null)
	{
		yield return StartCoroutine(base.Init(newsDM));
		if (LocalizationManager.LanguageEncodingType() == LocalizationManager.LanguageEncoding.Unicode && Singleton<LocalizationManager>.instance.currentLanguageFromEnum == LocalizationManager.Language.Chinese)
		{
			if ((bool)_logoAnimation)
			{
				_logoAnimation.gameObject.SetActive(false);
			}
			if ((bool)_chineseLogo)
			{
				_chineseLogo.SetActive(true);
			}
		}
		else
		{
			_logoAnimation.gameObject.SetActive(true);
			yield return new WaitForSeconds(1.5f);
			if ((bool)_tmText)
			{
				_tmText.gameObject.SetActive(true);
			}
		}
	}
}
