using System.Collections.Generic;

namespace MobageEditor
{
	public class EventReporterSessionFactory
	{
		private AnalyticsServer targetServer;

		private List<IJSONDictionaryHandler> handlers;

		public EventReporterSessionFactory(AnalyticsServer targetServer, string regionId, string appId, string appVersion, string ndkVersion)
		{
			Characteristics defaultCharacteristics = Characteristics.DefaultCharacteristics;
			this.targetServer = targetServer;
			handlers = new List<IJSONDictionaryHandler>
			{
				new EventAppMetaDataHandler(regionId, appId, appVersion, ndkVersion),
				new EventDeviceMetaDataHandler(defaultCharacteristics.Carrier)
			};
		}

		public EventReporterSession NewSession()
		{
			string hostname = ((targetServer != AnalyticsServer.Production) ? "ngpipes-sandbox.mobage.com" : "ngpipes.ngmoco.com");
			List<IJSONDictionaryHandler> list = new List<IJSONDictionaryHandler>(handlers);
			list.Add(new EventTimeHandler());
			list.Add(new EventSequenceNumberHandler());
			list.Add(new EventPropertyNormalizerHandler());
			return new EventReporterSession(new NGPipesSender(hostname), list);
		}
	}
}
