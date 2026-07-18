using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	public class ObjectManager
	{
		private ObjectRecord _objectRecordChain;

		private ObjectRecord _lastObjectRecord;

		private ArrayList _deserializedRecords = new ArrayList();

		private ArrayList _onDeserializedCallbackRecords = new ArrayList();

		private Hashtable _objectRecords = new Hashtable();

		private bool _finalFixup;

		private ISurrogateSelector _selector;

		private StreamingContext _context;

		private int _registeredObjectsCount;

		public ObjectManager(ISurrogateSelector selector, StreamingContext context)
		{
			_selector = selector;
			_context = context;
		}

		public virtual void DoFixups()
		{
			_finalFixup = true;
			try
			{
				if (_registeredObjectsCount < _objectRecords.Count)
				{
					throw new SerializationException("There are some fixups that refer to objects that have not been registered");
				}
				ObjectRecord lastObjectRecord = _lastObjectRecord;
				bool flag = true;
				ObjectRecord objectRecord = _objectRecordChain;
				while (objectRecord != null)
				{
					bool flag2 = !objectRecord.IsUnsolvedObjectReference || !flag;
					if (flag2)
					{
						flag2 = objectRecord.DoFixups(true, this, true);
					}
					if (flag2)
					{
						flag2 = objectRecord.LoadData(this, _selector, _context);
					}
					ObjectRecord objectRecord2;
					if (flag2)
					{
						if (objectRecord.OriginalObject is IDeserializationCallback)
						{
							_deserializedRecords.Add(objectRecord);
						}
						SerializationCallbacks serializationCallbacks = SerializationCallbacks.GetSerializationCallbacks(objectRecord.OriginalObject.GetType());
						if (serializationCallbacks.HasDeserializedCallbacks)
						{
							_onDeserializedCallbackRecords.Add(objectRecord);
						}
						objectRecord2 = objectRecord.Next;
					}
					else
					{
						if (objectRecord.ObjectInstance is IObjectReference && !flag)
						{
							if (objectRecord.Status == ObjectRecordStatus.ReferenceSolvingDelayed)
							{
								throw new SerializationException("The object with ID " + objectRecord.ObjectID + " could not be resolved");
							}
							objectRecord.Status = ObjectRecordStatus.ReferenceSolvingDelayed;
						}
						if (objectRecord != _lastObjectRecord)
						{
							objectRecord2 = objectRecord.Next;
							objectRecord.Next = null;
							_lastObjectRecord.Next = objectRecord;
							_lastObjectRecord = objectRecord;
						}
						else
						{
							objectRecord2 = objectRecord;
						}
					}
					if (objectRecord == lastObjectRecord)
					{
						flag = false;
					}
					objectRecord = objectRecord2;
				}
			}
			finally
			{
				_finalFixup = false;
			}
		}

		internal ObjectRecord GetObjectRecord(long objectID)
		{
			ObjectRecord objectRecord = (ObjectRecord)_objectRecords[objectID];
			if (objectRecord == null)
			{
				if (_finalFixup)
				{
					throw new SerializationException("The object with Id " + objectID + " has not been registered");
				}
				objectRecord = new ObjectRecord();
				objectRecord.ObjectID = objectID;
				_objectRecords[objectID] = objectRecord;
			}
			if (!objectRecord.IsRegistered && _finalFixup)
			{
				throw new SerializationException("The object with Id " + objectID + " has not been registered");
			}
			return objectRecord;
		}

		public virtual object GetObject(long objectID)
		{
			if (objectID <= 0)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			ObjectRecord objectRecord = (ObjectRecord)_objectRecords[objectID];
			if (objectRecord == null || !objectRecord.IsRegistered)
			{
				return null;
			}
			return objectRecord.ObjectInstance;
		}

		public virtual void RaiseDeserializationEvent()
		{
			for (int num = _onDeserializedCallbackRecords.Count - 1; num >= 0; num--)
			{
				ObjectRecord objectRecord = (ObjectRecord)_onDeserializedCallbackRecords[num];
				RaiseOnDeserializedEvent(objectRecord.OriginalObject);
			}
			for (int num2 = _deserializedRecords.Count - 1; num2 >= 0; num2--)
			{
				ObjectRecord objectRecord2 = (ObjectRecord)_deserializedRecords[num2];
				IDeserializationCallback deserializationCallback = objectRecord2.OriginalObject as IDeserializationCallback;
				if (deserializationCallback != null)
				{
					deserializationCallback.OnDeserialization(this);
				}
			}
		}

		public void RaiseOnDeserializingEvent(object obj)
		{
			SerializationCallbacks serializationCallbacks = SerializationCallbacks.GetSerializationCallbacks(obj.GetType());
			serializationCallbacks.RaiseOnDeserializing(obj, _context);
		}

		private void RaiseOnDeserializedEvent(object obj)
		{
			SerializationCallbacks serializationCallbacks = SerializationCallbacks.GetSerializationCallbacks(obj.GetType());
			serializationCallbacks.RaiseOnDeserialized(obj, _context);
		}

		private void AddFixup(BaseFixupRecord record)
		{
			record.ObjectToBeFixed.ChainFixup(record, true);
			record.ObjectRequired.ChainFixup(record, false);
		}

		public virtual void RecordArrayElementFixup(long arrayToBeFixed, int index, long objectRequired)
		{
			if (arrayToBeFixed <= 0)
			{
				throw new ArgumentOutOfRangeException("arrayToBeFixed", "The arrayToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			ArrayFixupRecord record = new ArrayFixupRecord(GetObjectRecord(arrayToBeFixed), index, GetObjectRecord(objectRequired));
			AddFixup(record);
		}

		public virtual void RecordArrayElementFixup(long arrayToBeFixed, int[] indices, long objectRequired)
		{
			if (arrayToBeFixed <= 0)
			{
				throw new ArgumentOutOfRangeException("arrayToBeFixed", "The arrayToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			if (indices == null)
			{
				throw new ArgumentNullException("indices");
			}
			MultiArrayFixupRecord record = new MultiArrayFixupRecord(GetObjectRecord(arrayToBeFixed), indices, GetObjectRecord(objectRequired));
			AddFixup(record);
		}

		public virtual void RecordDelayedFixup(long objectToBeFixed, string memberName, long objectRequired)
		{
			if (objectToBeFixed <= 0)
			{
				throw new ArgumentOutOfRangeException("objectToBeFixed", "The objectToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			if (memberName == null)
			{
				throw new ArgumentNullException("memberName");
			}
			DelayedFixupRecord record = new DelayedFixupRecord(GetObjectRecord(objectToBeFixed), memberName, GetObjectRecord(objectRequired));
			AddFixup(record);
		}

		public virtual void RecordFixup(long objectToBeFixed, MemberInfo member, long objectRequired)
		{
			if (objectToBeFixed <= 0)
			{
				throw new ArgumentOutOfRangeException("objectToBeFixed", "The objectToBeFixed parameter is less than or equal to zero");
			}
			if (objectRequired <= 0)
			{
				throw new ArgumentOutOfRangeException("objectRequired", "The objectRequired parameter is less than or equal to zero");
			}
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			FixupRecord record = new FixupRecord(GetObjectRecord(objectToBeFixed), member, GetObjectRecord(objectRequired));
			AddFixup(record);
		}

		private void RegisterObjectInternal(object obj, ObjectRecord record)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (record.IsRegistered)
			{
				if (record.OriginalObject != obj)
				{
					throw new SerializationException("An object with Id " + record.ObjectID + " has already been registered");
				}
				return;
			}
			record.ObjectInstance = obj;
			record.OriginalObject = obj;
			if (obj is IObjectReference)
			{
				record.Status = ObjectRecordStatus.ReferenceUnsolved;
			}
			else
			{
				record.Status = ObjectRecordStatus.ReferenceSolved;
			}
			if (_selector != null)
			{
				record.Surrogate = _selector.GetSurrogate(obj.GetType(), _context, out record.SurrogateSelector);
				if (record.Surrogate != null)
				{
					record.Status = ObjectRecordStatus.ReferenceUnsolved;
				}
			}
			record.DoFixups(true, this, false);
			record.DoFixups(false, this, false);
			_registeredObjectsCount++;
			if (_objectRecordChain == null)
			{
				_objectRecordChain = record;
				_lastObjectRecord = record;
			}
			else
			{
				_lastObjectRecord.Next = record;
				_lastObjectRecord = record;
			}
		}

		public virtual void RegisterObject(object obj, long objectID)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "The obj parameter is null.");
			}
			if (objectID <= 0)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			RegisterObjectInternal(obj, GetObjectRecord(objectID));
		}

		public void RegisterObject(object obj, long objectID, SerializationInfo info)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "The obj parameter is null.");
			}
			if (objectID <= 0)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			ObjectRecord objectRecord = GetObjectRecord(objectID);
			objectRecord.Info = info;
			RegisterObjectInternal(obj, objectRecord);
		}

		public void RegisterObject(object obj, long objectID, SerializationInfo info, long idOfContainingObj, MemberInfo member)
		{
			RegisterObject(obj, objectID, info, idOfContainingObj, member, null);
		}

		public void RegisterObject(object obj, long objectID, SerializationInfo info, long idOfContainingObj, MemberInfo member, int[] arrayIndex)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", "The obj parameter is null.");
			}
			if (objectID <= 0)
			{
				throw new ArgumentOutOfRangeException("objectID", "The objectID parameter is less than or equal to zero");
			}
			ObjectRecord objectRecord = GetObjectRecord(objectID);
			objectRecord.Info = info;
			objectRecord.IdOfContainingObj = idOfContainingObj;
			objectRecord.Member = member;
			objectRecord.ArrayIndex = arrayIndex;
			RegisterObjectInternal(obj, objectRecord);
		}
	}
}
