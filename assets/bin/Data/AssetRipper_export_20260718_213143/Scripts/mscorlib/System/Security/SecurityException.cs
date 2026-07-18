using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace System.Security
{
	[Serializable]
	[ComVisible(true)]
	public class SecurityException : SystemException
	{
		private string permissionState;

		private Type permissionType;

		private string _granted;

		private string _refused;

		private object _demanded;

		private IPermission _firstperm;

		private MethodInfo _method;

		private Evidence _evidence;

		private SecurityAction _action;

		private object _denyset;

		private object _permitset;

		private AssemblyName _assembly;

		private string _url;

		private SecurityZone _zone;

		[ComVisible(false)]
		public SecurityAction Action
		{
			get
			{
				return _action;
			}
			set
			{
				_action = value;
			}
		}

		[ComVisible(false)]
		public object DenySetInstance
		{
			get
			{
				return _denyset;
			}
			set
			{
				_denyset = value;
			}
		}

		[ComVisible(false)]
		public AssemblyName FailedAssemblyInfo
		{
			get
			{
				return _assembly;
			}
			set
			{
				_assembly = value;
			}
		}

		[ComVisible(false)]
		public MethodInfo Method
		{
			get
			{
				return _method;
			}
			set
			{
				_method = value;
			}
		}

		[ComVisible(false)]
		public object PermitOnlySetInstance
		{
			get
			{
				return _permitset;
			}
			set
			{
				_permitset = value;
			}
		}

		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}

		public SecurityZone Zone
		{
			get
			{
				return _zone;
			}
			set
			{
				_zone = value;
			}
		}

		[ComVisible(false)]
		public object Demanded
		{
			get
			{
				return _demanded;
			}
			set
			{
				_demanded = value;
			}
		}

		public IPermission FirstPermissionThatFailed
		{
			get
			{
				return _firstperm;
			}
			set
			{
				_firstperm = value;
			}
		}

		public string PermissionState
		{
			get
			{
				return permissionState;
			}
			set
			{
				permissionState = value;
			}
		}

		public Type PermissionType
		{
			get
			{
				return permissionType;
			}
			set
			{
				permissionType = value;
			}
		}

		public string GrantedSet
		{
			get
			{
				return _granted;
			}
			set
			{
				_granted = value;
			}
		}

		public string RefusedSet
		{
			get
			{
				return _refused;
			}
			set
			{
				_refused = value;
			}
		}

		public SecurityException()
			: base(Locale.GetText("A security error has been detected."))
		{
			base.HResult = -2146233078;
		}

		public SecurityException(string message)
			: base(message)
		{
			base.HResult = -2146233078;
		}

		protected SecurityException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			base.HResult = -2146233078;
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Name == "PermissionState")
				{
					permissionState = (string)enumerator.Value;
					break;
				}
			}
		}

		public SecurityException(string message, Exception inner)
			: base(message, inner)
		{
			base.HResult = -2146233078;
		}

		public SecurityException(string message, Type type)
			: base(message)
		{
			base.HResult = -2146233078;
			permissionType = type;
		}

		public SecurityException(string message, Type type, string state)
			: base(message)
		{
			base.HResult = -2146233078;
			permissionType = type;
			permissionState = state;
		}

		internal SecurityException(string message, PermissionSet granted, PermissionSet refused)
			: base(message)
		{
			base.HResult = -2146233078;
			_granted = granted.ToString();
			_refused = refused.ToString();
		}

		public SecurityException(string message, object deny, object permitOnly, MethodInfo method, object demanded, IPermission permThatFailed)
			: base(message)
		{
			base.HResult = -2146233078;
			_denyset = deny;
			_permitset = permitOnly;
			_method = method;
			_demanded = demanded;
			_firstperm = permThatFailed;
		}

		public SecurityException(string message, AssemblyName assemblyName, PermissionSet grant, PermissionSet refused, MethodInfo method, SecurityAction action, object demanded, IPermission permThatFailed, Evidence evidence)
			: base(message)
		{
			base.HResult = -2146233078;
			_assembly = assemblyName;
			_granted = ((grant != null) ? grant.ToString() : string.Empty);
			_refused = ((refused != null) ? refused.ToString() : string.Empty);
			_method = method;
			_action = action;
			_demanded = demanded;
			_firstperm = permThatFailed;
			if (_firstperm != null)
			{
				permissionType = _firstperm.GetType();
			}
			_evidence = evidence;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			try
			{
				info.AddValue("PermissionState", PermissionState);
			}
			catch (SecurityException)
			{
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.ToString());
			try
			{
				if (permissionType != null)
				{
					stringBuilder.AppendFormat("{0}Type: {1}", Environment.NewLine, PermissionType);
				}
				if (_method != null)
				{
					string text = _method.ToString();
					int startIndex = text.IndexOf(" ") + 1;
					stringBuilder.AppendFormat("{0}Method: {1} {2}.{3}", Environment.NewLine, _method.ReturnType.Name, _method.ReflectedType, text.Substring(startIndex));
				}
				if (permissionState != null)
				{
					stringBuilder.AppendFormat("{0}State: {1}", Environment.NewLine, PermissionState);
				}
				if (_granted != null && _granted.Length > 0)
				{
					stringBuilder.AppendFormat("{0}Granted: {1}", Environment.NewLine, GrantedSet);
				}
				if (_refused != null && _refused.Length > 0)
				{
					stringBuilder.AppendFormat("{0}Refused: {1}", Environment.NewLine, RefusedSet);
				}
				if (_demanded != null)
				{
					stringBuilder.AppendFormat("{0}Demanded: {1}", Environment.NewLine, Demanded);
				}
				if (_firstperm != null)
				{
					stringBuilder.AppendFormat("{0}Failed Permission: {1}", Environment.NewLine, FirstPermissionThatFailed);
				}
				if (_evidence != null)
				{
					stringBuilder.AppendFormat("{0}Evidences:", Environment.NewLine);
					foreach (object item in _evidence)
					{
						if (!(item is Hash))
						{
							stringBuilder.AppendFormat("{0}\t{1}", Environment.NewLine, item);
						}
					}
				}
			}
			catch (SecurityException)
			{
			}
			return stringBuilder.ToString();
		}
	}
}
