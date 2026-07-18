using System.IO;

namespace Microsoft.SqlServer.Server
{
	public interface IBinarySerialize
	{
		void Read(BinaryReader r);

		void Write(BinaryWriter r);
	}
}
