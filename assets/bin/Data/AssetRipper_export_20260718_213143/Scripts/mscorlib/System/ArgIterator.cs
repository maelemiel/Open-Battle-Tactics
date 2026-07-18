using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System
{
	[StructLayout(LayoutKind.Auto)]
	public struct ArgIterator
	{
		private IntPtr sig;

		private IntPtr args;

		private int next_arg;

		private int num_args;

		public ArgIterator(RuntimeArgumentHandle arglist)
		{
			sig = IntPtr.Zero;
			args = IntPtr.Zero;
			next_arg = (num_args = 0);
			Setup(arglist.args, IntPtr.Zero);
		}

		[CLSCompliant(false)]
		public unsafe ArgIterator(RuntimeArgumentHandle arglist, void* ptr)
		{
			sig = IntPtr.Zero;
			args = IntPtr.Zero;
			next_arg = (num_args = 0);
			Setup(arglist.args, (IntPtr)ptr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Setup(IntPtr argsp, IntPtr start);

		public void End()
		{
			next_arg = num_args;
		}

		public override bool Equals(object o)
		{
			throw new NotSupportedException(Locale.GetText("ArgIterator does not support Equals."));
		}

		public override int GetHashCode()
		{
			return sig.GetHashCode();
		}

		[CLSCompliant(false)]
		public TypedReference GetNextArg()
		{
			if (num_args == next_arg)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid iterator position."));
			}
			return IntGetNextArg();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TypedReference IntGetNextArg();

		[CLSCompliant(false)]
		public TypedReference GetNextArg(RuntimeTypeHandle rth)
		{
			if (num_args == next_arg)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid iterator position."));
			}
			return IntGetNextArg(rth.Value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern TypedReference IntGetNextArg(IntPtr rth);

		public RuntimeTypeHandle GetNextArgType()
		{
			if (num_args == next_arg)
			{
				throw new InvalidOperationException(Locale.GetText("Invalid iterator position."));
			}
			return new RuntimeTypeHandle(IntGetNextArgType());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr IntGetNextArgType();

		public int GetRemainingCount()
		{
			return num_args - next_arg;
		}
	}
}
