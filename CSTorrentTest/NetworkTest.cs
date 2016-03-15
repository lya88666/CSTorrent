using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSTorrent;
using System.Net;

namespace CSTorrentTest
{
    /// <summary>
    /// Summary description for Network
    /// </summary>
    [TestClass]
    public class NetworkTest
    {
        private Int32 mTransactionId;
        private Int64 mConnectionId;
        private string mTrackerHost;
        private int mTrackerPort;

        public NetworkTest()
        {
            //
            mTransactionId = 12345678;
            mConnectionId = 0xff << 63;
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
        }

        [TestMethod]
        public void TestConnectionId()
        {
            ConnectRequest connectRequest = new ConnectRequest();
            connectRequest.connection_id = 0x41727101980;
            connectRequest.action = 0;
            connectRequest.transaction_id = this.mTransactionId;

            byte[] response = null;
            bool ret = NetworkHelper.SendAndReceive(mTrackerHost, mTrackerPort, connectRequest.GetBytes(), ref response);

            ConnectResponse connectResponse = null;

            if (ret)
            {
                connectResponse = ConnectResponse.Parse(response, this.mTransactionId);
            }

            Assert.IsTrue(connectResponse != null);
        }

        [TestMethod]
        public void TestAnnounce()
        {

            //mTrackerHost = "tracker.openbittorrent.com";
            //mTrackerPort = 80;
            //mTrackerHost = "tracker.novalayer.org";
            //mTrackerPort = 6969;
            mTrackerHost = "tracker.istole.it";
            mTrackerPort = 80;

            //read torrent info
            byte[] bytes = System.IO.File.ReadAllBytes("C:\\1.torrent");
            MetaInfo metaInfo = TorrentParser.ParseTorrent(bytes);

            Assert.IsTrue(metaInfo != null);

            //get connection id
            TestConnectionId();

            Assert.IsTrue(mConnectionId != -1);

            //send announce request
            AnnounceRequest announceRequest = new AnnounceRequest();
            announceRequest.action = (int)ActionType.ANNOUNCE;
            announceRequest.transaction_id = this.mTransactionId;
            announceRequest.connection_id = mConnectionId;
            announceRequest.downloaded = 0;
            announceRequest.uploaded = 0;
            announceRequest.left = metaInfo.info.totalLength;
            announceRequest.info_hash = new byte[20];
            announceRequest.ipaddr = 0;
            announceRequest.key = 0;
            announceRequest.num_want = -1;
            announceRequest.port = 6881;
            announceRequest.evnt = (int)EventType.STARTED;
            announceRequest.peer_id = new byte[20];

            byte[] sha1 = metaInfo.infoHash;
            Buffer.BlockCopy(sha1, 0, announceRequest.info_hash, 0, 20);
            Buffer.BlockCopy(metaInfo.info.sha1List[0].value, 0, announceRequest.peer_id, 0, 20);

            //get response
            byte[] response = null;
            bool ret = NetworkHelper.SendAndReceive(mTrackerHost, mTrackerPort, announceRequest.GetBytes(), ref response);

            Assert.IsTrue(ret);

            //parse Response
            AnnounceResponse announceResp = AnnounceResponse.Parse(response, this.mTransactionId);

            Assert.IsTrue(announceResp != null);

        }
    }
}
