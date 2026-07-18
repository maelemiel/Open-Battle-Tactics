using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	public struct OpCode
	{
		internal byte op1;

		internal byte op2;

		private byte push;

		private byte pop;

		private byte size;

		private byte type;

		private byte args;

		private byte flow;

		public string Name
		{
			get
			{
				if (op1 == byte.MaxValue)
				{
					return OpCodeNames.names[op2];
				}
				return OpCodeNames.names[256 + op2];
			}
		}

		public int Size
		{
			get
			{
				return size;
			}
		}

		public OpCodeType OpCodeType
		{
			get
			{
				return (OpCodeType)type;
			}
		}

		public OperandType OperandType
		{
			get
			{
				return (OperandType)args;
			}
		}

		public FlowControl FlowControl
		{
			get
			{
				return (FlowControl)flow;
			}
		}

		public StackBehaviour StackBehaviourPop
		{
			get
			{
				return (StackBehaviour)pop;
			}
		}

		public StackBehaviour StackBehaviourPush
		{
			get
			{
				return (StackBehaviour)push;
			}
		}

		public short Value
		{
			get
			{
				if (size == 1)
				{
					return op2;
				}
				return (short)((op1 << 8) | op2);
			}
		}

		internal OpCode(int p, int q)
		{
			op1 = (byte)(p & 0xFF);
			op2 = (byte)((p >> 8) & 0xFF);
			push = (byte)((p >> 16) & 0xFF);
			pop = (byte)((p >> 24) & 0xFF);
			size = (byte)(q & 0xFF);
			type = (byte)((q >> 8) & 0xFF);
			args = (byte)((q >> 16) & 0xFF);
			flow = (byte)((q >> 24) & 0xFF);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is OpCode))
			{
				return false;
			}
			OpCode opCode = (OpCode)obj;
			return opCode.op1 == op1 && opCode.op2 == op2;
		}

		public bool Equals(OpCode obj)
		{
			return obj.op1 == op1 && obj.op2 == op2;
		}

		public override string ToString()
		{
			return Name;
		}

		public static bool operator ==(OpCode a, OpCode b)
		{
			return a.op1 == b.op1 && a.op2 == b.op2;
		}

		public static bool operator !=(OpCode a, OpCode b)
		{
			return a.op1 != b.op1 || a.op2 != b.op2;
		}
	}
}
