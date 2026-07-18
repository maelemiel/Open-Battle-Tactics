using System;

namespace Spine
{
	public class AtlasAttachmentLoader : AttachmentLoader
	{
		private Atlas atlas;

		public AtlasAttachmentLoader(Atlas atlas)
		{
			if (atlas == null)
			{
				throw new ArgumentNullException("atlas cannot be null.");
			}
			this.atlas = atlas;
		}

		public Attachment NewAttachment(Skin skin, AttachmentType type, string name)
		{
			if (type == AttachmentType.region)
			{
				AtlasRegion atlasRegion = atlas.FindRegion(name);
				if (atlasRegion == null)
				{
					throw new Exception(string.Concat("Region not found in atlas: ", name, " (", type, ")"));
				}
				RegionAttachment regionAttachment = new RegionAttachment(name);
				regionAttachment.RendererObject = atlasRegion.page.rendererObject;
				regionAttachment.SetUVs(atlasRegion.u, atlasRegion.v, atlasRegion.u2, atlasRegion.v2, atlasRegion.rotate);
				regionAttachment.RegionOffsetX = atlasRegion.offsetX;
				regionAttachment.RegionOffsetY = atlasRegion.offsetY;
				regionAttachment.RegionWidth = atlasRegion.width;
				regionAttachment.RegionHeight = atlasRegion.height;
				regionAttachment.RegionOriginalWidth = atlasRegion.originalWidth;
				regionAttachment.RegionOriginalHeight = atlasRegion.originalHeight;
				return regionAttachment;
			}
			throw new Exception("Unknown attachment type: " + type);
		}
	}
}
