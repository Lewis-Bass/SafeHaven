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
    [PageType(Title = "Customer secure")]
    [PageTypeRoute(Title = "Customer Portal", Route = "/customersecure")]
    public class CustomerSecure : Page<CustomerSecure>
    {
      
        #region Form properties

        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string ValidationErrors { get; set; }


        #endregion
    }
}
