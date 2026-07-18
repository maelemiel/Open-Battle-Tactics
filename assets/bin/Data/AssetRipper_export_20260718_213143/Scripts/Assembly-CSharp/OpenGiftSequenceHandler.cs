using System.Collections;

public abstract class OpenGiftSequenceHandler
{
	protected OpenGiftNotificationController sceneController;

	public AnnouncerSpeechController announcerSpeechController;

	public virtual void Init(OpenGiftNotificationController sceneController, AnnouncerSpeechController announcerSpeechController)
	{
		this.sceneController = sceneController;
		this.announcerSpeechController = announcerSpeechController;
	}

	public virtual IEnumerator GiftOpenedSequence()
	{
		yield break;
	}

	public virtual IEnumerator StartSequence()
	{
		yield break;
	}

	public virtual IEnumerator EndSequence()
	{
		yield break;
	}
}
