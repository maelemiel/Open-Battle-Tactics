public class SceneTransitionActivate : SceneTransition
{
	private bool original;

	public override void PrepareTransition()
	{
		original = base.gameObject.activeInHierarchy;
		base.gameObject.SetActive(true);
		base.PrepareTransition();
	}

	public override void EndTransition()
	{
		base.gameObject.SetActive(original);
		base.EndTransition();
	}
}
