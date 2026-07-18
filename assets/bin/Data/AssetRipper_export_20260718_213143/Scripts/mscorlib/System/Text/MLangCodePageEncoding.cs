using System.Runtime.Serialization;

namespace System.Text
{
	[Serializable]
	internal sealed class MLangCodePageEncoding : ISerializable, IObjectReference
	{
		[Serializable]
		private sealed class MLangEncoder : ISerializable, IObjectReference
		{
			private Encoding encoding;

			private Encoder realObject;

			private MLangEncoder(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				encoding = (Encoding)info.GetValue("m_encoding", typeof(Encoding));
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new ArgumentException("This class cannot be serialized.");
			}

			public object GetRealObject(StreamingContext context)
			{
				if (realObject == null)
				{
					realObject = encoding.GetEncoder();
				}
				return realObject;
			}
		}

		[Serializable]
		private sealed class MLangDecoder : ISerializable, IObjectReference
		{
			private Encoding encoding;

			private Decoder realObject;

			private MLangDecoder(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				encoding = (Encoding)info.GetValue("m_encoding", typeof(Encoding));
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new ArgumentException("This class cannot be serialized.");
			}

			public object GetRealObject(StreamingContext context)
			{
				if (realObject == null)
				{
					realObject = encoding.GetDecoder();
				}
				return realObject;
			}
		}

		private int codePage;

		private bool isReadOnly;

		private EncoderFallback encoderFallback;

		private DecoderFallback decoderFallback;

		private Encoding realObject;

		private MLangCodePageEncoding(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			codePage = (int)info.GetValue("m_codePage", typeof(int));
			try
			{
				isReadOnly = (bool)info.GetValue("m_isReadOnly", typeof(bool));
				encoderFallback = (EncoderFallback)info.GetValue("encoderFallback", typeof(EncoderFallback));
				decoderFallback = (DecoderFallback)info.GetValue("decoderFallback", typeof(DecoderFallback));
			}
			catch (SerializationException)
			{
				isReadOnly = true;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new ArgumentException("This class cannot be serialized.");
		}

		public object GetRealObject(StreamingContext context)
		{
			if (realObject == null)
			{
				Encoding encoding = Encoding.GetEncoding(codePage);
				if (!isReadOnly)
				{
					encoding = (Encoding)encoding.Clone();
					encoding.EncoderFallback = encoderFallback;
					encoding.DecoderFallback = decoderFallback;
				}
				realObject = encoding;
			}
			return realObject;
		}
	}
}
