using UnityEngine;

public class ItemsMixerGraphRow : MonoBehaviour
{
	public ItemsMixerGraphNode[] includedNodes;

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(base.transform.position, new Vector3(600f, 20f, 0f));
	}

	[ContextMenu("Assign Nodes")]
	private void AssignNodes()
	{
		ItemsMixerGraphNode[] componentsInChildren = GetComponentsInChildren<ItemsMixerGraphNode>();
		includedNodes = componentsInChildren;
	}
}
