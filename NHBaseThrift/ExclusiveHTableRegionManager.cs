using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Gridsum.NHBaseThrift
{
    internal class ExclusiveHTableRegionManager : IHTableRegionManager
    {
		#region Members.

		private uint _index = 0U;
		private readonly IPEndPoint[] _endPoints;

		#endregion

        #region Constructor.

        public ExclusiveHTableRegionManager(string ips)
        {
            string[] servers = ips.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            if(string.IsNullOrEmpty(ips) || servers.Length == 0) throw new ArgumentException("#Cannot ignore real RS servers when running on the exclusive mode.");
            int i = 0;
            _endPoints = new IPEndPoint[servers.Length];
            foreach (string server in servers)
            {
                string[] strPair = server.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
                if (strPair.Length != 2) throw new ArgumentException("Illegal RS server address: " + server);
                _endPoints[i++] = new IPEndPoint(IPAddress.Parse(strPair[0]), int.Parse(strPair[1]));
            }
        }

        #endregion

        #region Methods.

        public IPEndPoint GetRegionByRowKey(byte[] rowKey)
        {
            return _endPoints[_index++%_endPoints.Length];
        }

	    public IList<IPEndPoint> GetIpEndPoints()
	    {
		    return _endPoints.ToList();
	    }

        #endregion
    }
}