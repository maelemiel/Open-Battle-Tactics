namespace System
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	internal class MonoTODOAttribute : Attribute
	{
		private string comment;

		public string Comment
		{
			get
			{
				return comment;
			}
		}

		public MonoTODOAttribute()
		{
		}

		public MonoTODOAttribute(string comment)
		{
			this.comment = comment;
		}
	}
}
