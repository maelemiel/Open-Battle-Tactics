using System.Collections;

public class SecondGiftSequence : OpenGiftSequenceHandler
{
	public override IEnumerator StartSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PreSecondGift"));
	}

	public override IEnumerator GiftOpenedSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PostSecondGift"));
	}
}
