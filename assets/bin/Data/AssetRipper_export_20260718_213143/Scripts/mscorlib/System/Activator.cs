using System.Configuration.Assemblies;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Security.Policy;
using System.Text;

namespace System
{
	[ComDefaultInterface(typeof(_Activator))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public sealed class Activator : _Activator
	{
		private const BindingFlags _flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

		private const BindingFlags _accessFlags = BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		private Activator()
		{
		}

		void _Activator.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Activator.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Activator.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Activator.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("No COM support")]
		public static ObjectHandle CreateComInstanceFrom(string assemblyName, string typeName)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (assemblyName.Length == 0)
			{
				throw new ArgumentException("assemblyName");
			}
			throw new NotImplementedException();
		}

		[MonoTODO("Mono does not support COM")]
		public static ObjectHandle CreateComInstanceFrom(string assemblyName, string typeName, byte[] hashValue, AssemblyHashAlgorithm hashAlgorithm)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			if (assemblyName.Length == 0)
			{
				throw new ArgumentException("assemblyName");
			}
			throw new NotImplementedException();
		}

		public static ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName)
		{
			return CreateInstanceFrom(assemblyFile, typeName, null);
		}

		public static ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, object[] activationAttributes)
		{
			return CreateInstanceFrom(assemblyFile, typeName, false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null, activationAttributes, null);
		}

		public static ObjectHandle CreateInstanceFrom(string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityInfo)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyFile, securityInfo);
			if (assembly == null)
			{
				return null;
			}
			Type type = assembly.GetType(typeName, true, ignoreCase);
			if (type == null)
			{
				return null;
			}
			object obj = CreateInstance(type, bindingAttr, binder, args, culture, activationAttributes);
			return (obj == null) ? null : new ObjectHandle(obj);
		}

		public static ObjectHandle CreateInstance(string assemblyName, string typeName)
		{
			if (assemblyName == null)
			{
				assemblyName = Assembly.GetCallingAssembly().GetName().Name;
			}
			return CreateInstance(assemblyName, typeName, null);
		}

		public static ObjectHandle CreateInstance(string assemblyName, string typeName, object[] activationAttributes)
		{
			if (assemblyName == null)
			{
				assemblyName = Assembly.GetCallingAssembly().GetName().Name;
			}
			return CreateInstance(assemblyName, typeName, false, BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null, activationAttributes, null);
		}

		public static ObjectHandle CreateInstance(string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityInfo)
		{
			Assembly assembly = null;
			assembly = ((assemblyName != null) ? Assembly.Load(assemblyName, securityInfo) : Assembly.GetCallingAssembly());
			Type type = assembly.GetType(typeName, true, ignoreCase);
			object obj = CreateInstance(type, bindingAttr, binder, args, culture, activationAttributes);
			return (obj == null) ? null : new ObjectHandle(obj);
		}

		[MonoNotSupported("no ClickOnce in mono")]
		public static ObjectHandle CreateInstance(ActivationContext activationContext)
		{
			throw new NotImplementedException();
		}

		[MonoNotSupported("no ClickOnce in mono")]
		public static ObjectHandle CreateInstance(ActivationContext activationContext, string[] activationCustomData)
		{
			throw new NotImplementedException();
		}

		public static ObjectHandle CreateInstanceFrom(AppDomain domain, string assemblyFile, string typeName)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			return domain.CreateInstanceFrom(assemblyFile, typeName);
		}

		public static ObjectHandle CreateInstanceFrom(AppDomain domain, string assemblyFile, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			return domain.CreateInstanceFrom(assemblyFile, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		public static ObjectHandle CreateInstance(AppDomain domain, string assemblyName, string typeName)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			return domain.CreateInstance(assemblyName, typeName);
		}

		public static ObjectHandle CreateInstance(AppDomain domain, string assemblyName, string typeName, bool ignoreCase, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, Evidence securityAttributes)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			return domain.CreateInstance(assemblyName, typeName, ignoreCase, bindingAttr, binder, args, culture, activationAttributes, securityAttributes);
		}

		public static T CreateInstance<T>()
		{
			return (T)CreateInstance(typeof(T));
		}

		public static object CreateInstance(Type type)
		{
			return CreateInstance(type, false);
		}

		public static object CreateInstance(Type type, params object[] args)
		{
			return CreateInstance(type, args, new object[0]);
		}

		public static object CreateInstance(Type type, object[] args, object[] activationAttributes)
		{
			return CreateInstance(type, BindingFlags.Default, Binder.DefaultBinder, args, null, activationAttributes);
		}

		public static object CreateInstance(Type type, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture)
		{
			return CreateInstance(type, bindingAttr, binder, args, culture, new object[0]);
		}

		public static object CreateInstance(Type type, BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes)
		{
			CheckType(type);
			if (type.ContainsGenericParameters)
			{
				throw new ArgumentException(string.Concat(type, " is an open generic type"), "type");
			}
			if ((bindingAttr & (BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy)) == 0)
			{
				bindingAttr |= BindingFlags.Instance | BindingFlags.Public;
			}
			int num = 0;
			if (args != null)
			{
				num = args.Length;
			}
			Type[] array = ((num != 0) ? new Type[num] : Type.EmptyTypes);
			for (int i = 0; i < num; i++)
			{
				if (args[i] != null)
				{
					array[i] = args[i].GetType();
				}
			}
			if (binder == null)
			{
				binder = Binder.DefaultBinder;
			}
			ConstructorInfo constructorInfo = (ConstructorInfo)binder.SelectMethod(bindingAttr, type.GetConstructors(bindingAttr), array, null);
			if (constructorInfo == null)
			{
				if (type.IsValueType && array.Length == 0)
				{
					return CreateInstanceInternal(type);
				}
				StringBuilder stringBuilder = new StringBuilder();
				Type[] array2 = array;
				foreach (Type type2 in array2)
				{
					stringBuilder.Append((type2 == null) ? "(unknown)" : type2.ToString());
					stringBuilder.Append(", ");
				}
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Length -= 2;
				}
				throw new MissingMethodException(string.Format(Locale.GetText("No constructor found for {0}::.ctor({1})"), type.FullName, stringBuilder));
			}
			CheckAbstractType(type);
			if (activationAttributes != null && activationAttributes.Length > 0)
			{
				if (!type.IsMarshalByRef)
				{
					string text = Locale.GetText("Type '{0}' doesn't derive from MarshalByRefObject.", type.FullName);
					throw new NotSupportedException(text);
				}
				object obj = ActivationServices.CreateProxyFromAttributes(type, activationAttributes);
				if (obj != null)
				{
					constructorInfo.Invoke(obj, bindingAttr, binder, args, culture);
					return obj;
				}
			}
			return constructorInfo.Invoke(bindingAttr, binder, args, culture);
		}

		public static object CreateInstance(Type type, bool nonPublic)
		{
			CheckType(type);
			if (type.ContainsGenericParameters)
			{
				throw new ArgumentException(string.Concat(type, " is an open generic type"), "type");
			}
			CheckAbstractType(type);
			MonoType monoType = type as MonoType;
			ConstructorInfo constructorInfo;
			if (monoType != null)
			{
				constructorInfo = monoType.GetDefaultConstructor();
				if (!nonPublic && constructorInfo != null && !constructorInfo.IsPublic)
				{
					constructorInfo = null;
				}
			}
			else
			{
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
				if (nonPublic)
				{
					bindingFlags |= BindingFlags.NonPublic;
				}
				constructorInfo = type.GetConstructor(bindingFlags, null, CallingConventions.Any, Type.EmptyTypes, null);
			}
			if (constructorInfo == null)
			{
				if (type.IsValueType)
				{
					return CreateInstanceInternal(type);
				}
				throw new MissingMethodException(Locale.GetText("Default constructor not found."), ".ctor() of " + type.FullName);
			}
			return constructorInfo.Invoke(null);
		}

		private static void CheckType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type == typeof(TypedReference) || type == typeof(ArgIterator) || type == typeof(void) || type == typeof(RuntimeArgumentHandle))
			{
				string text = Locale.GetText("CreateInstance cannot be used to create this type ({0}).", type.FullName);
				throw new NotSupportedException(text);
			}
		}

		private static void CheckAbstractType(Type type)
		{
			if (type.IsAbstract)
			{
				string text = Locale.GetText("Cannot create an abstract class '{0}'.", type.FullName);
				throw new MissingMethodException(text);
			}
		}

		public static object GetObject(Type type, string url)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return RemotingServices.Connect(type, url);
		}

		public static object GetObject(Type type, string url, object state)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return RemotingServices.Connect(type, url, state);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object CreateInstanceInternal(Type type);
	}
}
