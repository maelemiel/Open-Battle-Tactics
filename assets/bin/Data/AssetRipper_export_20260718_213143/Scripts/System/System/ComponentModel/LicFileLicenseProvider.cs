using System.IO;

namespace System.ComponentModel
{
	public class LicFileLicenseProvider : LicenseProvider
	{
		public override License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions)
		{
			try
			{
				if (context == null || context.UsageMode != LicenseUsageMode.Designtime)
				{
					return null;
				}
				string directoryName = Path.GetDirectoryName(type.Assembly.Location);
				directoryName = Path.Combine(directoryName, type.FullName + ".LIC");
				if (!File.Exists(directoryName))
				{
					return null;
				}
				StreamReader streamReader = new StreamReader(directoryName);
				string key = streamReader.ReadLine();
				streamReader.Close();
				if (IsKeyValid(key, type))
				{
					return new System.ComponentModel.LicFileLicense(key);
				}
			}
			catch
			{
				if (allowExceptions)
				{
					throw;
				}
			}
			return null;
		}

		protected virtual string GetKey(Type type)
		{
			return type.FullName + " is a licensed component.";
		}

		protected virtual bool IsKeyValid(string key, Type type)
		{
			if (key == null)
			{
				return false;
			}
			return key.Equals(GetKey(type));
		}
	}
}
