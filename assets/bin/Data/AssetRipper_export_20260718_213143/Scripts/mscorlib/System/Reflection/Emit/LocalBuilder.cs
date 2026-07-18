using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_LocalBuilder))]
	public sealed class LocalBuilder : LocalVariableInfo, _LocalBuilder
	{
		private string name;

		internal ILGenerator ilgen;

		private int startOffset;

		private int endOffset;

		public override Type LocalType
		{
			get
			{
				return type;
			}
		}

		public override bool IsPinned
		{
			get
			{
				return is_pinned;
			}
		}

		public override int LocalIndex
		{
			get
			{
				return position;
			}
		}

		internal string Name
		{
			get
			{
				return name;
			}
		}

		internal int StartOffset
		{
			get
			{
				return startOffset;
			}
		}

		internal int EndOffset
		{
			get
			{
				return endOffset;
			}
		}

		internal LocalBuilder(Type t, ILGenerator ilgen)
		{
			type = t;
			this.ilgen = ilgen;
		}

		void _LocalBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _LocalBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _LocalBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _LocalBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public void SetLocalSymInfo(string name, int startOffset, int endOffset)
		{
			this.name = name;
			this.startOffset = startOffset;
			this.endOffset = endOffset;
		}

		public void SetLocalSymInfo(string name)
		{
			SetLocalSymInfo(name, 0, 0);
		}

		internal static int Mono_GetLocalIndex(LocalBuilder builder)
		{
			return builder.position;
		}
	}
}
