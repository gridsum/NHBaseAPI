using System;
using System.Reflection;
using System.Text;
using Gridsum.NHBaseThrift.Network;
using KJFramework.Cache.Cores;
using KJFramework.Cache.Objects;
using KJFramework.Net.Channels;
using KJFramework.Net.Channels.Caches;
using KJFramework.Net.Channels.Events;
using NUnit.Framework;

namespace Gridsum.NHBaseThrift.UnitTests
{
    [TestFixture]
    public class NetworkDataContainerUnitTests
    {
        #region Methods.

        [SetUp]
        public void Initialize()
        {
            ChannelConst.Initialize();
            InitializeSegments();
        }

        [Test]
        public void ReadByteTest()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                                              ↓ data(1)
             *      1: □□□□□□□□□□□□□□yyyyyyyyyx□□□□□□□□□□□□□□□
             */
            byte[] array = { 0xFF, 0xFF, 0XFF, 0XFF, 0x01, 0xFF, 0xFF, 0xFF };
            byte destValue = 0x01;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 4, 4)));
            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 4));
            byte value;
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == destValue);
        }

        [Test]
        public void ReadByteAndOtherDataTest()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                                              ↓ data start here
             *      1: □□□□□□□□□□□□□□yyyyyyyyyxxxx□□□□□□□□□□□□□□□
             */
            byte[] array = { 0xFF, 0xFF, 0XFF, 0XFF, 0x01, 0xFF, 0xFF, 0xFF };
            byte destValue = 0x01;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 4, 4)));
            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 4));
            byte value;
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == destValue);
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == 0xFF);
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == 0xFF);
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == 0xFF);
        }


        [Test]
        public void ReadByteOutOfRangeTest()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                                              ↓ data start here
             *                                                    ↓ OUT OF RANGE data here(Not existed)
             *      1: □□□□□□□□□□□□□□yyyyyyyyyxxxx
             */
            byte[] array = { 0xFF, 0xFF, 0XFF, 0XFF, 0x01, 0xFF, 0xFF, 0xFF };
            byte destValue = 0x01;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 4, 4)));
            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 4));
            byte value;
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == destValue);
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == 0xFF);
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == 0xFF);
            Assert.IsTrue(container.TryReadByte(out value));
            Assert.IsTrue(value == 0xFF);
            Assert.IsFalse(container.TryReadByte(out value));
        }

        [Test]
        public void ReadInt16_CrossSegment_Test()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *           ↓ data(1)
             *      1: yx
             *          ↓ data(2)
             *      2: xy
             */
            byte[] array = {0xFF, 0x01, 0x02, 0xFF};
            short destValue = BitConverter.ToInt16(array, 1);
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 1, 1)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 2, 1)));
            

            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 1));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 1));

            short value;
            Assert.IsTrue(container.TryReadInt16(out value));
            Assert.IsTrue(destValue == value);
        }


        [Test]
        public void ReadInt32_CrossSegment_Test()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *              ↓ data(1)
             *      1: yyyx
             *         ↓ others data start here
             *      2: xxxy
             */
            byte[] array = { 0xFF, 0xFF, 0xFF, 0x03, 0x00, 0x00, 0x00, 0xFF };
            const int destValue = 3;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 4)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 4, 4)));


            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 4));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 4));

            int value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadInt32(out value));
            Assert.IsTrue(value == destValue);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
        }

        [Test]
        public void ReadInt32_CrossSegment_Test2()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *           ↓ data(1)
             *      1: yxxx
             *         ↓ last one byte data
             *      2: xyyy
             */
            byte[] array = { 0xFF, 0x03, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF };
            const int destValue = 3;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 4)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 4, 4)));


            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 4));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 4));

            int value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadInt32(out value));
            Assert.IsTrue(value == destValue);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
        }


        [Test]
        public void ReadInt32Test()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *           ↓ data(1)
             *      1: yyyxxxx
             *         ↓ last one byte data
             *      2: yyyyyyy
             */
            byte[] array = { 0xFF, 0xFF, 0xFF, 0x03, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
            const int destValue = 3;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 7)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 7, 7)));


            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 7));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 7));

            int value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadInt32(out value));
            Assert.IsTrue(value == destValue);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFE);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
        }


        [Test]
        public void ReadInt16_Test()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *          ↓↓ data(1 + 2)
             *      1: xx
             *      2: yy
             */
            byte[] array = { 0x01, 0x02, 0xFF, 0xFF };
            short destValue = BitConverter.ToInt16(array, 0);
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 2)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 2, 2)));


            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 2));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 2));

            short value;
            Assert.IsTrue(container.TryReadInt16(out value));
            Assert.IsTrue(destValue == value);
        }

        [Test]
        public void ReadBinaryDataTest()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                   ↓data
             *      1: yyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *      2: yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
             */
            byte[] array = new byte[60];
            array[5] = 0xFF;
            string content = "Hello, Thrift Protocol.";
            byte[] stringData = Encoding.UTF8.GetBytes(content);
            Buffer.BlockCopy(stringData, 0, array, 6, stringData.Length);
            array[5 + stringData.Length+1] = 0xFE;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 30, 30)));

            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 30));

            byte[] value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadBinaryData(stringData.Length, out value));
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Length == stringData.Length);
            Assert.IsTrue(Encoding.UTF8.GetString(value) == content);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFE);
        }

        [Test]
        public void ReadBinaryData_CrossSegment_Test()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                   ↓data
             *      1: yyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      2: xxxxxxxxxxxxxyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
             */
            byte[] array = new byte[60];
            array[5] = 0xFF;
            string content = "Hello, Thrift Protocol. I am going to catch you.";
            byte[] stringData = Encoding.UTF8.GetBytes(content);
            Buffer.BlockCopy(stringData, 0, array, 6, stringData.Length);
            array[5 + stringData.Length + 1] = 0xFE;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 30, 30)));

            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 30));

            byte[] value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadBinaryData(stringData.Length, out value));
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Length == stringData.Length);
            Assert.IsTrue(Encoding.UTF8.GetString(value) == content);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFE);
        }

        [Test]
        public void ReadBinaryData_CrossSegment_Test2()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                   ↓data
             *      1: yyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      2: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      3: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      4: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxyyyyyy
             */
            byte[] array = new byte[120];
            array[5] = 0xFF;
            string content = "Hello, Thrift Protocol. I am going to catch you. Oneday, I'll use this code to communicates with HBase.";
            byte[] stringData = Encoding.UTF8.GetBytes(content);
            Buffer.BlockCopy(stringData, 0, array, 6, stringData.Length);
            array[5 + stringData.Length + 1] = 0xFE;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 30, 30)));
            //data preparation - 3
            IFixedCacheStub<SocketBuffStub> fixedStub3 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub3 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub3).Cache.SetValue(stub3);
            SetMemorySegment(stub3, new MemorySegment(new ArraySegment<byte>(array, 60, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub4 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub4 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub4).Cache.SetValue(stub4);
            SetMemorySegment(stub4, new MemorySegment(new ArraySegment<byte>(array, 90, 30)));

            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub3, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub4, 30));

            byte[] value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadBinaryData(stringData.Length, out value));
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Length == stringData.Length);
            Assert.IsTrue(Encoding.UTF8.GetString(value) == content);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFE);
        }

        [Test]
        public void ReadStringDataTest()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                   ↓data
             *      1: yyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *      2: yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
             */
            byte[] array = new byte[60];
            array[5] = 0xFF;
            string content = "Hello, Thrift Protocol.";
            byte[] stringData = Encoding.UTF8.GetBytes(content);
            Buffer.BlockCopy(stringData, 0, array, 6, stringData.Length);
            array[5 + stringData.Length + 1] = 0xFE;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 30, 30)));

            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 30));

            string value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadString(Encoding.UTF8, stringData.Length, out value));
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Length == stringData.Length);
            Assert.IsTrue(value == content);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFE);
        }


        [Test]
        public void ReadStringData_CrossSegment_Test()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                   ↓data
             *      1: yyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      2: xxxxxxxxxxxxxyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
             */
            byte[] array = new byte[60];
            array[5] = 0xFF;
            string content = "Hello, Thrift Protocol. I am going to catch you.";
            byte[] stringData = Encoding.UTF8.GetBytes(content);
            Buffer.BlockCopy(stringData, 0, array, 6, stringData.Length);
            array[5 + stringData.Length + 1] = 0xFE;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 30, 30)));

            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 30));

            string value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadString(Encoding.UTF8, stringData.Length, out value));
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Length == stringData.Length);
            Assert.IsTrue(value == content);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFE);
        }


        [Test]
        public void ReadStringData_CrossSegment_Test2()
        {
            /*
             *  segments:
             *  x - focused data.
             *  y - other unfocusd data.
             *  □ - un-use byte.
             *                   ↓data
             *      1: yyyyyyxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      2: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      3: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
             *          ↓data
             *      4: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxyyyyyy
             */
            byte[] array = new byte[120];
            array[5] = 0xFF;
            string content = "Hello, Thrift Protocol. I am going to catch you. Oneday, I'll use this code to communicates with HBase.";
            byte[] stringData = Encoding.UTF8.GetBytes(content);
            Buffer.BlockCopy(stringData, 0, array, 6, stringData.Length);
            array[5 + stringData.Length + 1] = 0xFE;
            //data preparation - 1
            IFixedCacheStub<SocketBuffStub> fixedStub = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub).Cache.SetValue(stub);
            SetMemorySegment(stub, new MemorySegment(new ArraySegment<byte>(array, 0, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub2 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub2 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub2).Cache.SetValue(stub2);
            SetMemorySegment(stub2, new MemorySegment(new ArraySegment<byte>(array, 30, 30)));
            //data preparation - 3
            IFixedCacheStub<SocketBuffStub> fixedStub3 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub3 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub3).Cache.SetValue(stub3);
            SetMemorySegment(stub3, new MemorySegment(new ArraySegment<byte>(array, 60, 30)));
            //data preparation - 2
            IFixedCacheStub<SocketBuffStub> fixedStub4 = new CacheStub<SocketBuffStub>();
            SocketBuffStub stub4 = new SocketBuffStub();
            ((ICacheStub<SocketBuffStub>)fixedStub4).Cache.SetValue(stub4);
            SetMemorySegment(stub4, new MemorySegment(new ArraySegment<byte>(array, 90, 30)));

            INetworkDataContainer container = new NetworkDataContainer();
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub2, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub3, 30));
            container.AppendNetworkData(new SocketSegmentReceiveEventArgs(fixedStub4, 30));

            string value;
            byte tmpValue;
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0x00);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFF);
            Assert.IsTrue(container.TryReadString(Encoding.UTF8, stringData.Length, out value));
            Assert.IsNotNull(value);
            Assert.IsTrue(value.Length == stringData.Length);
            Assert.IsTrue(value == content);
            Assert.IsTrue(container.TryReadByte(out tmpValue));
            Assert.IsTrue(tmpValue == 0xFE);
        }

        private void SetMemorySegment(SocketBuffStub stub, MemorySegment segment)
        {
            FieldInfo fieldInfo = stub.GetType().GetField("_segment", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo.SetValue(stub, segment);
        }

        private void InitializeSegments()
        {
            MethodInfo methodInfo = ChannelConst.SegmentContainer.GetType().GetMethod("Initialize", BindingFlags.Instance | BindingFlags.NonPublic);
            methodInfo.Invoke(ChannelConst.SegmentContainer, null);
        }

        #endregion
    }
}