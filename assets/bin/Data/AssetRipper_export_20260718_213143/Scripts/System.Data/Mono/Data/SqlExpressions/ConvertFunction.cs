using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class ConvertFunction : UnaryExpression
	{
		private Type targetType;

		public ConvertFunction(IExpression e, string targetType)
			: base(e)
		{
			try
			{
				this.targetType = Type.GetType(targetType, true);
			}
			catch (TypeLoadException)
			{
				throw new EvaluateException(string.Format("Invalid type name '{0}'.", targetType));
			}
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is ConvertFunction))
			{
				return false;
			}
			ConvertFunction convertFunction = (ConvertFunction)obj;
			if (convertFunction.targetType != targetType)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return targetType.GetHashCode() ^ base.GetHashCode();
		}

		public override object Eval(DataRow row)
		{
			object obj = expr.Eval(row);
			if (obj == null)
			{
				return DBNull.Value;
			}
			if (obj == DBNull.Value || obj.GetType() == targetType)
			{
				return obj;
			}
			if (targetType == typeof(string))
			{
				return obj.ToString();
			}
			if (targetType == typeof(TimeSpan))
			{
				if (obj is string)
				{
					return TimeSpan.Parse((string)obj);
				}
				ThrowInvalidCastException(obj);
			}
			if (obj is TimeSpan)
			{
				ThrowInvalidCastException(obj);
			}
			if (obj is char && targetType != typeof(int) && targetType != typeof(uint))
			{
				ThrowInvalidCastException(obj);
			}
			if (targetType == typeof(char) && !(obj is int) && !(obj is uint))
			{
				ThrowInvalidCastException(obj);
			}
			if (obj is bool && (targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal)))
			{
				ThrowInvalidCastException(obj);
			}
			if (targetType == typeof(bool) && (obj is float || obj is double || obj is decimal))
			{
				ThrowInvalidCastException(obj);
			}
			return Convert.ChangeType(obj, targetType);
		}

		private void ThrowInvalidCastException(object val)
		{
			throw new InvalidCastException(string.Format("Type '{0}' cannot be converted to '{1}'.", val.GetType(), targetType));
		}
	}
}
