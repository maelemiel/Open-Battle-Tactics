using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Mono.Data.Sqlite
{
	public abstract class SqliteFunction : IDisposable
	{
		private class AggregateData
		{
			internal int _count = 1;

			internal object _data;
		}

		internal SQLiteBase _base;

		private Dictionary<long, AggregateData> _contextDataList;

		private SQLiteCallback _InvokeFunc;

		private SQLiteCallback _StepFunc;

		private SQLiteFinalCallback _FinalFunc;

		private SQLiteCollation _CompareFunc;

		private SQLiteCollation _CompareFunc16;

		internal IntPtr _context;

		private static List<SqliteFunctionAttribute> _registeredFunctions;

		public SqliteConvert SqliteConvert
		{
			get
			{
				return _base;
			}
		}

		protected SqliteFunction()
		{
			_contextDataList = new Dictionary<long, AggregateData>();
		}

		static SqliteFunction()
		{
			_registeredFunctions = new List<SqliteFunctionAttribute>();
			try
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				int num = assemblies.Length;
				AssemblyName name = Assembly.GetCallingAssembly().GetName();
				for (int i = 0; i < num; i++)
				{
					bool flag = false;
					Type[] types;
					try
					{
						AssemblyName[] referencedAssemblies = assemblies[i].GetReferencedAssemblies();
						int num2 = referencedAssemblies.Length;
						for (int j = 0; j < num2; j++)
						{
							if (referencedAssemblies[j].Name == name.Name)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							continue;
						}
						types = assemblies[i].GetTypes();
						goto IL_00a5;
					}
					catch (ReflectionTypeLoadException ex)
					{
						types = ex.Types;
						goto IL_00a5;
					}
					IL_00a5:
					int num3 = types.Length;
					for (int k = 0; k < num3; k++)
					{
						if (types[k] == null)
						{
							continue;
						}
						object[] customAttributes = types[k].GetCustomAttributes(typeof(SqliteFunctionAttribute), false);
						int num4 = customAttributes.Length;
						for (int l = 0; l < num4; l++)
						{
							SqliteFunctionAttribute sqliteFunctionAttribute = customAttributes[l] as SqliteFunctionAttribute;
							if (sqliteFunctionAttribute != null)
							{
								sqliteFunctionAttribute._instanceType = types[k];
								_registeredFunctions.Add(sqliteFunctionAttribute);
							}
						}
					}
				}
			}
			catch
			{
			}
		}

		public virtual object Invoke(object[] args)
		{
			return null;
		}

		public virtual void Step(object[] args, int stepNumber, ref object contextData)
		{
		}

		public virtual object Final(object contextData)
		{
			return null;
		}

		public virtual int Compare(string param1, string param2)
		{
			return 0;
		}

		internal object[] ConvertParams(int nArgs, IntPtr argsptr)
		{
			object[] array = new object[nArgs];
			IntPtr[] array2 = new IntPtr[nArgs];
			Marshal.Copy(argsptr, array2, 0, nArgs);
			for (int i = 0; i < nArgs; i++)
			{
				switch (_base.GetParamValueType(array2[i]))
				{
				case TypeAffinity.Null:
					array[i] = DBNull.Value;
					break;
				case TypeAffinity.Int64:
					array[i] = _base.GetParamValueInt64(array2[i]);
					break;
				case TypeAffinity.Double:
					array[i] = _base.GetParamValueDouble(array2[i]);
					break;
				case TypeAffinity.Text:
					array[i] = _base.GetParamValueText(array2[i]);
					break;
				case TypeAffinity.Blob:
				{
					int num = (int)_base.GetParamValueBytes(array2[i], 0, null, 0, 0);
					byte[] array3 = new byte[num];
					_base.GetParamValueBytes(array2[i], 0, array3, 0, num);
					array[i] = array3;
					break;
				}
				case TypeAffinity.DateTime:
					array[i] = _base.ToDateTime(_base.GetParamValueText(array2[i]));
					break;
				}
			}
			return array;
		}

		private void SetReturnValue(IntPtr context, object returnValue)
		{
			if (returnValue == null || returnValue == DBNull.Value)
			{
				_base.ReturnNull(context);
				return;
			}
			Type type = returnValue.GetType();
			if (type == typeof(DateTime))
			{
				_base.ReturnText(context, _base.ToString((DateTime)returnValue));
				return;
			}
			Exception ex = returnValue as Exception;
			if (ex != null)
			{
				_base.ReturnError(context, ex.Message);
				return;
			}
			switch (SqliteConvert.TypeToAffinity(type))
			{
			case TypeAffinity.Null:
				_base.ReturnNull(context);
				break;
			case TypeAffinity.Int64:
				_base.ReturnInt64(context, Convert.ToInt64(returnValue, CultureInfo.CurrentCulture));
				break;
			case TypeAffinity.Double:
				_base.ReturnDouble(context, Convert.ToDouble(returnValue, CultureInfo.CurrentCulture));
				break;
			case TypeAffinity.Text:
				_base.ReturnText(context, returnValue.ToString());
				break;
			case TypeAffinity.Blob:
				_base.ReturnBlob(context, (byte[])returnValue);
				break;
			}
		}

		internal void ScalarCallback(IntPtr context, int nArgs, IntPtr argsptr)
		{
			_context = context;
			SetReturnValue(context, Invoke(ConvertParams(nArgs, argsptr)));
		}

		internal int CompareCallback(IntPtr ptr, int len1, IntPtr ptr1, int len2, IntPtr ptr2)
		{
			return Compare(SqliteConvert.UTF8ToString(ptr1, len1), SqliteConvert.UTF8ToString(ptr2, len2));
		}

		internal int CompareCallback16(IntPtr ptr, int len1, IntPtr ptr1, int len2, IntPtr ptr2)
		{
			return Compare(SQLite3_UTF16.UTF16ToString(ptr1, len1), SQLite3_UTF16.UTF16ToString(ptr2, len2));
		}

		internal void StepCallback(IntPtr context, int nArgs, IntPtr argsptr)
		{
			long key = (long)_base.AggregateContext(context);
			AggregateData value;
			if (!_contextDataList.TryGetValue(key, out value))
			{
				value = new AggregateData();
				_contextDataList[key] = value;
			}
			try
			{
				_context = context;
				Step(ConvertParams(nArgs, argsptr), value._count, ref value._data);
			}
			finally
			{
				value._count++;
			}
		}

		internal void FinalCallback(IntPtr context)
		{
			long key = (long)_base.AggregateContext(context);
			object obj = null;
			if (_contextDataList.ContainsKey(key))
			{
				obj = _contextDataList[key]._data;
				_contextDataList.Remove(key);
			}
			_context = context;
			SetReturnValue(context, Final(obj));
			IDisposable disposable = obj as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			foreach (KeyValuePair<long, AggregateData> contextData in _contextDataList)
			{
				IDisposable disposable = contextData.Value._data as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			_contextDataList.Clear();
			_InvokeFunc = null;
			_StepFunc = null;
			_FinalFunc = null;
			_CompareFunc = null;
			_base = null;
			_contextDataList = null;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public static void RegisterFunction(Type typ)
		{
			object[] customAttributes = typ.GetCustomAttributes(typeof(SqliteFunctionAttribute), false);
			int num = customAttributes.Length;
			for (int i = 0; i < num; i++)
			{
				SqliteFunctionAttribute sqliteFunctionAttribute = customAttributes[i] as SqliteFunctionAttribute;
				if (sqliteFunctionAttribute != null)
				{
					sqliteFunctionAttribute._instanceType = typ;
					_registeredFunctions.Add(sqliteFunctionAttribute);
				}
			}
		}

		internal static SqliteFunction[] BindFunctions(SQLiteBase sqlbase)
		{
			List<SqliteFunction> list = new List<SqliteFunction>();
			foreach (SqliteFunctionAttribute registeredFunction in _registeredFunctions)
			{
				SqliteFunction sqliteFunction = (SqliteFunction)Activator.CreateInstance(registeredFunction._instanceType);
				sqliteFunction._base = sqlbase;
				sqliteFunction._InvokeFunc = ((registeredFunction.FuncType != FunctionType.Scalar) ? null : new SQLiteCallback(sqliteFunction.ScalarCallback));
				sqliteFunction._StepFunc = ((registeredFunction.FuncType != FunctionType.Aggregate) ? null : new SQLiteCallback(sqliteFunction.StepCallback));
				sqliteFunction._FinalFunc = ((registeredFunction.FuncType != FunctionType.Aggregate) ? null : new SQLiteFinalCallback(sqliteFunction.FinalCallback));
				sqliteFunction._CompareFunc = ((registeredFunction.FuncType != FunctionType.Collation) ? null : new SQLiteCollation(sqliteFunction.CompareCallback));
				sqliteFunction._CompareFunc16 = ((registeredFunction.FuncType != FunctionType.Collation) ? null : new SQLiteCollation(sqliteFunction.CompareCallback16));
				if (registeredFunction.FuncType != FunctionType.Collation)
				{
					sqlbase.CreateFunction(registeredFunction.Name, registeredFunction.Arguments, sqliteFunction is SqliteFunctionEx, sqliteFunction._InvokeFunc, sqliteFunction._StepFunc, sqliteFunction._FinalFunc);
				}
				else
				{
					sqlbase.CreateCollation(registeredFunction.Name, sqliteFunction._CompareFunc, sqliteFunction._CompareFunc16);
				}
				list.Add(sqliteFunction);
			}
			SqliteFunction[] array = new SqliteFunction[list.Count];
			list.CopyTo(array, 0);
			return array;
		}
	}
}
