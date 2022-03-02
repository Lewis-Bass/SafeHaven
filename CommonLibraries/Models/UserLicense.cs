using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibraries.Models
{
    public class UserLicense
    {
        public string LicenseId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LicenseName { get; set; }
        public UserDevice[] Devices { get; set; }

    }
    public class UserDevice
    {
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DateTime DisabledDate { get; set; }
        public UserArk[] Arks {get;set;}
    }

    public class UserArk
    {
        public string ArkId { get; set; }
        public string ArkName { get; set; }
        public DateTime DisabledDate { get; set; }
    }
}
