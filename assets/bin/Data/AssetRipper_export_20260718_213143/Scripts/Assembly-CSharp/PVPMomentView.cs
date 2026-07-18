using Spine;
using UnityEngine;

public class PVPMomentView : MonoBehaviour
{
	private static string PVP_MOMENT_END_ANIMATION = "PVP End";

	private static string ROOT_BONE_NAME = "root";

	private static string PVP_MOMENT_PLAYER_1_BONE_NAME = "Player 1";

	private static string PVP_MOMENT_PLAYER_2_BONE_NAME = "Player 2";

	[SerializeField]
	private tk2dSpineAnimation pvpMomentAnimation;

	public PlayerView player1View;

	public PlayerView player2View;

	public float timeWhenShowing = 3f;

	private bool isPlaying;

	private Bone player1Bone;

	private Bone player2Bone;

	private Bone rootBone;

	public Vector3 initialAnimationLocalPosition = new Vector3(20f, 0f, 0f);

	private Vector3 initialLocalPosition;

	private EffectInstance pvpMomentAnimationEffect;

	private void Start()
	{
		initialLocalPosition = base.transform.localPosition;
		pvpMomentAnimationEffect = GlobalEffectsManager.Create(EffectType.PVP_MOMENT, base.transform.position, base.transform);
		pvpMomentAnimation = pvpMomentAnimationEffect.GetComponent<tk2dSpineAnimation>();
		if (!pvpMomentAnimation)
		{
			Log.Error("PVP Animation not loaded", base.gameObject);
			base.enabled = false;
			return;
		}
		if ((bool)pvpMomentAnimation)
		{
			rootBone = pvpMomentAnimation.Skeleton.skeleton.FindBone(ROOT_BONE_NAME);
		}
		if ((bool)pvpMomentAnimation)
		{
			player1Bone = pvpMomentAnimation.Skeleton.skeleton.FindBone(PVP_MOMENT_PLAYER_1_BONE_NAME);
		}
		if ((bool)pvpMomentAnimation)
		{
			player2Bone = pvpMomentAnimation.Skeleton.skeleton.FindBone(PVP_MOMENT_PLAYER_2_BONE_NAME);
		}
	}

	private void Update()
	{
		base.transform.localPosition = new Vector3(rootBone.X - 20f, initialLocalPosition.y, initialLocalPosition.z);
		base.transform.localRotation = Quaternion.Euler(0f, 0f, rootBone.WorldRotation);
		player1View.transform.localPosition = new Vector3(player1Bone.X, player1Bone.Y, 0f);
		player2View.transform.localPosition = new Vector3(player2Bone.X, player2Bone.Y, 0f);
	}

	public void ShowPVPMoment(OpponentData player1, OpponentData player2)
	{
		player1View.ConfigureView(player1, false, 0);
		player2View.ConfigureView(player2, false, 0);
	}

	public void HidePVPMoment()
	{
		if ((bool)pvpMomentAnimation)
		{
			pvpMomentAnimation.AnimationName = PVP_MOMENT_END_ANIMATION;
			pvpMomentAnimation.AnimationComplete += OnAnimationComplete;
		}
	}

	private void OnAnimationComplete(tk2dSpineAnimation animation)
	{
		if ((bool)pvpMomentAnimationEffect)
		{
			pvpMomentAnimationEffect.Destroy();
		}
		player1View.gameObject.SetActive(false);
		player2View.gameObject.SetActive(false);
	}
}
