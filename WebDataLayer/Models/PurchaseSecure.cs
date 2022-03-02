using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebDataLayer.Models
{

    public class PurchaseSecure 
    {

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

        #region messages

        public string ErrorMessage { get; set; }

        public string SuccessMessage { get; set; }

        #endregion
    }
}
