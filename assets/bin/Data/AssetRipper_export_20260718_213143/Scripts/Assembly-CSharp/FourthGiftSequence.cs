using System.Collections;

public class FourthGiftSequence : OpenGiftSequenceHandler
{
	public override IEnumerator StartSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PreFourthGift"));
	}

	public override IEnumerator GiftOpenedSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PostFourthGift"));
	}
}
