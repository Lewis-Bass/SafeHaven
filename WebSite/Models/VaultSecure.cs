using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;
using WebDataLayer.DBMS;

namespace WebSite.Models
{

    [PageType(Title = "Vault Secure")]
    [PageTypeRoute(Title = "Vault Secure", Route = "/vaultsecure")]

    public class VaultSecure : Page<CustomerSecure>
    {
        #region Piranha page properties
        public Guid PageId { get; set; }

        [Region]
        public Regions.Hero Hero { get; set; }

        #endregion

        //public License LicenseInfo { get; set; }
        public string LicenseName { get; set; }
        public string LicenseEmail { get; set; }
        public string LicensePhoneNumber { get; set; }
        public int LicenseMaxDevicesSupported { get; set; }
        public Device[] DeviceInfo { get; set; }
        
    }
}
