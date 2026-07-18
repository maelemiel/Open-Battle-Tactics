using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Mono.Security.Protocol.Tls;
using Mono.Security.X509;
using Mono.Security.X509.Extensions;

namespace System.Net
{
	public class ServicePointManager
	{
		private class SPKey
		{
			private Uri uri;

			private bool use_connect;

			public Uri Uri
			{
				get
				{
					return uri;
				}
			}

			public bool UseConnect
			{
				get
				{
					return use_connect;
				}
			}

			public SPKey(Uri uri, bool use_connect)
			{
				this.uri = uri;
				this.use_connect = use_connect;
			}

			public override int GetHashCode()
			{
				return uri.GetHashCode() + (use_connect ? 1 : 0);
			}

			public override bool Equals(object obj)
			{
				SPKey sPKey = obj as SPKey;
				if (obj == null)
				{
					return false;
				}
				return uri.Equals(sPKey.uri) && sPKey.use_connect == use_connect;
			}
		}

		internal class ChainValidationHelper
		{
			private object sender;

			private string host;

			private static bool is_macosx = File.Exists("/System/Library/Frameworks/Security.framework/Security");

			private static X509KeyUsageFlags s_flags = X509KeyUsageFlags.KeyAgreement | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature;

			public string Host
			{
				get
				{
					if (host == null && sender is HttpWebRequest)
					{
						host = ((HttpWebRequest)sender).Address.Host;
					}
					return host;
				}
				set
				{
					host = value;
				}
			}

			public ChainValidationHelper(object sender)
			{
				this.sender = sender;
			}

			internal ValidationResult ValidateChain(Mono.Security.X509.X509CertificateCollection certs)
			{
				bool user_denied = false;
				if (certs == null || certs.Count == 0)
				{
					return null;
				}
				ICertificatePolicy certificatePolicy = CertificatePolicy;
				RemoteCertificateValidationCallback serverCertificateValidationCallback = ServerCertificateValidationCallback;
				System.Security.Cryptography.X509Certificates.X509Chain x509Chain = new System.Security.Cryptography.X509Certificates.X509Chain();
				x509Chain.ChainPolicy = new X509ChainPolicy();
				for (int i = 1; i < certs.Count; i++)
				{
					X509Certificate2 certificate = new X509Certificate2(certs[i].RawData);
					x509Chain.ChainPolicy.ExtraStore.Add(certificate);
				}
				X509Certificate2 x509Certificate = new X509Certificate2(certs[0].RawData);
				int num = 0;
				SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
				try
				{
					if (!x509Chain.Build(x509Certificate))
					{
						sslPolicyErrors |= GetErrorsFromChain(x509Chain);
					}
				}
				catch (Exception arg)
				{
					Console.Error.WriteLine("ERROR building certificate chain: {0}", arg);
					Console.Error.WriteLine("Please, report this problem to the Mono team");
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
				}
				if (!CheckCertificateUsage(x509Certificate))
				{
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
					num = -2146762490;
				}
				if (!CheckServerIdentity(certs[0], Host))
				{
					sslPolicyErrors |= SslPolicyErrors.RemoteCertificateNameMismatch;
					num = -2146762481;
				}
				bool flag = false;
				try
				{
					Mono.Security.X509.OSX509Certificates.SecTrustResult secTrustResult = Mono.Security.X509.OSX509Certificates.TrustEvaluateSsl(certs);
					flag = secTrustResult == Mono.Security.X509.OSX509Certificates.SecTrustResult.Proceed || secTrustResult == Mono.Security.X509.OSX509Certificates.SecTrustResult.Unspecified;
				}
				catch
				{
				}
				if (flag)
				{
					num = 0;
					sslPolicyErrors = SslPolicyErrors.None;
				}
				if (certificatePolicy != null && (!(certificatePolicy is System.Net.DefaultCertificatePolicy) || serverCertificateValidationCallback == null))
				{
					ServicePoint srvPoint = null;
					HttpWebRequest httpWebRequest = sender as HttpWebRequest;
					if (httpWebRequest != null)
					{
						srvPoint = httpWebRequest.ServicePoint;
					}
					if (num == 0 && sslPolicyErrors != SslPolicyErrors.None)
					{
						num = GetStatusFromChain(x509Chain);
					}
					flag = certificatePolicy.CheckValidationResult(srvPoint, x509Certificate, httpWebRequest, num);
					user_denied = !flag && !(certificatePolicy is System.Net.DefaultCertificatePolicy);
				}
				if (serverCertificateValidationCallback != null)
				{
					flag = serverCertificateValidationCallback(sender, x509Certificate, x509Chain, sslPolicyErrors);
					user_denied = !flag;
				}
				return new ValidationResult(flag, user_denied, num);
			}

			private static int GetStatusFromChain(System.Security.Cryptography.X509Certificates.X509Chain chain)
			{
				long num = 0L;
				X509ChainStatus[] chainStatus = chain.ChainStatus;
				foreach (X509ChainStatus x509ChainStatus in chainStatus)
				{
					System.Security.Cryptography.X509Certificates.X509ChainStatusFlags status = x509ChainStatus.Status;
					if (status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						num = (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotTimeValid) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotTimeNested) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.Revoked) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotSignatureValid) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NotValidForUsage) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.RevocationStatusUnknown) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.Cyclic) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidExtension) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidPolicyConstraints) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidBasicConstraints) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.InvalidNameConstraints) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotSupportedNameConstraint) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotDefinedNameConstraint) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasNotPermittedNameConstraint) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.HasExcludedNameConstraint) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.PartialChain) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotTimeValid) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotSignatureValid) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.CtlNotValidForUsage) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.OfflineRevocation) == 0) ? (((status & System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoIssuanceChainPolicy) == 0) ? 2148204811u : 2148204807u) : 2148081682u) : 2148204816u) : 2148098052u) : 2148204801u) : 2148204810u) : 2148204820u) : 2148204820u) : 2148204820u) : 2148204820u) : 2148204820u) : 2148098073u) : 2148204813u) : 2148204811u) : 2148204810u) : 2148081682u) : 2148204809u) : 2148204816u) : 2148098052u) : 2148204812u) : 2148204802u) : 2148204801u);
						break;
					}
				}
				return (int)num;
			}

			private static SslPolicyErrors GetErrorsFromChain(System.Security.Cryptography.X509Certificates.X509Chain chain)
			{
				SslPolicyErrors sslPolicyErrors = SslPolicyErrors.None;
				X509ChainStatus[] chainStatus = chain.ChainStatus;
				foreach (X509ChainStatus x509ChainStatus in chainStatus)
				{
					if (x509ChainStatus.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
					{
						sslPolicyErrors |= SslPolicyErrors.RemoteCertificateChainErrors;
						break;
					}
				}
				return sslPolicyErrors;
			}

			private static bool CheckCertificateUsage(X509Certificate2 cert)
			{
				try
				{
					if (cert.Version < 3)
					{
						return true;
					}
					X509KeyUsageExtension x509KeyUsageExtension = (X509KeyUsageExtension)cert.Extensions["2.5.29.15"];
					X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = (X509EnhancedKeyUsageExtension)cert.Extensions["2.5.29.37"];
					if (x509KeyUsageExtension != null && x509EnhancedKeyUsageExtension != null)
					{
						if ((x509KeyUsageExtension.KeyUsages & s_flags) == 0)
						{
							return false;
						}
						return x509EnhancedKeyUsageExtension.EnhancedKeyUsages["1.3.6.1.5.5.7.3.1"] != null || x509EnhancedKeyUsageExtension.EnhancedKeyUsages["2.16.840.1.113730.4.1"] != null;
					}
					if (x509KeyUsageExtension != null)
					{
						return (x509KeyUsageExtension.KeyUsages & s_flags) != 0;
					}
					if (x509EnhancedKeyUsageExtension != null)
					{
						return x509EnhancedKeyUsageExtension.EnhancedKeyUsages["1.3.6.1.5.5.7.3.1"] != null || x509EnhancedKeyUsageExtension.EnhancedKeyUsages["2.16.840.1.113730.4.1"] != null;
					}
					System.Security.Cryptography.X509Certificates.X509Extension x509Extension = cert.Extensions["2.16.840.1.113730.1.1"];
					if (x509Extension != null)
					{
						string text = x509Extension.NetscapeCertType(false);
						return text.IndexOf("SSL Server Authentication") != -1;
					}
					return true;
				}
				catch (Exception arg)
				{
					Console.Error.WriteLine("ERROR processing certificate: {0}", arg);
					Console.Error.WriteLine("Please, report this problem to the Mono team");
					return false;
				}
			}

			private static bool CheckServerIdentity(Mono.Security.X509.X509Certificate cert, string targetHost)
			{
				try
				{
					Mono.Security.X509.X509Extension x509Extension = cert.Extensions["2.5.29.17"];
					if (x509Extension != null)
					{
						SubjectAltNameExtension subjectAltNameExtension = new SubjectAltNameExtension(x509Extension);
						string[] dNSNames = subjectAltNameExtension.DNSNames;
						foreach (string pattern in dNSNames)
						{
							if (Match(targetHost, pattern))
							{
								return true;
							}
						}
						string[] iPAddresses = subjectAltNameExtension.IPAddresses;
						foreach (string text in iPAddresses)
						{
							if (text == targetHost)
							{
								return true;
							}
						}
					}
					return CheckDomainName(cert.SubjectName, targetHost);
				}
				catch (Exception arg)
				{
					Console.Error.WriteLine("ERROR processing certificate: {0}", arg);
					Console.Error.WriteLine("Please, report this problem to the Mono team");
					return false;
				}
			}

			private static bool CheckDomainName(string subjectName, string targetHost)
			{
				string pattern = string.Empty;
				Regex regex = new Regex("CN\\s*=\\s*([^,]*)");
				MatchCollection matchCollection = regex.Matches(subjectName);
				if (matchCollection.Count == 1 && matchCollection[0].Success)
				{
					pattern = matchCollection[0].Groups[1].Value.ToString();
				}
				return Match(targetHost, pattern);
			}

			private static bool Match(string hostname, string pattern)
			{
				int num = pattern.IndexOf('*');
				if (num == -1)
				{
					return string.Compare(hostname, pattern, true, CultureInfo.InvariantCulture) == 0;
				}
				if (num != pattern.Length - 1 && pattern[num + 1] != '.')
				{
					return false;
				}
				int num2 = pattern.IndexOf('*', num + 1);
				if (num2 != -1)
				{
					return false;
				}
				string text = pattern.Substring(num + 1);
				int num3 = hostname.Length - text.Length;
				if (num3 <= 0)
				{
					return false;
				}
				if (string.Compare(hostname, num3, text, 0, text.Length, true, CultureInfo.InvariantCulture) != 0)
				{
					return false;
				}
				if (num == 0)
				{
					int num4 = hostname.IndexOf('.');
					return num4 == -1 || num4 >= hostname.Length - text.Length;
				}
				string text2 = pattern.Substring(0, num);
				return string.Compare(hostname, 0, text2, 0, text2.Length, true, CultureInfo.InvariantCulture) == 0;
			}
		}

		public const int DefaultNonPersistentConnectionLimit = 4;

		public const int DefaultPersistentConnectionLimit = 2;

		private static HybridDictionary servicePoints;

		private static ICertificatePolicy policy;

		private static int defaultConnectionLimit;

		private static int maxServicePointIdleTime;

		private static int maxServicePoints;

		private static bool _checkCRL;

		private static SecurityProtocolType _securityProtocol;

		private static bool expectContinue;

		private static bool useNagle;

		private static RemoteCertificateValidationCallback server_cert_cb;

		[Obsolete("Use ServerCertificateValidationCallback instead", false)]
		public static ICertificatePolicy CertificatePolicy
		{
			get
			{
				return policy;
			}
			set
			{
				policy = value;
			}
		}

		[System.MonoTODO("CRL checks not implemented")]
		public static bool CheckCertificateRevocationList
		{
			get
			{
				return _checkCRL;
			}
			set
			{
				_checkCRL = false;
			}
		}

		public static int DefaultConnectionLimit
		{
			get
			{
				return defaultConnectionLimit;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				defaultConnectionLimit = value;
			}
		}

		[System.MonoTODO]
		public static int DnsRefreshTimeout
		{
			get
			{
				throw GetMustImplement();
			}
			set
			{
				throw GetMustImplement();
			}
		}

		[System.MonoTODO]
		public static bool EnableDnsRoundRobin
		{
			get
			{
				throw GetMustImplement();
			}
			set
			{
				throw GetMustImplement();
			}
		}

		public static int MaxServicePointIdleTime
		{
			get
			{
				return maxServicePointIdleTime;
			}
			set
			{
				if (value < -2 || value > int.MaxValue)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				maxServicePointIdleTime = value;
			}
		}

		public static int MaxServicePoints
		{
			get
			{
				return maxServicePoints;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentException("value");
				}
				maxServicePoints = value;
				RecycleServicePoints();
			}
		}

		public static SecurityProtocolType SecurityProtocol
		{
			get
			{
				return _securityProtocol;
			}
			set
			{
				_securityProtocol = value;
			}
		}

		public static RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				return server_cert_cb;
			}
			set
			{
				server_cert_cb = value;
			}
		}

		public static bool Expect100Continue
		{
			get
			{
				return expectContinue;
			}
			set
			{
				expectContinue = value;
			}
		}

		public static bool UseNagleAlgorithm
		{
			get
			{
				return useNagle;
			}
			set
			{
				useNagle = value;
			}
		}

		private ServicePointManager()
		{
		}

		static ServicePointManager()
		{
			servicePoints = new HybridDictionary();
			policy = new System.Net.DefaultCertificatePolicy();
			defaultConnectionLimit = 2;
			maxServicePointIdleTime = 900000;
			maxServicePoints = 0;
			_checkCRL = false;
			_securityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
			expectContinue = true;
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		public static ServicePoint FindServicePoint(Uri address)
		{
			return FindServicePoint(address, GlobalProxySelection.Select);
		}

		public static ServicePoint FindServicePoint(string uriString, IWebProxy proxy)
		{
			return FindServicePoint(new Uri(uriString), proxy);
		}

		public static ServicePoint FindServicePoint(Uri address, IWebProxy proxy)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			RecycleServicePoints();
			bool usesProxy = false;
			bool flag = false;
			if (proxy != null && !proxy.IsBypassed(address))
			{
				usesProxy = true;
				bool flag2 = address.Scheme == "https";
				address = proxy.GetProxy(address);
				if (address.Scheme != "http" && !flag2)
				{
					throw new NotSupportedException("Proxy scheme not supported.");
				}
				if (flag2 && address.Scheme == "http")
				{
					flag = true;
				}
			}
			address = new Uri(address.Scheme + "://" + address.Authority);
			ServicePoint servicePoint = null;
			lock (servicePoints)
			{
				SPKey key = new SPKey(address, flag);
				servicePoint = servicePoints[key] as ServicePoint;
				if (servicePoint != null)
				{
					return servicePoint;
				}
				if (maxServicePoints > 0 && servicePoints.Count >= maxServicePoints)
				{
					throw new InvalidOperationException("maximum number of service points reached");
				}
				string text = address.ToString();
				int connectionLimit = defaultConnectionLimit;
				servicePoint = new ServicePoint(address, connectionLimit, maxServicePointIdleTime);
				servicePoint.Expect100Continue = expectContinue;
				servicePoint.UseNagleAlgorithm = useNagle;
				servicePoint.UsesProxy = usesProxy;
				servicePoint.UseConnect = flag;
				servicePoints.Add(key, servicePoint);
				return servicePoint;
			}
		}

		internal static void RecycleServicePoints()
		{
			ArrayList arrayList = new ArrayList();
			lock (servicePoints)
			{
				IDictionaryEnumerator enumerator = servicePoints.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ServicePoint servicePoint = (ServicePoint)enumerator.Value;
					if (servicePoint.AvailableForRecycling)
					{
						arrayList.Add(enumerator.Key);
					}
				}
				for (int i = 0; i < arrayList.Count; i++)
				{
					servicePoints.Remove(arrayList[i]);
				}
				if (maxServicePoints == 0 || servicePoints.Count <= maxServicePoints)
				{
					return;
				}
				SortedList sortedList = new SortedList(servicePoints.Count);
				enumerator = servicePoints.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ServicePoint servicePoint2 = (ServicePoint)enumerator.Value;
					if (servicePoint2.CurrentConnections == 0)
					{
						while (sortedList.ContainsKey(servicePoint2.IdleSince))
						{
							servicePoint2.IdleSince = servicePoint2.IdleSince.AddMilliseconds(1.0);
						}
						sortedList.Add(servicePoint2.IdleSince, servicePoint2.Address);
					}
				}
				for (int j = 0; j < sortedList.Count; j++)
				{
					if (servicePoints.Count <= maxServicePoints)
					{
						break;
					}
					servicePoints.Remove(sortedList.GetByIndex(j));
				}
			}
		}
	}
}
