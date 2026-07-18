namespace System.Diagnostics
{
	public enum ThreadState
	{
		Initialized = 0,
		Ready = 1,
		Running = 2,
		Standby = 3,
		Terminated = 4,
		Transition = 6,
		Unknown = 7,
		Wait = 5
	}
}
