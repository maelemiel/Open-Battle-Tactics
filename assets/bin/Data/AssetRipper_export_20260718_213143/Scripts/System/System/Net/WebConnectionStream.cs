using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace System.Net
{
	internal class WebConnectionStream : Stream
	{
		private static byte[] crlf = new byte[2] { 13, 10 };

		private bool isRead;

		private System.Net.WebConnection cnc;

		private HttpWebRequest request;

		private byte[] readBuffer;

		private int readBufferOffset;

		private int readBufferSize;

		private int contentLength;

		private int totalRead;

		private long totalWritten;

		private bool nextReadCalled;

		private int pendingReads;

		private int pendingWrites;

		private ManualResetEvent pending;

		private bool allowBuffering;

		private bool sendChunked;

		private MemoryStream writeBuffer;

		private bool requestWritten;

		private byte[] headers;

		private bool disposed;

		private bool headersSent;

		private object locker = new object();

		private bool initRead;

		private bool read_eof;

		private bool complete_request_written;

		private int read_timeout;

		private int write_timeout;

		internal HttpWebRequest Request
		{
			get
			{
				return request;
			}
		}

		internal System.Net.WebConnection Connection
		{
			get
			{
				return cnc;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return true;
			}
		}

		public override int ReadTimeout
		{
			get
			{
				return read_timeout;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				read_timeout = value;
			}
		}

		public override int WriteTimeout
		{
			get
			{
				return write_timeout;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				write_timeout = value;
			}
		}

		internal bool CompleteRequestWritten
		{
			get
			{
				return complete_request_written;
			}
		}

		internal bool SendChunked
		{
			set
			{
				sendChunked = value;
			}
		}

		internal byte[] ReadBuffer
		{
			set
			{
				readBuffer = value;
			}
		}

		internal int ReadBufferOffset
		{
			set
			{
				readBufferOffset = value;
			}
		}

		internal int ReadBufferSize
		{
			set
			{
				readBufferSize = value;
			}
		}

		internal byte[] WriteBuffer
		{
			get
			{
				return writeBuffer.GetBuffer();
			}
		}

		internal int WriteBufferLength
		{
			get
			{
				return (int)((writeBuffer == null) ? (-1) : writeBuffer.Length);
			}
		}

		internal bool RequestWritten
		{
			get
			{
				return requestWritten;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanRead
		{
			get
			{
				return !disposed && isRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return !disposed && !isRead;
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

		public WebConnectionStream(System.Net.WebConnection cnc)
		{
			isRead = true;
			pending = new ManualResetEvent(true);
			request = cnc.Data.request;
			read_timeout = request.ReadWriteTimeout;
			write_timeout = read_timeout;
			this.cnc = cnc;
			string text = cnc.Data.Headers["Transfer-Encoding"];
			bool flag = text != null && text.ToLower().IndexOf("chunked") != -1;
			string text2 = cnc.Data.Headers["Content-Length"];
			if (!flag && text2 != null && text2 != string.Empty)
			{
				try
				{
					contentLength = int.Parse(text2);
					if (contentLength == 0 && !IsNtlmAuth())
					{
						ReadAll();
					}
					return;
				}
				catch
				{
					contentLength = int.MaxValue;
					return;
				}
			}
			contentLength = int.MaxValue;
		}

		public WebConnectionStream(System.Net.WebConnection cnc, HttpWebRequest request)
		{
			read_timeout = request.ReadWriteTimeout;
			write_timeout = read_timeout;
			isRead = false;
			this.cnc = cnc;
			this.request = request;
			allowBuffering = request.InternalAllowBuffering;
			sendChunked = request.SendChunked;
			if (sendChunked)
			{
				pending = new ManualResetEvent(true);
			}
			else if (allowBuffering)
			{
				writeBuffer = new MemoryStream();
			}
		}

		private bool IsNtlmAuth()
		{
			string name = ((request.Proxy == null || request.Proxy.IsBypassed(request.Address)) ? "WWW-Authenticate" : "Proxy-Authenticate");
			string text = cnc.Data.Headers[name];
			return text != null && text.IndexOf("NTLM") != -1;
		}

		internal void CheckResponseInBuffer()
		{
			if (contentLength > 0 && readBufferSize - readBufferOffset >= contentLength && !IsNtlmAuth())
			{
				ReadAll();
			}
		}

		internal void ForceCompletion()
		{
			if (!nextReadCalled)
			{
				if (contentLength == int.MaxValue)
				{
					contentLength = 0;
				}
				nextReadCalled = true;
				cnc.NextRead();
			}
		}

		internal void CheckComplete()
		{
			if (!nextReadCalled && readBufferSize - readBufferOffset == contentLength)
			{
				nextReadCalled = true;
				cnc.NextRead();
			}
		}

		internal void ReadAll()
		{
			if (!isRead || read_eof || totalRead >= contentLength || nextReadCalled)
			{
				if (isRead && !nextReadCalled)
				{
					nextReadCalled = true;
					cnc.NextRead();
				}
				return;
			}
			pending.WaitOne();
			lock (locker)
			{
				if (totalRead >= contentLength)
				{
					return;
				}
				byte[] array = null;
				int num = readBufferSize - readBufferOffset;
				int num2;
				if (contentLength == int.MaxValue)
				{
					MemoryStream memoryStream = new MemoryStream();
					byte[] array2 = null;
					if (readBuffer != null && num > 0)
					{
						memoryStream.Write(readBuffer, readBufferOffset, num);
						if (readBufferSize >= 8192)
						{
							array2 = readBuffer;
						}
					}
					if (array2 == null)
					{
						array2 = new byte[8192];
					}
					int count;
					while ((count = cnc.Read(request, array2, 0, array2.Length)) != 0)
					{
						memoryStream.Write(array2, 0, count);
					}
					array = memoryStream.GetBuffer();
					num2 = (contentLength = (int)memoryStream.Length);
				}
				else
				{
					num2 = contentLength - totalRead;
					array = new byte[num2];
					if (readBuffer != null && num > 0)
					{
						if (num > num2)
						{
							num = num2;
						}
						Buffer.BlockCopy(readBuffer, readBufferOffset, array, 0, num);
					}
					int num3 = num2 - num;
					int num4 = -1;
					while (num3 > 0 && num4 != 0)
					{
						num4 = cnc.Read(request, array, num, num3);
						num3 -= num4;
						num += num4;
					}
				}
				readBuffer = array;
				readBufferOffset = 0;
				readBufferSize = num2;
				totalRead = 0;
				nextReadCalled = true;
			}
			cnc.NextRead();
		}

		private void WriteCallbackWrapper(IAsyncResult r)
		{
			System.Net.WebAsyncResult webAsyncResult = r as System.Net.WebAsyncResult;
			if (webAsyncResult == null || !webAsyncResult.AsyncWriteAll)
			{
				if (r.AsyncState != null)
				{
					webAsyncResult = (System.Net.WebAsyncResult)r.AsyncState;
					webAsyncResult.InnerAsyncResult = r;
					webAsyncResult.DoCallback();
				}
				else
				{
					EndWrite(r);
				}
			}
		}

		private void ReadCallbackWrapper(IAsyncResult r)
		{
			if (r.AsyncState != null)
			{
				System.Net.WebAsyncResult webAsyncResult = (System.Net.WebAsyncResult)r.AsyncState;
				webAsyncResult.InnerAsyncResult = r;
				webAsyncResult.DoCallback();
			}
			else
			{
				EndRead(r);
			}
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			AsyncCallback cb = ReadCallbackWrapper;
			System.Net.WebAsyncResult webAsyncResult = (System.Net.WebAsyncResult)BeginRead(buffer, offset, size, cb, null);
			if (!webAsyncResult.IsCompleted && !webAsyncResult.WaitUntilComplete(ReadTimeout, false))
			{
				nextReadCalled = true;
				cnc.Close(true);
				throw new WebException("The operation has timed out.", WebExceptionStatus.Timeout);
			}
			return EndRead(webAsyncResult);
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			if (!isRead)
			{
				throw new NotSupportedException("this stream does not allow reading");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || num < offset)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || num - offset < size)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			lock (locker)
			{
				pendingReads++;
				pending.Reset();
			}
			System.Net.WebAsyncResult webAsyncResult = new System.Net.WebAsyncResult(cb, state, buffer, offset, size);
			if (totalRead >= contentLength)
			{
				webAsyncResult.SetCompleted(true, -1);
				webAsyncResult.DoCallback();
				return webAsyncResult;
			}
			int num2 = readBufferSize - readBufferOffset;
			if (num2 > 0)
			{
				int num3 = ((num2 <= size) ? num2 : size);
				Buffer.BlockCopy(readBuffer, readBufferOffset, buffer, offset, num3);
				readBufferOffset += num3;
				offset += num3;
				size -= num3;
				totalRead += num3;
				if (size == 0 || totalRead >= contentLength)
				{
					webAsyncResult.SetCompleted(true, num3);
					webAsyncResult.DoCallback();
					return webAsyncResult;
				}
				webAsyncResult.NBytes = num3;
			}
			if (cb != null)
			{
				cb = ReadCallbackWrapper;
			}
			if (contentLength != int.MaxValue && contentLength - totalRead < size)
			{
				size = contentLength - totalRead;
			}
			if (!read_eof)
			{
				webAsyncResult.InnerAsyncResult = cnc.BeginRead(request, buffer, offset, size, cb, webAsyncResult);
			}
			else
			{
				webAsyncResult.SetCompleted(true, webAsyncResult.NBytes);
				webAsyncResult.DoCallback();
			}
			return webAsyncResult;
		}

		public override int EndRead(IAsyncResult r)
		{
			System.Net.WebAsyncResult webAsyncResult = (System.Net.WebAsyncResult)r;
			if (webAsyncResult.EndCalled)
			{
				int nBytes = webAsyncResult.NBytes;
				return (nBytes >= 0) ? nBytes : 0;
			}
			webAsyncResult.EndCalled = true;
			if (!webAsyncResult.IsCompleted)
			{
				int num = -1;
				try
				{
					num = cnc.EndRead(request, webAsyncResult);
				}
				catch (Exception e)
				{
					lock (locker)
					{
						pendingReads--;
						if (pendingReads == 0)
						{
							pending.Set();
						}
					}
					nextReadCalled = true;
					cnc.Close(true);
					webAsyncResult.SetCompleted(false, e);
					webAsyncResult.DoCallback();
					throw;
				}
				if (num < 0)
				{
					num = 0;
					read_eof = true;
				}
				totalRead += num;
				webAsyncResult.SetCompleted(false, num + webAsyncResult.NBytes);
				webAsyncResult.DoCallback();
				if (num == 0)
				{
					contentLength = totalRead;
				}
			}
			lock (locker)
			{
				pendingReads--;
				if (pendingReads == 0)
				{
					pending.Set();
				}
			}
			if (totalRead >= contentLength && !nextReadCalled)
			{
				ReadAll();
			}
			int nBytes2 = webAsyncResult.NBytes;
			return (nBytes2 >= 0) ? nBytes2 : 0;
		}

		private void WriteRequestAsyncCB(IAsyncResult r)
		{
			System.Net.WebAsyncResult webAsyncResult = (System.Net.WebAsyncResult)r.AsyncState;
			try
			{
				cnc.EndWrite2(request, r);
				webAsyncResult.SetCompleted(false, 0);
				if (!initRead)
				{
					initRead = true;
					System.Net.WebConnection.InitRead(cnc);
				}
			}
			catch (Exception ex)
			{
				KillBuffer();
				nextReadCalled = true;
				cnc.Close(true);
				if (ex is SocketException)
				{
					ex = new IOException("Error writing request", ex);
				}
				webAsyncResult.SetCompleted(false, ex);
			}
			complete_request_written = true;
			webAsyncResult.DoCallback();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			if (request.Aborted)
			{
				throw new WebException("The request was canceled.", null, WebExceptionStatus.RequestCanceled);
			}
			if (isRead)
			{
				throw new NotSupportedException("this stream does not allow writing");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = buffer.Length;
			if (offset < 0 || num < offset)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0 || num - offset < size)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (sendChunked)
			{
				lock (locker)
				{
					pendingWrites++;
					pending.Reset();
				}
			}
			System.Net.WebAsyncResult webAsyncResult = new System.Net.WebAsyncResult(cb, state);
			if (!sendChunked)
			{
				CheckWriteOverflow(request.ContentLength, totalWritten, size);
			}
			if (allowBuffering && !sendChunked)
			{
				if (writeBuffer == null)
				{
					writeBuffer = new MemoryStream();
				}
				writeBuffer.Write(buffer, offset, size);
				totalWritten += size;
				if (request.ContentLength > 0 && totalWritten == request.ContentLength)
				{
					try
					{
						webAsyncResult.AsyncWriteAll = true;
						webAsyncResult.InnerAsyncResult = WriteRequestAsync(WriteRequestAsyncCB, webAsyncResult);
						if (webAsyncResult.InnerAsyncResult == null)
						{
							if (!webAsyncResult.IsCompleted)
							{
								webAsyncResult.SetCompleted(true, 0);
							}
							webAsyncResult.DoCallback();
						}
					}
					catch (Exception e)
					{
						webAsyncResult.SetCompleted(true, e);
						webAsyncResult.DoCallback();
					}
				}
				else
				{
					webAsyncResult.SetCompleted(true, 0);
					webAsyncResult.DoCallback();
				}
				return webAsyncResult;
			}
			AsyncCallback cb2 = null;
			if (cb != null)
			{
				cb2 = WriteCallbackWrapper;
			}
			if (sendChunked)
			{
				WriteRequest();
				string s = string.Format("{0:X}\r\n", size);
				byte[] bytes = Encoding.ASCII.GetBytes(s);
				int num2 = 2 + size + bytes.Length;
				byte[] array = new byte[num2];
				Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
				Buffer.BlockCopy(buffer, offset, array, bytes.Length, size);
				Buffer.BlockCopy(crlf, 0, array, bytes.Length + size, crlf.Length);
				buffer = array;
				offset = 0;
				size = num2;
			}
			webAsyncResult.InnerAsyncResult = cnc.BeginWrite(request, buffer, offset, size, cb2, webAsyncResult);
			totalWritten += size;
			return webAsyncResult;
		}

		private void CheckWriteOverflow(long contentLength, long totalWritten, long size)
		{
			if (contentLength != -1)
			{
				long num = contentLength - totalWritten;
				if (size > num)
				{
					KillBuffer();
					nextReadCalled = true;
					cnc.Close(true);
					throw new ProtocolViolationException("The number of bytes to be written is greater than the specified ContentLength.");
				}
			}
		}

		public override void EndWrite(IAsyncResult r)
		{
			if (r == null)
			{
				throw new ArgumentNullException("r");
			}
			System.Net.WebAsyncResult webAsyncResult = r as System.Net.WebAsyncResult;
			if (webAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult");
			}
			if (webAsyncResult.EndCalled)
			{
				return;
			}
			webAsyncResult.EndCalled = true;
			if (webAsyncResult.AsyncWriteAll)
			{
				webAsyncResult.WaitUntilComplete();
				if (webAsyncResult.GotException)
				{
					throw webAsyncResult.Exception;
				}
			}
			else
			{
				if (allowBuffering && !sendChunked)
				{
					return;
				}
				if (webAsyncResult.GotException)
				{
					throw webAsyncResult.Exception;
				}
				try
				{
					cnc.EndWrite2(request, webAsyncResult.InnerAsyncResult);
					webAsyncResult.SetCompleted(false, 0);
					webAsyncResult.DoCallback();
				}
				catch (Exception e)
				{
					webAsyncResult.SetCompleted(false, e);
					webAsyncResult.DoCallback();
					throw;
				}
				finally
				{
					if (sendChunked)
					{
						lock (locker)
						{
							pendingWrites--;
							if (pendingWrites == 0)
							{
								pending.Set();
							}
						}
					}
				}
			}
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			AsyncCallback cb = WriteCallbackWrapper;
			System.Net.WebAsyncResult webAsyncResult = (System.Net.WebAsyncResult)BeginWrite(buffer, offset, size, cb, null);
			if (!webAsyncResult.IsCompleted && !webAsyncResult.WaitUntilComplete(WriteTimeout, false))
			{
				KillBuffer();
				nextReadCalled = true;
				cnc.Close(true);
				throw new IOException("Write timed out.");
			}
			EndWrite(webAsyncResult);
		}

		public override void Flush()
		{
		}

		internal void SetHeaders(byte[] buffer)
		{
			if (headersSent)
			{
				return;
			}
			headers = buffer;
			long num = request.ContentLength;
			string method = request.Method;
			int num2;
			switch (method)
			{
			default:
				num2 = ((method == "DELETE") ? 1 : 0);
				break;
			case "GET":
			case "CONNECT":
			case "HEAD":
			case "TRACE":
				num2 = 1;
				break;
			}
			bool flag = (byte)num2 != 0;
			if (sendChunked || num > -1 || flag)
			{
				WriteHeaders();
				if (!initRead)
				{
					initRead = true;
					System.Net.WebConnection.InitRead(cnc);
				}
				if (!sendChunked && num == 0L)
				{
					requestWritten = true;
				}
			}
		}

		private IAsyncResult WriteRequestAsync(AsyncCallback cb, object state)
		{
			requestWritten = true;
			byte[] buffer = writeBuffer.GetBuffer();
			int num = (int)writeBuffer.Length;
			object result;
			if (num > 0)
			{
				IAsyncResult asyncResult = cnc.BeginWrite(request, buffer, 0, num, cb, state);
				result = asyncResult;
			}
			else
			{
				result = null;
			}
			return (IAsyncResult)result;
		}

		private void WriteHeaders()
		{
			if (!headersSent)
			{
				headersSent = true;
				string err_msg = null;
				if (!cnc.Write(request, headers, 0, headers.Length, ref err_msg))
				{
					throw new WebException("Error writing request: " + err_msg, null, WebExceptionStatus.SendFailure, null);
				}
			}
		}

		internal void WriteRequest()
		{
			if (requestWritten)
			{
				return;
			}
			requestWritten = true;
			if (sendChunked || !allowBuffering || writeBuffer == null)
			{
				return;
			}
			byte[] buffer = writeBuffer.GetBuffer();
			int num = (int)writeBuffer.Length;
			if (request.ContentLength != -1 && request.ContentLength < num)
			{
				nextReadCalled = true;
				cnc.Close(true);
				throw new WebException("Specified Content-Length is less than the number of bytes to write", null, WebExceptionStatus.ServerProtocolViolation, null);
			}
			if (!headersSent)
			{
				string method = request.Method;
				switch (method)
				{
				default:
					if (!(method == "DELETE"))
					{
						request.InternalContentLength = num;
					}
					break;
				case "GET":
				case "CONNECT":
				case "HEAD":
				case "TRACE":
					break;
				}
				request.SendRequestHeaders(true);
			}
			WriteHeaders();
			if (cnc.Data.StatusCode == 0 || cnc.Data.StatusCode == 100)
			{
				IAsyncResult result = null;
				if (num > 0)
				{
					result = cnc.BeginWrite(request, buffer, 0, num, null, null);
				}
				if (!initRead)
				{
					initRead = true;
					System.Net.WebConnection.InitRead(cnc);
				}
				if (num > 0)
				{
					complete_request_written = cnc.EndWrite(request, result);
				}
				else
				{
					complete_request_written = true;
				}
			}
		}

		internal void InternalClose()
		{
			disposed = true;
		}

		public override void Close()
		{
			if (sendChunked)
			{
				if (!disposed)
				{
					disposed = true;
					pending.WaitOne();
					byte[] bytes = Encoding.ASCII.GetBytes("0\r\n\r\n");
					string err_msg = null;
					cnc.Write(request, bytes, 0, bytes.Length, ref err_msg);
				}
			}
			else if (isRead)
			{
				if (!nextReadCalled)
				{
					CheckComplete();
					if (!nextReadCalled)
					{
						nextReadCalled = true;
						cnc.Close(true);
					}
				}
			}
			else if (!allowBuffering)
			{
				complete_request_written = true;
				if (!initRead)
				{
					initRead = true;
					System.Net.WebConnection.InitRead(cnc);
				}
			}
			else if (!disposed && !requestWritten)
			{
				long num = request.ContentLength;
				if (!sendChunked && num != -1 && totalWritten != num)
				{
					IOException innerException = new IOException("Cannot close the stream until all bytes are written");
					nextReadCalled = true;
					cnc.Close(true);
					throw new WebException("Request was cancelled.", innerException, WebExceptionStatus.RequestCanceled);
				}
				WriteRequest();
				disposed = true;
			}
		}

		internal void KillBuffer()
		{
			writeBuffer = null;
		}

		public override long Seek(long a, SeekOrigin b)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long a)
		{
			throw new NotSupportedException();
		}
	}
}
