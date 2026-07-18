using Holoville.HOTween;
using UnityEngine;

public class HealthUi : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh healthText;

	private float startingHealth;

	private float damageFeedbackTime = 2f;

	public tk2dTextMesh damageFeedback;

	private Sequence animateLabelSequence;

	private IHOTweenComponent anim0;

	private IHOTweenComponent anim1;

	private IHOTweenComponent anim2;

	private Vector3 localPos;

	public void Start()
	{
		localPos = damageFeedback.transform.localPosition;
		float y = localPos.y + 150f;
		localPos = new Vector3(localPos.x, y, localPos.z);
		damageFeedback.gameObject.SetActive(false);
	}

	public void SetUp(int totalHealth, int armor)
	{
		startingHealth = totalHealth;
		UpdateHealth(totalHealth, armor);
	}

	public void UpdateDamageFeedback(int damage)
	{
		damageFeedback.gameObject.SetActive(true);
		damageFeedback.text = "-" + damage;
		AnimateDamageFeedback();
	}

	private void AnimateDamageFeedback()
	{
		if (anim0 != null)
		{
			anim0.Restart();
		}
		else
		{
			anim0 = HOTween.To(damageFeedback.transform, damageFeedbackTime, new TweenParms().AutoKill(false).Ease(EaseType.EaseOutBack, damageFeedbackTime * 0.5f).Prop("localScale", new Vector3(2f, 2f, 1f))
				.OnComplete(OnAnimateDone));
		}
		if (anim1 != null)
		{
			anim1.Restart();
		}
		else
		{
			anim1 = HOTween.To(damageFeedback.transform, damageFeedbackTime, new TweenParms().AutoKill(false).Ease(EaseType.Linear).Prop("localPosition", localPos));
		}
		if (anim2 != null)
		{
			anim2.Restart();
		}
		else
		{
			anim2 = HOTween.To(damageFeedback, damageFeedbackTime * 0.5f, new TweenParms().AutoKill(false).Ease(EaseType.Linear).Delay(damageFeedbackTime * 0.5f)
				.Prop("color", new Color(0f, 0f, 0f, 0f)));
		}
	}

	private void OnAnimateDone()
	{
		damageFeedback.gameObject.SetActive(false);
	}

	public void SetVisible(bool currentState)
	{
		base.gameObject.SetActive(currentState);
	}

	public void UpdateHealth(int health, int armor)
	{
		int num = health;
		if (armor > 0)
		{
			num += armor;
		}
		if ((bool)healthText)
		{
			healthText.text = num.ToString();
		}
	}
}
