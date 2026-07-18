using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public sealed class ComponentChangingEventArgs : EventArgs
	{
		private object component;

		private MemberDescriptor member;

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

		public ComponentChangingEventArgs(object component, MemberDescriptor member)
		{
			this.component = component;
			this.member = member;
		}
	}
}
