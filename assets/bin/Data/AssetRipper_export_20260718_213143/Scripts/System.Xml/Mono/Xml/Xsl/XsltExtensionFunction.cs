using System;
using System.Reflection;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Mono.Xml.Xsl
{
	internal class XsltExtensionFunction : XPFuncImpl
	{
		private object extension;

		private MethodInfo method;

		private TypeCode[] typeCodes;

		public XsltExtensionFunction(object extension, MethodInfo method, XPathNavigator currentNode)
		{
			this.extension = extension;
			this.method = method;
			ParameterInfo[] parameters = method.GetParameters();
			int num = parameters.Length;
			int maxArgs = parameters.Length;
			typeCodes = new TypeCode[parameters.Length];
			XPathResultType[] array = new XPathResultType[parameters.Length];
			bool flag = true;
			int num2 = parameters.Length - 1;
			while (0 <= num2)
			{
				typeCodes[num2] = Type.GetTypeCode(parameters[num2].ParameterType);
				array[num2] = XPFuncImpl.GetXPathType(parameters[num2].ParameterType, currentNode);
				if (flag)
				{
					if (parameters[num2].IsOptional)
					{
						num--;
					}
					else
					{
						flag = false;
					}
				}
				num2--;
			}
			Init(num, maxArgs, XPFuncImpl.GetXPathType(method.ReturnType, currentNode), array);
		}

		public override object Invoke(XsltCompiledContext xsltContext, object[] args, XPathNavigator docContext)
		{
			try
			{
				ParameterInfo[] parameters = method.GetParameters();
				object[] array = new object[parameters.Length];
				for (int i = 0; i < args.Length; i++)
				{
					Type parameterType = parameters[i].ParameterType;
					switch (parameterType.FullName)
					{
					case "System.Int16":
					case "System.UInt16":
					case "System.Int32":
					case "System.UInt32":
					case "System.Int64":
					case "System.UInt64":
					case "System.Single":
					case "System.Decimal":
						array[i] = Convert.ChangeType(args[i], parameterType);
						break;
					default:
						array[i] = args[i];
						break;
					}
				}
				object obj = null;
				switch (method.ReturnType.FullName)
				{
				case "System.Int16":
				case "System.UInt16":
				case "System.Int32":
				case "System.UInt32":
				case "System.Int64":
				case "System.UInt64":
				case "System.Single":
				case "System.Decimal":
					obj = Convert.ChangeType(method.Invoke(extension, array), typeof(double));
					break;
				default:
					obj = method.Invoke(extension, array);
					break;
				}
				IXPathNavigable iXPathNavigable = obj as IXPathNavigable;
				if (iXPathNavigable != null)
				{
					return iXPathNavigable.CreateNavigator();
				}
				return obj;
			}
			catch (Exception innerException)
			{
				throw new XsltException("Custom function reported an error.", innerException);
			}
		}
	}
}
