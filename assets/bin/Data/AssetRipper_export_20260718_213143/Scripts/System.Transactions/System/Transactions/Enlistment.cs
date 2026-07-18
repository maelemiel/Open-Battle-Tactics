namespace System.Transactions
{
	public class Enlistment
	{
		internal bool done;

		internal Enlistment()
		{
			done = false;
		}

		public void Done()
		{
			done = true;
		}
	}
}
