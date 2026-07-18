using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public abstract class EventDescriptor : MemberDescriptor
	{
		public abstract Type ComponentType { get; }

		public abstract Type EventType { get; }

		public abstract bool IsMulticast { get; }

		protected EventDescriptor(MemberDescriptor desc)
			: base(desc)
		{
		}

		protected EventDescriptor(MemberDescriptor desc, Attribute[] attrs)
			: base(desc, attrs)
		{
		}

		protected EventDescriptor(string str, Attribute[] attrs)
			: base(str, attrs)
		{
		}

		public abstract void AddEventHandler(object component, Delegate value);

		public abstract void RemoveEventHandler(object component, Delegate value);
	}
}
