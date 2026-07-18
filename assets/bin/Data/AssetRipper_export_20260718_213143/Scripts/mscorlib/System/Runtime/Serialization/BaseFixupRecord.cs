namespace System.Runtime.Serialization
{
	internal abstract class BaseFixupRecord
	{
		protected internal ObjectRecord ObjectToBeFixed;

		protected internal ObjectRecord ObjectRequired;

		public BaseFixupRecord NextSameContainer;

		public BaseFixupRecord NextSameRequired;

		public BaseFixupRecord(ObjectRecord objectToBeFixed, ObjectRecord objectRequired)
		{
			ObjectToBeFixed = objectToBeFixed;
			ObjectRequired = objectRequired;
		}

		public bool DoFixup(ObjectManager manager, bool strict)
		{
			if (ObjectToBeFixed.IsRegistered && ObjectRequired.IsInstanceReady)
			{
				FixupImpl(manager);
				return true;
			}
			if (strict)
			{
				if (!ObjectToBeFixed.IsRegistered)
				{
					throw new SerializationException("An object with ID " + ObjectToBeFixed.ObjectID + " was included in a fixup, but it has not been registered");
				}
				if (!ObjectRequired.IsRegistered)
				{
					throw new SerializationException("An object with ID " + ObjectRequired.ObjectID + " was included in a fixup, but it has not been registered");
				}
				return false;
			}
			return false;
		}

		protected abstract void FixupImpl(ObjectManager manager);
	}
}
