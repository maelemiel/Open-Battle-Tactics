using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[ComVisible(true)]
	public class RegionInfo
	{
		private static RegionInfo currentRegion;

		private int lcid;

		private int regionId;

		private string iso2Name;

		private string iso3Name;

		private string win3Name;

		private string englishName;

		private string currencySymbol;

		private string isoCurrencySymbol;

		private string currencyEnglishName;

		public static RegionInfo CurrentRegion
		{
			get
			{
				if (currentRegion == null)
				{
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					if (currentCulture == null || CultureInfo.BootstrapCultureID == 127)
					{
						return null;
					}
					currentRegion = new RegionInfo(CultureInfo.BootstrapCultureID);
				}
				return currentRegion;
			}
		}

		[ComVisible(false)]
		public virtual string CurrencyEnglishName
		{
			get
			{
				return currencyEnglishName;
			}
		}

		public virtual string CurrencySymbol
		{
			get
			{
				return currencySymbol;
			}
		}

		[MonoTODO("DisplayName currently only returns the EnglishName")]
		public virtual string DisplayName
		{
			get
			{
				return englishName;
			}
		}

		public virtual string EnglishName
		{
			get
			{
				return englishName;
			}
		}

		[ComVisible(false)]
		public virtual int GeoId
		{
			get
			{
				return regionId;
			}
		}

		public virtual bool IsMetric
		{
			get
			{
				switch (iso2Name)
				{
				case "US":
				case "UK":
					return false;
				default:
					return true;
				}
			}
		}

		public virtual string ISOCurrencySymbol
		{
			get
			{
				return isoCurrencySymbol;
			}
		}

		[ComVisible(false)]
		public virtual string NativeName
		{
			get
			{
				return DisplayName;
			}
		}

		[ComVisible(false)]
		[MonoTODO("Not implemented")]
		public virtual string CurrencyNativeName
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual string Name
		{
			get
			{
				return iso2Name;
			}
		}

		public virtual string ThreeLetterISORegionName
		{
			get
			{
				return iso3Name;
			}
		}

		public virtual string ThreeLetterWindowsRegionName
		{
			get
			{
				return win3Name;
			}
		}

		public virtual string TwoLetterISORegionName
		{
			get
			{
				return iso2Name;
			}
		}

		public RegionInfo(int culture)
		{
			if (!GetByTerritory(CultureInfo.GetCultureInfo(culture)))
			{
				throw new ArgumentException(string.Format("Region ID {0} (0x{0:X4}) is not a supported region.", culture), "culture");
			}
		}

		public RegionInfo(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			if (construct_internal_region_from_name(name.ToUpperInvariant()))
			{
				lcid = name.GetHashCode();
			}
			else if (!GetByTerritory(CultureInfo.GetCultureInfo(name)))
			{
				throw new ArgumentException(string.Format("Region name {0} is not supported.", name), "name");
			}
		}

		private bool GetByTerritory(CultureInfo ci)
		{
			if (ci == null)
			{
				throw new Exception("INTERNAL ERROR: should not happen.");
			}
			if (ci.IsNeutralCulture || ci.Territory == null)
			{
				return false;
			}
			lcid = ci.LCID;
			return construct_internal_region_from_name(ci.Territory.ToUpperInvariant());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool construct_internal_region_from_name(string name);

		public override bool Equals(object value)
		{
			RegionInfo regionInfo = value as RegionInfo;
			return regionInfo != null && lcid == regionInfo.lcid;
		}

		public override int GetHashCode()
		{
			return (int)(2147483648u + (regionId << 3) + regionId);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
