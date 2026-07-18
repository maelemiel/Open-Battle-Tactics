using System;
using UnityEngine;

[Serializable]
public class tk2dSpriteColliderDefinition
{
	public enum Type
	{
		Box = 0,
		Circle = 1
	}

	public Type type;

	public Vector3 origin;

	public float angle;

	public string name = string.Empty;

	public Vector3[] vectors = new Vector3[0];

	public float[] floats = new float[0];

	public float Radius
	{
		get
		{
			return (type != Type.Circle) ? 0f : floats[0];
		}
	}

	public Vector3 Size
	{
		get
		{
			return (type != Type.Box) ? Vector3.zero : vectors[0];
		}
	}

	public tk2dSpriteColliderDefinition(Type type, Vector3 origin, float angle)
	{
		this.type = type;
		this.origin = origin;
		this.angle = angle;
	}
}
