using System.Security.Principal;
using System.Threading;

namespace System.Security
{
	public sealed class SecurityContext
	{
		private bool _capture;

		private IntPtr _winid;

		private CompressedStack _stack;

		private bool _suppressFlowWindowsIdentity;

		private bool _suppressFlow;

		internal bool FlowSuppressed
		{
			get
			{
				return _suppressFlow;
			}
			set
			{
				_suppressFlow = value;
			}
		}

		internal bool WindowsIdentityFlowSuppressed
		{
			get
			{
				return _suppressFlowWindowsIdentity;
			}
			set
			{
				_suppressFlowWindowsIdentity = value;
			}
		}

		internal CompressedStack CompressedStack
		{
			get
			{
				return _stack;
			}
			set
			{
				_stack = value;
			}
		}

		internal IntPtr IdentityToken
		{
			get
			{
				return _winid;
			}
			set
			{
				_winid = value;
			}
		}

		internal SecurityContext()
		{
		}

		internal SecurityContext(SecurityContext sc)
		{
			_capture = true;
			_winid = sc._winid;
			if (sc._stack != null)
			{
				_stack = sc._stack.CreateCopy();
			}
		}

		public SecurityContext CreateCopy()
		{
			if (!_capture)
			{
				throw new InvalidOperationException();
			}
			return new SecurityContext(this);
		}

		public static SecurityContext Capture()
		{
			SecurityContext securityContext = Thread.CurrentThread.ExecutionContext.SecurityContext;
			if (securityContext.FlowSuppressed)
			{
				return null;
			}
			SecurityContext securityContext2 = new SecurityContext();
			securityContext2._capture = true;
			securityContext2._winid = WindowsIdentity.GetCurrentToken();
			securityContext2._stack = CompressedStack.Capture();
			return securityContext2;
		}

		public static bool IsFlowSuppressed()
		{
			return Thread.CurrentThread.ExecutionContext.SecurityContext.FlowSuppressed;
		}

		public static bool IsWindowsIdentityFlowSuppressed()
		{
			return Thread.CurrentThread.ExecutionContext.SecurityContext.WindowsIdentityFlowSuppressed;
		}

		public static void RestoreFlow()
		{
			SecurityContext securityContext = Thread.CurrentThread.ExecutionContext.SecurityContext;
			if (!securityContext.FlowSuppressed && !securityContext.WindowsIdentityFlowSuppressed)
			{
				throw new InvalidOperationException();
			}
			securityContext.FlowSuppressed = false;
			securityContext.WindowsIdentityFlowSuppressed = false;
		}

		public static void Run(SecurityContext securityContext, ContextCallback callback, object state)
		{
			if (securityContext == null)
			{
				throw new InvalidOperationException(Locale.GetText("Null SecurityContext"));
			}
			SecurityContext securityContext2 = Thread.CurrentThread.ExecutionContext.SecurityContext;
			IPrincipal currentPrincipal = Thread.CurrentPrincipal;
			try
			{
				if (securityContext2.IdentityToken != IntPtr.Zero)
				{
					Thread.CurrentPrincipal = new WindowsPrincipal(new WindowsIdentity(securityContext2.IdentityToken));
				}
				if (securityContext.CompressedStack != null)
				{
					CompressedStack.Run(securityContext.CompressedStack, callback, state);
				}
				else
				{
					callback(state);
				}
			}
			finally
			{
				if (currentPrincipal != null && securityContext2.IdentityToken != IntPtr.Zero)
				{
					Thread.CurrentPrincipal = currentPrincipal;
				}
			}
		}

		public static AsyncFlowControl SuppressFlow()
		{
			Thread currentThread = Thread.CurrentThread;
			currentThread.ExecutionContext.SecurityContext.FlowSuppressed = true;
			currentThread.ExecutionContext.SecurityContext.WindowsIdentityFlowSuppressed = true;
			return new AsyncFlowControl(currentThread, AsyncFlowControlType.Security);
		}

		public static AsyncFlowControl SuppressFlowWindowsIdentity()
		{
			Thread currentThread = Thread.CurrentThread;
			currentThread.ExecutionContext.SecurityContext.WindowsIdentityFlowSuppressed = true;
			return new AsyncFlowControl(currentThread, AsyncFlowControlType.Security);
		}
	}
}
