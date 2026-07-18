using UnityEngine;

public static class VersionUtility
{
	public static int CompareVersions(string versionA, string versionB)
	{
		string[] array = versionA.Split('.');
		string[] array2 = versionB.Split('.');
		for (int i = 0; i < Mathf.Max(array.Length, array2.Length); i++)
		{
			if (i < array.Length)
			{
				if (i < array2.Length)
				{
					int num = int.Parse(array[i]);
					int num2 = int.Parse(array2[i]);
					if (num > num2)
					{
						return 1;
					}
					if (num < num2)
					{
						return -1;
					}
				}
				else if (int.Parse(array[i]) != 0)
				{
					return 1;
				}
			}
			else if (int.Parse(array2[i]) != 0)
			{
				return -1;
			}
		}
		return 0;
	}
}
