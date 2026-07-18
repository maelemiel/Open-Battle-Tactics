namespace Spine
{
	public interface AttachmentLoader
	{
		Attachment NewAttachment(Skin skin, AttachmentType type, string name);
	}
}
