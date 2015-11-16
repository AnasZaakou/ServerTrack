using System;
using System.Net.Http;
using System.Web.Http;
using ServerTrack.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServerTrackTest
{
    [TestClass]
    public class LoadRepositoryTest
    {
        static readonly ILoadRecordRepository loadRecordRepository = new LoadRecordRepository();
        [TestMethod]
        public void testAddLoadRecordSampleRecord()
        {
            Random random = new Random();
            string serverName = "server_1";
            double cpuLoad = random.NextDouble();
            double ramLoad = random.NextDouble();
            LoadRecord loadRecord = loadRecordRepository.addLoadRecord(serverName, cpuLoad, ramLoad, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            Assert.Equals(cpuLoad, loadRecord.cpuLoad);
            Assert.Equals(ramLoad, loadRecord.ramLoad);
        }
    }
}
