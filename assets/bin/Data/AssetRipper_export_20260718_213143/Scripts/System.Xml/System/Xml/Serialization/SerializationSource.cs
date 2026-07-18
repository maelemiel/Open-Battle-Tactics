namespace System.Xml.Serialization
{
	internal abstract class SerializationSource
	{
		private Type[] includedTypes;

		private string namspace;

		private bool canBeGenerated = true;

		public virtual bool CanBeGenerated
		{
			get
			{
				return canBeGenerated;
			}
			set
			{
				canBeGenerated = value;
			}
		}

		public SerializationSource(string namspace, Type[] includedTypes)
		{
			this.namspace = namspace;
			this.includedTypes = includedTypes;
		}

		protected bool BaseEquals(SerializationSource other)
		{
			if (namspace != other.namspace)
			{
				return false;
			}
			if (canBeGenerated != other.canBeGenerated)
			{
				return false;
			}
			if (includedTypes == null)
			{
				return other.includedTypes == null;
			}
			if (other.includedTypes == null || includedTypes.Length != other.includedTypes.Length)
			{
				return false;
			}
			for (int i = 0; i < includedTypes.Length; i++)
			{
				if (!includedTypes[i].Equals(other.includedTypes[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
