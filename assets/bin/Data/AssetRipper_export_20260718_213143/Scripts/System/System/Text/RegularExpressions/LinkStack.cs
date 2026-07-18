using System.Collections;

namespace System.Text.RegularExpressions
{
	internal abstract class LinkStack : System.Text.RegularExpressions.LinkRef
	{
		private Stack stack;

		public LinkStack()
		{
			stack = new Stack();
		}

		public void Push()
		{
			stack.Push(GetCurrent());
		}

		public bool Pop()
		{
			if (stack.Count > 0)
			{
				SetCurrent(stack.Pop());
				return true;
			}
			return false;
		}

		protected abstract object GetCurrent();

		protected abstract void SetCurrent(object l);
	}
}
