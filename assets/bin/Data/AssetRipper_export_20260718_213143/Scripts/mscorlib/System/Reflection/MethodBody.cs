using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public sealed class MethodBody
	{
		private ExceptionHandlingClause[] clauses;

		private LocalVariableInfo[] locals;

		private byte[] il;

		private bool init_locals;

		private int sig_token;

		private int max_stack;

		public IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				return Array.AsReadOnly(clauses);
			}
		}

		public IList<LocalVariableInfo> LocalVariables
		{
			get
			{
				return Array.AsReadOnly(locals);
			}
		}

		public bool InitLocals
		{
			get
			{
				return init_locals;
			}
		}

		public int LocalSignatureMetadataToken
		{
			get
			{
				return sig_token;
			}
		}

		public int MaxStackSize
		{
			get
			{
				return max_stack;
			}
		}

		internal MethodBody()
		{
		}

		public byte[] GetILAsByteArray()
		{
			return il;
		}
	}
}
