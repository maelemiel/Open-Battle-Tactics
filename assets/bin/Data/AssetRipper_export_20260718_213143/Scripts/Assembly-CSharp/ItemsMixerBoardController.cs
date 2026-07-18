using System.Collections.Generic;
using UnityEngine;

public class ItemsMixerBoardController : MonoBehaviour
{
	private class ItemsMixerDFSNode
	{
		public ItemsMixerGraphNode node;

		public ItemsMixerGraphNode parent;

		public ItemsMixerDFSNode(ItemsMixerGraphNode node, ItemsMixerGraphNode parent)
		{
			this.parent = parent;
			this.node = node;
		}
	}

	public GameObject boardChip;

	public RecordGameObjectPosition chipRecorder;

	public GameObject[] playerSlots;

	public GameObject[] rewardSlots;

	public ItemsMixerGraphRow[] graphRows;

	[SerializeField]
	private ItemsMixerTrajectoryRecorder trajectoryRecorder;

	private float randomFactor = 0.5f;

	private List<Transform> pathPositions;

	private int initialSlotIndex = -1;

	public List<Transform> Simulate(int initialSlot, int finalSlot)
	{
		Random.seed = Time.frameCount;
		if (initialSlot >= playerSlots.Length)
		{
			Log.Error("[ItemsMixerBoardController] Initial slot simulation index inconsistent", base.gameObject);
		}
		if (finalSlot >= rewardSlots.Length)
		{
			Log.Error("[ItemsMixerBoardController] Initial slot simulation index inconsistent", base.gameObject);
		}
		ItemsMixerGraphNode initialNode = graphRows[graphRows.Length - 1].includedNodes[initialSlot];
		ItemsMixerGraphNode finalNode = graphRows[0].includedNodes[finalSlot];
		pathPositions = CalculatePath(initialNode, finalNode);
		if (pathPositions.Count == 0)
		{
			return pathPositions;
		}
		List<Transform> list = new List<Transform>();
		list.Add(rewardSlots[finalSlot].transform);
		list.AddRange(pathPositions);
		list.Add(playerSlots[initialSlot].transform);
		pathPositions = list;
		return list;
	}

	public List<Transform> CalculatePath(ItemsMixerGraphNode initialNode, ItemsMixerGraphNode finalNode)
	{
		Stack<ItemsMixerDFSNode> stack = new Stack<ItemsMixerDFSNode>();
		Stack<ItemsMixerDFSNode> stack2 = new Stack<ItemsMixerDFSNode>();
		stack2.Push(new ItemsMixerDFSNode(initialNode, null));
		ItemsMixerDFSNode itemsMixerDFSNode = null;
		while (stack2.Count > 0)
		{
			itemsMixerDFSNode = stack2.Pop();
			if (!stack.Contains(itemsMixerDFSNode))
			{
				stack.Push(itemsMixerDFSNode);
				if (itemsMixerDFSNode.node == finalNode)
				{
					break;
				}
				AddConnectedNodes(stack2, itemsMixerDFSNode);
			}
		}
		if (itemsMixerDFSNode.node == initialNode)
		{
			Log.Error("[ItemsMixerBoardController] Path not found", base.gameObject);
			return new List<Transform>();
		}
		if (itemsMixerDFSNode.node != finalNode)
		{
			Log.Error("[ItemsMixerBoardController] Path not found", base.gameObject);
			return new List<Transform>();
		}
		List<Transform> list = new List<Transform>();
		ItemsMixerDFSNode itemsMixerDFSNode2 = itemsMixerDFSNode;
		ItemsMixerDFSNode itemsMixerDFSNode3 = null;
		while (stack.Count > 0)
		{
			itemsMixerDFSNode3 = stack.Pop();
			if (itemsMixerDFSNode2.parent == itemsMixerDFSNode3.node)
			{
				list.Add(itemsMixerDFSNode2.node.transform);
				itemsMixerDFSNode2 = itemsMixerDFSNode3;
				if (stack.Count == 0)
				{
					list.Add(itemsMixerDFSNode3.node.transform);
				}
			}
		}
		return list;
	}

	private void AddConnectedNodes(Stack<ItemsMixerDFSNode> pathPositions, ItemsMixerDFSNode currentNode)
	{
		if (Random.Range(0f, 1f) > randomFactor)
		{
			for (int i = 0; i < currentNode.node.connectedNodes.Length; i++)
			{
				pathPositions.Push(new ItemsMixerDFSNode(currentNode.node.connectedNodes[i], currentNode.node));
			}
			return;
		}
		for (int num = currentNode.node.connectedNodes.Length - 1; num >= 0; num--)
		{
			pathPositions.Push(new ItemsMixerDFSNode(currentNode.node.connectedNodes[num], currentNode.node));
		}
	}

	public void StartTrajectory(int playerSlot)
	{
		if ((bool)chipRecorder)
		{
			initialSlotIndex = playerSlot;
			chipRecorder.gameObject.transform.position = playerSlots[playerSlot].transform.position;
			chipRecorder.gameObject.transform.rotation = Quaternion.identity;
			chipRecorder.StartRecording();
		}
	}

	public void EndTrajectory(int finalIndex)
	{
		if (initialSlotIndex >= 0)
		{
			List<Vector3> trajectoryPath = chipRecorder.StopRecording();
			if ((bool)trajectoryRecorder)
			{
				trajectoryRecorder.AddNewTrajectory(initialSlotIndex, finalIndex, trajectoryPath);
			}
			StartTrajectory(Random.Range(0, playerSlots.Length));
		}
	}
}
