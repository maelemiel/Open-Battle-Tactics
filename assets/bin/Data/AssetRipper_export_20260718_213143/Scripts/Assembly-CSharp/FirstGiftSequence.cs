using System.Collections;

public class FirstGiftSequence : OpenGiftSequenceHandler
{
	public override IEnumerator StartSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PreFirstGift"));
	}

	public override IEnumerator GiftOpenedSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PostFirstGift"));
	}
}
