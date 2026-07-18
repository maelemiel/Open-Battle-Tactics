using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public struct ParameterModifier
	{
		private bool[] _byref;

		public bool this[int index]
		{
			get
			{
				return _byref[index];
			}
			set
			{
				_byref[index] = value;
			}
		}

		public ParameterModifier(int parameterCount)
		{
			if (parameterCount <= 0)
			{
				throw new ArgumentException("Must specify one or more parameters.");
			}
			_byref = new bool[parameterCount];
		}
	}
}
