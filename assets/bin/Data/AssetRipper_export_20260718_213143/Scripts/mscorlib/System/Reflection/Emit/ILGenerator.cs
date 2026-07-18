using System.Collections;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ILGenerator))]
	public class ILGenerator : _ILGenerator
	{
		private struct LabelFixup
		{
			public int offset;

			public int pos;

			public int label_idx;
		}

		private struct LabelData
		{
			public int addr;

			public int maxStack;

			public LabelData(int addr, int maxStack)
			{
				this.addr = addr;
				this.maxStack = maxStack;
			}
		}

		private const int defaultFixupSize = 4;

		private const int defaultLabelsSize = 4;

		private const int defaultExceptionStackSize = 2;

		private static readonly Type void_type = typeof(void);

		private byte[] code;

		private int code_len;

		private int max_stack;

		private int cur_stack;

		private LocalBuilder[] locals;

		private ILExceptionInfo[] ex_handlers;

		private int num_token_fixups;

		private ILTokenInfo[] token_fixups;

		private LabelData[] labels;

		private int num_labels;

		private LabelFixup[] fixups;

		private int num_fixups;

		internal Module module;

		private int cur_block;

		private Stack open_blocks;

		private TokenGenerator token_gen;

		private ArrayList sequencePointLists;

		private SequencePointList currentSequence;

		internal bool HasDebugInfo
		{
			get
			{
				return sequencePointLists != null;
			}
		}

		internal ILGenerator(Module m, TokenGenerator token_gen, int size)
		{
			if (size < 0)
			{
				size = 128;
			}
			code = new byte[size];
			token_fixups = new ILTokenInfo[8];
			module = m;
			this.token_gen = token_gen;
		}

		void _ILGenerator.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ILGenerator.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ILGenerator.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ILGenerator.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private void add_token_fixup(MemberInfo mi)
		{
			if (num_token_fixups == token_fixups.Length)
			{
				ILTokenInfo[] array = new ILTokenInfo[num_token_fixups * 2];
				token_fixups.CopyTo(array, 0);
				token_fixups = array;
			}
			token_fixups[num_token_fixups].member = mi;
			token_fixups[num_token_fixups++].code_pos = code_len;
		}

		private void make_room(int nbytes)
		{
			if (code_len + nbytes >= code.Length)
			{
				byte[] destinationArray = new byte[(code_len + nbytes) * 2 + 128];
				Array.Copy(code, 0, destinationArray, 0, code.Length);
				code = destinationArray;
			}
		}

		private void emit_int(int val)
		{
			code[code_len++] = (byte)(val & 0xFF);
			code[code_len++] = (byte)((val >> 8) & 0xFF);
			code[code_len++] = (byte)((val >> 16) & 0xFF);
			code[code_len++] = (byte)((val >> 24) & 0xFF);
		}

		private void ll_emit(OpCode opcode)
		{
			if (opcode.Size == 2)
			{
				code[code_len++] = opcode.op1;
			}
			code[code_len++] = opcode.op2;
			switch (opcode.StackBehaviourPush)
			{
			case StackBehaviour.Push1:
			case StackBehaviour.Pushi:
			case StackBehaviour.Pushi8:
			case StackBehaviour.Pushr4:
			case StackBehaviour.Pushr8:
			case StackBehaviour.Pushref:
			case StackBehaviour.Varpush:
				cur_stack++;
				break;
			case StackBehaviour.Push1_push1:
				cur_stack += 2;
				break;
			}
			if (max_stack < cur_stack)
			{
				max_stack = cur_stack;
			}
			switch (opcode.StackBehaviourPop)
			{
			case StackBehaviour.Varpop:
				break;
			case StackBehaviour.Pop1:
			case StackBehaviour.Popi:
			case StackBehaviour.Popref:
				cur_stack--;
				break;
			case StackBehaviour.Pop1_pop1:
			case StackBehaviour.Popi_pop1:
			case StackBehaviour.Popi_popi:
			case StackBehaviour.Popi_popi8:
			case StackBehaviour.Popi_popr4:
			case StackBehaviour.Popi_popr8:
			case StackBehaviour.Popref_pop1:
			case StackBehaviour.Popref_popi:
				cur_stack -= 2;
				break;
			case StackBehaviour.Popi_popi_popi:
			case StackBehaviour.Popref_popi_popi:
			case StackBehaviour.Popref_popi_popi8:
			case StackBehaviour.Popref_popi_popr4:
			case StackBehaviour.Popref_popi_popr8:
			case StackBehaviour.Popref_popi_popref:
				cur_stack -= 3;
				break;
			case StackBehaviour.Push0:
			case StackBehaviour.Push1:
			case StackBehaviour.Push1_push1:
			case StackBehaviour.Pushi:
			case StackBehaviour.Pushi8:
			case StackBehaviour.Pushr4:
			case StackBehaviour.Pushr8:
			case StackBehaviour.Pushref:
				break;
			}
		}

		private static int target_len(OpCode opcode)
		{
			if (opcode.OperandType == OperandType.InlineBrTarget)
			{
				return 4;
			}
			return 1;
		}

		private void InternalEndClause()
		{
			switch (ex_handlers[cur_block].LastClauseType())
			{
			case -1:
			case 0:
			case 1:
				Emit(OpCodes.Leave, ex_handlers[cur_block].end);
				break;
			case 2:
			case 4:
				Emit(OpCodes.Endfinally);
				break;
			case 3:
				break;
			}
		}

		public virtual void BeginCatchBlock(Type exceptionType)
		{
			if (open_blocks == null)
			{
				open_blocks = new Stack(2);
			}
			if (open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			if (exceptionType != null && exceptionType.IsUserType)
			{
				throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
			}
			if (ex_handlers[cur_block].LastClauseType() == -1)
			{
				if (exceptionType != null)
				{
					throw new ArgumentException("Do not supply an exception type for filter clause");
				}
				Emit(OpCodes.Endfilter);
				ex_handlers[cur_block].PatchFilterClause(code_len);
			}
			else
			{
				InternalEndClause();
				ex_handlers[cur_block].AddCatch(exceptionType, code_len);
			}
			cur_stack = 1;
			if (max_stack < cur_stack)
			{
				max_stack = cur_stack;
			}
		}

		public virtual void BeginExceptFilterBlock()
		{
			if (open_blocks == null)
			{
				open_blocks = new Stack(2);
			}
			if (open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			InternalEndClause();
			ex_handlers[cur_block].AddFilter(code_len);
		}

		public virtual Label BeginExceptionBlock()
		{
			if (open_blocks == null)
			{
				open_blocks = new Stack(2);
			}
			if (ex_handlers != null)
			{
				cur_block = ex_handlers.Length;
				ILExceptionInfo[] destinationArray = new ILExceptionInfo[cur_block + 1];
				Array.Copy(ex_handlers, destinationArray, cur_block);
				ex_handlers = destinationArray;
			}
			else
			{
				ex_handlers = new ILExceptionInfo[1];
				cur_block = 0;
			}
			open_blocks.Push(cur_block);
			ex_handlers[cur_block].start = code_len;
			return ex_handlers[cur_block].end = DefineLabel();
		}

		public virtual void BeginFaultBlock()
		{
			if (open_blocks == null)
			{
				open_blocks = new Stack(2);
			}
			if (open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			if (ex_handlers[cur_block].LastClauseType() == -1)
			{
				Emit(OpCodes.Leave, ex_handlers[cur_block].end);
				ex_handlers[cur_block].PatchFilterClause(code_len);
			}
			InternalEndClause();
			ex_handlers[cur_block].AddFault(code_len);
		}

		public virtual void BeginFinallyBlock()
		{
			if (open_blocks == null)
			{
				open_blocks = new Stack(2);
			}
			if (open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			InternalEndClause();
			if (ex_handlers[cur_block].LastClauseType() == -1)
			{
				Emit(OpCodes.Leave, ex_handlers[cur_block].end);
				ex_handlers[cur_block].PatchFilterClause(code_len);
			}
			ex_handlers[cur_block].AddFinally(code_len);
		}

		public virtual void BeginScope()
		{
		}

		public virtual LocalBuilder DeclareLocal(Type localType)
		{
			return DeclareLocal(localType, false);
		}

		public virtual LocalBuilder DeclareLocal(Type localType, bool pinned)
		{
			if (localType == null)
			{
				throw new ArgumentNullException("localType");
			}
			if (localType.IsUserType)
			{
				throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
			}
			LocalBuilder localBuilder = new LocalBuilder(localType, this);
			localBuilder.is_pinned = pinned;
			if (locals != null)
			{
				LocalBuilder[] array = new LocalBuilder[locals.Length + 1];
				Array.Copy(locals, array, locals.Length);
				array[locals.Length] = localBuilder;
				locals = array;
			}
			else
			{
				locals = new LocalBuilder[1];
				locals[0] = localBuilder;
			}
			localBuilder.position = (ushort)(locals.Length - 1);
			return localBuilder;
		}

		public virtual Label DefineLabel()
		{
			if (labels == null)
			{
				labels = new LabelData[4];
			}
			else if (num_labels >= labels.Length)
			{
				LabelData[] destinationArray = new LabelData[labels.Length * 2];
				Array.Copy(labels, destinationArray, labels.Length);
				labels = destinationArray;
			}
			labels[num_labels] = new LabelData(-1, 0);
			return new Label(num_labels++);
		}

		public virtual void Emit(OpCode opcode)
		{
			make_room(2);
			ll_emit(opcode);
		}

		public virtual void Emit(OpCode opcode, byte arg)
		{
			make_room(3);
			ll_emit(opcode);
			code[code_len++] = arg;
		}

		[ComVisible(true)]
		public virtual void Emit(OpCode opcode, ConstructorInfo con)
		{
			int token = token_gen.GetToken(con);
			make_room(6);
			ll_emit(opcode);
			if (con.DeclaringType.Module == module)
			{
				add_token_fixup(con);
			}
			emit_int(token);
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				cur_stack -= con.GetParameterCount();
			}
		}

		public virtual void Emit(OpCode opcode, double arg)
		{
			byte[] bytes = BitConverter.GetBytes(arg);
			make_room(10);
			ll_emit(opcode);
			if (BitConverter.IsLittleEndian)
			{
				Array.Copy(bytes, 0, code, code_len, 8);
				code_len += 8;
				return;
			}
			code[code_len++] = bytes[7];
			code[code_len++] = bytes[6];
			code[code_len++] = bytes[5];
			code[code_len++] = bytes[4];
			code[code_len++] = bytes[3];
			code[code_len++] = bytes[2];
			code[code_len++] = bytes[1];
			code[code_len++] = bytes[0];
		}

		public virtual void Emit(OpCode opcode, FieldInfo field)
		{
			int token = token_gen.GetToken(field);
			make_room(6);
			ll_emit(opcode);
			if (field.DeclaringType.Module == module)
			{
				add_token_fixup(field);
			}
			emit_int(token);
		}

		public virtual void Emit(OpCode opcode, short arg)
		{
			make_room(4);
			ll_emit(opcode);
			code[code_len++] = (byte)(arg & 0xFF);
			code[code_len++] = (byte)((arg >> 8) & 0xFF);
		}

		public virtual void Emit(OpCode opcode, int arg)
		{
			make_room(6);
			ll_emit(opcode);
			emit_int(arg);
		}

		public virtual void Emit(OpCode opcode, long arg)
		{
			make_room(10);
			ll_emit(opcode);
			code[code_len++] = (byte)(arg & 0xFF);
			code[code_len++] = (byte)((arg >> 8) & 0xFF);
			code[code_len++] = (byte)((arg >> 16) & 0xFF);
			code[code_len++] = (byte)((arg >> 24) & 0xFF);
			code[code_len++] = (byte)((arg >> 32) & 0xFF);
			code[code_len++] = (byte)((arg >> 40) & 0xFF);
			code[code_len++] = (byte)((arg >> 48) & 0xFF);
			code[code_len++] = (byte)((arg >> 56) & 0xFF);
		}

		public virtual void Emit(OpCode opcode, Label label)
		{
			int num = target_len(opcode);
			make_room(6);
			ll_emit(opcode);
			if (cur_stack > labels[label.label].maxStack)
			{
				labels[label.label].maxStack = cur_stack;
			}
			if (fixups == null)
			{
				fixups = new LabelFixup[4];
			}
			else if (num_fixups >= fixups.Length)
			{
				LabelFixup[] destinationArray = new LabelFixup[fixups.Length * 2];
				Array.Copy(fixups, destinationArray, fixups.Length);
				fixups = destinationArray;
			}
			fixups[num_fixups].offset = num;
			fixups[num_fixups].pos = code_len;
			fixups[num_fixups].label_idx = label.label;
			num_fixups++;
			code_len += num;
		}

		public virtual void Emit(OpCode opcode, Label[] labels)
		{
			if (labels == null)
			{
				throw new ArgumentNullException("labels");
			}
			int num = labels.Length;
			make_room(6 + num * 4);
			ll_emit(opcode);
			for (int i = 0; i < num; i++)
			{
				if (cur_stack > this.labels[labels[i].label].maxStack)
				{
					this.labels[labels[i].label].maxStack = cur_stack;
				}
			}
			emit_int(num);
			if (fixups == null)
			{
				fixups = new LabelFixup[4 + num];
			}
			else if (num_fixups + num >= fixups.Length)
			{
				LabelFixup[] destinationArray = new LabelFixup[num + fixups.Length * 2];
				Array.Copy(fixups, destinationArray, fixups.Length);
				fixups = destinationArray;
			}
			int num2 = 0;
			int num3 = num * 4;
			while (num2 < num)
			{
				fixups[num_fixups].offset = num3;
				fixups[num_fixups].pos = code_len;
				fixups[num_fixups].label_idx = labels[num2].label;
				num_fixups++;
				code_len += 4;
				num2++;
				num3 -= 4;
			}
		}

		public virtual void Emit(OpCode opcode, LocalBuilder local)
		{
			if (local == null)
			{
				throw new ArgumentNullException("local");
			}
			uint position = local.position;
			bool flag = false;
			bool flag2 = false;
			make_room(6);
			if (local.ilgen != this)
			{
				throw new ArgumentException("Trying to emit a local from a different ILGenerator.");
			}
			if (opcode.StackBehaviourPop == StackBehaviour.Pop1)
			{
				cur_stack--;
				flag2 = true;
			}
			else
			{
				cur_stack++;
				if (cur_stack > max_stack)
				{
					max_stack = cur_stack;
				}
				flag = opcode.StackBehaviourPush == StackBehaviour.Pushi;
			}
			if (flag)
			{
				if (position < 256)
				{
					code[code_len++] = 18;
					code[code_len++] = (byte)position;
					return;
				}
				code[code_len++] = 254;
				code[code_len++] = 13;
				code[code_len++] = (byte)(position & 0xFF);
				code[code_len++] = (byte)((position >> 8) & 0xFF);
			}
			else if (flag2)
			{
				if (position < 4)
				{
					code[code_len++] = (byte)(10 + position);
					return;
				}
				if (position < 256)
				{
					code[code_len++] = 19;
					code[code_len++] = (byte)position;
					return;
				}
				code[code_len++] = 254;
				code[code_len++] = 14;
				code[code_len++] = (byte)(position & 0xFF);
				code[code_len++] = (byte)((position >> 8) & 0xFF);
			}
			else if (position < 4)
			{
				code[code_len++] = (byte)(6 + position);
			}
			else if (position < 256)
			{
				code[code_len++] = 17;
				code[code_len++] = (byte)position;
			}
			else
			{
				code[code_len++] = 254;
				code[code_len++] = 12;
				code[code_len++] = (byte)(position & 0xFF);
				code[code_len++] = (byte)((position >> 8) & 0xFF);
			}
		}

		public virtual void Emit(OpCode opcode, MethodInfo meth)
		{
			if (meth == null)
			{
				throw new ArgumentNullException("meth");
			}
			if (meth is DynamicMethod && (opcode == OpCodes.Ldftn || opcode == OpCodes.Ldvirtftn || opcode == OpCodes.Ldtoken))
			{
				throw new ArgumentException("Ldtoken, Ldftn and Ldvirtftn OpCodes cannot target DynamicMethods.");
			}
			int token = token_gen.GetToken(meth);
			make_room(6);
			ll_emit(opcode);
			Type declaringType = meth.DeclaringType;
			if (declaringType != null && declaringType.Module == module)
			{
				add_token_fixup(meth);
			}
			emit_int(token);
			if (meth.ReturnType != void_type)
			{
				cur_stack++;
			}
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				cur_stack -= meth.GetParameterCount();
			}
		}

		private void Emit(OpCode opcode, MethodInfo method, int token)
		{
			make_room(6);
			ll_emit(opcode);
			Type declaringType = method.DeclaringType;
			if (declaringType != null && declaringType.Module == module)
			{
				add_token_fixup(method);
			}
			emit_int(token);
			if (method.ReturnType != void_type)
			{
				cur_stack++;
			}
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				cur_stack -= method.GetParameterCount();
			}
		}

		[CLSCompliant(false)]
		public void Emit(OpCode opcode, sbyte arg)
		{
			make_room(3);
			ll_emit(opcode);
			code[code_len++] = (byte)arg;
		}

		public virtual void Emit(OpCode opcode, SignatureHelper signature)
		{
			int token = token_gen.GetToken(signature);
			make_room(6);
			ll_emit(opcode);
			emit_int(token);
		}

		public virtual void Emit(OpCode opcode, float arg)
		{
			byte[] bytes = BitConverter.GetBytes(arg);
			make_room(6);
			ll_emit(opcode);
			if (BitConverter.IsLittleEndian)
			{
				Array.Copy(bytes, 0, code, code_len, 4);
				code_len += 4;
				return;
			}
			code[code_len++] = bytes[3];
			code[code_len++] = bytes[2];
			code[code_len++] = bytes[1];
			code[code_len++] = bytes[0];
		}

		public virtual void Emit(OpCode opcode, string str)
		{
			int token = token_gen.GetToken(str);
			make_room(6);
			ll_emit(opcode);
			emit_int(token);
		}

		public virtual void Emit(OpCode opcode, Type cls)
		{
			make_room(6);
			ll_emit(opcode);
			emit_int(token_gen.GetToken(cls));
		}

		[MonoLimitation("vararg methods are not supported")]
		public virtual void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			short value = opcode.Value;
			if (value != OpCodes.Call.Value && value != OpCodes.Callvirt.Value)
			{
				throw new NotSupportedException("Only Call and CallVirt are allowed");
			}
			if ((methodInfo.CallingConvention & CallingConventions.VarArgs) == 0)
			{
				optionalParameterTypes = null;
			}
			if (optionalParameterTypes != null)
			{
				if ((methodInfo.CallingConvention & CallingConventions.VarArgs) == 0)
				{
					throw new InvalidOperationException("Method is not VarArgs method and optional types were passed");
				}
				int token = token_gen.GetToken(methodInfo, optionalParameterTypes);
				Emit(opcode, methodInfo, token);
			}
			else
			{
				Emit(opcode, methodInfo);
			}
		}

		public virtual void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(module, (CallingConventions)0, unmanagedCallConv, returnType, parameterTypes);
			Emit(opcode, methodSigHelper);
		}

		public virtual void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			if (optionalParameterTypes != null)
			{
				throw new NotImplementedException();
			}
			SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(module, callingConvention, (CallingConvention)0, returnType, parameterTypes);
			Emit(opcode, methodSigHelper);
		}

		public virtual void EmitWriteLine(FieldInfo fld)
		{
			if (fld == null)
			{
				throw new ArgumentNullException("fld");
			}
			if (fld.IsStatic)
			{
				Emit(OpCodes.Ldsfld, fld);
			}
			else
			{
				Emit(OpCodes.Ldarg_0);
				Emit(OpCodes.Ldfld, fld);
			}
			Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[1] { fld.FieldType }));
		}

		public virtual void EmitWriteLine(LocalBuilder localBuilder)
		{
			if (localBuilder == null)
			{
				throw new ArgumentNullException("localBuilder");
			}
			if (localBuilder.LocalType is TypeBuilder)
			{
				throw new ArgumentException("Output streams do not support TypeBuilders.");
			}
			Emit(OpCodes.Ldloc, localBuilder);
			Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[1] { localBuilder.LocalType }));
		}

		public virtual void EmitWriteLine(string value)
		{
			Emit(OpCodes.Ldstr, value);
			Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[1] { typeof(string) }));
		}

		public virtual void EndExceptionBlock()
		{
			if (open_blocks == null)
			{
				open_blocks = new Stack(2);
			}
			if (open_blocks.Count <= 0)
			{
				throw new NotSupportedException("Not in an exception block");
			}
			if (ex_handlers[cur_block].LastClauseType() == -1)
			{
				throw new InvalidOperationException("Incorrect code generation for exception block.");
			}
			InternalEndClause();
			MarkLabel(ex_handlers[cur_block].end);
			ex_handlers[cur_block].End(code_len);
			ex_handlers[cur_block].Debug(cur_block);
			open_blocks.Pop();
			if (open_blocks.Count > 0)
			{
				cur_block = (int)open_blocks.Peek();
			}
		}

		public virtual void EndScope()
		{
		}

		public virtual void MarkLabel(Label loc)
		{
			if (loc.label < 0 || loc.label >= num_labels)
			{
				throw new ArgumentException("The label is not valid");
			}
			if (labels[loc.label].addr >= 0)
			{
				throw new ArgumentException("The label was already defined");
			}
			labels[loc.label].addr = code_len;
			if (labels[loc.label].maxStack > cur_stack)
			{
				cur_stack = labels[loc.label].maxStack;
			}
		}

		public virtual void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
		{
			if (currentSequence == null || currentSequence.Document != document)
			{
				if (sequencePointLists == null)
				{
					sequencePointLists = new ArrayList();
				}
				currentSequence = new SequencePointList(document);
				sequencePointLists.Add(currentSequence);
			}
			currentSequence.AddSequencePoint(code_len, startLine, startColumn, endLine, endColumn);
		}

		internal void GenerateDebugInfo(ISymbolWriter symbolWriter)
		{
			if (sequencePointLists == null)
			{
				return;
			}
			SequencePointList sequencePointList = (SequencePointList)sequencePointLists[0];
			SequencePointList sequencePointList2 = (SequencePointList)sequencePointLists[sequencePointLists.Count - 1];
			symbolWriter.SetMethodSourceRange(sequencePointList.Document, sequencePointList.StartLine, sequencePointList.StartColumn, sequencePointList2.Document, sequencePointList2.EndLine, sequencePointList2.EndColumn);
			foreach (SequencePointList sequencePointList3 in sequencePointLists)
			{
				symbolWriter.DefineSequencePoints(sequencePointList3.Document, sequencePointList3.GetOffsets(), sequencePointList3.GetLines(), sequencePointList3.GetColumns(), sequencePointList3.GetEndLines(), sequencePointList3.GetEndColumns());
			}
			if (locals != null)
			{
				LocalBuilder[] array = locals;
				foreach (LocalBuilder localBuilder in array)
				{
					if (localBuilder.Name != null && localBuilder.Name.Length > 0)
					{
						SignatureHelper localVarSigHelper = SignatureHelper.GetLocalVarSigHelper(module);
						localVarSigHelper.AddArgument(localBuilder.LocalType);
						byte[] signature = localVarSigHelper.GetSignature();
						symbolWriter.DefineLocalVariable(localBuilder.Name, FieldAttributes.Public, signature, SymAddressKind.ILOffset, localBuilder.position, 0, 0, localBuilder.StartOffset, localBuilder.EndOffset);
					}
				}
			}
			sequencePointLists = null;
		}

		public virtual void ThrowException(Type excType)
		{
			if (excType == null)
			{
				throw new ArgumentNullException("excType");
			}
			if (excType != typeof(Exception) && !excType.IsSubclassOf(typeof(Exception)))
			{
				throw new ArgumentException("Type should be an exception type", "excType");
			}
			ConstructorInfo constructor = excType.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
			{
				throw new ArgumentException("Type should have a default constructor", "excType");
			}
			Emit(OpCodes.Newobj, constructor);
			Emit(OpCodes.Throw);
		}

		[MonoTODO("Not implemented")]
		public virtual void UsingNamespace(string usingNamespace)
		{
			throw new NotImplementedException();
		}

		internal void label_fixup()
		{
			for (int i = 0; i < num_fixups; i++)
			{
				if (labels[fixups[i].label_idx].addr < 0)
				{
					throw new ArgumentException("Label not marked");
				}
				int num = labels[fixups[i].label_idx].addr - (fixups[i].pos + fixups[i].offset);
				if (fixups[i].offset == 1)
				{
					code[fixups[i].pos] = (byte)(sbyte)num;
					continue;
				}
				int num2 = code_len;
				code_len = fixups[i].pos;
				emit_int(num);
				code_len = num2;
			}
		}

		[Obsolete("Use ILOffset")]
		internal static int Mono_GetCurrentOffset(ILGenerator ig)
		{
			return ig.code_len;
		}
	}
}
