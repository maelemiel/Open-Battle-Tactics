using System;
using System.IO;
using Spine;
using UnityEngine;

public class tk2dSpineSkeletonDataAsset : ScriptableObject
{
	public tk2dSpriteCollectionData spritesData;

	public tk2dSpriteCollection.NormalGenerationMode normalGenerationMode;

	public TextAsset skeletonJSON;

	public string[] fromAnimation;

	public string[] toAnimation;

	public float[] duration;

	private SkeletonData skeletonData;

	private AnimationStateData stateData;

	public SkeletonData GetSkeletonData()
	{
		if (skeletonData != null)
		{
			return skeletonData;
		}
		MakeSkeletonAndAnimationData();
		return skeletonData;
	}

	public AnimationStateData GetAnimationStateData()
	{
		if (stateData != null)
		{
			return stateData;
		}
		MakeSkeletonAndAnimationData();
		return stateData;
	}

	private void MakeSkeletonAndAnimationData()
	{
		if (spritesData == null)
		{
			Debug.LogWarning("Sprite collection not set for skeleton data asset: " + base.name, this);
			return;
		}
		if (skeletonJSON == null)
		{
			Debug.LogWarning("Skeleton JSON file not set for skeleton data asset: " + base.name, this);
			return;
		}
		SkeletonJson skeletonJson = new SkeletonJson(new tk2dSpineAttachmentLoader(spritesData));
		skeletonJson.Scale = 1f / (spritesData.invOrthoSize * spritesData.halfTargetHeight);
		try
		{
			skeletonData = skeletonJson.ReadSkeletonData(new StringReader(skeletonJSON.text));
		}
		catch (Exception ex)
		{
			Debug.Log("Error reading skeleton JSON file for skeleton data asset: " + base.name + "\n" + ex.Message + "\n" + ex.StackTrace, this);
			return;
		}
		stateData = new AnimationStateData(skeletonData);
		int i = 0;
		for (int num = fromAnimation.Length; i < num; i++)
		{
			if (fromAnimation[i].Length != 0 && toAnimation[i].Length != 0)
			{
				stateData.SetMix(fromAnimation[i], toAnimation[i], duration[i]);
			}
		}
	}

	public void ForceUpdate()
	{
		MakeSkeletonAndAnimationData();
	}
}
