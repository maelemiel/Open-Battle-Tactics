namespace System.Security.AccessControl
{
	public sealed class DirectorySecurity : FileSystemSecurity
	{
		public DirectorySecurity()
			: base(true)
		{
			throw new PlatformNotSupportedException();
		}

		public DirectorySecurity(string name, AccessControlSections includeSections)
			: base(true, name, includeSections)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
