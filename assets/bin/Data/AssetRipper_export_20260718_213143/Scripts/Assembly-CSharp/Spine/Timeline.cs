using System.Collections.Generic;

namespace Spine
{
	public interface Timeline
	{
		void Apply(Skeleton skeleton, float lastTime, float time, List<Event> events, float alpha);
	}
}
