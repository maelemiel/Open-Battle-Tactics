using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
	[ComVisible(false)]
	public sealed class LinkedListNode<T>
	{
		private T item;

		private LinkedList<T> container;

		internal LinkedListNode<T> forward;

		internal LinkedListNode<T> back;

		public LinkedList<T> List
		{
			get
			{
				return container;
			}
		}

		public LinkedListNode<T> Next
		{
			get
			{
				return (container == null || forward == container.first) ? null : forward;
			}
		}

		public LinkedListNode<T> Previous
		{
			get
			{
				return (container == null || this == container.first) ? null : back;
			}
		}

		public T Value
		{
			get
			{
				return item;
			}
			set
			{
				item = value;
			}
		}

		public LinkedListNode(T value)
		{
			item = value;
		}

		internal LinkedListNode(LinkedList<T> list, T value)
		{
			container = list;
			item = value;
			back = (forward = this);
		}

		internal LinkedListNode(LinkedList<T> list, T value, LinkedListNode<T> previousNode, LinkedListNode<T> nextNode)
		{
			container = list;
			item = value;
			back = previousNode;
			forward = nextNode;
			previousNode.forward = this;
			nextNode.back = this;
		}

		internal void Detach()
		{
			back.forward = forward;
			forward.back = back;
			forward = (back = null);
			container = null;
		}

		internal void SelfReference(LinkedList<T> list)
		{
			forward = this;
			back = this;
			container = list;
		}

		internal void InsertBetween(LinkedListNode<T> previousNode, LinkedListNode<T> nextNode, LinkedList<T> list)
		{
			previousNode.forward = this;
			nextNode.back = this;
			forward = nextNode;
			back = previousNode;
			container = list;
		}
	}
}
