namespace System.Collections.Generic
{
	internal class RBTree : IEnumerable, IEnumerable<System.Collections.Generic.RBTree.Node>
	{
		public interface INodeHelper<T>
		{
			int Compare(T key, Node node);

			Node CreateNode(T key);
		}

		public abstract class Node
		{
			private const uint black_mask = 1u;

			private const int black_shift = 1;

			public Node left;

			public Node right;

			private uint size_black;

			public bool IsBlack
			{
				get
				{
					return (size_black & 1) == 1;
				}
				set
				{
					size_black = ((!value) ? (size_black & 0xFFFFFFFEu) : (size_black | 1));
				}
			}

			public uint Size
			{
				get
				{
					return size_black >> 1;
				}
				set
				{
					size_black = (value << 1) | (size_black & 1);
				}
			}

			public Node()
			{
				size_black = 2u;
			}

			public uint FixSize()
			{
				Size = 1u;
				if (left != null)
				{
					Size += left.Size;
				}
				if (right != null)
				{
					Size += right.Size;
				}
				return Size;
			}

			public abstract void SwapValue(Node other);
		}

		public struct NodeEnumerator : IEnumerator, IDisposable, IEnumerator<Node>
		{
			private System.Collections.Generic.RBTree tree;

			private uint version;

			private Stack<Node> pennants;

			object IEnumerator.Current
			{
				get
				{
					check_current();
					return Current;
				}
			}

			public Node Current
			{
				get
				{
					return pennants.Peek();
				}
			}

			internal NodeEnumerator(System.Collections.Generic.RBTree tree)
			{
				this.tree = tree;
				version = tree.version;
				pennants = null;
			}

			public void Reset()
			{
				check_version();
				pennants = null;
			}

			public bool MoveNext()
			{
				check_version();
				Node node;
				if (pennants == null)
				{
					if (tree.root == null)
					{
						return false;
					}
					pennants = new Stack<Node>();
					node = tree.root;
				}
				else
				{
					if (pennants.Count == 0)
					{
						return false;
					}
					Node node2 = pennants.Pop();
					node = node2.right;
				}
				while (node != null)
				{
					pennants.Push(node);
					node = node.left;
				}
				return pennants.Count != 0;
			}

			public void Dispose()
			{
				tree = null;
				pennants = null;
			}

			private void check_version()
			{
				if (tree == null)
				{
					throw new ObjectDisposedException("enumerator");
				}
				if (version != tree.version)
				{
					throw new InvalidOperationException("tree modified");
				}
			}

			internal void check_current()
			{
				check_version();
				if (pennants == null)
				{
					throw new InvalidOperationException("state invalid before the first MoveNext()");
				}
			}
		}

		private Node root;

		private object hlp;

		private uint version;

		[ThreadStatic]
		private static List<Node> cached_path;

		public int Count
		{
			get
			{
				return (int)((root != null) ? root.Size : 0);
			}
		}

		public Node this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
				{
					throw new IndexOutOfRangeException("index");
				}
				Node node = root;
				while (node != null)
				{
					int num = (int)((node.left != null) ? node.left.Size : 0);
					if (index == num)
					{
						return node;
					}
					if (index < num)
					{
						node = node.left;
						continue;
					}
					index -= num + 1;
					node = node.right;
				}
				throw new SystemException("Internal Error: index calculation");
			}
		}

		public RBTree(object hlp)
		{
			this.hlp = hlp;
		}

		IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private static List<Node> alloc_path()
		{
			if (cached_path == null)
			{
				return new List<Node>();
			}
			List<Node> result = cached_path;
			cached_path = null;
			return result;
		}

		private static void release_path(List<Node> path)
		{
			if (cached_path == null || cached_path.Capacity < path.Capacity)
			{
				path.Clear();
				cached_path = path;
			}
		}

		public void Clear()
		{
			root = null;
			version++;
		}

		public Node Intern<T>(T key, Node new_node)
		{
			if (root == null)
			{
				if (new_node == null)
				{
					new_node = ((INodeHelper<T>)hlp).CreateNode(key);
				}
				root = new_node;
				root.IsBlack = true;
				version++;
				return root;
			}
			List<Node> list = alloc_path();
			int in_tree_cmp = find_key(key, list);
			Node node = list[list.Count - 1];
			if (node == null)
			{
				if (new_node == null)
				{
					new_node = ((INodeHelper<T>)hlp).CreateNode(key);
				}
				node = do_insert(in_tree_cmp, new_node, list);
			}
			release_path(list);
			return node;
		}

		public Node Remove<T>(T key)
		{
			if (root == null)
			{
				return null;
			}
			List<Node> path = alloc_path();
			int num = find_key(key, path);
			Node result = null;
			if (num == 0)
			{
				result = do_remove(path);
			}
			release_path(path);
			return result;
		}

		public Node Lookup<T>(T key)
		{
			INodeHelper<T> nodeHelper = (INodeHelper<T>)hlp;
			Node node = root;
			while (node != null)
			{
				int num = nodeHelper.Compare(key, node);
				if (num == 0)
				{
					break;
				}
				node = ((num >= 0) ? node.right : node.left);
			}
			return node;
		}

		public NodeEnumerator GetEnumerator()
		{
			return new NodeEnumerator(this);
		}

		private int find_key<T>(T key, List<Node> path)
		{
			INodeHelper<T> nodeHelper = (INodeHelper<T>)hlp;
			int num = 0;
			Node node = null;
			Node node2 = root;
			if (path != null)
			{
				path.Add(root);
			}
			while (node2 != null)
			{
				num = nodeHelper.Compare(key, node2);
				if (num == 0)
				{
					return num;
				}
				if (num < 0)
				{
					node = node2.right;
					node2 = node2.left;
				}
				else
				{
					node = node2.left;
					node2 = node2.right;
				}
				if (path != null)
				{
					path.Add(node);
					path.Add(node2);
				}
			}
			return num;
		}

		private Node do_insert(int in_tree_cmp, Node current, List<Node> path)
		{
			path[path.Count - 1] = current;
			Node node = path[path.Count - 3];
			if (in_tree_cmp < 0)
			{
				node.left = current;
			}
			else
			{
				node.right = current;
			}
			for (int i = 0; i < path.Count - 2; i += 2)
			{
				path[i].Size++;
			}
			if (!node.IsBlack)
			{
				rebalance_insert(path);
			}
			if (!root.IsBlack)
			{
				throw new SystemException("Internal error: root is not black");
			}
			version++;
			return current;
		}

		private Node do_remove(List<Node> path)
		{
			int index = path.Count - 1;
			Node node = path[index];
			if (node.left != null)
			{
				Node node2 = right_most(node.left, node.right, path);
				node.SwapValue(node2);
				if (node2.left != null)
				{
					Node left = node2.left;
					path.Add(null);
					path.Add(left);
					node2.SwapValue(left);
				}
			}
			else if (node.right != null)
			{
				Node right = node.right;
				path.Add(null);
				path.Add(right);
				node.SwapValue(right);
			}
			index = path.Count - 1;
			node = path[index];
			if (node.Size != 1)
			{
				throw new SystemException("Internal Error: red-black violation somewhere");
			}
			path[index] = null;
			node_reparent((index != 0) ? path[index - 2] : null, node, 0u, null);
			for (int i = 0; i < path.Count - 2; i += 2)
			{
				path[i].Size--;
			}
			if (index != 0 && node.IsBlack)
			{
				rebalance_delete(path);
			}
			if (root != null && !root.IsBlack)
			{
				throw new SystemException("Internal Error: root is not black");
			}
			version++;
			return node;
		}

		private void rebalance_insert(List<Node> path)
		{
			int num = path.Count - 1;
			do
			{
				if (path[num - 3] == null || path[num - 3].IsBlack)
				{
					rebalance_insert__rotate_final(num, path);
					break;
				}
				Node node = path[num - 2];
				bool isBlack = true;
				path[num - 3].IsBlack = isBlack;
				node.IsBlack = isBlack;
				num -= 4;
				if (num == 0)
				{
					break;
				}
				path[num].IsBlack = false;
			}
			while (!path[num - 2].IsBlack);
		}

		private void rebalance_delete(List<Node> path)
		{
			int num = path.Count - 1;
			do
			{
				Node node = path[num - 1];
				if (!node.IsBlack)
				{
					num = ensure_sibling_black(num, path);
					node = path[num - 1];
				}
				if ((node.left != null && !node.left.IsBlack) || (node.right != null && !node.right.IsBlack))
				{
					rebalance_delete__rotate_final(num, path);
					return;
				}
				node.IsBlack = false;
				num -= 2;
				if (num == 0)
				{
					return;
				}
			}
			while (path[num].IsBlack);
			path[num].IsBlack = true;
		}

		private void rebalance_insert__rotate_final(int curpos, List<Node> path)
		{
			Node node = path[curpos];
			Node node2 = path[curpos - 2];
			Node node3 = path[curpos - 4];
			uint size = node3.Size;
			bool flag = node2 == node3.left;
			bool flag2 = node == node2.left;
			Node node4;
			if (flag && flag2)
			{
				node3.left = node2.right;
				node2.right = node3;
				node4 = node2;
			}
			else if (flag && !flag2)
			{
				node3.left = node.right;
				node.right = node3;
				node2.right = node.left;
				node.left = node2;
				node4 = node;
			}
			else if (!flag && flag2)
			{
				node3.right = node.left;
				node.left = node3;
				node2.left = node.right;
				node.right = node2;
				node4 = node;
			}
			else
			{
				node3.right = node2.left;
				node2.left = node3;
				node4 = node2;
			}
			node3.FixSize();
			node3.IsBlack = false;
			if (node4 != node2)
			{
				node2.FixSize();
			}
			node4.IsBlack = true;
			node_reparent((curpos != 4) ? path[curpos - 6] : null, node3, size, node4);
		}

		private void rebalance_delete__rotate_final(int curpos, List<Node> path)
		{
			Node node = path[curpos - 1];
			Node node2 = path[curpos - 2];
			uint size = node2.Size;
			bool isBlack = node2.IsBlack;
			Node node3;
			if (node2.right == node)
			{
				if (node.right == null || node.right.IsBlack)
				{
					Node left = node.left;
					node2.right = left.left;
					left.left = node2;
					node.left = left.right;
					left.right = node;
					node3 = left;
				}
				else
				{
					node2.right = node.left;
					node.left = node2;
					node.right.IsBlack = true;
					node3 = node;
				}
			}
			else if (node.left == null || node.left.IsBlack)
			{
				Node right = node.right;
				node2.left = right.right;
				right.right = node2;
				node.right = right.left;
				right.left = node;
				node3 = right;
			}
			else
			{
				node2.left = node.right;
				node.right = node2;
				node.left.IsBlack = true;
				node3 = node;
			}
			node2.FixSize();
			node2.IsBlack = true;
			if (node3 != node)
			{
				node.FixSize();
			}
			node3.IsBlack = isBlack;
			node_reparent((curpos != 2) ? path[curpos - 4] : null, node2, size, node3);
		}

		private int ensure_sibling_black(int curpos, List<Node> path)
		{
			Node value = path[curpos];
			Node node = path[curpos - 1];
			Node node2 = path[curpos - 2];
			uint size = node2.Size;
			bool flag;
			if (node2.right == node)
			{
				node2.right = node.left;
				node.left = node2;
				flag = true;
			}
			else
			{
				node2.left = node.right;
				node.right = node2;
				flag = false;
			}
			node2.FixSize();
			node2.IsBlack = false;
			node.IsBlack = true;
			node_reparent((curpos != 2) ? path[curpos - 4] : null, node2, size, node);
			if (curpos + 1 == path.Count)
			{
				path.Add(null);
				path.Add(null);
			}
			path[curpos - 2] = node;
			path[curpos - 1] = ((!flag) ? node.left : node.right);
			path[curpos] = node2;
			path[curpos + 1] = ((!flag) ? node2.left : node2.right);
			path[curpos + 2] = value;
			return curpos + 2;
		}

		private void node_reparent(Node orig_parent, Node orig, uint orig_size, Node updated)
		{
			if (updated != null && updated.FixSize() != orig_size)
			{
				throw new SystemException("Internal error: rotation");
			}
			if (orig == root)
			{
				root = updated;
				return;
			}
			if (orig == orig_parent.left)
			{
				orig_parent.left = updated;
				return;
			}
			if (orig == orig_parent.right)
			{
				orig_parent.right = updated;
				return;
			}
			throw new SystemException("Internal error: path error");
		}

		private static Node right_most(Node current, Node sibling, List<Node> path)
		{
			while (true)
			{
				path.Add(sibling);
				path.Add(current);
				if (current.right == null)
				{
					break;
				}
				sibling = current.left;
				current = current.right;
			}
			return current;
		}
	}
}
