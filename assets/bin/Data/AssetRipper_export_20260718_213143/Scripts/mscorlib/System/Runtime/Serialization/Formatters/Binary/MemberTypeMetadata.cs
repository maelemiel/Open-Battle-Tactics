using System.IO;
using System.Reflection;

namespace System.Runtime.Serialization.Formatters.Binary
{
	internal class MemberTypeMetadata : ClrTypeMetadata
	{
		private MemberInfo[] members;

		public MemberTypeMetadata(Type type, StreamingContext context)
			: base(type)
		{
			members = FormatterServices.GetSerializableMembers(type, context);
		}

		public override void WriteAssemblies(ObjectWriter ow, BinaryWriter writer)
		{
			MemberInfo[] array = members;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = (FieldInfo)array[i];
				Type type = fieldInfo.FieldType;
				while (type.IsArray)
				{
					type = type.GetElementType();
				}
				ow.WriteAssembly(writer, type.Assembly);
			}
		}

		public override void WriteTypeData(ObjectWriter ow, BinaryWriter writer, bool writeTypes)
		{
			writer.Write(members.Length);
			MemberInfo[] array = members;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo fieldInfo = (FieldInfo)array[i];
				writer.Write(fieldInfo.Name);
			}
			if (writeTypes)
			{
				MemberInfo[] array2 = members;
				for (int j = 0; j < array2.Length; j++)
				{
					FieldInfo fieldInfo2 = (FieldInfo)array2[j];
					ObjectWriter.WriteTypeCode(writer, fieldInfo2.FieldType);
				}
				MemberInfo[] array3 = members;
				for (int k = 0; k < array3.Length; k++)
				{
					FieldInfo fieldInfo3 = (FieldInfo)array3[k];
					ow.WriteTypeSpec(writer, fieldInfo3.FieldType);
				}
			}
		}

		public override void WriteObjectData(ObjectWriter ow, BinaryWriter writer, object data)
		{
			object[] objectData = FormatterServices.GetObjectData(data, members);
			for (int i = 0; i < objectData.Length; i++)
			{
				ow.WriteValue(writer, ((FieldInfo)members[i]).FieldType, objectData[i]);
			}
		}
	}
}
