using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class UnitAbilityText : MonoBehaviour
{
	private tk2dTextMesh textMesh;

	private Sequence seq1;

	private Sequence seq2;

	private Vector3 startingPos;

	private void Start()
	{
		textMesh = GetComponent<tk2dTextMesh>();
		textMesh.Alpha = 0f;
		textMesh.Commit();
		seq1 = (seq2 = new Sequence());
		startingPos = base.transform.localPosition;
	}

	public void Present(string abilityName)
	{
		textMesh.text = abilityName;
		textMesh.Commit();
		base.transform.localPosition = startingPos;
		seq1.Kill();
		seq2.Kill();
		seq2 = new Sequence();
		seq2.Append(HOTween.To(textMesh, 0.5f, new TweenParms().Prop("Alpha", 1).Ease(EaseType.EaseOutBack).OnUpdate(CommitText)));
		seq2.Play();
		seq1 = new Sequence();
		seq1.Append(HOTween.To(base.transform, 0.5f, new TweenParms().Prop("localPosition", new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + 50f, base.transform.localPosition.z)).Ease(EaseType.EaseOutBack)));
		seq1.Play();
	}

	public void Present(string abilityName, float secondsToHide)
	{
		Present(abilityName);
		StartCoroutine(HideCoroutine(secondsToHide + 0.5f));
	}

	private IEnumerator HideCoroutine(float inSeconds)
	{
		yield return new WaitForSeconds(inSeconds);
		Hide();
	}

	public void Hide()
	{
		StopAllCoroutines();
		seq1.Kill();
		seq2.Kill();
		seq2 = new Sequence();
		seq2.Append(HOTween.To(textMesh, 0.5f, new TweenParms().Prop("Alpha", 0).Ease(EaseType.EaseInBack).OnUpdate(CommitText)));
		seq2.Play();
		seq1 = new Sequence();
		seq1.Append(HOTween.To(base.transform, 0.5f, new TweenParms().Prop("localPosition", startingPos).Ease(EaseType.EaseInBack)));
		seq1.Play();
	}

	private void CommitText()
	{
		textMesh.Commit();
	}
}
