using System;

namespace Spine
{
	public class Slot
	{
		private Attachment attachment;

		private float attachmentTime;

		public SlotData Data { get; private set; }

		public Bone Bone { get; private set; }

		public Skeleton Skeleton { get; private set; }

		public float R { get; set; }

		public float G { get; set; }

		public float B { get; set; }

		public float A { get; set; }

		public Attachment Attachment
		{
			get
			{
				return attachment;
			}
			set
			{
				attachment = value;
				attachmentTime = Skeleton.Time;
			}
		}

		public float AttachmentTime
		{
			get
			{
				return Skeleton.Time - attachmentTime;
			}
			set
			{
				attachmentTime = Skeleton.Time - value;
			}
		}

		public Slot(SlotData data, Skeleton skeleton, Bone bone)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data cannot be null.");
			}
			if (skeleton == null)
			{
				throw new ArgumentNullException("skeleton cannot be null.");
			}
			if (bone == null)
			{
				throw new ArgumentNullException("bone cannot be null.");
			}
			Data = data;
			Skeleton = skeleton;
			Bone = bone;
			SetToSetupPose();
		}

		internal void SetToSetupPose(int slotIndex)
		{
			R = Data.R;
			G = Data.G;
			B = Data.B;
			A = Data.A;
			Attachment = ((Data.AttachmentName != null) ? Skeleton.GetAttachment(slotIndex, Data.AttachmentName) : null);
		}

		public void SetToSetupPose()
		{
			SetToSetupPose(Skeleton.Data.Slots.IndexOf(Data));
		}

		public override string ToString()
		{
			return Data.Name;
		}
	}
}
