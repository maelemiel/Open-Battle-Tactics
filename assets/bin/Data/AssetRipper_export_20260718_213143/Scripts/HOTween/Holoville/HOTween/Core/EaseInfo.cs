using Holoville.HOTween.Core.Easing;

namespace Holoville.HOTween.Core
{
	public class EaseInfo
	{
		public TweenDelegate.EaseFunc ease;

		public TweenDelegate.EaseFunc inverseEase;

		private EaseInfo(TweenDelegate.EaseFunc p_ease, TweenDelegate.EaseFunc p_inverseEase)
		{
			ease = p_ease;
			inverseEase = p_inverseEase;
		}

		internal static EaseInfo GetEaseInfo(EaseType p_easeType)
		{
			switch (p_easeType)
			{
			case EaseType.EaseInSine:
				return new EaseInfo(Sine.EaseIn, Sine.EaseOut);
			case EaseType.EaseOutSine:
				return new EaseInfo(Sine.EaseOut, Sine.EaseIn);
			case EaseType.EaseInOutSine:
				return new EaseInfo(Sine.EaseInOut, null);
			case EaseType.EaseInQuad:
				return new EaseInfo(Quad.EaseIn, Quad.EaseOut);
			case EaseType.EaseOutQuad:
				return new EaseInfo(Quad.EaseOut, Quad.EaseIn);
			case EaseType.EaseInOutQuad:
				return new EaseInfo(Quad.EaseInOut, null);
			case EaseType.EaseInCubic:
				return new EaseInfo(Cubic.EaseIn, Cubic.EaseOut);
			case EaseType.EaseOutCubic:
				return new EaseInfo(Cubic.EaseOut, Cubic.EaseIn);
			case EaseType.EaseInOutCubic:
				return new EaseInfo(Cubic.EaseInOut, null);
			case EaseType.EaseInQuart:
				return new EaseInfo(Quart.EaseIn, Quart.EaseOut);
			case EaseType.EaseOutQuart:
				return new EaseInfo(Quart.EaseOut, Quart.EaseIn);
			case EaseType.EaseInOutQuart:
				return new EaseInfo(Quart.EaseInOut, null);
			case EaseType.EaseInQuint:
				return new EaseInfo(Quint.EaseIn, Quint.EaseOut);
			case EaseType.EaseOutQuint:
				return new EaseInfo(Quint.EaseOut, Quint.EaseIn);
			case EaseType.EaseInOutQuint:
				return new EaseInfo(Quint.EaseInOut, null);
			case EaseType.EaseInExpo:
				return new EaseInfo(Expo.EaseIn, Expo.EaseOut);
			case EaseType.EaseOutExpo:
				return new EaseInfo(Expo.EaseOut, Expo.EaseIn);
			case EaseType.EaseInOutExpo:
				return new EaseInfo(Expo.EaseInOut, null);
			case EaseType.EaseInCirc:
				return new EaseInfo(Circ.EaseIn, Circ.EaseOut);
			case EaseType.EaseOutCirc:
				return new EaseInfo(Circ.EaseOut, Circ.EaseIn);
			case EaseType.EaseInOutCirc:
				return new EaseInfo(Circ.EaseInOut, null);
			case EaseType.EaseInElastic:
				return new EaseInfo(Elastic.EaseIn, Elastic.EaseOut);
			case EaseType.EaseOutElastic:
				return new EaseInfo(Elastic.EaseOut, Elastic.EaseIn);
			case EaseType.EaseInOutElastic:
				return new EaseInfo(Elastic.EaseInOut, null);
			case EaseType.EaseInBack:
				return new EaseInfo(Back.EaseIn, Back.EaseOut);
			case EaseType.EaseOutBack:
				return new EaseInfo(Back.EaseOut, Back.EaseIn);
			case EaseType.EaseInOutBack:
				return new EaseInfo(Back.EaseInOut, null);
			case EaseType.EaseInBounce:
				return new EaseInfo(Bounce.EaseIn, Bounce.EaseOut);
			case EaseType.EaseOutBounce:
				return new EaseInfo(Bounce.EaseOut, Bounce.EaseIn);
			case EaseType.EaseInOutBounce:
				return new EaseInfo(Bounce.EaseInOut, null);
			case EaseType.EaseInStrong:
				return new EaseInfo(Strong.EaseIn, Strong.EaseOut);
			case EaseType.EaseOutStrong:
				return new EaseInfo(Strong.EaseOut, Strong.EaseIn);
			case EaseType.EaseInOutStrong:
				return new EaseInfo(Strong.EaseInOut, null);
			default:
				return new EaseInfo(Linear.EaseNone, null);
			}
		}
	}
}
