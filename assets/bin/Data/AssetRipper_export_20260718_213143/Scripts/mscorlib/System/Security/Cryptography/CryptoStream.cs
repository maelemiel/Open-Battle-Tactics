using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public class CryptoStream : Stream
	{
		private Stream _stream;

		private ICryptoTransform _transform;

		private CryptoStreamMode _mode;

		private byte[] _currentBlock;

		private bool _disposed;

		private bool _flushedFinalBlock;

		private int _partialCount;

		private bool _endOfStream;

		private byte[] _waitingBlock;

		private int _waitingCount;

		private byte[] _transformedBlock;

		private int _transformedPos;

		private int _transformedCount;

		private byte[] _workingBlock;

		private int _workingCount;

		public override bool CanRead
		{
			get
			{
				return _mode == CryptoStreamMode.Read;
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
				return _mode == CryptoStreamMode.Write;
			}
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException("Length");
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException("Position");
			}
			set
			{
				throw new NotSupportedException("Position");
			}
		}

		public CryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
		{
			if (mode == CryptoStreamMode.Read && !stream.CanRead)
			{
				throw new ArgumentException(Locale.GetText("Can't read on stream"));
			}
			if (mode == CryptoStreamMode.Write && !stream.CanWrite)
			{
				throw new ArgumentException(Locale.GetText("Can't write on stream"));
			}
			_stream = stream;
			_transform = transform;
			_mode = mode;
			_disposed = false;
			if (transform != null)
			{
				switch (mode)
				{
				case CryptoStreamMode.Read:
					_currentBlock = new byte[transform.InputBlockSize];
					_workingBlock = new byte[transform.InputBlockSize];
					break;
				case CryptoStreamMode.Write:
					_currentBlock = new byte[transform.OutputBlockSize];
					_workingBlock = new byte[transform.OutputBlockSize];
					break;
				}
			}
		}

		~CryptoStream()
		{
			Dispose(false);
		}

		public void Clear()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override void Close()
		{
			if (!_flushedFinalBlock && _mode == CryptoStreamMode.Write)
			{
				FlushFinalBlock();
			}
			if (_stream != null)
			{
				_stream.Close();
			}
		}

		public override int Read([In][Out] byte[] buffer, int offset, int count)
		{
			if (_mode != CryptoStreamMode.Read)
			{
				throw new NotSupportedException(Locale.GetText("not in Read mode"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Locale.GetText("negative"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Locale.GetText("negative"));
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("(offset+count)", Locale.GetText("buffer overflow"));
			}
			if (_workingBlock == null)
			{
				return 0;
			}
			int num = 0;
			if (count == 0 || (_transformedPos == _transformedCount && _endOfStream))
			{
				return num;
			}
			if (_waitingBlock == null)
			{
				_transformedBlock = new byte[_transform.OutputBlockSize << 2];
				_transformedPos = 0;
				_transformedCount = 0;
				_waitingBlock = new byte[_transform.InputBlockSize];
				_waitingCount = _stream.Read(_waitingBlock, 0, _waitingBlock.Length);
			}
			while (count > 0)
			{
				int num2 = _transformedCount - _transformedPos;
				if (num2 < _transform.InputBlockSize)
				{
					int num3 = 0;
					_workingCount = _stream.Read(_workingBlock, 0, _transform.InputBlockSize);
					_endOfStream = _workingCount < _transform.InputBlockSize;
					if (!_endOfStream)
					{
						num3 = _transform.TransformBlock(_waitingBlock, 0, _waitingBlock.Length, _transformedBlock, _transformedCount);
						Buffer.BlockCopy(_workingBlock, 0, _waitingBlock, 0, _workingCount);
						_waitingCount = _workingCount;
					}
					else
					{
						if (_workingCount > 0)
						{
							num3 = _transform.TransformBlock(_waitingBlock, 0, _waitingBlock.Length, _transformedBlock, _transformedCount);
							Buffer.BlockCopy(_workingBlock, 0, _waitingBlock, 0, _workingCount);
							_waitingCount = _workingCount;
							num2 += num3;
							_transformedCount += num3;
						}
						if (!_flushedFinalBlock)
						{
							byte[] array = _transform.TransformFinalBlock(_waitingBlock, 0, _waitingCount);
							num3 = array.Length;
							Buffer.BlockCopy(array, 0, _transformedBlock, _transformedCount, array.Length);
							Array.Clear(array, 0, array.Length);
							_flushedFinalBlock = true;
						}
					}
					num2 += num3;
					_transformedCount += num3;
				}
				if (_transformedPos > _transform.OutputBlockSize)
				{
					Buffer.BlockCopy(_transformedBlock, _transformedPos, _transformedBlock, 0, num2);
					_transformedCount -= _transformedPos;
					_transformedPos = 0;
				}
				num2 = ((count >= num2) ? num2 : count);
				if (num2 > 0)
				{
					Buffer.BlockCopy(_transformedBlock, _transformedPos, buffer, offset, num2);
					_transformedPos += num2;
					num += num2;
					offset += num2;
					count -= num2;
				}
				if ((num2 != _transform.InputBlockSize && _waitingCount != _transform.InputBlockSize) || _endOfStream)
				{
					count = 0;
				}
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_mode != CryptoStreamMode.Write)
			{
				throw new NotSupportedException(Locale.GetText("not in Write mode"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Locale.GetText("negative"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Locale.GetText("negative"));
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("(offset+count)", Locale.GetText("buffer overflow"));
			}
			if (_stream == null)
			{
				throw new ArgumentNullException("inner stream was diposed");
			}
			int num = count;
			if (_partialCount > 0 && _partialCount != _transform.InputBlockSize)
			{
				int num2 = _transform.InputBlockSize - _partialCount;
				num2 = ((count >= num2) ? num2 : count);
				Buffer.BlockCopy(buffer, offset, _workingBlock, _partialCount, num2);
				_partialCount += num2;
				offset += num2;
				count -= num2;
			}
			int num3 = offset;
			while (count > 0)
			{
				if (_partialCount == _transform.InputBlockSize)
				{
					int count2 = _transform.TransformBlock(_workingBlock, 0, _partialCount, _currentBlock, 0);
					_stream.Write(_currentBlock, 0, count2);
					_partialCount = 0;
				}
				if (_transform.CanTransformMultipleBlocks)
				{
					int num4 = count & ~(_transform.InputBlockSize - 1);
					int num5 = count & (_transform.InputBlockSize - 1);
					int num6 = (1 + num4 / _transform.InputBlockSize) * _transform.OutputBlockSize;
					if (_workingBlock.Length < num6)
					{
						Array.Clear(_workingBlock, 0, _workingBlock.Length);
						_workingBlock = new byte[num6];
					}
					if (num4 > 0)
					{
						int count3 = _transform.TransformBlock(buffer, offset, num4, _workingBlock, 0);
						_stream.Write(_workingBlock, 0, count3);
					}
					if (num5 > 0)
					{
						Buffer.BlockCopy(buffer, num - num5, _workingBlock, 0, num5);
					}
					_partialCount = num5;
					count = 0;
				}
				else
				{
					int num7 = Math.Min(_transform.InputBlockSize - _partialCount, count);
					Buffer.BlockCopy(buffer, num3, _workingBlock, _partialCount, num7);
					num3 += num7;
					_partialCount += num7;
					count -= num7;
				}
			}
		}

		public override void Flush()
		{
			if (_stream != null)
			{
				_stream.Flush();
			}
		}

		public void FlushFinalBlock()
		{
			if (_flushedFinalBlock)
			{
				throw new NotSupportedException(Locale.GetText("This method cannot be called twice."));
			}
			if (_disposed)
			{
				throw new NotSupportedException(Locale.GetText("CryptoStream was disposed."));
			}
			if (_mode != CryptoStreamMode.Write)
			{
				return;
			}
			_flushedFinalBlock = true;
			byte[] array = _transform.TransformFinalBlock(_workingBlock, 0, _partialCount);
			if (_stream != null)
			{
				_stream.Write(array, 0, array.Length);
				if (_stream is CryptoStream)
				{
					(_stream as CryptoStream).FlushFinalBlock();
				}
				_stream.Flush();
			}
			Array.Clear(array, 0, array.Length);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("SetLength");
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				_disposed = true;
				if (_workingBlock != null)
				{
					Array.Clear(_workingBlock, 0, _workingBlock.Length);
				}
				if (_currentBlock != null)
				{
					Array.Clear(_currentBlock, 0, _currentBlock.Length);
				}
				if (disposing)
				{
					_stream = null;
					_workingBlock = null;
					_currentBlock = null;
				}
			}
		}
	}
}
