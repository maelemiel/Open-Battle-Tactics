namespace System.ComponentModel
{
	public interface IComponent : IDisposable
	{
		ISite Site { get; set; }

		event EventHandler Disposed;
	}
}
