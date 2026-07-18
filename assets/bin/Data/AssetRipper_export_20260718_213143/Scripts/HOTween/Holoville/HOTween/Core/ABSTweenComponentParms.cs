namespace Holoville.HOTween.Core
{
	public abstract class ABSTweenComponentParms
	{
		protected string id = "";

		protected int intId = -1;

		protected bool autoKillOnComplete = true;

		protected UpdateType updateType = HOTween.defUpdateType;

		protected float timeScale = HOTween.defTimeScale;

		protected int loops = 1;

		protected LoopType loopType = HOTween.defLoopType;

		protected bool isPaused;

		protected TweenDelegate.TweenCallback onStart;

		protected TweenDelegate.TweenCallbackWParms onStartWParms;

		protected object[] onStartParms;

		protected TweenDelegate.TweenCallback onUpdate;

		protected TweenDelegate.TweenCallbackWParms onUpdateWParms;

		protected object[] onUpdateParms;

		protected TweenDelegate.TweenCallback onPause;

		protected TweenDelegate.TweenCallbackWParms onPauseWParms;

		protected object[] onPauseParms;

		protected TweenDelegate.TweenCallback onPlay;

		protected TweenDelegate.TweenCallbackWParms onPlayWParms;

		protected object[] onPlayParms;

		protected TweenDelegate.TweenCallback onRewinded;

		protected TweenDelegate.TweenCallbackWParms onRewindedWParms;

		protected object[] onRewindedParms;

		protected TweenDelegate.TweenCallback onStepComplete;

		protected TweenDelegate.TweenCallbackWParms onStepCompleteWParms;

		protected object[] onStepCompleteParms;

		protected TweenDelegate.TweenCallback onComplete;

		protected TweenDelegate.TweenCallbackWParms onCompleteWParms;

		protected object[] onCompleteParms;

		protected void InitializeOwner(ABSTweenComponent p_owner)
		{
			p_owner._id = id;
			p_owner._intId = intId;
			p_owner._autoKillOnComplete = autoKillOnComplete;
			p_owner._updateType = updateType;
			p_owner._timeScale = timeScale;
			p_owner._loops = loops;
			p_owner._loopType = loopType;
			p_owner._isPaused = isPaused;
			p_owner.onStart = onStart;
			p_owner.onStartWParms = onStartWParms;
			p_owner.onStartParms = onStartParms;
			p_owner.onUpdate = onUpdate;
			p_owner.onUpdateWParms = onUpdateWParms;
			p_owner.onUpdateParms = onUpdateParms;
			p_owner.onPause = onPause;
			p_owner.onPauseWParms = onPauseWParms;
			p_owner.onPauseParms = onPauseParms;
			p_owner.onPlay = onPlay;
			p_owner.onPlayWParms = onPlayWParms;
			p_owner.onPlayParms = onPlayParms;
			p_owner.onRewinded = onRewinded;
			p_owner.onRewindedWParms = onRewindedWParms;
			p_owner.onRewindedParms = onRewindedParms;
			p_owner.onStepComplete = onStepComplete;
			p_owner.onStepCompleteWParms = onStepCompleteWParms;
			p_owner.onStepCompleteParms = onStepCompleteParms;
			p_owner.onComplete = onComplete;
			p_owner.onCompleteWParms = onCompleteWParms;
			p_owner.onCompleteParms = onCompleteParms;
		}
	}
}
