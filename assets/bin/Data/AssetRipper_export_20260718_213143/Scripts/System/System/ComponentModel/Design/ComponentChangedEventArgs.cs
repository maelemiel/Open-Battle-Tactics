using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public sealed class ComponentChangedEventArgs : EventArgs
	{
		private object component;

		private MemberDescriptor member;

		private object oldValue;

		private object newValue;

		public object Component
		{
			get
			{
				return component;
			}
		}

		public MemberDescriptor Member
		{
			get
			{
				return member;
			}
		}

		public object NewValue
		{
			get
			{
				return oldValue;
			}
		}

		public object OldValue
		{
			get
			{
				return newValue;
			}
		}

		public ComponentChangedEventArgs(object component, MemberDescriptor member, object oldValue, object newValue)
		{
			this.component = component;
			this.member = member;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
	}
}
