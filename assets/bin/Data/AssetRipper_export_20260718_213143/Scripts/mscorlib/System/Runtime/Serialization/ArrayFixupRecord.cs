namespace System.Runtime.Serialization
{
	internal class ArrayFixupRecord : BaseFixupRecord
	{
		private int _index;

		public ArrayFixupRecord(ObjectRecord objectToBeFixed, int index, ObjectRecord objectRequired)
			: base(objectToBeFixed, objectRequired)
		{
			_index = index;
		}

		protected override void FixupImpl(ObjectManager manager)
		{
			Array array = (Array)ObjectToBeFixed.ObjectInstance;
			array.SetValue(ObjectRequired.ObjectInstance, _index);
		}
	}
}
