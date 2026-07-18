using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class StaticDiskDataSource : IStaticDataSource
	{
		private string fileName_;

		public StaticDiskDataSource(string fileName)
		{
			fileName_ = fileName;
		}

		public Stream GetSource()
		{
			return File.OpenRead(fileName_);
		}
	}
}
