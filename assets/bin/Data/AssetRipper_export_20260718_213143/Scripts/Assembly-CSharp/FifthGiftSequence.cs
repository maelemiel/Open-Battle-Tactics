using System.Collections;

public class FifthGiftSequence : OpenGiftSequenceHandler
{
	public override IEnumerator GiftOpenedSequence()
	{
		yield return sceneController.StartCoroutine(AnnouncerController.DialogTrigger("FifthGift"));
	}
}
