using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibraries.Models
{
    public class RequestLicense
    {
        public string Email { get; set; }
        public string DeviceName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string CreditName { get; set; }
        public string SecurityCode { get; set; }

        public DeviceInfo[] DeviceInformation { get; set; }

        public ArkInfo ArkInformation { get; set; }

        public string ValidationErrors { get; set; }

        public string Results { get; set; }

    }
}
