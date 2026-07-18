using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[ComVisible(true)]
	public class TrustManagerContext
	{
		private bool _ignorePersistedDecision;

		private bool _noPrompt;

		private bool _keepAlive;

		private bool _persist;

		private ApplicationIdentity _previousId;

		private TrustManagerUIContext _ui;

		public virtual bool IgnorePersistedDecision
		{
			get
			{
				return _ignorePersistedDecision;
			}
			set
			{
				_ignorePersistedDecision = value;
			}
		}

		public virtual bool KeepAlive
		{
			get
			{
				return _keepAlive;
			}
			set
			{
				_keepAlive = value;
			}
		}

		public virtual bool NoPrompt
		{
			get
			{
				return _noPrompt;
			}
			set
			{
				_noPrompt = value;
			}
		}

		public virtual bool Persist
		{
			get
			{
				return _persist;
			}
			set
			{
				_persist = value;
			}
		}

		public virtual ApplicationIdentity PreviousApplicationIdentity
		{
			get
			{
				return _previousId;
			}
			set
			{
				_previousId = value;
			}
		}

		public virtual TrustManagerUIContext UIContext
		{
			get
			{
				return _ui;
			}
			set
			{
				_ui = value;
			}
		}

		public TrustManagerContext()
			: this(TrustManagerUIContext.Run)
		{
		}

		public TrustManagerContext(TrustManagerUIContext uiContext)
		{
			_ignorePersistedDecision = false;
			_noPrompt = false;
			_keepAlive = false;
			_persist = false;
			_ui = uiContext;
		}
	}
}
