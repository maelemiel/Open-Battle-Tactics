public class OpenGiftSceneModel : SceneModel
{
	public GiftType giftType;

	public OpenGiftSceneModel(GiftType giftType, object payload)
	{
		this.giftType = giftType;
		base.payload = payload;
	}
}
