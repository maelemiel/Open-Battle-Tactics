using System;
using System.Data;

namespace Mono.Data.SqlExpressions
{
	internal class Aggregation : BaseExpression
	{
		private bool cacheResults;

		private DataRow[] rows;

		private ColumnReference column;

		private AggregationFunction function;

		private int count;

		private IConvertible result;

		private DataRowChangeEventHandler RowChangeHandler;

		private DataTable table;

		public Aggregation(bool cacheResults, DataRow[] rows, AggregationFunction function, ColumnReference column)
		{
			this.cacheResults = cacheResults;
			this.rows = rows;
			this.column = column;
			this.function = function;
			result = null;
			if (cacheResults)
			{
				RowChangeHandler = InvalidateCache;
			}
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			if (!(obj is Aggregation))
			{
				return false;
			}
			Aggregation aggregation = (Aggregation)obj;
			if (!aggregation.function.Equals(function))
			{
				return false;
			}
			if (!aggregation.column.Equals(column))
			{
				return false;
			}
			if (aggregation.rows != null && rows != null)
			{
				if (aggregation.rows.Length != rows.Length)
				{
					return false;
				}
				for (int i = 0; i < rows.Length; i++)
				{
					if (aggregation.rows[i] != rows[i])
					{
						return false;
					}
				}
			}
			else if (aggregation.rows != null || rows != null)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = base.GetHashCode();
			hashCode ^= function.GetHashCode();
			hashCode ^= column.GetHashCode();
			for (int i = 0; i < rows.Length; i++)
			{
				hashCode ^= rows[i].GetHashCode();
			}
			return hashCode;
		}

		public override object Eval(DataRow row)
		{
			if (cacheResults && result != null && column.ReferencedTable == ReferencedTable.Self)
			{
				return result;
			}
			count = 0;
			result = null;
			object[] array = ((rows != null) ? column.GetValues(rows) : column.GetValues(column.GetReferencedRows(row)));
			object[] array2 = array;
			foreach (object obj in array2)
			{
				if (obj != null)
				{
					count++;
					Aggregate((IConvertible)obj);
				}
			}
			switch (function)
			{
			case AggregationFunction.StDev:
			case AggregationFunction.Var:
				result = CalcStatisticalFunction(array);
				break;
			case AggregationFunction.Avg:
			{
				IConvertible convertible;
				if (count == 0)
				{
					IConvertible value = DBNull.Value;
					convertible = value;
				}
				else
				{
					convertible = Numeric.Divide(result, count);
				}
				result = convertible;
				break;
			}
			case AggregationFunction.Count:
				result = count;
				break;
			}
			if (result == null)
			{
				result = DBNull.Value;
			}
			if (cacheResults && column.ReferencedTable == ReferencedTable.Self)
			{
				table = row.Table;
				row.Table.RowChanged += RowChangeHandler;
			}
			return result;
		}

		public override bool DependsOn(DataColumn other)
		{
			return column.DependsOn(other);
		}

		private void Aggregate(IConvertible val)
		{
			switch (function)
			{
			case AggregationFunction.Min:
			{
				IConvertible convertible4;
				if (result != null)
				{
					IConvertible convertible = Numeric.Min(result, val);
					convertible4 = convertible;
				}
				else
				{
					convertible4 = val;
				}
				result = convertible4;
				break;
			}
			case AggregationFunction.Max:
			{
				IConvertible convertible3;
				if (result != null)
				{
					IConvertible convertible = Numeric.Max(result, val);
					convertible3 = convertible;
				}
				else
				{
					convertible3 = val;
				}
				result = convertible3;
				break;
			}
			case AggregationFunction.Sum:
			case AggregationFunction.Avg:
			case AggregationFunction.StDev:
			case AggregationFunction.Var:
			{
				IConvertible convertible2;
				if (result != null)
				{
					IConvertible convertible = Numeric.Add(result, val);
					convertible2 = convertible;
				}
				else
				{
					convertible2 = val;
				}
				result = convertible2;
				break;
			}
			}
		}

		private IConvertible CalcStatisticalFunction(object[] values)
		{
			if (count < 2)
			{
				return DBNull.Value;
			}
			double num = (double)Convert.ChangeType(result, TypeCode.Double) / (double)count;
			double num2 = 0.0;
			foreach (object obj in values)
			{
				if (obj != null)
				{
					double x = num - (double)Convert.ChangeType(obj, TypeCode.Double);
					num2 += System.Math.Pow(x, 2.0);
				}
			}
			num2 /= (double)(count - 1);
			if (function == AggregationFunction.StDev)
			{
				num2 = System.Math.Sqrt(num2);
			}
			return num2;
		}

		public override void ResetExpression()
		{
			if (table != null)
			{
				InvalidateCache(table, null);
			}
		}

		private void InvalidateCache(object sender, DataRowChangeEventArgs args)
		{
			result = null;
			((DataTable)sender).RowChanged -= RowChangeHandler;
		}
	}
}
