using System.Collections.Generic;

namespace MobageEditor
{
	public class EventReporterSession : ISessionAnalytics
	{
		private bool closing;

		private List<IJSONDictionaryHandler> handlers;

		private IDictionaryChannel channel;

		private EventSessionMetaDataHandler sessionMetaDataHandler = new EventSessionMetaDataHandler();

		public string SessionId
		{
			get
			{
				return sessionMetaDataHandler.SID;
			}
		}

		public EventReporterSession(IDictionaryChannel channel, List<IJSONDictionaryHandler> handlers)
			: this(channel, handlers, true)
		{
		}

		public EventReporterSession(IDictionaryChannel channel, List<IJSONDictionaryHandler> handlers, bool sendSessionStart)
		{
			closing = false;
			this.channel = channel;
			this.handlers = new List<IJSONDictionaryHandler>(handlers);
			this.handlers.Add(sessionMetaDataHandler);
			if (sendSessionStart)
			{
				Report(new SessionStartEvent());
			}
		}

		public void ReportEventString(string eventString)
		{
			JsonData dic = JsonMapper.ToObject(eventString);
			ReportDictionary(dic);
		}

		public void Report(IAnalyticsEvent analyticsEvent)
		{
			ReportDictionary(analyticsEvent.Envelope);
		}

		public void ReportDictionary(JsonData dic)
		{
			foreach (IJSONDictionaryHandler handler in handlers)
			{
				handler.Process(dic);
			}
			channel.Send(dic);
		}
	}
}
