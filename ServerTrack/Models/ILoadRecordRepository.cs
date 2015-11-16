using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTrack.Models
{
    /// <summary>
    /// ILoadRecordRepository is acting like an interface that leverages the LoadRecord model to provide 
    /// the basic function members to manipulate the model
    /// </summary>
    public interface ILoadRecordRepository
    {
        /// <summary>
        /// addLoadRecord method is used to add a new LoadRecord to the existing LoadRecord lists
        /// Parameters: serverName, cpuLoad, ramLoad, time
        /// Returns: LoadRecord
        /// </summary>
        LoadRecord addLoadRecord(string serverName, double cpuLoad, double ramLoad, long time);
        /// <summary>
        /// getAveragePerDuration method is used to a list of LoadRecords for for a specific server within a specific time duration
        /// Parameters: long endTime, ConcurrentQueue<LoadRecord> loadRecords, LoadRecord.TimeDuration timeDuration
        /// Returns: List<LoadRecord>
        /// </summary>
        List<LoadRecord>  getAveragePerDuration(long endTime, ConcurrentQueue<LoadRecord> loadRecords, LoadRecord.TimeDuration timeDuration);
        /// <summary>
        /// getAverageLoadRecords method is used to return a list of LoadRecords for for a specific server. 
        /// It's an outer method to work with the getAveragePerDuration method
        /// Parameters: string serverName, long endTime
        /// Returns: List<LoadRecord>
        /// </summary>
        List<LoadRecord> getAverageLoadRecords(string serverName, long endTime);
    }
}
