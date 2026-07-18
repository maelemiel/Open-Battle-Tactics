using System.IO;
using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public abstract class WebResponse : MarshalByRefObject, IDisposable, ISerializable
	{
		public virtual long ContentLength
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public virtual string ContentType
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public virtual WebHeaderCollection Headers
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		[System.MonoTODO]
		public virtual bool IsFromCache
		{
			get
			{
				throw GetMustImplement();
			}
		}

		[System.MonoTODO]
		public virtual bool IsMutuallyAuthenticated
		{
			get
			{
				throw GetMustImplement();
			}
		}

		public virtual Uri ResponseUri
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		protected WebResponse()
		{
		}

		protected WebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw new NotSupportedException();
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw new NotSupportedException();
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		public virtual void Close()
		{
			throw new NotSupportedException();
		}

		public virtual Stream GetResponseStream()
		{
			throw new NotSupportedException();
		}

		[System.MonoTODO]
		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw GetMustImplement();
		}
	}
}
