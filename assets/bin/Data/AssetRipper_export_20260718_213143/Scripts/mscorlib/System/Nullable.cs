using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	public struct Nullable<T> where T : struct
	{
		internal T value;

		internal bool has_value;

		public bool HasValue
		{
			get
			{
				return has_value;
			}
		}

		public T Value
		{
			get
			{
				if (!has_value)
				{
					throw new InvalidOperationException("Nullable object must have a value.");
				}
				return value;
			}
		}

		public Nullable(T value)
		{
			has_value = true;
			this.value = value;
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return !has_value;
			}
			if (!(other is T?))
			{
				return false;
			}
			return Equals((T?)other);
		}

		private bool Equals(T? other)
		{
			if (other.has_value != has_value)
			{
				return false;
			}
			if (!has_value)
			{
				return true;
			}
			return other.value.Equals(value);
		}

		public override int GetHashCode()
		{
			if (!has_value)
			{
				return 0;
			}
			return value.GetHashCode();
		}

		public T GetValueOrDefault()
		{
			return (!has_value) ? default(T) : value;
		}

		public T GetValueOrDefault(T defaultValue)
		{
			return (!has_value) ? defaultValue : value;
		}

		public override string ToString()
		{
			if (has_value)
			{
				return value.ToString();
			}
			return string.Empty;
		}

		private static object Box(T? o)
		{
			if (!o.has_value)
			{
				return null;
			}
			return o.value;
		}

		private static T? Unbox(object o)
		{
			if (o == null)
			{
				return null;
			}
			return (T)o;
		}

		public static implicit operator T?(T value)
		{
			return value;
		}

		public static explicit operator T(T? value)
		{
			return value.Value;
		}
	}
	[ComVisible(true)]
	public static class Nullable
	{
		[ComVisible(false)]
		public static int Compare<T>(T? value1, T? value2) where T : struct
		{
			if (value1.has_value)
			{
				if (!value2.has_value)
				{
					return 1;
				}
				return Comparer<T>.Default.Compare(value1.value, value2.value);
			}
			return value2.has_value ? (-1) : 0;
		}

		[ComVisible(false)]
		public static bool Equals<T>(T? value1, T? value2) where T : struct
		{
			if (value1.has_value != value2.has_value)
			{
				return false;
			}
			if (!value1.has_value)
			{
				return true;
			}
			return EqualityComparer<T>.Default.Equals(value1.value, value2.value);
		}

		public static Type GetUnderlyingType(Type nullableType)
		{
			if (nullableType == null)
			{
				throw new ArgumentNullException("nullableType");
			}
			if (nullableType.IsGenericType && nullableType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return nullableType.GetGenericArguments()[0];
			}
			return null;
		}
	}
}
