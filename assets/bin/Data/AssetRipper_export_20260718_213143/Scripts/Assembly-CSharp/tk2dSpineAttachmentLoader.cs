using System;
using Spine;
using UnityEngine;

public class tk2dSpineAttachmentLoader : AttachmentLoader
{
	private tk2dSpriteCollectionData sprites;

	public tk2dSpineAttachmentLoader(tk2dSpriteCollectionData sprites)
	{
		if (sprites == null)
		{
			throw new ArgumentNullException("sprites cannot be null.");
		}
		this.sprites = sprites;
	}

	public Attachment NewAttachment(Skin skin, AttachmentType type, string name)
	{
		if (type != AttachmentType.region)
		{
			throw new Exception("Unknown attachment type: " + type);
		}
		int num = name.LastIndexOfAny(new char[2] { '/', '\\' });
		if (num != -1)
		{
			name = name.Substring(num + 1);
		}
		tk2dSpriteDefinition spriteDefinition = sprites.GetSpriteDefinition(name);
		if (spriteDefinition == null)
		{
			throw new Exception(string.Concat("Sprite not found in atlas: ", name, " (", type, ")"));
		}
		if (spriteDefinition.complexGeometry)
		{
			throw new NotImplementedException(string.Concat("Complex geometry is not supported: ", name, " (", type, ")"));
		}
		if (spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.TPackerCW)
		{
			throw new NotImplementedException(string.Concat("Only 2D Toolkit atlases are supported: ", name, " (", type, ")"));
		}
		RegionAttachment regionAttachment = new RegionAttachment(name);
		Vector2 lhs = Vector2.one;
		Vector2 lhs2 = Vector2.zero;
		for (int i = 0; i < spriteDefinition.uvs.Length; i++)
		{
			Vector2 rhs = spriteDefinition.uvs[i];
			lhs = Vector2.Min(lhs, rhs);
			lhs2 = Vector2.Max(lhs2, rhs);
		}
		bool flag = spriteDefinition.flipped == tk2dSpriteDefinition.FlipMode.Tk2d;
		if (flag)
		{
			float x = lhs.x;
			lhs.x = lhs2.x;
			lhs2.x = x;
		}
		regionAttachment.SetUVs(lhs.x, lhs2.y, lhs2.x, lhs.y, flag);
		regionAttachment.RegionOriginalWidth = (int)(spriteDefinition.untrimmedBoundsData[1].x / spriteDefinition.texelSize.x);
		regionAttachment.RegionOriginalHeight = (int)(spriteDefinition.untrimmedBoundsData[1].y / spriteDefinition.texelSize.y);
		regionAttachment.RegionWidth = (int)(spriteDefinition.boundsData[1].x / spriteDefinition.texelSize.x);
		regionAttachment.RegionHeight = (int)(spriteDefinition.boundsData[1].y / spriteDefinition.texelSize.y);
		float num2 = spriteDefinition.untrimmedBoundsData[0].x - spriteDefinition.untrimmedBoundsData[1].x / 2f;
		float num3 = spriteDefinition.boundsData[0].x - spriteDefinition.boundsData[1].x / 2f;
		regionAttachment.RegionOffsetX = (int)((num3 - num2) / spriteDefinition.texelSize.x);
		float num4 = spriteDefinition.untrimmedBoundsData[0].y - spriteDefinition.untrimmedBoundsData[1].y / 2f;
		float num5 = spriteDefinition.boundsData[0].y - spriteDefinition.boundsData[1].y / 2f;
		regionAttachment.RegionOffsetY = (int)((num5 - num4) / spriteDefinition.texelSize.y);
		return regionAttachment;
	}
}
