using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Mono.Security.X509;

namespace Mono.Security.Protocol.Tls
{
	public abstract class SslStreamBase : Stream, IDisposable
	{
		private class InternalAsyncResult : IAsyncResult
		{
			private object locker = new object();

			private AsyncCallback _userCallback;

			private object _userState;

			private Exception _asyncException;

			private ManualResetEvent handle;

			private bool completed;

			private int _bytesRead;

			private bool _fromWrite;

			private bool _proceedAfterHandshake;

			private byte[] _buffer;

			private int _offset;

			private int _count;

			public bool ProceedAfterHandshake
			{
				get
				{
					return _proceedAfterHandshake;
				}
			}

			public bool FromWrite
			{
				get
				{
					return _fromWrite;
				}
			}

			public byte[] Buffer
			{
				get
				{
					return _buffer;
				}
			}

			public int Offset
			{
				get
				{
					return _offset;
				}
			}

			public int Count
			{
				get
				{
					return _count;
				}
			}

			public int BytesRead
			{
				get
				{
					return _bytesRead;
				}
			}

			public object AsyncState
			{
				get
				{
					return _userState;
				}
			}

			public Exception AsyncException
			{
				get
				{
					return _asyncException;
				}
			}

			public bool CompletedWithError
			{
				get
				{
					if (!IsCompleted)
					{
						return false;
					}
					return null != _asyncException;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					lock (locker)
					{
						if (handle == null)
						{
							handle = new ManualResetEvent(completed);
						}
					}
					return handle;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public bool IsCompleted
			{
				get
				{
					lock (locker)
					{
						return completed;
					}
				}
			}

			public InternalAsyncResult(AsyncCallback userCallback, object userState, byte[] buffer, int offset, int count, bool fromWrite, bool proceedAfterHandshake)
			{
				_userCallback = userCallback;
				_userState = userState;
				_buffer = buffer;
				_offset = offset;
				_count = count;
				_fromWrite = fromWrite;
				_proceedAfterHandshake = proceedAfterHandshake;
			}

			private void SetComplete(Exception ex, int bytesRead)
			{
				lock (locker)
				{
					if (completed)
					{
						return;
					}
					completed = true;
					_asyncException = ex;
					_bytesRead = bytesRead;
					if (handle != null)
					{
						handle.Set();
					}
				}
				if (_userCallback != null)
				{
					_userCallback.BeginInvoke(this, null, null);
				}
			}

			public void SetComplete(Exception ex)
			{
				SetComplete(ex, 0);
			}

			public void SetComplete(int bytesRead)
			{
				SetComplete(null, bytesRead);
			}

			public void SetComplete()
			{
				SetComplete(null, 0);
			}
		}

		private delegate void AsyncHandshakeDelegate(InternalAsyncResult asyncResult, bool fromWrite);

		private const int WaitTimeOut = 300000;

		private static ManualResetEvent record_processing = new ManualResetEvent(true);

		internal Stream innerStream;

		internal MemoryStream inputBuffer;

		internal Context context;

		internal RecordProtocol protocol;

		internal bool ownsStream;

		private volatile bool disposed;

		private bool checkCertRevocationStatus;

		private object negotiate;

		private object read;

		private object write;

		private ManualResetEvent negotiationComplete;

		private byte[] recbuf = new byte[16384];

		private MemoryStream recordStream = new MemoryStream();

		internal bool MightNeedHandshake
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return false;
				}
				lock (negotiate)
				{
					return context.HandshakeState != HandshakeState.Finished;
				}
			}
		}

		internal abstract bool HaveRemoteValidation2Callback { get; }

		public bool CheckCertRevocationStatus
		{
			get
			{
				return checkCertRevocationStatus;
			}
			set
			{
				checkCertRevocationStatus = value;
			}
		}

		public CipherAlgorithmType CipherAlgorithm
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.Current.Cipher.CipherAlgorithmType;
				}
				return CipherAlgorithmType.None;
			}
		}

		public int CipherStrength
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.Current.Cipher.EffectiveKeyBits;
				}
				return 0;
			}
		}

		public HashAlgorithmType HashAlgorithm
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.Current.Cipher.HashAlgorithmType;
				}
				return HashAlgorithmType.None;
			}
		}

		public int HashStrength
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.Current.Cipher.HashSize * 8;
				}
				return 0;
			}
		}

		public int KeyExchangeStrength
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.ServerSettings.Certificates[0].RSA.KeySize;
				}
				return 0;
			}
		}

		public ExchangeAlgorithmType KeyExchangeAlgorithm
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.Current.Cipher.ExchangeAlgorithmType;
				}
				return ExchangeAlgorithmType.None;
			}
		}

		public SecurityProtocolType SecurityProtocol
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished)
				{
					return context.SecurityProtocol;
				}
				return (SecurityProtocolType)0;
			}
		}

		public System.Security.Cryptography.X509Certificates.X509Certificate ServerCertificate
		{
			get
			{
				if (context.HandshakeState == HandshakeState.Finished && context.ServerSettings.Certificates != null && context.ServerSettings.Certificates.Count > 0)
				{
					return new System.Security.Cryptography.X509Certificates.X509Certificate(context.ServerSettings.Certificates[0].RawData);
				}
				return null;
			}
		}

		internal Mono.Security.X509.X509CertificateCollection ServerCertificates
		{
			get
			{
				return context.ServerSettings.Certificates;
			}
		}

		public override bool CanRead
		{
			get
			{
				return innerStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return innerStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		protected SslStreamBase(Stream stream, bool ownsStream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream is null.");
			}
			if (!stream.CanRead || !stream.CanWrite)
			{
				throw new ArgumentNullException("stream is not both readable and writable.");
			}
			inputBuffer = new MemoryStream();
			innerStream = stream;
			this.ownsStream = ownsStream;
			negotiate = new object();
			read = new object();
			write = new object();
			negotiationComplete = new ManualResetEvent(false);
		}

		private void AsyncHandshakeCallback(IAsyncResult asyncResult)
		{
			InternalAsyncResult internalAsyncResult = asyncResult.AsyncState as InternalAsyncResult;
			try
			{
				try
				{
					OnNegotiateHandshakeCallback(asyncResult);
				}
				catch (TlsException ex)
				{
					protocol.SendAlert(ex.Alert);
					throw new IOException("The authentication or decryption has failed.", ex);
				}
				catch (Exception innerException)
				{
					protocol.SendAlert(AlertDescription.InternalError);
					throw new IOException("The authentication or decryption has failed.", innerException);
				}
				if (internalAsyncResult.ProceedAfterHandshake)
				{
					if (internalAsyncResult.FromWrite)
					{
						InternalBeginWrite(internalAsyncResult);
					}
					else
					{
						InternalBeginRead(internalAsyncResult);
					}
					negotiationComplete.Set();
				}
				else
				{
					negotiationComplete.Set();
					internalAsyncResult.SetComplete();
				}
			}
			catch (Exception complete)
			{
				negotiationComplete.Set();
				internalAsyncResult.SetComplete(complete);
			}
		}

		internal void NegotiateHandshake()
		{
			if (MightNeedHandshake)
			{
				InternalAsyncResult asyncResult = new InternalAsyncResult(null, null, null, 0, 0, false, false);
				if (!BeginNegotiateHandshake(asyncResult))
				{
					negotiationComplete.WaitOne();
				}
				else
				{
					EndNegotiateHandshake(asyncResult);
				}
			}
		}

		internal abstract IAsyncResult OnBeginNegotiateHandshake(AsyncCallback callback, object state);

		internal abstract void OnNegotiateHandshakeCallback(IAsyncResult asyncResult);

		internal abstract System.Security.Cryptography.X509Certificates.X509Certificate OnLocalCertificateSelection(System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates, System.Security.Cryptography.X509Certificates.X509Certificate serverCertificate, string targetHost, System.Security.Cryptography.X509Certificates.X509CertificateCollection serverRequestedCertificates);

		internal abstract bool OnRemoteCertificateValidation(System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] errors);

		internal abstract ValidationResult OnRemoteCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection);

		internal abstract AsymmetricAlgorithm OnLocalPrivateKeySelection(System.Security.Cryptography.X509Certificates.X509Certificate certificate, string targetHost);

		internal System.Security.Cryptography.X509Certificates.X509Certificate RaiseLocalCertificateSelection(System.Security.Cryptography.X509Certificates.X509CertificateCollection certificates, System.Security.Cryptography.X509Certificates.X509Certificate remoteCertificate, string targetHost, System.Security.Cryptography.X509Certificates.X509CertificateCollection requestedCertificates)
		{
			return OnLocalCertificateSelection(certificates, remoteCertificate, targetHost, requestedCertificates);
		}

		internal bool RaiseRemoteCertificateValidation(System.Security.Cryptography.X509Certificates.X509Certificate certificate, int[] errors)
		{
			return OnRemoteCertificateValidation(certificate, errors);
		}

		internal ValidationResult RaiseRemoteCertificateValidation2(Mono.Security.X509.X509CertificateCollection collection)
		{
			return OnRemoteCertificateValidation2(collection);
		}

		internal AsymmetricAlgorithm RaiseLocalPrivateKeySelection(System.Security.Cryptography.X509Certificates.X509Certificate certificate, string targetHost)
		{
			return OnLocalPrivateKeySelection(certificate, targetHost);
		}

		private bool BeginNegotiateHandshake(InternalAsyncResult asyncResult)
		{
			try
			{
				lock (negotiate)
				{
					if (context.HandshakeState == HandshakeState.None)
					{
						OnBeginNegotiateHandshake(AsyncHandshakeCallback, asyncResult);
						return true;
					}
					return false;
				}
			}
			catch (TlsException ex)
			{
				negotiationComplete.Set();
				protocol.SendAlert(ex.Alert);
				throw new IOException("The authentication or decryption has failed.", ex);
			}
			catch (Exception innerException)
			{
				negotiationComplete.Set();
				protocol.SendAlert(AlertDescription.InternalError);
				throw new IOException("The authentication or decryption has failed.", innerException);
			}
		}

		private void EndNegotiateHandshake(InternalAsyncResult asyncResult)
		{
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			if (asyncResult.CompletedWithError)
			{
				throw asyncResult.AsyncException;
			}
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			checkDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is a null reference.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset is less than 0.");
			}
			if (offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset is greater than the length of buffer.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0.");
			}
			if (count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count is less than the length of buffer minus the value of the offset parameter.");
			}
			InternalAsyncResult internalAsyncResult = new InternalAsyncResult(callback, state, buffer, offset, count, false, true);
			if (MightNeedHandshake)
			{
				if (!BeginNegotiateHandshake(internalAsyncResult))
				{
					negotiationComplete.WaitOne();
					InternalBeginRead(internalAsyncResult);
				}
			}
			else
			{
				InternalBeginRead(internalAsyncResult);
			}
			return internalAsyncResult;
		}

		private void InternalBeginRead(InternalAsyncResult asyncResult)
		{
			try
			{
				int num = 0;
				lock (read)
				{
					bool flag = inputBuffer.Position == inputBuffer.Length && inputBuffer.Length > 0;
					bool flag2 = inputBuffer.Length > 0 && asyncResult.Count > 0;
					if (flag)
					{
						resetBuffer();
					}
					else if (flag2)
					{
						num = inputBuffer.Read(asyncResult.Buffer, asyncResult.Offset, asyncResult.Count);
					}
				}
				if (0 < num)
				{
					asyncResult.SetComplete(num);
				}
				else if (!context.ReceivedConnectionEnd)
				{
					innerStream.BeginRead(recbuf, 0, recbuf.Length, InternalReadCallback, new object[2] { recbuf, asyncResult });
				}
				else
				{
					asyncResult.SetComplete(0);
				}
			}
			catch (TlsException ex)
			{
				protocol.SendAlert(ex.Alert);
				throw new IOException("The authentication or decryption has failed.", ex);
			}
			catch (Exception innerException)
			{
				throw new IOException("IO exception during read.", innerException);
			}
		}

		private void InternalReadCallback(IAsyncResult result)
		{
			if (disposed)
			{
				return;
			}
			object[] array = (object[])result.AsyncState;
			byte[] array2 = (byte[])array[0];
			InternalAsyncResult internalAsyncResult = (InternalAsyncResult)array[1];
			try
			{
				int num = innerStream.EndRead(result);
				if (num > 0)
				{
					recordStream.Write(array2, 0, num);
					bool flag = false;
					long position = recordStream.Position;
					recordStream.Position = 0L;
					byte[] array3 = null;
					if (recordStream.Length >= 5)
					{
						array3 = protocol.ReceiveRecord(recordStream);
					}
					while (array3 != null)
					{
						long num2 = recordStream.Length - recordStream.Position;
						byte[] array4 = null;
						if (num2 > 0)
						{
							array4 = new byte[num2];
							recordStream.Read(array4, 0, array4.Length);
						}
						lock (read)
						{
							long position2 = inputBuffer.Position;
							if (array3.Length > 0)
							{
								inputBuffer.Seek(0L, SeekOrigin.End);
								inputBuffer.Write(array3, 0, array3.Length);
								inputBuffer.Seek(position2, SeekOrigin.Begin);
								flag = true;
							}
						}
						recordStream.SetLength(0L);
						array3 = null;
						if (num2 > 0)
						{
							recordStream.Write(array4, 0, array4.Length);
							if (recordStream.Length >= 5)
							{
								recordStream.Position = 0L;
								array3 = protocol.ReceiveRecord(recordStream);
								if (array3 == null)
								{
									position = recordStream.Length;
								}
							}
							else
							{
								position = num2;
							}
						}
						else
						{
							position = 0L;
						}
					}
					if (!flag && num > 0)
					{
						if (context.ReceivedConnectionEnd)
						{
							internalAsyncResult.SetComplete(0);
							return;
						}
						recordStream.Position = recordStream.Length;
						innerStream.BeginRead(array2, 0, array2.Length, InternalReadCallback, array);
						return;
					}
					recordStream.Position = position;
					int complete = 0;
					lock (read)
					{
						complete = inputBuffer.Read(internalAsyncResult.Buffer, internalAsyncResult.Offset, internalAsyncResult.Count);
					}
					internalAsyncResult.SetComplete(complete);
				}
				else
				{
					internalAsyncResult.SetComplete(0);
				}
			}
			catch (Exception complete2)
			{
				internalAsyncResult.SetComplete(complete2);
			}
		}

		private void InternalBeginWrite(InternalAsyncResult asyncResult)
		{
			try
			{
				lock (write)
				{
					byte[] array = protocol.EncodeRecord(ContentType.ApplicationData, asyncResult.Buffer, asyncResult.Offset, asyncResult.Count);
					innerStream.BeginWrite(array, 0, array.Length, InternalWriteCallback, asyncResult);
				}
			}
			catch (TlsException ex)
			{
				protocol.SendAlert(ex.Alert);
				Close();
				throw new IOException("The authentication or decryption has failed.", ex);
			}
			catch (Exception innerException)
			{
				throw new IOException("IO exception during Write.", innerException);
			}
		}

		private void InternalWriteCallback(IAsyncResult ar)
		{
			if (disposed)
			{
				return;
			}
			InternalAsyncResult internalAsyncResult = (InternalAsyncResult)ar.AsyncState;
			try
			{
				innerStream.EndWrite(ar);
				internalAsyncResult.SetComplete();
			}
			catch (Exception complete)
			{
				internalAsyncResult.SetComplete(complete);
			}
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			checkDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer is a null reference.");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset is less than 0.");
			}
			if (offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset is greater than the length of buffer.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0.");
			}
			if (count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count is less than the length of buffer minus the value of the offset parameter.");
			}
			InternalAsyncResult internalAsyncResult = new InternalAsyncResult(callback, state, buffer, offset, count, true, true);
			if (MightNeedHandshake)
			{
				if (!BeginNegotiateHandshake(internalAsyncResult))
				{
					negotiationComplete.WaitOne();
					InternalBeginWrite(internalAsyncResult);
				}
			}
			else
			{
				InternalBeginWrite(internalAsyncResult);
			}
			return internalAsyncResult;
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			checkDisposed();
			InternalAsyncResult internalAsyncResult = asyncResult as InternalAsyncResult;
			if (internalAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult is null or was not obtained by calling BeginRead.");
			}
			if (!asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(300000, false))
			{
				throw new TlsException(AlertDescription.InternalError, "Couldn't complete EndRead");
			}
			if (internalAsyncResult.CompletedWithError)
			{
				throw internalAsyncResult.AsyncException;
			}
			return internalAsyncResult.BytesRead;
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			checkDisposed();
			InternalAsyncResult internalAsyncResult = asyncResult as InternalAsyncResult;
			if (internalAsyncResult == null)
			{
				throw new ArgumentNullException("asyncResult is null or was not obtained by calling BeginWrite.");
			}
			if (!asyncResult.IsCompleted && !internalAsyncResult.AsyncWaitHandle.WaitOne(300000, false))
			{
				throw new TlsException(AlertDescription.InternalError, "Couldn't complete EndWrite");
			}
			if (internalAsyncResult.CompletedWithError)
			{
				throw internalAsyncResult.AsyncException;
			}
		}

		public override void Close()
		{
			base.Close();
		}

		public override void Flush()
		{
			checkDisposed();
			innerStream.Flush();
		}

		public int Read(byte[] buffer)
		{
			return Read(buffer, 0, buffer.Length);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			checkDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset is less than 0.");
			}
			if (offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset is greater than the length of buffer.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0.");
			}
			if (count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count is less than the length of buffer minus the value of the offset parameter.");
			}
			if (context.HandshakeState != HandshakeState.Finished)
			{
				NegotiateHandshake();
			}
			lock (read)
			{
				try
				{
					record_processing.Reset();
					if (inputBuffer.Position > 0)
					{
						if (inputBuffer.Position == inputBuffer.Length)
						{
							inputBuffer.SetLength(0L);
						}
						else
						{
							int num = inputBuffer.Read(buffer, offset, count);
							if (num > 0)
							{
								record_processing.Set();
								return num;
							}
						}
					}
					bool flag = false;
					while (true)
					{
						if (recordStream.Position == 0L || flag)
						{
							flag = false;
							byte[] array = new byte[16384];
							int num2 = 0;
							if (count == 1)
							{
								int num3 = innerStream.ReadByte();
								if (num3 >= 0)
								{
									array[0] = (byte)num3;
									num2 = 1;
								}
							}
							else
							{
								num2 = innerStream.Read(array, 0, array.Length);
							}
							if (num2 <= 0)
							{
								break;
							}
							if (recordStream.Length > 0 && recordStream.Position != recordStream.Length)
							{
								recordStream.Seek(0L, SeekOrigin.End);
							}
							recordStream.Write(array, 0, num2);
						}
						bool flag2 = false;
						recordStream.Position = 0L;
						byte[] array2 = null;
						if (recordStream.Length >= 5)
						{
							array2 = protocol.ReceiveRecord(recordStream);
							flag = array2 == null;
						}
						while (array2 != null)
						{
							long num4 = recordStream.Length - recordStream.Position;
							byte[] array3 = null;
							if (num4 > 0)
							{
								array3 = new byte[num4];
								recordStream.Read(array3, 0, array3.Length);
							}
							long position = inputBuffer.Position;
							if (array2.Length > 0)
							{
								inputBuffer.Seek(0L, SeekOrigin.End);
								inputBuffer.Write(array2, 0, array2.Length);
								inputBuffer.Seek(position, SeekOrigin.Begin);
								flag2 = true;
							}
							recordStream.SetLength(0L);
							array2 = null;
							if (num4 > 0)
							{
								recordStream.Write(array3, 0, array3.Length);
							}
							if (flag2)
							{
								int result = inputBuffer.Read(buffer, offset, count);
								record_processing.Set();
								return result;
							}
						}
					}
					record_processing.Set();
					return 0;
				}
				catch (TlsException innerException)
				{
					throw new IOException("The authentication or decryption has failed.", innerException);
				}
				catch (Exception innerException2)
				{
					throw new IOException("IO exception during read.", innerException2);
				}
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public void Write(byte[] buffer)
		{
			Write(buffer, 0, buffer.Length);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			checkDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset is less than 0.");
			}
			if (offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset is greater than the length of buffer.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count is less than 0.");
			}
			if (count > buffer.Length - offset)
			{
				throw new ArgumentOutOfRangeException("count is less than the length of buffer minus the value of the offset parameter.");
			}
			if (context.HandshakeState != HandshakeState.Finished)
			{
				NegotiateHandshake();
			}
			lock (write)
			{
				try
				{
					byte[] array = protocol.EncodeRecord(ContentType.ApplicationData, buffer, offset, count);
					innerStream.Write(array, 0, array.Length);
				}
				catch (TlsException ex)
				{
					protocol.SendAlert(ex.Alert);
					Close();
					throw new IOException("The authentication or decryption has failed.", ex);
				}
				catch (Exception innerException)
				{
					throw new IOException("IO exception during Write.", innerException);
				}
			}
		}

		~SslStreamBase()
		{
			Dispose(false);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}
			if (disposing)
			{
				if (innerStream != null)
				{
					if (context.HandshakeState == HandshakeState.Finished && !context.SentConnectionEnd)
					{
						try
						{
							protocol.SendAlert(AlertDescription.CloseNotify);
						}
						catch
						{
						}
					}
					if (ownsStream)
					{
						innerStream.Close();
					}
				}
				ownsStream = false;
				innerStream = null;
			}
			disposed = true;
			base.Dispose(disposing);
		}

		private void resetBuffer()
		{
			inputBuffer.SetLength(0L);
			inputBuffer.Position = 0L;
		}

		internal void checkDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException("The Stream is closed.");
			}
		}
	}
}
