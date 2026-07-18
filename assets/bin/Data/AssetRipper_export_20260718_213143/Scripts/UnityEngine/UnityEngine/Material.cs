using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class Material : Object
	{
		public extern Shader shader
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Color color
		{
			get
			{
				return GetColor("_Color");
			}
			set
			{
				SetColor("_Color", value);
			}
		}

		public Texture mainTexture
		{
			get
			{
				return GetTexture("_MainTex");
			}
			set
			{
				SetTexture("_MainTex", value);
			}
		}

		public Vector2 mainTextureOffset
		{
			get
			{
				return GetTextureOffset("_MainTex");
			}
			set
			{
				SetTextureOffset("_MainTex", value);
			}
		}

		public Vector2 mainTextureScale
		{
			get
			{
				return GetTextureScale("_MainTex");
			}
			set
			{
				SetTextureScale("_MainTex", value);
			}
		}

		public extern int passCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int renderQueue
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern string[] shaderKeywords
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Material(string contents)
		{
			Internal_CreateWithString(this, contents);
		}

		public Material(Shader shader)
		{
			Internal_CreateWithShader(this, shader);
		}

		public Material(Material source)
		{
			Internal_CreateWithMaterial(this, source);
		}

		public void SetColor(string propertyName, Color color)
		{
			SetColor(Shader.PropertyToID(propertyName), color);
		}

		public void SetColor(int nameID, Color color)
		{
			INTERNAL_CALL_SetColor(this, nameID, ref color);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetColor(Material self, int nameID, ref Color color);

		public Color GetColor(string propertyName)
		{
			return GetColor(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color GetColor(int nameID);

		public void SetVector(string propertyName, Vector4 vector)
		{
			SetColor(propertyName, new Color(vector.x, vector.y, vector.z, vector.w));
		}

		public void SetVector(int nameID, Vector4 vector)
		{
			SetColor(nameID, new Color(vector.x, vector.y, vector.z, vector.w));
		}

		public Vector4 GetVector(string propertyName)
		{
			Color color = GetColor(propertyName);
			return new Vector4(color.r, color.g, color.b, color.a);
		}

		public Vector4 GetVector(int nameID)
		{
			Color color = GetColor(nameID);
			return new Vector4(color.r, color.g, color.b, color.a);
		}

		public void SetTexture(string propertyName, Texture texture)
		{
			SetTexture(Shader.PropertyToID(propertyName), texture);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetTexture(int nameID, Texture texture);

		public Texture GetTexture(string propertyName)
		{
			return GetTexture(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Texture GetTexture(int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_GetTextureOffset(Material mat, string name, out Vector2 output);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_GetTextureScale(Material mat, string name, out Vector2 output);

		public void SetTextureOffset(string propertyName, Vector2 offset)
		{
			INTERNAL_CALL_SetTextureOffset(this, propertyName, ref offset);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetTextureOffset(Material self, string propertyName, ref Vector2 offset);

		public Vector2 GetTextureOffset(string propertyName)
		{
			Vector2 output;
			Internal_GetTextureOffset(this, propertyName, out output);
			return output;
		}

		public void SetTextureScale(string propertyName, Vector2 scale)
		{
			INTERNAL_CALL_SetTextureScale(this, propertyName, ref scale);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetTextureScale(Material self, string propertyName, ref Vector2 scale);

		public Vector2 GetTextureScale(string propertyName)
		{
			Vector2 output;
			Internal_GetTextureScale(this, propertyName, out output);
			return output;
		}

		public void SetMatrix(string propertyName, Matrix4x4 matrix)
		{
			SetMatrix(Shader.PropertyToID(propertyName), matrix);
		}

		public void SetMatrix(int nameID, Matrix4x4 matrix)
		{
			INTERNAL_CALL_SetMatrix(this, nameID, ref matrix);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetMatrix(Material self, int nameID, ref Matrix4x4 matrix);

		public Matrix4x4 GetMatrix(string propertyName)
		{
			return GetMatrix(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Matrix4x4 GetMatrix(int nameID);

		public void SetFloat(string propertyName, float value)
		{
			SetFloat(Shader.PropertyToID(propertyName), value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetFloat(int nameID, float value);

		public float GetFloat(string propertyName)
		{
			return GetFloat(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern float GetFloat(int nameID);

		public void SetInt(string propertyName, int value)
		{
			SetFloat(propertyName, value);
		}

		public void SetInt(int nameID, int value)
		{
			SetFloat(nameID, value);
		}

		public int GetInt(string propertyName)
		{
			return (int)GetFloat(propertyName);
		}

		public int GetInt(int nameID)
		{
			return (int)GetFloat(nameID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetBuffer(string propertyName, ComputeBuffer buffer);

		public bool HasProperty(string propertyName)
		{
			return HasProperty(Shader.PropertyToID(propertyName));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool HasProperty(int nameID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern string GetTag(string tag, bool searchFallbacks, [DefaultValue("\"\"")] string defaultValue);

		[ExcludeFromDocs]
		public string GetTag(string tag, bool searchFallbacks)
		{
			string empty = string.Empty;
			return GetTag(tag, searchFallbacks, empty);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Lerp(Material start, Material end, float t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool SetPass(int pass);

		[Obsolete("Use the Material constructor instead.")]
		public static Material Create(string scriptContents)
		{
			return new Material(scriptContents);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateWithString([Writable] Material mono, string contents);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateWithShader([Writable] Material mono, Shader shader);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateWithMaterial([Writable] Material mono, Material source);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void CopyPropertiesFromMaterial(Material mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void EnableKeyword(string keyword);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void DisableKeyword(string keyword);
	}
}
