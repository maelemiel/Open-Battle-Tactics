using System;
using System.IO;
using System.Xml;
using Mono.Xml2;

namespace Mono.Xml
{
	internal class DTDEntityBase : DTDNode
	{
		private string name;

		private string publicId;

		private string systemId;

		private string literalValue;

		private string replacementText;

		private string uriString;

		private Uri absUri;

		private bool isInvalid;

		private bool loadFailed;

		private XmlResolver resolver;

		internal bool IsInvalid
		{
			get
			{
				return isInvalid;
			}
			set
			{
				isInvalid = value;
			}
		}

		public bool LoadFailed
		{
			get
			{
				return loadFailed;
			}
			set
			{
				loadFailed = value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string PublicId
		{
			get
			{
				return publicId;
			}
			set
			{
				publicId = value;
			}
		}

		public string SystemId
		{
			get
			{
				return systemId;
			}
			set
			{
				systemId = value;
			}
		}

		public string LiteralEntityValue
		{
			get
			{
				return literalValue;
			}
			set
			{
				literalValue = value;
			}
		}

		public string ReplacementText
		{
			get
			{
				return replacementText;
			}
			set
			{
				replacementText = value;
			}
		}

		public XmlResolver XmlResolver
		{
			set
			{
				resolver = value;
			}
		}

		public string ActualUri
		{
			get
			{
				if (uriString == null)
				{
					if (resolver == null || SystemId == null || SystemId.Length == 0)
					{
						uriString = BaseURI;
					}
					else
					{
						Uri baseUri = null;
						try
						{
							if (BaseURI != null && BaseURI.Length > 0)
							{
								baseUri = new Uri(BaseURI);
							}
						}
						catch (UriFormatException)
						{
						}
						absUri = resolver.ResolveUri(baseUri, SystemId);
						uriString = ((!(absUri != null)) ? string.Empty : absUri.ToString());
					}
				}
				return uriString;
			}
		}

		protected DTDEntityBase(DTDObjectModel root)
		{
			SetRoot(root);
		}

		public void Resolve()
		{
			if (ActualUri == string.Empty)
			{
				LoadFailed = true;
				LiteralEntityValue = string.Empty;
				return;
			}
			if (base.Root.ExternalResources.ContainsKey(ActualUri))
			{
				LiteralEntityValue = (string)base.Root.ExternalResources[ActualUri];
			}
			Stream stream = null;
			try
			{
				stream = resolver.GetEntity(absUri, null, typeof(Stream)) as Stream;
				Mono.Xml2.XmlTextReader xmlTextReader = new Mono.Xml2.XmlTextReader(ActualUri, stream, base.Root.NameTable);
				LiteralEntityValue = xmlTextReader.GetRemainder().ReadToEnd();
				base.Root.ExternalResources.Add(ActualUri, LiteralEntityValue);
				if (base.Root.ExternalResources.Count > 256)
				{
					throw new InvalidOperationException("The total amount of external entities exceeded the allowed number.");
				}
			}
			catch (Exception)
			{
				LiteralEntityValue = string.Empty;
				LoadFailed = true;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}
	}
}
