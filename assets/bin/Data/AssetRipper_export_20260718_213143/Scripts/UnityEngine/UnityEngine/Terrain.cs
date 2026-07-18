using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public sealed class Terrain : MonoBehaviour
	{
		[SerializeField]
		private TerrainData m_TerrainData;

		[SerializeField]
		private float m_TreeDistance = 5000f;

		[SerializeField]
		private float m_TreeBillboardDistance = 50f;

		[SerializeField]
		private float m_TreeCrossFadeLength = 5f;

		[SerializeField]
		private int m_TreeMaximumFullLODCount = 50;

		[SerializeField]
		private float m_DetailObjectDistance = 80f;

		[SerializeField]
		private float m_DetailObjectDensity = 1f;

		[SerializeField]
		private float m_HeightmapPixelError = 5f;

		[SerializeField]
		private float m_SplatMapDistance = 1000f;

		[SerializeField]
		private int m_HeightmapMaximumLOD;

		[SerializeField]
		private bool m_CastShadows = true;

		[SerializeField]
		private int m_LightmapIndex = -1;

		[SerializeField]
		private int m_LightmapSize = 1024;

		[SerializeField]
		private bool m_DrawTreesAndFoliage = true;

		[SerializeField]
		private bool m_CollectDetailPatches = true;

		[SerializeField]
		private Material m_MaterialTemplate;

		[NonSerialized]
		private IntPtr m_TerrainInstance;

		private IntPtr InstanceObject
		{
			get
			{
				MakeSureObjectIsAlive();
				if (m_TerrainInstance == IntPtr.Zero)
				{
					m_TerrainInstance = Construct();
					Internal_SetTerrainData(m_TerrainInstance, m_TerrainData);
					Internal_SetTreeDistance(m_TerrainInstance, m_TreeDistance);
					Internal_SetTreeBillboardDistance(m_TerrainInstance, m_TreeBillboardDistance);
					Internal_SetTreeCrossFadeLength(m_TerrainInstance, m_TreeCrossFadeLength);
					Internal_SetTreeMaximumFullLODCount(m_TerrainInstance, m_TreeMaximumFullLODCount);
					Internal_SetDetailObjectDistance(m_TerrainInstance, m_DetailObjectDistance);
					Internal_SetDetailObjectDensity(m_TerrainInstance, m_DetailObjectDensity);
					Internal_SetHeightmapPixelError(m_TerrainInstance, m_HeightmapPixelError);
					Internal_SetBasemapDistance(m_TerrainInstance, m_SplatMapDistance);
					Internal_SetHeightmapMaximumLOD(m_TerrainInstance, m_HeightmapMaximumLOD);
					Internal_SetCastShadows(m_TerrainInstance, m_CastShadows);
					Internal_SetLightmapIndex(m_TerrainInstance, m_LightmapIndex);
					Internal_SetLightmapSize(m_TerrainInstance, m_LightmapSize);
					Internal_SetDrawTreesAndFoliage(m_TerrainInstance, m_DrawTreesAndFoliage);
					Internal_SetCollectDetailPatches(m_TerrainInstance, m_CollectDetailPatches);
					Internal_SetMaterialTemplate(m_TerrainInstance, m_MaterialTemplate);
				}
				return m_TerrainInstance;
			}
			set
			{
				m_TerrainInstance = value;
			}
		}

		public TerrainRenderFlags editorRenderFlags
		{
			get
			{
				return (TerrainRenderFlags)GetEditorRenderFlags(InstanceObject);
			}
			set
			{
				SetEditorRenderFlags(InstanceObject, (int)value);
			}
		}

		public TerrainData terrainData
		{
			get
			{
				if (m_TerrainData != Internal_GetTerrainData(InstanceObject))
				{
					Internal_SetTerrainData(InstanceObject, m_TerrainData);
				}
				return m_TerrainData;
			}
			set
			{
				m_TerrainData = value;
				Internal_SetTerrainData(InstanceObject, value);
			}
		}

		public float treeDistance
		{
			get
			{
				if (m_TreeDistance != Internal_GetTreeDistance(InstanceObject))
				{
					Internal_SetTreeDistance(InstanceObject, m_TreeDistance);
				}
				return m_TreeDistance;
			}
			set
			{
				m_TreeDistance = value;
				Internal_SetTreeDistance(InstanceObject, value);
			}
		}

		public float treeBillboardDistance
		{
			get
			{
				if (m_TreeBillboardDistance != Internal_GetTreeBillboardDistance(InstanceObject))
				{
					Internal_SetTreeBillboardDistance(InstanceObject, m_TreeBillboardDistance);
				}
				return m_TreeBillboardDistance;
			}
			set
			{
				m_TreeBillboardDistance = value;
				Internal_SetTreeBillboardDistance(InstanceObject, value);
			}
		}

		public float treeCrossFadeLength
		{
			get
			{
				if (m_TreeCrossFadeLength != Internal_GetTreeCrossFadeLength(InstanceObject))
				{
					Internal_SetTreeCrossFadeLength(InstanceObject, m_TreeCrossFadeLength);
				}
				return m_TreeCrossFadeLength;
			}
			set
			{
				m_TreeCrossFadeLength = value;
				Internal_SetTreeCrossFadeLength(InstanceObject, value);
			}
		}

		public int treeMaximumFullLODCount
		{
			get
			{
				if (m_TreeMaximumFullLODCount != Internal_GetTreeMaximumFullLODCount(InstanceObject))
				{
					Internal_SetTreeMaximumFullLODCount(InstanceObject, m_TreeMaximumFullLODCount);
				}
				return m_TreeMaximumFullLODCount;
			}
			set
			{
				m_TreeMaximumFullLODCount = value;
				Internal_SetTreeMaximumFullLODCount(InstanceObject, value);
			}
		}

		public float detailObjectDistance
		{
			get
			{
				if (m_DetailObjectDistance != Internal_GetDetailObjectDistance(InstanceObject))
				{
					Internal_SetDetailObjectDistance(InstanceObject, m_DetailObjectDistance);
				}
				return m_DetailObjectDistance;
			}
			set
			{
				m_DetailObjectDistance = value;
				Internal_SetDetailObjectDistance(InstanceObject, value);
			}
		}

		public float detailObjectDensity
		{
			get
			{
				if (m_DetailObjectDensity != Internal_GetDetailObjectDensity(InstanceObject))
				{
					Internal_SetDetailObjectDensity(InstanceObject, m_DetailObjectDensity);
				}
				return m_DetailObjectDensity;
			}
			set
			{
				m_DetailObjectDensity = value;
				Internal_SetDetailObjectDensity(InstanceObject, value);
			}
		}

		public float heightmapPixelError
		{
			get
			{
				if (m_HeightmapPixelError != Internal_GetHeightmapPixelError(InstanceObject))
				{
					Internal_SetHeightmapPixelError(InstanceObject, m_HeightmapPixelError);
				}
				return m_HeightmapPixelError;
			}
			set
			{
				m_HeightmapPixelError = value;
				Internal_SetHeightmapPixelError(InstanceObject, value);
			}
		}

		public int heightmapMaximumLOD
		{
			get
			{
				if (m_HeightmapMaximumLOD != Internal_GetHeightmapMaximumLOD(InstanceObject))
				{
					Internal_SetHeightmapMaximumLOD(InstanceObject, m_HeightmapMaximumLOD);
				}
				return m_HeightmapMaximumLOD;
			}
			set
			{
				m_HeightmapMaximumLOD = value;
				Internal_SetHeightmapMaximumLOD(InstanceObject, value);
			}
		}

		public float basemapDistance
		{
			get
			{
				if (m_SplatMapDistance != Internal_GetBasemapDistance(InstanceObject))
				{
					Internal_SetBasemapDistance(InstanceObject, m_SplatMapDistance);
				}
				return m_SplatMapDistance;
			}
			set
			{
				m_SplatMapDistance = value;
				Internal_SetBasemapDistance(InstanceObject, value);
			}
		}

		[Obsolete("use basemapDistance", true)]
		public float splatmapDistance
		{
			get
			{
				return basemapDistance;
			}
			set
			{
				basemapDistance = value;
			}
		}

		public int lightmapIndex
		{
			get
			{
				if (m_LightmapIndex != Internal_GetLightmapIndex(InstanceObject))
				{
					Internal_SetLightmapIndex(InstanceObject, m_LightmapIndex);
				}
				return m_LightmapIndex;
			}
			set
			{
				m_LightmapIndex = value;
				Internal_SetLightmapIndex(InstanceObject, value);
			}
		}

		internal int lightmapSize
		{
			get
			{
				if (m_LightmapSize != Internal_GetLightmapSize(InstanceObject))
				{
					Internal_SetLightmapSize(InstanceObject, m_LightmapSize);
				}
				return m_LightmapSize;
			}
			set
			{
				m_LightmapSize = value;
				Internal_SetLightmapSize(InstanceObject, value);
			}
		}

		public bool castShadows
		{
			get
			{
				if (m_CastShadows != Internal_GetCastShadows(InstanceObject))
				{
					Internal_SetCastShadows(InstanceObject, m_CastShadows);
				}
				return m_CastShadows;
			}
			set
			{
				m_CastShadows = value;
				Internal_SetCastShadows(InstanceObject, value);
			}
		}

		public Material materialTemplate
		{
			get
			{
				if (m_MaterialTemplate != Internal_GetMaterialTemplate(InstanceObject))
				{
					Internal_SetMaterialTemplate(InstanceObject, m_MaterialTemplate);
				}
				return m_MaterialTemplate;
			}
			set
			{
				m_MaterialTemplate = value;
				Internal_SetMaterialTemplate(InstanceObject, value);
			}
		}

		internal bool drawTreesAndFoliage
		{
			get
			{
				if (m_DrawTreesAndFoliage != Internal_GetDrawTreesAndFoliage(InstanceObject))
				{
					Internal_SetDrawTreesAndFoliage(InstanceObject, m_DrawTreesAndFoliage);
				}
				return m_DrawTreesAndFoliage;
			}
			set
			{
				m_DrawTreesAndFoliage = value;
				Internal_SetDrawTreesAndFoliage(InstanceObject, value);
			}
		}

		public bool collectDetailPatches
		{
			get
			{
				if (m_CollectDetailPatches != Internal_GetCollectDetailPatches(InstanceObject))
				{
					Internal_SetCollectDetailPatches(InstanceObject, m_CollectDetailPatches);
				}
				return m_CollectDetailPatches;
			}
			set
			{
				m_CollectDetailPatches = value;
				Internal_SetCollectDetailPatches(InstanceObject, value);
			}
		}

		public static extern Terrain activeTerrain
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern Terrain[] activeTerrains
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		private void OnDestroy()
		{
			OnDisable();
			Cleanup(m_TerrainInstance);
			m_TerrainInstance = IntPtr.Zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void MakeSureObjectIsAlive();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Cleanup(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern int GetEditorRenderFlags(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void SetEditorRenderFlags(IntPtr terrainInstance, int flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern TerrainData Internal_GetTerrainData(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetTerrainData(IntPtr terrainInstance, TerrainData value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float Internal_GetTreeDistance(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetTreeDistance(IntPtr terrainInstance, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float Internal_GetTreeBillboardDistance(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetTreeBillboardDistance(IntPtr terrainInstance, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float Internal_GetTreeCrossFadeLength(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetTreeCrossFadeLength(IntPtr terrainInstance, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern int Internal_GetTreeMaximumFullLODCount(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetTreeMaximumFullLODCount(IntPtr terrainInstance, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float Internal_GetDetailObjectDistance(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetDetailObjectDistance(IntPtr terrainInstance, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float Internal_GetDetailObjectDensity(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetDetailObjectDensity(IntPtr terrainInstance, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float Internal_GetHeightmapPixelError(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetHeightmapPixelError(IntPtr terrainInstance, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern int Internal_GetHeightmapMaximumLOD(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetHeightmapMaximumLOD(IntPtr terrainInstance, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern float Internal_GetBasemapDistance(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetBasemapDistance(IntPtr terrainInstance, float value);

		private void SetLightmapIndex(int value)
		{
			lightmapIndex = value;
		}

		private void ShiftLightmapIndex(int offset)
		{
			lightmapIndex += offset;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern int Internal_GetLightmapIndex(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetLightmapIndex(IntPtr terrainInstance, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern int Internal_GetLightmapSize(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetLightmapSize(IntPtr terrainInstance, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool Internal_GetCastShadows(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetCastShadows(IntPtr terrainInstance, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Material Internal_GetMaterialTemplate(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetMaterialTemplate(IntPtr terrainInstance, Material value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool Internal_GetDrawTreesAndFoliage(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetDrawTreesAndFoliage(IntPtr terrainInstance, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool Internal_GetCollectDetailPatches(IntPtr terrainInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetCollectDetailPatches(IntPtr terrainInstance, bool value);

		public float SampleHeight(Vector3 worldPosition)
		{
			return Internal_SampleHeight(InstanceObject, worldPosition);
		}

		private float Internal_SampleHeight(IntPtr terrainInstance, Vector3 worldPosition)
		{
			return INTERNAL_CALL_Internal_SampleHeight(this, terrainInstance, ref worldPosition);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float INTERNAL_CALL_Internal_SampleHeight(Terrain self, IntPtr terrainInstance, ref Vector3 worldPosition);

		internal void ApplyDelayedHeightmapModification()
		{
			Internal_ApplyDelayedHeightmapModification(InstanceObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern void Internal_ApplyDelayedHeightmapModification(IntPtr terrainInstance);

		public void AddTreeInstance(TreeInstance instance)
		{
			Internal_AddTreeInstance(InstanceObject, instance);
		}

		private void Internal_AddTreeInstance(IntPtr terrainInstance, TreeInstance instance)
		{
			INTERNAL_CALL_Internal_AddTreeInstance(this, terrainInstance, ref instance);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Internal_AddTreeInstance(Terrain self, IntPtr terrainInstance, ref TreeInstance instance);

		public void SetNeighbors(Terrain left, Terrain top, Terrain right, Terrain bottom)
		{
			Internal_SetNeighbors(InstanceObject, (!(left != null)) ? IntPtr.Zero : left.InstanceObject, (!(top != null)) ? IntPtr.Zero : top.InstanceObject, (!(right != null)) ? IntPtr.Zero : right.InstanceObject, (!(bottom != null)) ? IntPtr.Zero : bottom.InstanceObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetNeighbors(IntPtr terrainInstance, IntPtr left, IntPtr top, IntPtr right, IntPtr bottom);

		public Vector3 GetPosition()
		{
			return Internal_GetPosition(InstanceObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Vector3 Internal_GetPosition(IntPtr terrainInstance);

		public void Flush()
		{
			Internal_Flush(InstanceObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_Flush(IntPtr terrainInstance);

		internal void RemoveTrees(Vector2 position, float radius, int prototypeIndex)
		{
			Internal_RemoveTrees(InstanceObject, position, radius, prototypeIndex);
		}

		private void Internal_RemoveTrees(IntPtr terrainInstance, Vector2 position, float radius, int prototypeIndex)
		{
			INTERNAL_CALL_Internal_RemoveTrees(this, terrainInstance, ref position, radius, prototypeIndex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Internal_RemoveTrees(Terrain self, IntPtr terrainInstance, ref Vector2 position, float radius, int prototypeIndex);

		private void OnTerrainChanged(TerrainChangedFlags flags)
		{
			Internal_OnTerrainChanged(InstanceObject, flags);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_OnTerrainChanged(IntPtr terrainInstance, TerrainChangedFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern IntPtr Construct();

		internal void OnEnable()
		{
			Internal_OnEnable(InstanceObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_OnEnable(IntPtr terrainInstance);

		internal void OnDisable()
		{
			Internal_OnDisable(InstanceObject);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_OnDisable(IntPtr terrainInstance);

		public static GameObject CreateTerrainGameObject(TerrainData assignTerrain)
		{
			GameObject gameObject = new GameObject("Terrain", typeof(Terrain), typeof(TerrainCollider));
			gameObject.isStatic = true;
			Terrain terrain = gameObject.GetComponent(typeof(Terrain)) as Terrain;
			TerrainCollider terrainCollider = gameObject.GetComponent(typeof(TerrainCollider)) as TerrainCollider;
			terrainCollider.terrainData = assignTerrain;
			terrain.terrainData = assignTerrain;
			terrain.OnEnable();
			return gameObject;
		}

		private static void ReconnectTerrainData()
		{
			List<Terrain> list = new List<Terrain>(activeTerrains);
			foreach (Terrain item in list)
			{
				if (item.terrainData == null)
				{
					item.OnDisable();
				}
				else if (!item.terrainData.HasUser(item.gameObject))
				{
					item.OnDisable();
					item.OnEnable();
				}
			}
		}
	}
}
