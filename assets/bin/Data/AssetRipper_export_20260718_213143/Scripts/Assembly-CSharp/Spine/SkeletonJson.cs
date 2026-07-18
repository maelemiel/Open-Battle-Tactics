using System;
using System.Collections.Generic;
using System.IO;

namespace Spine
{
	public class SkeletonJson
	{
		public static string TIMELINE_SCALE = "scale";

		public static string TIMELINE_ROTATE = "rotate";

		public static string TIMELINE_TRANSLATE = "translate";

		public static string TIMELINE_ATTACHMENT = "attachment";

		public static string TIMELINE_COLOR = "color";

		public static string ATTACHMENT_REGION = "region";

		public static string ATTACHMENT_REGION_SEQUENCE = "regionSequence";

		private AttachmentLoader attachmentLoader;

		public float Scale { get; set; }

		public SkeletonJson(Atlas atlas)
		{
			attachmentLoader = new AtlasAttachmentLoader(atlas);
			Scale = 1f;
		}

		public SkeletonJson(AttachmentLoader attachmentLoader)
		{
			if (attachmentLoader == null)
			{
				throw new ArgumentNullException("attachmentLoader cannot be null.");
			}
			this.attachmentLoader = attachmentLoader;
			Scale = 1f;
		}

		public SkeletonData ReadSkeletonData(string path)
		{
			using (StreamReader reader = new StreamReader(path))
			{
				SkeletonData skeletonData = ReadSkeletonData(reader);
				skeletonData.Name = Path.GetFileNameWithoutExtension(path);
				return skeletonData;
			}
		}

		public SkeletonData ReadSkeletonData(TextReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader cannot be null.");
			}
			SkeletonData skeletonData = new SkeletonData();
			Dictionary<string, object> dictionary = Json.Deserialize(reader) as Dictionary<string, object>;
			if (dictionary == null)
			{
				throw new Exception("Invalid JSON.");
			}
			foreach (Dictionary<string, object> item in (List<object>)dictionary["bones"])
			{
				BoneData boneData = null;
				if (item.ContainsKey("parent"))
				{
					boneData = skeletonData.FindBone((string)item["parent"]);
					if (boneData == null)
					{
						throw new Exception("Parent bone not found: " + item["parent"]);
					}
				}
				BoneData boneData2 = new BoneData((string)item["name"], boneData);
				boneData2.Length = GetFloat(item, "length", 0f) * Scale;
				boneData2.X = GetFloat(item, "x", 0f) * Scale;
				boneData2.Y = GetFloat(item, "y", 0f) * Scale;
				boneData2.Rotation = GetFloat(item, "rotation", 0f);
				boneData2.ScaleX = GetFloat(item, "scaleX", 1f);
				boneData2.ScaleY = GetFloat(item, "scaleY", 1f);
				boneData2.InheritScale = GetBoolean(item, "inheritScale", true);
				boneData2.InheritRotation = GetBoolean(item, "inheritRotation", true);
				skeletonData.AddBone(boneData2);
			}
			if (dictionary.ContainsKey("slots"))
			{
				List<object> list = (List<object>)dictionary["slots"];
				foreach (Dictionary<string, object> item2 in list)
				{
					string name = (string)item2["name"];
					string text = (string)item2["bone"];
					BoneData boneData3 = skeletonData.FindBone(text);
					if (boneData3 == null)
					{
						throw new Exception("Slot bone not found: " + text);
					}
					SlotData slotData = new SlotData(name, boneData3);
					if (item2.ContainsKey("color"))
					{
						string hexString = (string)item2["color"];
						slotData.R = ToColor(hexString, 0);
						slotData.G = ToColor(hexString, 1);
						slotData.B = ToColor(hexString, 2);
						slotData.A = ToColor(hexString, 3);
					}
					if (item2.ContainsKey("attachment"))
					{
						slotData.AttachmentName = (string)item2["attachment"];
					}
					skeletonData.AddSlot(slotData);
				}
			}
			if (dictionary.ContainsKey("skins"))
			{
				Dictionary<string, object> dictionary4 = (Dictionary<string, object>)dictionary["skins"];
				foreach (KeyValuePair<string, object> item3 in dictionary4)
				{
					Skin skin = new Skin(item3.Key);
					foreach (KeyValuePair<string, object> item4 in (Dictionary<string, object>)item3.Value)
					{
						int slotIndex = skeletonData.FindSlotIndex(item4.Key);
						foreach (KeyValuePair<string, object> item5 in (Dictionary<string, object>)item4.Value)
						{
							Attachment attachment = ReadAttachment(skin, item5.Key, (Dictionary<string, object>)item5.Value);
							skin.AddAttachment(slotIndex, item5.Key, attachment);
						}
					}
					skeletonData.AddSkin(skin);
					if (skin.Name == "default")
					{
						skeletonData.DefaultSkin = skin;
					}
				}
			}
			if (dictionary.ContainsKey("events"))
			{
				foreach (KeyValuePair<string, object> item6 in (Dictionary<string, object>)dictionary["events"])
				{
					Dictionary<string, object> map = (Dictionary<string, object>)item6.Value;
					EventData eventData = new EventData(item6.Key);
					eventData.Int = GetInt(map, "int", 0);
					eventData.Float = GetFloat(map, "float", 0f);
					eventData.String = GetString(map, "string", null);
					skeletonData.events.Add(eventData);
				}
			}
			if (dictionary.ContainsKey("animations"))
			{
				Dictionary<string, object> dictionary5 = (Dictionary<string, object>)dictionary["animations"];
				foreach (KeyValuePair<string, object> item7 in dictionary5)
				{
					ReadAnimation(item7.Key, (Dictionary<string, object>)item7.Value, skeletonData);
				}
			}
			skeletonData.Bones.TrimExcess();
			skeletonData.Slots.TrimExcess();
			skeletonData.Skins.TrimExcess();
			skeletonData.Animations.TrimExcess();
			return skeletonData;
		}

		private Attachment ReadAttachment(Skin skin, string name, Dictionary<string, object> map)
		{
			if (map.ContainsKey("name"))
			{
				name = (string)map["name"];
			}
			AttachmentType type = AttachmentType.region;
			if (map.ContainsKey("type"))
			{
				type = (AttachmentType)(int)Enum.Parse(typeof(AttachmentType), (string)map["type"], false);
			}
			Attachment attachment = attachmentLoader.NewAttachment(skin, type, name);
			if (attachment is RegionAttachment)
			{
				RegionAttachment regionAttachment = (RegionAttachment)attachment;
				regionAttachment.X = GetFloat(map, "x", 0f) * Scale;
				regionAttachment.Y = GetFloat(map, "y", 0f) * Scale;
				regionAttachment.ScaleX = GetFloat(map, "scaleX", 1f);
				regionAttachment.ScaleY = GetFloat(map, "scaleY", 1f);
				regionAttachment.Rotation = GetFloat(map, "rotation", 0f);
				regionAttachment.Width = GetFloat(map, "width", 32f) * Scale;
				regionAttachment.Height = GetFloat(map, "height", 32f) * Scale;
				regionAttachment.UpdateOffset();
			}
			return attachment;
		}

		private float[] GetFloatArray(Dictionary<string, object> map, string name, float scale)
		{
			List<object> list = (List<object>)map[name];
			float[] array = new float[list.Count];
			if (scale == 1f)
			{
				int i = 0;
				for (int count = list.Count; i < count; i++)
				{
					array[i] = (float)list[i];
				}
			}
			else
			{
				int j = 0;
				for (int count2 = list.Count; j < count2; j++)
				{
					array[j] = (float)list[j] * scale;
				}
			}
			return array;
		}

		private int[] GetIntArray(Dictionary<string, object> map, string name)
		{
			List<object> list = (List<object>)map[name];
			int[] array = new int[list.Count];
			int i = 0;
			for (int count = list.Count; i < count; i++)
			{
				array[i] = (int)(float)list[i];
			}
			return array;
		}

		private float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (float)map[name];
		}

		private int GetInt(Dictionary<string, object> map, string name, int defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (int)(float)map[name];
		}

		private bool GetBoolean(Dictionary<string, object> map, string name, bool defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (bool)map[name];
		}

		private string GetString(Dictionary<string, object> map, string name, string defaultValue)
		{
			if (!map.ContainsKey(name))
			{
				return defaultValue;
			}
			return (string)map[name];
		}

		public static float ToColor(string hexString, int colorIndex)
		{
			if (hexString.Length != 8)
			{
				throw new ArgumentException("Color hexidecimal length must be 8, recieved: " + hexString);
			}
			return (float)Convert.ToInt32(hexString.Substring(colorIndex * 2, 2), 16) / 255f;
		}

		private void ReadAnimation(string name, Dictionary<string, object> map, SkeletonData skeletonData)
		{
			List<Timeline> list = new List<Timeline>();
			float num = 0f;
			if (map.ContainsKey("bones"))
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)map["bones"];
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					string key = item.Key;
					int num2 = skeletonData.FindBoneIndex(key);
					if (num2 == -1)
					{
						throw new Exception("Bone not found: " + key);
					}
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)item.Value;
					foreach (KeyValuePair<string, object> item2 in dictionary2)
					{
						List<object> list2 = (List<object>)item2.Value;
						string key2 = item2.Key;
						if (key2.Equals(TIMELINE_ROTATE))
						{
							RotateTimeline rotateTimeline = new RotateTimeline(list2.Count);
							rotateTimeline.BoneIndex = num2;
							int num3 = 0;
							foreach (Dictionary<string, object> item3 in list2)
							{
								float time = (float)item3["time"];
								rotateTimeline.SetFrame(num3, time, (float)item3["angle"]);
								ReadCurve(rotateTimeline, num3, item3);
								num3++;
							}
							list.Add(rotateTimeline);
							num = Math.Max(num, rotateTimeline.Frames[rotateTimeline.FrameCount * 2 - 2]);
							continue;
						}
						if (key2.Equals(TIMELINE_TRANSLATE) || key2.Equals(TIMELINE_SCALE))
						{
							float num4 = 1f;
							TranslateTimeline translateTimeline;
							if (key2.Equals(TIMELINE_SCALE))
							{
								translateTimeline = new ScaleTimeline(list2.Count);
							}
							else
							{
								translateTimeline = new TranslateTimeline(list2.Count);
								num4 = Scale;
							}
							translateTimeline.BoneIndex = num2;
							int num5 = 0;
							foreach (Dictionary<string, object> item4 in list2)
							{
								float time2 = (float)item4["time"];
								float num6 = ((!item4.ContainsKey("x")) ? 0f : ((float)item4["x"]));
								float num7 = ((!item4.ContainsKey("y")) ? 0f : ((float)item4["y"]));
								translateTimeline.SetFrame(num5, time2, num6 * num4, num7 * num4);
								ReadCurve(translateTimeline, num5, item4);
								num5++;
							}
							list.Add(translateTimeline);
							num = Math.Max(num, translateTimeline.Frames[translateTimeline.FrameCount * 3 - 3]);
							continue;
						}
						throw new Exception("Invalid timeline type for a bone: " + key2 + " (" + key + ")");
					}
				}
			}
			if (map.ContainsKey("slots"))
			{
				Dictionary<string, object> dictionary5 = (Dictionary<string, object>)map["slots"];
				foreach (KeyValuePair<string, object> item5 in dictionary5)
				{
					string key3 = item5.Key;
					int slotIndex = skeletonData.FindSlotIndex(key3);
					Dictionary<string, object> dictionary6 = (Dictionary<string, object>)item5.Value;
					foreach (KeyValuePair<string, object> item6 in dictionary6)
					{
						List<object> list3 = (List<object>)item6.Value;
						string key4 = item6.Key;
						if (key4.Equals(TIMELINE_COLOR))
						{
							ColorTimeline colorTimeline = new ColorTimeline(list3.Count);
							colorTimeline.SlotIndex = slotIndex;
							int num8 = 0;
							foreach (Dictionary<string, object> item7 in list3)
							{
								float time3 = (float)item7["time"];
								string hexString = (string)item7["color"];
								colorTimeline.setFrame(num8, time3, ToColor(hexString, 0), ToColor(hexString, 1), ToColor(hexString, 2), ToColor(hexString, 3));
								ReadCurve(colorTimeline, num8, item7);
								num8++;
							}
							list.Add(colorTimeline);
							num = Math.Max(num, colorTimeline.Frames[colorTimeline.FrameCount * 5 - 5]);
							continue;
						}
						if (key4.Equals(TIMELINE_ATTACHMENT))
						{
							AttachmentTimeline attachmentTimeline = new AttachmentTimeline(list3.Count);
							attachmentTimeline.SlotIndex = slotIndex;
							int num9 = 0;
							foreach (Dictionary<string, object> item8 in list3)
							{
								float time4 = (float)item8["time"];
								attachmentTimeline.setFrame(num9++, time4, (string)item8["name"]);
							}
							list.Add(attachmentTimeline);
							num = Math.Max(num, attachmentTimeline.Frames[attachmentTimeline.FrameCount - 1]);
							continue;
						}
						throw new Exception("Invalid timeline type for a slot: " + key4 + " (" + key3 + ")");
					}
				}
			}
			if (map.ContainsKey("events"))
			{
				List<object> list4 = (List<object>)map["events"];
				EventTimeline eventTimeline = new EventTimeline(list4.Count);
				int num10 = 0;
				foreach (Dictionary<string, object> item9 in list4)
				{
					EventData eventData = skeletonData.FindEvent((string)item9["name"]);
					if (eventData == null)
					{
						throw new Exception("Event not found: " + item9["name"]);
					}
					Event obj = new Event(eventData);
					obj.Int = GetInt(item9, "int", eventData.Int);
					obj.Float = GetFloat(item9, "float", eventData.Float);
					obj.String = GetString(item9, "string", eventData.String);
					eventTimeline.setFrame(num10++, (float)item9["time"], obj);
				}
				list.Add(eventTimeline);
				num = Math.Max(num, eventTimeline.frames[eventTimeline.FrameCount - 1]);
			}
			list.TrimExcess();
			skeletonData.AddAnimation(new Animation(name, list, num));
		}

		private void ReadCurve(CurveTimeline timeline, int frameIndex, Dictionary<string, object> valueMap)
		{
			if (valueMap.ContainsKey("curve"))
			{
				object obj = valueMap["curve"];
				if (obj.Equals("stepped"))
				{
					timeline.SetStepped(frameIndex);
				}
				else if (obj is List<object>)
				{
					List<object> list = (List<object>)obj;
					timeline.SetCurve(frameIndex, (float)list[0], (float)list[1], (float)list[2], (float)list[3]);
				}
			}
		}
	}
}
