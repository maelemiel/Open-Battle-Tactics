using System.Runtime.InteropServices;

namespace System.Configuration.Assemblies
{
	[Serializable]
	[Obsolete]
	[ComVisible(true)]
	public struct AssemblyHash : ICloneable
	{
		private AssemblyHashAlgorithm _algorithm;

		private byte[] _value;

		[Obsolete]
		public static readonly AssemblyHash Empty = new AssemblyHash(AssemblyHashAlgorithm.None, null);

		[Obsolete]
		public AssemblyHashAlgorithm Algorithm
		{
			get
			{
				return _algorithm;
			}
			set
			{
				_algorithm = value;
			}
		}

		[Obsolete]
		public AssemblyHash(AssemblyHashAlgorithm algorithm, byte[] value)
		{
			_algorithm = algorithm;
			if (value != null)
			{
				_value = (byte[])value.Clone();
			}
			else
			{
				_value = null;
			}
		}

		[Obsolete]
		public AssemblyHash(byte[] value)
			: this(AssemblyHashAlgorithm.SHA1, value)
		{
		}

		[Obsolete]
		public object Clone()
		{
			return new AssemblyHash(_algorithm, _value);
		}

		[Obsolete]
		public byte[] GetValue()
		{
			return _value;
		}

		[Obsolete]
		public void SetValue(byte[] value)
		{
			_value = value;
		}
	}
}
