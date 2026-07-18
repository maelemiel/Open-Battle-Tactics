namespace System
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal class MonoDocumentationNoteAttribute : System.MonoTODOAttribute
	{
		public MonoDocumentationNoteAttribute(string comment)
			: base(comment)
		{
		}
	}
}
