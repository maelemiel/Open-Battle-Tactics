using System.Collections;

namespace System.Reflection.Emit
{
	internal class EventOnTypeBuilderInst : EventInfo
	{
		private MonoGenericClass instantiation;

		private EventBuilder evt;

		public override EventAttributes Attributes
		{
			get
			{
				return evt.attrs;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return instantiation;
			}
		}

		public override string Name
		{
			get
			{
				return evt.name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return instantiation;
			}
		}

		internal EventOnTypeBuilderInst(MonoGenericClass instantiation, EventBuilder evt)
		{
			this.instantiation = instantiation;
			this.evt = evt;
		}

		public override MethodInfo GetAddMethod(bool nonPublic)
		{
			if (evt.add_method == null || (!nonPublic && !evt.add_method.IsPublic))
			{
				return null;
			}
			return TypeBuilder.GetMethod(instantiation, evt.add_method);
		}

		public override MethodInfo GetRaiseMethod(bool nonPublic)
		{
			if (evt.raise_method == null || (!nonPublic && !evt.raise_method.IsPublic))
			{
				return null;
			}
			return TypeBuilder.GetMethod(instantiation, evt.raise_method);
		}

		public override MethodInfo GetRemoveMethod(bool nonPublic)
		{
			if (evt.remove_method == null || (!nonPublic && !evt.remove_method.IsPublic))
			{
				return null;
			}
			return TypeBuilder.GetMethod(instantiation, evt.remove_method);
		}

		public override MethodInfo[] GetOtherMethods(bool nonPublic)
		{
			if (evt.other_methods == null)
			{
				return new MethodInfo[0];
			}
			ArrayList arrayList = new ArrayList();
			MethodBuilder[] other_methods = evt.other_methods;
			foreach (MethodInfo methodInfo in other_methods)
			{
				if (nonPublic || methodInfo.IsPublic)
				{
					arrayList.Add(TypeBuilder.GetMethod(instantiation, methodInfo));
				}
			}
			MethodInfo[] array = new MethodInfo[arrayList.Count];
			arrayList.CopyTo(array, 0);
			return array;
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}
	}
}
