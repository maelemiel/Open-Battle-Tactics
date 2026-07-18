namespace System.Collections.Generic
{
	public interface IEnumerator<T> : IEnumerator, IDisposable
	{
		new T Current { get; }
	}
}
