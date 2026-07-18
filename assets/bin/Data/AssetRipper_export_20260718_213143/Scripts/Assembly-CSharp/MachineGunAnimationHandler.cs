public class MachineGunAnimationHandler : BasicWeaponAnimationHandler
{
	public MachineGunAnimationHandler(EffectType fireEffect, EffectType hitEffect, UnitWeaponType weaponType)
		: base(fireEffect, hitEffect, weaponType)
	{
		screenShake = 0.5f;
		kickbackAmount = 5f;
	}
}
