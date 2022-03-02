using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonLibraries.Extensions;

namespace WebLibraries.CardProcessing
{
    public class PurchaseLicense
    {

        public bool Purchase(string licenseId, string creditCard, string securityCode, decimal amount)
        {
            // TODO: ADD CREDIT CARD PROCESSING HERE

            // update the license
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.LicenseId == licenseId);
                if (license == null)
                {
                    return false;
                }
                //license.ExpirationDate = DateTime.MaxValue.ToNodaDate();
                //license.LicensePaidDate = DateTime.Today.ToNodaDate();
                license.ExpirationDate = DateTime.MaxValue;
                license.LicensePaidDate = DateTime.Today;
                dbms.SaveChanges();
            }

            return true;
        }

    }
}
