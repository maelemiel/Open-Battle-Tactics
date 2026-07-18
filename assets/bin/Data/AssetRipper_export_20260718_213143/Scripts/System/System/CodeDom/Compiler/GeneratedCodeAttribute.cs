namespace System.CodeDom.Compiler
{
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
	public sealed class GeneratedCodeAttribute : Attribute
	{
		private string tool;

		private string version;

		public string Tool
		{
			get
			{
				return tool;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
		}

		public GeneratedCodeAttribute(string tool, string version)
		{
			this.tool = tool;
			this.version = version;
		}
	}
}
