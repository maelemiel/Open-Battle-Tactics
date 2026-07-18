using System.Collections;

public class ThirdGiftSequence : OpenGiftSequenceHandler
{
	public override IEnumerator StartSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PreThirdGift"));
	}

	public override IEnumerator GiftOpenedSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("PostThirdGift"));
	}
}
