using System;
using System.Collections.Generic;
using System.Text;

namespace WebDataLayer.Models
{
    public class InfoDetails { 
        public List<InfoDevice> Devices { get; set; }
    }

    public class InfoDevice {
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public List<InfoTypes> Types { get; set; }
    }
    public class InfoTypes
    {
        public string InformationType { get; set; }
        public List<InfoDetail> Details { get; set; }
    }

    public class InfoDetail
    {
        public string InformationCategory { get; set; }
        public string InformationName { get; set; }
        public string InformationValue { get; set; }
        public string InformatiionDisplay
        {
            get
            {
                return InformationCategory + (string.IsNullOrWhiteSpace(InformationName) ? string.Empty : $" - {InformationName}");
            }
        }
    }
}
