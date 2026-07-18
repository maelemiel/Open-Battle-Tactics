using Mono.Data.SqlExpressions;

namespace System.Data
{
	internal class RelatedDataView : DataView, IExpression
	{
		private object[] _keyValues;

		private DataColumn[] _columns;

		internal override IExpression FilterExpression
		{
			get
			{
				return this;
			}
		}

		internal RelatedDataView(DataColumn[] relatedColumns, object[] keyValues)
		{
			dataTable = relatedColumns[0].Table;
			rowState = DataViewRowState.CurrentRows;
			_columns = relatedColumns;
			_keyValues = keyValues;
			Open();
		}

		void IExpression.ResetExpression()
		{
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RelatedDataView))
			{
				if (base.FilterExpression == null)
				{
					return false;
				}
				return base.FilterExpression.Equals(obj);
			}
			RelatedDataView relatedDataView = (RelatedDataView)obj;
			if (_columns.Length != relatedDataView._columns.Length)
			{
				return false;
			}
			for (int i = 0; i < _columns.Length; i++)
			{
				if (!_columns[i].Equals(relatedDataView._columns[i]) || !_keyValues[i].Equals(relatedDataView._keyValues[i]))
				{
					return false;
				}
			}
			if (!relatedDataView.FilterExpression.Equals(base.FilterExpression))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < _columns.Length; i++)
			{
				num ^= _columns[i].GetHashCode();
				num ^= _keyValues[i].GetHashCode();
			}
			if (base.FilterExpression != null)
			{
				num ^= base.FilterExpression.GetHashCode();
			}
			return num;
		}

		public object Eval(DataRow row)
		{
			return EvalBoolean(row);
		}

		public bool EvalBoolean(DataRow row)
		{
			for (int i = 0; i < _columns.Length; i++)
			{
				if (!row[_columns[i]].Equals(_keyValues[i]))
				{
					return false;
				}
			}
			IExpression filterExpression = base.FilterExpression;
			return filterExpression == null || filterExpression.EvalBoolean(row);
		}

		public bool DependsOn(DataColumn other)
		{
			for (int i = 0; i < _columns.Length; i++)
			{
				if (_columns[i] == other)
				{
					return true;
				}
			}
			IExpression filterExpression = base.FilterExpression;
			return filterExpression != null && filterExpression.DependsOn(other);
		}
	}
}
