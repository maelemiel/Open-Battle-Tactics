using System.Collections.Generic;
using LitJson0;

public class UserStepGacha
{
	public string userId;

	public int stepUpId;

	public int stepUpNum;

	public int stepUpDraw;

	public UserStepGacha()
	{
	}

	public UserStepGacha(string userId, int stepGachaId, int stepUpNum, int stepUpDraw)
	{
		this.userId = userId;
		stepUpId = stepGachaId;
		this.stepUpNum = stepUpNum;
		this.stepUpDraw = stepUpDraw;
	}

	public static Dictionary<int, UserStepGacha> FromJSON(List<JsonObject> resultsObject)
	{
		Dictionary<int, UserStepGacha> dictionary = new Dictionary<int, UserStepGacha>();
		if (resultsObject.Count == 0)
		{
			return dictionary;
		}
		UserStepGacha userStepGacha = null;
		foreach (JsonObject item in resultsObject)
		{
			if (item != null && item.Dictionary != null)
			{
				userStepGacha = new UserStepGacha();
				userStepGacha.userId = item.GetString("user_id");
				userStepGacha.stepUpId = item.GetInt("step_up_id");
				userStepGacha.stepUpNum = item.GetInt("step_up_num");
				userStepGacha.stepUpDraw = item.GetInt("step_up_draw");
				dictionary.Add(item.GetInt("step_up_id"), userStepGacha);
			}
		}
		return dictionary;
	}
}
