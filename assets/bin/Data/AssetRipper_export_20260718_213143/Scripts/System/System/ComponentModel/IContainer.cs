using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public interface IContainer : IDisposable
	{
		ComponentCollection Components { get; }

		void Add(IComponent component);

		void Add(IComponent component, string name);

		void Remove(IComponent component);
	}
}
