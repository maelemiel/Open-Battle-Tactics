namespace Holoville.HOTween
{
	public class TweenEvent
	{
		private readonly IHOTweenComponent _tween;

		private readonly object[] _parms;

		public IHOTweenComponent tween
		{
			get
			{
				return _tween;
			}
		}

		public object[] parms
		{
			get
			{
				return _parms;
			}
		}

		internal TweenEvent(IHOTweenComponent p_tween, object[] p_parms)
		{
			_tween = p_tween;
			_parms = p_parms;
		}
	}
}
