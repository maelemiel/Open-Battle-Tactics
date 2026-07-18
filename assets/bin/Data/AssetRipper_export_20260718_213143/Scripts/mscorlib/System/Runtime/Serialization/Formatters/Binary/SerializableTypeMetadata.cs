using System.IO;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal class SerializableTypeMetadata : TypeMetadata
	{
		private Type[] types;

		private string[] names;

		public override bool RequiresTypes
		{
			get
			{
				return true;
			}
		}

		public SerializableTypeMetadata(Type itype, SerializationInfo info)
		{
			types = new Type[info.MemberCount];
			names = new string[info.MemberCount];
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				types[num] = enumerator.ObjectType;
				names[num] = enumerator.Name;
				num++;
			}
			TypeAssemblyName = info.AssemblyName;
			InstanceTypeName = info.FullTypeName;
		}

		public override bool IsCompatible(TypeMetadata other)
		{
			if (!(other is SerializableTypeMetadata))
			{
				return false;
			}
			SerializableTypeMetadata serializableTypeMetadata = (SerializableTypeMetadata)other;
			if (types.Length != serializableTypeMetadata.types.Length)
			{
				return false;
			}
			if (TypeAssemblyName != serializableTypeMetadata.TypeAssemblyName)
			{
				return false;
			}
			if (InstanceTypeName != serializableTypeMetadata.InstanceTypeName)
			{
				return false;
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] != serializableTypeMetadata.types[i])
				{
					return false;
				}
				if (names[i] != serializableTypeMetadata.names[i])
				{
					return false;
				}
			}
			return true;
		}

		public override void WriteAssemblies(ObjectWriter ow, BinaryWriter writer)
		{
			Type[] array = types;
			foreach (Type type in array)
			{
				Type type2 = type;
				while (type2.IsArray)
				{
					type2 = type2.GetElementType();
				}
				ow.WriteAssembly(writer, type2.Assembly);
			}
		}

		public override void WriteTypeData(ObjectWriter ow, BinaryWriter writer, bool writeTypes)
		{
			writer.Write(types.Length);
			string[] array = names;
			foreach (string value in array)
			{
				writer.Write(value);
			}
			Type[] array2 = types;
			foreach (Type type in array2)
			{
				ObjectWriter.WriteTypeCode(writer, type);
			}
			Type[] array3 = types;
			foreach (Type type2 in array3)
			{
				ow.WriteTypeSpec(writer, type2);
			}
		}

		public override void WriteObjectData(ObjectWriter ow, BinaryWriter writer, object data)
		{
			SerializationInfo serializationInfo = (SerializationInfo)data;
			SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ow.WriteValue(writer, enumerator.ObjectType, enumerator.Value);
			}
		}
	}
}
