namespace UnityEngine
{
	internal class AndroidJavaRunnableProxy : AndroidJavaProxy
	{
		private AndroidJavaRunnable mRunnable;

		public AndroidJavaRunnableProxy(AndroidJavaRunnable runnable)
			: base("java/lang/Runnable")
		{
			mRunnable = runnable;
		}

		public void run()
		{
			mRunnable();
		}
	}
}
