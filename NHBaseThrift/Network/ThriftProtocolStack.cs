using System;
using System.Collections.Generic;
using System.Reflection;
using Gridsum.NHBaseThrift.Messages;
using KJFramework.Net.ProtocolStacks;

namespace Gridsum.NHBaseThrift.Network
{
    /// <summary>
    ///    基于Thrift协议的协议栈
    /// </summary>
    public class ThriftProtocolStack : IProtocolStack<ThriftMessage>
    {
        #region Constructor.

        /// <summary>
        ///    基于Thrift协议的协议栈
        /// </summary>
        public ThriftProtocolStack()
        {
            AutoInitialize();
        }

        #endregion

        #region Members.

        private static readonly Dictionary<string, Type> _reqMessageTypes = new Dictionary<string, Type>(); 
        private static readonly Dictionary<string, Type> _rspMessageTypes = new Dictionary<string, Type>(); 

        #endregion

        #region Methods.


        /// <summary>
        ///    初始化
        /// </summary>
        /// <returns>
        ///    返回初始化的结果
        /// </returns>
        /// <exception cref="T:KJFramework.Net.Exception.InitializeFailedException">初始化失败</exception>
        public bool Initialize()
        {
            _reqMessageTypes.Add("createTable", typeof(CreateTableRequestMessage));
            _rspMessageTypes.Add("createTable", typeof(CreateTableResponseMessage));
            return true;
        }

        /// <summary>
        ///    根据一些特定的条件获取一个消息的类型
        /// </summary>
        /// <param name="command">命令信息</param>
        /// <param name="isReq">是否期望返回关于REQ消息的类型</param>
        /// <returns>返回消息的类型信息</returns>
        public Type GetMessageType(string command, bool isReq)
        {
            Type type;
            Dictionary<string, Type> dic = (isReq ? _reqMessageTypes : _rspMessageTypes);
            return (dic.TryGetValue(command, out type) ? type : null);
        }

        /// <summary>
        ///    解析元数据
        /// </summary>
        /// <param name="data">元数据</param>
        /// <returns>
        ///   返回能否解析的一个标示
        /// </returns>
        public List<ThriftMessage> Parse(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///    解析元数据
        /// </summary>
        /// <param name="data">总BUFF长度</param><param name="offset">可用偏移量</param><param name="count">可用长度</param>
        /// <returns>
        ///    返回能否解析的一个标示
        /// </returns>
        public List<ThriftMessage> Parse(byte[] data, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///    将一个消息转换为2进制形式
        /// </summary>
        /// <param name="message">需要转换的消息</param>
        /// <returns>
        ///    返回转换后的2进制
        /// </returns>
        public byte[] ConvertToBytes(ThriftMessage message)
        {
            message.Bind();
            return message.Body;
        }

        /// <summary>
        ///    将一个消息转换为多个分包二进制数据
        /// </summary>
        /// <param name="message">消息</param><param name="maxSize">封包片最大容量</param>
        /// <returns>
        /// 返回转换后的2进制集合
        /// </returns>
        public List<byte[]> ConvertMultiMessage(ThriftMessage message, int maxSize)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     自动化初始工作
        /// </summary>
        /// <returns>返回协议栈实例</returns>
        public void AutoInitialize()
        {
            Assembly assembly = GetType().Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(ThriftMessage)) && !type.IsAbstract)
                {
                    //create instance for msg, need default ctor.
                    ThriftMessage baseMsg = (ThriftMessage)Activator.CreateInstance(type);
                    if (type.Name.Contains("RequestMessage")) _reqMessageTypes.Add(baseMsg.Identity.Command, type);
                    else if (type.Name.Contains("ResponseMessage")) _rspMessageTypes.Add(baseMsg.Identity.Command, type);
                }
            }
        }

        #endregion
    }
}