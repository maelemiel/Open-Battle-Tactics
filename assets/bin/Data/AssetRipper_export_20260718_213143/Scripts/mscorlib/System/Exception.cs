using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System
{
	[Serializable]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_Exception))]
	[ComVisible(true)]
	public class Exception : ISerializable, _Exception
	{
		private IntPtr[] trace_ips;

		private Exception inner_exception;

		internal string message;

		private string help_link;

		private string class_name;

		private string stack_trace;

		private string _remoteStackTraceString;

		private int remote_stack_index;

		internal int hresult = -2146233088;

		private string source;

		private IDictionary _data;

		public Exception InnerException
		{
			get
			{
				return inner_exception;
			}
		}

		public virtual string HelpLink
		{
			get
			{
				return help_link;
			}
			set
			{
				help_link = value;
			}
		}

		protected int HResult
		{
			get
			{
				return hresult;
			}
			set
			{
				hresult = value;
			}
		}

		private string ClassName
		{
			get
			{
				if (class_name == null)
				{
					class_name = GetType().ToString();
				}
				return class_name;
			}
		}

		public virtual string Message
		{
			get
			{
				if (message == null)
				{
					message = string.Format(Locale.GetText("Exception of type '{0}' was thrown."), ClassName);
				}
				return message;
			}
		}

		public virtual string Source
		{
			get
			{
				if (source == null)
				{
					StackTrace stackTrace = new StackTrace(this, true);
					if (stackTrace.FrameCount > 0)
					{
						StackFrame frame = stackTrace.GetFrame(0);
						if (stackTrace != null)
						{
							MethodBase method = frame.GetMethod();
							if (method != null)
							{
								source = method.DeclaringType.Assembly.UnprotectedGetName().Name;
							}
						}
					}
				}
				return source;
			}
			set
			{
				source = value;
			}
		}

		public virtual string StackTrace
		{
			get
			{
				if (stack_trace == null)
				{
					if (trace_ips == null)
					{
						return null;
					}
					StackTrace stackTrace = new StackTrace(this, 0, true, true);
					StringBuilder stringBuilder = new StringBuilder();
					string value = string.Format("{0}  {1} ", Environment.NewLine, Locale.GetText("at"));
					string text = Locale.GetText("<unknown method>");
					for (int i = 0; i < stackTrace.FrameCount; i++)
					{
						StackFrame frame = stackTrace.GetFrame(i);
						if (i == 0)
						{
							stringBuilder.AppendFormat("  {0} ", Locale.GetText("at"));
						}
						else
						{
							stringBuilder.Append(value);
						}
						if (frame.GetMethod() == null)
						{
							string internalMethodName = frame.GetInternalMethodName();
							if (internalMethodName != null)
							{
								stringBuilder.Append(internalMethodName);
							}
							else
							{
								stringBuilder.AppendFormat("<0x{0:x5}> {1}", frame.GetNativeOffset(), text);
							}
							continue;
						}
						GetFullNameForStackTrace(stringBuilder, frame.GetMethod());
						if (frame.GetILOffset() == -1)
						{
							stringBuilder.AppendFormat(" <0x{0:x5}> ", frame.GetNativeOffset());
						}
						else
						{
							stringBuilder.AppendFormat(" [0x{0:x5}] ", frame.GetILOffset());
						}
						stringBuilder.AppendFormat("in {0}:{1} ", frame.GetSecureFileName(), frame.GetFileLineNumber());
					}
					stack_trace = stringBuilder.ToString();
				}
				return stack_trace;
			}
		}

		public MethodBase TargetSite
		{
			get
			{
				StackTrace stackTrace = new StackTrace(this, true);
				if (stackTrace.FrameCount > 0)
				{
					return stackTrace.GetFrame(0).GetMethod();
				}
				return null;
			}
		}

		public virtual IDictionary Data
		{
			get
			{
				if (_data == null)
				{
					_data = new Hashtable();
				}
				return _data;
			}
		}

		public Exception()
		{
		}

		public Exception(string message)
		{
			this.message = message;
		}

		protected Exception(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			class_name = info.GetString("ClassName");
			message = info.GetString("Message");
			help_link = info.GetString("HelpURL");
			stack_trace = info.GetString("StackTraceString");
			_remoteStackTraceString = info.GetString("RemoteStackTraceString");
			remote_stack_index = info.GetInt32("RemoteStackIndex");
			hresult = info.GetInt32("HResult");
			source = info.GetString("Source");
			inner_exception = (Exception)info.GetValue("InnerException", typeof(Exception));
			try
			{
				_data = (IDictionary)info.GetValue("Data", typeof(IDictionary));
			}
			catch (SerializationException)
			{
			}
		}

		public Exception(string message, Exception innerException)
		{
			inner_exception = innerException;
			this.message = message;
		}

		internal void SetMessage(string s)
		{
			message = s;
		}

		internal void SetStackTrace(string s)
		{
			stack_trace = s;
		}

		public virtual Exception GetBaseException()
		{
			Exception innerException = inner_exception;
			while (innerException != null)
			{
				if (innerException.InnerException != null)
				{
					innerException = innerException.InnerException;
					continue;
				}
				return innerException;
			}
			return this;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("ClassName", ClassName);
			info.AddValue("Message", message);
			info.AddValue("InnerException", inner_exception);
			info.AddValue("HelpURL", help_link);
			info.AddValue("StackTraceString", StackTrace);
			info.AddValue("RemoteStackTraceString", _remoteStackTraceString);
			info.AddValue("RemoteStackIndex", remote_stack_index);
			info.AddValue("HResult", hresult);
			info.AddValue("Source", Source);
			info.AddValue("ExceptionMethod", null);
			info.AddValue("Data", _data, typeof(IDictionary));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(ClassName);
			stringBuilder.Append(": ").Append(Message);
			if (_remoteStackTraceString != null)
			{
				stringBuilder.Append(_remoteStackTraceString);
			}
			if (inner_exception != null)
			{
				stringBuilder.Append(" ---> ").Append(inner_exception.ToString());
				stringBuilder.Append(Environment.NewLine);
				stringBuilder.Append(Locale.GetText("  --- End of inner exception stack trace ---"));
			}
			if (StackTrace != null)
			{
				stringBuilder.Append(Environment.NewLine).Append(StackTrace);
			}
			return stringBuilder.ToString();
		}

		internal Exception FixRemotingException()
		{
			string format = ((remote_stack_index != 0) ? Locale.GetText("{1}{0}{0}Exception rethrown at [{2}]: {0}") : Locale.GetText("{0}{0}Server stack trace: {0}{1}{0}{0}Exception rethrown at [{2}]: {0}"));
			string remoteStackTraceString = string.Format(format, Environment.NewLine, StackTrace, remote_stack_index);
			_remoteStackTraceString = remoteStackTraceString;
			remote_stack_index++;
			stack_trace = null;
			return this;
		}

		internal void GetFullNameForStackTrace(StringBuilder sb, MethodBase mi)
		{
			ParameterInfo[] parameters = mi.GetParameters();
			sb.Append(mi.DeclaringType.ToString());
			sb.Append(".");
			sb.Append(mi.Name);
			if (mi.IsGenericMethod)
			{
				Type[] genericArguments = mi.GetGenericArguments();
				sb.Append("[");
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (i > 0)
					{
						sb.Append(",");
					}
					sb.Append(genericArguments[i].Name);
				}
				sb.Append("]");
			}
			sb.Append(" (");
			for (int j = 0; j < parameters.Length; j++)
			{
				if (j > 0)
				{
					sb.Append(", ");
				}
				Type parameterType = parameters[j].ParameterType;
				if (parameterType.IsClass && parameterType.Namespace != string.Empty)
				{
					sb.Append(parameterType.Namespace);
					sb.Append(".");
				}
				sb.Append(parameterType.Name);
				if (parameters[j].Name != null)
				{
					sb.Append(" ");
					sb.Append(parameters[j].Name);
				}
			}
			sb.Append(")");
		}

		public new Type GetType()
		{
			return base.GetType();
		}
	}
}
