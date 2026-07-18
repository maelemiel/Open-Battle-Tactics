using System;
using System.Collections.Generic;

namespace Spine
{
	public class Skeleton
	{
		public SkeletonData Data { get; private set; }

		public List<Bone> Bones { get; private set; }

		public List<Slot> Slots { get; private set; }

		public List<Slot> DrawOrder { get; private set; }

		public Skin Skin { get; set; }

		public float R { get; set; }

		public float G { get; set; }

		public float B { get; set; }

		public float A { get; set; }

		public float Time { get; set; }

		public bool FlipX { get; set; }

		public bool FlipY { get; set; }

		public Bone RootBone
		{
			get
			{
				return (Bones.Count != 0) ? Bones[0] : null;
			}
		}

		public float X { get; set; }

		public float Y { get; set; }

		public Skeleton(SkeletonData data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data cannot be null.");
			}
			Data = data;
			Bones = new List<Bone>(Data.Bones.Count);
			foreach (BoneData bone2 in Data.Bones)
			{
				Bone parent = ((bone2.Parent != null) ? Bones[Data.Bones.IndexOf(bone2.Parent)] : null);
				Bones.Add(new Bone(bone2, parent));
			}
			Slots = new List<Slot>(Data.Slots.Count);
			DrawOrder = new List<Slot>(Data.Slots.Count);
			foreach (SlotData slot in Data.Slots)
			{
				Bone bone = Bones[Data.Bones.IndexOf(slot.BoneData)];
				Slot item = new Slot(slot, this, bone);
				Slots.Add(item);
				DrawOrder.Add(item);
			}
			R = 1f;
			G = 1f;
			B = 1f;
			A = 1f;
		}

		public void UpdateWorldTransform()
		{
			bool flipX = FlipX;
			bool flipY = FlipY;
			List<Bone> bones = Bones;
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				bones[i].UpdateWorldTransform(flipX, flipY);
			}
		}

		public void SetToSetupPose()
		{
			SetBonesToSetupPose();
			SetSlotsToSetupPose();
		}

		public void SetBonesToSetupPose()
		{
			List<Bone> bones = Bones;
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				bones[i].SetToSetupPose();
			}
		}

		public void SetSlotsToSetupPose()
		{
			List<Slot> slots = Slots;
			int i = 0;
			for (int count = slots.Count; i < count; i++)
			{
				slots[i].SetToSetupPose(i);
			}
		}

		public Bone FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName cannot be null.");
			}
			List<Bone> bones = Bones;
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				Bone bone = bones[i];
				if (bone.Data.Name == boneName)
				{
					return bone;
				}
			}
			return null;
		}

		public int FindBoneIndex(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName cannot be null.");
			}
			List<Bone> bones = Bones;
			int i = 0;
			for (int count = bones.Count; i < count; i++)
			{
				if (bones[i].Data.Name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public Slot FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			List<Slot> slots = Slots;
			int i = 0;
			for (int count = slots.Count; i < count; i++)
			{
				Slot slot = slots[i];
				if (slot.Data.Name == slotName)
				{
					return slot;
				}
			}
			return null;
		}

		public int FindSlotIndex(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			List<Slot> slots = Slots;
			int i = 0;
			for (int count = slots.Count; i < count; i++)
			{
				if (slots[i].Data.Name.Equals(slotName))
				{
					return i;
				}
			}
			return -1;
		}

		public void SetSkin(string skinName)
		{
			Skin skin = Data.FindSkin(skinName);
			if (skin == null)
			{
				throw new ArgumentException("Skin not found: " + skinName);
			}
			SetSkin(skin);
		}

		public void SetSkin(Skin newSkin)
		{
			if (Skin != null && newSkin != null)
			{
				newSkin.AttachAll(this, Skin);
			}
			Skin = newSkin;
		}

		public Attachment GetAttachment(string slotName, string attachmentName)
		{
			return GetAttachment(Data.FindSlotIndex(slotName), attachmentName);
		}

		public Attachment GetAttachment(int slotIndex, string attachmentName)
		{
			if (attachmentName == null)
			{
				throw new ArgumentNullException("attachmentName cannot be null.");
			}
			if (Skin != null)
			{
				Attachment attachment = Skin.GetAttachment(slotIndex, attachmentName);
				if (attachment != null)
				{
					return attachment;
				}
			}
			if (Data.DefaultSkin != null)
			{
				return Data.DefaultSkin.GetAttachment(slotIndex, attachmentName);
			}
			return null;
		}

		public void SetAttachment(string slotName, string attachmentName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			List<Slot> slots = Slots;
			int i = 0;
			for (int count = slots.Count; i < count; i++)
			{
				Slot slot = slots[i];
				if (!(slot.Data.Name == slotName))
				{
					continue;
				}
				Attachment attachment = null;
				if (attachmentName != null)
				{
					attachment = GetAttachment(i, attachmentName);
					if (attachment == null)
					{
						throw new ArgumentNullException("Attachment not found: " + attachmentName + ", for slot: " + slotName);
					}
				}
				slot.Attachment = attachment;
				return;
			}
			throw new Exception("Slot not found: " + slotName);
		}

		public void Update(float delta)
		{
			Time += delta;
		}
	}
}
