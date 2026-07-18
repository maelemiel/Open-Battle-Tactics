namespace System
{
	internal class DefaultUriParser : UriParser
	{
		public DefaultUriParser()
		{
		}

		public DefaultUriParser(string scheme)
		{
			scheme_name = scheme;
		}
	}
}
