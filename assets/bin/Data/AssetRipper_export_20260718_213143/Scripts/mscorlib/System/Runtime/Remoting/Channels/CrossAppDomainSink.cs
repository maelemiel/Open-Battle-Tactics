using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Channels
{
	[MonoTODO("Handle domain unloading?")]
	internal class CrossAppDomainSink : IMessageSink
	{
		private struct ProcessMessageRes
		{
			public byte[] arrResponse;

			public CADMethodReturnMessage cadMrm;
		}

		private static Hashtable s_sinks = new Hashtable();

		private static MethodInfo processMessageMethod = typeof(CrossAppDomainSink).GetMethod("ProcessMessageInDomain", BindingFlags.Static | BindingFlags.NonPublic);

		private int _domainID;

		internal int TargetDomainId
		{
			get
			{
				return _domainID;
			}
		}

		public IMessageSink NextSink
		{
			get
			{
				return null;
			}
		}

		internal CrossAppDomainSink(int domainID)
		{
			_domainID = domainID;
		}

		internal static CrossAppDomainSink GetSink(int domainID)
		{
			lock (s_sinks.SyncRoot)
			{
				if (s_sinks.ContainsKey(domainID))
				{
					return (CrossAppDomainSink)s_sinks[domainID];
				}
				CrossAppDomainSink crossAppDomainSink = new CrossAppDomainSink(domainID);
				s_sinks[domainID] = crossAppDomainSink;
				return crossAppDomainSink;
			}
		}

		private static ProcessMessageRes ProcessMessageInDomain(byte[] arrRequest, CADMethodCallMessage cadMsg)
		{
			ProcessMessageRes result = default(ProcessMessageRes);
			try
			{
				AppDomain.CurrentDomain.ProcessMessageInDomain(arrRequest, cadMsg, out result.arrResponse, out result.cadMrm);
			}
			catch (Exception e)
			{
				IMessage msg = new MethodResponse(e, new ErrorMessage());
				result.arrResponse = CADSerializer.SerializeMessage(msg).GetBuffer();
			}
			return result;
		}

		public virtual IMessage SyncProcessMessage(IMessage msgRequest)
		{
			IMessage result = null;
			try
			{
				byte[] array = null;
				byte[] array2 = null;
				CADMethodReturnMessage retmsg = null;
				CADMethodCallMessage cADMethodCallMessage = CADMethodCallMessage.Create(msgRequest);
				if (cADMethodCallMessage == null)
				{
					MemoryStream memoryStream = CADSerializer.SerializeMessage(msgRequest);
					array2 = memoryStream.GetBuffer();
				}
				Context currentContext = Thread.CurrentContext;
				try
				{
					ProcessMessageRes processMessageRes = (ProcessMessageRes)AppDomain.InvokeInDomainByID(_domainID, processMessageMethod, null, new object[2] { array2, cADMethodCallMessage });
					array = processMessageRes.arrResponse;
					retmsg = processMessageRes.cadMrm;
				}
				finally
				{
					AppDomain.InternalSetContext(currentContext);
				}
				if (array != null)
				{
					MemoryStream mem = new MemoryStream(array);
					result = CADSerializer.DeserializeMessage(mem, msgRequest as IMethodCallMessage);
				}
				else
				{
					result = new MethodResponse(msgRequest as IMethodCallMessage, retmsg);
				}
			}
			catch (Exception e)
			{
				try
				{
					result = new ReturnMessage(e, msgRequest as IMethodCallMessage);
				}
				catch (Exception)
				{
				}
			}
			return result;
		}

		public virtual IMessageCtrl AsyncProcessMessage(IMessage reqMsg, IMessageSink replySink)
		{
			AsyncRequest state = new AsyncRequest(reqMsg, replySink);
			ThreadPool.QueueUserWorkItem(SendAsyncMessage, state);
			return null;
		}

		public void SendAsyncMessage(object data)
		{
			AsyncRequest asyncRequest = (AsyncRequest)data;
			IMessage msg = SyncProcessMessage(asyncRequest.MsgRequest);
			asyncRequest.ReplySink.SyncProcessMessage(msg);
		}
	}
}
