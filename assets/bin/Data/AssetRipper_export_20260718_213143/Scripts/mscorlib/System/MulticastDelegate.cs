using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	public abstract class MulticastDelegate : Delegate
	{
		private MulticastDelegate prev;

		private MulticastDelegate kpm_next;

		protected MulticastDelegate(object target, string method)
			: base(target, method)
		{
			prev = null;
		}

		protected MulticastDelegate(Type target, string method)
			: base(target, method)
		{
			prev = null;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		protected sealed override object DynamicInvokeImpl(object[] args)
		{
			if ((object)prev != null)
			{
				prev.DynamicInvokeImpl(args);
			}
			return base.DynamicInvokeImpl(args);
		}

		public sealed override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			MulticastDelegate multicastDelegate = obj as MulticastDelegate;
			if ((object)multicastDelegate == null)
			{
				return false;
			}
			if ((object)prev == null)
			{
				if ((object)multicastDelegate.prev == null)
				{
					return true;
				}
				return false;
			}
			return prev.Equals(multicastDelegate.prev);
		}

		public sealed override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public sealed override Delegate[] GetInvocationList()
		{
			MulticastDelegate multicastDelegate = (MulticastDelegate)Clone();
			multicastDelegate.kpm_next = null;
			while ((object)multicastDelegate.prev != null)
			{
				multicastDelegate.prev.kpm_next = multicastDelegate;
				multicastDelegate = multicastDelegate.prev;
			}
			if ((object)multicastDelegate.kpm_next == null)
			{
				MulticastDelegate multicastDelegate2 = (MulticastDelegate)multicastDelegate.Clone();
				multicastDelegate2.prev = null;
				multicastDelegate2.kpm_next = null;
				return new Delegate[1] { multicastDelegate2 };
			}
			ArrayList arrayList = new ArrayList();
			while ((object)multicastDelegate != null)
			{
				MulticastDelegate multicastDelegate3 = (MulticastDelegate)multicastDelegate.Clone();
				multicastDelegate3.prev = null;
				multicastDelegate3.kpm_next = null;
				arrayList.Add(multicastDelegate3);
				multicastDelegate = multicastDelegate.kpm_next;
			}
			return (Delegate[])arrayList.ToArray(typeof(Delegate));
		}

		protected sealed override Delegate CombineImpl(Delegate follow)
		{
			if (GetType() != follow.GetType())
			{
				throw new ArgumentException(Locale.GetText("Incompatible Delegate Types."));
			}
			MulticastDelegate multicastDelegate = (MulticastDelegate)follow.Clone();
			multicastDelegate.SetMulticastInvoke();
			MulticastDelegate multicastDelegate2 = multicastDelegate;
			MulticastDelegate multicastDelegate3 = ((MulticastDelegate)follow).prev;
			while ((object)multicastDelegate3 != null)
			{
				multicastDelegate2.prev = (MulticastDelegate)multicastDelegate3.Clone();
				multicastDelegate2 = multicastDelegate2.prev;
				multicastDelegate3 = multicastDelegate3.prev;
			}
			multicastDelegate2.prev = (MulticastDelegate)Clone();
			multicastDelegate2 = multicastDelegate2.prev;
			multicastDelegate3 = prev;
			while ((object)multicastDelegate3 != null)
			{
				multicastDelegate2.prev = (MulticastDelegate)multicastDelegate3.Clone();
				multicastDelegate2 = multicastDelegate2.prev;
				multicastDelegate3 = multicastDelegate3.prev;
			}
			return multicastDelegate;
		}

		private bool BaseEquals(MulticastDelegate value)
		{
			return base.Equals((object)value);
		}

		private static MulticastDelegate KPM(MulticastDelegate needle, MulticastDelegate haystack, out MulticastDelegate tail)
		{
			MulticastDelegate multicastDelegate = needle;
			MulticastDelegate multicastDelegate2 = (needle.kpm_next = null);
			while (true)
			{
				if ((object)multicastDelegate2 != null && !multicastDelegate2.BaseEquals(multicastDelegate))
				{
					multicastDelegate2 = multicastDelegate2.kpm_next;
					continue;
				}
				multicastDelegate = multicastDelegate.prev;
				if ((object)multicastDelegate == null)
				{
					break;
				}
				multicastDelegate2 = (((object)multicastDelegate2 != null) ? multicastDelegate2.prev : needle);
				if (multicastDelegate.BaseEquals(multicastDelegate2))
				{
					multicastDelegate.kpm_next = multicastDelegate2.kpm_next;
				}
				else
				{
					multicastDelegate.kpm_next = multicastDelegate2;
				}
			}
			MulticastDelegate multicastDelegate3 = haystack;
			multicastDelegate2 = needle;
			multicastDelegate = haystack;
			while (true)
			{
				if ((object)multicastDelegate2 != null && !multicastDelegate2.BaseEquals(multicastDelegate))
				{
					multicastDelegate2 = multicastDelegate2.kpm_next;
					multicastDelegate3 = multicastDelegate3.prev;
					continue;
				}
				multicastDelegate2 = (((object)multicastDelegate2 != null) ? multicastDelegate2.prev : needle);
				if ((object)multicastDelegate2 == null)
				{
					tail = multicastDelegate.prev;
					return multicastDelegate3;
				}
				multicastDelegate = multicastDelegate.prev;
				if ((object)multicastDelegate == null)
				{
					break;
				}
			}
			tail = null;
			return null;
		}

		protected sealed override Delegate RemoveImpl(Delegate value)
		{
			if ((object)value == null)
			{
				return this;
			}
			MulticastDelegate tail;
			MulticastDelegate multicastDelegate = KPM((MulticastDelegate)value, this, out tail);
			if ((object)multicastDelegate == null)
			{
				return this;
			}
			MulticastDelegate multicastDelegate2 = null;
			MulticastDelegate result = null;
			MulticastDelegate multicastDelegate3 = this;
			while ((object)multicastDelegate3 != multicastDelegate)
			{
				MulticastDelegate multicastDelegate4 = (MulticastDelegate)multicastDelegate3.Clone();
				if ((object)multicastDelegate2 != null)
				{
					multicastDelegate2.prev = multicastDelegate4;
				}
				else
				{
					result = multicastDelegate4;
				}
				multicastDelegate2 = multicastDelegate4;
				multicastDelegate3 = multicastDelegate3.prev;
			}
			multicastDelegate3 = tail;
			while ((object)multicastDelegate3 != null)
			{
				MulticastDelegate multicastDelegate5 = (MulticastDelegate)multicastDelegate3.Clone();
				if ((object)multicastDelegate2 != null)
				{
					multicastDelegate2.prev = multicastDelegate5;
				}
				else
				{
					result = multicastDelegate5;
				}
				multicastDelegate2 = multicastDelegate5;
				multicastDelegate3 = multicastDelegate3.prev;
			}
			if ((object)multicastDelegate2 != null)
			{
				multicastDelegate2.prev = null;
			}
			return result;
		}

		public static bool operator ==(MulticastDelegate d1, MulticastDelegate d2)
		{
			if ((object)d1 == null)
			{
				return (object)d2 == null;
			}
			return d1.Equals(d2);
		}

		public static bool operator !=(MulticastDelegate d1, MulticastDelegate d2)
		{
			if ((object)d1 == null)
			{
				return (object)d2 != null;
			}
			return !d1.Equals(d2);
		}
	}
}
