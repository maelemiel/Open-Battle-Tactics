using System;
using UnityEngine;

[Serializable]
public class tk2dCameraSettings
{
	public enum ProjectionType
	{
		Orthographic = 0,
		Perspective = 1
	}

	public enum OrthographicType
	{
		PixelsPerMeter = 0,
		OrthographicSize = 1
	}

	public enum OrthographicOrigin
	{
		BottomLeft = 0,
		Center = 1
	}

	public ProjectionType projection;

	public float orthographicSize = 10f;

	public float orthographicPixelsPerMeter = 100f;

	public OrthographicOrigin orthographicOrigin = OrthographicOrigin.Center;

	public OrthographicType orthographicType;

	public TransparencySortMode transparencySortMode;

	public float fieldOfView = 60f;

	public Rect rect = new Rect(0f, 0f, 1f, 1f);
}
