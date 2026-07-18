using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace System.Net.NetworkInformation
{
	[System.MonoTODO("IPv6 support is missing")]
	public class Ping : Component, IDisposable
	{
		private struct cap_user_header_t
		{
			public uint version;

			public int pid;
		}

		private struct cap_user_data_t
		{
			public uint effective;

			public uint permitted;

			public uint inheritable;
		}

		private class IcmpMessage
		{
			private byte[] bytes;

			public byte Type
			{
				get
				{
					return bytes[0];
				}
			}

			public byte Code
			{
				get
				{
					return bytes[1];
				}
			}

			public byte Identifier
			{
				get
				{
					return (byte)(bytes[4] + (bytes[5] << 8));
				}
			}

			public byte Sequence
			{
				get
				{
					return (byte)(bytes[6] + (bytes[7] << 8));
				}
			}

			public byte[] Data
			{
				get
				{
					byte[] array = new byte[bytes.Length - 8];
					Buffer.BlockCopy(bytes, 0, array, 0, array.Length);
					return array;
				}
			}

			public IPStatus IPStatus
			{
				get
				{
					switch (Type)
					{
					case 0:
						return IPStatus.Success;
					case 3:
						switch (Code)
						{
						case 0:
							return IPStatus.DestinationNetworkUnreachable;
						case 1:
							return IPStatus.DestinationHostUnreachable;
						case 2:
							return IPStatus.DestinationProhibited;
						case 3:
							return IPStatus.DestinationPortUnreachable;
						case 4:
							return IPStatus.BadOption;
						case 5:
							return IPStatus.BadRoute;
						}
						break;
					case 11:
						switch (Code)
						{
						case 0:
							return IPStatus.TimeExceeded;
						case 1:
							return IPStatus.TtlReassemblyTimeExceeded;
						}
						break;
					case 12:
						return IPStatus.ParameterProblem;
					case 4:
						return IPStatus.SourceQuench;
					case 8:
						return IPStatus.Success;
					}
					return IPStatus.Unknown;
				}
			}

			public IcmpMessage(byte[] bytes, int offset, int size)
			{
				this.bytes = new byte[size];
				Buffer.BlockCopy(bytes, offset, this.bytes, 0, size);
			}

			public IcmpMessage(byte type, byte code, short identifier, short sequence, byte[] data)
			{
				bytes = new byte[data.Length + 8];
				bytes[0] = type;
				bytes[1] = code;
				bytes[4] = (byte)(identifier & 0xFF);
				bytes[5] = (byte)(identifier >> 8);
				bytes[6] = (byte)(sequence & 0xFF);
				bytes[7] = (byte)(sequence >> 8);
				Buffer.BlockCopy(data, 0, bytes, 8, data.Length);
				ushort num = ComputeChecksum(bytes);
				bytes[2] = (byte)(num & 0xFF);
				bytes[3] = (byte)(num >> 8);
			}

			public byte[] GetBytes()
			{
				return bytes;
			}

			private static ushort ComputeChecksum(byte[] data)
			{
				uint num = 0u;
				for (int i = 0; i < data.Length; i += 2)
				{
					ushort num2 = (ushort)((i + 1 < data.Length) ? data[i + 1] : 0);
					num2 <<= 8;
					num2 += data[i];
					num += num2;
				}
				num = (num >> 16) + (num & 0xFFFF);
				return (ushort)(~num);
			}
		}

		private const int DefaultCount = 1;

		private const string PingBinPath = "/bin/ping";

		private const int default_timeout = 4000;

		private const int identifier = 1;

		private const uint linux_cap_version = 537333798u;

		private static readonly byte[] default_buffer;

		private static bool canSendPrivileged;

		private BackgroundWorker worker;

		private object user_async_state;

		public event PingCompletedEventHandler PingCompleted;

		static Ping()
		{
			default_buffer = new byte[0];
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				CheckLinuxCapabilities();
				if (!canSendPrivileged && WindowsIdentity.GetCurrent().Name == "root")
				{
					canSendPrivileged = true;
				}
			}
			else
			{
				canSendPrivileged = true;
			}
		}

		void IDisposable.Dispose()
		{
		}

		[DllImport("libc")]
		private static extern int capget(ref cap_user_header_t header, ref cap_user_data_t data);

		private static void CheckLinuxCapabilities()
		{
			try
			{
				cap_user_header_t header = default(cap_user_header_t);
				cap_user_data_t data = default(cap_user_data_t);
				header.version = 537333798u;
				int num = -1;
				try
				{
					num = capget(ref header, ref data);
				}
				catch (Exception)
				{
				}
				if (num != -1)
				{
					canSendPrivileged = (data.effective & 0x2000) != 0;
				}
			}
			catch
			{
				canSendPrivileged = false;
			}
		}

		protected void OnPingCompleted(PingCompletedEventArgs e)
		{
			if (this.PingCompleted != null)
			{
				this.PingCompleted(this, e);
			}
			user_async_state = null;
			worker = null;
		}

		public PingReply Send(IPAddress address)
		{
			return Send(address, 4000);
		}

		public PingReply Send(IPAddress address, int timeout)
		{
			return Send(address, timeout, default_buffer);
		}

		public PingReply Send(IPAddress address, int timeout, byte[] buffer)
		{
			return Send(address, timeout, buffer, new PingOptions());
		}

		public PingReply Send(string hostNameOrAddress)
		{
			return Send(hostNameOrAddress, 4000);
		}

		public PingReply Send(string hostNameOrAddress, int timeout)
		{
			return Send(hostNameOrAddress, timeout, default_buffer);
		}

		public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer)
		{
			return Send(hostNameOrAddress, timeout, buffer, new PingOptions());
		}

		public PingReply Send(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options)
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostNameOrAddress);
			return Send(hostAddresses[0], timeout, buffer, options);
		}

		private static IPAddress GetNonLoopbackIP()
		{
			IPAddress[] addressList = Dns.GetHostByName(Dns.GetHostName()).AddressList;
			foreach (IPAddress iPAddress in addressList)
			{
				if (!IPAddress.IsLoopback(iPAddress))
				{
					return iPAddress;
				}
			}
			throw new InvalidOperationException("Could not resolve non-loopback IP address for localhost");
		}

		public PingReply Send(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			if (timeout < 0)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout must be non-negative integer");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length > 65500)
			{
				throw new ArgumentException("buffer");
			}
			if (canSendPrivileged)
			{
				return SendPrivileged(address, timeout, buffer, options);
			}
			return SendUnprivileged(address, timeout, buffer, options);
		}

		private PingReply SendPrivileged(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			IPEndPoint iPEndPoint = new IPEndPoint(address, 0);
			IPEndPoint iPEndPoint2 = new IPEndPoint(GetNonLoopbackIP(), 0);
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp))
			{
				if (options != null)
				{
					socket.DontFragment = options.DontFragment;
					socket.Ttl = (short)options.Ttl;
				}
				socket.SendTimeout = timeout;
				socket.ReceiveTimeout = timeout;
				IcmpMessage icmpMessage = new IcmpMessage(8, 0, 1, 0, buffer);
				byte[] bytes = icmpMessage.GetBytes();
				socket.SendBufferSize = bytes.Length;
				socket.SendTo(bytes, bytes.Length, SocketFlags.None, iPEndPoint);
				DateTime now = DateTime.Now;
				bytes = new byte[100];
				while (true)
				{
					EndPoint remote_end = iPEndPoint2;
					int error = 0;
					int num = socket.ReceiveFrom_nochecks_exc(bytes, 0, 100, SocketFlags.None, ref remote_end, false, out error);
					switch (error)
					{
					case 10060:
						return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
					default:
						throw new NotSupportedException(string.Format("Unexpected socket error during ping request: {0}", error));
					case 0:
					{
						long num2 = (long)(DateTime.Now - now).TotalMilliseconds;
						int num3 = (bytes[0] & 0xF) << 2;
						int size = num - num3;
						if (!((IPEndPoint)remote_end).Address.Equals(iPEndPoint.Address))
						{
							long num4 = timeout - num2;
							if (num4 <= 0)
							{
								return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
							}
							socket.ReceiveTimeout = (int)num4;
							break;
						}
						IcmpMessage icmpMessage2 = new IcmpMessage(bytes, num3, size);
						if (icmpMessage2.Identifier != 1 || icmpMessage2.Type == 8)
						{
							long num5 = timeout - num2;
							if (num5 <= 0)
							{
								return new PingReply(null, new byte[0], options, 0L, IPStatus.TimedOut);
							}
							socket.ReceiveTimeout = (int)num5;
							break;
						}
						return new PingReply(address, icmpMessage2.Data, options, num2, icmpMessage2.IPStatus);
					}
					}
				}
			}
		}

		private PingReply SendUnprivileged(IPAddress address, int timeout, byte[] buffer, PingOptions options)
		{
			DateTime now = DateTime.Now;
			Process process = new Process();
			string arguments = BuildPingArgs(address, timeout, options);
			long roundtripTime = 0L;
			process.StartInfo.FileName = "/bin/ping";
			process.StartInfo.Arguments = arguments;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			DateTime utcNow = DateTime.UtcNow;
			try
			{
				process.Start();
				roundtripTime = (long)(DateTime.Now - now).TotalMilliseconds;
				if (!process.WaitForExit(timeout) || (process.HasExited && process.ExitCode == 2))
				{
					return new PingReply(address, buffer, options, roundtripTime, IPStatus.TimedOut);
				}
				if (process.ExitCode == 1)
				{
					return new PingReply(address, buffer, options, roundtripTime, IPStatus.TtlExpired);
				}
			}
			catch (Exception)
			{
				return new PingReply(address, buffer, options, roundtripTime, IPStatus.Unknown);
			}
			finally
			{
				if (process != null)
				{
					if (!process.HasExited)
					{
						process.Kill();
					}
					process.Dispose();
				}
			}
			return new PingReply(address, buffer, options, roundtripTime, IPStatus.Success);
		}

		public void SendAsync(IPAddress address, int timeout, byte[] buffer, object userToken)
		{
			SendAsync(address, 4000, default_buffer, new PingOptions(), userToken);
		}

		public void SendAsync(IPAddress address, int timeout, object userToken)
		{
			SendAsync(address, 4000, default_buffer, userToken);
		}

		public void SendAsync(IPAddress address, object userToken)
		{
			SendAsync(address, 4000, userToken);
		}

		public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, object userToken)
		{
			SendAsync(hostNameOrAddress, timeout, buffer, new PingOptions(), userToken);
		}

		public void SendAsync(string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options, object userToken)
		{
			IPAddress address = Dns.GetHostEntry(hostNameOrAddress).AddressList[0];
			SendAsync(address, timeout, buffer, options, userToken);
		}

		public void SendAsync(string hostNameOrAddress, int timeout, object userToken)
		{
			SendAsync(hostNameOrAddress, timeout, default_buffer, userToken);
		}

		public void SendAsync(string hostNameOrAddress, object userToken)
		{
			SendAsync(hostNameOrAddress, 4000, userToken);
		}

		public void SendAsync(IPAddress address, int timeout, byte[] buffer, PingOptions options, object userToken)
		{
			if (worker != null)
			{
				throw new InvalidOperationException("Another SendAsync operation is in progress");
			}
			worker = new BackgroundWorker();
			worker.DoWork += delegate(object o, DoWorkEventArgs ea)
			{
				try
				{
					user_async_state = ea.Argument;
					ea.Result = Send(address, timeout, buffer, options);
				}
				catch (Exception result)
				{
					ea.Result = result;
				}
			};
			worker.WorkerSupportsCancellation = true;
			worker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs ea)
			{
				OnPingCompleted(new PingCompletedEventArgs(ea.Error, ea.Cancelled, user_async_state, ea.Result as PingReply));
			};
			worker.RunWorkerAsync(userToken);
		}

		public void SendAsyncCancel()
		{
			if (worker == null)
			{
				throw new InvalidOperationException("SendAsync operation is not in progress");
			}
			worker.CancelAsync();
		}

		private string BuildPingArgs(IPAddress address, int timeout, PingOptions options)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			StringBuilder stringBuilder = new StringBuilder();
			uint num = Convert.ToUInt32(Math.Floor((double)(timeout + 1000) / 1000.0));
			bool flag = Environment.OSVersion.Platform == PlatformID.MacOSX;
			if (!flag)
			{
				stringBuilder.AppendFormat(invariantCulture, "-q -n -c {0} -w {1} -t {2} -M ", 1, num, options.Ttl);
			}
			else
			{
				stringBuilder.AppendFormat(invariantCulture, "-q -n -c {0} -t {1} -o -m {2} ", 1, num, options.Ttl);
			}
			if (!flag)
			{
				stringBuilder.Append((!options.DontFragment) ? "dont " : "do ");
			}
			else if (options.DontFragment)
			{
				stringBuilder.Append("-D ");
			}
			stringBuilder.Append(address.ToString());
			return stringBuilder.ToString();
		}
	}
}
