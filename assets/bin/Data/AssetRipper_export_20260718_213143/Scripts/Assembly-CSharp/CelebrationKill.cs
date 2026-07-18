using Holoville.HOTween;
using UnityEngine;

public class CelebrationKill : MonoBehaviour
{
	private tk2dTextMesh text;

	private Sequence posSeq;

	private Sequence alphaSeq;

	private void Start()
	{
		text = GetComponent<tk2dTextMesh>();
		text.Alpha = 0f;
	}

	public void CelebrateKill(UnitView unit, string txt, Color clr)
	{
		Vector3 position = unit.BattleField.unityCamera.WorldToScreenPoint(unit.transform.position);
		Vector3 position2 = unit.BattleController.hud.uiCamera.camera.ScreenToWorldPoint(position);
		if (posSeq != null)
		{
			posSeq.Kill();
		}
		if (alphaSeq != null)
		{
			alphaSeq.Kill();
		}
		base.transform.position = position2;
		text.text = txt;
		text.color = clr;
		Animate();
	}

	private void Animate()
	{
		posSeq = new Sequence();
		posSeq.Append(base.transform.TweenLocalYPosition(base.transform.localPosition.y + 50f, 0.2f));
		posSeq.Play();
		alphaSeq = new Sequence();
		alphaSeq.Append(text.TweenAlpha(1f, 0.2f));
		alphaSeq.AppendInterval(0.7f);
		alphaSeq.Append(text.TweenAlpha(0f, 0.2f));
		alphaSeq.Play();
	}
}
