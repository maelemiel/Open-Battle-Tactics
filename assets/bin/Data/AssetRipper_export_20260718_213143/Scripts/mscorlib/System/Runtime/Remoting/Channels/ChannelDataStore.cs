using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[Serializable]
	[ComVisible(true)]
	public class ChannelDataStore : IChannelDataStore
	{
		private string[] _channelURIs;

		private DictionaryEntry[] _extraData;

		public string[] ChannelUris
		{
			get
			{
				return _channelURIs;
			}
			set
			{
				_channelURIs = value;
			}
		}

		public object this[object key]
		{
			get
			{
				if (_extraData == null)
				{
					return null;
				}
				DictionaryEntry[] extraData = _extraData;
				for (int i = 0; i < extraData.Length; i++)
				{
					DictionaryEntry dictionaryEntry = extraData[i];
					if (dictionaryEntry.Key.Equals(key))
					{
						return dictionaryEntry.Value;
					}
				}
				return null;
			}
			set
			{
				if (_extraData == null)
				{
					_extraData = new DictionaryEntry[1]
					{
						new DictionaryEntry(key, value)
					};
					return;
				}
				DictionaryEntry[] array = new DictionaryEntry[_extraData.Length + 1];
				_extraData.CopyTo(array, 0);
				array[_extraData.Length] = new DictionaryEntry(key, value);
				_extraData = array;
			}
		}

		public ChannelDataStore(string[] channelURIs)
		{
			_channelURIs = channelURIs;
		}
	}
}
