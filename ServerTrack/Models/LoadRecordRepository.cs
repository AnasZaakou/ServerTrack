using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerTrack.Models
{
    /// <summary>
    /// LoadRecordRepository provides the implementation for the ILoadRecordRepository to leverage the LoadRecord model
    /// </summary>
    public class LoadRecordRepository : ILoadRecordRepository
    {
        /// <summary>
        /// variables to provide helper information according to the requirements
        /// DayHours: hours in a day
        /// HourMinutes: minutes in an hour
        /// HourMilliseconds: milliseconds in an hour
        /// MinuteMilliseconds: milliseconds in a minutes
        /// </summary>
        public const int DayHours = 24;
        public const int HourMinutes = 60;
        public const long HourMilliseconds = 1000 * 60 * 60;
        public const long MinuteMilliseconds = 1000 * 60;
        /// <summary>
        /// ServersLoadRecords provides the mapping of serverNames and related LoadRecords
        /// ConcurrentDictionary is used to support concurrency to handle high number of requests
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentQueue<LoadRecord>> ServersLoadRecords = new ConcurrentDictionary<string, ConcurrentQueue<LoadRecord>>();

        /// <summary>
        /// addLoadRecord method is used to add a new LoadRecord to the existing LoadRecord lists
        /// Parameters: serverName, cpuLoad, ramLoad, time: represents time now in milliseconds
        /// Returns: LoadRecord
        /// </summary>
        public LoadRecord addLoadRecord(string serverName, double cpuLoad, double ramLoad, long time)
        {
            // A new LoadRecord object
            ConcurrentQueue<LoadRecord> serverLoadRecords;
            // Check if the serverName is existing
            // if Yes => get all it's LoadRecords in a queue fashion and then add a new LoadRecord object
            //      Why Queue: to maintain just 24 hours records in LIFO manner
            // if No => create new Queue of LoadRecords, add the queue to the ServersLoadRecords dictionary
            ServersLoadRecords.TryGetValue(serverName, out serverLoadRecords);
            if (serverLoadRecords == null)
            {
                serverLoadRecords = new ConcurrentQueue<LoadRecord>();
                ServersLoadRecords.TryAdd(serverName, serverLoadRecords);
            }

            // inistantiate the new LoadRecord object
            LoadRecord loadRecord = new LoadRecord(cpuLoad, ramLoad, time);
            // Add the new LoadRecord to the LoadRecords's queue
            serverLoadRecords.Enqueue(loadRecord);

            // After adding the new LoadRecord to the LoadRecords's queue, check if the queue has LoadRecords that's before 24 hours from 'time' variable
            // and remove those LoadRecords.
            // The condition checks with accuracy of milliseconds
            long todayStartTime = time - DayHours * HourMilliseconds;
            LoadRecord existingLoadRecord;
            while (serverLoadRecords.TryPeek(out existingLoadRecord))
            {
                if (existingLoadRecord.time < todayStartTime)
                    serverLoadRecords.TryDequeue(out existingLoadRecord);
            }

            return loadRecord;
        }

        /// <summary>
        /// getAveragePerDuration method is used to add a new LoadRecord to the existing LoadRecord lists
        /// Parameters: serverName, cpuLoad, ramLoad, time: represents time now in milliseconds
        /// Returns: List<LoadRecord>
        /// </summary>
        public List<LoadRecord> getAveragePerDuration(long endTime, ConcurrentQueue<LoadRecord> loadRecords, LoadRecord.TimeDuration timeDuration)
        {
            // A new list of LoadRecord
            List<LoadRecord> averageLoadRecords = new List<LoadRecord>();
            // set the time criterea whether hour = 24 or minutes = 60
            int durationCount = timeDuration == LoadRecord.TimeDuration.Hour ? DayHours : HourMinutes;
            // a temporary variable to hold the time now
            long tempEndTime = endTime;
            // get the average for each time-criterea/duration: 24 hours or by minute-by-minute = 60
            for (int i = 0; i < durationCount; i++)
            {
                // set end time for this time slot based on time-criterea/duration
                long itemEndTime = tempEndTime;
                // set the start time for this slot in milliseconds. 
                long itemStartTime = timeDuration == LoadRecord.TimeDuration.Hour ? itemEndTime - HourMilliseconds : itemEndTime - MinuteMilliseconds;
                // set avergae cpuLoad for all LoadRecords falling in this time slot
                double cpu = loadRecords.Where(x => x.isInRange(itemStartTime, itemEndTime)).Average(x => x.cpuLoad);
                // set avergae ramLoad for all LoadRecords falling in this time slot
                double ram = loadRecords.Where(x => x.isInRange(itemStartTime, itemEndTime)).Average(x => x.ramLoad);
                averageLoadRecords.Add(new LoadRecord(cpu, ram, tempEndTime));
                // move to the next time slot
                tempEndTime = itemStartTime;
            }
            // return a list of average LoadRecords for the specific time-criterea/duration 'TimeDuration'
            return averageLoadRecords;
        }

        /// <summary>
        /// getAverageLoadRecords method is used to return a list of LoadRecords for for a specific server. 
        /// It's an outer method to work with the getAveragePerDuration method
        /// Parameters: string serverName, long endTime
        /// Returns: List<LoadRecord>
        /// </summary>
        public List<LoadRecord> getAverageLoadRecords(string serverName, long nowTime)
        {
            // A new list of LoadRecord
            List<LoadRecord> averageLoadRecords;
            // Queue to hold a list of LoadRecord a specific serverName within 24 hours
            ConcurrentQueue<LoadRecord> serverLoadRecords;
            ServersLoadRecords.TryGetValue(serverName, out serverLoadRecords);
            if (serverLoadRecords.Count > 0)
            {
                // First get average LoadRecords for today on hourly basis 
                averageLoadRecords = getAveragePerDuration(nowTime, serverLoadRecords, LoadRecord.TimeDuration.Hour);
                // Second add to the first list, average LoadRecords for today minute-by-minute
                averageLoadRecords.AddRange(getAveragePerDuration(nowTime, serverLoadRecords, LoadRecord.TimeDuration.Minute));
                return averageLoadRecords;
            }
            else
                return null;
        }

    }
}