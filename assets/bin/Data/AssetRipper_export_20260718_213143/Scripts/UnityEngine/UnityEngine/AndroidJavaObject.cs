using System;
using System.Text;

namespace UnityEngine
{
	public class AndroidJavaObject : IDisposable
	{
		private static bool enableDebugPrints;

		private bool m_disposed;

		protected IntPtr m_jobject;

		protected IntPtr m_jclass;

		private static AndroidJavaClass s_JavaLangClass;

		protected static AndroidJavaClass JavaLangClass
		{
			get
			{
				if (s_JavaLangClass == null)
				{
					s_JavaLangClass = new AndroidJavaClass(AndroidJNISafe.FindClass("java/lang/Class"));
				}
				return s_JavaLangClass;
			}
		}

		internal AndroidJavaObject(IntPtr jobject)
			: this()
		{
			if (jobject == IntPtr.Zero)
			{
				throw new Exception("JNI: Init'd AndroidJavaObject with null ptr!");
			}
			IntPtr objectClass = AndroidJNISafe.GetObjectClass(jobject);
			m_jobject = AndroidJNI.NewGlobalRef(jobject);
			m_jclass = AndroidJNI.NewGlobalRef(objectClass);
			AndroidJNISafe.DeleteLocalRef(objectClass);
		}

		internal AndroidJavaObject()
		{
		}

		public AndroidJavaObject(string className, params object[] args)
			: this()
		{
			_AndroidJavaObject(className, args);
		}

		protected void DebugPrint(string msg)
		{
			if (enableDebugPrints)
			{
				Debug.Log(msg);
			}
		}

		protected void DebugPrint(string call, string methodName, string signature, object[] args)
		{
			if (enableDebugPrints)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (object obj in args)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append((obj != null) ? obj.GetType().ToString() : "<null>");
				}
				Debug.Log(call + "(\"" + methodName + "\"" + stringBuilder.ToString() + ") = " + signature);
			}
		}

		private void _AndroidJavaObject(string className, params object[] args)
		{
			DebugPrint("Creating AndroidJavaObject from " + className);
			if (args == null)
			{
				args = new object[1];
			}
			using (AndroidJavaObject androidJavaObject = FindClass(className))
			{
				m_jclass = AndroidJNI.NewGlobalRef(androidJavaObject.GetRawObject());
				jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
				try
				{
					IntPtr constructorID = AndroidJNIHelper.GetConstructorID(m_jclass, args);
					IntPtr intPtr = AndroidJNISafe.NewObject(m_jclass, constructorID, array);
					m_jobject = AndroidJNI.NewGlobalRef(intPtr);
					AndroidJNISafe.DeleteLocalRef(intPtr);
				}
				finally
				{
					AndroidJNIHelper.DeleteJNIArgArray(args, array);
				}
			}
		}

		~AndroidJavaObject()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_disposed)
			{
				m_disposed = true;
				AndroidJNISafe.DeleteGlobalRef(m_jobject);
				AndroidJNISafe.DeleteGlobalRef(m_jclass);
			}
		}

		protected void _Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void _Call(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID(m_jclass, methodName, args, false);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				AndroidJNISafe.CallVoidMethod(m_jobject, methodID, array);
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		protected ReturnType _Call<ReturnType>(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(m_jclass, methodName, args, false);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				if (typeof(ReturnType).IsPrimitive)
				{
					if (typeof(ReturnType) == typeof(int))
					{
						return (ReturnType)(object)AndroidJNISafe.CallIntMethod(m_jobject, methodID, array);
					}
					if (typeof(ReturnType) == typeof(bool))
					{
						return (ReturnType)(object)AndroidJNISafe.CallBooleanMethod(m_jobject, methodID, array);
					}
					if (typeof(ReturnType) == typeof(byte))
					{
						return (ReturnType)(object)AndroidJNISafe.CallByteMethod(m_jobject, methodID, array);
					}
					if (typeof(ReturnType) == typeof(short))
					{
						return (ReturnType)(object)AndroidJNISafe.CallShortMethod(m_jobject, methodID, array);
					}
					if (typeof(ReturnType) == typeof(long))
					{
						return (ReturnType)(object)AndroidJNISafe.CallLongMethod(m_jobject, methodID, array);
					}
					if (typeof(ReturnType) == typeof(float))
					{
						return (ReturnType)(object)AndroidJNISafe.CallFloatMethod(m_jobject, methodID, array);
					}
					if (typeof(ReturnType) == typeof(double))
					{
						return (ReturnType)(object)AndroidJNISafe.CallDoubleMethod(m_jobject, methodID, array);
					}
					if (typeof(ReturnType) == typeof(char))
					{
						return (ReturnType)(object)AndroidJNISafe.CallCharMethod(m_jobject, methodID, array);
					}
					return default(ReturnType);
				}
				if (typeof(ReturnType) == typeof(string))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStringMethod(m_jobject, methodID, array);
				}
				if (typeof(ReturnType) == typeof(AndroidJavaClass))
				{
					IntPtr jclass = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
					return (ReturnType)(object)AndroidJavaClassDeleteLocalRef(jclass);
				}
				if (typeof(ReturnType) == typeof(AndroidJavaObject))
				{
					IntPtr jobject = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
					return (ReturnType)(object)AndroidJavaObjectDeleteLocalRef(jobject);
				}
				if (typeof(Array).IsAssignableFrom(typeof(ReturnType)))
				{
					IntPtr array2 = AndroidJNISafe.CallObjectMethod(m_jobject, methodID, array);
					return (ReturnType)(object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(array2);
				}
				throw new Exception(string.Concat("JNI: Unknown return type '", typeof(ReturnType), "'"));
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		protected FieldType _Get<FieldType>(string fieldName)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, false);
			if (typeof(FieldType).IsPrimitive)
			{
				if (typeof(FieldType) == typeof(int))
				{
					return (FieldType)(object)AndroidJNISafe.GetIntField(m_jobject, fieldID);
				}
				if (typeof(FieldType) == typeof(bool))
				{
					return (FieldType)(object)AndroidJNISafe.GetBooleanField(m_jobject, fieldID);
				}
				if (typeof(FieldType) == typeof(byte))
				{
					return (FieldType)(object)AndroidJNISafe.GetByteField(m_jobject, fieldID);
				}
				if (typeof(FieldType) == typeof(short))
				{
					return (FieldType)(object)AndroidJNISafe.GetShortField(m_jobject, fieldID);
				}
				if (typeof(FieldType) == typeof(long))
				{
					return (FieldType)(object)AndroidJNISafe.GetLongField(m_jobject, fieldID);
				}
				if (typeof(FieldType) == typeof(float))
				{
					return (FieldType)(object)AndroidJNISafe.GetFloatField(m_jobject, fieldID);
				}
				if (typeof(FieldType) == typeof(double))
				{
					return (FieldType)(object)AndroidJNISafe.GetDoubleField(m_jobject, fieldID);
				}
				if (typeof(FieldType) == typeof(char))
				{
					return (FieldType)(object)AndroidJNISafe.GetCharField(m_jobject, fieldID);
				}
				return default(FieldType);
			}
			if (typeof(FieldType) == typeof(string))
			{
				return (FieldType)(object)AndroidJNISafe.GetStringField(m_jobject, fieldID);
			}
			if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				IntPtr objectField = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
				return (FieldType)(object)AndroidJavaClassDeleteLocalRef(objectField);
			}
			if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				IntPtr objectField2 = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
				return (FieldType)(object)AndroidJavaObjectDeleteLocalRef(objectField2);
			}
			if (typeof(Array).IsAssignableFrom(typeof(FieldType)))
			{
				IntPtr objectField3 = AndroidJNISafe.GetObjectField(m_jobject, fieldID);
				return (FieldType)(object)AndroidJNIHelper.ConvertFromJNIArray<FieldType>(objectField3);
			}
			throw new Exception(string.Concat("JNI: Unknown field type '", typeof(FieldType), "'"));
		}

		protected void _Set<FieldType>(string fieldName, FieldType val)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, false);
			if (typeof(FieldType).IsPrimitive)
			{
				if (typeof(FieldType) == typeof(int))
				{
					AndroidJNISafe.SetIntField(m_jobject, fieldID, (int)(object)val);
				}
				else if (typeof(FieldType) == typeof(bool))
				{
					AndroidJNISafe.SetBooleanField(m_jobject, fieldID, (bool)(object)val);
				}
				else if (typeof(FieldType) == typeof(byte))
				{
					AndroidJNISafe.SetByteField(m_jobject, fieldID, (byte)(object)val);
				}
				else if (typeof(FieldType) == typeof(short))
				{
					AndroidJNISafe.SetShortField(m_jobject, fieldID, (short)(object)val);
				}
				else if (typeof(FieldType) == typeof(long))
				{
					AndroidJNISafe.SetLongField(m_jobject, fieldID, (long)(object)val);
				}
				else if (typeof(FieldType) == typeof(float))
				{
					AndroidJNISafe.SetFloatField(m_jobject, fieldID, (float)(object)val);
				}
				else if (typeof(FieldType) == typeof(double))
				{
					AndroidJNISafe.SetDoubleField(m_jobject, fieldID, (double)(object)val);
				}
				else if (typeof(FieldType) == typeof(char))
				{
					AndroidJNISafe.SetCharField(m_jobject, fieldID, (char)(object)val);
				}
			}
			else if (typeof(FieldType) == typeof(string))
			{
				AndroidJNISafe.SetStringField(m_jobject, fieldID, (string)(object)val);
			}
			else if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				AndroidJNISafe.SetObjectField(m_jobject, fieldID, ((AndroidJavaClass)(object)val).m_jclass);
			}
			else if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				AndroidJNISafe.SetObjectField(m_jobject, fieldID, ((AndroidJavaObject)(object)val).m_jobject);
			}
			else
			{
				if (!typeof(Array).IsAssignableFrom(typeof(FieldType)))
				{
					throw new Exception(string.Concat("JNI: Unknown field type '", typeof(FieldType), "'"));
				}
				IntPtr val2 = AndroidJNIHelper.ConvertToJNIArray((Array)(object)val);
				AndroidJNISafe.SetObjectField(m_jclass, fieldID, val2);
			}
		}

		protected void _CallStatic(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID(m_jclass, methodName, args, true);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				AndroidJNISafe.CallStaticVoidMethod(m_jclass, methodID, array);
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		protected ReturnType _CallStatic<ReturnType>(string methodName, params object[] args)
		{
			if (args == null)
			{
				args = new object[1];
			}
			IntPtr methodID = AndroidJNIHelper.GetMethodID<ReturnType>(m_jclass, methodName, args, true);
			jvalue[] array = AndroidJNIHelper.CreateJNIArgArray(args);
			try
			{
				if (typeof(ReturnType).IsPrimitive)
				{
					if (typeof(ReturnType) == typeof(int))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticIntMethod(m_jclass, methodID, array);
					}
					if (typeof(ReturnType) == typeof(bool))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticBooleanMethod(m_jclass, methodID, array);
					}
					if (typeof(ReturnType) == typeof(byte))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticByteMethod(m_jclass, methodID, array);
					}
					if (typeof(ReturnType) == typeof(short))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticShortMethod(m_jclass, methodID, array);
					}
					if (typeof(ReturnType) == typeof(long))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticLongMethod(m_jclass, methodID, array);
					}
					if (typeof(ReturnType) == typeof(float))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticFloatMethod(m_jclass, methodID, array);
					}
					if (typeof(ReturnType) == typeof(double))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticDoubleMethod(m_jclass, methodID, array);
					}
					if (typeof(ReturnType) == typeof(char))
					{
						return (ReturnType)(object)AndroidJNISafe.CallStaticCharMethod(m_jclass, methodID, array);
					}
					return default(ReturnType);
				}
				if (typeof(ReturnType) == typeof(string))
				{
					return (ReturnType)(object)AndroidJNISafe.CallStaticStringMethod(m_jclass, methodID, array);
				}
				if (typeof(ReturnType) == typeof(AndroidJavaClass))
				{
					IntPtr jclass = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
					return (ReturnType)(object)AndroidJavaClassDeleteLocalRef(jclass);
				}
				if (typeof(ReturnType) == typeof(AndroidJavaObject))
				{
					IntPtr jobject = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
					return (ReturnType)(object)AndroidJavaObjectDeleteLocalRef(jobject);
				}
				if (typeof(Array).IsAssignableFrom(typeof(ReturnType)))
				{
					IntPtr array2 = AndroidJNISafe.CallStaticObjectMethod(m_jclass, methodID, array);
					return (ReturnType)(object)AndroidJNIHelper.ConvertFromJNIArray<ReturnType>(array2);
				}
				throw new Exception(string.Concat("JNI: Unknown return type '", typeof(ReturnType), "'"));
			}
			finally
			{
				AndroidJNIHelper.DeleteJNIArgArray(args, array);
			}
		}

		protected FieldType _GetStatic<FieldType>(string fieldName)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, true);
			if (typeof(FieldType).IsPrimitive)
			{
				if (typeof(FieldType) == typeof(int))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticIntField(m_jclass, fieldID);
				}
				if (typeof(FieldType) == typeof(bool))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticBooleanField(m_jclass, fieldID);
				}
				if (typeof(FieldType) == typeof(byte))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticByteField(m_jclass, fieldID);
				}
				if (typeof(FieldType) == typeof(short))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticShortField(m_jclass, fieldID);
				}
				if (typeof(FieldType) == typeof(long))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticLongField(m_jclass, fieldID);
				}
				if (typeof(FieldType) == typeof(float))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticFloatField(m_jclass, fieldID);
				}
				if (typeof(FieldType) == typeof(double))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticDoubleField(m_jclass, fieldID);
				}
				if (typeof(FieldType) == typeof(char))
				{
					return (FieldType)(object)AndroidJNISafe.GetStaticCharField(m_jclass, fieldID);
				}
				return default(FieldType);
			}
			if (typeof(FieldType) == typeof(string))
			{
				return (FieldType)(object)AndroidJNISafe.GetStaticStringField(m_jclass, fieldID);
			}
			if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				IntPtr staticObjectField = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
				return (FieldType)(object)AndroidJavaClassDeleteLocalRef(staticObjectField);
			}
			if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				IntPtr staticObjectField2 = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
				return (FieldType)(object)AndroidJavaObjectDeleteLocalRef(staticObjectField2);
			}
			if (typeof(Array).IsAssignableFrom(typeof(FieldType)))
			{
				IntPtr staticObjectField3 = AndroidJNISafe.GetStaticObjectField(m_jclass, fieldID);
				return (FieldType)(object)AndroidJNIHelper.ConvertFromJNIArray<FieldType>(staticObjectField3);
			}
			throw new Exception(string.Concat("JNI: Unknown field type '", typeof(FieldType), "'"));
		}

		protected void _SetStatic<FieldType>(string fieldName, FieldType val)
		{
			IntPtr fieldID = AndroidJNIHelper.GetFieldID<FieldType>(m_jclass, fieldName, true);
			if (typeof(FieldType).IsPrimitive)
			{
				if (typeof(FieldType) == typeof(int))
				{
					AndroidJNISafe.SetStaticIntField(m_jclass, fieldID, (int)(object)val);
				}
				else if (typeof(FieldType) == typeof(bool))
				{
					AndroidJNISafe.SetStaticBooleanField(m_jclass, fieldID, (bool)(object)val);
				}
				else if (typeof(FieldType) == typeof(byte))
				{
					AndroidJNISafe.SetStaticByteField(m_jclass, fieldID, (byte)(object)val);
				}
				else if (typeof(FieldType) == typeof(short))
				{
					AndroidJNISafe.SetStaticShortField(m_jclass, fieldID, (short)(object)val);
				}
				else if (typeof(FieldType) == typeof(long))
				{
					AndroidJNISafe.SetStaticLongField(m_jclass, fieldID, (long)(object)val);
				}
				else if (typeof(FieldType) == typeof(float))
				{
					AndroidJNISafe.SetStaticFloatField(m_jclass, fieldID, (float)(object)val);
				}
				else if (typeof(FieldType) == typeof(double))
				{
					AndroidJNISafe.SetStaticDoubleField(m_jclass, fieldID, (double)(object)val);
				}
				else if (typeof(FieldType) == typeof(char))
				{
					AndroidJNISafe.SetStaticCharField(m_jclass, fieldID, (char)(object)val);
				}
			}
			else if (typeof(FieldType) == typeof(string))
			{
				AndroidJNISafe.SetStaticStringField(m_jclass, fieldID, (string)(object)val);
			}
			else if (typeof(FieldType) == typeof(AndroidJavaClass))
			{
				AndroidJNISafe.SetStaticObjectField(m_jclass, fieldID, ((AndroidJavaClass)(object)val).m_jclass);
			}
			else if (typeof(FieldType) == typeof(AndroidJavaObject))
			{
				AndroidJNISafe.SetStaticObjectField(m_jclass, fieldID, ((AndroidJavaObject)(object)val).m_jobject);
			}
			else
			{
				if (!typeof(Array).IsAssignableFrom(typeof(FieldType)))
				{
					throw new Exception(string.Concat("JNI: Unknown field type '", typeof(FieldType), "'"));
				}
				IntPtr val2 = AndroidJNIHelper.ConvertToJNIArray((Array)(object)val);
				AndroidJNISafe.SetStaticObjectField(m_jclass, fieldID, val2);
			}
		}

		internal static AndroidJavaObject AndroidJavaObjectDeleteLocalRef(IntPtr jobject)
		{
			try
			{
				return new AndroidJavaObject(jobject);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(jobject);
			}
		}

		internal static AndroidJavaClass AndroidJavaClassDeleteLocalRef(IntPtr jclass)
		{
			try
			{
				return new AndroidJavaClass(jclass);
			}
			finally
			{
				AndroidJNISafe.DeleteLocalRef(jclass);
			}
		}

		protected IntPtr _GetRawObject()
		{
			return m_jobject;
		}

		protected IntPtr _GetRawClass()
		{
			return m_jclass;
		}

		protected static AndroidJavaObject FindClass(string name)
		{
			return JavaLangClass.CallStatic<AndroidJavaObject>("forName", new object[1] { name.Replace('/', '.') });
		}

		public void Dispose()
		{
			_Dispose();
		}

		public void Call(string methodName, params object[] args)
		{
			_Call(methodName, args);
		}

		public void CallStatic(string methodName, params object[] args)
		{
			_CallStatic(methodName, args);
		}

		public FieldType Get<FieldType>(string fieldName)
		{
			return _Get<FieldType>(fieldName);
		}

		public void Set<FieldType>(string fieldName, FieldType val)
		{
			_Set(fieldName, val);
		}

		public FieldType GetStatic<FieldType>(string fieldName)
		{
			return _GetStatic<FieldType>(fieldName);
		}

		public void SetStatic<FieldType>(string fieldName, FieldType val)
		{
			_SetStatic(fieldName, val);
		}

		public IntPtr GetRawObject()
		{
			return _GetRawObject();
		}

		public IntPtr GetRawClass()
		{
			return _GetRawClass();
		}

		public ReturnType Call<ReturnType>(string methodName, params object[] args)
		{
			return _Call<ReturnType>(methodName, args);
		}

		public ReturnType CallStatic<ReturnType>(string methodName, params object[] args)
		{
			return _CallStatic<ReturnType>(methodName, args);
		}
	}
}
