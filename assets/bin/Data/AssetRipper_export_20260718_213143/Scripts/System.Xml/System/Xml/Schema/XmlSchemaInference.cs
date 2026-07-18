namespace System.Xml.Schema
{
	[System.MonoTODO]
	public sealed class XmlSchemaInference
	{
		public enum InferenceOption
		{
			Restricted = 0,
			Relaxed = 1
		}

		private InferenceOption occurrence;

		private InferenceOption typeInference;

		public InferenceOption Occurrence
		{
			get
			{
				return occurrence;
			}
			set
			{
				occurrence = value;
			}
		}

		public InferenceOption TypeInference
		{
			get
			{
				return typeInference;
			}
			set
			{
				typeInference = value;
			}
		}

		public XmlSchemaSet InferSchema(XmlReader xmlReader)
		{
			return InferSchema(xmlReader, new XmlSchemaSet());
		}

		public XmlSchemaSet InferSchema(XmlReader xmlReader, XmlSchemaSet schemas)
		{
			return XsdInference.Process(xmlReader, schemas, occurrence == InferenceOption.Relaxed, typeInference == InferenceOption.Relaxed);
		}
	}
}
