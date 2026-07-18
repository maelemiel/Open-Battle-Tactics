using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module)]
	public sealed class DebuggableAttribute : Attribute
	{
		[ComVisible(true)]
		[Flags]
		public enum DebuggingModes
		{
			None = 0,
			Default = 1,
			IgnoreSymbolStoreSequencePoints = 2,
			EnableEditAndContinue = 4,
			DisableOptimizations = 0x100
		}

		private bool JITTrackingEnabledFlag;

		private bool JITOptimizerDisabledFlag;

		private DebuggingModes debuggingModes;

		public DebuggingModes DebuggingFlags
		{
			get
			{
				return debuggingModes;
			}
		}

		public bool IsJITTrackingEnabled
		{
			get
			{
				return JITTrackingEnabledFlag;
			}
		}

		public bool IsJITOptimizerDisabled
		{
			get
			{
				return JITOptimizerDisabledFlag;
			}
		}

		public DebuggableAttribute(bool isJITTrackingEnabled, bool isJITOptimizerDisabled)
		{
			JITTrackingEnabledFlag = isJITTrackingEnabled;
			JITOptimizerDisabledFlag = isJITOptimizerDisabled;
			if (isJITTrackingEnabled)
			{
				debuggingModes |= DebuggingModes.Default;
			}
			if (isJITOptimizerDisabled)
			{
				debuggingModes |= DebuggingModes.DisableOptimizations;
			}
		}

		public DebuggableAttribute(DebuggingModes modes)
		{
			debuggingModes = modes;
			JITTrackingEnabledFlag = (debuggingModes & DebuggingModes.Default) != 0;
			JITOptimizerDisabledFlag = (debuggingModes & DebuggingModes.DisableOptimizations) != 0;
		}
	}
}
