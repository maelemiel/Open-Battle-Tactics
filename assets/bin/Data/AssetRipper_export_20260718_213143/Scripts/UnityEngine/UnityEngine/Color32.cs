namespace UnityEngine
{
	public struct Color32
	{
		public byte r;

		public byte g;

		public byte b;

		public byte a;

		public Color32(byte r, byte g, byte b, byte a)
		{
			this.r = r;
			this.g = g;
			this.b = b;
			this.a = a;
		}

		public override string ToString()
		{
			return UnityString.Format("RGBA({0}, {1}, {2}, {3})", r, g, b, a);
		}

		public string ToString(string format)
		{
			return UnityString.Format("RGBA({0}, {1}, {2}, {3})", r.ToString(format), g.ToString(format), b.ToString(format), a.ToString(format));
		}

		public static Color32 Lerp(Color32 a, Color32 b, float t)
		{
			t = Mathf.Clamp01(t);
			return new Color32((byte)((float)(int)a.r + (float)(b.r - a.r) * t), (byte)((float)(int)a.g + (float)(b.g - a.g) * t), (byte)((float)(int)a.b + (float)(b.b - a.b) * t), (byte)((float)(int)a.a + (float)(b.a - a.a) * t));
		}

		public static implicit operator Color32(Color c)
		{
			return new Color32((byte)(Mathf.Clamp01(c.r) * 255f), (byte)(Mathf.Clamp01(c.g) * 255f), (byte)(Mathf.Clamp01(c.b) * 255f), (byte)(Mathf.Clamp01(c.a) * 255f));
		}

		public static implicit operator Color(Color32 c)
		{
			return new Color((float)(int)c.r / 255f, (float)(int)c.g / 255f, (float)(int)c.b / 255f, (float)(int)c.a / 255f);
		}
	}
}
