namespace System.ComponentModel
{
	internal sealed class WeakObjectWrapper
	{
		public int TargetHashCode { get; private set; }

		public WeakReference Weak { get; private set; }

		public WeakObjectWrapper(object target)
		{
			TargetHashCode = target.GetHashCode();
			Weak = new WeakReference(target);
		}
	}
}
