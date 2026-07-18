namespace System.Runtime.InteropServices
{
	[Serializable]
	[ComVisible(true)]
	public sealed class DispatchWrapper
	{
		private object wrappedObject;

		public object WrappedObject
		{
			get
			{
				return wrappedObject;
			}
		}

		public DispatchWrapper(object obj)
		{
			Marshal.GetIDispatchForObject(obj);
			wrappedObject = obj;
		}
	}
}
