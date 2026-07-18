using System.Collections;

namespace System.Xml.XPath
{
	internal class FunctionArguments
	{
		protected Expression _arg;

		protected FunctionArguments _tail;

		public Expression Arg
		{
			get
			{
				return _arg;
			}
		}

		public FunctionArguments Tail
		{
			get
			{
				return _tail;
			}
		}

		public FunctionArguments(Expression arg, FunctionArguments tail)
		{
			_arg = arg;
			_tail = tail;
		}

		public void ToArrayList(ArrayList a)
		{
			FunctionArguments functionArguments = this;
			do
			{
				a.Add(functionArguments._arg);
				functionArguments = functionArguments._tail;
			}
			while (functionArguments != null);
		}
	}
}
