using System.Collections;

namespace System.Diagnostics
{
	public class CorrelationManager
	{
		private Guid activity;

		private Stack op_stack = new Stack();

		public Guid ActivityId
		{
			get
			{
				return activity;
			}
			set
			{
				activity = value;
			}
		}

		public Stack LogicalOperationStack
		{
			get
			{
				return op_stack;
			}
		}

		internal CorrelationManager()
		{
		}

		public void StartLogicalOperation()
		{
			StartLogicalOperation(Guid.NewGuid());
		}

		public void StartLogicalOperation(object operationId)
		{
			op_stack.Push(operationId);
		}

		public void StopLogicalOperation()
		{
			op_stack.Pop();
		}
	}
}
