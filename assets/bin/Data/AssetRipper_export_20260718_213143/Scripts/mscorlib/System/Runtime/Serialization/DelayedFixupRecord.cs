namespace System.Runtime.Serialization
{
	internal class DelayedFixupRecord : BaseFixupRecord
	{
		public string _memberName;

		public DelayedFixupRecord(ObjectRecord objectToBeFixed, string memberName, ObjectRecord objectRequired)
			: base(objectToBeFixed, objectRequired)
		{
			_memberName = memberName;
		}

		protected override void FixupImpl(ObjectManager manager)
		{
			ObjectToBeFixed.SetMemberValue(manager, _memberName, ObjectRequired.ObjectInstance);
		}
	}
}
