using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public class ManifestResourceInfo
	{
		private Assembly _assembly;

		private string _filename;

		private ResourceLocation _location;

		public virtual string FileName
		{
			get
			{
				return _filename;
			}
		}

		public virtual Assembly ReferencedAssembly
		{
			get
			{
				return _assembly;
			}
		}

		public virtual ResourceLocation ResourceLocation
		{
			get
			{
				return _location;
			}
		}

		internal ManifestResourceInfo()
		{
		}

		internal ManifestResourceInfo(Assembly assembly, string filename, ResourceLocation location)
		{
			_assembly = assembly;
			_filename = filename;
			_location = location;
		}
	}
}
