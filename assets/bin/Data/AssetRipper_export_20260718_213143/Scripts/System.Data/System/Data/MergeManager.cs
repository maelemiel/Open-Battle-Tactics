using System.Collections;

namespace System.Data
{
	internal class MergeManager
	{
		internal static void Merge(DataSet targetSet, DataSet sourceSet, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (targetSet == null)
			{
				throw new ArgumentNullException("targetSet");
			}
			if (sourceSet == null)
			{
				throw new ArgumentNullException("sourceSet");
			}
			if (sourceSet == targetSet)
			{
				return;
			}
			bool enforceConstraints = targetSet.EnforceConstraints;
			targetSet.EnforceConstraints = false;
			foreach (DataTable table in sourceSet.Tables)
			{
				Merge(targetSet, table, preserveChanges, missingSchemaAction);
			}
			AdjustSchemaRelations(targetSet, sourceSet, missingSchemaAction);
			targetSet.EnforceConstraints = enforceConstraints;
		}

		internal static void Merge(DataSet targetSet, DataTable sourceTable, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (targetSet == null)
			{
				throw new ArgumentNullException("targetSet");
			}
			if (sourceTable == null)
			{
				throw new ArgumentNullException("sourceTable");
			}
			if (sourceTable.DataSet == targetSet)
			{
				return;
			}
			bool enforceConstraints = targetSet.EnforceConstraints;
			targetSet.EnforceConstraints = false;
			DataTable newTable = null;
			if (AdjustSchema(targetSet, sourceTable, missingSchemaAction, ref newTable))
			{
				if (newTable != null)
				{
					fillData(newTable, sourceTable, preserveChanges);
				}
				targetSet.EnforceConstraints = enforceConstraints;
			}
		}

		internal static void Merge(DataTable targetTable, DataTable sourceTable, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (targetTable == null)
			{
				throw new ArgumentNullException("targetTable");
			}
			if (sourceTable == null)
			{
				throw new ArgumentNullException("sourceTable");
			}
			if (sourceTable != targetTable)
			{
				bool enforceConstraints = targetTable.EnforceConstraints;
				targetTable.EnforceConstraints = false;
				if (AdjustSchema(targetTable, sourceTable, missingSchemaAction))
				{
					fillData(targetTable, sourceTable, preserveChanges);
					targetTable.EnforceConstraints = enforceConstraints;
				}
			}
		}

		internal static void Merge(DataSet targetSet, DataRow[] sourceRows, bool preserveChanges, MissingSchemaAction missingSchemaAction)
		{
			if (targetSet == null)
			{
				throw new ArgumentNullException("targetSet");
			}
			if (sourceRows == null)
			{
				throw new ArgumentNullException("sourceRows");
			}
			bool enforceConstraints = targetSet.EnforceConstraints;
			targetSet.EnforceConstraints = false;
			ArrayList arrayList = new ArrayList();
			foreach (DataRow dataRow in sourceRows)
			{
				DataTable table = dataRow.Table;
				DataTable newTable = null;
				if (!AdjustSchema(targetSet, table, missingSchemaAction, ref newTable))
				{
					return;
				}
				if (newTable != null)
				{
					MergeRow(newTable, dataRow, preserveChanges);
					if (arrayList.IndexOf(newTable) < 0)
					{
						arrayList.Add(newTable);
					}
				}
			}
			targetSet.EnforceConstraints = enforceConstraints;
		}

		private static void MergeRow(DataTable targetTable, DataRow row, bool preserveChanges)
		{
			DataColumn[] primaryKey = targetTable.PrimaryKey;
			DataRow dataRow = null;
			DataRowVersion version = DataRowVersion.Default;
			if (row.RowState == DataRowState.Deleted)
			{
				version = DataRowVersion.Original;
			}
			if (primaryKey != null && primaryKey.Length > 0)
			{
				object[] array = new object[primaryKey.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = row[primaryKey[i].ColumnName, version];
				}
				dataRow = targetTable.Rows.Find(array, DataViewRowState.OriginalRows);
				if (dataRow == null)
				{
					dataRow = targetTable.Rows.Find(array);
				}
			}
			if (dataRow == null)
			{
				DataRow row2 = targetTable.NewNotInitializedRow();
				row.CopyValuesToRow(row2);
				targetTable.Rows.AddInternal(row2);
			}
			else
			{
				row.MergeValuesToRow(dataRow, preserveChanges);
			}
		}

		private static bool AdjustSchemaRelations(DataSet targetSet, DataSet sourceSet, MissingSchemaAction missingSchemaAction)
		{
			if (missingSchemaAction == MissingSchemaAction.Ignore)
			{
				return true;
			}
			foreach (DataTable table in sourceSet.Tables)
			{
				DataTable dataTable2 = targetSet.Tables[table.TableName];
				if (dataTable2 == null)
				{
					continue;
				}
				foreach (Constraint constraint3 in table.Constraints)
				{
					Constraint constraint2 = null;
					string text = constraint3.ConstraintName;
					if (dataTable2.Constraints.Contains(text))
					{
						text = string.Empty;
					}
					UniqueConstraint uniqueConstraint = constraint3 as UniqueConstraint;
					if (uniqueConstraint != null)
					{
						if (uniqueConstraint.IsPrimaryKey || uniqueConstraint.ChildConstraint != null)
						{
							continue;
						}
						DataColumn[] columns = ResolveColumns(dataTable2, uniqueConstraint.Columns);
						constraint2 = new UniqueConstraint(text, columns, false);
					}
					ForeignKeyConstraint foreignKeyConstraint = constraint3 as ForeignKeyConstraint;
					if (foreignKeyConstraint != null)
					{
						DataColumn[] childColumns = ResolveColumns(dataTable2, foreignKeyConstraint.Columns);
						DataColumn[] parentColumns = ResolveColumns(targetSet.Tables[foreignKeyConstraint.RelatedTable.TableName], foreignKeyConstraint.RelatedColumns);
						constraint2 = new ForeignKeyConstraint(text, parentColumns, childColumns);
					}
					bool flag = false;
					foreach (Constraint constraint4 in dataTable2.Constraints)
					{
						if (!constraint2.Equals(constraint4))
						{
							continue;
						}
						flag = true;
						break;
					}
					if (!flag)
					{
						if (missingSchemaAction == MissingSchemaAction.Error)
						{
							throw new DataException(string.Concat("Target DataSet missing ", constraint2.GetType(), constraint2.ConstraintName));
						}
						dataTable2.Constraints.Add(constraint2);
					}
				}
			}
			foreach (DataRelation relation in sourceSet.Relations)
			{
				DataRelation dataRelation2 = targetSet.Relations[relation.RelationName];
				if (dataRelation2 == null)
				{
					if (missingSchemaAction == MissingSchemaAction.Error)
					{
						throw new ArgumentException("Target DataSet mising definition for " + relation.RelationName);
					}
					DataColumn[] parentColumns2 = ResolveColumns(targetSet.Tables[relation.ParentTable.TableName], relation.ParentColumns);
					DataColumn[] childColumns2 = ResolveColumns(targetSet.Tables[relation.ChildTable.TableName], relation.ChildColumns);
					dataRelation2 = targetSet.Relations.Add(relation.RelationName, parentColumns2, childColumns2, relation.createConstraints);
					dataRelation2.Nested = relation.Nested;
				}
				else if (!CompareColumnArrays(relation.ParentColumns, dataRelation2.ParentColumns) || !CompareColumnArrays(relation.ChildColumns, dataRelation2.ChildColumns))
				{
					RaiseMergeFailedEvent(null, "Relation " + relation.RelationName + " cannot be merged, because keys have mismatch columns.");
				}
			}
			return true;
		}

		private static DataColumn[] ResolveColumns(DataTable targetTable, DataColumn[] sourceColumns)
		{
			if (sourceColumns != null && sourceColumns.Length > 0 && targetTable != null)
			{
				int num = 0;
				DataColumn[] array = new DataColumn[sourceColumns.Length];
				foreach (DataColumn dataColumn in sourceColumns)
				{
					DataColumn dataColumn2 = targetTable.Columns[dataColumn.ColumnName];
					if (dataColumn2 == null)
					{
						throw new DataException("Column " + dataColumn.ColumnName + " does not belong to table " + targetTable.TableName);
					}
					array[num++] = dataColumn2;
				}
				return array;
			}
			return null;
		}

		private static bool AdjustSchema(DataSet targetSet, DataTable sourceTable, MissingSchemaAction missingSchemaAction, ref DataTable newTable)
		{
			DataTable dataTable = targetSet.Tables[sourceTable.TableName];
			if (dataTable == null)
			{
				switch (missingSchemaAction)
				{
				case MissingSchemaAction.Ignore:
					return true;
				case MissingSchemaAction.Error:
					throw new ArgumentException("Target DataSet missing definition for " + sourceTable.TableName + ".");
				}
				dataTable = sourceTable.Clone();
				targetSet.Tables.Add(dataTable);
			}
			AdjustSchema(dataTable, sourceTable, missingSchemaAction);
			newTable = dataTable;
			return true;
		}

		private static bool AdjustSchema(DataTable targetTable, DataTable sourceTable, MissingSchemaAction missingSchemaAction)
		{
			if (missingSchemaAction == MissingSchemaAction.Ignore)
			{
				return true;
			}
			for (int i = 0; i < sourceTable.Columns.Count; i++)
			{
				DataColumn dataColumn = sourceTable.Columns[i];
				DataColumn dataColumn2 = targetTable.Columns[dataColumn.ColumnName];
				if (dataColumn2 == null)
				{
					if (missingSchemaAction == MissingSchemaAction.Error)
					{
						throw new DataException("Target table " + targetTable.TableName + " missing definition for column " + dataColumn.ColumnName);
					}
					dataColumn2 = new DataColumn(dataColumn.ColumnName, dataColumn.DataType, dataColumn.Expression, dataColumn.ColumnMapping);
					targetTable.Columns.Add(dataColumn2);
				}
				if (dataColumn.AutoIncrement)
				{
					dataColumn2.AutoIncrement = dataColumn.AutoIncrement;
					dataColumn2.AutoIncrementSeed = dataColumn.AutoIncrementSeed;
					dataColumn2.AutoIncrementStep = dataColumn.AutoIncrementStep;
				}
			}
			if (!AdjustPrimaryKeys(targetTable, sourceTable))
			{
				return false;
			}
			checkColumnTypes(targetTable, sourceTable);
			return true;
		}

		private static bool AdjustPrimaryKeys(DataTable targetTable, DataTable sourceTable)
		{
			if (sourceTable.PrimaryKey.Length == 0)
			{
				return true;
			}
			if (targetTable.PrimaryKey.Length == 0)
			{
				DataColumn[] primaryKey = ResolveColumns(targetTable, sourceTable.PrimaryKey);
				targetTable.PrimaryKey = primaryKey;
				return true;
			}
			if (targetTable.PrimaryKey.Length != sourceTable.PrimaryKey.Length)
			{
				RaiseMergeFailedEvent(targetTable, "<target>.PrimaryKey and <source>.PrimaryKey have different Length.");
				return false;
			}
			for (int i = 0; i < targetTable.PrimaryKey.Length; i++)
			{
				if (!targetTable.PrimaryKey[i].ColumnName.Equals(sourceTable.PrimaryKey[i].ColumnName))
				{
					RaiseMergeFailedEvent(targetTable, "Mismatch columns in the PrimaryKey : <target>." + targetTable.PrimaryKey[i].ColumnName + " versus <source>." + sourceTable.PrimaryKey[i].ColumnName);
					return false;
				}
			}
			return true;
		}

		private static void fillData(DataTable targetTable, DataTable sourceTable, bool preserveChanges)
		{
			for (int i = 0; i < sourceTable.Rows.Count; i++)
			{
				DataRow row = sourceTable.Rows[i];
				MergeRow(targetTable, row, preserveChanges);
			}
		}

		private static void checkColumnTypes(DataTable targetTable, DataTable sourceTable)
		{
			for (int i = 0; i < sourceTable.Columns.Count; i++)
			{
				DataColumn dataColumn = sourceTable.Columns[i];
				DataColumn dataColumn2 = targetTable.Columns[dataColumn.ColumnName];
				if (dataColumn2 != null && !dataColumn2.DataTypeMatches(dataColumn))
				{
					throw new DataException("<target>." + dataColumn.ColumnName + " and <source>." + dataColumn.ColumnName + " have conflicting properties: DataType  property mismatch.");
				}
			}
		}

		private static bool CompareColumnArrays(DataColumn[] arr1, DataColumn[] arr2)
		{
			if (arr1.Length != arr2.Length)
			{
				return false;
			}
			for (int i = 0; i < arr1.Length; i++)
			{
				if (!arr1[i].ColumnName.Equals(arr2[i].ColumnName))
				{
					return false;
				}
			}
			return true;
		}

		private static void RaiseMergeFailedEvent(DataTable targetTable, string errMsg)
		{
			MergeFailedEventArgs e = new MergeFailedEventArgs(targetTable, errMsg);
			if (targetTable.DataSet != null)
			{
				targetTable.DataSet.OnMergeFailed(e);
			}
		}
	}
}
