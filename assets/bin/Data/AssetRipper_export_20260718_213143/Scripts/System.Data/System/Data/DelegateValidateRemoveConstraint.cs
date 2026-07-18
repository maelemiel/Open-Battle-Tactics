using System.ComponentModel;

namespace System.Data
{
	[Serializable]
	[Editor]
	internal delegate void DelegateValidateRemoveConstraint(ConstraintCollection sender, Constraint constraintToRemove, ref bool fail, ref string failReason);
}
