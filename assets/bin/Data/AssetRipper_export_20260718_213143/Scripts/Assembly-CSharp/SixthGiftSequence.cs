using System.Collections;
using UnityEngine;

public class SixthGiftSequence : OpenGiftSequenceHandler
{
	private const string FIRST_TEXT = "tutorial_sixthgift_firstannouncer_2";

	private const string SECOND_TEXT = "tutorial_sixthgift_firstannouncer_3";

	private const string START_TEXT = "tutorial_sixthgift_firstannouncer_1";

	public override IEnumerator StartSequence()
	{
		if ((bool)announcerSpeechController)
		{
			yield return sceneController.StartCoroutine(announcerSpeechController.ShowAnnouncerAndText(AnnouncerType.MALE_ANNOUNCER, "tutorial_sixthgift_firstannouncer_1", null, -1f, 3f));
		}
	}

	public override IEnumerator GiftOpenedSequence()
	{
		announcerSpeechController.ShowSmallText("tutorial_sixthgift_firstannouncer_2", null, 3f);
		yield return new WaitForSeconds(3f);
		announcerSpeechController.ShowSmallText("tutorial_sixthgift_firstannouncer_3", null, 3f);
		yield return new WaitForSeconds(3f);
	}
}
