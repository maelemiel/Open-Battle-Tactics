using System.Reflection;

namespace System.Runtime.Serialization
{
	internal class ObjectRecord
	{
		public ObjectRecordStatus Status;

		public object OriginalObject;

		public object ObjectInstance;

		public long ObjectID;

		public SerializationInfo Info;

		public long IdOfContainingObj;

		public ISerializationSurrogate Surrogate;

		public ISurrogateSelector SurrogateSelector;

		public MemberInfo Member;

		public int[] ArrayIndex;

		public BaseFixupRecord FixupChainAsContainer;

		public BaseFixupRecord FixupChainAsRequired;

		public ObjectRecord Next;

		public bool IsInstanceReady
		{
			get
			{
				if (!IsRegistered)
				{
					return false;
				}
				if (IsUnsolvedObjectReference)
				{
					return false;
				}
				if (ObjectInstance.GetType().IsValueType && (HasPendingFixups || Info != null))
				{
					return false;
				}
				return true;
			}
		}

		public bool IsUnsolvedObjectReference
		{
			get
			{
				return Status != ObjectRecordStatus.ReferenceSolved;
			}
		}

		public bool IsRegistered
		{
			get
			{
				return Status != ObjectRecordStatus.Unregistered;
			}
		}

		public bool HasPendingFixups
		{
			get
			{
				return FixupChainAsContainer != null;
			}
		}

		public void SetMemberValue(ObjectManager manager, MemberInfo member, object value)
		{
			if (member is FieldInfo)
			{
				((FieldInfo)member).SetValue(ObjectInstance, value);
			}
			else
			{
				if (!(member is PropertyInfo))
				{
					throw new SerializationException("Cannot perform fixup");
				}
				((PropertyInfo)member).SetValue(ObjectInstance, value, null);
			}
			if (Member != null)
			{
				ObjectRecord objectRecord = manager.GetObjectRecord(IdOfContainingObj);
				if (objectRecord.IsRegistered)
				{
					objectRecord.SetMemberValue(manager, Member, ObjectInstance);
				}
			}
			else if (ArrayIndex != null)
			{
				ObjectRecord objectRecord2 = manager.GetObjectRecord(IdOfContainingObj);
				if (objectRecord2.IsRegistered)
				{
					objectRecord2.SetArrayValue(manager, ObjectInstance, ArrayIndex);
				}
			}
		}

		public void SetArrayValue(ObjectManager manager, object value, int[] indices)
		{
			((Array)ObjectInstance).SetValue(value, indices);
		}

		public void SetMemberValue(ObjectManager manager, string memberName, object value)
		{
			if (Info == null)
			{
				throw new SerializationException("Cannot perform fixup");
			}
			Info.AddValue(memberName, value, value.GetType());
		}

		public bool DoFixups(bool asContainer, ObjectManager manager, bool strict)
		{
			BaseFixupRecord prevFixup = null;
			BaseFixupRecord baseFixupRecord = ((!asContainer) ? FixupChainAsRequired : FixupChainAsContainer);
			bool result = true;
			while (baseFixupRecord != null)
			{
				if (baseFixupRecord.DoFixup(manager, strict))
				{
					UnchainFixup(baseFixupRecord, prevFixup, asContainer);
					if (asContainer)
					{
						baseFixupRecord.ObjectRequired.RemoveFixup(baseFixupRecord, false);
					}
					else
					{
						baseFixupRecord.ObjectToBeFixed.RemoveFixup(baseFixupRecord, true);
					}
				}
				else
				{
					prevFixup = baseFixupRecord;
					result = false;
				}
				baseFixupRecord = ((!asContainer) ? baseFixupRecord.NextSameRequired : baseFixupRecord.NextSameContainer);
			}
			return result;
		}

		public void RemoveFixup(BaseFixupRecord fixupToRemove, bool asContainer)
		{
			BaseFixupRecord prevFixup = null;
			for (BaseFixupRecord baseFixupRecord = ((!asContainer) ? FixupChainAsRequired : FixupChainAsContainer); baseFixupRecord != null; baseFixupRecord = ((!asContainer) ? baseFixupRecord.NextSameRequired : baseFixupRecord.NextSameContainer))
			{
				if (baseFixupRecord == fixupToRemove)
				{
					UnchainFixup(baseFixupRecord, prevFixup, asContainer);
					break;
				}
				prevFixup = baseFixupRecord;
			}
		}

		private void UnchainFixup(BaseFixupRecord fixup, BaseFixupRecord prevFixup, bool asContainer)
		{
			if (prevFixup == null)
			{
				if (asContainer)
				{
					FixupChainAsContainer = fixup.NextSameContainer;
				}
				else
				{
					FixupChainAsRequired = fixup.NextSameRequired;
				}
			}
			else if (asContainer)
			{
				prevFixup.NextSameContainer = fixup.NextSameContainer;
			}
			else
			{
				prevFixup.NextSameRequired = fixup.NextSameRequired;
			}
		}

		public void ChainFixup(BaseFixupRecord fixup, bool asContainer)
		{
			if (asContainer)
			{
				fixup.NextSameContainer = FixupChainAsContainer;
				FixupChainAsContainer = fixup;
			}
			else
			{
				fixup.NextSameRequired = FixupChainAsRequired;
				FixupChainAsRequired = fixup;
			}
		}

		public bool LoadData(ObjectManager manager, ISurrogateSelector selector, StreamingContext context)
		{
			if (Info != null)
			{
				if (Surrogate != null)
				{
					object obj = Surrogate.SetObjectData(ObjectInstance, Info, context, SurrogateSelector);
					if (obj != null)
					{
						ObjectInstance = obj;
					}
					Status = ObjectRecordStatus.ReferenceSolved;
				}
				else
				{
					if (!(ObjectInstance is ISerializable))
					{
						throw new SerializationException("No surrogate selector was found for type " + ObjectInstance.GetType().FullName);
					}
					object[] parameters = new object[2] { Info, context };
					ConstructorInfo constructor = ObjectInstance.GetType().GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
					{
						typeof(SerializationInfo),
						typeof(StreamingContext)
					}, null);
					if (constructor == null)
					{
						throw new SerializationException("The constructor to deserialize an object of type " + ObjectInstance.GetType().FullName + " was not found.");
					}
					constructor.Invoke(ObjectInstance, parameters);
				}
				Info = null;
			}
			if (ObjectInstance is IObjectReference && Status != ObjectRecordStatus.ReferenceSolved)
			{
				try
				{
					ObjectInstance = ((IObjectReference)ObjectInstance).GetRealObject(context);
					int num = 100;
					while (ObjectInstance is IObjectReference && num > 0)
					{
						object realObject = ((IObjectReference)ObjectInstance).GetRealObject(context);
						if (realObject == ObjectInstance)
						{
							break;
						}
						ObjectInstance = realObject;
						num--;
					}
					if (num == 0)
					{
						throw new SerializationException("The implementation of the IObjectReference interface returns too many nested references to other objects that implement IObjectReference.");
					}
					Status = ObjectRecordStatus.ReferenceSolved;
				}
				catch (NullReferenceException)
				{
					return false;
				}
			}
			if (Member != null)
			{
				ObjectRecord objectRecord = manager.GetObjectRecord(IdOfContainingObj);
				objectRecord.SetMemberValue(manager, Member, ObjectInstance);
			}
			else if (ArrayIndex != null)
			{
				ObjectRecord objectRecord2 = manager.GetObjectRecord(IdOfContainingObj);
				objectRecord2.SetArrayValue(manager, ObjectInstance, ArrayIndex);
			}
			return true;
		}
	}
}
