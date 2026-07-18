using UnityEngine;

public class RailgunEffect : MonoBehaviour
{
	private tk2dSpineAnimation spineAnim;

	public bool IsCharging { get; private set; }

	public bool IsDeploying { get; private set; }

	private void Awake()
	{
		spineAnim = GetComponent<tk2dSpineAnimation>();
		spineAnim.renderer.enabled = false;
	}

	private void OnDestroy()
	{
		Object.Destroy(spineAnim.gameObject);
	}

	public void Deploy()
	{
		IsDeploying = true;
		spineAnim.renderer.enabled = true;
		spineAnim.AnimationName = "Deploy";
		spineAnim.AnimationComplete += DeployComplete;
	}

	private void DeployComplete(tk2dSpineAnimation anim)
	{
		IsDeploying = false;
		spineAnim.AnimationComplete -= DeployComplete;
		Idle();
	}

	public void Chargeup()
	{
		IsCharging = true;
		spineAnim.AnimationName = "Charge";
		spineAnim.loop = false;
		spineAnim.AnimationComplete += ChargeComplete;
	}

	public void Recoil()
	{
		spineAnim.AnimationName = "Recoil";
		spineAnim.loop = false;
	}

	public void ChargeComplete(tk2dSpineAnimation anim)
	{
		spineAnim.AnimationComplete -= ChargeComplete;
		IsCharging = false;
	}

	public void Idle()
	{
		spineAnim.AnimationName = "Idle";
		spineAnim.loop = true;
	}
}
