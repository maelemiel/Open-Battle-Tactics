using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
	[ComVisible(true)]
	public sealed class DebuggerVisualizerAttribute : Attribute
	{
		private string description;

		private string visualizerSourceName;

		private string visualizerName;

		private string targetTypeName;

		private Type target;

		public string Description
		{
			get
			{
				return description;
			}
			set
			{
				description = value;
			}
		}

		public Type Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
				targetTypeName = target.AssemblyQualifiedName;
			}
		}

		public string TargetTypeName
		{
			get
			{
				return targetTypeName;
			}
			set
			{
				targetTypeName = value;
			}
		}

		public string VisualizerObjectSourceTypeName
		{
			get
			{
				return visualizerSourceName;
			}
		}

		public string VisualizerTypeName
		{
			get
			{
				return visualizerName;
			}
		}

		public DebuggerVisualizerAttribute(string visualizerTypeName)
		{
			visualizerName = visualizerTypeName;
		}

		public DebuggerVisualizerAttribute(Type visualizer)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			visualizerName = visualizer.AssemblyQualifiedName;
		}

		public DebuggerVisualizerAttribute(string visualizerTypeName, string visualizerObjectSourceTypeName)
		{
			visualizerName = visualizerTypeName;
			visualizerSourceName = visualizerObjectSourceTypeName;
		}

		public DebuggerVisualizerAttribute(string visualizerTypeName, Type visualizerObjectSource)
		{
			if (visualizerObjectSource == null)
			{
				throw new ArgumentNullException("visualizerObjectSource");
			}
			visualizerName = visualizerTypeName;
			visualizerSourceName = visualizerObjectSource.AssemblyQualifiedName;
		}

		public DebuggerVisualizerAttribute(Type visualizer, string visualizerObjectSourceTypeName)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			visualizerName = visualizer.AssemblyQualifiedName;
			visualizerSourceName = visualizerObjectSourceTypeName;
		}

		public DebuggerVisualizerAttribute(Type visualizer, Type visualizerObjectSource)
		{
			if (visualizer == null)
			{
				throw new ArgumentNullException("visualizer");
			}
			if (visualizerObjectSource == null)
			{
				throw new ArgumentNullException("visualizerObjectSource");
			}
			visualizerName = visualizer.AssemblyQualifiedName;
			visualizerSourceName = visualizerObjectSource.AssemblyQualifiedName;
		}
	}
}
