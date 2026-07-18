using System;

namespace UnityEngine
{
	internal class AndroidReflection
	{
		private const string RELECTION_HELPER_CLASS_NAME = "com/unity3d/player/ReflectionHelper";

		private static IntPtr s_ReflectionHelperClass = AndroidJNI.NewGlobalRef(AndroidJNISafe.FindClass("com/unity3d/player/ReflectionHelper"));

		private static IntPtr s_ReflectionHelperGetConstructorID = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getConstructorID", "(Ljava/lang/Class;Ljava/lang/String;)Ljava/lang/reflect/Constructor;");

		private static IntPtr s_ReflectionHelperGetMethodID = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getMethodID", "(Ljava/lang/Class;Ljava/lang/String;Ljava/lang/String;Z)Ljava/lang/reflect/Method;");

		private static IntPtr s_ReflectionHelperGetFieldID = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "getFieldID", "(Ljava/lang/Class;Ljava/lang/String;Ljava/lang/String;Z)Ljava/lang/reflect/Field;");

		private static IntPtr s_ReflectionHelperNewProxyInstance = GetStaticMethodID("com/unity3d/player/ReflectionHelper", "newProxyInstance", "(ILjava/lang/Class;)Ljava/lang/Object;");

		private static IntPtr GetStaticMethodID(string clazz, string methodName, string signature)
		{
			IntPtr intPtr = AndroidJNISafe.FindClass(clazz);
			try
			{
				return AndroidJNISafe.GetStaticMethodID(intPtr, methodName, signature);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(intPtr);
			}
		}

		public static IntPtr GetConstructorMember(IntPtr jclass, string signature)
		{
			jvalue[] array = new jvalue[2];
			try
			{
				array[0].l = jclass;
				array[1].l = AndroidJNISafe.NewStringUTF(signature);
				return AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperGetConstructorID, array);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(array[1].l);
			}
		}

		public static IntPtr GetMethodMember(IntPtr jclass, string methodName, string signature, bool isStatic)
		{
			jvalue[] array = new jvalue[4];
			try
			{
				array[0].l = jclass;
				array[1].l = AndroidJNISafe.NewStringUTF(methodName);
				array[2].l = AndroidJNISafe.NewStringUTF(signature);
				array[3].z = isStatic;
				return AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperGetMethodID, array);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(array[1].l);
				AndroidJNISafe.DeleteLocalRef(array[2].l);
			}
		}

		public static IntPtr GetFieldMember(IntPtr jclass, string fieldName, string signature, bool isStatic)
		{
			jvalue[] array = new jvalue[4];
			try
			{
				array[0].l = jclass;
				array[1].l = AndroidJNISafe.NewStringUTF(fieldName);
				array[2].l = AndroidJNISafe.NewStringUTF(signature);
				array[3].z = isStatic;
				return AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperGetFieldID, array);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(array[1].l);
				AndroidJNISafe.DeleteLocalRef(array[2].l);
			}
		}

		public static IntPtr NewProxyInstance(int delegateHandle, IntPtr interfaze)
		{
			jvalue[] array = new jvalue[2];
			array[0].i = delegateHandle;
			array[1].l = interfaze;
			return AndroidJNISafe.CallStaticObjectMethod(s_ReflectionHelperClass, s_ReflectionHelperNewProxyInstance, array);
		}
	}
}
