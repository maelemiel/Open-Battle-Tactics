namespace System.Reflection.Emit
{
	internal struct ILExceptionInfo
	{
		private ILExceptionBlock[] handlers;

		internal int start;

		private int len;

		internal Label end;

		internal int NumHandlers()
		{
			return handlers.Length;
		}

		internal void AddCatch(Type extype, int offset)
		{
			End(offset);
			add_block(offset);
			int num = handlers.Length - 1;
			handlers[num].type = 0;
			handlers[num].start = offset;
			handlers[num].extype = extype;
		}

		internal void AddFinally(int offset)
		{
			End(offset);
			add_block(offset);
			int num = handlers.Length - 1;
			handlers[num].type = 2;
			handlers[num].start = offset;
			handlers[num].extype = null;
		}

		internal void AddFault(int offset)
		{
			End(offset);
			add_block(offset);
			int num = handlers.Length - 1;
			handlers[num].type = 4;
			handlers[num].start = offset;
			handlers[num].extype = null;
		}

		internal void AddFilter(int offset)
		{
			End(offset);
			add_block(offset);
			int num = handlers.Length - 1;
			handlers[num].type = -1;
			handlers[num].extype = null;
			handlers[num].filter_offset = offset;
		}

		internal void End(int offset)
		{
			if (handlers != null)
			{
				int num = handlers.Length - 1;
				if (num >= 0)
				{
					handlers[num].len = offset - handlers[num].start;
				}
			}
		}

		internal int LastClauseType()
		{
			if (handlers != null)
			{
				return handlers[handlers.Length - 1].type;
			}
			return 0;
		}

		internal void PatchFilterClause(int start)
		{
			if (handlers != null && handlers.Length > 0)
			{
				handlers[handlers.Length - 1].start = start;
				handlers[handlers.Length - 1].type = 1;
			}
		}

		internal void Debug(int b)
		{
		}

		private void add_block(int offset)
		{
			if (handlers != null)
			{
				int num = handlers.Length;
				ILExceptionBlock[] destinationArray = new ILExceptionBlock[num + 1];
				Array.Copy(handlers, destinationArray, num);
				handlers = destinationArray;
				handlers[num].len = offset - handlers[num].start;
			}
			else
			{
				handlers = new ILExceptionBlock[1];
				len = offset - start;
			}
		}
	}
}
