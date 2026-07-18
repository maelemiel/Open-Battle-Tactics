using UnityEngine;

public class ItemsMixerGraphNode : MonoBehaviour
{
	public ItemsMixerGraphNode[] connectedNodes;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		if (connectedNodes != null)
		{
			for (int i = 0; i < connectedNodes.Length; i++)
			{
				if ((bool)connectedNodes[i])
				{
					Gizmos.DrawLine(base.transform.position, connectedNodes[i].transform.position);
				}
			}
		}
		Gizmos.color = Color.green;
		Gizmos.DrawCube(base.transform.position, new Vector3(10f, 10f, 0f));
		Gizmos.color = Color.white;
	}
}
