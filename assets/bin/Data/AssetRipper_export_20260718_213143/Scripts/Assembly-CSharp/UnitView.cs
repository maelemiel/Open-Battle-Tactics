using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;

public class UnitView : MonoBehaviour
{
	public delegate void Handler();

	private const string EXPLOSION_ANIMATION_NAME = "Regular Damage Hit";

	private const string LINGERING_EFFECT_ANIMATION_NAME = "Electricity Effect ";

	private const float MAX_ARMOR_ALPHA_VALUE = 8f;

	public const string PASSIVE_EFFECT_INITIATIVE = "Initiative Passive Effect";

	public const string PASSIVE_EFFECT_MANIPULATION = "Manipulation Passive";

	public const string PASSIVE_EFFECT_REWARD = "Reward Passive";

	[SerializeField]
	private tk2dSpineAnimation _upgrade;

	[SerializeField]
	private tk2dSpineAnimation _smoke;

	[SerializeField]
	private tk2dSprite _damageDecal;

	[SerializeField]
	private ExplosionAnimation _explosionAnimation;

	[SerializeField]
	private HealthUi _healthUi;

	[SerializeField]
	private ChartBarView rarityStars;

	[SerializeField]
	private List<UnitAnimationLayer> animationLayers;

	[SerializeField]
	private List<DieFaceSimple> dieFaces;

	[SerializeField]
	private UnitWeaponSystem weaponSystem;

	[SerializeField]
	private GameObject shadow;

	[SerializeField]
	private GameObject dustEffect;

	[SerializeField]
	private Transform tankSpritesTransform;

	[SerializeField]
	private UnitPossibleRollsSimple _unitPossibleRollsSimple;

	[SerializeField]
	private UnitProxy unitProxy;

	[SerializeField]
	private UnitAbilityText abilityText;

	[SerializeField]
	private UnitBuffEffects buffEffects;

	[SerializeField]
	private UnitIcon unitIcon;

	[SerializeField]
	private Vector3 shadowOffset;

	[SerializeField]
	private Vector3 dustOffset;

	private BattleController battleController;

	private UnitDamageTint _unitDamageTint;

	private tk2dSprite _sprite;

	private ShieldEffect shieldView;

	private List<EffectInstance> burnEffects;

	private List<EffectInstance> acidEffects;

	private EffectInstance preventRerollEffectInstance;

	private Tweener ambientMovementTween;

	private bool deactivatingShield;

	private bool rolledDice;

	private int localHealth;

	private int localArmor;

	private int localDamagePerRound;

	private int localAcidPerRound;

	private int localExtraDamage;

	private bool localPreventReroll;

	private int localRoundsUntilRerollEnabled;

	private int localCoinIncrease;

	public List<IEnumerator> TakeDamageAnimationList = new List<IEnumerator>();

	public List<IEnumerator> DeathAnimationList = new List<IEnumerator>();

	[HideInInspector]
	public Vector3 originalLocalPosition;

	[HideInInspector]
	public float yOffset;

	[HideInInspector]
	public bool isOnScreen;

	[HideInInspector]
	public UnitState state;

	public bool isEnemy;

	[SerializeField]
	private EngineSoundEffect _engineEffect;

	private Dictionary<DieFaceType, Handler> _actionAbilityHandlers;

	private int _currentAssetBundle = -1;

	public virtual string DAMAGE_SMOKE_HEAVY_ANIMATION_NAME
	{
		get
		{
			return "Damage Effects Smoke Loop Heavy";
		}
		set
		{
		}
	}

	public virtual string DAMAGE_SMOKE_MODERATE_ANIMATION_NAME
	{
		get
		{
			return "Damage Effects Smoke Loop Moderate";
		}
		set
		{
		}
	}

	public virtual float BURN_OFFSET
	{
		get
		{
			return 10f;
		}
		set
		{
		}
	}

	public BattleController BattleController
	{
		get
		{
			return battleController;
		}
	}

	public UnitPossibleRollsSimple PossibleRollsSimple
	{
		get
		{
			return _unitPossibleRollsSimple;
		}
	}

	public UnitBuffEffects BuffEffects
	{
		get
		{
			return buffEffects;
		}
	}

	public Transform TankSpritesTransform
	{
		get
		{
			return tankSpritesTransform;
		}
	}

	public GameObject DustEffect
	{
		get
		{
			return dustEffect;
		}
	}

	public ShieldEffect ShieldView
	{
		get
		{
			return shieldView;
		}
	}

	public ChartBarView RarityStars
	{
		get
		{
			return rarityStars;
		}
	}

	public HealthUi HealthUI
	{
		get
		{
			return _healthUi;
		}
	}

	private tk2dSprite TankSprite
	{
		get
		{
			return _sprite;
		}
	}

	public UnitAbilityText AbilityText
	{
		get
		{
			return abilityText;
		}
	}

	public bool IsLoaded { get; private set; }

	public TeamState Team
	{
		get
		{
			return state.team;
		}
	}

	public BattleField BattleField
	{
		get
		{
			return state.team.battleField;
		}
	}

	public bool RolledDice
	{
		get
		{
			return rolledDice;
		}
		set
		{
			rolledDice = value;
		}
	}

	public int[] DiceValues
	{
		get
		{
			return state.rollValues;
		}
	}

	public DieFaceType[] DiceSides
	{
		get
		{
			return state.rollTypes;
		}
	}

	public int StartingHealth
	{
		get
		{
			return state.startingHp;
		}
	}

	public int CurrentRoll
	{
		get
		{
			return state.currentRoll;
		}
	}

	public DieFaceType CurrentRollAction
	{
		get
		{
			return DiceSides[CurrentRoll];
		}
	}

	public int Rarity
	{
		get
		{
			return (state.UserUnitMetadata != null) ? state.UserUnitMetadata.Rarity : 0;
		}
	}

	public int LocalHealth
	{
		get
		{
			return localHealth;
		}
		set
		{
			localHealth = value;
			UpdateDamageEffects();
		}
	}

	public int LocalArmor
	{
		get
		{
			return localArmor;
		}
		set
		{
			SetLocalArmor(value, true);
		}
	}

	public int LocalDamagePerRound
	{
		get
		{
			return localDamagePerRound;
		}
		set
		{
			SetLocalDamagePerRound(value);
		}
	}

	public int LocalAcidPerRound
	{
		get
		{
			return localAcidPerRound;
		}
		set
		{
			SetLocalAcidPerRound(value);
		}
	}

	public int LocalExtraDamage
	{
		get
		{
			return localExtraDamage;
		}
		set
		{
			localExtraDamage = value;
		}
	}

	public bool LocalPreventReroll
	{
		get
		{
			return localPreventReroll;
		}
		set
		{
			SetPreventReroll(value);
		}
	}

	public int LocalRoundsUntilRerollEnabled
	{
		get
		{
			return localRoundsUntilRerollEnabled;
		}
		set
		{
			localRoundsUntilRerollEnabled = value;
		}
	}

	public bool LocalIsDead
	{
		get
		{
			return localHealth <= 0;
		}
	}

	public int LocalTotalDamageReceived { get; set; }

	public UnitWeaponSystem WeaponSystem
	{
		get
		{
			return weaponSystem;
		}
	}

	public bool HasAttacked { get; set; }

	public int AlternativeWeapon
	{
		get
		{
			return state.UserUnitMetadata.AlternativeWeapon;
		}
	}

	public event Action<UnitView> OnDiceFinishRollEvent;

	public event Action<UnitView, int> OnReceiveDamageEvent;

	private void Awake()
	{
		originalLocalPosition = base.transform.localPosition;
		_unitDamageTint = GetComponent<UnitDamageTint>();
		burnEffects = new List<EffectInstance>();
		acidEffects = new List<EffectInstance>();
	}

	public void Init(BattleController battleController, UnitState unitState)
	{
		StopAllCoroutines();
		this.battleController = battleController;
		state = unitState;
		unitState.unitView = this;
		_unitPossibleRollsSimple.Init();
		LocalHealth = state.hp;
		LocalArmor = state.armor;
		LocalDamagePerRound = state.damagePerRound;
		LocalAcidPerRound = state.acidPerRound;
		LocalExtraDamage = state.extraDamage;
		LocalPreventReroll = state.preventReroll;
		LocalRoundsUntilRerollEnabled = state.roundsUntilRerollEnabled;
		if ((bool)rarityStars)
		{
			rarityStars.SetBarLevel(Rarity);
		}
		if ((bool)_healthUi)
		{
			_healthUi.SetUp(LocalHealth, LocalArmor);
			if (unitState.metadata.UnitType == UnitType.RAID_BOSS)
			{
				_healthUi.gameObject.SetActive(false);
			}
		}
		StartCoroutine(UpdateTankSprite());
		RegisterActionAbilityHandler();
		if ((bool)_engineEffect)
		{
			_engineEffect.Init(state);
		}
		SetupDieFaces();
		if ((bool)unitIcon)
		{
			unitIcon.SetUnitIcon(state.metadata.UnitType);
		}
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.SHIELD, 1);
		if (AlternativeWeapon != 0)
		{
			AudioTrigger newAudioTrigger = (AudioTrigger)(int)Enum.Parse(typeof(AudioTrigger), "AudioABEffectFiring_" + AlternativeWeapon);
			AudioTrigger newAudioTrigger2 = (AudioTrigger)(int)Enum.Parse(typeof(AudioTrigger), "AudioABEffectFiring_" + AlternativeWeapon);
			Singleton<AudioCacheManager>.instance.RegisterSingleAudioClipFromAB(newAudioTrigger);
			Singleton<AudioCacheManager>.instance.RegisterSingleAudioClipFromAB(newAudioTrigger2);
		}
	}

	private void SetupDieFaces()
	{
		if (dieFaces != null)
		{
			for (int i = 0; i < dieFaces.Count; i++)
			{
				dieFaces[i].SetBoost(state.metadata.UnitBoost);
			}
		}
	}

	public void SetLocalArmor(int value, bool isDamage)
	{
		int num = localArmor;
		localArmor = value;
		if (localArmor > 0)
		{
			_healthUi.UpdateHealth(LocalHealth, value);
			if (num <= 0)
			{
				ActivateShieldView();
			}
			SetShieldAlpha();
		}
		else
		{
			if (localArmor > 0)
			{
				return;
			}
			localArmor = 0;
			if (num > 0)
			{
				if (isDamage)
				{
					DestroyShieldView();
				}
				else
				{
					DeactivateShieldView();
				}
			}
		}
	}

	public virtual Vector3 BurnEffectCenter()
	{
		return base.transform.position;
	}

	private void SetLocalDamagePerRound(int value)
	{
		bool flag = value > localDamagePerRound;
		localDamagePerRound = value;
		if (value > 0 && flag)
		{
			Vector3 worldPosition = BurnEffectCenter() + new Vector3(UnityEngine.Random.Range(BURN_OFFSET, BURN_OFFSET * 2f), UnityEngine.Random.Range(0f - BURN_OFFSET, BURN_OFFSET), -10f);
			EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.BURN, worldPosition, GetUnitObject());
			effectInstance.gameObject.SetLayerRecursively(base.gameObject.layer);
			burnEffects.Add(effectInstance);
		}
		else if (value <= 0)
		{
			foreach (EffectInstance burnEffect in burnEffects)
			{
				burnEffect.Destroy();
			}
			burnEffects.Clear();
		}
		else
		{
			for (int num = burnEffects.Count - 1; num > 0; num--)
			{
				burnEffects[num].Destroy();
				burnEffects.RemoveAt(num);
			}
		}
	}

	private void SetLocalAcidPerRound(int value)
	{
		bool flag = value > localAcidPerRound;
		localAcidPerRound = value;
		if (value > 0 && flag)
		{
			Vector3 worldPosition = BurnEffectCenter() + new Vector3(UnityEngine.Random.Range(BURN_OFFSET, BURN_OFFSET * 2f), UnityEngine.Random.Range(0f - BURN_OFFSET, BURN_OFFSET), -10f);
			EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.ACID_STRIKE, worldPosition, GetUnitObject());
			tk2dSpineAnimation component = effectInstance.GetComponent<tk2dSpineAnimation>();
			string[] animationNames = component.GetAnimationNames();
			component.AnimationName = animationNames[0];
			component.loop = true;
			effectInstance.gameObject.SetLayerRecursively(base.gameObject.layer);
			acidEffects.Add(effectInstance);
		}
		else if (value <= 0)
		{
			foreach (EffectInstance acidEffect in acidEffects)
			{
				acidEffect.Destroy();
			}
			acidEffects.Clear();
		}
		else
		{
			for (int num = acidEffects.Count - 1; num > 0; num--)
			{
				acidEffects[num].Destroy();
				acidEffects.RemoveAt(num);
			}
		}
	}

	private void SetPreventReroll(bool state)
	{
		if (state == localPreventReroll)
		{
			return;
		}
		localPreventReroll = state;
		if (localPreventReroll)
		{
			preventRerollEffectInstance = GlobalEffectsManager.Create(EffectType.SHORT_CIRCUIT_HIT, RepositionUnit(new Vector3(0f, 0f, -1f)), GetUnitTransform());
			tk2dSpineAnimation component = preventRerollEffectInstance.GetComponent<tk2dSpineAnimation>();
			if ((bool)component)
			{
				component.loop = true;
				component.AnimationName = "Electricity Effect " + UnityEngine.Random.Range(1, 3);
			}
		}
		else if ((bool)preventRerollEffectInstance)
		{
			preventRerollEffectInstance.Destroy();
		}
	}

	public void SetOriginalPosition(Vector2 position)
	{
		originalLocalPosition.x = position.x;
		originalLocalPosition.y = position.y;
	}

	public void RefreshHealthHUD()
	{
		_healthUi.UpdateHealth(LocalHealth, LocalArmor);
	}

	public IEnumerator RollDice(int targetRoll, bool shouldAnimate, int numRevolutions = 5, bool isInitialRoll = false)
	{
		if (!isInitialRoll && LocalPreventReroll)
		{
			if (this.OnDiceFinishRollEvent != null)
			{
				this.OnDiceFinishRollEvent(this);
			}
			yield break;
		}
		_unitPossibleRollsSimple.ResetAllPlusNumber();
		buffEffects.ResetBuffEffects();
		if (shouldAnimate)
		{
			yield return StartCoroutine(_unitPossibleRollsSimple.StartSpinAnimation(targetRoll, numRevolutions));
		}
		OnRolledDiceComplete(targetRoll);
	}

	public void SetDieFace(int index, int value)
	{
		_unitPossibleRollsSimple.SetDiceValue(index, value);
	}

	public void ToggleDieFaces(int[] values, bool show, bool ignoreBoost)
	{
		if (show)
		{
			_unitPossibleRollsSimple.OpenDieBox(0.5f);
		}
		else
		{
			_unitPossibleRollsSimple.CloseDieBox(0.5f);
		}
		_healthUi.SetVisible(show);
		_unitPossibleRollsSimple.SetVisible(show);
		_unitPossibleRollsSimple.SetDiceValues(values, ignoreBoost);
	}

	public void PlayUpgradeAnimation()
	{
		if ((bool)_upgrade)
		{
			StartCoroutine(_upgrade.PlayAnimCoroutine("Unit Special Level-up"));
		}
	}

	private void RegisterActionAbilityHandler()
	{
		if (_actionAbilityHandlers != null)
		{
			return;
		}
		_actionAbilityHandlers = new Dictionary<DieFaceType, Handler>();
		_actionAbilityHandlers.Add(DieFaceType.DirectDamage, delegate
		{
			if ((bool)abilityText && state.team == battleController.playerTeam)
			{
				PresentSpecialText("ui_battle_unitattack".Localize("Attack"), battleController.tunables.abilityTextFeedbackDelay);
			}
		});
		_actionAbilityHandlers.Add(DieFaceType.Initiative, delegate
		{
			if ((bool)abilityText && state.team == battleController.playerTeam)
			{
				PresentSpecialText("ui_battle_unitfirststrike".Localize("First Strike"), battleController.tunables.abilityTextFeedbackDelay);
			}
		});
		_actionAbilityHandlers.Add(DieFaceType.None, delegate
		{
			if ((bool)abilityText && state.team == battleController.playerTeam)
			{
				PresentSpecialText("ui_battle_unitmiss".Localize("Miss"), battleController.tunables.abilityTextFeedbackDelay);
			}
		});
		_actionAbilityHandlers.Add(DieFaceType.Special, delegate
		{
			if ((bool)abilityText && state.team == battleController.playerTeam)
			{
				abilityText.Present(state.AbilityName);
			}
		});
		_actionAbilityHandlers.Add(DieFaceType.ArmourPiercing, delegate
		{
			if ((bool)abilityText && state.team == battleController.playerTeam)
			{
				PresentSpecialText("ui_battle_unitArmourPiercing".Localize("Armour Piercing"), battleController.tunables.abilityTextFeedbackDelay);
			}
		});
		_actionAbilityHandlers.Add(DieFaceType.AcidStrike, delegate
		{
			if ((bool)abilityText && state.team == battleController.playerTeam)
			{
				PresentSpecialText("ui_battle_acidstrike".Localize("Acid Strike"), battleController.tunables.abilityTextFeedbackDelay);
			}
		});
	}

	private void OnRolledDiceComplete(int targetRoll)
	{
		_actionAbilityHandlers[state.rollTypes[targetRoll]]();
		foreach (AbilityState ability in state.abilities)
		{
			battleController.animationHandler.AddImmediateSequence(ability.animationHandler.OnRollFinished());
		}
		if (battleController.battleHooks != null)
		{
			battleController.battleHooks.OnPostReroll();
		}
		rolledDice = true;
		if (!isEnemy)
		{
			buffEffects.UpdateBuffEffects();
		}
		if (this.OnDiceFinishRollEvent != null)
		{
			this.OnDiceFinishRollEvent(this);
		}
	}

	public void PresentSpecial()
	{
		List<string> list = new List<string>();
		foreach (AbilityState ability in state.abilities)
		{
			list.Add((!string.IsNullOrEmpty(ability.animationHandler.InBattleName)) ? ability.animationHandler.InBattleName : ability.Name);
		}
		StartCoroutine(DisplayAllAbilities(list));
		if (CurrentRollAction == DieFaceType.Special)
		{
			_unitPossibleRollsSimple.ActivateActiveDie();
		}
	}

	public void PresentSpecialText(string text, float secondsToHide)
	{
		abilityText.Present(text, secondsToHide);
	}

	public void HideSpecialText()
	{
		abilityText.Hide();
	}

	public void OnUnitTapped()
	{
		PossibleRollsSimple.ToggleOpenClose();
		if ((!Team.IsEnemy && CurrentRollAction == DieFaceType.Special) || state.abilities.Count <= 0)
		{
			return;
		}
		if (!PossibleRollsSimple.DieBoxIsOpen())
		{
			List<string> list = new List<string>();
			{
				foreach (AbilityState ability in state.abilities)
				{
					list.Add(ability.Name);
					StartCoroutine(DisplayAllAbilities(list));
				}
				return;
			}
		}
		HideSpecialText();
	}

	private IEnumerator DisplayAllAbilities(List<string> abilityNames)
	{
		for (int i = 0; i < abilityNames.Count; i++)
		{
			PresentSpecialText(abilityNames[i], 1f);
			yield return new WaitForSeconds(2.5f);
		}
	}

	public void ShakeMyField(float intensity, float decay)
	{
		Team.battleField.shaker.Shake(intensity, decay);
	}

	private void ApplyLocalDamage(int damage, DamageType damageType)
	{
		if (!LocalIsDead)
		{
			LocalTotalDamageReceived += damage;
			if (this.OnReceiveDamageEvent != null)
			{
				this.OnReceiveDamageEvent(this, damage);
			}
			if (damageType == DamageType.ArmourPiercing)
			{
				LocalArmor = 0;
			}
			else
			{
				int num = Mathf.Min(localArmor, damage);
				LocalArmor -= num;
				damage -= num;
			}
			LocalHealth -= damage;
		}
	}

	public void TakeDamage(int damage, DamageType damageType = DamageType.Standard, bool isDamagePerRound = false)
	{
		if (LocalIsDead)
		{
			return;
		}
		int num = localArmor;
		ApplyLocalDamage(damage, damageType);
		if (isDamagePerRound)
		{
			AudioTrigger.DOTDamage.Play();
		}
		if (damage > 0)
		{
			PlayKickbackAnimation();
			_unitDamageTint.PlayReceiveDamageTint();
			_healthUi.UpdateDamageFeedback(damage);
		}
		foreach (IEnumerator takeDamageAnimation in TakeDamageAnimationList)
		{
			battleController.StartCoroutine(takeDamageAnimation);
		}
		TakeDamageAnimationList.Clear();
		RefreshHealthHUD();
		if (_engineEffect != null)
		{
			_engineEffect.ChangeHealth(LocalHealth);
		}
		if (num > 0 && localArmor <= 0)
		{
			DestroyShieldView();
		}
		if (!LocalIsDead)
		{
			return;
		}
		if (battleController.battleHooks != null)
		{
			StartCoroutine(battleController.battleHooks.OnUnitDeath(this));
			if (!battleController.battleHooks.OverrideUnitDeath(state))
			{
				OnUnitKilled(damage);
			}
		}
		else
		{
			OnUnitKilled(damage);
		}
	}

	private void UpdateDamageEffects()
	{
		float num = (float)LocalHealth / (float)StartingHealth;
		if (num < 0.5f)
		{
			if (num < 0.25f)
			{
				_smoke.AnimationName = DAMAGE_SMOKE_HEAVY_ANIMATION_NAME;
			}
			else
			{
				_smoke.AnimationName = DAMAGE_SMOKE_MODERATE_ANIMATION_NAME;
			}
			if (!_smoke.renderer.enabled)
			{
				tk2dSpineSkeleton skeleton = _smoke.GetComponent<tk2dSpineSkeleton>();
				skeleton.skeleton.A = 0f;
				_smoke.renderer.enabled = true;
				SimpleTween.Start(0f, 1f, 0.5f, 0.25f, EaseType.EaseInExpo, delegate(float val)
				{
					skeleton.skeleton.A = Mathf.Lerp(0f, 1f, val);
				});
			}
			_damageDecal.renderer.enabled = true;
		}
		else
		{
			_damageDecal.renderer.enabled = false;
			_smoke.renderer.enabled = false;
		}
	}

	private void OnUnitKilled(int damage)
	{
		if ((bool)_explosionAnimation)
		{
			UnitType unitType = ((state.UserUnitMetadata != null) ? state.UserUnitMetadata.UnitType : UnitType.NONE);
			_explosionAnimation.PlayAndMove(base.transform.parent, tankSpritesTransform.position - Vector3.up * 50f, damage, Rarity, unitType);
		}
		if (isEnemy)
		{
			if (state.droppedParts != null)
			{
				DropPartsAnimation();
				battleController.animationHandler.AddImmediateSequence(WaitSequence(0.4f));
			}
			if (battleController.MatchHandler.IsEventPointsMatch && UserProfile.player.divisionInt >= Constants.MinTierEventContent)
			{
				ShowCashStackEffect(state.metadata.DestroyEventPoints, UserInventory.ItemType.EventPoint);
			}
			else if (battleController.MatchHandler.IsRaidBossEventActive && state.metadata.UnitType == UnitType.RAID_BOSS)
			{
				ShowCashStackEffect(state.metadata.DestroyEventPoints, UserInventory.ItemType.RaidBossEventPoint);
			}
			else if (battleController.MatchHandler.IsEventMatch && UserProfile.player.divisionInt >= Constants.MinTierEventContent)
			{
				EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
				if (activeEvent != null && activeEvent.EventType == EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT)
				{
					ShowCashStackEffect(state.metadata.DestroyEventPoints, UserInventory.ItemType.VictoryPoint);
				}
			}
			if (state.gemsDropped > 0)
			{
				ShowGemEffect(state.gemsDropped);
			}
			ShowCashEffect(BattleLogic.GetUnitDestroyCoinReward(state, state.team));
			CelebrateKillAnimation();
			AudioTrigger.CrowdCheering.Play();
		}
		foreach (IEnumerator deathAnimation in DeathAnimationList)
		{
			battleController.animationHandler.AddImmediateSequence(deathAnimation);
		}
		AudioTrigger.TankKilled.Play();
		ShakeMyField(20f, 0.2f);
		battleController.RemoveUnit(this);
		StartCoroutine(DeactivateIn(0.25f));
	}

	public IEnumerator DeactivateIn(float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		base.gameObject.SetActive(false);
	}

	private void CelebrateKillAnimation()
	{
		string text = null;
		if (LocalHealth == 0)
		{
			text = "ui_battle_perfectkill".Localize("Perfect Kill");
		}
		else if (LocalHealth <= -Constants.BattleOverkillDamage)
		{
			text = "ui_battle_overkill".Localize("Over Kill");
		}
		if (state.killIndex == 1)
		{
			text = "ui_battle_doublekill".Localize("DOUBLE\nKILL!");
		}
		else if (state.killIndex == 2)
		{
			text = "ui_battle_triplekill".Localize("TRIPLE\nKILL");
		}
		else if (state.killIndex == 3)
		{
			text = "ui_battle_quadkill".Localize("QUAD\nKILL!");
		}
		else if (state.killIndex > 3)
		{
			text = "ui_battle_pentakill".Localize("PENTA\nKILL!!");
		}
		if (text != null)
		{
			StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.CRITICAL_HIT, base.transform.position, battleController.enemyField.gameObject, delegate(EffectInstance x)
			{
				x.AutoDestroy();
			}));
			battleController.hud.celebrationKill.CelebrateKill(this, text, Color.white);
		}
	}

	public void DropPartsAnimation()
	{
		List<UnitPartTypesDataModel> parts = state.droppedParts.Select((IPartMetadata x) => UnitPartTypesDataModel.GetSingle(x.ID)).ToList();
		DropPartsAnimation(parts);
	}

	private void DropPartsAnimation(List<UnitPartTypesDataModel> parts)
	{
		PartFoundEffect component = GlobalEffectsManager.Create(EffectType.PART_DROP, GetUnitTransform().position).SetLayer(base.gameObject.layer).GetComponent<PartFoundEffect>();
		component.PlayAnimation(parts, this);
		AudioTrigger.CrowdCheering.Play();
	}

	public IEnumerator WaitSequence(float seconds)
	{
		yield return new WaitForSeconds(0.7f);
	}

	public void ShowGiftEffect(List<ItemCollectionDataModel> gifts)
	{
		List<UnitPartTypesDataModel> list = new List<UnitPartTypesDataModel>();
		for (int i = 0; i < gifts.Count; i++)
		{
			for (int j = 0; j < gifts[i].items.Count; j++)
			{
				switch (gifts[i].items[j].itemType)
				{
				case UserInventory.ItemType.Coins:
					ShowCashEffect(gifts[i].items[j].amount);
					break;
				case UserInventory.ItemType.PremiumCurrency:
					ShowGemEffect(gifts[i].items[j].amount);
					break;
				case UserInventory.ItemType.Parts:
				{
					for (int k = 0; k < gifts[i].items[j].amount; k++)
					{
						list.Add(gifts[i].items[j].Part);
					}
					break;
				}
				case UserInventory.ItemType.EventPoint:
					ShowEventPointEffect(gifts[i].items[j].amount, UserInventory.ItemType.EventPoint);
					break;
				case UserInventory.ItemType.RaidBossEventPoint:
					ShowEventPointEffect(gifts[i].items[j].amount, UserInventory.ItemType.RaidBossEventPoint);
					break;
				case UserInventory.ItemType.Energy:
					ShowEnergyEffect(gifts[i].items[j].amount);
					break;
				}
			}
		}
		if (list.Count > 0)
		{
			DropPartsAnimation(list);
		}
	}

	public void ShowPassiveEffect(string animationName, string abilityName)
	{
		StartCoroutine(LoadPassiveEffect(animationName, abilityName));
	}

	private IEnumerator LoadPassiveEffect(string animationName, string abilityName)
	{
		EffectInstance passiveEffect = null;
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.PASSIVE_EFFECT, base.transform.position, null, delegate(EffectInstance x)
		{
			passiveEffect = x;
		}));
		passiveEffect.AutoDestroy().SetLayer(base.gameObject.layer);
		PresentSpecialText(abilityName, 1f);
		passiveEffect.SpineAnimation.animationName = animationName;
		float dirScaler = ((!Team.IsEnemy) ? (-1f) : 1f);
		passiveEffect.transform.localScale = new Vector3(dirScaler, 1f, 1f);
	}

	public void ShowRewardEffect()
	{
		GlobalEffectsManager.Create(EffectType.REWARD, base.transform.position).AutoDestroy().SetLayer(base.gameObject.layer);
	}

	public void ShowCashStackEffect(int amount, UserInventory.ItemType itemType)
	{
		CurrencyEffect component = GlobalEffectsManager.Create(EffectType.CASH, base.transform.position).SetLayer(base.gameObject.layer).GetComponent<CurrencyEffect>();
		component.ShowText = false;
		Vector3 endPoint = BattleField.unityCamera.ViewportToWorldPoint(new Vector3(0.5f, 1.1f, 0f));
		endPoint.z = base.transform.position.z;
		component.EndPoint = endPoint;
		component.ConfigureEffect(itemType, amount);
	}

	public void ShowEventPointEffect(int amount, UserInventory.ItemType eventType)
	{
		ShowRewardEffect();
		ShowCashStackEffect(amount, eventType);
	}

	public void ShowEnergyEffect(int amount)
	{
		ShowRewardEffect();
		ShowCashStackEffect(amount, UserInventory.ItemType.Energy);
	}

	public void ShowCashEffect(int amount)
	{
		ShowRewardEffect();
		ShowCashStackEffect(amount, UserInventory.ItemType.Coins);
	}

	public void ShowGemEffect(int amount)
	{
		ShowRewardEffect();
		ShowCashStackEffect(amount, UserInventory.ItemType.PremiumCurrency);
	}

	private void OnMouseDown()
	{
		battleController.OnSelectUnit(this);
	}

	public IEnumerator PlayWeaponFiringAnimation()
	{
		yield return StartCoroutine(weaponSystem.FiringAnimation(this));
	}

	public IEnumerator PlayWeaponHitAnimation(UnitView target)
	{
		yield return StartCoroutine(weaponSystem.HitAnimation(this, target));
	}

	public virtual void PlayKickbackAnimation(float kickbackAmount = 40f)
	{
		Sequence sequence = new Sequence();
		if (ambientMovementTween != null)
		{
			ambientMovementTween.Kill();
		}
		float newLocalXPosition = kickbackAmount * tankSpritesTransform.localScale.x + tankSpritesTransform.localPosition.x;
		sequence.Append(tankSpritesTransform.TweenLocalXPosition(newLocalXPosition, 0.25f));
		sequence.Append(tankSpritesTransform.TweenLocalXPosition(0f, 1f, EaseType.Linear));
		sequence.AppendCallback(BeginAmbientMovement);
		sequence.Play();
	}

	public void OnDrift()
	{
		if (shieldView != null)
		{
			shieldView.gameObject.transform.localPosition = new Vector3(tankSpritesTransform.localScale.x, shieldView.transform.localPosition.y, shieldView.transform.localPosition.z);
		}
	}

	public virtual void BeginAmbientMovement()
	{
		if (ambientMovementTween == null)
		{
			BattleTunables tunables = battleController.tunables;
			Vector3 vector = new Vector3((!isEnemy) ? (0f - tunables.unitDriftRange) : tunables.unitDriftRange, 0f, 0f);
			float unitDriftTime = tunables.unitDriftTime;
			ambientMovementTween = HOTween.To(tankSpritesTransform, unitDriftTime, new TweenParms().Prop("localPosition", vector).Loops(-1, LoopType.YoyoInverse).Delay(UnityEngine.Random.Range(-0.5f, tunables.unitDriftDelayRange))
				.Ease(EaseType.EaseInQuad)
				.OnUpdate(OnDrift));
			ambientMovementTween.Play();
		}
	}

	public IEnumerator UpdateTankSprite()
	{
		IsLoaded = false;
		_damageDecal.transform.parent = base.transform;
		if (_currentAssetBundle != state.metadata.AssetBundleID)
		{
			_currentAssetBundle = state.metadata.AssetBundleID;
			yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", state.metadata.AssetBundleID));
			GameObject tankSprite = unitProxy.Prefab;
			tankSprite.layer = base.gameObject.layer;
			_sprite = tankSprite.GetComponent<tk2dSprite>();
			_sprite.FlipX = !_sprite.FlipX;
			tankSprite.AddComponent<UnitSpriteTween>();
			Vector3 decalLocalPos = _damageDecal.transform.localPosition;
			Vector3 decalLocalSca = _damageDecal.transform.localScale;
			_damageDecal.transform.parent = tankSprite.transform;
			_damageDecal.transform.localScale = decalLocalSca;
			Vector3 parentPosition = tankSprite.transform.localPosition;
			_damageDecal.transform.localPosition = new Vector3(0f - parentPosition.x, 0f - parentPosition.y, decalLocalPos.z);
			IsLoaded = true;
			tk2dSprite spr = tankSprite.GetComponent<tk2dSprite>();
			string sprName = spr.GetCurrentSpriteDef().name;
			foreach (UnitAnimationLayer animationLayer in animationLayers)
			{
				animationLayer.Activate(spr.Collection, sprName);
				if (animationLayer.shouldBounce)
				{
					animationLayer.bounceTarget = tankSprite.transform;
				}
				if (animationLayer.hideInsideBattle)
				{
					animationLayer.gameObject.SetActive(false);
				}
			}
			UnitWeaponType defaultWeaponType = UnitWeaponType.CannonLow;
			if (state.UserUnitMetadata != null)
			{
				defaultWeaponType = state.UserUnitMetadata.WeaponType;
			}
			weaponSystem.Init(this, defaultWeaponType, GetUnitSprite());
			tk2dSpriteDefinition.AttachPoint attachPoint;
			if ((bool)shadow)
			{
				float horizontalSize = spr.GetBounds().extents.x;
				shadow.GetComponent<tk2dBaseSprite>().scale *= horizontalSize * 0.01f;
				attachPoint = spr.GetAttachPointByName("ground");
				if (attachPoint != null)
				{
					shadow.transform.localPosition = new Vector3(0f, attachPoint.position.y, attachPoint.position.z) + shadowOffset;
				}
			}
			bool isHelicopter = spr.GetAttachPointByName("helicopter") != null;
			attachPoint = spr.GetAttachPointByName("dust");
			if (attachPoint != null)
			{
				if (isHelicopter)
				{
					dustEffect.SetActive(false);
					EffectInstance effect = GlobalEffectsManager.Create(EffectType.DUST_AIR, tankSprite.transform.position, tankSpritesTransform);
					effect.SpineAnimation.state.Time += UnityEngine.Random.value * 30f;
					dustEffect = effect.gameObject;
				}
				else
				{
					dustEffect.SetActive(true);
				}
				dustEffect.transform.localPosition = new Vector3(0f - attachPoint.position.x, attachPoint.position.y, attachPoint.position.z) + dustOffset;
			}
		}
		else
		{
			IsLoaded = true;
			_unitDamageTint.Reset();
		}
	}

	public List<tk2dBaseSprite> GetSprites(bool includeTank = true, bool includeSelectedDie = true, bool includeHealth = true, bool includePossibleRolls = true)
	{
		List<tk2dBaseSprite> list = new List<tk2dBaseSprite>();
		if (includeTank)
		{
			list.Add(_sprite);
		}
		foreach (UnitAnimationLayer animationLayer in animationLayers)
		{
			list.Add(animationLayer.Sprite);
		}
		int num = 0;
		tk2dSprite[] dieFaceSprites = _unitPossibleRollsSimple._dieFaceSprites;
		foreach (tk2dSprite item in dieFaceSprites)
		{
			bool flag = false;
			if (includePossibleRolls)
			{
				flag = true;
			}
			if (num == CurrentRoll)
			{
				flag = includeSelectedDie;
			}
			if (flag)
			{
				list.Add(item);
			}
			num++;
		}
		return list;
	}

	public virtual bool HasUnitSprite()
	{
		return TankSprite != null;
	}

	public virtual tk2dSpriteDefinition GetUnitSprite()
	{
		if (TankSprite != null)
		{
			return TankSprite.CurrentSprite;
		}
		return null;
	}

	public virtual tk2dSpriteDefinition.AttachPoint GetUnitAttachPointByName(string name)
	{
		if (TankSprite != null)
		{
			return TankSprite.GetAttachPointByName(name);
		}
		return null;
	}

	public virtual Transform GetUnitTransform()
	{
		if (TankSprite != null)
		{
			return TankSprite.transform;
		}
		return null;
	}

	public virtual Vector3 RepositionUnit(Vector3 vector)
	{
		if (TankSprite != null)
		{
			return TankSprite.transform.position + vector;
		}
		return vector;
	}

	public virtual Vector3 GetUnitScale()
	{
		if (TankSprite != null)
		{
			return TankSprite.scale;
		}
		return Vector3.one;
	}

	public virtual GameObject GetUnitObject()
	{
		if (TankSprite != null)
		{
			return TankSprite.gameObject;
		}
		return null;
	}

	public virtual void SetUnitColor(Color color)
	{
		if (TankSprite != null)
		{
			TankSprite.color = color;
		}
	}

	public void ActivateShieldView()
	{
		if (!(this == null))
		{
			EffectInstance effectInstance = GlobalEffectsManager.Create(EffectType.SHIELD, tankSpritesTransform.position, tankSpritesTransform.gameObject);
			shieldView = effectInstance.GetComponent<ShieldEffect>();
			shieldView.SetTarget(tankSpritesTransform);
			shieldView.ActivateShield();
		}
	}

	public void SetShieldAlpha()
	{
		if ((bool)shieldView)
		{
			float alpha = Mathf.Clamp(localArmor, 3f, 8f) / 8f;
			shieldView.SetAlpha(alpha);
		}
	}

	public void DeactivateShieldView()
	{
		if ((bool)shieldView && !deactivatingShield)
		{
			deactivatingShield = true;
			shieldView.DeactivateShield(OnShieldDeactivated);
		}
	}

	public void DestroyShieldView()
	{
		if ((bool)shieldView)
		{
			deactivatingShield = true;
			shieldView.DestroyShield(OnShieldDeactivated);
		}
	}

	private void OnShieldDeactivated(tk2dSpineAnimation spineAnimation)
	{
		if ((bool)shieldView)
		{
			spineAnimation.AnimationComplete -= OnShieldDeactivated;
			GlobalEffectsManager.Return(shieldView.gameObject);
			deactivatingShield = false;
		}
	}

	public void PlayBurnFlareUp()
	{
		for (int i = 0; i < burnEffects.Count; i++)
		{
			EffectInstance effectInstance = burnEffects[i];
			Transform transform = effectInstance.transform;
			Vector3 localScale = transform.localScale;
			localScale.x *= 2.5f;
			localScale.y *= 2.5f;
			localScale.z *= 2.5f;
			Sequence sequence = new Sequence();
			sequence.Append(HOTween.To(transform, 0.2f, new TweenParms().Prop("localScale", localScale)));
			sequence.Append(HOTween.To(transform, 0.2f, new TweenParms().Prop("localScale", new Vector3(Mathf.Sign(localScale.x) * effectInstance.originalScale.x, effectInstance.originalScale.y, 1f))));
			sequence.Play();
		}
	}

	public void DriveOffScreen(float time = 1.75f)
	{
		float num = ((!isEnemy) ? (-1f) : 1f);
		Vector3 vector = originalLocalPosition + new Vector3(960f * num, 0f, 0f);
		if (time == 0f)
		{
			base.transform.localPosition = vector;
		}
		else
		{
			base.transform.TweenLocalPosition(vector, time, EaseType.EaseOutSine);
		}
		isOnScreen = false;
	}

	public void OnToField()
	{
		StartCoroutine(WaitForAnimationHandlerOntoField());
	}

	private IEnumerator WaitForAnimationHandlerOntoField()
	{
		if (state.abilities.Count == 0)
		{
			yield break;
		}
		while (true)
		{
			foreach (AbilityState ability in state.abilities)
			{
				if (ability.animationHandler != null)
				{
					AddOntoFieldAnimation();
					yield break;
				}
			}
			yield return new WaitForEndOfFrame();
		}
	}

	private void AddOntoFieldAnimation()
	{
		foreach (AbilityState ability in state.abilities)
		{
			if (ability.animationHandler != null)
			{
				battleController.animationHandler.AddImmediateSequence(ability.animationHandler.OntoFieldAnimation());
			}
		}
	}

	public void DriveOnScreen(float time = 1.75f, Action onComplete = null)
	{
		base.transform.TweenLocalPosition(originalLocalPosition, time, EaseType.EaseOutSine, onComplete);
		isOnScreen = true;
	}

	public override string ToString()
	{
		if (isEnemy)
		{
			return "Enemy" + state.index;
		}
		return "Player" + state.index;
	}

	public bool IsHelicopter()
	{
		GameObject prefab = unitProxy.Prefab;
		tk2dSprite component = prefab.GetComponent<tk2dSprite>();
		return component.GetAttachPointByName("helicopter") != null;
	}
}
