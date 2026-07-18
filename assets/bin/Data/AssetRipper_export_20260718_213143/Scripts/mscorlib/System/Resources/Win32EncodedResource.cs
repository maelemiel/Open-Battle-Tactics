using System.IO;

namespace System.Resources
{
	internal class Win32EncodedResource : Win32Resource
	{
		private byte[] data;

		public byte[] Data
		{
			get
			{
				return data;
			}
		}

		internal Win32EncodedResource(NameOrId type, NameOrId name, int language, byte[] data)
			: base(type, name, language)
		{
			this.data = data;
		}

		public override void WriteTo(Stream s)
		{
			s.Write(data, 0, data.Length);
		}
	}
}
