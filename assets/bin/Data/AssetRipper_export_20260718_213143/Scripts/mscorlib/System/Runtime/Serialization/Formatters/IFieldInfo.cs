using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.Formatters
{
	[ComVisible(true)]
	public interface IFieldInfo
	{
		string[] FieldNames { get; set; }

		Type[] FieldTypes { get; set; }
	}
}
