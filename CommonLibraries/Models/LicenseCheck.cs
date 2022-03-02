using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibraries.Models
{
    public class LicenseCheck
    {
        public Login UserInfo { get; set; }
        public string DeviceId { get; set; }
        public string ArkId { get; set; }
    }
}
