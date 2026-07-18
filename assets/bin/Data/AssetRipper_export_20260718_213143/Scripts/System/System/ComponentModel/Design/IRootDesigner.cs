using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public interface IRootDesigner : IDisposable, IDesigner
	{
		ViewTechnology[] SupportedTechnologies { get; }

		object GetView(ViewTechnology technology);
	}
}
