using System.Collections.Generic;

namespace MobageEditor
{
	public interface ISerializableItem
	{
		Dictionary<string, object> PackForEnvironment(ModelSerializationEnvironment env);
	}
}
