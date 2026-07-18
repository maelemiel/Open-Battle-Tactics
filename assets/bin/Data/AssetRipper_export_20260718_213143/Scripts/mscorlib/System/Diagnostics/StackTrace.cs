using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.Diagnostics
{
	[Serializable]
	[MonoTODO("Serialized objects are not compatible with .NET")]
	[ComVisible(true)]
	public class StackTrace
	{
		public const int METHODS_TO_SKIP = 0;

		private StackFrame[] frames;

		private bool debug_info;

		public virtual int FrameCount
		{
			get
			{
				return (frames != null) ? frames.Length : 0;
			}
		}

		public StackTrace()
		{
			init_frames(0, false);
		}

		public StackTrace(bool fNeedFileInfo)
		{
			init_frames(0, fNeedFileInfo);
		}

		public StackTrace(int skipFrames)
		{
			init_frames(skipFrames, false);
		}

		public StackTrace(int skipFrames, bool fNeedFileInfo)
		{
			init_frames(skipFrames, fNeedFileInfo);
		}

		public StackTrace(Exception e)
			: this(e, 0, false)
		{
		}

		public StackTrace(Exception e, bool fNeedFileInfo)
			: this(e, 0, fNeedFileInfo)
		{
		}

		public StackTrace(Exception e, int skipFrames)
			: this(e, skipFrames, false)
		{
		}

		public StackTrace(Exception e, int skipFrames, bool fNeedFileInfo)
			: this(e, skipFrames, fNeedFileInfo, false)
		{
		}

		internal StackTrace(Exception e, int skipFrames, bool fNeedFileInfo, bool returnNativeFrames)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			if (skipFrames < 0)
			{
				throw new ArgumentOutOfRangeException("< 0", "skipFrames");
			}
			frames = get_trace(e, skipFrames, fNeedFileInfo);
			if (returnNativeFrames)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < frames.Length; i++)
			{
				if (frames[i].GetMethod() == null)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			ArrayList arrayList = new ArrayList();
			for (int j = 0; j < frames.Length; j++)
			{
				if (frames[j].GetMethod() != null)
				{
					arrayList.Add(frames[j]);
				}
			}
			frames = (StackFrame[])arrayList.ToArray(typeof(StackFrame));
		}

		public StackTrace(StackFrame frame)
		{
			frames = new StackFrame[1];
			frames[0] = frame;
		}

		[MonoTODO("Not possible to create StackTraces from other threads")]
		public StackTrace(Thread targetThread, bool needFileInfo)
		{
			throw new NotImplementedException();
		}

		private void init_frames(int skipFrames, bool fNeedFileInfo)
		{
			if (skipFrames < 0)
			{
				throw new ArgumentOutOfRangeException("< 0", "skipFrames");
			}
			ArrayList arrayList = new ArrayList();
			skipFrames += 2;
			StackFrame stackFrame;
			while ((stackFrame = new StackFrame(skipFrames, fNeedFileInfo)) != null && stackFrame.GetMethod() != null)
			{
				arrayList.Add(stackFrame);
				skipFrames++;
			}
			debug_info = fNeedFileInfo;
			frames = (StackFrame[])arrayList.ToArray(typeof(StackFrame));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern StackFrame[] get_trace(Exception e, int skipFrames, bool fNeedFileInfo);

		public virtual StackFrame GetFrame(int index)
		{
			if (index < 0 || index >= FrameCount)
			{
				return null;
			}
			return frames[index];
		}

		[ComVisible(false)]
		public virtual StackFrame[] GetFrames()
		{
			return frames;
		}

		public override string ToString()
		{
			string value = string.Format("{0}   {1} ", Environment.NewLine, Locale.GetText("at"));
			string text = Locale.GetText("<unknown method>");
			string text2 = Locale.GetText(" in {0}:line {1}");
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < FrameCount; i++)
			{
				StackFrame frame = GetFrame(i);
				if (i > 0)
				{
					stringBuilder.Append(value);
				}
				else
				{
					stringBuilder.AppendFormat("   {0} ", Locale.GetText("at"));
				}
				MethodBase method = frame.GetMethod();
				if (method != null)
				{
					stringBuilder.AppendFormat("{0}.{1}", method.DeclaringType.FullName, method.Name);
					stringBuilder.Append("(");
					ParameterInfo[] parameters = method.GetParameters();
					for (int j = 0; j < parameters.Length; j++)
					{
						if (j > 0)
						{
							stringBuilder.Append(", ");
						}
						Type type = parameters[j].ParameterType;
						bool isByRef = type.IsByRef;
						if (isByRef)
						{
							type = type.GetElementType();
						}
						if (type.IsClass && type.Namespace != string.Empty)
						{
							stringBuilder.Append(type.Namespace);
							stringBuilder.Append(".");
						}
						stringBuilder.Append(type.Name);
						if (isByRef)
						{
							stringBuilder.Append(" ByRef");
						}
						stringBuilder.AppendFormat(" {0}", parameters[j].Name);
					}
					stringBuilder.Append(")");
				}
				else
				{
					stringBuilder.Append(text);
				}
				if (debug_info)
				{
					string secureFileName = frame.GetSecureFileName();
					if (secureFileName != "<filename unknown>")
					{
						stringBuilder.AppendFormat(text2, secureFileName, frame.GetFileLineNumber());
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
