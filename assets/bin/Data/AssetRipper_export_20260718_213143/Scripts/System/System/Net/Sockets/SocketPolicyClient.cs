using System.IO;
using System.Text;

namespace System.Net.Sockets
{
	internal static class SocketPolicyClient
	{
		private const string policy_request = "<policy-file-request/>\0";

		private static int session;

		private static void Log(string msg)
		{
			Console.WriteLine("SocketPolicyClient" + session + ": " + msg);
		}

		private static Stream GetPolicyStreamForIP(string ip, int policyport, int timeout)
		{
			session++;
			Log("Incoming GetPolicyStreamForIP");
			IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), policyport);
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			byte[] array = new byte[5000];
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				Log("About to BeginConnect to " + iPEndPoint);
				IAsyncResult asyncResult = socket.BeginConnect(iPEndPoint, null, null, false);
				Log("About to WaitOne");
				DateTime now = DateTime.Now;
				if (!asyncResult.AsyncWaitHandle.WaitOne(timeout))
				{
					Log("WaitOne timed out. Duration: " + (DateTime.Now - now).TotalMilliseconds);
					socket.Close();
					throw new Exception("BeginConnect timed out");
				}
				socket.EndConnect(asyncResult);
				Log("Socket connected");
				byte[] bytes = Encoding.ASCII.GetBytes("<policy-file-request/>\0");
				SocketError error;
				socket.Send_nochecks(bytes, 0, bytes.Length, SocketFlags.None, out error);
				if (error != SocketError.Success)
				{
					Log("Socket error: " + error);
					return memoryStream;
				}
				int count = socket.Receive_nochecks(array, 0, array.Length, SocketFlags.None, out error);
				if (error != SocketError.Success)
				{
					Log("Socket error: " + error);
					return memoryStream;
				}
				try
				{
					socket.Shutdown(SocketShutdown.Both);
					socket.Close();
				}
				catch (SocketException)
				{
				}
				memoryStream = new MemoryStream(array, 0, count);
			}
			catch (Exception ex2)
			{
				Log("Caught exception: " + ex2.Message);
				return memoryStream;
			}
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return memoryStream;
		}
	}
}
