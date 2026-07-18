namespace System
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal class MonoNotSupportedAttribute : System.MonoTODOAttribute
	{
		public MonoNotSupportedAttribute(string comment)
			: base(comment)
		{
		}
	}
}
