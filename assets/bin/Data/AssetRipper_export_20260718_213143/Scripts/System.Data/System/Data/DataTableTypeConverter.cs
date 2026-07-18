using System.ComponentModel;

namespace System.Data
{
	internal class DataTableTypeConverter : ReferenceConverter
	{
		public DataTableTypeConverter()
			: base(typeof(DataTable))
		{
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false;
		}
	}
}
