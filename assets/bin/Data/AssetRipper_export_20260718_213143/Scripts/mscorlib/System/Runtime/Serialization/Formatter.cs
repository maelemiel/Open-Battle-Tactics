using System.Collections;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[Serializable]
	[CLSCompliant(false)]
	[ComVisible(true)]
	public abstract class Formatter : IFormatter
	{
		protected ObjectIDGenerator m_idGenerator = new ObjectIDGenerator();

		protected Queue m_objectQueue = new Queue();

		public abstract SerializationBinder Binder { get; set; }

		public abstract StreamingContext Context { get; set; }

		public abstract ISurrogateSelector SurrogateSelector { get; set; }

		public abstract object Deserialize(Stream serializationStream);

		protected virtual object GetNext(out long objID)
		{
			if (m_objectQueue.Count == 0)
			{
				objID = 0L;
				return null;
			}
			object obj = m_objectQueue.Dequeue();
			bool firstTime;
			objID = m_idGenerator.HasId(obj, out firstTime);
			return obj;
		}

		protected virtual long Schedule(object obj)
		{
			if (obj == null)
			{
				return 0L;
			}
			bool firstTime;
			long id = m_idGenerator.GetId(obj, out firstTime);
			if (firstTime)
			{
				m_objectQueue.Enqueue(obj);
			}
			return id;
		}

		public abstract void Serialize(Stream serializationStream, object graph);

		protected abstract void WriteArray(object obj, string name, Type memberType);

		protected abstract void WriteBoolean(bool val, string name);

		protected abstract void WriteByte(byte val, string name);

		protected abstract void WriteChar(char val, string name);

		protected abstract void WriteDateTime(DateTime val, string name);

		protected abstract void WriteDecimal(decimal val, string name);

		protected abstract void WriteDouble(double val, string name);

		protected abstract void WriteInt16(short val, string name);

		protected abstract void WriteInt32(int val, string name);

		protected abstract void WriteInt64(long val, string name);

		protected virtual void WriteMember(string memberName, object data)
		{
			if (data == null)
			{
				WriteObjectRef(data, memberName, typeof(object));
			}
			Type type = data.GetType();
			if (type.IsArray)
			{
				WriteArray(data, memberName, type);
			}
			else if (type == typeof(bool))
			{
				WriteBoolean((bool)data, memberName);
			}
			else if (type == typeof(byte))
			{
				WriteByte((byte)data, memberName);
			}
			else if (type == typeof(char))
			{
				WriteChar((char)data, memberName);
			}
			else if (type == typeof(DateTime))
			{
				WriteDateTime((DateTime)data, memberName);
			}
			else if (type == typeof(decimal))
			{
				WriteDecimal((decimal)data, memberName);
			}
			else if (type == typeof(double))
			{
				WriteDouble((double)data, memberName);
			}
			else if (type == typeof(short))
			{
				WriteInt16((short)data, memberName);
			}
			else if (type == typeof(int))
			{
				WriteInt32((int)data, memberName);
			}
			else if (type == typeof(long))
			{
				WriteInt64((long)data, memberName);
			}
			else if (type == typeof(sbyte))
			{
				WriteSByte((sbyte)data, memberName);
			}
			else if (type == typeof(float))
			{
				WriteSingle((float)data, memberName);
			}
			else if (type == typeof(TimeSpan))
			{
				WriteTimeSpan((TimeSpan)data, memberName);
			}
			else if (type == typeof(ushort))
			{
				WriteUInt16((ushort)data, memberName);
			}
			else if (type == typeof(uint))
			{
				WriteUInt32((uint)data, memberName);
			}
			else if (type == typeof(ulong))
			{
				WriteUInt64((ulong)data, memberName);
			}
			else if (type.IsValueType)
			{
				WriteValueType(data, memberName, type);
			}
			WriteObjectRef(data, memberName, type);
		}

		protected abstract void WriteObjectRef(object obj, string name, Type memberType);

		[CLSCompliant(false)]
		protected abstract void WriteSByte(sbyte val, string name);

		protected abstract void WriteSingle(float val, string name);

		protected abstract void WriteTimeSpan(TimeSpan val, string name);

		[CLSCompliant(false)]
		protected abstract void WriteUInt16(ushort val, string name);

		[CLSCompliant(false)]
		protected abstract void WriteUInt32(uint val, string name);

		[CLSCompliant(false)]
		protected abstract void WriteUInt64(ulong val, string name);

		protected abstract void WriteValueType(object obj, string name, Type memberType);
	}
}
