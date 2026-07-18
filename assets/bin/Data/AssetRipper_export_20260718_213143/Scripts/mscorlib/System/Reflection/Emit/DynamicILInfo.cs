using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	public class DynamicILInfo
	{
		[MonoTODO]
		public DynamicMethod DynamicMethod
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		internal DynamicILInfo()
		{
		}

		[MonoTODO]
		public int GetTokenFor(byte[] signature)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int GetTokenFor(DynamicMethod method)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int GetTokenFor(RuntimeFieldHandle field)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int GetTokenFor(RuntimeMethodHandle method)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int GetTokenFor(RuntimeTypeHandle type)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int GetTokenFor(string literal)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public int GetTokenFor(RuntimeMethodHandle method, RuntimeTypeHandle contextType)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetCode(byte[] code, int maxStackSize)
		{
			throw new NotImplementedException();
		}

		[CLSCompliant(false)]
		[MonoTODO]
		public unsafe void SetCode(byte* code, int codeSize, int maxStackSize)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetExceptions(byte[] exceptions)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[CLSCompliant(false)]
		public unsafe void SetExceptions(byte* exceptions, int exceptionsSize)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public void SetLocalSignature(byte[] localSignature)
		{
			throw new NotImplementedException();
		}

		[CLSCompliant(false)]
		[MonoTODO]
		public unsafe void SetLocalSignature(byte* localSignature, int signatureSize)
		{
			throw new NotImplementedException();
		}
	}
}
