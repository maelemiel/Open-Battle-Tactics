namespace Mono.Data.Tds.Protocol
{
	public enum TdsRpcProcId
	{
		Cursor = 1,
		CursorOpen = 2,
		CursorPrepare = 3,
		CursorExecute = 4,
		CursorPrepExec = 5,
		CursorUnprepare = 6,
		CursorFetch = 7,
		CursorOption = 8,
		CursorClose = 9,
		ExecuteSql = 10,
		Prepare = 11,
		Execute = 12,
		PrepExec = 13,
		PrepExecRpc = 14,
		Unprepare = 15
	}
}
