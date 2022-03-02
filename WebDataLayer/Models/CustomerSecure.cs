using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace WebDataLayer.Models
{
    public class CustomerSecure
    {
       
        public string Title { get; set; }
        #region Form properties

        //[DisplayName(WebResources.Properties.Resources.CustomerEmail)]
        //[DisplayName(WebDataLayer.Properties.Resources.String1)]
        [Required()]
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Password1 { get; set; }
        public string Password2 { get; set; }
        public string ValidationErrors { get; set; }


        #endregion
    }
}
