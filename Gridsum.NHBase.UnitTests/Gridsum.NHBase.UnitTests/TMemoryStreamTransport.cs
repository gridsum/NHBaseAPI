using System.IO;
using Thrift.Transport;

namespace Gridsum.NHBase.UnitTests
{
	public class TMemoryStreamTransport : TTransport
	{
		private MemoryStream _stream = new MemoryStream();

		public override bool IsOpen
		{
			get { return true; }
		}

		public override void Open()
		{
		}

		public override void Close()
		{
		}

		public override int Read(byte[] buf, int off, int len)
		{
			return -1;
		}

		public override void Write(byte[] buf, int off, int len)
		{
			_stream.Write(buf, off, len);
		}

		protected override void Dispose(bool disposing)
		{
		}

		public byte[] GetBuffer()
		{
			return _stream.ToArray();
		}
	}
}
