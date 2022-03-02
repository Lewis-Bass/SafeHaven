using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace WebSite.Models
{


    [PageType(Title = "Purchase Secure")]
    [PageTypeRoute(Title = "Purchase Secure", Route = "/purchasesecure")]

    public class PurchaseSecure : Page<PurchaseSecure>
    {

        #region Piranha page properties

        [Region]
        public Regions.Hero Hero { get; set; }

        #endregion

        #region Field Information

        public string LicenseId { get; set; }
        public string ProductName { get; set; }
        public DateTime ProductExpirationDate { get; set; }
        public string ProductExpirationDateString
        {
            get
            {
                return ProductExpirationDate.ToShortDateString();
            }
        }
        public string CreditCard { get; set; }
        public string SecurityCode { get; set; }

        #endregion

    }
}
