using System.Collections;
using Holoville.HOTween;
using Spine;
using UnityEngine;

public class FastForwardWaitingPopUpController : PopupController
{
	private const string BONE_NAME_TANK_1 = "Tank 1";

	private const string BONE_NAME_TANK_1_UC = "1 UC";

	private const string BONE_NAME_TANK_2_R = "2 R";

	private const string BONE_NAME_TANK_3_SR = "3 SR";

	[SerializeField]
	private GameObject _tvStatic;

	[SerializeField]
	private GameObject _info;

	[SerializeField]
	private FireworksAnimation _fireworks;

	[SerializeField]
	private tk2dSpineAnimation _tankAnimation;

	[SerializeField]
	private tk2dSprite _spriteOverlay;

	[SerializeField]
	private tk2dTextMesh _descriptionText;

	[SerializeField]
	private tk2dTextMesh _loadingText;

	[SerializeField]
	private GameObject _continueButton;

	[SerializeField]
	private GameObject _unit1;

	[SerializeField]
	private GameObject _unit2;

	[SerializeField]
	private GameObject _unit3;

	[SerializeField]
	private GameObject _unit4;

	[SerializeField]
	private GameObject _unit5;

	[SerializeField]
	private GameObject _unit6;

	[SerializeField]
	private UnitProxy _unitProxy1;

	[SerializeField]
	private UnitProxy _unitProxy2;

	[SerializeField]
	private UnitProxy _unitProxy3;

	[SerializeField]
	private UnitProxy _unitProxy4;

	[SerializeField]
	private UnitProxy _unitProxy5;

	[SerializeField]
	private UnitProxy _unitProxy6;

	private bool _assetBundleReady;

	private bool _finish;

	private Bone _tank1Bone;

	private Bone _UCBone;

	private Bone _RBone;

	private Bone _SRBone;

	private float _volume;

	private AudioManager.RepeatingSfx _staticChannelSFX;

	private int[] _unitsIds = new int[6] { 4101008, 4301009, 4302001, 4302005, 4303002, 4303003 };

	protected override void Awake()
	{
		_spriteOverlay.Alpha = 0f;
		_continueButton.SetActive(false);
		SceneController.resumeCallbackEnable = false;
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		Singleton<AudioManager>.instance.StopMusic();
		_staticChannelSFX = AudioTrigger.ChannelStatic.PlayRepeating();
		if ((bool)_tankAnimation)
		{
			_tank1Bone = _tankAnimation.Skeleton.skeleton.FindBone("Tank 1");
			_UCBone = _tankAnimation.Skeleton.skeleton.FindBone("1 UC");
			_RBone = _tankAnimation.Skeleton.skeleton.FindBone("2 R");
			_SRBone = _tankAnimation.Skeleton.skeleton.FindBone("3 SR");
		}
		StartCoroutine(SetAssetBundle());
		StartCoroutine(Init());
	}

	protected override void Update()
	{
		base.Update();
		if ((bool)_tankAnimation && _assetBundleReady)
		{
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			float num = 0f;
			switch (_tankAnimation.animationName)
			{
			case "Driving 1 R":
				zero = new Vector3(_RBone.X, _RBone.Y, 0f);
				zero2 = new Vector3(_RBone.ScaleX, _RBone.ScaleY, 1f);
				num = _RBone.WorldRotation;
				break;
			case "Driving 1 SR":
				zero = new Vector3(_SRBone.X, _SRBone.Y, 0f);
				zero2 = new Vector3(_SRBone.ScaleX, _SRBone.ScaleY, 1f);
				num = _SRBone.WorldRotation;
				break;
			case "Driving 1 UC":
				zero = new Vector3(_UCBone.X, _UCBone.Y, 0f);
				zero2 = new Vector3(_UCBone.ScaleX, _UCBone.ScaleY, 1f);
				num = _UCBone.WorldRotation;
				break;
			default:
				zero = new Vector3(_tank1Bone.X, _tank1Bone.Y, 0f);
				zero2 = new Vector3(_tank1Bone.ScaleX, _tank1Bone.ScaleY, 1f);
				num = _tank1Bone.WorldRotation;
				break;
			}
			_unitProxy1.transform.localPosition = zero;
			_unitProxy1.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			_unitProxy1.transform.localScale = zero2;
			_unitProxy2.transform.localPosition = zero;
			_unitProxy2.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			_unitProxy2.transform.localScale = zero2;
			_unitProxy3.transform.localPosition = zero;
			_unitProxy3.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			_unitProxy3.transform.localScale = zero2;
			_unitProxy4.transform.localPosition = zero;
			_unitProxy4.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			_unitProxy4.transform.localScale = zero2;
			_unitProxy5.transform.localPosition = zero;
			_unitProxy5.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			_unitProxy5.transform.localScale = zero2;
			_unitProxy6.transform.localPosition = zero;
			_unitProxy6.transform.localRotation = Quaternion.Euler(0f, 0f, num);
			_unitProxy6.transform.localScale = zero2;
		}
	}

	private IEnumerator Init()
	{
		_volume = Singleton<AudioManager>.instance.SfxVolume;
		yield return new WaitForSeconds(0.5f);
		_spriteOverlay.TweenAlpha(1f, 0.1f, EaseType.Linear);
		yield return new WaitForSeconds(0.1f);
		_tvStatic.SetActive(false);
		_spriteOverlay.TweenAlpha(0f, 0.1f, EaseType.Linear);
		yield return new WaitForSeconds(0.1f);
		_staticChannelSFX.Stop();
		AudioTrigger.Sponsor.PlayMusic();
		StartCoroutine(GoAndJumpBucle());
		_fireworks.PlayEffect();
		yield return new WaitForSeconds(3f);
		while (Singleton<SponsorPayManager>.instance.CurrentBrandEngageStatus == SponsorPayManager.BrandEngageStatus.WaitingForRequestResult)
		{
			yield return 0;
		}
		if (Singleton<SponsorPayManager>.instance.CurrentBrandEngageResult != SponsorPayManager.BrandEngageResult.NoVideoAvailable)
		{
			_spriteOverlay.TweenAlpha(1f, 0.1f, EaseType.Linear);
			yield return new WaitForSeconds(0.1f);
		}
		Singleton<AudioManager>.instance.SfxVolume = 0f;
		Singleton<SponsorPayManager>.instance.StartBrandEngage();
		while (Singleton<SponsorPayManager>.instance.CurrentBrandEngageStatus != SponsorPayManager.BrandEngageStatus.ReadyToRequest)
		{
			yield return 0;
		}
		if (Singleton<SponsorPayManager>.instance.CurrentBrandEngageResult != SponsorPayManager.BrandEngageResult.NoVideoAvailable)
		{
			yield return StartCoroutine(CloseAnimation());
		}
		StopAllCoroutines();
		if (Singleton<SponsorPayManager>.instance.CurrentBrandEngageResult == SponsorPayManager.BrandEngageResult.NoVideoAvailable)
		{
			_unit1.SetActive(false);
			_unit2.SetActive(false);
			_unit3.SetActive(false);
			_unit4.SetActive(false);
			_unit5.SetActive(false);
			_unit6.SetActive(false);
			_loadingText.gameObject.SetActive(false);
			_continueButton.SetActive(true);
			_descriptionText.text = LocalizationManager.GetString("ui_fast_forward_no_video_available", "No videos available at this time");
		}
		else
		{
			ContinueButton();
		}
	}

	private IEnumerator GoAndJumpBucle()
	{
		while (!_finish)
		{
			yield return StartCoroutine(GoAndJumpAnimation(_unit1, _unitProxy1));
			yield return StartCoroutine(GoAndJumpAnimation(_unit2, _unitProxy2));
			yield return StartCoroutine(GoAndJumpAnimation(_unit3, _unitProxy3));
			yield return StartCoroutine(GoAndJumpAnimation(_unit4, _unitProxy4));
			yield return StartCoroutine(GoAndJumpAnimation(_unit5, _unitProxy5));
			yield return StartCoroutine(GoAndJumpAnimation(_unit6, _unitProxy6));
		}
	}

	private IEnumerator GoAndJumpAnimation(GameObject unit, UnitProxy unitProxy)
	{
		Transform[] unitChildrens = unitProxy.GetComponentsInChildren<Transform>();
		unit.transform.localPosition = new Vector3(-800f, unit.transform.localPosition.y, unit.transform.localPosition.z);
		unit.transform.TweenLocalXPosition(-300f, 0.5f);
		yield return new WaitForSeconds(0.3f);
		Transform[] array = unitChildrens;
		foreach (Transform transform in array)
		{
			if (transform.gameObject.name != unitProxy.Prefab.name && transform.gameObject.name != unitProxy.gameObject.name)
			{
				transform.gameObject.SetActive(false);
			}
		}
		_tankAnimation.animationName = "Happy Bounce 2";
		yield return new WaitForSeconds(0.7f);
		Transform[] array2 = unitChildrens;
		foreach (Transform transform2 in array2)
		{
			transform2.gameObject.SetActive(true);
		}
		_tankAnimation.animationName = "Driving 1 UC";
		unit.transform.TweenLocalXPosition(0f, 0.7f);
		yield return new WaitForSeconds(0.5f);
		Transform[] array3 = unitChildrens;
		foreach (Transform transform3 in array3)
		{
			if (transform3.gameObject.name != unitProxy.Prefab.name && transform3.gameObject.name != unitProxy.gameObject.name)
			{
				transform3.gameObject.SetActive(false);
			}
		}
		_tankAnimation.animationName = "Happy Bounce 1";
		yield return new WaitForSeconds(0.7f);
		Transform[] array4 = unitChildrens;
		foreach (Transform transform4 in array4)
		{
			transform4.gameObject.SetActive(true);
		}
		_tankAnimation.animationName = "Driving 1 UC";
		unit.transform.TweenLocalXPosition(800f, 0.9f);
	}

	private IEnumerator CloseAnimation()
	{
		_info.SetActive(false);
		yield return new WaitForSeconds(0.1f);
		_spriteOverlay.TweenAlpha(0f, 0.1f, EaseType.Linear);
		yield return new WaitForSeconds(0.1f);
	}

	public virtual IEnumerator SetAssetBundle()
	{
		if ((bool)_unitProxy1)
		{
			_unitProxy1.gameObject.SetActive(true);
			yield return StartCoroutine(_unitProxy1.ChangeAssetCoroutine("Prefab.prefab", _unitsIds[0]));
			UnitSpriteTween unitSpriteTween = _unitProxy1.Prefab.GetComponent<UnitSpriteTween>();
			if ((bool)unitSpriteTween)
			{
				unitSpriteTween.enabled = false;
			}
		}
		if ((bool)_unitProxy2)
		{
			_unitProxy2.gameObject.SetActive(true);
			yield return StartCoroutine(_unitProxy2.ChangeAssetCoroutine("Prefab.prefab", _unitsIds[1]));
			UnitSpriteTween unitSpriteTween = _unitProxy2.Prefab.GetComponent<UnitSpriteTween>();
			if ((bool)unitSpriteTween)
			{
				unitSpriteTween.enabled = false;
			}
		}
		if ((bool)_unitProxy3)
		{
			_unitProxy3.gameObject.SetActive(true);
			yield return StartCoroutine(_unitProxy3.ChangeAssetCoroutine("Prefab.prefab", _unitsIds[2]));
			UnitSpriteTween unitSpriteTween = _unitProxy3.Prefab.GetComponent<UnitSpriteTween>();
			if ((bool)unitSpriteTween)
			{
				unitSpriteTween.enabled = false;
			}
		}
		if ((bool)_unitProxy4)
		{
			_unitProxy4.gameObject.SetActive(true);
			yield return StartCoroutine(_unitProxy4.ChangeAssetCoroutine("Prefab.prefab", _unitsIds[3]));
			UnitSpriteTween unitSpriteTween = _unitProxy4.Prefab.GetComponent<UnitSpriteTween>();
			if ((bool)unitSpriteTween)
			{
				unitSpriteTween.enabled = false;
			}
		}
		if ((bool)_unitProxy5)
		{
			_unitProxy5.gameObject.SetActive(true);
			yield return StartCoroutine(_unitProxy5.ChangeAssetCoroutine("Prefab.prefab", _unitsIds[4]));
			UnitSpriteTween unitSpriteTween = _unitProxy5.Prefab.GetComponent<UnitSpriteTween>();
			if ((bool)unitSpriteTween)
			{
				unitSpriteTween.enabled = false;
			}
		}
		if ((bool)_unitProxy6)
		{
			_unitProxy6.gameObject.SetActive(true);
			yield return StartCoroutine(_unitProxy6.ChangeAssetCoroutine("Prefab.prefab", _unitsIds[5]));
			UnitSpriteTween unitSpriteTween = _unitProxy6.Prefab.GetComponent<UnitSpriteTween>();
			if ((bool)unitSpriteTween)
			{
				unitSpriteTween.enabled = false;
			}
		}
		_assetBundleReady = true;
	}

	private void ContinueButton()
	{
		AudioTrigger.MenuBackground_Music.PlayMusic();
		Singleton<AudioManager>.instance.SfxVolume = _volume;
		SceneController.resumeCallbackEnable = true;
		ShopItemsSceneController.instance.ChangeListButtonStatus(true);
		base.OnCloseButton();
	}
}
