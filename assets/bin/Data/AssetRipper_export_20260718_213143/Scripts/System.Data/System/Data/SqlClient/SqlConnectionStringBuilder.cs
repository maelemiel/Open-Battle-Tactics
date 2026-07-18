using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;

namespace System.Data.SqlClient
{
	[DefaultProperty("DataSource")]
	[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlConnectionStringBuilderConverter, System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class SqlConnectionStringBuilder : DbConnectionStringBuilder
	{
		private const string DEF_APPLICATIONNAME = ".NET SqlClient Data Provider";

		private const bool DEF_ASYNCHRONOUSPROCESSING = false;

		private const string DEF_ATTACHDBFILENAME = "";

		private const bool DEF_CONNECTIONRESET = true;

		private const int DEF_CONNECTTIMEOUT = 15;

		private const string DEF_CURRENTLANGUAGE = "";

		private const string DEF_DATASOURCE = "";

		private const bool DEF_ENCRYPT = false;

		private const bool DEF_ENLIST = false;

		private const string DEF_FAILOVERPARTNER = "";

		private const string DEF_INITIALCATALOG = "";

		private const bool DEF_INTEGRATEDSECURITY = false;

		private const int DEF_LOADBALANCETIMEOUT = 0;

		private const int DEF_MAXPOOLSIZE = 100;

		private const int DEF_MINPOOLSIZE = 0;

		private const bool DEF_MULTIPLEACTIVERESULTSETS = false;

		private const string DEF_NETWORKLIBRARY = "";

		private const int DEF_PACKETSIZE = 8000;

		private const string DEF_PASSWORD = "";

		private const bool DEF_PERSISTSECURITYINFO = false;

		private const bool DEF_POOLING = true;

		private const bool DEF_REPLICATION = false;

		private const string DEF_USERID = "";

		private const string DEF_WORKSTATIONID = "";

		private const string DEF_TYPESYSTEMVERSION = "Latest";

		private const bool DEF_TRUSTSERVERCERTIFICATE = false;

		private const bool DEF_USERINSTANCE = false;

		private const bool DEF_CONTEXTCONNECTION = false;

		private const string DEF_TRANSACTIONBINDING = "Implicit Unbind";

		private string _applicationName;

		private bool _asynchronousProcessing;

		private string _attachDBFilename;

		private bool _connectionReset;

		private int _connectTimeout;

		private string _currentLanguage;

		private string _dataSource;

		private bool _encrypt;

		private bool _enlist;

		private string _failoverPartner;

		private string _initialCatalog;

		private bool _integratedSecurity;

		private int _loadBalanceTimeout;

		private int _maxPoolSize;

		private int _minPoolSize;

		private bool _multipleActiveResultSets;

		private string _networkLibrary;

		private int _packetSize;

		private string _password;

		private bool _persistSecurityInfo;

		private bool _pooling;

		private bool _replication;

		private string _userID;

		private string _workstationID;

		private bool _trustServerCertificate;

		private string _typeSystemVersion;

		private bool _userInstance;

		private bool _contextConnection;

		private string _transactionBinding;

		private static Dictionary<string, string> _keywords;

		private static Dictionary<string, object> _defaults;

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Application Name")]
		public string ApplicationName
		{
			get
			{
				return _applicationName;
			}
			set
			{
				base["Application Name"] = value;
				_applicationName = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Asynchronous Processing")]
		public bool AsynchronousProcessing
		{
			get
			{
				return _asynchronousProcessing;
			}
			set
			{
				base["Asynchronous Processing"] = value;
				_asynchronousProcessing = value;
			}
		}

		[DisplayName("AttachDbFilename")]
		[RefreshProperties(RefreshProperties.All)]
		[Editor("System.Windows.Forms.Design.FileNameEditor, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public string AttachDBFilename
		{
			get
			{
				return _attachDBFilename;
			}
			set
			{
				base["AttachDbFilename"] = value;
				_attachDBFilename = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Connection Reset")]
		public bool ConnectionReset
		{
			get
			{
				return _connectionReset;
			}
			set
			{
				base["Connection Reset"] = value;
				_connectionReset = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Connect Timeout")]
		public int ConnectTimeout
		{
			get
			{
				return _connectTimeout;
			}
			set
			{
				base["Connect Timeout"] = value;
				_connectTimeout = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Current Language")]
		public string CurrentLanguage
		{
			get
			{
				return _currentLanguage;
			}
			set
			{
				base["Current Language"] = value;
				_currentLanguage = value;
			}
		}

		[DisplayName("Data Source")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlDataSourceConverter, System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public string DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				base["Data Source"] = value;
				_dataSource = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Encrypt")]
		public bool Encrypt
		{
			get
			{
				return _encrypt;
			}
			set
			{
				base["Encrypt"] = value;
				_encrypt = value;
			}
		}

		[DisplayName("Enlist")]
		[RefreshProperties(RefreshProperties.All)]
		public bool Enlist
		{
			get
			{
				return _enlist;
			}
			set
			{
				base["Enlist"] = value;
				_enlist = value;
			}
		}

		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlDataSourceConverter, System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		[DisplayName("Failover Partner")]
		[RefreshProperties(RefreshProperties.All)]
		public string FailoverPartner
		{
			get
			{
				return _failoverPartner;
			}
			set
			{
				base["Failover Partner"] = value;
				_failoverPartner = value;
			}
		}

		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+SqlInitialCatalogConverter, System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		[DisplayName("Initial Catalog")]
		[RefreshProperties(RefreshProperties.All)]
		public string InitialCatalog
		{
			get
			{
				return _initialCatalog;
			}
			set
			{
				base["Initial Catalog"] = value;
				_initialCatalog = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Integrated Security")]
		public bool IntegratedSecurity
		{
			get
			{
				return _integratedSecurity;
			}
			set
			{
				base["Integrated Security"] = value;
				_integratedSecurity = value;
			}
		}

		public override bool IsFixedSize
		{
			get
			{
				return true;
			}
		}

		public override object this[string keyword]
		{
			get
			{
				string text = MapKeyword(keyword);
				if (base.ContainsKey(text))
				{
					return base[text];
				}
				return _defaults[text];
			}
			set
			{
				SetValue(keyword, value);
			}
		}

		public override ICollection Keys
		{
			get
			{
				List<string> list = new List<string>();
				list.Add("Data Source");
				list.Add("Failover Partner");
				list.Add("AttachDbFilename");
				list.Add("Initial Catalog");
				list.Add("Integrated Security");
				list.Add("Persist Security Info");
				list.Add("User ID");
				list.Add("Password");
				list.Add("Enlist");
				list.Add("Pooling");
				list.Add("Min Pool Size");
				list.Add("Max Pool Size");
				list.Add("Asynchronous Processing");
				list.Add("Connection Reset");
				list.Add("MultipleActiveResultSets");
				list.Add("Replication");
				list.Add("Connect Timeout");
				list.Add("Encrypt");
				list.Add("TrustServerCertificate");
				list.Add("Load Balance Timeout");
				list.Add("Network Library");
				list.Add("Packet Size");
				list.Add("Type System Version");
				list.Add("Application Name");
				list.Add("Current Language");
				list.Add("Workstation ID");
				list.Add("User Instance");
				list.Add("Context Connection");
				list.Add("Transaction Binding");
				return new ReadOnlyCollection<string>(list);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Load Balance Timeout")]
		public int LoadBalanceTimeout
		{
			get
			{
				return _loadBalanceTimeout;
			}
			set
			{
				base["Load Balance Timeout"] = value;
				_loadBalanceTimeout = value;
			}
		}

		[DisplayName("Max Pool Size")]
		[RefreshProperties(RefreshProperties.All)]
		public int MaxPoolSize
		{
			get
			{
				return _maxPoolSize;
			}
			set
			{
				base["Max Pool Size"] = value;
				_maxPoolSize = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Min Pool Size")]
		public int MinPoolSize
		{
			get
			{
				return _minPoolSize;
			}
			set
			{
				base["Min Pool Size"] = value;
				_minPoolSize = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("MultipleActiveResultSets")]
		public bool MultipleActiveResultSets
		{
			get
			{
				return _multipleActiveResultSets;
			}
			set
			{
				base["Multiple Active Resultsets"] = value;
				_multipleActiveResultSets = value;
			}
		}

		[DisplayName("Network Library")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter("System.Data.SqlClient.SqlConnectionStringBuilder+NetworkLibraryConverter, System.Data, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
		public string NetworkLibrary
		{
			get
			{
				return _networkLibrary;
			}
			set
			{
				base["Network Library"] = value;
				_networkLibrary = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Packet Size")]
		public int PacketSize
		{
			get
			{
				return _packetSize;
			}
			set
			{
				base["Packet Size"] = value;
				_packetSize = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Password")]
		[PasswordPropertyText(true)]
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				base["Password"] = value;
				_password = value;
			}
		}

		[DisplayName("Persist Security Info")]
		[RefreshProperties(RefreshProperties.All)]
		public bool PersistSecurityInfo
		{
			get
			{
				return _persistSecurityInfo;
			}
			set
			{
				base["Persist Security Info"] = value;
				_persistSecurityInfo = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Pooling")]
		public bool Pooling
		{
			get
			{
				return _pooling;
			}
			set
			{
				base["Pooling"] = value;
				_pooling = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Replication")]
		public bool Replication
		{
			get
			{
				return _replication;
			}
			set
			{
				base["Replication"] = value;
				_replication = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("User ID")]
		public string UserID
		{
			get
			{
				return _userID;
			}
			set
			{
				base["User Id"] = value;
				_userID = value;
			}
		}

		public override ICollection Values
		{
			get
			{
				List<object> list = new List<object>();
				list.Add(_dataSource);
				list.Add(_failoverPartner);
				list.Add(_attachDBFilename);
				list.Add(_initialCatalog);
				list.Add(_integratedSecurity);
				list.Add(_persistSecurityInfo);
				list.Add(_userID);
				list.Add(_password);
				list.Add(_enlist);
				list.Add(_pooling);
				list.Add(_minPoolSize);
				list.Add(_maxPoolSize);
				list.Add(_asynchronousProcessing);
				list.Add(_connectionReset);
				list.Add(_multipleActiveResultSets);
				list.Add(_replication);
				list.Add(_connectTimeout);
				list.Add(_encrypt);
				list.Add(_trustServerCertificate);
				list.Add(_loadBalanceTimeout);
				list.Add(_networkLibrary);
				list.Add(_packetSize);
				list.Add(_typeSystemVersion);
				list.Add(_applicationName);
				list.Add(_currentLanguage);
				list.Add(_workstationID);
				list.Add(_userInstance);
				list.Add(_contextConnection);
				list.Add(_transactionBinding);
				return new ReadOnlyCollection<object>(list);
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Workstation ID")]
		public string WorkstationID
		{
			get
			{
				return _workstationID;
			}
			set
			{
				base["Workstation Id"] = value;
				_workstationID = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("TrustServerCertificate")]
		public bool TrustServerCertificate
		{
			get
			{
				return _trustServerCertificate;
			}
			set
			{
				base["Trust Server Certificate"] = value;
				_trustServerCertificate = value;
			}
		}

		[DisplayName("Type System Version")]
		[RefreshProperties(RefreshProperties.All)]
		public string TypeSystemVersion
		{
			get
			{
				return _typeSystemVersion;
			}
			set
			{
				base["Type System Version"] = value;
				_typeSystemVersion = value;
			}
		}

		[DisplayName("User Instance")]
		[RefreshProperties(RefreshProperties.All)]
		public bool UserInstance
		{
			get
			{
				return _userInstance;
			}
			set
			{
				base["User Instance"] = value;
				_userInstance = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DisplayName("Context Connection")]
		public bool ContextConnection
		{
			get
			{
				return _contextConnection;
			}
			set
			{
				base["Context Connection"] = value;
				_contextConnection = value;
			}
		}

		public SqlConnectionStringBuilder()
			: this(string.Empty)
		{
		}

		public SqlConnectionStringBuilder(string connectionString)
		{
			Init();
			base.ConnectionString = connectionString;
		}

		static SqlConnectionStringBuilder()
		{
			_keywords = new Dictionary<string, string>();
			_keywords["APP"] = "Application Name";
			_keywords["APPLICATION NAME"] = "Application Name";
			_keywords["ATTACHDBFILENAME"] = "AttachDbFilename";
			_keywords["EXTENDED PROPERTIES"] = "Extended Properties";
			_keywords["INITIAL FILE NAME"] = "Initial File Name";
			_keywords["TIMEOUT"] = "Connect Timeout";
			_keywords["CONNECT TIMEOUT"] = "Connect Timeout";
			_keywords["CONNECTION TIMEOUT"] = "Connect Timeout";
			_keywords["CONNECTION RESET"] = "Connection Reset";
			_keywords["LANGUAGE"] = "Current Language";
			_keywords["CURRENT LANGUAGE"] = "Current Language";
			_keywords["DATA SOURCE"] = "Data Source";
			_keywords["SERVER"] = "Data Source";
			_keywords["ADDRESS"] = "Data Source";
			_keywords["ADDR"] = "Data Source";
			_keywords["NETWORK ADDRESS"] = "Data Source";
			_keywords["ENCRYPT"] = "Encrypt";
			_keywords["ENLIST"] = "Enlist";
			_keywords["INITIAL CATALOG"] = "Initial Catalog";
			_keywords["DATABASE"] = "Initial Catalog";
			_keywords["INTEGRATED SECURITY"] = "Integrated Security";
			_keywords["TRUSTED_CONNECTION"] = "Integrated Security";
			_keywords["MAX POOL SIZE"] = "Max Pool Size";
			_keywords["MIN POOL SIZE"] = "Min Pool Size";
			_keywords["MULTIPLEACTIVERESULTSETS"] = "MultipleActiveResultSets";
			_keywords["ASYNCHRONOUS PROCESSING"] = "Asynchronous Processing";
			_keywords["ASYNC"] = "Async";
			_keywords["NET"] = "Network Library";
			_keywords["NETWORK"] = "Network Library";
			_keywords["NETWORK LIBRARY"] = "Network Library";
			_keywords["PACKET SIZE"] = "Packet Size";
			_keywords["PASSWORD"] = "Password";
			_keywords["PWD"] = "Password";
			_keywords["PERSISTSECURITYINFO"] = "Persist Security Info";
			_keywords["PERSIST SECURITY INFO"] = "Persist Security Info";
			_keywords["POOLING"] = "Pooling";
			_keywords["UID"] = "User ID";
			_keywords["USER"] = "User ID";
			_keywords["USER ID"] = "User ID";
			_keywords["WSID"] = "Workstation ID";
			_keywords["WORKSTATION ID"] = "Workstation ID";
			_keywords["USER INSTANCE"] = "User Instance";
			_keywords["CONTEXT CONNECTION"] = "Context Connection";
			_keywords["TRANSACTION BINDING"] = "Transaction Binding";
			_keywords["FAILOVER PARTNER"] = "Failover Partner";
			_keywords["REPLICATION"] = "Replication";
			_keywords["TRUSTSERVERCERTIFICATE"] = "TrustServerCertificate";
			_keywords["LOAD BALANCE TIMEOUT"] = "Load Balance Timeout";
			_keywords["TYPE SYSTEM VERSION"] = "Type System Version";
			_defaults = new Dictionary<string, object>();
			_defaults.Add("Data Source", string.Empty);
			_defaults.Add("Failover Partner", string.Empty);
			_defaults.Add("AttachDbFilename", string.Empty);
			_defaults.Add("Initial Catalog", string.Empty);
			_defaults.Add("Integrated Security", false);
			_defaults.Add("Persist Security Info", false);
			_defaults.Add("User ID", string.Empty);
			_defaults.Add("Password", string.Empty);
			_defaults.Add("Enlist", false);
			_defaults.Add("Pooling", true);
			_defaults.Add("Min Pool Size", 0);
			_defaults.Add("Max Pool Size", 100);
			_defaults.Add("Asynchronous Processing", false);
			_defaults.Add("Connection Reset", true);
			_defaults.Add("MultipleActiveResultSets", false);
			_defaults.Add("Replication", false);
			_defaults.Add("Connect Timeout", 15);
			_defaults.Add("Encrypt", false);
			_defaults.Add("TrustServerCertificate", false);
			_defaults.Add("Load Balance Timeout", 0);
			_defaults.Add("Network Library", string.Empty);
			_defaults.Add("Packet Size", 8000);
			_defaults.Add("Type System Version", "Latest");
			_defaults.Add("Application Name", ".NET SqlClient Data Provider");
			_defaults.Add("Current Language", string.Empty);
			_defaults.Add("Workstation ID", string.Empty);
			_defaults.Add("User Instance", false);
			_defaults.Add("Context Connection", false);
			_defaults.Add("Transaction Binding", "Implicit Unbind");
		}

		private void Init()
		{
			_applicationName = ".NET SqlClient Data Provider";
			_asynchronousProcessing = false;
			_attachDBFilename = string.Empty;
			_connectionReset = true;
			_connectTimeout = 15;
			_currentLanguage = string.Empty;
			_dataSource = string.Empty;
			_encrypt = false;
			_enlist = false;
			_failoverPartner = string.Empty;
			_initialCatalog = string.Empty;
			_integratedSecurity = false;
			_loadBalanceTimeout = 0;
			_maxPoolSize = 100;
			_minPoolSize = 0;
			_multipleActiveResultSets = false;
			_networkLibrary = string.Empty;
			_packetSize = 8000;
			_password = string.Empty;
			_persistSecurityInfo = false;
			_pooling = true;
			_replication = false;
			_userID = string.Empty;
			_workstationID = string.Empty;
			_trustServerCertificate = false;
			_typeSystemVersion = "Latest";
			_userInstance = false;
			_contextConnection = false;
			_transactionBinding = "Implicit Unbind";
		}

		public override void Clear()
		{
			base.Clear();
			Init();
		}

		public override bool ContainsKey(string keyword)
		{
			keyword = keyword.ToUpper().Trim();
			if (_keywords.ContainsKey(keyword))
			{
				return base.ContainsKey(_keywords[keyword]);
			}
			return false;
		}

		public override bool Remove(string keyword)
		{
			if (!ContainsKey(keyword))
			{
				return false;
			}
			this[keyword] = null;
			return true;
		}

		[System.MonoNotSupported("")]
		public override bool ShouldSerialize(string keyword)
		{
			if (!ContainsKey(keyword))
			{
				return false;
			}
			keyword = keyword.ToUpper().Trim();
			if (_keywords[keyword] == "Password")
			{
				return false;
			}
			return base.ShouldSerialize(_keywords[keyword]);
		}

		public override bool TryGetValue(string keyword, out object value)
		{
			if (!ContainsKey(keyword))
			{
				value = string.Empty;
				return false;
			}
			return base.TryGetValue(_keywords[keyword.ToUpper().Trim()], out value);
		}

		private string MapKeyword(string keyword)
		{
			keyword = keyword.ToUpper().Trim();
			if (!_keywords.ContainsKey(keyword))
			{
				throw new ArgumentException("Keyword not supported :" + keyword);
			}
			return _keywords[keyword];
		}

		private void SetValue(string key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key cannot be null!");
			}
			string text = MapKeyword(key);
			switch (text.ToUpper().Trim())
			{
			case "APPLICATION NAME":
				if (value == null)
				{
					_applicationName = ".NET SqlClient Data Provider";
					base.Remove(text);
				}
				else
				{
					ApplicationName = value.ToString();
				}
				break;
			case "ATTACHDBFILENAME":
				throw new NotImplementedException("Attachable database support is not implemented.");
			case "CONNECT TIMEOUT":
				if (value == null)
				{
					_connectTimeout = 15;
					base.Remove(text);
				}
				else
				{
					ConnectTimeout = DbConnectionStringBuilderHelper.ConvertToInt32(value);
				}
				break;
			case "CONNECTION LIFETIME":
				break;
			case "CONNECTION RESET":
				if (value == null)
				{
					_connectionReset = true;
					base.Remove(text);
				}
				else
				{
					ConnectionReset = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
				}
				break;
			case "CURRENT LANGUAGE":
				if (value == null)
				{
					_currentLanguage = string.Empty;
					base.Remove(text);
				}
				else
				{
					CurrentLanguage = value.ToString();
				}
				break;
			case "CONTEXT CONNECTION":
				if (value == null)
				{
					_contextConnection = false;
					base.Remove(text);
				}
				else
				{
					ContextConnection = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
				}
				break;
			case "DATA SOURCE":
				if (value == null)
				{
					_dataSource = string.Empty;
					base.Remove(text);
				}
				else
				{
					DataSource = value.ToString();
				}
				break;
			case "ENCRYPT":
				if (value == null)
				{
					_encrypt = false;
					base.Remove(text);
				}
				else if (DbConnectionStringBuilderHelper.ConvertToBoolean(value))
				{
					throw new NotImplementedException("SSL encryption for data sent between client and server is not implemented.");
				}
				break;
			case "ENLIST":
				if (value == null)
				{
					_enlist = false;
					base.Remove(text);
				}
				else if (!DbConnectionStringBuilderHelper.ConvertToBoolean(value))
				{
					throw new NotImplementedException("Disabling the automatic enlistment of connections in the thread's current transaction context is not implemented.");
				}
				break;
			case "INITIAL CATALOG":
				if (value == null)
				{
					_initialCatalog = string.Empty;
					base.Remove(text);
				}
				else
				{
					InitialCatalog = value.ToString();
				}
				break;
			case "INTEGRATED SECURITY":
				if (value == null)
				{
					_integratedSecurity = false;
					base.Remove(text);
				}
				else
				{
					IntegratedSecurity = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
				}
				break;
			case "MAX POOL SIZE":
				if (value == null)
				{
					_maxPoolSize = 100;
					base.Remove(text);
				}
				else
				{
					MaxPoolSize = DbConnectionStringBuilderHelper.ConvertToInt32(value);
				}
				break;
			case "MIN POOL SIZE":
				if (value == null)
				{
					_minPoolSize = 0;
					base.Remove(text);
				}
				else
				{
					MinPoolSize = DbConnectionStringBuilderHelper.ConvertToInt32(value);
				}
				break;
			case "MULTIPLEACTIVERESULTSETS":
				if (value == null)
				{
					_multipleActiveResultSets = false;
					base.Remove(text);
				}
				else if (DbConnectionStringBuilderHelper.ConvertToBoolean(value))
				{
					throw new NotImplementedException("MARS is not yet implemented!");
				}
				break;
			case "ASYNCHRONOUS PROCESSING":
				if (value == null)
				{
					_asynchronousProcessing = false;
					base.Remove(text);
				}
				else
				{
					AsynchronousProcessing = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
				}
				break;
			case "NETWORK LIBRARY":
				if (value == null)
				{
					_networkLibrary = string.Empty;
					base.Remove(text);
					break;
				}
				if (!value.ToString().ToUpper().Equals("DBMSSOCN"))
				{
					throw new ArgumentException("Unsupported network library.");
				}
				NetworkLibrary = value.ToString().ToLower();
				break;
			case "LOAD BALANCE TIMEOUT":
				break;
			case "PACKET SIZE":
				if (value == null)
				{
					_packetSize = 8000;
					base.Remove(text);
				}
				else
				{
					PacketSize = DbConnectionStringBuilderHelper.ConvertToInt32(value);
				}
				break;
			case "PASSWORD":
				if (value == null)
				{
					_password = string.Empty;
					base.Remove(text);
				}
				else
				{
					Password = value.ToString();
				}
				break;
			case "PERSIST SECURITY INFO":
				if (value == null)
				{
					_persistSecurityInfo = false;
					base.Remove(text);
				}
				else if (DbConnectionStringBuilderHelper.ConvertToBoolean(value))
				{
					throw new NotImplementedException("Persisting security info is not yet implemented");
				}
				break;
			case "POOLING":
				if (value == null)
				{
					_pooling = true;
					base.Remove(text);
				}
				else
				{
					Pooling = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
				}
				break;
			case "USER ID":
				if (value == null)
				{
					_userID = string.Empty;
					base.Remove(text);
				}
				else
				{
					UserID = value.ToString();
				}
				break;
			case "USER INSTANCE":
				if (value == null)
				{
					_userInstance = false;
					base.Remove(text);
				}
				else
				{
					UserInstance = DbConnectionStringBuilderHelper.ConvertToBoolean(value);
				}
				break;
			case "WORKSTATION ID":
				if (value == null)
				{
					_workstationID = string.Empty;
					base.Remove(text);
				}
				else
				{
					WorkstationID = value.ToString();
				}
				break;
			case "TRANSACTION BINDING":
				break;
			default:
				throw new ArgumentException("Keyword not supported :" + key);
			}
		}
	}
}
