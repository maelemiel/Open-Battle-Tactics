using System.Collections.Generic;

namespace System.Security.AccessControl
{
	public sealed class RawAcl : GenericAcl
	{
		private byte revision;

		private List<GenericAce> list;

		[MonoTODO]
		public override int BinaryLength
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override int Count
		{
			get
			{
				return list.Count;
			}
		}

		public override GenericAce this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				list[index] = value;
			}
		}

		public override byte Revision
		{
			get
			{
				return revision;
			}
		}

		public RawAcl(byte revision, int capacity)
		{
			this.revision = revision;
			list = new List<GenericAce>(capacity);
		}

		public RawAcl(byte[] binaryForm, int offset)
			: this(0, 10)
		{
		}

		[MonoTODO]
		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			throw new NotImplementedException();
		}

		public void InsertAce(int index, GenericAce ace)
		{
			if (ace == null)
			{
				throw new ArgumentNullException("ace");
			}
			list.Insert(index, ace);
		}

		public void RemoveAce(int index)
		{
			list.RemoveAt(index);
		}
	}
}
