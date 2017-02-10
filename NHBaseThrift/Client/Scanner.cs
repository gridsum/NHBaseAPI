using System;
using System.Net;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Objects;
using KJFramework.Tracing;

namespace Gridsum.NHBaseThrift.Client
{
	/// <summary>
	///		Scanner查询器
	/// </summary>
	public class Scanner : IDisposable
	{
		#region Members.
		
		private bool isDisposed;
		private readonly int BatchSize;
		private readonly int _scannerId;
		private readonly IPEndPoint _iep;
		private readonly HBaseClient _client;
		private static readonly ITracing _tracing = TracingManager.GetTracing(typeof(Scanner));

		#endregion

		#region Constructor

		///  <summary>
		/// 		Scanner查询器
		///  </summary>
		///  <param name="scannerId">scannerId</param>
		/// <param name="batchSize">单次从数据库获取的行数</param>
		/// <param name="client">Hbase客户端</param>
		///  <param name="iep">网络端点</param>
		public Scanner(int scannerId, HBaseClient client, IPEndPoint iep,int batchSize = 1)
		{
			_scannerId = scannerId;
			_client = client;
			_iep = iep;
			BatchSize = batchSize;
		}

		#endregion

		#region Methods.

		/// <summary>
		///		获取下一行数据。当返回null时为止
		/// </summary>
		/// <exception cref="IOErrorException">IO错误</exception>
		/// <exception cref="CommunicationTimeoutException">通信超时</exception>
		/// <exception cref="CommunicationFailException">通信失败</exception>
		/// <returns>返回下一行数据</returns>
		public RowInfo GetNext()
		{
			if (isDisposed) return null;
			RowInfo[] infos = _client.GetRowsFromScanner(_scannerId, BatchSize, _iep);
			if (infos == null || infos.Length == 0)
			{
				_client.ScannerClose(_scannerId, _iep);
				isDisposed = true;
				return null;
			}
			return infos[0];
		}

		/// <summary>
		///		回收服务器scanner资源
		/// </summary>
		public void Dispose()
		{
			if (!isDisposed)
			{
				_client.ScannerClose(_scannerId, _iep);
				isDisposed = true;
			}
		}

		#endregion


	}
}
