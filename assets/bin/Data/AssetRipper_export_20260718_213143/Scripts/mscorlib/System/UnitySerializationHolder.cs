using System.Reflection;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	internal class UnitySerializationHolder : ISerializable, IObjectReference
	{
		private enum UnityType : byte
		{
			DBNull = 2,
			Type = 4,
			Module = 5,
			Assembly = 6
		}

		private string _data;

		private UnityType _unityType;

		private string _assemblyName;

		private UnitySerializationHolder(SerializationInfo info, StreamingContext ctx)
		{
			_data = info.GetString("Data");
			_unityType = (UnityType)info.GetInt32("UnityType");
			_assemblyName = info.GetString("AssemblyName");
		}

		public static void GetTypeData(Type instance, SerializationInfo info, StreamingContext ctx)
		{
			info.AddValue("Data", instance.FullName);
			info.AddValue("UnityType", 4);
			info.AddValue("AssemblyName", instance.Assembly.FullName);
			info.SetType(typeof(UnitySerializationHolder));
		}

		public static void GetDBNullData(DBNull instance, SerializationInfo info, StreamingContext ctx)
		{
			info.AddValue("Data", null);
			info.AddValue("UnityType", 2);
			info.AddValue("AssemblyName", instance.GetType().Assembly.FullName);
			info.SetType(typeof(UnitySerializationHolder));
		}

		public static void GetAssemblyData(Assembly instance, SerializationInfo info, StreamingContext ctx)
		{
			info.AddValue("Data", instance.FullName);
			info.AddValue("UnityType", 6);
			info.AddValue("AssemblyName", instance.FullName);
			info.SetType(typeof(UnitySerializationHolder));
		}

		public static void GetModuleData(Module instance, SerializationInfo info, StreamingContext ctx)
		{
			info.AddValue("Data", instance.ScopeName);
			info.AddValue("UnityType", 5);
			info.AddValue("AssemblyName", instance.Assembly.FullName);
			info.SetType(typeof(UnitySerializationHolder));
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException();
		}

		public virtual object GetRealObject(StreamingContext context)
		{
			switch (_unityType)
			{
			case UnityType.Type:
			{
				Assembly assembly2 = Assembly.Load(_assemblyName);
				return assembly2.GetType(_data);
			}
			case UnityType.DBNull:
				return DBNull.Value;
			case UnityType.Module:
			{
				Assembly assembly = Assembly.Load(_assemblyName);
				return assembly.GetModule(_data);
			}
			case UnityType.Assembly:
				return Assembly.Load(_data);
			default:
				throw new NotSupportedException(Locale.GetText("UnitySerializationHolder does not support this type."));
			}
		}
	}
}
