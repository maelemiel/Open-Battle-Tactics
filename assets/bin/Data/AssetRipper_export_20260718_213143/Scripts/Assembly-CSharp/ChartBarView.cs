using UnityEngine;

public class ChartBarView : MonoBehaviour
{
	public Renderer[] barPieces;

	public void SetBarLevel(int level)
	{
		if (level < 0)
		{
			DeactivateBar();
			return;
		}
		level = Mathf.Min(barPieces.Length, level);
		bool flag = false;
		for (int i = 0; i < barPieces.Length; i++)
		{
			flag = i <= level;
			barPieces[i].gameObject.SetActive(flag);
		}
	}

	public void DeactivateBar()
	{
		for (int i = 0; i < barPieces.Length; i++)
		{
			barPieces[i].gameObject.SetActive(false);
		}
	}
}
