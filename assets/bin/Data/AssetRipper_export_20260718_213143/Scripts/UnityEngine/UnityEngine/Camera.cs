using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Camera : Behaviour
	{
		[Obsolete("use Camera.fieldOfView instead.")]
		public float fov
		{
			get
			{
				return fieldOfView;
			}
			set
			{
				fieldOfView = value;
			}
		}

		[Obsolete("use Camera.nearClipPlane instead.")]
		public float near
		{
			get
			{
				return nearClipPlane;
			}
			set
			{
				nearClipPlane = value;
			}
		}

		[Obsolete("use Camera.farClipPlane instead.")]
		public float far
		{
			get
			{
				return farClipPlane;
			}
			set
			{
				farClipPlane = value;
			}
		}

		public extern float fieldOfView
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float nearClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float farClipPlane
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern RenderingPath renderingPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern RenderingPath actualRenderingPath
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool hdr
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float orthographicSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool orthographic
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern TransparencySortMode transparencySortMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public bool isOrthoGraphic
		{
			get
			{
				return orthographic;
			}
			set
			{
				orthographic = value;
			}
		}

		public extern float depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float aspect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int cullingMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int eventMask
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Color backgroundColor
		{
			get
			{
				Color value;
				INTERNAL_get_backgroundColor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_backgroundColor(ref value);
			}
		}

		public Rect rect
		{
			get
			{
				Rect value;
				INTERNAL_get_rect(out value);
				return value;
			}
			set
			{
				INTERNAL_set_rect(ref value);
			}
		}

		public Rect pixelRect
		{
			get
			{
				Rect value;
				INTERNAL_get_pixelRect(out value);
				return value;
			}
			set
			{
				INTERNAL_set_pixelRect(ref value);
			}
		}

		public extern RenderTexture targetTexture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float pixelWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float pixelHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Matrix4x4 cameraToWorldMatrix
		{
			get
			{
				Matrix4x4 value;
				INTERNAL_get_cameraToWorldMatrix(out value);
				return value;
			}
		}

		public Matrix4x4 worldToCameraMatrix
		{
			get
			{
				Matrix4x4 value;
				INTERNAL_get_worldToCameraMatrix(out value);
				return value;
			}
			set
			{
				INTERNAL_set_worldToCameraMatrix(ref value);
			}
		}

		public Matrix4x4 projectionMatrix
		{
			get
			{
				Matrix4x4 value;
				INTERNAL_get_projectionMatrix(out value);
				return value;
			}
			set
			{
				INTERNAL_set_projectionMatrix(ref value);
			}
		}

		public Vector3 velocity
		{
			get
			{
				Vector3 value;
				INTERNAL_get_velocity(out value);
				return value;
			}
		}

		public extern CameraClearFlags clearFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool stereoEnabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float stereoSeparation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float stereoConvergence
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern Camera main
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern Camera current
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern Camera[] allCameras
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern int allCamerasCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[Obsolete("use Camera.main instead.")]
		public static Camera mainCamera
		{
			get
			{
				return main;
			}
		}

		public extern bool useOcclusionCulling
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float[] layerCullDistances
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool layerCullSpherical
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern DepthTextureMode depthTextureMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool clearStencilAfterLightingPass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_backgroundColor(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_backgroundColor(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_rect(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_rect(ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_pixelRect(out Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_pixelRect(ref Rect value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetTargetBuffersImpl(out RenderBuffer color, out RenderBuffer depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetTargetBuffersMRTImpl(RenderBuffer[] color, out RenderBuffer depth);

		public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
		{
			SetTargetBuffersImpl(out colorBuffer, out depthBuffer);
		}

		public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
		{
			SetTargetBuffersMRTImpl(colorBuffer, out depthBuffer);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_cameraToWorldMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_worldToCameraMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_worldToCameraMatrix(ref Matrix4x4 value);

		public void ResetWorldToCameraMatrix()
		{
			INTERNAL_CALL_ResetWorldToCameraMatrix(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_ResetWorldToCameraMatrix(Camera self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_projectionMatrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_projectionMatrix(ref Matrix4x4 value);

		public void ResetProjectionMatrix()
		{
			INTERNAL_CALL_ResetProjectionMatrix(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_ResetProjectionMatrix(Camera self);

		public void ResetAspect()
		{
			INTERNAL_CALL_ResetAspect(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_ResetAspect(Camera self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_velocity(out Vector3 value);

		public Vector3 WorldToScreenPoint(Vector3 position)
		{
			return INTERNAL_CALL_WorldToScreenPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_WorldToScreenPoint(Camera self, ref Vector3 position);

		public Vector3 WorldToViewportPoint(Vector3 position)
		{
			return INTERNAL_CALL_WorldToViewportPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_WorldToViewportPoint(Camera self, ref Vector3 position);

		public Vector3 ViewportToWorldPoint(Vector3 position)
		{
			return INTERNAL_CALL_ViewportToWorldPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_ViewportToWorldPoint(Camera self, ref Vector3 position);

		public Vector3 ScreenToWorldPoint(Vector3 position)
		{
			return INTERNAL_CALL_ScreenToWorldPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_ScreenToWorldPoint(Camera self, ref Vector3 position);

		public Vector3 ScreenToViewportPoint(Vector3 position)
		{
			return INTERNAL_CALL_ScreenToViewportPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_ScreenToViewportPoint(Camera self, ref Vector3 position);

		public Vector3 ViewportToScreenPoint(Vector3 position)
		{
			return INTERNAL_CALL_ViewportToScreenPoint(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 INTERNAL_CALL_ViewportToScreenPoint(Camera self, ref Vector3 position);

		public Ray ViewportPointToRay(Vector3 position)
		{
			return INTERNAL_CALL_ViewportPointToRay(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Ray INTERNAL_CALL_ViewportPointToRay(Camera self, ref Vector3 position);

		public Ray ScreenPointToRay(Vector3 position)
		{
			return INTERNAL_CALL_ScreenPointToRay(this, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Ray INTERNAL_CALL_ScreenPointToRay(Camera self, ref Vector3 position);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetAllCameras(Camera[] cameras);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("use Screen.width instead.")]
		[WrapperlessIcall]
		public extern float GetScreenWidth();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("use Screen.height instead.")]
		[WrapperlessIcall]
		public extern float GetScreenHeight();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("Camera.DoClear is deprecated and may be removed in the future.")]
		public extern void DoClear();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Render();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void RenderWithShader(Shader shader, string replacementTag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetReplacementShader(Shader shader, string replacementTag);

		public void ResetReplacementShader()
		{
			INTERNAL_CALL_ResetReplacementShader(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_ResetReplacementShader(Camera self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void RenderDontRestore();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void SetupCurrent(Camera cur);

		[ExcludeFromDocs]
		public bool RenderToCubemap(Cubemap cubemap)
		{
			int faceMask = 63;
			return RenderToCubemap(cubemap, faceMask);
		}

		public bool RenderToCubemap(Cubemap cubemap, [DefaultValue("63")] int faceMask)
		{
			return Internal_RenderToCubemapTexture(cubemap, faceMask);
		}

		[ExcludeFromDocs]
		public bool RenderToCubemap(RenderTexture cubemap)
		{
			int faceMask = 63;
			return RenderToCubemap(cubemap, faceMask);
		}

		public bool RenderToCubemap(RenderTexture cubemap, [DefaultValue("63")] int faceMask)
		{
			return Internal_RenderToCubemapRT(cubemap, faceMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool Internal_RenderToCubemapTexture(Cubemap cubemap, int faceMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void CopyFrom(Camera other);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern bool IsFiltered(GameObject go);

		public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
		{
			return INTERNAL_CALL_CalculateObliqueMatrix(this, ref clipPlane);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Matrix4x4 INTERNAL_CALL_CalculateObliqueMatrix(Camera self, ref Vector4 clipPlane);
	}
}
