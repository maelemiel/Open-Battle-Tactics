namespace System.Security.AccessControl
{
	public sealed class FileSecurity : FileSystemSecurity
	{
		public FileSecurity()
			: base(false)
		{
			throw new PlatformNotSupportedException();
		}

		public FileSecurity(string fileName, AccessControlSections includeSections)
			: base(false, fileName, includeSections)
		{
			throw new PlatformNotSupportedException();
		}
	}
}
