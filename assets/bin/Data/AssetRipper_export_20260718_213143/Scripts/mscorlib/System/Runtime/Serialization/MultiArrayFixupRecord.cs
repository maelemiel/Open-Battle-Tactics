namespace System.Runtime.Serialization
{
	internal class MultiArrayFixupRecord : BaseFixupRecord
	{
		private int[] _indices;

		public MultiArrayFixupRecord(ObjectRecord objectToBeFixed, int[] indices, ObjectRecord objectRequired)
			: base(objectToBeFixed, objectRequired)
		{
			_indices = indices;
		}

		protected override void FixupImpl(ObjectManager manager)
		{
			ObjectToBeFixed.SetArrayValue(manager, ObjectRequired.ObjectInstance, _indices);
		}
	}
}
