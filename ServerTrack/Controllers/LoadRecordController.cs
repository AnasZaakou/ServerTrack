using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ServerTrack.Models;

namespace ServerTrack.Controllers
{
    public class LoadRecordController : ApiController
    {
        // the LoadRecordRepository to manipulate the LoadRecord model
        static readonly ILoadRecordRepository loadRecordRepository = new LoadRecordRepository();

        /// <summary>
        /// RecordLoad provides the addition of new LoadRecord to the existing underlaying dictionary of <serverName, ConcurrentQueue<LoadRecord>>
        /// Parameters: serverName, cpuLoad, ramLoad
        /// Returns: LoadRecord
        /// </summary>
        public IHttpActionResult RecordLoad(string serverName, double cpuLoad, double ramLoad)
        {
            LoadRecord loadRecord = loadRecordRepository.addLoadRecord(serverName, cpuLoad, ramLoad, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            return Ok(loadRecord);
        }

        /// <summary>
        /// RecordLoad returns a list of average-LoadRecords for the last 24 hours, hour-by-hour and minute-by-minute
        /// Parameters: serverName, cpuLoad, ramLoad
        /// Returns: Json representation of the list of average-LoadRecords
        /// </summary>
        public IHttpActionResult DisplayLoad(string serverName)
        {
            return Json(loadRecordRepository.getAverageLoadRecords(serverName, DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond));
        }
    }
}
