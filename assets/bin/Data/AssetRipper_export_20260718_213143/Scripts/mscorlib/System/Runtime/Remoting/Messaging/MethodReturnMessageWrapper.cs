using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class MethodReturnMessageWrapper : InternalMessageWrapper, IMessage, IMethodMessage, IMethodReturnMessage
	{
		private class DictionaryWrapper : MethodReturnDictionary
		{
			private IDictionary _wrappedDictionary;

			private static string[] _keys = new string[2] { "__Args", "__Return" };

			public DictionaryWrapper(IMethodReturnMessage message, IDictionary wrappedDictionary)
				: base(message)
			{
				_wrappedDictionary = wrappedDictionary;
				base.MethodKeys = _keys;
			}

			protected override IDictionary AllocInternalProperties()
			{
				return _wrappedDictionary;
			}

			protected override void SetMethodProperty(string key, object value)
			{
				if (key == "__Args")
				{
					((MethodReturnMessageWrapper)_message)._args = (object[])value;
				}
				else if (key == "__Return")
				{
					((MethodReturnMessageWrapper)_message)._return = value;
				}
				else
				{
					base.SetMethodProperty(key, value);
				}
			}

			protected override object GetMethodProperty(string key)
			{
				if (key == "__Args")
				{
					return ((MethodReturnMessageWrapper)_message)._args;
				}
				if (key == "__Return")
				{
					return ((MethodReturnMessageWrapper)_message)._return;
				}
				return base.GetMethodProperty(key);
			}
		}

		private object[] _args;

		private ArgInfo _outArgInfo;

		private DictionaryWrapper _properties;

		private Exception _exception;

		private object _return;

		public virtual int ArgCount
		{
			get
			{
				return _args.Length;
			}
		}

		public virtual object[] Args
		{
			get
			{
				return _args;
			}
			set
			{
				_args = value;
			}
		}

		public virtual Exception Exception
		{
			get
			{
				return _exception;
			}
			set
			{
				_exception = value;
			}
		}

		public virtual bool HasVarArgs
		{
			get
			{
				return ((IMethodReturnMessage)WrappedMessage).HasVarArgs;
			}
		}

		public virtual LogicalCallContext LogicalCallContext
		{
			get
			{
				return ((IMethodReturnMessage)WrappedMessage).LogicalCallContext;
			}
		}

		public virtual MethodBase MethodBase
		{
			get
			{
				return ((IMethodReturnMessage)WrappedMessage).MethodBase;
			}
		}

		public virtual string MethodName
		{
			get
			{
				return ((IMethodReturnMessage)WrappedMessage).MethodName;
			}
		}

		public virtual object MethodSignature
		{
			get
			{
				return ((IMethodReturnMessage)WrappedMessage).MethodSignature;
			}
		}

		public virtual int OutArgCount
		{
			get
			{
				return (_outArgInfo != null) ? _outArgInfo.GetInOutArgCount() : 0;
			}
		}

		public virtual object[] OutArgs
		{
			get
			{
				return (_outArgInfo == null) ? _args : _outArgInfo.GetInOutArgs(_args);
			}
		}

		public virtual IDictionary Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = new DictionaryWrapper(this, WrappedMessage.Properties);
				}
				return _properties;
			}
		}

		public virtual object ReturnValue
		{
			get
			{
				return _return;
			}
			set
			{
				_return = value;
			}
		}

		public virtual string TypeName
		{
			get
			{
				return ((IMethodReturnMessage)WrappedMessage).TypeName;
			}
		}

		public string Uri
		{
			get
			{
				return ((IMethodReturnMessage)WrappedMessage).Uri;
			}
			set
			{
				Properties["__Uri"] = value;
			}
		}

		public MethodReturnMessageWrapper(IMethodReturnMessage msg)
			: base(msg)
		{
			if (msg.Exception != null)
			{
				_exception = msg.Exception;
				_args = new object[0];
				return;
			}
			_args = msg.Args;
			_return = msg.ReturnValue;
			if (msg.MethodBase != null)
			{
				_outArgInfo = new ArgInfo(msg.MethodBase, ArgInfoType.Out);
			}
		}

		public virtual object GetArg(int argNum)
		{
			return _args[argNum];
		}

		public virtual string GetArgName(int index)
		{
			return ((IMethodReturnMessage)WrappedMessage).GetArgName(index);
		}

		public virtual object GetOutArg(int argNum)
		{
			return _args[_outArgInfo.GetInOutArgIndex(argNum)];
		}

		public virtual string GetOutArgName(int index)
		{
			return _outArgInfo.GetInOutArgName(index);
		}
	}
}
