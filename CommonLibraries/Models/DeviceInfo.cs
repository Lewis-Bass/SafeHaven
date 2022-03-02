using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibraries.Models
{
    public class DeviceInfo
    {
        /// <summary>
        /// Type of the stat - example Disk
        /// </summary>
        public string StatisticType { get; set; }
        /// <summary>
        /// Category of the stat - C: or the name of the disk
        /// </summary>
        public string StatisticCategory { get; set; }
        /// <summary>
        /// Name of the stat - Available Disk Space
        /// </summary>
        public string StatisticName { get; set; }
        /// <summary>
        /// The statistic value
        /// </summary>
        public string StatisticValue { get; set; }
       
       
        public DeviceInfo(string type, string category, string name, string value)
        {
            StatisticType = type;
            StatisticCategory = category;
            StatisticName = name;
            StatisticValue = value;
        }
    }
}
