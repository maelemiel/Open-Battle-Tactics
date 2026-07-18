namespace System.Text.RegularExpressions
{
	internal class MRUList
	{
		private class Node
		{
			public object value;

			public Node previous;

			public Node next;

			public Node(object value)
			{
				this.value = value;
			}
		}

		private Node head;

		private Node tail;

		public MRUList()
		{
			head = (tail = null);
		}

		public void Use(object o)
		{
			Node node;
			if (head == null)
			{
				node = new Node(o);
				head = (tail = node);
				return;
			}
			node = head;
			while (node != null && !o.Equals(node.value))
			{
				node = node.previous;
			}
			if (node == null)
			{
				node = new Node(o);
			}
			else
			{
				if (node == head)
				{
					return;
				}
				if (node == tail)
				{
					tail = node.next;
				}
				else
				{
					node.previous.next = node.next;
				}
				node.next.previous = node.previous;
			}
			head.next = node;
			node.previous = head;
			node.next = null;
			head = node;
		}

		public object Evict()
		{
			if (tail == null)
			{
				return null;
			}
			object value = tail.value;
			tail = tail.next;
			if (tail == null)
			{
				head = null;
			}
			else
			{
				tail.previous = null;
			}
			return value;
		}
	}
}
