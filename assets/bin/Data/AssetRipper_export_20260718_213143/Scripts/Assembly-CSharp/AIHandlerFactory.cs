using System;
using System.Collections.Generic;

public class AIHandlerFactory
{
	private static Dictionary<string, Func<BaseAI>> factoryFunctions;

	private static void Init()
	{
		factoryFunctions = new Dictionary<string, Func<BaseAI>>();
		RegisterType("simple_ai", () => new SimpleAI());
		RegisterType("simple_ai_mod01", () => new SimpleAI_mod01());
		RegisterType("simple_ai_mod02", () => new SimpleAI_mod02());
		RegisterType("simple_ai_boss", () => new SimpleAI_boss());
	}

	private static void RegisterType(string aiType, Func<BaseAI> factoryFunc)
	{
		factoryFunctions[aiType] = factoryFunc;
	}

	public static BaseAI Create(string aiType)
	{
		if (factoryFunctions == null)
		{
			Init();
		}
		if (factoryFunctions.ContainsKey(aiType))
		{
			Func<BaseAI> func = factoryFunctions[aiType];
			return func();
		}
		return null;
	}
}
