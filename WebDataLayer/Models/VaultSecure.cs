using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WebDataLayer.DBMS;

namespace WebDataLayer.Models
{

    public class VaultSecure 
    {
        
        //public License LicenseInfo { get; set; }
        public string LicenseName { get; set; }
        public string LicenseEmail { get; set; }
        public string LicensePhoneNumber { get; set; }
        public int LicenseMaxDevicesSupported { get; set; }
        public Device[] DeviceInfo { get; set; }
        
    }
}
