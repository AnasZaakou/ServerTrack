using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerTrack.Models
{
    /// <summary>
    /// LoadRecord is a class that used to maintain a single record for each load entry
    /// Properties: cpuLoad, ramLoad, time: time represents in long to hold time in milliseconds
    /// </summary>
    public class LoadRecord
    {
        public double cpuLoad { get; set; }
        public double ramLoad { get; set; }
        public long time { get; set; }
        /// <summary>
        /// Enumueration to indicate the criterea of time whether Hour or Minutes. This enum is extensible to other changes
        /// </summary>
        public enum TimeDuration { Hour, Minute };

        /// <summary>
        /// LoadRecord constructor to create new instance of the LoadRecord class and set the instance's properties
        /// Parameters: cpuLoad, ramLoad, time
        /// </summary>
        public LoadRecord(double cpuLoad, double ramLoad, long time)
        {
            this.cpuLoad = cpuLoad;
            this.ramLoad = ramLoad;
            this.time = time;
        }

        /// <summary>
        /// Returns whether the time of this LoadRecord falling between start and end range
        /// </summary>
        public bool isInRange(long start, long end)
        {
            return time > start && time <= end;
        }
    }
}