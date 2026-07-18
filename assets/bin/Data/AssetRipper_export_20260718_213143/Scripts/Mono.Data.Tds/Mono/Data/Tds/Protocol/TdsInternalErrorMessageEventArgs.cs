namespace Mono.Data.Tds.Protocol
{
	public sealed class TdsInternalErrorMessageEventArgs : TdsInternalInfoMessageEventArgs
	{
		public TdsInternalErrorMessageEventArgs(TdsInternalError error)
			: base(error)
		{
		}
	}
}
