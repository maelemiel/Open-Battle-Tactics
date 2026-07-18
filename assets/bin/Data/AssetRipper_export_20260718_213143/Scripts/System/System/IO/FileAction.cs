namespace System.IO
{
	internal enum FileAction
	{
		Added = 1,
		Removed = 2,
		Modified = 3,
		RenamedOldName = 4,
		RenamedNewName = 5
	}
}
