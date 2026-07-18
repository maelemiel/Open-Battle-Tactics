using System.Collections;

namespace Mono.Data.Tds.Protocol
{
	public class TdsBulkCopy
	{
		private Tds tds;

		public TdsBulkCopy(Tds tds)
		{
			this.tds = tds;
		}

		public bool SendColumnMetaData(string colMetaData)
		{
			tds.Comm.StartPacket(TdsPacketType.Query);
			tds.Comm.Append(colMetaData);
			tds.ExecBulkCopyMetaData(30, false);
			return true;
		}

		public bool BulkCopyStart(TdsMetaParameterCollection parameters)
		{
			tds.Comm.StartPacket(TdsPacketType.Bulk);
			tds.Comm.Append((byte)129);
			short num = 0;
			foreach (TdsMetaParameter item in (IEnumerable)parameters)
			{
				if (item.Value == null)
				{
					num++;
				}
			}
			tds.Comm.Append(num);
			if (parameters != null)
			{
				foreach (TdsMetaParameter item2 in (IEnumerable)parameters)
				{
					if (item2.Value == null)
					{
						tds.Comm.Append((short)0);
						tds.Comm.Append((short)10);
						WriteParameterInfo(item2);
						tds.Comm.Append((byte)item2.ParameterName.Length);
						tds.Comm.Append(item2.ParameterName);
					}
				}
			}
			return true;
		}

		public bool BulkCopyData(object o, int size, bool isNewRow)
		{
			if (isNewRow)
			{
				tds.Comm.Append((byte)209);
			}
			if (size > 0)
			{
				tds.Comm.Append((short)size);
			}
			tds.Comm.Append(o);
			return true;
		}

		public bool BulkCopyEnd()
		{
			tds.Comm.Append((short)253);
			tds.ExecBulkCopy(30, false);
			return true;
		}

		private void WriteParameterInfo(TdsMetaParameter param)
		{
			param.IsNullable = true;
			TdsColumnType metaType = param.GetMetaType();
			param.IsNullable = false;
			tds.Comm.Append((byte)metaType);
			int num = 0;
			num = ((param.Size != 0) ? param.Size : param.GetActualSize());
			if (metaType == TdsColumnType.BigNVarChar)
			{
				num <<= 1;
			}
			if (tds.IsLargeType(metaType))
			{
				tds.Comm.Append((short)num);
			}
			else if (tds.IsBlobType(metaType))
			{
				tds.Comm.Append(num);
			}
			else
			{
				tds.Comm.Append((byte)num);
			}
			if (param.TypeName == "decimal" || param.TypeName == "numeric")
			{
				tds.Comm.Append((byte)((param.Precision == 0) ? 29 : param.Precision));
				tds.Comm.Append(param.Scale);
			}
		}
	}
}
