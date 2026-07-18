public static class SpeechTypeExtensions
{
	public static string GetSpeechSpriteName(this SpeechType speechType)
	{
		switch (speechType)
		{
		case SpeechType.NORMAL:
			return "SpeechBuble_Talk";
		case SpeechType.EXCITED:
			return "SpeechBuble_Scream";
		default:
			return string.Empty;
		}
	}
}
