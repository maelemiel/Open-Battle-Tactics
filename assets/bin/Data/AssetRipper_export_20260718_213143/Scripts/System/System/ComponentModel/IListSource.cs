using System.Collections;

namespace System.ComponentModel
{
	[TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	[MergableProperty(false)]
	public interface IListSource
	{
		bool ContainsListCollection { get; }

		IList GetList();
	}
}
