using System.Collections.Generic;

public class MediaTypes
{
	private Dictionary<string, MediaType> _mediaTypes = new Dictionary<string, MediaType> { 
	{
		"application/octet-stream",
		new BinaryDownloadMediaType("/tmp")
	} };

	public bool IsSupported(string contentType)
	{
		return _mediaTypes.ContainsKey(contentType);
	}

	public MediaType valueOf(string type)
	{
		if (IsSupported(type))
		{
			return _mediaTypes[type];
		}
		return null;
	}

	public string ContentTypeFromHTTPFormat(string type)
	{
		return type.Split(';')[0];
	}

	public void SetMediaType(string key, MediaType mediaType)
	{
		_mediaTypes[key] = mediaType;
	}
}
