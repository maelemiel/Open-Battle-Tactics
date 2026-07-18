namespace System.Text.RegularExpressions
{
	[Serializable]
	public class RegexCompilationInfo
	{
		private string pattern;

		private string name;

		private string nspace;

		private RegexOptions options;

		private bool isPublic;

		public bool IsPublic
		{
			get
			{
				return isPublic;
			}
			set
			{
				isPublic = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Name");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("Name");
				}
				name = value;
			}
		}

		public string Namespace
		{
			get
			{
				return nspace;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Namespace");
				}
				nspace = value;
			}
		}

		public RegexOptions Options
		{
			get
			{
				return options;
			}
			set
			{
				options = value;
			}
		}

		public string Pattern
		{
			get
			{
				return pattern;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("pattern");
				}
				pattern = value;
			}
		}

		public RegexCompilationInfo(string pattern, RegexOptions options, string name, string fullnamespace, bool ispublic)
		{
			Pattern = pattern;
			Options = options;
			Name = name;
			Namespace = fullnamespace;
			IsPublic = ispublic;
		}
	}
}
