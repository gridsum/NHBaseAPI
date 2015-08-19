using System;
using System.Text;
using Gridsum.NHBaseThrift.Exceptions;
using Gridsum.NHBaseThrift.Objects;
using KJFramework.Net.Channels.Events;

namespace Gridsum.NHBaseThrift.Network
{
    /// <summary>
    ///    网络数据容器接口
    /// </summary>
    public interface INetworkDataContainer : IDisposable
    {
        #region Methods.

        /// <summary>
        ///    追加一段刚刚接收到的网络可用数据到此容器中
        /// </summary>
        /// <param name="args">接收到的网络数据</param>
        void AppendNetworkData(SegmentReceiveEventArgs args);
        /// <summary>
        ///    尝试检查当前剩余的可用数据长度是否满足指定数据长度的需求
        /// </summary>
        /// <param name="length">需要检查的数据长度</param>
        /// <returns>如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        bool CheckEnoughSize(int length);
        /// <summary>
        ///    重置内部数据的起始可用偏移
        /// </summary>
        void ResetOffset();
        /// <summary>
        ///    尝试读取一个字节的数据
        /// </summary>
        /// <param name="data">如果返回True, 则这个字段携带了读取成功的数据</param>
        /// <returns>返回一个值，该值标示了当前是否读取成功。如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        /// <exception cref="IncorrectCalculationException">内部错误，应该终止业务程序</exception>
        bool TryReadByte(out byte data);
        /// <summary>
        ///    尝试读取一个Int16类型的数据
        /// </summary>
        /// <param name="data">如果返回True, 则这个字段携带了读取成功的数据</param>
        /// <returns>返回一个值，该值标示了当前是否读取成功。如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        bool TryReadInt16(out short data);
        /// <summary>
        ///    尝试读取一个Int32类型的数据
        /// </summary>
        /// <param name="data">如果返回True, 则这个字段携带了读取成功的数据</param>
        /// <returns>返回一个值，该值标示了当前是否读取成功。如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        bool TryReadInt32(out int data);
        /// <summary>
        ///    尝试读取一个Int64类型的数据
        /// </summary>
        /// <param name="data">如果返回True, 则这个字段携带了读取成功的数据</param>
        /// <returns>返回一个值，该值标示了当前是否读取成功。如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        bool TryReadInt64(out long data);
        /// <summary>
        ///    尝试读取一段字节数组类型的数据
        /// </summary>
        /// <param name="length">需要读取的数据长度</param>
        /// <param name="data">如果返回True, 则这个字段携带了读取成功的数据</param>
        /// <returns>返回一个值，该值标示了当前是否读取成功。如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        bool TryReadBinaryData(int length, out byte[] data);
        /// <summary>
        ///    尝试读取一段字节数组类型的数据
        /// </summary>
        /// <param name="encoding">解析字符串所使用的编码集</param>
        /// <param name="length">需要读取的数据长度</param>
        /// <param name="data">如果返回True, 则这个字段携带了读取成功的数据</param>
        /// <returns>返回一个值，该值标示了当前是否读取成功。如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        /// <exception cref="ArgumentNullException">参数不能为空</exception>
        /// <exception cref="IncorrectCalculationException">内部错误，应该终止业务程序</exception>
        bool TryReadString(Encoding encoding, int length, out string data);
        /// <summary>
        ///    尝试读取一个完整的Thrift消息头类型的数据
        /// </summary>
        /// <returns>返回一个值，该值标示了当前是否读取成功。如果返回False, 则证明内部还没有足够的可用数据以供读取</returns>
        bool TryReadMessageIdentity(out MessageIdentity identity);
        /// <summary>
        ///    变更内部可用数据的偏移
        /// </summary>
        void UpdateOffset();
        /// <summary>
        ///     DUMP出当前NetworkContainer内部所包含的所有数据
        /// </summary>
        /// <returns>返回包含的数据</returns>
        string Dump();

        #endregion
    }
}