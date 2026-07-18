using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions
{
	internal class EmitContext
	{
		private CompilationContext context;

		private EmitContext parent;

		private LambdaExpression lambda;

		private DynamicMethod method;

		private LocalBuilder hoisted_store;

		private List<ParameterExpression> hoisted;

		public readonly ILGenerator ig;

		public bool HasHoistedLocals
		{
			get
			{
				return hoisted != null && hoisted.Count > 0;
			}
		}

		public LambdaExpression Lambda
		{
			get
			{
				return lambda;
			}
		}

		public EmitContext(CompilationContext context, EmitContext parent, LambdaExpression lambda)
		{
			this.context = context;
			this.parent = parent;
			this.lambda = lambda;
			hoisted = context.GetHoistedLocals(lambda);
			method = new DynamicMethod("lambda_method", lambda.GetReturnType(), CreateParameterTypes(lambda.Parameters), typeof(ExecutionScope), true);
			ig = method.GetILGenerator();
		}

		public void Emit()
		{
			if (HasHoistedLocals)
			{
				EmitStoreHoistedLocals();
			}
			lambda.EmitBody(this);
		}

		private static Type[] CreateParameterTypes(IList<ParameterExpression> parameters)
		{
			Type[] array = new Type[parameters.Count + 1];
			array[0] = typeof(ExecutionScope);
			for (int i = 0; i < parameters.Count; i++)
			{
				array[i + 1] = parameters[i].Type;
			}
			return array;
		}

		public bool IsLocalParameter(ParameterExpression parameter, ref int position)
		{
			position = lambda.Parameters.IndexOf(parameter);
			if (position > -1)
			{
				position++;
				return true;
			}
			return false;
		}

		public Delegate CreateDelegate(ExecutionScope scope)
		{
			return method.CreateDelegate(lambda.Type, scope);
		}

		public void Emit(Expression expression)
		{
			expression.Emit(this);
		}

		public LocalBuilder EmitStored(Expression expression)
		{
			LocalBuilder localBuilder = ig.DeclareLocal(expression.Type);
			expression.Emit(this);
			ig.Emit(OpCodes.Stloc, localBuilder);
			return localBuilder;
		}

		public void EmitLoadAddress(Expression expression)
		{
			ig.Emit(OpCodes.Ldloca, EmitStored(expression));
		}

		public void EmitLoadSubject(Expression expression)
		{
			if (expression.Type.IsValueType)
			{
				EmitLoadAddress(expression);
			}
			else
			{
				Emit(expression);
			}
		}

		public void EmitLoadSubject(LocalBuilder local)
		{
			if (local.LocalType.IsValueType)
			{
				EmitLoadAddress(local);
			}
			else
			{
				EmitLoad(local);
			}
		}

		public void EmitLoadAddress(LocalBuilder local)
		{
			ig.Emit(OpCodes.Ldloca, local);
		}

		public void EmitLoad(LocalBuilder local)
		{
			ig.Emit(OpCodes.Ldloc, local);
		}

		public void EmitCall(LocalBuilder local, IList<Expression> arguments, MethodInfo method)
		{
			EmitLoadSubject(local);
			EmitArguments(method, arguments);
			EmitCall(method);
		}

		public void EmitCall(LocalBuilder local, MethodInfo method)
		{
			EmitLoadSubject(local);
			EmitCall(method);
		}

		public void EmitCall(Expression expression, MethodInfo method)
		{
			if (!method.IsStatic)
			{
				EmitLoadSubject(expression);
			}
			EmitCall(method);
		}

		public void EmitCall(Expression expression, IList<Expression> arguments, MethodInfo method)
		{
			if (!method.IsStatic)
			{
				EmitLoadSubject(expression);
			}
			EmitArguments(method, arguments);
			EmitCall(method);
		}

		private void EmitArguments(MethodInfo method, IList<Expression> arguments)
		{
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				Expression expression = arguments[i];
				if (parameterInfo.ParameterType.IsByRef)
				{
					ig.Emit(OpCodes.Ldloca, EmitStored(expression));
				}
				else
				{
					Emit(arguments[i]);
				}
			}
		}

		public void EmitCall(MethodInfo method)
		{
			ig.Emit((!method.IsVirtual) ? OpCodes.Call : OpCodes.Callvirt, method);
		}

		public void EmitNullableHasValue(LocalBuilder local)
		{
			EmitCall(local, "get_HasValue");
		}

		public void EmitNullableInitialize(LocalBuilder local)
		{
			ig.Emit(OpCodes.Ldloca, local);
			ig.Emit(OpCodes.Initobj, local.LocalType);
			ig.Emit(OpCodes.Ldloc, local);
		}

		public void EmitNullableGetValue(LocalBuilder local)
		{
			EmitCall(local, "get_Value");
		}

		public void EmitNullableGetValueOrDefault(LocalBuilder local)
		{
			EmitCall(local, "GetValueOrDefault");
		}

		private void EmitCall(LocalBuilder local, string method_name)
		{
			EmitCall(local, local.LocalType.GetMethod(method_name, Type.EmptyTypes));
		}

		public void EmitNullableNew(Type of)
		{
			ig.Emit(OpCodes.Newobj, of.GetConstructor(new Type[1] { of.GetFirstGenericArgument() }));
		}

		public void EmitCollection<T>(IEnumerable<T> collection) where T : Expression
		{
			foreach (T item in collection)
			{
				T current = item;
				current.Emit(this);
			}
		}

		public void EmitCollection(IEnumerable<ElementInit> initializers, LocalBuilder local)
		{
			foreach (ElementInit initializer in initializers)
			{
				initializer.Emit(this, local);
			}
		}

		public void EmitCollection(IEnumerable<MemberBinding> bindings, LocalBuilder local)
		{
			foreach (MemberBinding binding in bindings)
			{
				binding.Emit(this, local);
			}
		}

		public void EmitIsInst(Expression expression, Type candidate)
		{
			expression.Emit(this);
			Type type = expression.Type;
			if (type.IsValueType)
			{
				ig.Emit(OpCodes.Box, type);
			}
			ig.Emit(OpCodes.Isinst, candidate);
		}

		public void EmitScope()
		{
			ig.Emit(OpCodes.Ldarg_0);
		}

		public void EmitReadGlobal(object global)
		{
			EmitReadGlobal(global, global.GetType());
		}

		public void EmitLoadGlobals()
		{
			EmitScope();
			ig.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Globals"));
		}

		public void EmitReadGlobal(object global, Type type)
		{
			EmitLoadGlobals();
			ig.Emit(OpCodes.Ldc_I4, AddGlobal(global, type));
			ig.Emit(OpCodes.Ldelem, typeof(object));
			EmitLoadStrongBoxValue(type);
		}

		public void EmitLoadStrongBoxValue(Type type)
		{
			Type type2 = type.MakeStrongBoxType();
			ig.Emit(OpCodes.Isinst, type2);
			ig.Emit(OpCodes.Ldfld, type2.GetField("Value"));
		}

		private int AddGlobal(object value, Type type)
		{
			return context.AddGlobal(CreateStrongBox(value, type));
		}

		public void EmitCreateDelegate(LambdaExpression lambda)
		{
			EmitScope();
			ig.Emit(OpCodes.Ldc_I4, AddChildContext(lambda));
			if (hoisted_store != null)
			{
				ig.Emit(OpCodes.Ldloc, hoisted_store);
			}
			else
			{
				ig.Emit(OpCodes.Ldnull);
			}
			ig.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateDelegate"));
			ig.Emit(OpCodes.Castclass, lambda.Type);
		}

		private void EmitStoreHoistedLocals()
		{
			EmitHoistedLocalsStore();
			for (int i = 0; i < hoisted.Count; i++)
			{
				EmitStoreHoistedLocal(i, hoisted[i]);
			}
		}

		private void EmitStoreHoistedLocal(int position, ParameterExpression parameter)
		{
			ig.Emit(OpCodes.Ldloc, hoisted_store);
			ig.Emit(OpCodes.Ldc_I4, position);
			parameter.Emit(this);
			EmitCreateStrongBox(parameter.Type);
			ig.Emit(OpCodes.Stelem, typeof(object));
		}

		public void EmitLoadHoistedLocalsStore()
		{
			ig.Emit(OpCodes.Ldloc, hoisted_store);
		}

		private void EmitCreateStrongBox(Type type)
		{
			ig.Emit(OpCodes.Newobj, type.MakeStrongBoxType().GetConstructor(new Type[1] { type }));
		}

		private void EmitHoistedLocalsStore()
		{
			EmitScope();
			hoisted_store = ig.DeclareLocal(typeof(object[]));
			ig.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("CreateHoistedLocals"));
			ig.Emit(OpCodes.Stloc, hoisted_store);
		}

		public void EmitLoadLocals()
		{
			ig.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Locals"));
		}

		public void EmitParentScope()
		{
			ig.Emit(OpCodes.Ldfld, typeof(ExecutionScope).GetField("Parent"));
		}

		public void EmitIsolateExpression()
		{
			ig.Emit(OpCodes.Callvirt, typeof(ExecutionScope).GetMethod("IsolateExpression"));
		}

		public int IndexOfHoistedLocal(ParameterExpression parameter)
		{
			if (!HasHoistedLocals)
			{
				return -1;
			}
			return hoisted.IndexOf(parameter);
		}

		public bool IsHoistedLocal(ParameterExpression parameter, ref int level, ref int position)
		{
			if (parent == null)
			{
				return false;
			}
			if (parent.hoisted != null)
			{
				position = parent.hoisted.IndexOf(parameter);
				if (position > -1)
				{
					return true;
				}
			}
			level++;
			return parent.IsHoistedLocal(parameter, ref level, ref position);
		}

		private int AddChildContext(LambdaExpression lambda)
		{
			return context.AddCompilationUnit(this, lambda);
		}

		private static object CreateStrongBox(object value, Type type)
		{
			return Activator.CreateInstance(type.MakeStrongBoxType(), value);
		}
	}
}
