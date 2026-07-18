using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Graphics
	{
		public static RenderBuffer activeColorBuffer
		{
			get
			{
				RenderBuffer res;
				GetActiveColorBuffer(out res);
				return res;
			}
		}

		public static RenderBuffer activeDepthBuffer
		{
			get
			{
				RenderBuffer res;
				GetActiveDepthBuffer(out res);
				return res;
			}
		}

		[Obsolete("Use SystemInfo.graphicsDeviceName instead.")]
		public static string deviceName
		{
			get
			{
				return SystemInfo.graphicsDeviceName;
			}
		}

		[Obsolete("Use SystemInfo.graphicsDeviceVendor instead.")]
		public static string deviceVendor
		{
			get
			{
				return SystemInfo.graphicsDeviceVendor;
			}
		}

		[Obsolete("Use SystemInfo.graphicsDeviceVersion instead.")]
		public static string deviceVersion
		{
			get
			{
				return SystemInfo.graphicsDeviceVersion;
			}
		}

		[Obsolete("Use SystemInfo.supportsVertexPrograms instead.")]
		public static bool supportsVertexProgram
		{
			get
			{
				return SystemInfo.supportsVertexPrograms;
			}
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
		{
			MaterialPropertyBlock properties = null;
			DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
		{
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
		{
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			Camera camera = null;
			DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties);
		}

		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties)
		{
			Internal_DrawMeshTRArguments arguments = new Internal_DrawMeshTRArguments
			{
				position = position,
				rotation = rotation,
				layer = layer,
				submeshIndex = submeshIndex,
				castShadows = 1,
				receiveShadows = 1
			};
			Internal_DrawMeshTR(ref arguments, properties, material, mesh, camera);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
		{
			MaterialPropertyBlock properties = null;
			DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
		{
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties);
		}

		[ExcludeFromDocs]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
		{
			MaterialPropertyBlock properties = null;
			int submeshIndex = 0;
			Camera camera = null;
			DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties);
		}

		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties)
		{
			Internal_DrawMeshMatrixArguments arguments = new Internal_DrawMeshMatrixArguments
			{
				matrix = matrix,
				layer = layer,
				submeshIndex = submeshIndex,
				castShadows = 1,
				receiveShadows = 1
			};
			Internal_DrawMeshMatrix(ref arguments, properties, material, mesh, camera);
		}

		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			Internal_DrawMeshTRArguments arguments = new Internal_DrawMeshTRArguments
			{
				position = position,
				rotation = rotation,
				layer = layer,
				submeshIndex = submeshIndex,
				castShadows = (castShadows ? 1 : 0),
				receiveShadows = (receiveShadows ? 1 : 0)
			};
			Internal_DrawMeshTR(ref arguments, properties, material, mesh, camera);
		}

		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows, bool receiveShadows)
		{
			Internal_DrawMeshMatrixArguments arguments = new Internal_DrawMeshMatrixArguments
			{
				matrix = matrix,
				layer = layer,
				submeshIndex = submeshIndex,
				castShadows = (castShadows ? 1 : 0),
				receiveShadows = (receiveShadows ? 1 : 0)
			};
			Internal_DrawMeshMatrix(ref arguments, properties, material, mesh, camera);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_DrawMeshTR(ref Internal_DrawMeshTRArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_DrawMeshMatrix(ref Internal_DrawMeshMatrixArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);

		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
		{
			Internal_DrawMeshNow1(mesh, position, rotation, -1);
		}

		public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
			Internal_DrawMeshNow1(mesh, position, rotation, materialIndex);
		}

		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
		{
			Internal_DrawMeshNow2(mesh, matrix, -1);
		}

		public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
			Internal_DrawMeshNow2(mesh, matrix, materialIndex);
		}

		private static void Internal_DrawMeshNow1(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
			INTERNAL_CALL_Internal_DrawMeshNow1(mesh, ref position, ref rotation, materialIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Internal_DrawMeshNow1(Mesh mesh, ref Vector3 position, ref Quaternion rotation, int materialIndex);

		private static void Internal_DrawMeshNow2(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
			INTERNAL_CALL_Internal_DrawMeshNow2(mesh, ref matrix, materialIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Internal_DrawMeshNow2(Mesh mesh, ref Matrix4x4 matrix, int materialIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void DrawProcedural(MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount);

		[ExcludeFromDocs]
		public static void DrawProcedural(MeshTopology topology, int vertexCount)
		{
			int instanceCount = 1;
			DrawProcedural(topology, vertexCount, instanceCount);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset);

		[ExcludeFromDocs]
		public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs)
		{
			int argsOffset = 0;
			DrawProceduralIndirect(topology, bufferWithArgs, argsOffset);
		}

		[Obsolete("Use Graphics.DrawMeshNow instead.")]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
		{
			Internal_DrawMeshNow1(mesh, position, rotation, -1);
		}

		[Obsolete("Use Graphics.DrawMeshNow instead.")]
		public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
		{
			Internal_DrawMeshNow1(mesh, position, rotation, materialIndex);
		}

		[Obsolete("Use Graphics.DrawMeshNow instead.")]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix)
		{
			Internal_DrawMeshNow2(mesh, matrix, -1);
		}

		[Obsolete("Use Graphics.DrawMeshNow instead.")]
		public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, int materialIndex)
		{
			Internal_DrawMeshNow2(mesh, matrix, materialIndex);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture)
		{
			Material mat = null;
			DrawTexture(screenRect, texture, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat)
		{
			DrawTexture(screenRect, texture, 0, 0, 0, 0, mat);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Material mat = null;
			DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
		{
			DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Material mat = null;
			DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
		{
			InternalDrawTextureArguments arguments = new InternalDrawTextureArguments
			{
				screenRect = screenRect,
				texture = texture,
				sourceRect = sourceRect,
				leftBorder = leftBorder,
				rightBorder = rightBorder,
				topBorder = topBorder,
				bottomBorder = bottomBorder
			};
			Color32 color = default(Color32);
			color.r = (color.g = (color.b = (color.a = 128)));
			arguments.color = color;
			arguments.mat = mat;
			DrawTexture(ref arguments);
		}

		[ExcludeFromDocs]
		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
		{
			Material mat = null;
			DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat);
		}

		public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [DefaultValue("null")] Material mat)
		{
			InternalDrawTextureArguments arguments = new InternalDrawTextureArguments
			{
				screenRect = screenRect,
				texture = texture,
				sourceRect = sourceRect,
				leftBorder = leftBorder,
				rightBorder = rightBorder,
				topBorder = topBorder,
				bottomBorder = bottomBorder,
				color = color,
				mat = mat
			};
			DrawTexture(ref arguments);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void DrawTexture(ref InternalDrawTextureArguments arguments);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void Blit(Texture source, RenderTexture dest);

		[ExcludeFromDocs]
		public static void Blit(Texture source, RenderTexture dest, Material mat)
		{
			int pass = -1;
			Blit(source, dest, mat, pass);
		}

		public static void Blit(Texture source, RenderTexture dest, Material mat, [DefaultValue("-1")] int pass)
		{
			Internal_BlitMaterial(source, dest, mat, pass, true);
		}

		[ExcludeFromDocs]
		public static void Blit(Texture source, Material mat)
		{
			int pass = -1;
			Blit(source, mat, pass);
		}

		public static void Blit(Texture source, Material mat, [DefaultValue("-1")] int pass)
		{
			Internal_BlitMaterial(source, null, mat, pass, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_BlitMaterial(Texture source, RenderTexture dest, Material mat, int pass, bool setRT);

		public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
		{
			Internal_BlitMultiTap(source, dest, mat, offsets);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_BlitMultiTap(Texture source, RenderTexture dest, Material mat, Vector2[] offsets);

		public static void SetRenderTarget(RenderTexture rt)
		{
			Internal_SetRT(rt, 0, -1);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel)
		{
			Internal_SetRT(rt, mipLevel, -1);
		}

		public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
		{
			Internal_SetRT(rt, mipLevel, (int)face);
		}

		public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			Internal_SetRTBuffer(out colorBuffer, out depthBuffer);
		}

		public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
		{
			Internal_SetRTBuffers(colorBuffers, out depthBuffer);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetRT(RenderTexture rt, int mipLevel, int face);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetRTBuffer(out RenderBuffer colorBuffer, out RenderBuffer depthBuffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetRTBuffers(RenderBuffer[] colorBuffers, out RenderBuffer depthBuffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void GetActiveColorBuffer(out RenderBuffer res);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void GetActiveDepthBuffer(out RenderBuffer res);

		public static void SetRandomWriteTarget(int index, RenderTexture uav)
		{
			Internal_SetRandomWriteTargetRT(index, uav);
		}

		public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
		{
			Internal_SetRandomWriteTargetBuffer(index, uav);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void ClearRandomWriteTargets();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void SetupVertexLights(Light[] lights);
	}
}
