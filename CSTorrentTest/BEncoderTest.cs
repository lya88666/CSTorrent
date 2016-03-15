using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSTorrent;

namespace CSTorrentTest
{
    /// <summary>
    /// Summary description for BEncoderTest
    /// </summary>
    [TestClass]
    public class BEncoderTest
    {
        public BEncoderTest()
        {
            //
            // TODO: Add constructor logic here
            //
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
        public void TestEncode()
        {
            BString foo = new BString("foo");
            BString bar = new BString("bar");
            BString spam = new BString("spam");

            BInteger i42 = new BInteger(42);

            BList list = new BList();

            BDictionary dict = new BDictionary();

            list.Add(spam);
            list.Add(i42);
            dict.Add(foo, i42);
            dict.Add(bar, spam);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            Assert.IsTrue(encoding.GetString(i42.Encode()) == "i42e");
            Assert.IsTrue(encoding.GetString(spam.Encode()) == "4:spam");
            Assert.IsTrue(encoding.GetString(list.Encode()) == "l4:spami42ee");
            Assert.IsTrue(encoding.GetString(dict.Encode()) == "d3:bar4:spam3:fooi42ee");

        }

        [TestMethod]
        public void TestDecode()
        {
            byte[] bytes = System.IO.File.ReadAllBytes("C:\\1.torrent");
            //byte[] bytes = System.IO.File.ReadAllBytes("C:\\announce.txt");

            BObject objlist = BDecoder.Decode(bytes);

            Assert.IsTrue(objlist != null);

            System.Diagnostics.Debug.WriteLine(objlist.ToString(0));
        }

        [TestMethod]
        public void TestTorrentParser()
        {
            byte[] bytes = System.IO.File.ReadAllBytes("C:\\1.torrent");

            MetaInfo metaInfo = TorrentParser.ParseTorrent(bytes);

            Assert.IsTrue(metaInfo != null);
        }

        [TestMethod]
        public void TestParams()
        {
            byte[] bytes = System.IO.File.ReadAllBytes("C:\\1.torrent");

            MetaInfo metaInfo = TorrentParser.ParseTorrent(bytes);

            string info_hash = Utils.EscapeByteForURL(metaInfo.infoHash);
            //generate a new guid as peer id
            string peer_id = "01234567890123456789";// Guid.NewGuid().ToString().Substring(0, 20);
            int port = 6883;
            int uploaded = 0;
            int downloaded = 0;
            int left = metaInfo.info.totalLength;
            int compact = 1;
            string evnt = "started";
            int numwant = 50;

            string url = metaInfo.announce + "?" + "info_hash=" + info_hash + "&peer_id=" + peer_id + "&port=" + port + "&uploaded=" + uploaded + "&downloaded=" + downloaded
                + "&left=" + left + "&compact=" + compact + "&event=" + evnt + "&numwant=" + numwant;

            System.Diagnostics.Debug.WriteLine(url);

            Assert.IsTrue(url.Length > 0);

        }
    }
}
