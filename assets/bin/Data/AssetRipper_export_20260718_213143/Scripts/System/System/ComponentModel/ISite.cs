using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public interface ISite : IServiceProvider
	{
		IComponent Component { get; }

		IContainer Container { get; }

		bool DesignMode { get; }

		string Name { get; set; }
	}
}
