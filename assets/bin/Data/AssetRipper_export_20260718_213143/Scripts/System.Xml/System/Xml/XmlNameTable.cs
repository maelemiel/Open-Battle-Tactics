namespace System.Xml
{
	public abstract class XmlNameTable
	{
		public abstract string Add(string name);

		public abstract string Add(char[] buffer, int offset, int length);

		public abstract string Get(string name);

		public abstract string Get(char[] buffer, int offset, int length);
	}
}
