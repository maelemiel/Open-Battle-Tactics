using UnityEngine;

namespace MobageEditor
{
	public class ModelMemoryCache
	{
		private class UserSerializer
		{
			public string nickname;

			public UserSerializer(User user)
			{
				nickname = user.nickname;
			}
		}

		public static User RegisterOrLookup(User obj, string cacheId)
		{
			Debug.Log("RegisterOrLookup " + cacheId);
			string text = PlayerPrefs.GetString(string.Concat(obj.GetType(), cacheId));
			Debug.Log("tmp:" + text);
			if (!string.IsNullOrEmpty(text))
			{
				return JsonMapper.ToObject<User>(text);
			}
			Debug.Log("register");
			PlayerPrefs.SetString(string.Concat(obj.GetType(), cacheId), JsonMapper.ToJson(new UserSerializer(obj)));
			return obj;
		}
	}
}
