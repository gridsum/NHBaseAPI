using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace NHBaseThrift
{
    /// <summary>
    ///     HOST名称映射管理器
    /// </summary>
    internal class HostMappingManager : IHostMappingManager
    {
        #region Constructor.

        /// <summary>
        ///     HOST名称映射管理器
        /// </summary>
        public HostMappingManager()
        {
            InitializeHostMappingInformation();
        }

        #endregion

        #region Members.

        private readonly Dictionary<string, string> _hostAddresses = new Dictionary<string, string>();

        #endregion

        #region Methods.

        /// <summary>
        ///     根据一个主机名获取IP映射关系
        /// </summary>
        /// <param name="hostName">主机名</param>
        /// <returns>返回对应的IP地址</returns>
        public string GetIPAddressByHostName(string hostName)
        {
            string address;
            if (_hostAddresses.TryGetValue(hostName, out address)) return address;
            IPAddress[] addresses = Dns.GetHostAddresses(hostName);
            if (addresses != null) return addresses[0].MapToIPv4().ToString();
            //actually this return value will never be uses because of an exception occured before.
            return null;
        }

        private void InitializeHostMappingInformation()
        {
            string workPath = Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf('\\') + 1);
            //FORMAT: HOST IP
            string[] files = Directory.GetFiles(workPath, "host.mapping");
            if (files.Length == 0) return;
            if (files.Length > 1) throw new Exception("#Confused by multiple \"host.mapping\" files we've founded.");
            string[] lines = File.ReadAllLines(files[0]);
            if (lines.Length == 0) return;
            foreach (string line in lines)
            {
                string[] content = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                _hostAddresses[content[0]] = content[1];
            }
        }

        #endregion
    }
}