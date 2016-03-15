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
    /// Summary description for UtilsTest
    /// </summary>
    [TestClass]
    public class UtilsTest
    {
        public UtilsTest()
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
        public void TestByteEncoder()
        {
            byte[] bytes = { 0x12, 0x34, 0x56, 0x78, 0x9a, 0xbc, 0xde, 0xf1, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef, 0x12, 0x34, 0x56, 0x78, 0x9a };
            string result = "%124Vx%9A%BC%DE%F1%23Eg%89%AB%CD%EF%124Vx%9A";

            Assert.IsTrue(Utils.EscapeByteForURL(bytes).CompareTo(result) == 0);
        }

        [TestMethod]
        public void TestEndian()
        {
            IPAddress addr = IPAddress.Parse("192.168.0.1");
            System.Diagnostics.Debug.WriteLine(addr.ToString());

            System.Diagnostics.Debug.WriteLine(BitConverter.IsLittleEndian);
        }
    }
}
