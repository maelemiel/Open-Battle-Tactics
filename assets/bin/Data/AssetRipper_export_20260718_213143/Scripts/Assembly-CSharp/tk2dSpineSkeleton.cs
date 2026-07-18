using System.Collections.Generic;
using Spine;
using UnityEngine;
using tk2dRuntime;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class tk2dSpineSkeleton : MonoBehaviour, ISpriteCollectionForceBuild
{
	public tk2dSpineSkeletonDataAsset skeletonDataAsset;

	public Skeleton skeleton;

	private Mesh mesh;

	private Vector3[] vertices;

	private Color32[] colors;

	private Vector2[] uvs;

	private int cachedQuadCount;

	private float[] vertexPositions;

	private List<Material> submeshMaterials = new List<Material>();

	private List<int[]> submeshIndices = new List<int[]>();

	private Bounds notCullingBounds;

	[SerializeField]
	private int SortingOrder;

	public int SortOrder
	{
		get
		{
			return SortingOrder;
		}
		set
		{
			SortingOrder = value;
			base.renderer.sortingOrder = SortingOrder;
		}
	}

	private void Awake()
	{
		vertexPositions = new float[8];
		submeshMaterials = new List<Material>();
		submeshIndices = new List<int[]>();
		notCullingBounds = new Bounds(Vector3.zero, Vector3.one * 5000f);
		Initialize();
		SortOrder = SortingOrder;
	}

	private void Start()
	{
	}

	public void Update()
	{
		SkeletonData skeletonData = ((!(skeletonDataAsset == null)) ? skeletonDataAsset.GetSkeletonData() : null);
		if (skeletonData == null)
		{
			Clear();
			return;
		}
		if (skeleton == null || skeleton.Data != skeletonData)
		{
			Initialize();
		}
		skeleton.UpdateWorldTransform();
		UpdateCache();
		UpdateMesh();
	}

	public void ResetData()
	{
		if (mesh != null && vertices != null)
		{
			skeleton.SetToSetupPose();
		}
	}

	private void Clear()
	{
		GetComponent<MeshFilter>().mesh = null;
		Object.DestroyImmediate(mesh);
		mesh = null;
		skeleton = null;
	}

	public void Initialize()
	{
		mesh = new Mesh();
		GetComponent<MeshFilter>().mesh = mesh;
		mesh.name = "tk2dSkeleton Mesh";
		mesh.hideFlags = HideFlags.HideAndDontSave;
		if (skeletonDataAsset != null)
		{
			skeleton = new Skeleton(skeletonDataAsset.GetSkeletonData());
		}
	}

	private void UpdateMesh()
	{
		int num = 0;
		int count = skeleton.DrawOrder.Count;
		Color32 color = default(Color32);
		for (int i = 0; i < count; i++)
		{
			Slot slot = skeleton.DrawOrder[i];
			Attachment attachment = slot.Attachment;
			if (attachment is RegionAttachment)
			{
				RegionAttachment regionAttachment = attachment as RegionAttachment;
				regionAttachment.ComputeVertices(skeleton.X, skeleton.Y, slot.Bone, vertexPositions);
				int num2 = num * 4;
				vertices[num2] = new Vector3(vertexPositions[0], vertexPositions[1], 0f);
				vertices[num2 + 1] = new Vector3(vertexPositions[6], vertexPositions[7], 0f);
				vertices[num2 + 2] = new Vector3(vertexPositions[2], vertexPositions[3], 0f);
				vertices[num2 + 3] = new Vector3(vertexPositions[4], vertexPositions[5], 0f);
				float[] uVs = regionAttachment.UVs;
				uvs[num2] = new Vector2(uVs[0], uVs[1]);
				uvs[num2 + 1] = new Vector2(uVs[6], uVs[7]);
				uvs[num2 + 2] = new Vector2(uVs[2], uVs[3]);
				uvs[num2 + 3] = new Vector2(uVs[4], uVs[5]);
				color.a = (byte)(skeleton.A * slot.A * 255f);
				color.r = (byte)(skeleton.R * slot.R * (float)(int)color.a);
				color.g = (byte)(skeleton.G * slot.G * (float)(int)color.a);
				color.b = (byte)(skeleton.B * slot.B * (float)(int)color.a);
				colors[num2] = color;
				colors[num2 + 1] = color;
				colors[num2 + 2] = color;
				colors[num2 + 3] = color;
				num++;
			}
		}
		mesh.vertices = vertices;
		mesh.colors32 = colors;
		mesh.uv = uvs;
		if (skeletonDataAsset.normalGenerationMode != tk2dSpriteCollection.NormalGenerationMode.None)
		{
			mesh.RecalculateNormals();
			if (skeletonDataAsset.normalGenerationMode == tk2dSpriteCollection.NormalGenerationMode.NormalsAndTangents)
			{
				Vector4[] array = new Vector4[mesh.normals.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = new Vector4(1f, 0f, 0f, 1f);
				}
				mesh.tangents = array;
			}
		}
		mesh.bounds = notCullingBounds;
	}

	private void UpdateCache()
	{
		int num = 0;
		int count = skeleton.DrawOrder.Count;
		for (int i = 0; i < count; i++)
		{
			Attachment attachment = skeleton.DrawOrder[i].Attachment;
			if (attachment is RegionAttachment)
			{
				num++;
			}
		}
		if (num != cachedQuadCount)
		{
			cachedQuadCount = num;
			vertices = new Vector3[num * 4];
			uvs = new Vector2[num * 4];
			colors = new Color32[num * 4];
			UpdateSubmeshCache();
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.colors32 = colors;
			mesh.uv = uvs;
			mesh.subMeshCount = submeshIndices.Count;
			for (int j = 0; j < mesh.subMeshCount; j++)
			{
				mesh.SetTriangles(submeshIndices[j], j);
			}
		}
	}

	private void UpdateSubmeshCache()
	{
		submeshIndices.Clear();
		submeshMaterials.Clear();
		Material material = null;
		List<int> list = new List<int>();
		int num = 0;
		int count = skeleton.DrawOrder.Count;
		for (int i = 0; i < count; i++)
		{
			Attachment attachment = skeleton.DrawOrder[i].Attachment;
			if (attachment is RegionAttachment)
			{
				Material materialInst = skeletonDataAsset.spritesData.GetSpriteDefinition(attachment.Name).materialInst;
				if (material == null)
				{
					material = materialInst;
				}
				if (material != materialInst)
				{
					submeshIndices.Add(list.ToArray());
					submeshMaterials.Add(material);
					list.Clear();
				}
				int num2 = num * 4;
				list.Add(num2);
				list.Add(num2 + 2);
				list.Add(num2 + 1);
				list.Add(num2 + 2);
				list.Add(num2 + 3);
				list.Add(num2 + 1);
				num++;
				material = materialInst;
			}
		}
		submeshIndices.Add(list.ToArray());
		submeshMaterials.Add(material);
		base.renderer.sharedMaterials = submeshMaterials.ToArray();
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		return skeletonDataAsset.spritesData == spriteCollection;
	}

	public void ForceBuild()
	{
		skeletonDataAsset.ForceUpdate();
		skeleton = new Skeleton(skeletonDataAsset.GetSkeletonData());
		UpdateSubmeshCache();
		UpdateMesh();
	}
}
