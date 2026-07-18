using System;
using System.Collections.Generic;

namespace Spine
{
	public class SkeletonData
	{
		internal List<EventData> events = new List<EventData>();

		public Skin DefaultSkin;

		public string Name { get; set; }

		public List<BoneData> Bones { get; private set; }

		public List<SlotData> Slots { get; private set; }

		public List<EventData> Events
		{
			get
			{
				return events;
			}
			set
			{
				events = value;
			}
		}

		public List<Skin> Skins { get; private set; }

		public List<Animation> Animations { get; private set; }

		public SkeletonData()
		{
			Bones = new List<BoneData>();
			Slots = new List<SlotData>();
			Skins = new List<Skin>();
			Animations = new List<Animation>();
		}

		public void AddBone(BoneData bone)
		{
			if (bone == null)
			{
				throw new ArgumentNullException("bone cannot be null.");
			}
			Bones.Add(bone);
		}

		public BoneData FindBone(string boneName)
		{
			if (boneName == null)
			{
				throw new ArgumentNullException("boneName cannot be null.");
			}
			int i = 0;
			for (int count = Bones.Count; i < count; i++)
			{
				BoneData boneData = Bones[i];
				if (boneData.Name == boneName)
				{
					return boneData;
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
			int i = 0;
			for (int count = Bones.Count; i < count; i++)
			{
				if (Bones[i].Name == boneName)
				{
					return i;
				}
			}
			return -1;
		}

		public void AddSlot(SlotData slot)
		{
			if (slot == null)
			{
				throw new ArgumentNullException("slot cannot be null.");
			}
			Slots.Add(slot);
		}

		public SlotData FindSlot(string slotName)
		{
			if (slotName == null)
			{
				throw new ArgumentNullException("slotName cannot be null.");
			}
			int i = 0;
			for (int count = Slots.Count; i < count; i++)
			{
				SlotData slotData = Slots[i];
				if (slotData.Name == slotName)
				{
					return slotData;
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
			int i = 0;
			for (int count = Slots.Count; i < count; i++)
			{
				if (Slots[i].Name == slotName)
				{
					return i;
				}
			}
			return -1;
		}

		public void AddSkin(Skin skin)
		{
			if (skin == null)
			{
				throw new ArgumentNullException("skin cannot be null.");
			}
			Skins.Add(skin);
		}

		public Skin FindSkin(string skinName)
		{
			if (skinName == null)
			{
				throw new ArgumentNullException("skinName cannot be null.");
			}
			foreach (Skin skin in Skins)
			{
				if (skin.Name == skinName)
				{
					return skin;
				}
			}
			return null;
		}

		public EventData FindEvent(string eventDataName)
		{
			if (eventDataName == null)
			{
				throw new ArgumentNullException("eventDataName cannot be null.");
			}
			foreach (EventData @event in events)
			{
				if (@event.name == eventDataName)
				{
					return @event;
				}
			}
			return null;
		}

		public void AddAnimation(Animation animation)
		{
			if (animation == null)
			{
				throw new ArgumentNullException("animation cannot be null.");
			}
			Animations.Add(animation);
		}

		public Animation FindAnimation(string animationName)
		{
			if (animationName == null)
			{
				throw new ArgumentNullException("animationName cannot be null.");
			}
			int i = 0;
			for (int count = Animations.Count; i < count; i++)
			{
				Animation animation = Animations[i];
				if (animation.Name == animationName)
				{
					return animation;
				}
			}
			return null;
		}

		public override string ToString()
		{
			return Name ?? base.ToString();
		}
	}
}
