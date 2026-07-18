using System.Collections.Generic;
using UnityEngine;

public class RecordGameObjectPosition : MonoBehaviour
{
	private List<Vector3> recordedPositions = new List<Vector3>();

	private bool isRecording;

	private Transform cachedTransform;

	private Vector3 currentPosition = Vector3.zero;

	private void Start()
	{
		cachedTransform = base.transform;
		StartRecording();
	}

	private void FixedUpdate()
	{
		if (isRecording)
		{
			currentPosition = cachedTransform.position;
			currentPosition.z = cachedTransform.rotation.eulerAngles.z;
			recordedPositions.Add(currentPosition);
		}
	}

	public void StartRecording()
	{
		isRecording = true;
		recordedPositions.Clear();
	}

	public List<Vector3> StopRecording()
	{
		isRecording = false;
		return new List<Vector3>(recordedPositions);
	}

	private void OnDrawGizmosSelected()
	{
		if (recordedPositions.Count > 1)
		{
			for (int i = 1; i < recordedPositions.Count; i++)
			{
				Gizmos.DrawLine(recordedPositions[i - 1], recordedPositions[i]);
			}
		}
	}
}
