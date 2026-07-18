public interface IMessageFormatter
{
	string GetFormattedString(Log.Level level, string tag, string message, params object[] args);
}
