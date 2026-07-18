using System.Collections.Generic;

public class CustomFilter : IMessageFilter
{
	public const string STOREDFILTERLIST = "FilteredTags";

	public const string STOREDFILTERTYPE = "FilteredType";

	public static bool show;

	private List<string> blockList;

	public CustomFilter()
	{
		blockList = LoadFilteredList();
	}

	public bool IsTagAllowed(Log.Level level, string tag)
	{
		if (level == Log.Level.Error || level == Log.Level.Warning)
		{
			return true;
		}
		if (blockList != null)
		{
			return (!show) ? (!blockList.Contains(tag.ToLower())) : blockList.Contains(tag.ToLower());
		}
		return true;
	}

	public static List<string> LoadFilteredList()
	{
		return null;
	}
}
