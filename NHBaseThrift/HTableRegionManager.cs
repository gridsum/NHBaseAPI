using System;
using System.Collections.Generic;
using System.Net;
using Gridsum.NHBaseThrift.Comparator;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Objects;

namespace Gridsum.NHBaseThrift
{
    /// <summary>
    ///     HBase表区域分布管理器
    /// </summary>
    internal class HTableRegionManager : IHTableRegionManager
    {
        #region Constructor.

        /// <summary>
        ///     HBase表区域分布管理器
        /// </summary>
        /// <param name="regions">表原始的Region分布范围</param>
        /// <param name="hostMappingManager">主机名/IP映射管理器</param>
        /// <exception cref="IPMappingFailException">主机名找不到IP</exception>
		public HTableRegionManager(Region[] regions, IHostMappingManager hostMappingManager)
        {
            _hostMappingManager = hostMappingManager;
            for (int i = 0; i < regions.Length; i++)
            {
                Region region = regions[i];
                RegionInfo regionInfo = new RegionInfo
                {
                    StartKey = region.StartKey,
                    EndKey = region.EndKey,
                    Id = region.Id,
                    Name = region.Name,
                    Version = region.Version,
                    Comparator = _comparator
                };
                string ip = _hostMappingManager.GetIPAddressByHostName(region.ServerName);
                if (string.IsNullOrEmpty(ip)) throw new IPMappingFailException("#Couldn't found any IP address can match this host: " + region.ServerName);
                regionInfo.Address = new IPEndPoint(IPAddress.Parse(ip), region.Port);
                _regions.Add(regionInfo);
            }
        }

        #endregion

        #region Members.

        private List<RegionInfo> _regions = new List<RegionInfo>(); 
        private readonly IHostMappingManager _hostMappingManager;
        private static readonly IByteArrayComparator _comparator = new ByteArrayComparator();

        #endregion

        #region Methods.

        /// <summary>
        ///     根据一个指定的行键来确定远程Region服务器地址
        /// </summary>
        /// <param name="rowKey">行键</param>
        /// <returns>返回对应的Region服务器地址</returns>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="RegionNotFoundException">找不到对应的RegionServer</exception>
        public IPEndPoint GetRegionByRowKey(byte[] rowKey)
        {
            if (_regions.Count == 0) throw new RegionNotFoundException("#We couldn't match any remote region server by given Row-Key: " + rowKey);
            //byte[] data = Encoding.UTF8.GetBytes(rowKey);
			if (_regions.Count == 1 && _regions[0].IsMatch(rowKey)) return _regions[0].Address;
            foreach (RegionInfo region in _regions)
				if (region.IsMatch(rowKey)) return region.Address;
            throw new RegionNotFoundException("#We couldn't match any remote region server by given Row-Key: " + rowKey);
        }

        #endregion
    }
}