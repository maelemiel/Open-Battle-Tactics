public interface IMessageFilter
{
	bool IsTagAllowed(Log.Level level, string tag);
}
