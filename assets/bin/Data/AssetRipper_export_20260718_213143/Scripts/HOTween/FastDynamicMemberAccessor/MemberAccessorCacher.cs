using System;
using System.Collections.Generic;
using System.Reflection;

namespace FastDynamicMemberAccessor
{
	internal static class MemberAccessorCacher
	{
		private static Dictionary<Type, Dictionary<string, MemberAccessor>> dcMemberAccessors;

		internal static MemberAccessor Make(Type p_targetType, string p_propName, PropertyInfo p_propertyInfo, FieldInfo p_fieldInfo)
		{
			if (dcMemberAccessors != null && dcMemberAccessors.ContainsKey(p_targetType) && dcMemberAccessors[p_targetType].ContainsKey(p_propName))
			{
				return dcMemberAccessors[p_targetType][p_propName];
			}
			if (dcMemberAccessors == null)
			{
				dcMemberAccessors = new Dictionary<Type, Dictionary<string, MemberAccessor>>();
			}
			if (!dcMemberAccessors.ContainsKey(p_targetType))
			{
				dcMemberAccessors.Add(p_targetType, new Dictionary<string, MemberAccessor>());
			}
			Dictionary<string, MemberAccessor> dictionary = dcMemberAccessors[p_targetType];
			MemberAccessor memberAccessor = MemberAccessor.Make(p_propertyInfo, p_fieldInfo);
			dictionary.Add(p_propName, memberAccessor);
			return memberAccessor;
		}

		internal static void Clear()
		{
			dcMemberAccessors = null;
		}
	}
}
