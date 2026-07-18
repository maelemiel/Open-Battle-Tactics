using System.Collections.Generic;

namespace Spine
{
	public class AttachmentTimeline : Timeline
	{
		public int SlotIndex { get; set; }

		public float[] Frames { get; private set; }

		public string[] AttachmentNames { get; private set; }

		public int FrameCount
		{
			get
			{
				return Frames.Length;
			}
		}

		public AttachmentTimeline(int frameCount)
		{
			Frames = new float[frameCount];
			AttachmentNames = new string[frameCount];
		}

		public void setFrame(int frameIndex, float time, string attachmentName)
		{
			Frames[frameIndex] = time;
			AttachmentNames[frameIndex] = attachmentName;
		}

		public void Apply(Skeleton skeleton, float lastTime, float time, List<Event> firedEvents, float alpha)
		{
			float[] frames = Frames;
			if (!(time < frames[0]))
			{
				int num = ((!(time >= frames[frames.Length - 1])) ? (Animation.binarySearch(frames, time, 1) - 1) : (frames.Length - 1));
				string text = AttachmentNames[num];
				skeleton.Slots[SlotIndex].Attachment = ((text != null) ? skeleton.GetAttachment(SlotIndex, text) : null);
			}
		}
	}
}
