using System.Reflection;

namespace System.Runtime.Serialization
{
	internal class FixupRecord : BaseFixupRecord
	{
		public MemberInfo _member;

		public FixupRecord(ObjectRecord objectToBeFixed, MemberInfo member, ObjectRecord objectRequired)
			: base(objectToBeFixed, objectRequired)
		{
			_member = member;
		}

		protected override void FixupImpl(ObjectManager manager)
		{
			ObjectToBeFixed.SetMemberValue(manager, _member, ObjectRequired.ObjectInstance);
		}
	}
}
