namespace System.Data
{
	internal class Node
	{
		protected int _iBalance;

		internal Node _nNext;

		protected Node _nLeft;

		protected Node _nRight;

		protected Node _nParent;

		protected DataRow _row;

		internal DataRow Row
		{
			get
			{
				return _row;
			}
		}

		internal Node Left
		{
			get
			{
				if (_iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				return _nLeft;
			}
			set
			{
				if (_iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				_nLeft = value;
			}
		}

		internal Node Right
		{
			get
			{
				if (_iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				return _nRight;
			}
			set
			{
				if (_iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				_nRight = value;
			}
		}

		internal Node Parent
		{
			get
			{
				if (_iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				return _nParent;
			}
			set
			{
				if (_iBalance == -2)
				{
					throw new SystemException("Node is deleted.");
				}
				_nParent = value;
			}
		}

		public Node(DataRow row)
		{
			_row = row;
		}

		internal int GetBalance()
		{
			if (_iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			return _iBalance;
		}

		internal void Delete()
		{
			_iBalance = -2;
			_nLeft = null;
			_nRight = null;
			_nParent = null;
		}

		internal bool IsRoot()
		{
			return _nParent == null;
		}

		internal void SetBalance(int b)
		{
			if (_iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			_iBalance = b;
		}

		internal bool From()
		{
			if (IsRoot())
			{
				return true;
			}
			if (_iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			Node parent = Parent;
			return Equals(parent.Left);
		}

		internal object[] GetData()
		{
			if (_iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			return _row.ItemArray;
		}

		internal bool Equals(Node n)
		{
			if (_iBalance == -2)
			{
				throw new SystemException("Node is deleted.");
			}
			return n == this;
		}
	}
}
