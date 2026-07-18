using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngineInternal;

namespace UnityEngine
{
	public sealed class Network
	{
		public static extern string incomingPassword
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern NetworkLogLevel logLevel
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern NetworkPlayer[] connections
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static NetworkPlayer player
		{
			get
			{
				NetworkPlayer result = default(NetworkPlayer);
				result.index = Internal_GetPlayer();
				return result;
			}
		}

		public static extern bool isClient
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern bool isServer
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern NetworkPeerType peerType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static extern float sendRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool isMessageQueueRunning
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static double time
		{
			get
			{
				double t;
				Internal_GetTime(out t);
				return t;
			}
		}

		public static extern int minimumAllocatableViewIDs
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("No longer needed. This is now explicitly set in the InitializeServer function call. It is implicitly set when calling Connect depending on if an IP/port combination is used (useNat=false) or a GUID is used(useNat=true).")]
		public static extern bool useNat
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern string natFacilitatorIP
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern int natFacilitatorPort
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern string connectionTesterIP
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern int connectionTesterPort
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern int maxConnections
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern string proxyIP
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern int proxyPort
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern bool useProxy
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern string proxyPassword
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern NetworkConnectionError InitializeServer(int connections, int listenPort, bool useNat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern NetworkConnectionError Internal_InitializeServerDeprecated(int connections, int listenPort);

		[Obsolete("Use the IntializeServer(connections, listenPort, useNat) function instead")]
		public static NetworkConnectionError InitializeServer(int connections, int listenPort)
		{
			return Internal_InitializeServerDeprecated(connections, listenPort);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void InitializeSecurity();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

		[ExcludeFromDocs]
		private static NetworkConnectionError Internal_ConnectToSingleIP(string IP, int remotePort, int localPort)
		{
			string empty = string.Empty;
			return Internal_ConnectToSingleIP(IP, remotePort, localPort, empty);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern NetworkConnectionError Internal_ConnectToGuid(string guid, string password);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort, [DefaultValue("\"\"")] string password);

		[ExcludeFromDocs]
		private static NetworkConnectionError Internal_ConnectToIPs(string[] IP, int remotePort, int localPort)
		{
			string empty = string.Empty;
			return Internal_ConnectToIPs(IP, remotePort, localPort, empty);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string IP, int remotePort)
		{
			string empty = string.Empty;
			return Connect(IP, remotePort, empty);
		}

		public static NetworkConnectionError Connect(string IP, int remotePort, [DefaultValue("\"\"")] string password)
		{
			return Internal_ConnectToSingleIP(IP, remotePort, 0, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string[] IPs, int remotePort)
		{
			string empty = string.Empty;
			return Connect(IPs, remotePort, empty);
		}

		public static NetworkConnectionError Connect(string[] IPs, int remotePort, [DefaultValue("\"\"")] string password)
		{
			return Internal_ConnectToIPs(IPs, remotePort, 0, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(string GUID)
		{
			string empty = string.Empty;
			return Connect(GUID, empty);
		}

		public static NetworkConnectionError Connect(string GUID, [DefaultValue("\"\"")] string password)
		{
			return Internal_ConnectToGuid(GUID, password);
		}

		[ExcludeFromDocs]
		public static NetworkConnectionError Connect(HostData hostData)
		{
			string empty = string.Empty;
			return Connect(hostData, empty);
		}

		public static NetworkConnectionError Connect(HostData hostData, [DefaultValue("\"\"")] string password)
		{
			if (hostData == null)
			{
				throw new NullReferenceException();
			}
			if (hostData.guid.Length > 0 && hostData.useNat)
			{
				return Connect(hostData.guid, password);
			}
			return Connect(hostData.ip, hostData.port, password);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void Disconnect([DefaultValue("200")] int timeout);

		[ExcludeFromDocs]
		public static void Disconnect()
		{
			int timeout = 200;
			Disconnect(timeout);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void CloseConnection(NetworkPlayer target, bool sendDisconnectionNotification);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int Internal_GetPlayer();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_AllocateViewID(out NetworkViewID viewID);

		public static NetworkViewID AllocateViewID()
		{
			NetworkViewID viewID;
			Internal_AllocateViewID(out viewID);
			return viewID;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object prefab, Vector3 position, Quaternion rotation, int group)
		{
			return INTERNAL_CALL_Instantiate(prefab, ref position, ref rotation, group);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Object INTERNAL_CALL_Instantiate(Object prefab, ref Vector3 position, ref Quaternion rotation, int group);

		public static void Destroy(NetworkViewID viewID)
		{
			INTERNAL_CALL_Destroy(ref viewID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Destroy(ref NetworkViewID viewID);

		public static void Destroy(GameObject gameObject)
		{
			if (gameObject != null)
			{
				NetworkView networkView = gameObject.networkView;
				if (networkView != null)
				{
					Destroy(networkView.viewID);
				}
				else
				{
					Debug.LogError("Couldn't destroy game object because no network view is attached to it.", gameObject);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void DestroyPlayerObjects(NetworkPlayer playerID);

		private static void Internal_RemoveRPCs(NetworkPlayer playerID, NetworkViewID viewID, uint channelMask)
		{
			INTERNAL_CALL_Internal_RemoveRPCs(playerID, ref viewID, channelMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Internal_RemoveRPCs(NetworkPlayer playerID, ref NetworkViewID viewID, uint channelMask);

		public static void RemoveRPCs(NetworkPlayer playerID)
		{
			Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, uint.MaxValue);
		}

		public static void RemoveRPCs(NetworkPlayer playerID, int group)
		{
			Internal_RemoveRPCs(playerID, NetworkViewID.unassigned, (uint)(1 << group));
		}

		public static void RemoveRPCs(NetworkViewID viewID)
		{
			Internal_RemoveRPCs(NetworkPlayer.unassigned, viewID, uint.MaxValue);
		}

		public static void RemoveRPCsInGroup(int group)
		{
			Internal_RemoveRPCs(NetworkPlayer.unassigned, NetworkViewID.unassigned, (uint)(1 << group));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void SetLevelPrefix(int prefix);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetLastPing(NetworkPlayer player);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetAveragePing(NetworkPlayer player);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void SetReceivingEnabled(NetworkPlayer player, int group, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetSendingGlobal(int group, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetSendingSpecific(NetworkPlayer player, int group, bool enabled);

		public static void SetSendingEnabled(int group, bool enabled)
		{
			Internal_SetSendingGlobal(group, enabled);
		}

		public static void SetSendingEnabled(NetworkPlayer player, int group, bool enabled)
		{
			Internal_SetSendingSpecific(player, group, enabled);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_GetTime(out double t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern ConnectionTesterStatus TestConnection([DefaultValue("false")] bool forceTest);

		[ExcludeFromDocs]
		public static ConnectionTesterStatus TestConnection()
		{
			bool forceTest = false;
			return TestConnection(forceTest);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern ConnectionTesterStatus TestConnectionNAT([DefaultValue("false")] bool forceTest);

		[ExcludeFromDocs]
		public static ConnectionTesterStatus TestConnectionNAT()
		{
			bool forceTest = false;
			return TestConnectionNAT(forceTest);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool HavePublicAddress();
	}
}
