using System.Collections;

namespace System.IO
{
	internal class FAMData
	{
		public FileSystemWatcher FSW;

		public string Directory;

		public string FileMask;

		public bool IncludeSubdirs;

		public bool Enabled;

		public System.IO.FAMRequest Request;

		public Hashtable SubDirs;
	}
}
