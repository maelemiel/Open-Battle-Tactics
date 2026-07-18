using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace MobageEditor
{
	public class User : WW_DataModel, ISerializableItem
	{
		public string uid;

		public string nickname;

		public string thumbnailUrl_;

		public string aboutMe;

		public bool hasApp;

		public int age;

		public int grade;

		public bool isFamous;

		public string bloodType;

		public string firstName;

		public string lastName;

		public string relation;

		public string gender;

		public string phoneNumber;

		public int unreadWallPostCount;

		public int gamerScore;

		public int levelNumber;

		public string levelName;

		public int currentLevelScore;

		public int nextLevelScore;

		public int sessionCount;

		public bool isNuxComplete;

		public bool isMobageUser;

		public bool isGameHubUser;

		public bool isNewBuddy;

		public bool isMutualFriend;

		public bool privacyFlag;

		public bool mailOptInFlag;

		public bool hidePresenceFlag;

		public bool ignoreFriendRequestsFlag;

		public bool onlyShowFriendNotifications;

		public bool filterWallPostsToFriendsOnly;

		public string email;

		public bool age_restricted;

		public int id
		{
			get
			{
				return int.Parse(uid);
			}
			set
			{
				uid = value.ToString();
			}
		}

		public string gamertag
		{
			get
			{
				return nickname;
			}
			set
			{
				nickname = value;
			}
		}

		public string displayName
		{
			get
			{
				return first_name + " " + last_name;
			}
		}

		public string thumbnailUrl
		{
			get
			{
				if (string.IsNullOrEmpty(thumbnailUrl_))
				{
					return badge_id;
				}
				return thumbnailUrl_;
			}
			set
			{
				thumbnailUrl_ = value;
			}
		}

		public string motto
		{
			get
			{
				return aboutMe;
			}
			set
			{
				aboutMe = value;
			}
		}

		public string first_name
		{
			get
			{
				return firstName;
			}
			set
			{
				firstName = value;
			}
		}

		public string last_name
		{
			get
			{
				return lastName;
			}
			set
			{
				lastName = value;
			}
		}

		public int level_position
		{
			get
			{
				return levelNumber;
			}
			set
			{
				levelNumber = value;
			}
		}

		public int level_next_points
		{
			get
			{
				return nextLevelScore;
			}
			set
			{
				nextLevelScore = value;
			}
		}

		public int num_session
		{
			get
			{
				return sessionCount;
			}
			set
			{
				sessionCount = value;
			}
		}

		public bool new_user_experience_complete
		{
			get
			{
				return isNuxComplete;
			}
			set
			{
				isNuxComplete = value;
			}
		}

		public bool mobage_user
		{
			get
			{
				return isMobageUser;
			}
			set
			{
				isMobageUser = value;
			}
		}

		public bool opt_in
		{
			get
			{
				return mailOptInFlag;
			}
			set
			{
				mailOptInFlag = value;
			}
		}

		public string badge_id
		{
			get
			{
				return thumbnailUrl_;
			}
			set
			{
				thumbnailUrl_ = value;
			}
		}

		public UserList BlacklistedUsers
		{
			get
			{
				return new UserList(this, "enemies");
			}
		}

		public int user_id
		{
			get
			{
				return System.Convert.ToInt32(uid);
			}
			set
			{
				uid = System.Convert.ToString(value);
			}
		}

		public string CacheId
		{
			get
			{
				return uid;
			}
		}

		public UserList MutualFriends
		{
			get
			{
				return new UserList(this, "mutual");
			}
		}

		public UserList MutualFriendsWithCurrentGame
		{
			get
			{
				Mobage sharedInstance = Mobage.sharedInstance;
				return new UserList(this, sharedInstance.AppId, "mutual");
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			User user = obj as User;
			if (user == null)
			{
				return false;
			}
			return uid == user.uid;
		}

		public override int GetHashCode()
		{
			return uid.GetHashCode();
		}

		public Dictionary<string, object> PackForEnvironment(ModelSerializationEnvironment env)
		{
			return new Dictionary<string, object>();
		}

		public static void FetchUserWithUserID(string uid, Action<Error, User> cb)
		{
			FetchUserWithString(uid, cb);
		}

		public static void FetchUserWithString(string uid, Action<Error, User> cb)
		{
			User user = new User();
			user.uid = uid;
			User user2 = user;
			user2.UpdateFromServerWithCallback(cb);
		}

		public static User Create(JsonData data)
		{
			User user = JsonMapper.ToObject<User>(data.ToJson());
			user.UpdateWithDictionary(data);
			return user;
		}

		public void UpdateFromServerWithCallback(Action<Error, User> cb)
		{
			Action<Error, User> onComplete = delegate(Error error, User user)
			{
				cb(error, user);
			};
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "users/" + uid;
			request.HTTPMethod = "GET";
			request.EntityTag = CacheControl;
			request.Send(delegate(Error error, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				if (error != null)
				{
					MobageLogger.log("MB_WW_User: Error updating from server. " + error.ToString());
					onComplete(error, null);
				}
				else
				{
					if (status == HttpStatusCode.NotModified)
					{
						Debug.Log("MB_WW_User: got etag hit updating: " + ToString());
					}
					else
					{
						UpdateWithDictionary(data);
						if (headers["ETag"] != null)
						{
							CacheControl = headers["ETag"];
						}
					}
					onComplete(null, this);
				}
			});
		}

		public bool UpdateWithDictionary(JsonData data)
		{
			User user = JsonMapper.ToObject<User>(data.ToJson());
			uid = user.uid;
			nickname = user.nickname;
			firstName = user.firstName;
			lastName = user.lastName;
			thumbnailUrl = user.thumbnailUrl;
			aboutMe = user.aboutMe;
			mobage_user = user.mobage_user;
			level_next_points = user.level_next_points;
			level_position = user.level_position;
			relation = user.relation;
			grade = user.grade;
			return true;
		}
	}
}
