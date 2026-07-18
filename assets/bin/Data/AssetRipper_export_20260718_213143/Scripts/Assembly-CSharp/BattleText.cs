using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class BattleText : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh textMesh;

	[SerializeField]
	private tk2dBaseSprite background;

	[SerializeField]
	private float leftPosition = -1000f;

	[SerializeField]
	private float rightPosition = 1000f;

	[SerializeField]
	private float holdWidth = 60f;

	[SerializeField]
	private float inSpeed = 0.3f;

	[SerializeField]
	private float holdSpeed = 0.9f;

	[SerializeField]
	private float outSpeed = 0.3f;

	private Vector3 bgOpenScale;

	private Vector3 bgCloseScale;

	private Sequence textSequence;

	private Sequence bgSequence;

	private float startHeight;

	private EffectInstance messageEffectInstance;

	private float TotalSpeed
	{
		get
		{
			return inSpeed + holdSpeed + outSpeed;
		}
	}

	private void Awake()
	{
		base.gameObject.SetActive(false);
		bgOpenScale = background.scale;
		bgCloseScale = background.scale;
		bgCloseScale.y = 0f;
		startHeight = base.transform.localPosition.y;
	}

	public void ShowMessage(string text)
	{
		ShowMessage(text, Color.white);
	}

	public void ShowMessage(string text, Color color, InBattleMessageType msgType = InBattleMessageType.NONE, float modifyHeight = 0f)
	{
		if (textSequence != null)
		{
			textSequence.Kill();
		}
		if (bgSequence != null)
		{
			bgSequence.Kill();
		}
		base.gameObject.SetActive(true);
		base.transform.localPosition += Vector3.up * modifyHeight;
		textMesh.color = color;
		textMesh.text = text;
		textMesh.transform.localPosition = new Vector3(leftPosition, 0f, 0f);
		background.scale = bgCloseScale;
		textSequence = new Sequence();
		textSequence.Append(textMesh.transform.TweenLocalXPosition(holdWidth * -0.5f, inSpeed, EaseType.Linear));
		textSequence.Append(textMesh.transform.TweenLocalXPosition(holdWidth * 0.5f, holdSpeed, EaseType.Linear));
		textSequence.Append(textMesh.transform.TweenLocalXPosition(rightPosition, outSpeed, EaseType.Linear));
		textSequence.Play();
		bgSequence = new Sequence();
		bgSequence.Append(background.TweenScale(bgOpenScale, inSpeed, EaseType.Linear));
		bgSequence.AppendInterval(holdSpeed);
		bgSequence.Append(background.TweenScale(bgCloseScale, outSpeed, EaseType.Linear));
		bgSequence.AppendCallback(OnComplete);
		bgSequence.Play();
		EffectType effectType = msgType.GetEffectType();
		if (effectType != EffectType.NONE)
		{
			messageEffectInstance = GlobalEffectsManager.Create(effectType, base.gameObject.transform.position + Vector3.forward, base.gameObject).AutoDestroy();
			if ((bool)messageEffectInstance)
			{
				messageEffectInstance.gameObject.transform.localScale = Vector3.zero;
				messageEffectInstance.gameObject.transform.TweenLocalScale(1f, inSpeed);
			}
		}
	}

	public IEnumerator ShowMessageSequence(string text, float textScale)
	{
		Vector3 originalScale = textMesh.scale;
		Vector3 modifiedScale = new Vector3(textScale, textScale, textScale);
		textMesh.scale = modifiedScale;
		ShowMessage(text, Color.white);
		yield return new WaitForSeconds(inSpeed + holdSpeed + outSpeed);
		textMesh.scale = originalScale;
	}

	public IEnumerator ShowMessageSequence(string text, InBattleMessageType msgAnimationType = InBattleMessageType.NONE)
	{
		ShowMessage(text, Color.white, msgAnimationType);
		yield return new WaitForSeconds(inSpeed + holdSpeed);
	}

	public IEnumerator ShowMessageSequence(string text, Color color, InBattleMessageType msgAnimationType = InBattleMessageType.NONE)
	{
		ShowMessage(text, color, msgAnimationType);
		yield return new WaitForSeconds(inSpeed + holdSpeed);
	}

	private void OnComplete()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, startHeight, base.transform.localPosition.z);
		if ((bool)messageEffectInstance)
		{
			messageEffectInstance.gameObject.SetActive(false);
			messageEffectInstance.Destroy();
		}
		base.gameObject.SetActive(false);
	}

	[ContextMenu("Test Message")]
	private void TestMessage()
	{
		ShowMessage("Test Message", Color.red);
	}

	private void OnDestroy()
	{
		if (textSequence != null)
		{
			textSequence.Kill();
		}
		if (bgSequence != null)
		{
			bgSequence.Kill();
		}
	}
}
